﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Plugins.Project.Model;
using WebVella.Erp.Plugins.Project.Services;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;

namespace WebVella.Erp.Plugins.Project.Components
{
	[PageComponent(Label = "Project Widget Tasks Chart", Library = "WebVella", Description = "Chart presenting the current project tasks", Version = "0.0.1", IconClass = "fas fa-chart-pie")]
	public class PcProjectWidgetTasksChart : PageComponent
	{
		protected ErpRequestContext ErpRequestContext { get; set; }

		public PcProjectWidgetTasksChart([FromServices]ErpRequestContext coreReqCtx)
		{
			ErpRequestContext = coreReqCtx;
		}

		public class PcProjectWidgetTasksChartOptions
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

				var options = new PcProjectWidgetTasksChartOptions();
				if (context.Options != null)
				{
					options = JsonConvert.DeserializeObject<PcProjectWidgetTasksChartOptions>(context.Options.ToString());
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

					var overdueTasks = (int)0;
					var dueTodayTasks = (int)0;
					var notDueTasks = (int)0;

					foreach (var task in projectTasks)
					{
						var targetDate = ((DateTime?)task["target_date"]).ConvertToAppDate();

						if (targetDate != null && (targetDate ?? DateTime.Now).Date < DateTime.Now.Date)
							overdueTasks++;
						else if (targetDate != null && (targetDate ?? DateTime.Now).Date == DateTime.Now.Date)
							dueTodayTasks++;
						else
							notDueTasks++;
					}


					var theme = new Theme();
					var chartDatasets = new List<ErpChartDataset>() {
						new ErpChartDataset(){
							Data = new List<decimal>(){ overdueTasks, dueTodayTasks, notDueTasks },
							BackgroundColor = new List<string>{ theme.RedColor, theme.OrangeColor, theme.GreenColor},
							BorderColor = new List<string>{ theme.RedColor, theme.OrangeColor, theme.GreenColor }
						}
					};

					ViewBag.Datasets = chartDatasets;
					ViewBag.OverdueTasks = overdueTasks;
					ViewBag.DueTodayTasks = dueTodayTasks;
					ViewBag.NotDueTasks = notDueTasks;
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
