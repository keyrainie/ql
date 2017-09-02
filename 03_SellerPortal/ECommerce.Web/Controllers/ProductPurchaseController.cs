using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Enums;
using ECommerce.Service.Product;
using ECommerce.Utility;

namespace ECommerce.Web.Controllers
{
    public class ProductPurchaseController : SSLControllerBase
    {
        
        public ActionResult New()
        {
            ViewBag.Model = "New";
            return View("ProductPurchaseMaintain");
        }

        
        public ActionResult Maintain()
        {
            ViewBag.Model = "Maintain";
            return View("ProductPurchaseMaintain");
        }

        /// <summary>
        /// 商品库存调整单列表
        /// </summary>
        /// <returns></returns>
                
        public ActionResult ProductStockAdjustmentList()
        {
            return View();
        }

        /// <summary>
        /// 商品库存调整单维护
        /// </summary>
        /// <returns></returns>
                
        public ActionResult ProductStockAdjustmentMain(int? sysNo)
        {
            if (!sysNo.HasValue)
            {
                //新建商品库存调整单:
                return View();
            }
            else
            {
                //编辑商品库存调整单:
                var editInfo = ProductPurchaseService.GetProductStockAdjustmentInfo(sysNo.Value);
                return View(editInfo);
            }
        }

        [HttpPost]
        public ActionResult QueryProductStockAdjustList()
        {
            ProductStockAdjustListQueryFilter queryFilter = BuildQueryFilterEntity<ProductStockAdjustListQueryFilter>((s) =>
            {
                s.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            });

            var result = ProductPurchaseService.QueryProductStockAdjustmentList(queryFilter);
            return AjaxGridJson(result);
        }

        [HttpPost]
        public ActionResult AjaxSaveProductStockAdjustmentInfo()
        {
            if (!string.IsNullOrEmpty(Request["info"]))
            {
                ProductStockAdjustInfo adjustInfo = SerializationUtility.JsonDeserialize2<ProductStockAdjustInfo>(Request["info"]);
                if (null != adjustInfo)
                {
                    if (!adjustInfo.SysNo.HasValue)
                    {
                        adjustInfo.Status = ProductStockAdjustStatus.Origin;
                    }
                    else
                    {
                        foreach (var item in adjustInfo.AdjustItemList)
                        {
                            item.InUserSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
                        }
                    }
                    adjustInfo.CurrencyCode = 1;
                    adjustInfo.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
                    adjustInfo.InUserSysNo = adjustInfo.EditUserSysNo = adjustInfo.AuditUserSysNo = UserAuthHelper.GetCurrentUser().UserSysNo;
                    adjustInfo.InDate = adjustInfo.EditDate = adjustInfo.AuditDate = DateTime.Now;
                }
                ProductPurchaseService.SaveProductStockAdjustmentInfo(adjustInfo);
            }
            return Json(new { Data = true });
        }

        [HttpPost]
        public JsonResult AjaxSubmitAuditAdjust()
        {
            if (string.IsNullOrEmpty(Request["sysNo"]))
            {
                throw new BusinessException("操作失败！,没有传入单号！");
            }

            int adjustSysNo = Convert.ToInt32(Request["sysNo"]);
            var result = ProductPurchaseService.UpdateProductStockAdjustmentStatus(ProductStockAdjustStatus.WaitingAudit, adjustSysNo, UserAuthHelper.GetCurrentUser().UserSysNo);
            return Json(new { Data = true });

        }
        [HttpPost]
        public JsonResult AjaxAbandonAdjust()
        {
            if (string.IsNullOrEmpty(Request["sysNo"]))
            {
                throw new BusinessException("操作失败！,没有传入单号！");
            }
            int adjustSysNo = Convert.ToInt32(Request["sysNo"]);
            var result = ProductPurchaseService.UpdateProductStockAdjustmentStatus(ProductStockAdjustStatus.Abandon, adjustSysNo, UserAuthHelper.GetCurrentUser().UserSysNo);
            return Json(new { Data = true });

        }
        [HttpPost]
        public JsonResult AjaxAuditPassAdjust()
        {
            if (string.IsNullOrEmpty(Request["sysNo"]))
            {
                throw new BusinessException("操作失败！,没有传入单号！");
            }

            int adjustSysNo = Convert.ToInt32(Request["sysNo"]);
            var result = ProductPurchaseService.UpdateProductStockAdjustmentStatus(ProductStockAdjustStatus.AuditPass, adjustSysNo, UserAuthHelper.GetCurrentUser().UserSysNo);
            return Json(new { Data = true });

        }
        [HttpPost]
        public JsonResult AjaxAuditFailedAdjust()
        {
            if (string.IsNullOrEmpty(Request["sysNo"]))
            {
                throw new BusinessException("操作失败！,没有传入单号！");
            }


            int adjustSysNo = Convert.ToInt32(Request["sysNo"]);
            var result = ProductPurchaseService.UpdateProductStockAdjustmentStatus(ProductStockAdjustStatus.AuditFaild, adjustSysNo, UserAuthHelper.GetCurrentUser().UserSysNo);
            return Json(new { Data = true });

        }

        
        public ActionResult Query()
        {
            return View("ProductPurchaseQuery");
        }

