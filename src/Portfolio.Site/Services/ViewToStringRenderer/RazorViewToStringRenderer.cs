﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Site.Services.ViewToStringRenderer
{
	public sealed class RazorViewToStringRenderer : IViewToStringRenderer
	{
		private readonly IRazorViewEngine viewEngine;
		private readonly ITempDataProvider tempDataProvider;
		private readonly IServiceProvider serviceProvider;

		public RazorViewToStringRenderer(
			IRazorViewEngine viewEngine,
			ITempDataProvider tempDataProvider,
			IServiceProvider serviceProvider)
		{
			this.viewEngine = viewEngine;
			this.tempDataProvider = tempDataProvider;
			this.serviceProvider = serviceProvider;
		}

		public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
		{
			var actionContext = GetActionContext();
			var view = FindView(actionContext, viewName);

			using var output = new StringWriter();

			var viewContext = new ViewContext(
				actionContext,
				view,
				new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
				{
					Model = model
				},
				new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
				output,
				new HtmlHelperOptions()
			);

			await view.RenderAsync(viewContext);

			return output.ToString();
		}

		private IView FindView(ActionContext actionContext, string viewName)
		{
			var getViewResult = viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
			if (getViewResult.Success)
			{
				return getViewResult.View;
			}

			var findViewResult = viewEngine.FindView(actionContext, viewName, isMainPage: true);
			if (findViewResult.Success)
			{
				return findViewResult.View;
			}

			var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
			string errorMessage = string.Join(
				Environment.NewLine,
				new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

			throw new InvalidOperationException(errorMessage);
		}

		private ActionContext GetActionContext()
		{
			var httpContext = new DefaultHttpContext
			{
				RequestServices = serviceProvider
			};
			return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
		}
	}
}
