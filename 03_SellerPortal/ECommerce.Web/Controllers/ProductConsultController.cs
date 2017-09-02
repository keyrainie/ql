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
    public class ProductConsultController : SSLControllerBase
    {
        //
        // GET: /ProductConsult/

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
            ProductConsultQueryFilter queryCriteria = BuildQueryFilterEntity<ProductConsultQueryFilter>();
            queryCriteria.SortFields = "ProductSysNo";

            int? currentSellerSysNo = null;

            int currentSellerSysNoValue = UserAuthHelper.GetCurrentUser().SellerSysNo;
            if (currentSellerSysNoValue > 0)
            {
                currentSellerSysNo = currentSellerSysNoValue;
            }

            queryCriteria.SellerSysNo = currentSellerSysNo;

            QueryResult<ProductConsultQueryBasicInfo> result = ProductConsultService.QueryProductConsultBasicInfoList(queryCriteria);

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
                    ProductConsultService.BatchSetProductConsultValid(sysNoList, currentUser);
                }
                 //作废
                else if (statusText == "D")
                {
                    ProductConsultService.BatchSetProductConsultInvalid(sysNoList, currentUser);
                }
                //阅读
                else if (statusText == "E")
                {
                    ProductConsultService.BatchSetProductConsultRead(sysNoList, currentUser);
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
                ProductConsultInfo entity = ProductConsultService.LoadProductConsultWithoutReply(sysNo);

                if (entity == null)
                {
                    return RedirectToAction("Index", "ProductConsult");
                }

                ViewBag.ProductConsultInfo = entity;
            }
            else
            {
                return RedirectToAction("Index", "ProductConsult");
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
                    ProductConsultReplyInfo entity = new ProductConsultReplyInfo();
                    entity.ReplyContent = content;
                    entity.ConsultSysNo = sysNo;
                    entity.SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
                    entity.Status = "A";
                    entity.StatusValue = "A";
                    entity.Type = "M";
                    //entity.NeedAdditionalText = "N";

                    entity.InUser = UserAuthHelper.GetCurrentUser().UserID;
                    //entity.InUser = UserAuthHelper.GetCurrentUser().SellerName;
                    entity.InDate = DateTime.Now;

                    ProductConsultService.AddProductConsultReply(entity);

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

                    var result = ProductConsultService.GetProductConsultFactoryReply(sysNo, currentSellerSysNo);

                    return AjaxGridJson(result);
                }
            }

            return Json(1);

        }
    }
}
