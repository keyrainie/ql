using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity.Member;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Member;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.WebFramework;

namespace Nesoft.ECWeb.M.Controllers
{
    public class CustomerController : SSLControllerBase
    {
        //
        // GET: /Customer/

        public ActionResult Coupons()
        {
            return View();
        }
        public ActionResult MyFavorite()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Ajax_QueryMyFavorite()
        {
            var result = new AjaxResult { Success = true };
            int pageIndex = int.Parse(Request["PageIndex"]);
            Nesoft.ECWeb.Entity.PageInfo pageInfo = new Entity.PageInfo();
            pageInfo.PageIndex = pageIndex;
            pageInfo.PageSize = 10;

            var user = UserManager.ReadUserInfo();

            var data = CustomerFacade.GetMyFavoriteProductList(user.UserSysNo, pageInfo);
            var wishSysNos = CookieHelper.GetCookie<List<int>>("DeletedFavorite") ?? new List<int>();
            data.ResultList.RemoveAll(p => wishSysNos.Any(q => p.WishSysNo == q));
            data.ResultList.ForEach(p =>
            {
                p.DefaultImage = ProductFacade.BuildProductImage(ImageSize.P60, p.DefaultImage);
            });
            result.Data = data;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Ajax_DeleteMyFavorite()
        {
            var result = new AjaxResult() { Success = true };
            var sysNo = int.Parse(Request["SysNo"]);
            CustomerFacade.DeleteMyFavorite(sysNo);
            var wishSysNos = CookieHelper.GetCookie<List<int>>("DeletedFavorite") ?? new List<int>();
            wishSysNos.Add(sysNo);
            CookieHelper.SaveCookie("DeletedFavorite", wishSysNos);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Ajax_QueryCoupons()
        {
            var result = new AjaxResult { Success = true };

            int pageIndex = int.Parse(Request["PageIndex"]);

            CustomerCouponCodeQueryInfo query = new CustomerCouponCodeQueryInfo();
            query.PageInfo.PageIndex = pageIndex;
            query.PageInfo.PageSize = 10;
            query.CustomerSysNo = this.CurrUser.UserSysNo;
            query.Status = "A";

            var data = CustomerFacade.QueryCouponCode(query);
            data.PageInfo.PageIndex++;
            result.Data = data;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Ajax_AddFavorite()
        {
            var result = new AjaxResult();
            result.Success = true;

            if (CurrUser == null)
            {
                result.Success = false;
                result.Message = "对不起,你还没有登录";
                result.Code = (int)HttpStatusCode.Unauthorized;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            int productSysNo = int.Parse(Request["ProductSysNo"]);
            CustomerFacade.AddProductToWishList(CurrUser.UserSysNo, productSysNo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Ajax_CancelFavorite()
        {
            var result = new AjaxResult();
            result.Success = true;

            if (CurrUser == null)
            {
                result.Success = false;
                result.Message = "对不起,你还没有登录";
                result.Code = (int)HttpStatusCode.Unauthorized;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //收藏的SysNo
            int productSysNo = int.Parse(Request["SysNo"]);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
