using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity.Category;
using Nesoft.ECWeb.Entity.SearchEngine;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Facade.Product.Models;
using Nesoft.ECWeb.Facade.Recommend;
using Nesoft.ECWeb.M.Models;
using Nesoft.ECWeb.M.Models.Search;
using Nesoft.Utility;

namespace Nesoft.ECWeb.M.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/


        /// <summary>
        /// 三级分类-所有分类
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoryAll()
        {
            return View();
        }

        /// <summary>
        /// 三级分类-详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Category3(int ID)
        {
            SearchCriteriaModel criteria = new SearchCriteriaModel()
            {
                Category3ID = ID,
                PageSize = 10
            };
            ProductSearchResultVM pageVM = SearchManager.Search(criteria, HttpContext.Request.QueryString);

            var allCategories = CategoryFacade.QueryCategoryInfos();
            var subCategoryInfo = allCategories.Find(f => f.CategoryType == CategoryType.SubCategory && f.CategoryID == ID);
            if (subCategoryInfo != null)
            {
                ViewBag.Title = subCategoryInfo.CategoryName;
            }
            ViewBag.ID = ID;
            return View(pageVM);
        }

        /// <summary>
        /// 三级分类商品
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult AjaxCategory3Products(int ID)
        {
            SearchCriteriaModel criteria = new SearchCriteriaModel()
            {
                Category3ID = ID,
                PageSize = 10
            };
            ProductSearchResultVM pageVM = SearchManager.Search(criteria, HttpContext.Request.QueryString);
            ViewBag.ID = ID;

            return PartialView("~/Views/UserControl/_SearchProductList.cshtml", pageVM);
        }
    }
}
