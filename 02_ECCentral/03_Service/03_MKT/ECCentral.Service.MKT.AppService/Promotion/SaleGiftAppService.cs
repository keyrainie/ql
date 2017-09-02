using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.MKT.AppService.Promotion
{
    [VersionExport(typeof(SaleGiftAppService))]
    public class SaleGiftAppService
    {
        private SaleGiftProcessor _processor = ObjectFactory<SaleGiftProcessor>.Instance;

        /// <summary>
        /// 获取赠品所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual SaleGiftInfo Load(int? sysNo)
        {
            SaleGiftInfo info = _processor.Load(sysNo);
            if (info.ProductCondition != null)
            {
                foreach (SaleGift_RuleSetting setting in info.ProductCondition)
                {
                    if (setting.RelBrand != null && setting.RelBrand.SysNo.HasValue)
                    {
                        var brandName = ExternalDomainBroker.GetBrandInfoBySysNo(setting.RelBrand.SysNo.Value);
                        if (brandName != null && brandName.BrandNameLocal != null)
                        {
                            setting.RelBrand.Name = brandName.BrandNameLocal.Content;
                        }
                    }
                    if (setting.RelC3 != null && setting.RelC3.SysNo.HasValue)
                    {
                        var categoryName = ExternalDomainBroker.GetCategory3Info(setting.RelC3.SysNo.Value);
                        if (categoryName != null && categoryName.CategoryName != null)
                        {
                            setting.RelC3.Name = categoryName.CategoryName.Content;
                        }
                    }
                    if (setting.RelProduct != null && setting.RelProduct.ProductSysNo.HasValue)
                    {
                        ProductInfo product = ExternalDomainBroker.GetProductInfo(setting.RelProduct.ProductSysNo.Value);
                        if (product != null)
                        {
                            setting.RelProduct.ProductName = product.ProductName;
                            setting.RelProduct.ProductID = product.ProductID;
                            //获取商品可用库存，代销库存，毛利率等接口
                            ProductInventoryInfo inventory = ExternalDomainBroker.GetProductTotalInventoryInfo(product.SysNo);
                            setting.RelProduct.AvailableQty = inventory.AvailableQty;
                            setting.RelProduct.ConsignQty = inventory.ConsignQty;
                            setting.RelProduct.VirtualQty = inventory.VirtualQty;

                            setting.RelProduct.UnitCost = product.ProductPriceInfo.UnitCost;
                            setting.RelProduct.CurrentPrice = product.ProductPriceInfo.CurrentPrice;
                        }
                    }
                }

                foreach (SaleGift_RuleSetting setting in info.ProductCondition)
                {
                    if (setting.RelProduct.ProductSysNo.HasValue)
                    {
                        ProductInfo product = ExternalDomainBroker.GetProductInfo(setting.RelProduct.ProductSysNo.Value);
                        if (product != null)
                        {
                            int minBuyQty = setting.RelProduct.MinQty.HasValue ? (setting.RelProduct.MinQty.Value == 0 ? 1 : setting.RelProduct.MinQty.Value) : 1;
                            setting.RelProduct.GrossMarginRate = ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGift_SaleItemGrossMarginRate(product,
                                minBuyQty, sysNo.Value, info);
                        }
                    }
                }
            }

            if (info.GiftItemList != null)
            {
                foreach (RelProductAndQty giftItem in info.GiftItemList)
                {
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(giftItem.ProductSysNo.Value);
                    if (product == null) continue;
                    giftItem.ProductName = product.ProductName; ;
                    giftItem.ProductID = product.ProductID;
                    //获取商品可用库存，代销库存，毛利率等接口
                    ProductInventoryInfo inventory = ExternalDomainBroker.GetProductTotalInventoryInfo(product.SysNo);
                    if (inventory == null) continue;
                    giftItem.AvailableQty = inventory.AvailableQty;
                    giftItem.ConsignQty = inventory.ConsignQty;
                    giftItem.VirtualQty = inventory.VirtualQty;
                    giftItem.GrossMarginRate = ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGift_GiftItemGrossMarginRate(product, info.DisCountType.Value);
                    giftItem.UnitCost = product.ProductPriceInfo.UnitCost;
                    giftItem.CurrentPrice = product.ProductPriceInfo.CurrentPrice;
                }
            }
            return info;
        }

        /// <summary>
        /// 创建赠主信息 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int? CreateMaster(SaleGiftInfo info)
        {
            return _processor.CreateMaster(info);
        }

        /// <summary>
        /// 更新主信息
        /// </summary>
        /// <param name="info"></param>
        public virtual void UpdateMaster(SaleGiftInfo info)
        {
            _processor.UpdateMaster(info);
        }

        /// <summary>
        /// 促销活动规则设置
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetSaleRules(SaleGiftInfo info)
        {
            _processor.SetSaleRules(info);
        }

        /// <summary>
        /// 赠品设置
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetGiftItemRules(SaleGiftInfo info)
        {
            _processor.SetGiftItemRules(info);
        }

        /// <summary>
        /// 复制新建
        /// </summary>
        /// <param name="oldSysNo"></param>
        /// <returns></returns>
        public virtual int? CopyCreateNew(int? oldSysNo)
        {
            return _processor.CopyCreateNew(oldSysNo);
        }

        #region 全局行为
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void SubmitAudit(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.SubmitAudit(sysNoList, out successRecords, out failureRecords);
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void CancelAudit(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.CancelAudit(sysNoList, out successRecords, out failureRecords);
        }

        /// <summary>
        /// 审核，包含审核通过和审核拒绝
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="auditType"></param>
        public virtual void Audit(List<int?> sysNoList, PromotionAuditType auditType, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.Audit(sysNoList, auditType, out successRecords, out failureRecords);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Void(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.Void(sysNoList, out successRecords, out failureRecords);
        }

        /// <summary>
        /// 手动提前中止
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void ManualStop(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            _processor.ManualStop(sysNoList, out successRecords, out failureRecords);
        }

        #endregion
        /// <summary>
        ///检查主商品和赠品库存后返回结果
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string CheckGiftStockResult(SaleGiftInfo info)
        {
            return _processor.CheckGiftStockResult(info);
        }

        public List<RelVendor> GetGiftVendorList()
        {
            return _processor.GetGiftVendorList();
        }
    }
}
