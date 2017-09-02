using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
//using ECCentral.Service.MKT.BizProcessor.PromotionEngine;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(SaleDiscountRuleProcessor))]
    public class SaleDiscountRuleProcessor : IPromotionCalculate
    {
        private ISaleDiscountRuleDA _daSaleDiscountRule = ObjectFactory<ISaleDiscountRuleDA>.Instance;
        //private SaleDiscountRulePromotionEngine _promotionEngine = ObjectFactory<SaleDiscountRulePromotionEngine>.Instance;

        public virtual SaleDiscountRule Load(int sysNo)
        {
            var result = _daSaleDiscountRule.Load(sysNo);
            if (result == null)
            {
                //throw new BizException(string.Format("系统编号为{0}的销售立减规则不存在。", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_NotExistSaleDiscountRule"), sysNo));
            }
            if (result.BrandSysNo > 0)
            {
                var brand = ExternalDomainBroker.GetBrandInfoBySysNo(result.BrandSysNo.Value);
                if (brand != null && brand.BrandNameLocal != null)
                {
                    result.UIBrandName = brand.BrandNameLocal.Content;
                }
            }
            if (result.ProductSysNo > 0)
            {
                var productInfo = ExternalDomainBroker.GetProductInfo(result.ProductSysNo.Value);
                if (productInfo != null)
                {
                    result.UIProductID = productInfo.ProductID;
                }
            }

            return result;
        }

        public virtual void Insert(SaleDiscountRule data)
        {
            CheckData(data);
            TransactionScopeFactory.TransactionAction(() =>
            {
                _daSaleDiscountRule.Insert(data);
                //_promotionEngine.SaveActivity(data);
            });
        }

        public virtual void Update(SaleDiscountRule data)
        {
            CheckData(data);
            TransactionScopeFactory.TransactionAction(() =>
           {
               _daSaleDiscountRule.Update(data);
               //_promotionEngine.SaveActivity(data);
           });
        }

        #region Helpers

        private void CheckData(SaleDiscountRule data)
        {
            data.SysNo = data.SysNo ?? 0;
            if (string.IsNullOrWhiteSpace(data.ActivityName))
            {
                //throw new BizException("活动名称必须填写。");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_ActivityNameNotNull"));
            }
            if (data.BeginDate == null)
            {
                //throw new BizException("开始时间必须填写。");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_StartDateNotNull"));
            }

            if (data.EndDate == null)
            {
                //throw new BizException("结束时间必须填写。");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_EndDateNotNull"));
            }

            if (data.EndDate <= data.BeginDate)
            {
                //throw new BizException("结束时间必须大于开始时间。");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_EndDateMoreThanStartDate"));
            }
            if (data.EndDate.HasValue && data.EndDate.Value <= DateTime.Now)
            {
                //throw new BizException("结束时间必须大于当前时间。");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_EndDateMoreThanCurrentDate"));
            }

            if (!data.IsC3SysNoValid()
                && !data.IsBrandSysNoValid()
                && !data.IsProductSysNoValid())
            {
                //throw new BizException("商品分类，品牌，商品ID至少设置一项。");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_AtLeastCatgoryBrandID"));
            }

            if (data.RuleType == SaleDiscountRuleType.QtyRule)
            {
                if (data.MinQty <= 0)
                {
                    //throw new BizException("数量下限必须大于等于零。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_LowerLimitMoreThan0"));
                }
                if (data.MaxQty <= 0)
                {
                    //throw new BizException("数量上限必须大于等于零。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_UpperLimitMoreThan0"));
                }
                if (data.MaxQty < data.MinQty)
                {
                    //throw new BizException("数量上限必须大于等于数量下限");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_UpperLimitMoreThanLowerLimit"));
                }
            }

            if (data.RuleType == SaleDiscountRuleType.AmountRule)
            {
                if (data.MinAmt <= 0)
                {
                    //throw new BizException("商品金额下限必须大于等于零。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_AmountLowerLimitMoreThan0"));
                }
                if (data.MaxAmt <= 0)
                {
                    //throw new BizException("商品金额上限必须大于等于零。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_AmountUpperLimitMoreThan0"));
                }
                if (data.MaxAmt < data.MinAmt)
                {
                    //throw new BizException("商品金额上限必须大于等于商品金额下限。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_AmountUpperLimitMoreThanAmountUpperLimit"));
                }
            }

            if (data.DiscountAmount > 0)
            {
                data.DiscountAmount = data.DiscountAmount * -1;
            }
            else if (data.DiscountAmount == 0)
            {
                //throw new BizException("销售折扣不能为零。");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_DiscountRuleNot0"));
            }


            CheckExistsRule(data);
        }

        private void CheckExistsRule(SaleDiscountRule data)
        {

            int c3SysNo = data.C3SysNo ?? 0;
            int brandSysNo = data.BrandSysNo ?? 0;
            int productSysNo = data.ProductSysNo ?? 0;
            int excludeSysNo = data.SysNo ?? 0;
            bool exists = true;
            //限定分类+品牌
            if (c3SysNo > 0 && brandSysNo > 0)
            {
                exists = _daSaleDiscountRule.CheckExistsProductScope_CategoryBrand(excludeSysNo, c3SysNo, brandSysNo);
                if (exists)
                {
                    //throw new BizException("已存在限定此分类+此品牌的有效销售规则，请不要重复设置。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_ExsistCategoryAndBrandActiveDiscountRule"));
                }
            }
            //限定分类
            else if (c3SysNo > 0)
            {
                exists = _daSaleDiscountRule.CheckExistsProductScope_Category(excludeSysNo, c3SysNo);
                if (exists)
                {
                    //throw new BizException("已存在限定此分类的有效销售规则，请不要重复设置。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_ExsistCategoryActiveDiscountRule"));
                }
            }
            //限定品牌
            else if (brandSysNo > 0)
            {
                exists = _daSaleDiscountRule.CheckExistsProductScope_Brand(excludeSysNo, brandSysNo);
                if (exists)
                {
                    //throw new BizException("已存在限定此品牌的有效销售规则，请不要重复设置。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_ExsistBrandActiveDiscountRule"));
                }
            }
            //限定商品(包含商品组的概念)
            else if (productSysNo > 0)
            {
                List<int> productSysNos = new List<int>(0);
                productSysNos.Add(productSysNo);
                var productInGroup = ExternalDomainBroker.GetProductsInSameGroupWithProductSysNo(productSysNo);
                if (productInGroup != null && productInGroup.Count > 0)
                {
                    productSysNos.Clear();
                    productSysNos = productInGroup.Select(item => item.SysNo).ToList();
                }
                exists = _daSaleDiscountRule.CheckExistsProductScope_Product(excludeSysNo, productSysNo);
                if (exists)
                {
                    //throw new BizException("已存在限定此商品或商品组的有效销售规则，请不要重复设置。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.SaleDiscountRule", "SaleDiscountRule_ExsistActiveDiscountRule"));
                }
            }
        }

        #endregion

        #region 计算类行为
        /// <summary>
        /// 为SO提供计算当前订单能能享受的所有促销活动结果
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns></returns>
        public virtual List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo,List<SOPromotionInfo> alreadyApplyPromoList)
        {
            //只有零售才享受销售立减
            if (soInfo.BaseInfo.IsWholeSale.HasValue && soInfo.BaseInfo.IsWholeSale.Value) return new List<SOPromotionInfo>(0);

            List<SOItemInfo> soItemList = new List<SOItemInfo>();
            foreach (SOItemInfo soItem in soInfo.Items)
            {
                if (soItem.ProductType.Value == SOProductType.Product)
                {
                    var product = ExternalDomainBroker.GetSimpleProductInfo(soItem.ProductSysNo.Value);
                    soItem.C3SysNo = product.ProductBasicInfo.ProductCategoryInfo.SysNo ?? 0;
                    soItem.BrandSysNo = product.ProductBasicInfo.ProductBrandInfo.SysNo ?? 0;
                    soItem.ProductGroupSysNo = product.ProductGroupSysno;
                    soItemList.Add(soItem);
                }
            }

            if (soItemList.Count == 0) return new List<SOPromotionInfo>(0);

            var promotionInfoList = SaleDiscountRuleCalculator.Instance.CalcSaleDiscountRule(soItemList, soInfo.SysNo ?? 0,alreadyApplyPromoList);

            return promotionInfoList;
        }
        #endregion
    }
}
