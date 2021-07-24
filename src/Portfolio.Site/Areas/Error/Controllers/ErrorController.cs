﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Site.Areas.Error.Models;

namespace Portfolio.Site.Areas.Error.Controllers
{
	[ApiController]
	[Area("Error")]
	[Route("error/{code:int}")]
	[ApiExplorerSettings(GroupName = "Error")]
	public class ErrorController : Controller
	{
		public ErrorController()
		{
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IActionResult Index(int code)
		{
			return View("ServerError", new ErrorViewModel()
			{

			});
		}
	}
}
