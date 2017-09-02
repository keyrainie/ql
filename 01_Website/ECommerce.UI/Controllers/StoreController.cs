using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Store;
using ECommerce.Entity.Store.Filter;
using ECommerce.Facade.Store;
using ECommerce.Utility;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Member;
using ECommerce.Facade.Shopping;

namespace ECommerce.UI.Controllers
{
    public class StoreController : Controller
    {
        //
        // GET: /Store/

        /// <summary>
        /// 如果是预览则PageSysNo是StorePageInfo中的sysno
        /// </summary>
        /// <param name="SellerSysNo"></param>
        /// <param name="PageSysNo"></param>
        /// <param name="Preview"></param>
        /// <returns></returns>
        public ActionResult Index(int SellerSysNo, int? PageSysNo, bool Preview)
        {
            if (!PageSysNo.HasValue)
            {
                var home = StoreFacade.QueryHomePage(SellerSysNo);
                if (home != null)
                {
                    PageSysNo = home.SysNo;
                }
                
            }

            var page = StoreFacade.QueryStorePage(new StorePageFilter
            {
                SellerSysNo = SellerSysNo,
                PublishPageSysNo = PageSysNo,
                IsPreview = Preview
            });

            ViewBag.SellerSysNo = page.SellerSysNo;
            ViewBag.PageSysNo = PageSysNo;
            ViewBag.Theme = page.StorePageTemplate.StorePageThemeCssUrl;
            ViewBag.PageTypeClassName = page.StorePageType.ClassName;
            ViewBag.Preview = Preview;

            return View("~/Views/Store/Index.cshtml", page);
        }

        public ActionResult GetCouponPopContent(int MerchantSysNo)
        {
            CouponContentInfo Model = new CouponContentInfo();
            //优惠卷
            LoginUser user = UserMgr.ReadUserInfo();
            List<CustomerCouponInfo> CustomerCouponList = new List<CustomerCouponInfo>();
            if (user != null)
            {
                CustomerCouponList = ShoppingFacade.GetCustomerPlatformCouponCode(user.UserSysNo, MerchantSysNo);
            }
            //获取当前有效的优惠券活动
            List<CouponInfo> CouponList = new List<CouponInfo>();
            if (user != null)
            {
                CouponList = ShoppingFacade.GetCouponList(user.UserSysNo, MerchantSysNo);
            }
            if (user != null)
            {
                Model.UserSysNo = user.UserSysNo;
                Model.MerchantSysNo = MerchantSysNo;
                Model.customerCouponCodeList = CustomerCouponList;
                Model.couponList = CouponList;
            }
            PartialViewResult view = PartialView("~/Views/Shared/_CouponPop.cshtml", Model);
            return view;
        }

    }
}
