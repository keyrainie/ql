using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.M.Models.Search;

namespace Nesoft.ECWeb.M.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        [ValidateInput(false)]
        public ActionResult Index()
        {
            ProductSearchResultVM pageVM = SearchManager.Search(new SearchCriteriaModel() { PageSize = 10 }, HttpContext.Request.QueryString);
            ViewBag.Title = HttpContext.Request.QueryString["keyword"];
            return View(pageVM);
        }

        [ValidateInput(false)]
        public ActionResult AjaxSearchProducts()
        {
            ProductSearchResultVM pageVM = SearchManager.Search(new SearchCriteriaModel() { PageSize = 10 }, HttpContext.Request.QueryString);
            ViewBag.Title = HttpContext.Request.QueryString["keyword"];

            return PartialView("~/Views/UserControl/_SearchProductList.cshtml", pageVM);
        }
    }
}
