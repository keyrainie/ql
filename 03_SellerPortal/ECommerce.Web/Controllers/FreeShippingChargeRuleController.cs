using ECommerce.Entity.Common;
using ECommerce.Service.Common;
using ECommerce.Utility;
using ECommerce.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ECommerce.Web.Controllers
{
    public class FreeShippingChargeRuleController : WWWControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ContentResult Query()
        {
            var queryFilter = BuildQueryFilterEntity<FreeShippingChargeRuleQueryFilter>();
            queryFilter.SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            return AjaxGridJson(FreeShippingChargeRuleService.QueryRules(queryFilter));
        }

        public ActionResult FreeShippingMaintain()
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            FreeShippingChargeRuleInfo data = null;
            if (sysNo != null && int.TryParse(sysNo, out id))
            {
                var loginUser = UserAuthHelper.GetCurrentUser();
                data = FreeShippingChargeRuleService.Load(id, loginUser.SellerSysNo);
            }
            ViewBag.Data = new JavaScriptSerializer().Serialize(data);
            return View();
        }

        /// <summary>
        /// 创建或编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FreeShippingChargeRuleSave()
        {
            FreeShippingChargeRuleInfo info = SerializationUtility.JsonDeserialize2<FreeShippingChargeRuleInfo>(Request.Form["data"]);
            var loginUser = UserAuthHelper.GetCurrentUser();
            info.SellerSysNo = loginUser.SellerSysNo;
            info.InUserSysNo = loginUser.UserSysNo;
            info.InUserName = loginUser.UserDisplayName;
            info.EditUserSysNo = loginUser.UserSysNo;
            info.EditUserName = loginUser.UserDisplayName;
            if (info.SysNo > 0)
            {
                FreeShippingChargeRuleService.Update(info);
                return JsonSuccess(info.SysNo.Value, LanguageHelper.GetText("更新成功。"));
            }
            else
            {
                FreeShippingChargeRuleService.Create(info);
                return JsonSuccess(info.SysNo.Value, LanguageHelper.GetText("创建成功。"));
            }
        }

        /// <summary>
        /// 设置一条免运费规则为有效状态
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult Valid(int sysno)
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            if (int.TryParse(sysNo, out id))
            {
                var loginUser = UserAuthHelper.GetCurrentUser();
                FreeShippingChargeRuleService.Valid(id, loginUser.SellerSysNo, loginUser.UserSysNo);
            }
            else
            {
                return Json(LanguageHelper.GetText("参数错误。"));
            }

            return JsonSuccess(id, LanguageHelper.GetText("有效成功。"));
        }

        /// <summary>
        /// 设置一条免运费规则为无效状态
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult Invalid(int sysno)
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            if (int.TryParse(sysNo, out id))
            {
                var loginUser = UserAuthHelper.GetCurrentUser();
                FreeShippingChargeRuleService.Invalid(id, loginUser.SellerSysNo, loginUser.UserSysNo);
            }
            else
            {
                return Json(LanguageHelper.GetText("参数错误。"));
            }

            return JsonSuccess(id, LanguageHelper.GetText("无效成功。"));
        }

        private JsonResult JsonSuccess(int sysNo, string msg)
        {
            return Json(new { sysNo = sysNo, msg = msg });
        }
    }
}