        [HttpPost]
        [ActionName("Query")]
        public ActionResult AjaxQuery()
        {
            ProductPurchaseQueryFilter queryCriteria = BuildQueryFilterEntity<ProductPurchaseQueryFilter>();
            queryCriteria.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo.ToString();

            QueryResult<ProductPurchaseQueryBasicInfo> result
                        = ProductPurchaseService.QueryProductPurchase(queryCriteria);

            return AjaxGridJson(result);
        }

        [HttpPost]
        [ActionName("AddNewPurchaseOrderItem")]
        public ActionResult AjaxAddNewPurchaseOrderItem()
        {
            if (string.IsNullOrWhiteSpace(Request["Data"]))
            {
                throw new BusinessException("采购商品信息为空");
            }
            PurchaseOrderItemProductInfo product = SerializationUtility.JsonDeserialize2<PurchaseOrderItemProductInfo>(Request["Data"]);
            PurchaseOrderItemInfo orderItemInfo = ProductPurchaseService.AddNewPurchaseOrderItem(product, UserAuthHelper.GetCurrentUser().SellerSysNo);
            return Json(orderItemInfo);
        }

        [HttpPost]
        [ActionName("SavePO")]
        public ActionResult AjaxSavePurchaseOrder()
        {
            if (string.IsNullOrWhiteSpace(Request["Data"]))
            {
                throw new BusinessException("采购单信息为空");
            }
            PurchaseOrderInfo poInfo = SerializationUtility.JsonDeserialize2<PurchaseOrderInfo>(Request["Data"]);
            SetUserInfo(poInfo);
            poInfo.VendorInfo = new VendorBasicInfo()
            {
                VendorID = UserAuthHelper.GetCurrentUser().SellerSysNo.ToString()
            };
            poInfo = ProductPurchaseService.CreatePO(poInfo);
            return Json(poInfo);
        }

        [HttpPost]
        [ActionName("SubmitPO")]
        public ActionResult AjaxSubmitPurchaseOrder()
        {
            if (string.IsNullOrWhiteSpace(Request["Data"]))
            {
                throw new BusinessException("采购单信息为空");
            }
            PurchaseOrderInfo poInfo = SerializationUtility.JsonDeserialize2<PurchaseOrderInfo>(Request["Data"]);
            SetUserInfo(poInfo);
            poInfo = ProductPurchaseService.Submit(poInfo);
            return Json(poInfo);
        }

        [HttpPost]
        [ActionName("ValidPO")]
        public ActionResult AjaxValidPurchaseOrder()
        {
            if (string.IsNullOrWhiteSpace(Request["Data"]))
            {
                throw new BusinessException("采购单信息为空");
            }
            PurchaseOrderInfo poInfo = SerializationUtility.JsonDeserialize2<PurchaseOrderInfo>(Request["Data"]);
            SetUserInfo(poInfo);
            poInfo = ProductPurchaseService.ValidPO(poInfo);
            return Json(poInfo);
        }

        [HttpPost]
        [ActionName("RejectPO")]
        public ActionResult AjaxRejectPurchaseOrder()
        {
            if (string.IsNullOrWhiteSpace(Request["Data"]))
            {
                throw new BusinessException("采购单信息为空");
            }
            PurchaseOrderInfo poInfo = SerializationUtility.JsonDeserialize2<PurchaseOrderInfo>(Request["Data"]);
            SetUserInfo(poInfo);
            poInfo = ProductPurchaseService.RejectPO(poInfo);
            return Json(poInfo);
        }

        [HttpPost]
        [ActionName("AbandonPO")]
        public ActionResult AjaxAbandonPurchaseOrder()
        {
            if (string.IsNullOrWhiteSpace(Request["Data"]))
            {
                throw new BusinessException("采购单信息为空");
            }
            PurchaseOrderInfo poInfo = SerializationUtility.JsonDeserialize2<PurchaseOrderInfo>(Request["Data"]);
            SetUserInfo(poInfo);
            poInfo = ProductPurchaseService.AbandonPO(poInfo);
            return Json(poInfo);
        }

        [NonAction]
        private void SetUserInfo(PurchaseOrderInfo poInfo)
        {
            if (poInfo == null) return;
            var curentUser = UserAuthHelper.GetCurrentUser();
            poInfo.SellerSysNo = curentUser.SellerSysNo;
            poInfo.InUserSysNo = curentUser.UserSysNo;
            poInfo.InUserName = curentUser.UserDisplayName;
            poInfo.EditUserSysNo = curentUser.UserSysNo;
            poInfo.EditUserName = curentUser.UserDisplayName;
        }
    }
}