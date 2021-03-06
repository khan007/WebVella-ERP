﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Eql;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Models;

namespace WebVella.Erp.Plugins.SDK.Pages.Role
{
	public class ManageModel : BaseErpPageModel
	{
		public ManageModel([FromServices]ErpRequestContext reqCtx) { ErpRequestContext = reqCtx; }

		[BindProperty]
		public string Name { get; set; } = "";

		[BindProperty]
		public string Description { get; set; } = "";

		private EntityRecord RoleRecord { get; set; } = null;

		private void InitPage()
		{
			Init();

			if (RecordId != null)
			{
				var resultRecords = new EqlCommand($"select * from role where id = '{RecordId}'").Execute();
				if (resultRecords.Any()) RoleRecord = resultRecords.First();
			}

			if (string.IsNullOrWhiteSpace(ReturnUrl)) ReturnUrl = "/sdk/access/role/l/list";
		}

		public IActionResult OnGet()
		{
			InitPage();

			if (RoleRecord == null)
				return NotFound();

			Name = (string)RoleRecord["name"];
			Description = (string)RoleRecord["description"];

			return Page();
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid) throw new Exception("Antiforgery check failed.");
			InitPage();
			if (RoleRecord == null) return NotFound();

			try
			{
				ErpRole role = new ErpRole();
				role.Id = (Guid)RoleRecord["id"];
				role.Name = Name;
				role.Description = Description;
				new SecurityManager().SaveRole(role);

				return Redirect(ReturnUrl);
			}
			catch (ValidationException ex)
			{
				Validation.Message = ex.Message;
				Validation.Errors = ex.Errors;
				return Page();
			}
		}
	}
}