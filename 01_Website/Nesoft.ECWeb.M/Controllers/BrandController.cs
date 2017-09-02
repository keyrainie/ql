using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity.Product;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Facade.Recommend;
using Nesoft.ECWeb.M.Models;
using Nesoft.ECWeb.M.Models.Brand;
using Nesoft.ECWeb.M.Models.Search;

namespace Nesoft.ECWeb.M.Controllers
{
    public class BrandController : Controller
    {
        //
        // GET: /Brand/

        /// <summary>
        /// 品牌街
        /// </summary>
        /// <returns></returns>
        public ActionResult BrandStreet()
        {

            BrandStreetVM brandStreetVM = new BrandStreetVM()
            {
                TopBrands = RecommendFacade.GetBannerInfoByPositionID(-1, PageType.PageTypeAppHome, BannerPosition.PositionAppHomeBrandBig)
                        .Take(4)
                        .ToList()
                        .ConvertAll<BrandItemVM>(banner =>
                        {
                            return new BrandItemVM()
                            {
                                ImageUrl = (banner.BannerResourceUrl ?? "").Trim(),
                                BrandName = (banner.BannerTitle ?? "").Trim(),
                                BrandID = banner.BannerLink.ExtractBrandSysNo()
                            };
                        }),

                BrandGrid = RecommendFacade.GetBannerInfoByPositionID(-1, PageType.PageTypeAppHome, BannerPosition.PositionAppHomeBrandSmall)
                       .Take(15)
                       .ToList()
                       .ConvertAll<BrandItemVM>(banner =>
                       {
                           return new BrandItemVM()
                           {
                               ImageUrl = (banner.BannerResourceUrl ?? "").Trim(),
                               BrandName = (banner.BannerTitle ?? "").Trim(),
                               BrandID = banner.BannerLink.ExtractBrandSysNo()
                           };
                       })
            };

            return View(brandStreetVM);
        }


        public ActionResult BrandDetail(int ID)
        {
            SearchCriteriaModel criteria = new SearchCriteriaModel()
            {
                BrandID = ID,
                PageSize = 6
            };
            ProductSearchResultVM pageVM = SearchManager.Search(criteria, HttpContext.Request.QueryString);
            var allBrands = ProductFacade.GetAllBrands();
            var brandInfo = allBrands.Find(f => f.SysNo == ID);
            if (brandInfo != null)
            {
                ViewBag.Title = string.Format("{0}({1})", brandInfo.BrandName_Ch, brandInfo.BrandName_En);
            }
            ViewBag.ID = ID;

            return View(pageVM);
        }


        public ActionResult AjaxBrandProducts(int ID)
        {
            SearchCriteriaModel criteria = new SearchCriteriaModel()
            {
                BrandID = ID,
                PageSize = 6
            };
            ProductSearchResultVM pageVM = SearchManager.Search(criteria, HttpContext.Request.QueryString);

            return PartialView("_BrandProductList", pageVM);
        }
    }
}
