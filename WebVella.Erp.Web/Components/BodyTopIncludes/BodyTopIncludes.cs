﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVella.Erp.Web.Hooks;
using WebVella.Erp.Web.Models;

namespace WebVella.Erp.Web.Components
{

	[RenderHookAttachment("body-top", 1)]
	public class BodyTopIncludes : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(BaseErpPageModel pageModel)
        {
			ViewBag.ScriptTags = new List<ScriptTagInclude>();

			#region === <script> ===
			{
				var includedScriptTags = pageModel.HttpContext.Items.ContainsKey(typeof(List<ScriptTagInclude>)) ? (List<ScriptTagInclude>)pageModel.HttpContext.Items[typeof(List<ScriptTagInclude>)] : new List<ScriptTagInclude>();
				var scriptTagsToInclude = new List<ScriptTagInclude>();

				//Your includes below >>>>

				#region << bootstrap >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/bootstrap")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/twitter-bootstrap/js/bootstrap.bundle.min.js"
						});
					}
				}
				#endregion

				#region << uri.js >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/uri")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/URI.js/URI.min.js"
						});
					}
				}
				#endregion

				#region << moment >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/moment")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/moment.js/moment.min.js"
						});
					}
				}
				#endregion

				#region << ckeditor >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/ckeditor")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/ckeditor/ckeditor.js"
						});
					}
				}
				#endregion

				#region << lodash >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/lodash")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/lodash.js/lodash.min.js"
						});
					}
				}
				#endregion

				#region << flatpickr >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/flatpickr")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/flatpickr/flatpickr.min.js"
						});
					}
				}
				#endregion

				#region << select2 >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/select2")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/select2/js/select2.min.js"
						});
					}
				}
				#endregion

				#region << js-cookie >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/js-cookie")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/js-cookie/js.cookie.min.js"
						});
					}
				}
				#endregion

				#region << decimal >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/decimal")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/decimal.js/decimal.min.js"
						});
					}
				}
				#endregion

				#region << toastr >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/toastr")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/toastr.js/toastr.min.js"
						});
					}
				}
				#endregion

				#region << colorpicker >>
				{
					if (!includedScriptTags.Any(x => x.Src.Contains("/colorpicker")))
					{
						scriptTagsToInclude.Add(new ScriptTagInclude()
						{
							Src = "/lib/spectrum/spectrum.min.js"
						});
					}
				}
				#endregion

				#region << wv-lazyload >>
				{
					//Always add
					scriptTagsToInclude.Add(new ScriptTagInclude()
					{
						Src = "/js/wv-lazyload/wv-lazyload.js"
					});
				}
				#endregion

				//<<<< Your includes up

				includedScriptTags.AddRange(scriptTagsToInclude);
				pageModel.HttpContext.Items[typeof(List<ScriptTagInclude>)] = includedScriptTags;
				ViewBag.ScriptTags = scriptTagsToInclude;
			}
			#endregion

			return await Task.FromResult<IViewComponentResult>(View("Default"));
        }
    }
}
