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
	[PageComponent(Label = "Button", Library = "WebVella", Description = "Renderes a button", Version = "0.0.1", IconClass = "far fa-caret-square-right", IsInline = true)]
	public class PcButton : PageComponent
	{
		protected ErpRequestContext ErpRequestContext { get; set; }

		public PcButton([FromServices]ErpRequestContext coreReqCtx)
		{
			ErpRequestContext = coreReqCtx;
		}

		public class PcButtonOptions
		{

			[JsonProperty(PropertyName = "type")]
			public ButtonType Type { get; set; } = ButtonType.Button;

			[JsonProperty(PropertyName = "is_outline")]
			public bool isOutline { get; set; } = false;

			[JsonProperty(PropertyName = "is_block")]
			public bool isBlock { get; set; } = false;

			[JsonProperty(PropertyName = "is_active")]
			public bool isActive { get; set; } = false;

			[JsonProperty(PropertyName = "is_disabled")]
			public bool isDisabled { get; set; } = false;

			[JsonProperty(PropertyName = "color")]
			public ErpColor Color { get; set; } = ErpColor.White;

			[JsonProperty(PropertyName = "size")]
			public CssSize Size { get; set; } = CssSize.Inherit;

			[JsonProperty(PropertyName = "class")]
			public string Class { get; set; } = "";

			[JsonProperty(PropertyName = "id")]
			public string Id { get; set; } = "";

			[JsonProperty(PropertyName = "text")]
			public string Text { get; set; } = "button";

			[JsonProperty(PropertyName = "onclick")]
			public string OnClick { get; set; } = "";

			[JsonProperty(PropertyName = "href")]
			public string Href { get; set; } = "";

			[JsonProperty(PropertyName = "new_tab")]
			public bool NewTab { get; set; } = false;

			[JsonProperty(PropertyName = "icon_class")]
			public string IconClass { get; set; } = "";

			[JsonProperty(PropertyName = "form")]
			public string Form { get; set; } = "";

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

				var instanceOptions = new PcButtonOptions();
				if (context.Options != null)
				{
					instanceOptions = JsonConvert.DeserializeObject<PcButtonOptions>(context.Options.ToString());
				}

				var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
				#endregion


				ViewBag.Options = instanceOptions;
				ViewBag.Node = context.Node;
				ViewBag.ComponentMeta = componentMeta;
				ViewBag.RequestContext = ErpRequestContext;
				ViewBag.AppContext = ErpAppContext.Current;
				ViewBag.ProcessedHref = context.DataModel.GetPropertyValueByDataSource(instanceOptions.Href);

				#region << Select options >>
				ViewBag.CssSize = ModelExtensions.GetEnumAsSelectOptions<CssSize>(); 

				ViewBag.ColorOptions = ModelExtensions.GetEnumAsSelectOptions<ErpColor>().OrderBy(x=> x.Label).ToList();

				ViewBag.TypeOptions = ModelExtensions.GetEnumAsSelectOptions<ButtonType>(); 

				#endregion

				switch (context.Mode)
				{
					case ComponentMode.Display:
						return await Task.FromResult<IViewComponentResult>(View("Display"));
					case ComponentMode.Design:
						return await Task.FromResult<IViewComponentResult>(View("Design"));
					case ComponentMode.Options:
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
