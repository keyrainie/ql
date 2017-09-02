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
    [VersionExport(typeof(ComboAppService))]
    public class ComboAppService
    {
        private ComboProcessor _processor = ObjectFactory<ComboProcessor>.Instance;

        /// <summary>
        /// 加载Combo所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ComboInfo Load(int? sysNo)
        {
            ComboInfo info = _processor.Load(sysNo);
            if (info != null && info.Items != null && info.Items.Count > 0)
            {
                foreach (ComboItem item in info.Items)
                {
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);

                    VendorBasicInfo vendor = new VendorBasicInfo();
                    if (product != null && product.Merchant != null)
                    {
                        vendor
                            = ExternalDomainBroker.GetVendorBasicInfoBySysNo(product.Merchant.SysNo.Value);
                    }

                    item.MerchantName = vendor != null
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
        /// 创建Combo Master
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int? CreateCombo(ComboInfo info)
        {
            return _processor.CreateCombo(info);
        }

        /// <summary>
        /// 批量创建Combo
        /// </summary>
        /// <param name="comboList"></param>
        public List<ComboInfo> BatchCreateCombo(List<ComboInfo> comboList)
        {
            return _processor.BatchCreateCombo(comboList);
        }

        public List<ComboInfo> BatchUpdateCombo(List<ComboInfo> comboList)
        {
            return _processor.BatchUpdateCombo(comboList);
        }

        public List<ProductInfo> GetProductInfoListByProductSysNoList(List<int> productSysNoList)
        {
            return ExternalDomainBroker.GetProductInfoListByProductSysNoList(productSysNoList);
        }

        public List<string> CheckOptionalAccessoriesItemAndDiys(List<int> sysNos)
        {
            return _processor.CheckOptionalAccessoriesItemAndDiys(sysNos);
        }

        /// <summary>
        /// 更新Combo Master，包含：更新主信息，更新状态：
        /// 无效->有效,无效->待审核，有效->无效，有效->待审核,待审核->无效，待审核->有效
        /// 其中无效->有效需要Check RequiredSaleRule4UpdateValidate
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCombo(ComboInfo info)
        {
            _processor.UpdateCombo(info);
        }

        /// <summary>
        /// 对Combo Item进行检查,本方法对添加ComboItem时也有用
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<string> CheckComboItemIsPass(ComboInfo info)
        {
            return _processor.CheckComboItemIsPass(info);
        }



        //设置产品的价格和+折扣<成本价格和  变为待审核状态
        //提供一个接口供商品价格管理模块来调用，传入商品ID或者sysno，
        //然后检查商品对应捆绑规则是否有低于成本价的情况，有的就将其变为待审核(status=1)！
        /// <summary>
        /// 检查条件：Combo当前是有效状态, 检查内容：价格和+折扣和 小于 成本价格和 ，价格检查不通过
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CheckPriceIsPass(int comboSysNo)
        {
            ComboInfo info = _processor.Load(comboSysNo);
            return _processor.CheckPriceIsPass(info);
        }

        /// <summary>
        /// 仅仅更新状态，不做任何检查，主要是为外部系统提供服务
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="targetStatus"></param>
        public void UpdateStatus(int sysNo, ComboStatus targetStatus)
        {
            _processor.UpdateStatus(sysNo, targetStatus);
        }

        public void ApproveCombo(int sysNo, ComboStatus targetStatus)
        {
            _processor.ApproveCombo(sysNo, targetStatus);
        }

        public void BatchDelete(List<int> sysNoList)
        {
            _processor.BatchDelete(sysNoList);
        }

        /// <summary>
        /// 提供一个接口供商品价格管理模块来调用，传入商品sysno
        /// 然后检查商品对应捆绑规则是否有低于成本价的情况(价格和+折扣 《 成本价格和)，有的就将其变为待审核(status=1)
        /// </summary>
        /// <param name="productSysNo"></param>
        public void CheckComboPriceAndSetStatus(int productSysNo)
        {
            _processor.CheckComboPriceAndSetStatus(productSysNo);
        }
    }
}
