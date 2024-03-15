using Amazon.S3;
using Amazon.SimpleEmail;
using Fydar.Dev.Services.EmailTickets;
using Fydar.Dev.WebApp.Client.Components.Pages;
using Fydar.Dev.WebApp.Components;
using Fydar.Dev.WebApp.Components.Pages;
using Fydar.Dev.WebApp.Internal;
using Fydar.Dev.WebApp.Internal.AntiforgeryNoStoreWorkaround;
using Fydar.Dev.WebApp.Toolkit.Icons;
using LettuceEncrypt;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Events;
using System.Net;

namespace Fydar.Dev.WebApp;

public class Program
{
	public static async Task<int> Main(string[] args)
	{
		var loggerConfiguration = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.MinimumLevel.Override("Microsoft.AspNetCore.Server.Kestrel", LogEventLevel.Error)
			.Enrich.FromLogContext()
			.WriteTo.Sink(new ColoredConsoleLogEventSink());

		var logger = loggerConfiguration.CreateLogger();
		Log.Logger = logger;

		try
		{
			var host = CreateHost(args);
			host.Start();

			var server = host.Services.GetRequiredService<IServer>();
			var addresses = server.Features.GetRequiredFeature<IServerAddressesFeature>().Addresses;

			Log.Information($"Web host started listening on '{string.Join("', '", addresses)}'.");

			await host.WaitForShutdownAsync();

			return 0;
		}
		catch (Exception exception)
		{
			Log.Fatal(exception, "Host terminated unexpectedly.");
			return 1;
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}

	public static IHost CreateHost(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Host.UseSerilog();

		builder.WebHost.UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True");

		builder.Configuration.AddEnvironmentVariables("CONFIG_");

		// Add services to the container.
		builder.Services.AddHealthChecks();

		builder.Services.AddAntiforgery();
		builder.Services.RemoveAntiforgeryNoStore();

		builder.Services.AddResponseCompression(options =>
		{
			options.EnableForHttps = true;

			options.Providers.Add<BrotliCompressionProvider>();
			options.Providers.Add<GzipCompressionProvider>();
			options.MimeTypes = ResponseCompressionDefaults.MimeTypes
				.Concat(["image/svg+xml"]);
		});

		builder.Services.AddRazorComponents()
			.AddInteractiveWebAssemblyComponents();

		builder.Services.AddLettuceEncrypt();

		builder.Services.AddSingleton(new S3EmailReaderServiceConfiguration()
		{
			Bucket = "fydar.dev-inbound-email"
		});
		builder.Services.AddSingleton<IEmailReaderService, S3EmailReaderService>();
		builder.Services.AddScoped<HtmlRenderer>();

		builder.Services.AddScoped<IContactSubmitSink, SaveTicketSubmitSink>();
		builder.Services.AddScoped<IContactSubmitSink, ContactNotificationSubmitSink>();

		builder.Services.AddAWSService<IAmazonSimpleEmailService>();
		builder.Services.AddAWSService<IAmazonS3>();

		if (builder.WebHost.GetSetting("Environment") != "Development")
		{
			builder.Services.Configure<StaticFileOptions>(opts =>
			{
				opts.OnPrepareResponse = ctx =>
				{
					ctx.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age=31536000";
				};
			});

			string domainName = builder.Configuration.GetValue<string>("DOMAINNAME") ?? string.Empty;

			builder.Services.AddLettuceEncrypt(options =>
			{
				options.AcceptTermsOfService = true;
				options.DomainNames = [domainName];
				options.EmailAddress = "dev.anthonymarmont@gmail.com";
			})
				.PersistDataToDirectory(new DirectoryInfo("lettuce"), null);

			builder.WebHost.UseKestrel(kestrel =>
			{
				var appServices = kestrel.ApplicationServices;

				kestrel.Listen(IPAddress.Any, 8060);

				kestrel.Listen(
					IPAddress.Any, 8061,
					listen =>
					{
						listen.Protocols = HttpProtocols.Http1 | HttpProtocols.Http2 | HttpProtocols.Http3;

						listen.UseHttps(adapter =>
						{
							adapter.UseLettuceEncrypt(appServices);
						});
					});
			});
		}

		var app = builder.Build();

		app.UseHealthChecks("/api/health");

		app.Use(async (context, next) =>
		{
			if (context.Request.Path.Equals("/favicon.ico")
				&& context.Request.Headers.Accept.Any(a => a?.Contains("image/svg+xml", StringComparison.OrdinalIgnoreCase) ?? false))
			{
				context.Request.Path = "/favicon.svg";
			}
			await next.Invoke();
		});

		if (Environment.GetEnvironmentVariable("__ASPNETCORE_BROWSER_TOOLS") is null)
		{
			app.UseResponseCompression();
		}

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseHttpsRedirection();
			app.UseHsts();
			app.UseExceptionHandler("/error");
		}

		app.UseStatusCodePagesWithReExecute("/error/{0}");

		app.MapIconLibrary<SiteIcons>("/icons.svg");

		var provider = new FileExtensionContentTypeProvider();
		provider.Mappings.Remove(".br");
		provider.Mappings[".br"] = "application/octet-stream";
		provider.Mappings[".data"] = "application/octet-stream";

		app.UseStaticFiles(new StaticFileOptions
		{
			ContentTypeProvider = provider,
			OnPrepareResponse = context =>
			{
				var headers = context.Context.Response.Headers;
				if (context.File.Name.EndsWith(".wasm.br", StringComparison.OrdinalIgnoreCase))
				{
					headers.ContentEncoding = "br";
					headers.ContentType = "application/wasm";
				}
				else if (context.File.Name.EndsWith(".data.br", StringComparison.OrdinalIgnoreCase))
				{
					headers.ContentEncoding = "br";
					headers.ContentType = "application/octet-stream";
				}
				else if (context.File.Name.EndsWith(".js.br", StringComparison.OrdinalIgnoreCase))
				{
					headers.ContentEncoding = "br";
					headers.ContentType = "application/javascript";
				}
				else if (context.File.Name.EndsWith(".data", StringComparison.OrdinalIgnoreCase))
				{
					headers.ContentType = "application/octet-stream";
				}
			}
		});

		app.UseStaticFiles();

		app.UseAntiforgery();

		app.MapRazorComponents<App>()
			.AddInteractiveWebAssemblyRenderMode()
			.AddAdditionalAssemblies(
				typeof(Counter).Assembly,
				typeof(Icon).Assembly);

		return app;
	}
}
