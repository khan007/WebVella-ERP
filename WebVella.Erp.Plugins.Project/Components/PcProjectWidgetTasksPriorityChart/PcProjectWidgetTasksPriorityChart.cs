﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Plugins.Project.Model;
using WebVella.Erp.Plugins.Project.Services;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;

namespace WebVella.Erp.Plugins.Project.Components
{
	[PageComponent(Label = "Project Widget Priority Chart", Library = "WebVella", Description = "Chart presenting the current project tasks by priority", Version = "0.0.1", IconClass = "fas fa-chart-pie")]
	public class PcProjectWidgetTasksPriorityChart : PageComponent
	{
		protected ErpRequestContext ErpRequestContext { get; set; }

		public PcProjectWidgetTasksPriorityChart([FromServices]ErpRequestContext coreReqCtx)
		{
			ErpRequestContext = coreReqCtx;
		}

		public class PcProjectWidgetTasksPriorityChartOptions
		{

			[JsonProperty(PropertyName = "project_id")]
			public string ProjectId { get; set; } = null;

			[JsonProperty(PropertyName = "user_id")]
			public string UserId { get; set; } = null;
		}

		public async Task<IViewComponentResult> InvokeAsync(PageComponentContext context)
		{
			ErpPage currentPage = null;
			try
			{
				#region << Init >>
				if (context.Node == null)
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: The node Id is required to be set as query parameter 'nid', when requesting this component"));
				}

				var pageFromModel = context.DataModel.GetProperty("Page");
				if (pageFromModel == null)
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: PageModel cannot be null"));
				}
				else if (pageFromModel is ErpPage)
				{
					currentPage = (ErpPage)pageFromModel;
				}
				else
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: PageModel does not have Page property or it is not from ErpPage Type"));
				}

				var options = new PcProjectWidgetTasksPriorityChartOptions();
				if (context.Options != null)
				{
					options = JsonConvert.DeserializeObject<PcProjectWidgetTasksPriorityChartOptions>(context.Options.ToString());
				}

				var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
				#endregion


				ViewBag.Options = options;
				ViewBag.Node = context.Node;
				ViewBag.ComponentMeta = componentMeta;
				ViewBag.RequestContext = ErpRequestContext;
				ViewBag.AppContext = ErpAppContext.Current;
				ViewBag.ComponentContext = context;

				if (context.Mode != ComponentMode.Options && context.Mode != ComponentMode.Help)
				{

					Guid? projectId = context.DataModel.GetPropertyValueByDataSource(options.ProjectId) as Guid?;
					Guid? userId = context.DataModel.GetPropertyValueByDataSource(options.UserId) as Guid?;


					var projectTasks = new TaskService().GetTaskQueue(projectId, userId, TasksDueType.StartDateDue);
					int lowPriority = 0;
					int normalPriority = 0;
					int highPriority = 0;

					foreach (var task in projectTasks)
					{
						var taskPriority = (string)task["priority"];
						switch (taskPriority) {
							case "1":
								lowPriority++;
								break;
							case "2":
								normalPriority++;
								break;
							case "3":
								highPriority++;
								break;
							default:
								throw new Exception("Unknown task priority: " + taskPriority);
						}
					}


					var theme = new Theme();
					var chartDatasets = new List<ErpChartDataset>() {
						new ErpChartDataset(){
							Data = new List<decimal>(){ highPriority, normalPriority, lowPriority },
							BackgroundColor = new List<string>{ theme.RedColor, theme.LightBlueColor, theme.GreenColor},
							BorderColor = new List<string>{ theme.RedColor, theme.LightBlueColor, theme.GreenColor }
						}
					};

					ViewBag.LowPriority = lowPriority;
					ViewBag.NormalPriority = normalPriority;
					ViewBag.HighPriority = highPriority;
					ViewBag.PriorityOptions = ((SelectField)new EntityManager().ReadEntity("task").Object.Fields.First(x => x.Name == "priority")).Options;
					ViewBag.Datasets = chartDatasets;
				}
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
						return await Task.FromResult<IViewComponentResult>(View("Error"));
				}
			}
			catch (ValidationException ex)
			{
				ViewBag.ExceptionMessage = ex.Message;
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
			catch (Exception ex)
			{
				ViewBag.ExceptionMessage = ex.Message;
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
		}
	}
}
