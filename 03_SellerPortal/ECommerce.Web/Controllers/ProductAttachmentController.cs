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
    /// <summary>
    /// 
    /// </summary>
    public class ProductAttachmentController : SSLControllerBase
    {
        
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AjaxQuery()
        {
            ProductAttachmentQueryFilter queryCriteria = BuildQueryFilterEntity<ProductAttachmentQueryFilter>();
            queryCriteria.SortFields = "ProductSysNo";

            int? currentSellerSysNo = null;
            int currentSellerSysNoValue = UserAuthHelper.GetCurrentUser().SellerSysNo;

            if (currentSellerSysNoValue > 0)
            {
                currentSellerSysNo = currentSellerSysNoValue;
            }

            queryCriteria.SellerSysNo = currentSellerSysNo;

            QueryResult<ProductAttachmentQueryBasicInfo> result
                        = ProductAttachmentService.QueryProductAttachment(queryCriteria);

            return AjaxGridJson(result);
        }


        [HttpPost]
        public ActionResult AjaxMaintainQuery()
        {
            string productSysNoText = Request.Form["ProductSysNo[]"];
            if (!string.IsNullOrEmpty(productSysNoText))
            {
                int productSysNo = 0;
                int.TryParse(productSysNoText, out productSysNo);

                if (productSysNo > 0)
                {
                    int? currentSellerSysNo = null;

                    int currentSellerSysNoValue = UserAuthHelper.GetCurrentUser().SellerSysNo;
                    if (currentSellerSysNoValue > 0)
                    {
                        currentSellerSysNo = currentSellerSysNoValue;
                    }

                    var result = ProductAttachmentService.GetProductAttachmentList(productSysNo, currentSellerSysNo);

                    return AjaxGridJson(result);
                }
            }

            return Json(1);

        }

        
        public ActionResult Maintain()
        {
            string productSysNoText = Request["ProductSysNo"];

            if (!string.IsNullOrEmpty(productSysNoText))
            {
                int productSysNo = 0;
                int.TryParse(productSysNoText, out productSysNo);
                ProductAttachmentStatus status = ProductAttachmentService.CheckTheProductStatusForEdit(productSysNo);
                ViewBag.ProductSysNo = status.ProductSysNo;
                ViewBag.ProductID = status.ProductID;
            }

            return View();
        }

        [HttpPost]
        public ActionResult AjaxRemove()
        {
            string sysNoText = Request.Form["ProductSysNo"];

            int productSysNo = 0;

            int.TryParse(sysNoText, out productSysNo);

            ProductAttachmentService.DeleteAttachmentByProductSysNo(productSysNo);

            return Json(1);
        }

        [HttpPost]
        public ActionResult AjaxRemoveAttachment()
        {
            string productSysNoText = Request.Form["ProductSysNo"];
            string sysNoText = Request.Form["SysNo"];

            int productSysNo = 0;
            int.TryParse(productSysNoText, out productSysNo);

            int sysNo = 0;
            int.TryParse(sysNoText, out sysNo);

            if (productSysNo > 0 && sysNo > 0)
            {
                ProductAttachmentService.DeleteSingleAttachment(productSysNo, sysNo);
            }

            return Json(1);
        }

        [HttpPost]
        public ActionResult AjaxSave()
        {
            ProductAttachmentStatus status = new ProductAttachmentStatus();

            string qtyText = Request.Form["Quantity"];
            int qty = 0;
            int.TryParse(qtyText, out qty);
            if (qty > 0)
            {
                ProductAttachmentInfo entity = new ProductAttachmentInfo();

                entity.AttachmentID = Request.Form["AttachmentID"];
                entity.ProductID = Request.Form["ProductID"];
                entity.Quantity = qty;
                entity.InUser = UserAuthHelper.GetCurrentUser().UserID;
                entity.InUserName = UserAuthHelper.GetCurrentUser().UserID;
                entity.InUserSysNo = UserAuthHelper.GetCurrentUser().UserSysNo;


                status = ProductAttachmentService.CreateProductAttachment(entity);
            }

            return Json(status);
        }
    }
}
