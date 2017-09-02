using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.BizProcessor.Promotion.Processors;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.IM;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.MKT.AppService.Promotion
{
    [VersionExport(typeof(OptionalAccessoriesAppService))]
    public class OptionalAccessoriesAppService
    {
        private OptionalAccessoriesProcessor _processor = ObjectFactory<OptionalAccessoriesProcessor>.Instance;

        /// <summary>
        /// 加载OptionalAccessories所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public OptionalAccessoriesInfo Load(int? sysNo)
        {
            OptionalAccessoriesInfo info = _processor.Load(sysNo);
            if (info != null && info.Items != null && info.Items.Count > 0)
            {
                foreach (OptionalAccessoriesItem item in info.Items)
                {
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);

                    VendorBasicInfo vendor = new VendorBasicInfo();
                    if (product != null && product.Merchant != null)
                    {
                        vendor
                            = ExternalDomainBroker.GetVendorBasicInfoBySysNo(product.Merchant.SysNo.Value);
                    }

                    item.MerchantName = vendor != null && !string.IsNullOrEmpty(vendor.VendorID)
                        ? (vendor.VendorType == VendorType.IPP
                        ? ResouceManager.GetMessageString("MKT.Promotion.Combo", "Combo_NeweggMerchant")
                        : product.Merchant.MerchantName)
                        : "";
                    item.MerchantSysNo = product.Merchant != null ? product.Merchant.MerchantID : null;
                    item.ProductID = product.ProductID;
                    item.ProductName = product.ProductName;
                    item.ProductPoint = product.ProductPriceInfo.Point;
                    item.ProductCurrentPrice = product.ProductPriceInfo.CurrentPrice;
                    item.ProductUnitCost = product.ProductPriceInfo.UnitCost;
                }
            }
            return info;
        }

        /// <summary>
        /// 创建OptionalAccessories Master
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int? CreateOptionalAccessories(OptionalAccessoriesInfo info)
        {
            return _processor.CreateOptionalAccessories(info);
        }

        /// <summary>
        /// 更新Combo Master，包含：更新主信息，更新状态：
        /// 无效->有效,无效->待审核，有效->无效，有效->待审核,待审核->无效，待审核->有效
        /// 其中无效->有效需要Check RequiredSaleRule4UpdateValidate
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOptionalAccessories(OptionalAccessoriesInfo info)
        {
            _processor.UpdateOptionalAccessories(info);
        }

        public void ApproveOptionalAccessories(int? sysNo, ComboStatus targetStatus)
        {
            _processor.ApproveOptionalAccessories(sysNo, targetStatus);
        }

        /// <summary>
        /// 对OptionalAccessories Item进行检查,本方法对添加时也有用
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<string> CheckOptionalAccessoriesItemIsPass(OptionalAccessoriesInfo info)
        {
            return _processor.CheckOptionalAccessoriesItemIsPass(info, true);
        }

        public List<string> CheckSaleRuleItemAndDiys(List<int> sysNos)
        {
            return _processor.CheckSaleRuleItemAndDiys(sysNos);
        }
    }
}
