﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using WebVella.Erp.Hooks;
using WebVella.Erp.Web.Hooks;
using WebVella.Erp.Web.Models;

namespace WebVella.Erp.Web.Pages.Application
{
	[Authorize]
	public class RecordRelatedRecordDetailsPageModel : BaseErpPageModel
	{
		public RecordRelatedRecordDetailsPageModel([FromServices]ErpRequestContext reqCtx) { ErpRequestContext = reqCtx; }

		public IActionResult OnGet()
		{
			Init();
			if (ErpRequestContext.Page == null) return NotFound();
			if (!RecordsExists()) return NotFound();
			if (PageName != ErpRequestContext.Page.Name)
			{
				var queryString = HttpContext.Request.QueryString.ToString();
				return Redirect($"/{ErpRequestContext.App.Name}/{ErpRequestContext.SitemapArea.Name}/{ErpRequestContext.SitemapNode.Name}/r/{ErpRequestContext.ParentRecordId}/rl/{ErpRequestContext.RelationId}/r/{ErpRequestContext.RecordId}/{ErpRequestContext.Page.Name}{queryString}");
			}

			var globalHookInstances = HookManager.GetHookedInstances<IPageHook>(HookKey);
			foreach (IPageHook inst in globalHookInstances)
			{
				var result = inst.OnGet(this);
				if (result != null) return result;
			}

			BeforeRender();
			return Page();
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid) throw new Exception("Antiforgery check failed.");
			Init();
			if (ErpRequestContext.Page == null) return NotFound();
			if (!RecordsExists()) return NotFound();

			var globalHookInstances = HookManager.GetHookedInstances<IPageHook>(HookKey);
			foreach (IPageHook inst in globalHookInstances)
			{
				var result = inst.OnPost(this);
				if (result != null) return result;
			}

			var hookInstances = HookManager.GetHookedInstances<IRecordRelatedRecordDetailsPageHook>(HookKey);
			foreach (IRecordRelatedRecordDetailsPageHook inst in hookInstances)
			{
				var result = inst.OnPost(this);
				if (result != null) return result;
			}

			BeforeRender();
			return Page();
		}
	}
}