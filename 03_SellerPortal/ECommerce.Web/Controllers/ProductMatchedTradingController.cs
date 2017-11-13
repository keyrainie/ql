using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Service.Product;

namespace ECommerce.Web.Controllers
{
    public class ProductMatchedTradingController : SSLControllerBase
    {
        //
        // GET: /ProductMatchedTrading/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Query查询咨询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxQuery()
        {
            ProductMatchedTradingQueryFilter queryCriteria = BuildQueryFilterEntity<ProductMatchedTradingQueryFilter>();
            queryCriteria.SortFields = "ProductSysNo";

            int? currentSellerSysNo = null;

            int currentSellerSysNoValue = UserAuthHelper.GetCurrentUser().SellerSysNo;
            if (currentSellerSysNoValue > 0)
            {
                currentSellerSysNo = currentSellerSysNoValue;
            }

            queryCriteria.SellerSysNo = currentSellerSysNo;

            QueryResult<ProductMatchedTradingQueryBasicInfo> result = ProductMatchedTradingService.QueryProductMatchedTradingBasicInfoList(queryCriteria);

            return AjaxGridJson(result);
        }
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxChangeStatus()
        {
            List<int> sysNoList = new List<int>();

            string sysNoText = Request.Form["SysNo"];
            string statusText = Request.Form["Status"];

            string[] array = sysNoText.Split(',');

            foreach (var item in array)
            {
                int sysNo = 0;
                if (int.TryParse(item, out sysNo))
                {
                    sysNoList.Add(sysNo);
                }
            }
            string currentUser = UserAuthHelper.GetCurrentUser().UserID;
            if (sysNoList.Count > 0)
            {
                //审核
                if (statusText == "A")
                {
                    ProductMatchedTradingService.BatchSetProductMatchedTradingValid(sysNoList, currentUser);
                }
                //作废
                else if (statusText == "D")
                {
                    ProductMatchedTradingService.BatchSetProductMatchedTradingInvalid(sysNoList, currentUser);
                }
                //阅读
                else if (statusText == "E")
                {
                    ProductMatchedTradingService.BatchSetProductMatchedTradingRead(sysNoList, currentUser);
                }
            }

            return Json(1);
        }
        /// <summary>
        /// 回复页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Maintain()
        {
            string sysNoText = Request["SysNo"];
            if (!string.IsNullOrEmpty(sysNoText))
            {
                int sysNo = 0;
                int.TryParse(sysNoText, out sysNo);
                ProductMatchedTradingInfo entity = ProductMatchedTradingService.LoadProductMatchedTradingWithoutReply(sysNo);

                if (entity == null)
                {
                    return RedirectToAction("Index", "ProductMatchedTrading");
                }

                ViewBag.ProductMatchedTradingInfo = entity;
            }
            else
            {
                return RedirectToAction("Index", "ProductMatchedTrading");
            }

            return View();
        }
        /// <summary>
        /// 咨询回复
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSaveReplay()
        {
            string sysNoText = Request.Form["SysNo"];
            int sysNo = 0;
            int.TryParse(sysNoText, out sysNo);
            if (sysNo > 0)
            {
                string content = Request.Form["Content"];
                if (!string.IsNullOrEmpty(content))
                {
                    ProductMatchedTradingReplyInfo entity = new ProductMatchedTradingReplyInfo();
                    entity.ReplyContent = content;
                    entity.MatchedTradingSysNo = sysNo;
                    entity.SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
                    entity.Status = "A";
                    entity.StatusValue = "A";
                    entity.Type = "M";
                    //entity.NeedAdditionalText = "N";

                    entity.InUser = UserAuthHelper.GetCurrentUser().UserID;
                    //entity.InUser = UserAuthHelper.GetCurrentUser().SellerName;
                    entity.InDate = DateTime.Now;

                    ProductMatchedTradingService.AddProductMatchedTradingReply(entity);

                    return Json(entity);
                }
            }
            return Json(1);
        }
        /// <summary>
        /// Query查询回复
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxMaintainQuery()
        {
            string sysNoText = Request.Form["SysNo[]"];
            if (!string.IsNullOrEmpty(sysNoText))
            {
                int sysNo = 0;
                int.TryParse(sysNoText, out sysNo);
                if (sysNo > 0)
                {
                    int? currentSellerSysNo = null;

                    int currentSellerSysNoValue = UserAuthHelper.GetCurrentUser().SellerSysNo;
                    if (currentSellerSysNoValue > 0)
                    {
                        currentSellerSysNo = currentSellerSysNoValue;
                    }

                    var result = ProductMatchedTradingService.GetProductMatchedTradingFactoryReply(sysNo, currentSellerSysNo);

                    return AjaxGridJson(result);
                }
            }

            return Json(1);

        }
    }
}
