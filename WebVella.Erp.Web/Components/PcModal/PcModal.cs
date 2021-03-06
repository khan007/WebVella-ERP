﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.Erp.Web.Utils;

namespace WebVella.Erp.Web.Components
{
	[PageComponent(Label = "Modal", Library = "WebVella", Description = "A modal", Version = "0.0.1", IconClass = "far fa-window-maximize")]
	public class PcModal : PageComponent
	{
		protected ErpRequestContext ErpRequestContext { get; set; }

		public PcModal([FromServices]ErpRequestContext coreReqCtx)
		{
			ErpRequestContext = coreReqCtx;
		}

		public class PcModalOptions
		{
			[JsonProperty(PropertyName = "title")]
			public string Title { get; set; } = "";

			[JsonProperty(PropertyName = "position")]
			public ModalPosition Position { get; set; } = ModalPosition.Top;

			[JsonProperty(PropertyName = "size")]
			public ModalSize Size { get; set; } = ModalSize.Normal;

			[JsonProperty(PropertyName = "backdrop")]
			public string Backdrop { get; set; } = "true";

		}

		public async Task<IViewComponentResult> InvokeAsync(PageComponentContext context)
		{
			ErpPage currentPage = null;
			try
			{
				#region << Init >>
				if (context.Node == null)
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: The node Id is required to be set as query param 'nid', when requesting this component"));
				}

				var pageFromModel = context.DataModel.GetProperty("Page");
				if (pageFromModel is ErpPage)
				{
					currentPage = (ErpPage)pageFromModel;
				}
				else
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: PageModel does not have Page property or it is not from ErpPage Type"));
				}

				if (currentPage == null)
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: The page Id is required to be set as query param 'pid', when requesting this component"));
				}

				var instanceOptions = new PcModalOptions();
				if (context.Options != null)
				{
					instanceOptions = JsonConvert.DeserializeObject<PcModalOptions>(context.Options.ToString());
				}

				var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
				#endregion


				ViewBag.Options = instanceOptions;
				ViewBag.Node = context.Node;
				ViewBag.ComponentMeta = componentMeta;
				ViewBag.RequestContext = ErpRequestContext;
				ViewBag.AppContext = ErpAppContext.Current;
				ViewBag.ComponentContext = context;
				ViewBag.GeneralHelpSection = HelpJsApiGeneralSection;

				if (context.Mode == ComponentMode.Display)
				{
					
				}

				switch (context.Mode)
				{
					case ComponentMode.Display:
						ViewBag.ProcessedTitle = context.DataModel.GetPropertyValueByDataSource(instanceOptions.Title);
						return await Task.FromResult<IViewComponentResult>(View("Display"));
					case ComponentMode.Design:
						return await Task.FromResult<IViewComponentResult>(View("Design"));
					case ComponentMode.Options:
						ViewBag.PositionOptions = ModelExtensions.GetEnumAsSelectOptions<ModalPosition>();
						ViewBag.SizeOptions = ModelExtensions.GetEnumAsSelectOptions<ModalSize>();
						return await Task.FromResult<IViewComponentResult>(View("Options"));
					case ComponentMode.Help:
						return await Task.FromResult<IViewComponentResult>(View("Help"));
					default:
						ViewBag.ExceptionMessage = "Unknown component mode";
						ViewBag.Errors = new List<ValidationError>();
						return await Task.FromResult<IViewComponentResult>(View("Error"));
				}

			}
			catch (ValidationException ex)
			{
				ViewBag.ExceptionMessage = ex.Message;
				ViewBag.Errors = new List<ValidationError>();
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
			catch (Exception ex)
			{
				ViewBag.ExceptionMessage = ex.Message;
				ViewBag.Errors = new List<ValidationError>();
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
		}
	}
}
