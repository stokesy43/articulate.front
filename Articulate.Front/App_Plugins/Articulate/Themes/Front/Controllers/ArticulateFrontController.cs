using Articulate.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Articulate.Themes.Front.Controllers
{
    [PluginController("Articulate")]
    public class ArticulateFrontController : SurfaceController
    {
        [HttpGet]
        public ActionResult GetPosts(int id, int page)
        {           

            var content = Umbraco.TypedContent(id);

            var master = new MasterModel(content);

            var count = Umbraco.GetPostCount(id);

            var posts = Umbraco.GetRecentPosts(master, page, master.PageSize);

            var viewPath = PathHelper.GetThemePartialViewPath(master, "ListPost");

            var html = string.Empty;

            foreach(var post in posts)
            {
                html += RenderViewToString(this, viewPath, post, true);
            }

            return Content(html);
        }

        /// <summary>
		/// Renders the partial view to string.
		/// </summary>
		/// <param name="controller">The controller context.</param>
		/// <param name="viewName">Name of the view.</param>
		/// <param name="model">The model.</param>
		/// <param name="isPartial">true if it is a Partial view, otherwise false for a normal view </param>
		/// <returns></returns>
		internal static string RenderViewToString(ControllerBase controller, string viewName, object model, bool isPartial = false)
        {
            if (controller.ControllerContext == null)
                throw new ArgumentException("The controller must have an assigned ControllerContext to execute this method.");

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = !isPartial
                    ? ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null)
                    : ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}