using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Facade;
using ECommerce.Facade.Catalog;
using ECommerce.Facade.Product.Models;

namespace ECommerce.UI.Controllers
{


    public class CatalogController : WWWControllerBase
    {
        //
        // GET: /SubCatalog/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestRegionAreaSelector2()
        {
            return View();
        }

        /// <summary>
        /// 三级类别页
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SubCategory(string subCategoryID)
        {
            var model = new StoreParams();
            //string subCategoryID = Request.QueryString["subCategoryID"];
            //if (string.IsNullOrWhiteSpace(subCategoryID))
            //{
            //    model.CurrentSysNo = "100101";
            //}

            //else
            //{
            //    model.CurrentSysNo = subCategoryID;
            //}
            ViewBag.SubCategoryID = subCategoryID;
            model.CurrentSysNo = subCategoryID;
            ViewBag.PageType = (int)ECommerce.Enums.PageType.SubStore;
            ViewBag.PageID = model.CurrentSysNo;
            return View(model);
        }

        /// <summary>
        /// 三级类别页
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult MidCategory(string subCategoryID)
        {
            var model = new StoreParams();
            //string subCategoryID = Request.QueryString["subCategoryID"];
            //if (string.IsNullOrWhiteSpace(subCategoryID))
            //{
            //    model.CurrentSysNo = "100101";
            //}

            //else
            //{
            //    model.CurrentSysNo = subCategoryID;
            //}
            //ViewBag.SubCategoryID = subCategoryID;
            model.CurrentSysNo = subCategoryID;
            ViewBag.PageType = (int)ECommerce.Enums.PageType.MidCategory;
            ViewBag.PageID = model.CurrentSysNo;
            return View(model);
        }

        public ActionResult TabStore(string currentSysNo)
        {
            var model = new StoreParams();
            if (string.IsNullOrWhiteSpace(currentSysNo))
            {
                model.CurrentSysNo = "1";
            }
            else
            {
                model.CurrentSysNo = currentSysNo;
            }
            ViewBag.PageType = (int)ECommerce.Enums.PageType.SubStore;
            ViewBag.PageID = model.CurrentSysNo;
            return View(model);
        }

        public ActionResult CategoryAll()
        {
            return View();
        }

        public ActionResult BrandAll()
        {
            return RedirectToAction("BrandList");
        }

        public ActionResult BrandList()
        {
            return View();
        }

        /// <summary>
        /// 品牌专区
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult BrandZone(int brandSysNo)
        {
            BrandZoneQueryVM queryInfo = new BrandZoneQueryVM();
            queryInfo.BrandSysNo = brandSysNo;
            queryInfo.SubCategoryEnID = Request[SearchPageFacade.Enid];

            if (!String.IsNullOrWhiteSpace(Request[SearchPageFacade.Keyword]))
            {
                queryInfo.Keyword = HttpUtility.UrlDecode(Request[SearchPageFacade.Keyword]);
            }

            int pageNumber = 0;
            if (!String.IsNullOrWhiteSpace(Request[SearchPageFacade.PageNumber]))
            {
                int.TryParse(Request[SearchPageFacade.PageNumber], out pageNumber);
            }
            queryInfo.PageNumber = pageNumber;

            int sortMode = 0;
            if (!String.IsNullOrWhiteSpace(Request[SearchPageFacade.SortKey]))
            {
                int.TryParse(Request[SearchPageFacade.SortKey], out sortMode);
            }
            queryInfo.SortMode = sortMode;

            BrandZoneVM resultVM = BrandZoneFacade.QueryBrandZoneVM(queryInfo);

            return View(resultVM);
        }
    }
}
