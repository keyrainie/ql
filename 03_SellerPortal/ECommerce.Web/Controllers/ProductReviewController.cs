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
    public class ProductReviewController : SSLControllerBase
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AjaxQuery()
        {
            ProductReviewQueryFilter queryCriteria = BuildQueryFilterEntity<ProductReviewQueryFilter>();
            queryCriteria.SortFields = "ProductSysNo";

            int? currentSellerSysNo = null;

            int currentSellerSysNoValue = UserAuthHelper.GetCurrentUser().SellerSysNo;
            if (currentSellerSysNoValue > 0)
            {
                currentSellerSysNo = currentSellerSysNoValue;
            }

            queryCriteria.SellerSysNo = currentSellerSysNo;

            QueryResult<ProductReviewQueryBasicInfo> result
                        = ProductReviewService.QueryProductReviewBasicInfoList(queryCriteria);

            return AjaxGridJson(result);
        }

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
                if (statusText == "A")
                {
                    ProductReviewService.BatchSetProductReviewValid(sysNoList, currentUser);
                }
                else if (statusText == "D")
                {
                    ProductReviewService.BatchSetProductReviewInvalid(sysNoList, currentUser);
                }
                else if (statusText == "E")
                {
                    ProductReviewService.BatchSetProductReviewRead(sysNoList, currentUser);
                }
            }

            return Json(1);
        }

        
        public ActionResult Maintain()
        {
            string sysNoText = Request["SysNo"];
            if (!string.IsNullOrEmpty(sysNoText))
            {
                int sysNo = 0;
                int.TryParse(sysNoText, out sysNo);
                ProductReviewInfo entity = ProductReviewService.LoadProductReviewWithoutReply(sysNo);

                if (entity == null)
                {
                    return RedirectToAction("Index", "ProductReview");
                }

                ViewBag.ProductReviewInfo = entity;
            }
            else
            {
                return RedirectToAction("Index", "ProductReview");
            }

            return View();
        }

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

                    var result = ProductReviewService.GetProductReviewFactoryReply(sysNo, currentSellerSysNo);

                    return AjaxGridJson(result);
                }
            }

            return Json(1);

        }

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
                    ProductReviewReplyInfo entity = new ProductReviewReplyInfo();
                    entity.Content = content;
                    entity.ReviewSysNo = sysNo;
                    entity.SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
                    entity.Status = "A";
                    entity.StatusValue = "A";
                    entity.Type = "M";
                    entity.NeedAdditionalText = "N";

                    entity.InUserName = UserAuthHelper.GetCurrentUser().UserID;
                    entity.InUser = UserAuthHelper.GetCurrentUser().UserID;
                    entity.InDate = DateTime.Now;

                    ProductReviewService.AddProductReviewReply(entity);
                    
                    return Json(entity);
                }
            }

            return Json(1);
        } 
    }
}