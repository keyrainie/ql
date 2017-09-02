using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.ComponentModel;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using System.Data;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(GrossMarginProcessor))]
    public class GrossMarginProcessor
    {
        private IGrossMarginProcessorDA _da = ObjectFactory<IGrossMarginProcessorDA>.Instance;

        //积分与钱的兑换比例
        private decimal pointExchangeRate = ObjectFactory<ICustomerBizInteract>.Instance.GetPointToMoneyRatio();


        /// <summary>
        /// 获取指定商品当前时间生效的“厂商赠品”的赠品成本总和  
        /// Old Method:GetGiftAmount
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetSaleGiftCurrentAmountForVendor(int productSysNo)
        {
            decimal result = 0.00m;
            //取得指定商品所有的有效赠品及赠品数量
            DataTable dt = _da.GetSaleGiftCurrentGiftProductsForVendor(productSysNo);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    int giftProductSysNo = int.Parse(dr["ProductSysNo"].ToString());
                    int productCount = int.Parse(dr["ProductCount"].ToString());
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(giftProductSysNo);
                    result += Math.Round(productCount * product.ProductPriceInfo.UnitCost, 2);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定商品，指定赠品活动的非“买满即赠”的赠品成本总和 -OK
        /// Old Method:GetCurrentGiftAmount
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetSaleGiftAmountForNotFull(int productSysNo, int saleGiftSysNo)
        {
            decimal result = 0.00m;
            //获取指定商品，指定赠品活动的非“买满即赠”的赠品及赠品数量
            DataTable dt = _da.GetSaleGiftGiftProductsExcludeFull(productSysNo, saleGiftSysNo);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    int giftProductSysNo = int.Parse(dr["ProductSysNo"].ToString());
                    int productCount = int.Parse(dr["ProductCount"].ToString());
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(giftProductSysNo);
                    result += Math.Round(productCount * product.ProductPriceInfo.CurrentPrice.Value, 2);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定商品，赠品池活动的非“买满即赠”的赠品成本总和 -OK
        /// Old Method:GetCurrentGiftAmount
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetSaleGiftAmountForFull(int productSysNo, int saleGiftSysNo, int ItemGiftCount)
        {
            decimal result = 0.00m;
            //获取指定商品，指定赠品活动的非“买满即赠”的赠品及赠品数量
            DataTable dt = _da.GetSaleGiftGiftProductsExcludeFull(productSysNo, saleGiftSysNo);
            var productPrices = new List<decimal>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    int giftProductSysNo = int.Parse(dr["ProductSysNo"].ToString());
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(giftProductSysNo);
                    productPrices.Add(product.ProductPriceInfo.CurrentPrice.Value);
                }
            }
            var price = productPrices.OrderByDescending(p => p).Take(ItemGiftCount).Sum();

            return price;
        }
        /// <summary>
        /// 获取指定商品当前时间生效的“PM-产品优惠券”的折扣金额
        /// Old Method:GetCouponAmount
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetCouponCurrentAmountForPM(int productSysNo)
        {
            ProductInfo product = ExternalDomainBroker.GetProductInfo(productSysNo);
            return GetCouponCurrentAmountForPM(product);
        }

        public virtual decimal GetCouponCurrentAmountForPM(ProductInfo product)
        {
            decimal result = 0.00m;
            //获取指定商品当前 时间生效,优惠券范围=“PM-产品优惠券”,商品范围=指定商品，的优惠券活动系统编号列表
            List<int> sysNoList = _da.GetCurrentCouponsForPM(product.SysNo);
            if (sysNoList.Count > 0)
            {
                List<CouponsInfo> couponList = new List<CouponsInfo>();
                foreach (int sysno in sysNoList)
                {
                    couponList.Add(ObjectFactory<CouponsProcessor>.Instance.Load(sysno));
                }

                decimal maxDiscount = 0.00m;
                foreach (CouponsInfo coupon in couponList)
                {
                    if (coupon.OrderAmountDiscountRule != null && coupon.OrderAmountDiscountRule.OrderAmountDiscountRank != null && coupon.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
                    {
                        foreach (OrderAmountDiscountRank rank in coupon.OrderAmountDiscountRule.OrderAmountDiscountRank)
                        {
                            decimal tempDiscount = 0.00m;
                            if (rank.DiscountType.Value == PSDiscountTypeForOrderAmount.OrderAmountPercentage)
                            {
                                tempDiscount = Math.Round(product.ProductPriceInfo.CurrentPrice.Value * rank.DiscountValue.Value, 2);
                            }
                            else
                            {
                                tempDiscount = Convert.ToDecimal(rank.DiscountValue);
                            }
                            if (tempDiscount > maxDiscount)
                            {
                                maxDiscount = tempDiscount;
                            }
                        }
                    }

                    if (coupon.PriceDiscountRule != null && coupon.PriceDiscountRule.Count > 0)
                    {
                        foreach (PSPriceDiscountRule rule in coupon.PriceDiscountRule)
                        {
                            decimal tempDiscount = 0.00m;
                            if (rule.DiscountType.Value == PSDiscountTypeForProductPrice.ProductPriceDiscount)
                            {
                                tempDiscount = rule.DiscountValue.Value;
                            }
                            if (rule.DiscountType.Value == PSDiscountTypeForProductPrice.ProductPriceFinal)
                            {
                                tempDiscount = product.ProductPriceInfo.CurrentPrice.Value - rule.DiscountValue.Value;
                            }

                            if (tempDiscount > maxDiscount)
                            {
                                maxDiscount = tempDiscount;
                            }
                        }
                    }
                }

                result = maxDiscount;
            }

            return result;
        }


        /// <summary>
        /// 指定活动，指定商品的“PM-产品优惠券”的折扣金额
        /// Old Method:GetCurrentCouponAmount
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetCouponAmountForPM(int productSysNo, int couponSysNo)
        {
            ProductInfo product = ExternalDomainBroker.GetProductInfo(productSysNo);
            return GetCouponAmountForPM(product, couponSysNo);
        }

        public virtual decimal GetCouponAmountForPM(ProductInfo product, int couponSysNo)
        {
            decimal result = 0.00m;
            CouponsInfo coupon = ObjectFactory<CouponsProcessor>.Instance.Load(couponSysNo);
            if (coupon.CouponChannelType.Value == CouponsMKTType.MKTPM && coupon.ProductRangeType.Value == CouponsProductRangeType.LimitProduct)
            {
                decimal maxDiscount = 0.00m;
                if (coupon.OrderAmountDiscountRule != null && coupon.OrderAmountDiscountRule.OrderAmountDiscountRank != null && coupon.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
                {
                    foreach (OrderAmountDiscountRank rank in coupon.OrderAmountDiscountRule.OrderAmountDiscountRank)
                    {
                        decimal tempDiscount = 0.00m;
                        if (rank.DiscountType.Value == PSDiscountTypeForOrderAmount.OrderAmountPercentage) //百分比:优惠券金额=商品当前价*百分比
                        {
                            tempDiscount = Math.Round(product.ProductPriceInfo.CurrentPrice.Value * rank.DiscountValue.Value, 2);

                        }
                        else //折扣金额:优惠券金额=折扣金额
                        {
                            tempDiscount = Convert.ToDecimal(rank.DiscountValue);
                        }
                        if (tempDiscount > maxDiscount)
                        {
                            maxDiscount = tempDiscount;
                        }
                    }
                }

                if (coupon.PriceDiscountRule != null && coupon.PriceDiscountRule.Count > 0)
                {
                    foreach (PSPriceDiscountRule rule in coupon.PriceDiscountRule)
                    {
                        decimal tempDiscount = 0.00m;
                        if (rule.DiscountType.Value == PSDiscountTypeForProductPrice.ProductPriceDiscount)
                        {
                            tempDiscount = rule.DiscountValue.Value;
                        }
                        if (rule.DiscountType.Value == PSDiscountTypeForProductPrice.ProductPriceFinal)
                        {
                            tempDiscount = product.ProductPriceInfo.CurrentPrice.Value - rule.DiscountValue.Value;
                        }

                        if (tempDiscount > maxDiscount)
                        {
                            maxDiscount = tempDiscount;
                        }
                    }
                }

                result = maxDiscount;
            }



            return result;
        }
        //---------------------------上面是各项Amount基础数据的获取，下面是毛利率的获取------------------------------//

        /// <summary>
        /// 获取指定商品初级库龄毛利率下限（根据入库时间） --ok
        /// Old Method: GetMinMarginByProductSysNo
        /// </summary>
        /// <param name="itemSysNo"></param>
        /// <param name="minMargin"></param>
        public virtual decimal GetStockPrimaryGrossMarginRate(int productSysNo)
        {
            ProductInfo product = ExternalDomainBroker.GetProductInfo(productSysNo);
            return GetStockGrossMarginRate(product, true);
        }

        /// <summary>
        /// 获取指定商品的库龄高级毛利率下限
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetStockSeniorGrossMarginRate(int productSysNo)
        {
            ProductInfo product = ExternalDomainBroker.GetProductInfo(productSysNo);
            return GetStockGrossMarginRate(product, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="isPrimary">是否为初级毛利，否的话就是高级毛利</param>
        /// <returns></returns>
        public virtual decimal GetStockGrossMarginRate(ProductInfo product, bool isPrimary)
        {
            int inStockDays = ExternalDomainBroker.GetInStockDaysByProductSysNo(product.SysNo);
            CategorySetting cateSetting = ExternalDomainBroker.GetCategorySetting(product.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);
            MinMarginKPI kpi = new MinMarginKPI();
            if (cateSetting.CategoryMinMarginInfo != null && cateSetting.CategoryMinMarginInfo.Margin != null && cateSetting.CategoryMinMarginInfo.Margin.Count() > 0)
            {
                if (inStockDays >= 0 && inStockDays <= 30)
                {
                    kpi = cateSetting.CategoryMinMarginInfo.Margin[MinMarginDays.Thirty];
                }

                if (inStockDays >= 31 && inStockDays <= 60)
                {
                    kpi = cateSetting.CategoryMinMarginInfo.Margin[MinMarginDays.Sixty];
                }

                if (inStockDays >= 61 && inStockDays <= 90)
                {
                    kpi = cateSetting.CategoryMinMarginInfo.Margin[MinMarginDays.Ninety];
                }

                if (inStockDays >= 91 && inStockDays <= 120)
                {
                    kpi = cateSetting.CategoryMinMarginInfo.Margin[MinMarginDays.OneHundredAndTwenty];
                }

                if (inStockDays >= 120 && inStockDays <= 180)
                {
                    kpi = cateSetting.CategoryMinMarginInfo.Margin[MinMarginDays.OneHundredAndEighty];
                }

                if (inStockDays > 180)
                {
                    kpi = cateSetting.CategoryMinMarginInfo.Margin[MinMarginDays.Other];
                }
            }
            else
            {
                if (inStockDays >= 0 && inStockDays <= 30)
                {
                    kpi.MinMargin = 0.05m;
                    kpi.MaxMargin = 0.05m;
                }

                if (inStockDays >= 31 && inStockDays <= 180)
                {
                    kpi.MinMargin = 0.01m;
                    kpi.MaxMargin = 0.01m;
                }

                if (inStockDays > 180)
                {
                    kpi.MinMargin = -0.03m;
                    kpi.MaxMargin = -0.03m;
                }
            }
            if (isPrimary)
                return kpi.MinMargin;//初级毛利率下限
            else
                return kpi.MaxMargin;//高级毛利率下限
        }

        /// <summary>
        /// 获取指定商品类别下最小毛利
        /// </summary>
        /// <param name="itemSysNo"></param>
        /// <param name="minMargin"></param>
        public virtual decimal GetCategoryMinGrossMarginRate(int productSysNo)
        {
            ProductInfo product = ExternalDomainBroker.GetProductInfo(productSysNo);
            return GetCategoryMinGrossMarginRate(product);
        }

        public virtual decimal GetCategoryMinGrossMarginRate(ProductInfo product)
        {
            CategorySetting setting = ExternalDomainBroker.GetCategorySetting(product.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);
            return setting.PrimaryMargin;
        }

        /// <summary>
        /// 获取商品按当前售价出售时的毛利率
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual decimal GetSalesGrossMarginRate(ProductInfo product)
        {
            ProductMarginer productMarginer = new ProductMarginer();
            productMarginer.ProductSysNo = product.SysNo;
            productMarginer.Point = Convert.ToDecimal(product.ProductPriceInfo.Point);
            productMarginer.UnitCost = product.ProductPriceInfo.UnitCost;
            productMarginer.CurrentPrice = product.ProductPriceInfo.CurrentPrice.Value;
            productMarginer.SalePrice = productMarginer.CurrentPrice;
            productMarginer.CouponAmount = Math.Round(ObjectFactory<GrossMarginProcessor>.Instance.GetCouponCurrentAmountForPM(product.SysNo), 2);
            productMarginer.GiftAmount = Math.Round(ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGiftCurrentAmountForVendor(product.SysNo), 2);
            return CalcGrossMarginRate(productMarginer);
        }

        /// <summary>
        /// 获取指定商品Countdown的毛利
        /// </summary>
        /// <param name="countDownCurrentPrice"></param>
        /// <param name="unitcost"></param>
        /// <param name="countDownPoint"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetCurrentGrossMarginForCountdown(decimal countDownCurrentPrice, decimal unitcost, int countDownPoint, int productSysNo)
        {
            //return countDownCurrentPrice - unitcost - countDownPoint / pointExchangeRate - GetSaleGiftCurrentAmountForVendor(productSysNo) 
            //            - GetCouponCurrentAmountForPM(productSysNo);

            //计算毛利统一归入 IM
            return ObjectFactory<IIMBizInteract>.Instance.GetProductMarginAmount(
                                    countDownCurrentPrice - GetSaleGiftCurrentAmountForVendor(productSysNo) - GetCouponCurrentAmountForPM(productSysNo),
                                    countDownPoint, unitcost);
        }

        public virtual decimal GetCurrentGrossMarginRateForCountdown(decimal countDownCurrentPrice, decimal unitcost, int countDownPoint, int productSysNo)
        {
            //计算毛利率统一归入 IM
            return ObjectFactory<IIMBizInteract>.Instance.GetProductMargin(
                                    countDownCurrentPrice, countDownPoint, unitcost,
                                    GetSaleGiftCurrentAmountForVendor(productSysNo) + GetCouponCurrentAmountForPM(productSysNo));
        }

        /// <summary>
        ///  指定活动，指定商品的为“PM-产品优惠券”的Coupon的毛利率
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetCouponGrossMarginRateForPM(ProductInfo product, int couponSysNo)
        {
            decimal couponAmount = GetCouponAmountForPM(product, couponSysNo);
            decimal giftAmount = GetSaleGiftCurrentAmountForVendor(product.SysNo);

            ProductMarginer productMarginer = new ProductMarginer()
            {
                CurrentPrice = product.ProductPriceInfo.CurrentPrice.Value,
                SalePrice = product.ProductPriceInfo.CurrentPrice.Value,
                Point = Convert.ToDecimal(product.ProductPriceInfo.Point),
                UnitCost = product.ProductPriceInfo.UnitCost,
                CouponAmount = couponAmount,
                GiftAmount = giftAmount,
                ProductSysNo = product.SysNo
            };
            return CalcGrossMarginRate(productMarginer);
        }

        /// <summary>
        /// Coupon: 指定商品的Coupon的毛利率
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual decimal GetCouponGrossMarginRate(ProductInfo product)
        {
            decimal couponAmount = GetCouponCurrentAmountForPM(product);
            decimal giftAmount = GetSaleGiftCurrentAmountForVendor(product.SysNo);
            ProductMarginer productMarginer = new ProductMarginer()
            {
                CurrentPrice = product.ProductPriceInfo.CurrentPrice.Value,
                SalePrice = product.ProductPriceInfo.CurrentPrice.Value,
                Point = Convert.ToDecimal(product.ProductPriceInfo.Point),
                UnitCost = product.ProductPriceInfo.UnitCost,
                CouponAmount = couponAmount,
                GiftAmount = giftAmount,
                ProductSysNo = product.SysNo
            };
            return CalcGrossMarginRate(productMarginer);
        }

        /// <summary>
        /// 赠品活动中获取销售商品的毛利率:
        /// </summary>
        /// <param name="product"></param>
        /// <param name="minBuyQty"></param>
        /// <param name="giftSaleSysNo"></param>
        /// <returns></returns>
        public virtual decimal GetSaleGift_SaleItemGrossMarginRate(ProductInfo product, int minBuyQty, int giftSaleSysNo, SaleGiftInfo info)
        {
            ProductMarginer productMarginer = new ProductMarginer();
            productMarginer.ProductSysNo = product.SysNo;
            productMarginer.Point = Convert.ToDecimal(product.ProductPriceInfo.Point);
            productMarginer.UnitCost = product.ProductPriceInfo.UnitCost;
            productMarginer.CurrentPrice = product.ProductPriceInfo.CurrentPrice.Value;
            productMarginer.CouponAmount = Math.Round(ObjectFactory<GrossMarginProcessor>.Instance.GetCouponCurrentAmountForPM(product) / minBuyQty, 2);
            if (info.GiftComboType == SaleGiftGiftItemType.AssignGift)
            {
                productMarginer.GiftAmount = Math.Round(ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGiftAmountForNotFull(product.SysNo, giftSaleSysNo) / minBuyQty, 2);
            }
            else
            {
                var count = info.ItemGiftCount == null ? 0 : info.ItemGiftCount.Value;
                productMarginer.GiftAmount = Math.Round(ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGiftAmountForFull(product.SysNo, giftSaleSysNo, count) / minBuyQty, 2);
            }

            if (info.Type == SaleGiftType.Multiple && info.DisCountType == SaleGiftDiscountBelongType.BelongMasterItem)
            {
                decimal? salePriceTotalTmp = info.ProductCondition.Where(ps => ps.Type == SaleGiftSaleRuleType.Item)
                                                .Sum(ps => ps.RelProduct.CurrentPrice);
                productMarginer.GiftAmount = productMarginer.GiftAmount * productMarginer.CurrentPrice / salePriceTotalTmp.Value;
            }

            productMarginer.SalePrice = productMarginer.CurrentPrice;
            return CalcGrossMarginRate(productMarginer);
        }

        /// <summary>
        /// 赠品活动中获取赠品的毛利率
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual decimal GetSaleGift_GiftItemGrossMarginRate(ProductInfo product, SaleGiftDiscountBelongType discountBelongType)
        {
            ProductMarginer productMarginer = new ProductMarginer();
            productMarginer.ProductSysNo = product.SysNo;
            productMarginer.Point = Convert.ToDecimal(product.ProductPriceInfo.Point);
            productMarginer.UnitCost = product.ProductPriceInfo.UnitCost;
            productMarginer.CurrentPrice = product.ProductPriceInfo.CurrentPrice.Value;
            productMarginer.CouponAmount = 0.00m;
            productMarginer.GiftAmount = 0.00m;
            productMarginer.SalePrice = productMarginer.CurrentPrice;
            if (discountBelongType == SaleGiftDiscountBelongType.BelongMasterItem)
            {
                return CalcGrossMarginRate(productMarginer);
            }
            return CalcGiftGrossMarginRate(productMarginer);
        }

        /// <summary>
        /// 计算单个商品的毛利率:含Coupon，SaleGift的计算
        /// </summary>
        /// <param name="myProduct"></param>
        /// <returns></returns>
        public virtual decimal CalcGrossMarginRate(ProductMarginer productMarginer)
        {
            #region Old Calc
            //if ((productMarginer.CurrentPrice - productMarginer.Point / pointExchangeRate) == 0)
            //{
            //    return 0.00m;
            //}
            //decimal calcValue = Math.Round((
            //    (productMarginer.SalePrice - productMarginer.UnitCost - productMarginer.Point / pointExchangeRate - productMarginer.CouponAmount - productMarginer.GiftAmount)
            //    /
            //    (productMarginer.CurrentPrice - productMarginer.Point / pointExchangeRate)
            //    ), 4);
            //return calcValue;

            #endregion

            var disCount = productMarginer.CouponAmount + productMarginer.GiftAmount;
            ////毛利率计算统一归入IM
            decimal calcValue = ObjectFactory<IIMBizInteract>.Instance.GetProductMargin(
                                    productMarginer.SalePrice, decimal.ToInt32(productMarginer.Point), productMarginer.UnitCost, disCount);

            return Math.Round(calcValue, 4);
        }


        /// <summary>
        /// 计算单个赠品品的毛利率:含Coupon，SaleGift的计算
        /// </summary>
        /// <param name="myProduct"></param>
        /// <returns></returns>
        public virtual decimal CalcGiftGrossMarginRate(ProductMarginer productMarginer)
        {
            #region Old Calc
            //if ((productMarginer.CurrentPrice - productMarginer.Point / pointExchangeRate) == 0)
            //{
            //    return 0.00m;
            //}
            //decimal calcValue = Math.Round((
            //    (productMarginer.SalePrice - productMarginer.UnitCost - productMarginer.Point / pointExchangeRate - productMarginer.CouponAmount - productMarginer.GiftAmount)
            //    /
            //    (productMarginer.CurrentPrice - productMarginer.Point / pointExchangeRate)
            //    ), 4);
            //return calcValue;

            #endregion

            ////毛利率计算统一归入IM
            decimal calcValue = ObjectFactory<IIMBizInteract>.Instance.GetGiftProductMargin(
                                    productMarginer.SalePrice, decimal.ToInt32(productMarginer.Point), productMarginer.UnitCost, 0m);

            return Math.Round(calcValue, 4);
        }
    }


    public class ProductMarginer
    {
        public int ProductSysNo;
        public decimal CurrentPrice;
        public decimal UnitCost;
        public decimal Point;
        public decimal CouponAmount;
        public decimal GiftAmount;
        public decimal SalePrice;
    }
}
