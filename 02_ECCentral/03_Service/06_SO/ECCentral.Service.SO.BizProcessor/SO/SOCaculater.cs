using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.SO.BizProcessor.SO
{
    /// <summary>
    /// 订单价格计算
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "General", "Calc" })]
    public class SOCaculator
    {
        public SOCaculator()
        {
            IsUpdate = false;
        }
        public ISODA SODA = ObjectFactory<ISODA>.Instance;
        public ISOItemDA SOItemDA = ObjectFactory<ISOItemDA>.Instance;
        public bool IsUpdate { get; set; }
        public SOInfo OriginalSOInfo { get; set; }

        bool m_isManualPrice = false;

        /// <summary>
        /// 订单价格计算
        /// </summary>
        public virtual void Calculate(SOInfo soInfo)
        {
            m_isManualPrice = soInfo.Items.Exists(p => !string.IsNullOrEmpty(p.AdjustPriceReason));
            //手动改价走批发
            soInfo.BaseInfo.IsWholeSale = m_isManualPrice;
            //计算明细价格
            CalculateItemPrice(soInfo);


            //计算总积分和总金额
            CalculatePointAndAmt(soInfo);

            //计算促销(销售规则，优惠券，百货类赠品规则)
            CalculatePromotion(soInfo);

            //计算关税
            CalculateTariffAmt(soInfo);

            //计算运费
            CalculateShipPrice(soInfo);

            //计算保价费
            CalculatePrimiumAmt(soInfo);

            //计算积分支付
            CalculatePointPayAmt(soInfo);

            //计算现金支付
            //CalculateCashPay(soInfo);

            //计算礼品卡支付金额
            CalculateGiftCardPay(soInfo);

            //计算现金帐户支付金额
            CalculatePrePay(soInfo);

            //计算手续费
            CalculatePayPrice(soInfo);

            //计算毛利分配
            //CalculateItemGrossProfit(soInfo);
        }

        /// <summary>
        /// 计算关税
        /// </summary>
        /// <param name="soInfo"></param>
        private void CalculateTariffAmt(SOInfo soInfo)
        {
            //获取商品价格信息
            var soItems = soInfo.Items.Where(item => item.ProductType == SOProductType.Accessory
                                                     || item.ProductType == SOProductType.Award
                                                     || item.ProductType == SOProductType.Gift
                                                     || item.ProductType == SOProductType.Product
                                                     || item.ProductType == SOProductType.SelfGift);

            List<int> itemSysNos = soItems.Select<SOItemInfo, int>(item => item.ProductSysNo.Value).ToList<int>();

            //<商品编号，关税>
            //List<KeyValuePair<int, decimal>> productTaxList = ExternalDomainBroker.GetProductTax(itemSysNos);

            foreach (SOItemInfo soItem in soItems)
            {
                //soItem.TariffAmount = (((soItem.OriginalPrice - soItem.CouponAverageDiscount)* soItem.Quantity.Value) + soItem.PromotionAmount) * soItem.TariffRate ; 
                //soItem.TariffAmount = (soItem.OriginalPrice - soItem.CouponAverageDiscount + (soItem.PromotionAmount / soItem.Quantity.Value)) * soItem.TariffRate;//扣除优惠算关税
                soItem.TariffAmount = (soItem.OriginalPrice + (soItem.PromotionAmount / soItem.Quantity.Value)) * soItem.TariffRate;
                soItem.TariffAmount = decimal.Round(soItem.TariffAmount.Value, 2, MidpointRounding.AwayFromZero);
            }

            soInfo.BaseInfo.TariffAmount = soInfo.Items.Sum(item => item.TariffAmount.GetValueOrDefault() * item.Quantity.GetValueOrDefault());

            //一票货物的关税税额在人民币50元以下的免税。 
            if (soInfo.BaseInfo.TariffAmount <= 50m)
            {
                foreach (SOItemInfo soItem in soItems)
                {
                    soItem.TariffAmount = 0m;
                }
                soInfo.BaseInfo.TariffAmount = 0m;
            }
        }

        /// <summary>
        /// 计算手续费
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculatePayPrice(SOInfo soInfo)
        {
            decimal payPriceBase = 0;
            PayType ptInfo = ExternalDomainBroker.GetPayTypeBySysNo(soInfo.BaseInfo.PayTypeSysNo.Value);
            if (ptInfo != null)
            {
                payPriceBase = soInfo.BaseInfo.CashPay
                        + soInfo.BaseInfo.ShipPrice.Value
                        + soInfo.BaseInfo.PromotionAmount.Value
                        + soInfo.BaseInfo.PremiumAmount.Value
                        + soInfo.BaseInfo.TariffAmount.Value
                        - soInfo.BaseInfo.PrepayAmount.Value
                        - soInfo.BaseInfo.GiftCardPay.Value;
                soInfo.BaseInfo.PayPrice = decimal.Round(ptInfo.PayRate.Value * payPriceBase, 2);
            }
            if (soInfo.BaseInfo.PayPrice <= 0)
            {
                soInfo.BaseInfo.PayPrice = null;
            }
        }

        /// <summary>
        /// 计算余额支付
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculatePrePay(SOInfo soInfo)
        {
            if (!soInfo.BaseInfo.IsUsePrePay.Value)
            {
                soInfo.BaseInfo.PrepayAmount = null;
                return;
            }
#warning 需要再考虑修改订单

            //获取客户可用余额
            decimal validPrePay = ExternalDomainBroker.GetCustomerInfo(soInfo.BaseInfo.CustomerSysNo.Value).ValidPrepayAmt.Value;
            if (this.IsUpdate)
            {
                validPrePay += this.OriginalSOInfo.BaseInfo.PrepayAmount.Value;
            }

            //可供余额支付的订单金额
            decimal needPayAmt = soInfo.BaseInfo.CashPay
                    + soInfo.BaseInfo.ShipPrice.Value
                    + soInfo.BaseInfo.PremiumAmount.Value
                    + soInfo.BaseInfo.PromotionAmount.Value
                    + soInfo.BaseInfo.TariffAmount.Value
                    - soInfo.BaseInfo.GiftCardPay.Value;

            //取"可供支付金额"与"客户可用余额"中较小值做为实际余额支付金额
            soInfo.BaseInfo.PrepayAmount = Math.Min(validPrePay, needPayAmt);
            if (soInfo.BaseInfo.PrepayAmount == 0)
            {
                soInfo.BaseInfo.PrepayAmount = null;
            }
        }

        /// <summary>
        /// 计算礼品卡支付金额
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculateGiftCardPay(SOInfo soInfo)
        {
            //如果使用礼品卡
            if (soInfo.BaseInfo.IsUseGiftCard.Value)
            {
                soInfo.BaseInfo.GiftCardPay = null;
                if (soInfo.SOGiftCardList.Count == 0)
                {
                    return;
                }
                //计算可供礼品卡支付的订单金额
                decimal needPayAmt = soInfo.BaseInfo.CashPay
                    + soInfo.BaseInfo.ShipPrice.Value
                    + soInfo.BaseInfo.PremiumAmount.Value
                    + soInfo.BaseInfo.PromotionAmount.Value;

                for (int i = 0; i < soInfo.SOGiftCardList.Count; i++)
                {
                    if (needPayAmt <= 0)
                    {
                        soInfo.SOGiftCardList.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (soInfo.SOGiftCardList[i].Amount.HasValue && soInfo.SOGiftCardList[i].Amount > 0)
                    {
                        soInfo.BaseInfo.GiftCardPay += soInfo.SOGiftCardList[i].Amount;
                        needPayAmt -= soInfo.SOGiftCardList[i].Amount.Value;
                    }
                    else
                    {
                        decimal Amount = 0M;
                        //当前卡足够支付，直接在当前卡中减掉使用额度
                        if (soInfo.SOGiftCardList[i].AvailAmount >= needPayAmt)
                        {
                            Amount = needPayAmt;
                            needPayAmt = 0;
                        }
                        else if (soInfo.SOGiftCardList[i].AvailAmount != 0)
                        {
                            Amount = soInfo.SOGiftCardList[i].AvailAmount;
                            needPayAmt -= Amount;
                        }
                        soInfo.BaseInfo.GiftCardPay += Amount;
                    }
                }
            }
            else
            {
                soInfo.BaseInfo.GiftCardPay = null;
                soInfo.SOGiftCardList.Clear();
            }
        }

        /// <summary>
        /// 计算积分支付金额
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculatePointPayAmt(SOInfo soInfo)
        {
            //礼品卡订单不能使用积分支付
            if (soInfo.BaseInfo.SOType == SOType.ElectronicCard
                || soInfo.BaseInfo.SOType == SOType.PhysicalCard)
            {
                soInfo.BaseInfo.PointPay = null;
                return;
            }

            //如果本单支付积分不足最低积分，将本单支付积分自动更新为最低积分
            //如果本单支付积分超出最大值，将本单支付积分自动更新为最大值
            //此处不对客户帐户积分做判断，留在后续的update中进行
            int minPoint = 0;
            int maxPoint = 0;

            foreach (SOItemInfo soItem in soInfo.Items)
            {
                if (soItem.ProductType == SOProductType.Product)
                {
                    decimal pointToMoneyRatio = ExternalDomainBroker.GetPointToMoneyRatio();
                    if (soItem.PayType == ProductPayType.PointOnly)
                    {
                        minPoint += Convert.ToInt32((soItem.Price * soItem.Quantity + soItem.PromotionAmount) * pointToMoneyRatio);
                        maxPoint += Convert.ToInt32((soItem.Price * soItem.Quantity + soItem.PromotionAmount) * pointToMoneyRatio);
                    }
                    else if (soItem.PayType == ProductPayType.All)
                    {
                        maxPoint += Convert.ToInt32((soItem.Price * soItem.Quantity + soItem.PromotionAmount) * pointToMoneyRatio);
                    }
                }
            }

            if (minPoint > soInfo.BaseInfo.PointPay)
            {
                soInfo.BaseInfo.PointPay = minPoint;
            }
            else if (maxPoint < soInfo.BaseInfo.PointPay)
            {
                soInfo.BaseInfo.PointPay = maxPoint;
            }
            if (soInfo.BaseInfo.PointPay <= 0)
            {
                soInfo.BaseInfo.PointPay = null;
                soInfo.BaseInfo.PointPayAmount = null;
            }
        }

        /// <summary>
        /// 计算保价费
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculatePrimiumAmt(SOInfo soInfo)
        {
            //如果是电子卡，值为0
            if (soInfo.BaseInfo.SOType == SOType.ElectronicCard)
            {
                soInfo.BaseInfo.PremiumAmount = 0;
                return;
            }

            ShippingType st = ExternalDomainBroker.GetShippingTypeBySysNo(soInfo.ShippingInfo.ShipTypeSysNo.Value);

            if (soInfo.BaseInfo.IsPremium.Value && st != null)
            {
                if (soInfo.ShippingInfo != null
                    && soInfo.BaseInfo.SOAmount > 0
                    && soInfo.BaseInfo.SOAmount > st.PremiumBase)
                {
                    soInfo.BaseInfo.PremiumAmount = decimal.Round((soInfo.BaseInfo.SOAmount.Value * st.PremiumRate.Value), 2);

                    if (soInfo.BaseInfo.PremiumAmount == 0)
                    {
                        soInfo.BaseInfo.PremiumAmount = null;
                    }
                    return;
                }
            }
            soInfo.BaseInfo.PremiumAmount = null;
        }

        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculateShipPrice(SOInfo soInfo)
        {
            //电子卡订单运费为零
            if (soInfo.BaseInfo.SOType == SOType.ElectronicCard)
            {
                soInfo.BaseInfo.ShipPrice = null;
                return;
            }

            //手工设置运费
            if (soInfo.BaseInfo.ManualShipPrice.HasValue && soInfo.BaseInfo.ManualShipPrice > 0)
            {
                soInfo.BaseInfo.ShipPrice = soInfo.BaseInfo.ManualShipPrice.Value;
                return;
            }

            //计算订单中商品单件最大重量
            int soSingelMaxWeight = SOCommon.GetSOSingleMaxWeight(soInfo.Items);
            int soTotalWeight = SOCommon.GetTotalWeight(soInfo.Items);

#warning 这里注销掉了关于Ozzo优先逻辑
            //bool isUseDiscount = false;
            soInfo.BaseInfo.ShipPrice = SODA.CaclShipPrice(
                    soTotalWeight
                    , soInfo.BaseInfo.SOAmount
                    , soInfo.ShippingInfo.ShipTypeSysNo
                    , soInfo.ReceiverInfo.AreaSysNo
                    , soInfo.BaseInfo.CustomerSysNo
                    , soSingelMaxWeight
                    , soInfo.BaseInfo.CompanyCode);

            soInfo.BaseInfo.ShipPrice = decimal.Round(soInfo.BaseInfo.ShipPrice.Value, 2);
            if (soInfo.BaseInfo.ShipPrice == 0)
            {
                soInfo.BaseInfo.ShipPrice = null;
            }
        }

        protected virtual void CalculateVendorGift(SOInfo soInfo)
        {
            //找出所有的GiftVendor的promotion
            if (soInfo.SOPromotions.Count > 0)
            {
                var vendorPromotions = soInfo.SOPromotions.FindAll(x => (x.PromotionType == SOPromotionType.VendorGift));
                if (vendorPromotions.Count > 0)
                {
                    List<SOItemInfo> itemsInSO = SerializationUtility.DeepClone<List<SOItemInfo>>(soInfo.Items);

                    //删除没有主商品和赠品的Promotion
                    for (int i = vendorPromotions.Count - 1; i >= 0; i--)
                    {
                        if (!itemsInSO.Exists(item => item.ProductSysNo == vendorPromotions[i].MasterList[0].ProductSysNo)
                            || !itemsInSO.Exists(x => vendorPromotions[i].GiftList.Exists(y => y.ProductSysNo == x.ProductSysNo)))
                        {
                            soInfo.SOPromotions.Remove(vendorPromotions[i]);
                            vendorPromotions.RemoveAt(i);
                        }
                    }

                    //计算每个Promotion的最大赠品数量
                    foreach (var promotion in vendorPromotions)
                    {
                        promotion.SOSysNo = soInfo.SysNo;
                        //主商品
                        var master = itemsInSO.First(item => item.ProductSysNo == promotion.MasterList[0].ProductSysNo);
                        //次数
                        int times = master.Quantity.Value;

                        foreach (var giftInPromotion in promotion.GiftList)
                        {
                            //赠品的最大数量
                            int Ratio = SOItemDA.GetRatioOfGift(master.ProductSysNo.Value, promotion.PromotionSysNo.Value, giftInPromotion.ProductSysNo);
                            int maxGiftQty = times * Ratio;

                            //找到SO里对应赠品
                            var giftInSO = itemsInSO.FirstOrDefault(x => x.ProductSysNo == giftInPromotion.ProductSysNo);

                            //跟新Promotional的实际赠品数量
                            int realQty = giftInSO == null ? 0 : giftInSO.Quantity >= maxGiftQty ? maxGiftQty : giftInSO.Quantity.Value;
                            giftInPromotion.Quantity = realQty;
                            //扣去被占用的数量
                            giftInSO.Quantity -= realQty;
                        }
                    }

                    //没有主商品的赠品
                    var noMasterGifts = itemsInSO.Where(item => item.Quantity > 0 && item.ProductType == SOProductType.Gift);

                    //移除这些赠品
                    foreach (var noMasterGift in noMasterGifts)
                    {
                        for (int i = soInfo.Items.Count - 1; i >= 0; i--)
                        {
                            if (soInfo.Items[i].ProductSysNo == noMasterGift.ProductSysNo)
                            {
                                int adjustQty = soInfo.Items[i].Quantity.Value - noMasterGift.Quantity.Value;

                                if (adjustQty > 0)
                                    soInfo.Items[i].Quantity = adjustQty;
                                else
                                    soInfo.Items.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 调用促销引擎计算促销折扣
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculatePromotion(SOInfo soInfo)
        {
            //电子卡和实物卡Portal放在Portal计算
            if (soInfo.BaseInfo.SOType == SOType.ElectronicCard
                || soInfo.BaseInfo.SOType == SOType.PhysicalCard)
            {
                soInfo.BaseInfo.CouponAmount = null;
                return;
            }

            //修改订单时，如果没有改变优惠券，则不重新验证和计算优惠券信息
            bool isModifyCoupons = true;
            if (IsUpdate
                && OriginalSOInfo.SOPromotions != null
                && OriginalSOInfo.SOPromotions.Count > 0
                && soInfo.SOPromotions != null
                && soInfo.SOPromotions.Count > 0)
            {
                var currentCoupon = soInfo.Items.FirstOrDefault(item =>
                {
                    return item.ProductType == SOProductType.Coupon;
                });
                int tmpCouponSysNo = currentCoupon != null ? currentCoupon.ProductSysNo.Value : 0;

                if (tmpCouponSysNo > 0)
                {
                    var orgCoupon = OriginalSOInfo.Items.FirstOrDefault(item =>
                    {
                        return item.ProductType == SOProductType.Coupon;
                    });
                    int couponSysNo = orgCoupon != null ? orgCoupon.ProductSysNo.Value : 0;

                    if (couponSysNo == tmpCouponSysNo)
                    {
                        soInfo.SOPromotions = OriginalSOInfo.SOPromotions;
                        isModifyCoupons = false;
                    }
                }
            }

            var newPromotionCalculates = ExternalDomainBroker.CalculateSOPromotion(soInfo, isModifyCoupons);
            soInfo.SOPromotions.RemoveAll(p => p.PromotionType != SOPromotionType.VendorGift);
            soInfo.SOPromotions.AddRange(newPromotionCalculates);

            //SOItemInfo的PromotionAmount 需要先初始化为0
            soInfo.BaseInfo.PromotionAmount = 0;
            soInfo.Items.ForEach(p => p.PromotionAmount = 0);
            if (soInfo.SOPromotions != null
                && soInfo.SOPromotions.Count > 0)
            {
                #region 如果有调价，将不使用的规则

                bool isManualPrice = soInfo.Items.Exists(p => p.AdjustPrice != 0 && !string.IsNullOrEmpty(p.AdjustPriceReason));
                //绑定
                if (isManualPrice)
                {
                    soInfo.SOPromotions = soInfo.SOPromotions.FindAll(p => p.PromotionType != SOPromotionType.Combo);
                }

                #endregion

                soInfo.Items.ForEach(item =>
                {
                    item.CouponAverageDiscount = (from p in soInfo.SOPromotions
                                                  from d in p.SOPromotionDetails
                                                  where d.MasterProductSysNo == item.ProductSysNo && p.PromotionType == SOPromotionType.Coupon
                                                  select d.DiscountAmount.Value).Sum() / item.Quantity.Value;

                    item.Price = (item.OriginalPrice - item.CouponAverageDiscount).Round(2);

                    item.PromotionAmount = (from p in soInfo.SOPromotions
                                            from d in p.SOPromotionDetails
                                            where d.MasterProductSysNo == item.ProductSysNo && p.PromotionType != SOPromotionType.Coupon
                                            select d.DiscountAmount.Value).Sum();
                    item.PromotionAmount = -item.PromotionAmount.Round(2);
                });

                foreach (var couponItem in soInfo.Items.Where(p => p.ProductType == SOProductType.Coupon))
                {
                    couponItem.Price = 0;
                    couponItem.CostPrice = 0;
                }

                //电子卡和实物卡
                if (soInfo.BaseInfo.SOType == SOType.ElectronicCard
                    || soInfo.BaseInfo.SOType == SOType.PhysicalCard)
                {
                    soInfo.BaseInfo.PromotionAmount = soInfo.SOPromotions
                    .Sum(item => item.DiscountAmount);

                    soInfo.BaseInfo.CouponAmount = null;
                    return;
                }

                //统计销售规则,销售立减总折扣
                soInfo.BaseInfo.PromotionAmount = soInfo.SOPromotions
                    .Where(item => item.PromotionType == SOPromotionType.Combo || item.PromotionType == SOPromotionType.SaleDiscountRule)
                    .Sum(item => item.DiscountAmount);

                if (soInfo.BaseInfo.PromotionAmount == 0)
                {
                    soInfo.BaseInfo.PromotionAmount = null;
                }

                //统计优惠券总折扣
                soInfo.BaseInfo.CouponAmount = soInfo.SOPromotions
                    .Where(item => item.PromotionType == SOPromotionType.Coupon)
                    .Sum(item => item.DiscountAmount);

                if (soInfo.BaseInfo.CouponAmount == 0)
                {
                    soInfo.BaseInfo.CouponAmount = null;
                }
            }

            //赠品信息计算
            CalculateVendorGift(soInfo);

        }

        private void RemovePromotionFromSO(SOInfo soInfo)
        {
            OriginalSOInfo.SOPromotions.Clear();
            soInfo.SOPromotions.Clear();
        }

        /// <summary>
        /// 计算总积分及总金额
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculatePointAndAmt(SOInfo soInfo)
        {
            //计算商品赠送的总积分数
            soInfo.BaseInfo.GainPoint = soInfo.Items.Sum(item => item.GainPoint);

            //计算订单中商品总金额
            soInfo.BaseInfo.SOAmount = decimal.Round(soInfo.Items.Sum(item =>
            {
                if (item.ProductType == SOProductType.Product
                    || item.ProductType == SOProductType.ExtendWarranty)
                {
                    return item.OriginalPrice * item.Quantity;
                }
                return 0;
            }).Value, 2);
        }

        /// <summary>
        /// 计算商品最终售价
        /// 原则：取最优客户价格
        /// </summary>
        /// <param name="soInfo"></param>
        public virtual void CalculateItemPrice(SOInfo soInfo)
        {
            //礼品卡订单不获取价格
            if (soInfo.BaseInfo.SOType == SOType.ElectronicCard
                || soInfo.BaseInfo.SOType == SOType.PhysicalCard)
            {
                soInfo.Items.ForEach(item =>
                {
                    item.GainAveragePoint = 0;
                    item.PayType = ProductPayType.MoneyOnly;

                });

                return;
            }

            //获取商品价格信息
            List<int> itemSysNos = soInfo.Items.Where(item => item.ProductType == SOProductType.Accessory
                                                     || item.ProductType == SOProductType.Award
                                                     || item.ProductType == SOProductType.Gift
                                                     || item.ProductType == SOProductType.Product
                                                     || item.ProductType == SOProductType.SelfGift)
                                               .Select<SOItemInfo, int>(item => item.ProductSysNo.Value).ToList<int>();

            List<ProductInfo> productInfoList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(itemSysNos);
            if (productInfoList != null && productInfoList.Count > 0)
            {
                foreach (var item in soInfo.Items)
                {
                    //普通商品处理逻辑
                    if (item.ProductType == SOProductType.Product)
                    {

                        ProductPriceInfo priceInfo = null;
                        ProductInfo productInfoTemp = productInfoList.Find(x => x.SysNo == item.ProductSysNo);
                        if (productInfoTemp != null)
                        {
                            priceInfo = productInfoTemp.ProductPriceInfo;
                        }
                        if (priceInfo != null)
                        {
                            item.CostPrice = priceInfo.UnitCost;
                            item.PayType = priceInfo.PayType;//Vantal Update 属性名称修改
                            item.GainAveragePoint = priceInfo.Point;
                            item.OriginalPrice = priceInfo.CurrentPrice;

                            CustomerRank customerRank = ExternalDomainBroker.GetCustomerRank(soInfo.BaseInfo.CustomerSysNo.Value);

#warning 批发与手动修改价格逻辑需要再处理, 更新订单逻辑也需要考虑

                            decimal tempWholeSalePrice = 0.00M;
                            decimal tempMemberPrice = 0.00M;
                            //如果商品支持批发价，商品售价取最优批发价格
                            if (priceInfo.ProductWholeSalePriceInfo != null && priceInfo.ProductWholeSalePriceInfo.Count > 0)
                            {
                                //取最优批发价
                                priceInfo.ProductWholeSalePriceInfo.OrderBy(l => l.Level)
                                         .ToList()
                                         .ForEach(w =>
                                         {
                                             if (item.Quantity >= w.Qty)
                                             {
                                                 tempWholeSalePrice = w.Price.Value;
                                             }
                                         });
                            }

                            //优先使用会员价
                            priceInfo.ProductRankPrice.OrderBy(r => r.Rank)
                                .ToList()
                                .ForEach(r =>
                                {
                                    if (r.Status.HasValue
                                        && r.Status == ProductRankPriceStatus.Active
                                        && customerRank >= r.Rank)
                                    {
                                        tempMemberPrice = r.RankPrice.Value;
                                    }
                                });
                            if (tempWholeSalePrice != 0)
                            {
                                item.OriginalPrice = Math.Min(tempWholeSalePrice, item.OriginalPrice.Value);
                                item.PriceType = SOProductPriceType.WholeSale;
                            }
                            if (tempMemberPrice != 0)
                            {
                                if (tempMemberPrice <= tempWholeSalePrice)
                                {
                                    item.OriginalPrice = Math.Min(tempMemberPrice, item.OriginalPrice.Value);
                                    item.PriceType = SOProductPriceType.Member;
                                }
                            }
                            if (item.PriceType == SOProductPriceType.WholeSale)
                            {
                                //批发无积分
                                item.GainAveragePoint = 0;
                            }

                            if (m_isManualPrice)
                            {
                                //重新计算Discount
                                if (item.Price_End > 0)
                                {
                                    item.AdjustPrice = item.OriginalPrice.Value - item.Price_End;
                                    //初始化调价
                                    item.Price_End = 0;
                                }
                                item.OriginalPrice = item.OriginalPrice - item.AdjustPrice;
                            }

                            //更新状态购物车的单件商品没有修改数量按原始价格计算
                            if (IsUpdate)
                            {
                                var originalItem = OriginalSOInfo.Items.Find(oitem => oitem.ProductSysNo == item.ProductSysNo);
                                if (originalItem != null && item.Quantity == originalItem.Quantity)
                                {
                                    //如果手动改价
                                    if (item.AdjustPrice != originalItem.AdjustPrice)
                                    {
                                        item.OriginalPrice = (originalItem.OriginalPrice ?? 0) + originalItem.AdjustPrice - item.AdjustPrice;
                                        //continue;
                                    }
                                }
                            }
                            item.Price = item.OriginalPrice;
                        }
                    }
                    //优惠券和延保不赠积分
                    else if (item.ProductType == SOProductType.Coupon)
                    {
                        item.GainAveragePoint = 0;
                        item.PayType = ProductPayType.All;
                    }
                    else if (item.ProductType == SOProductType.ExtendWarranty)
                    {
                        item.OriginalPrice = item.Price;
                        item.GainAveragePoint = 0;
                        item.PayType = ProductPayType.All;
                        //数量
                        var masterProduct = soInfo.Items.First(p => p.ProductSysNo.ToString() == item.MasterProductSysNo);
                        item.Quantity = masterProduct.Quantity;
                    }
                    //是赠品,附件，奖品则价格为0, 积分为0
                    else
                    {
                        ProductPriceInfo priceInfo = null;
                        ProductInfo productInfoTemp = productInfoList.Find(x => x.SysNo == item.ProductSysNo);
                        if (productInfoTemp != null)
                        {
                            priceInfo = productInfoTemp.ProductPriceInfo;
                        }
                        if (priceInfo != null)
                        {
                            item.CostPrice = priceInfo.UnitCost;
                        }
                        item.OriginalPrice = 0;
                        item.Price = 0;
                        item.PayType = ProductPayType.All;
                    }
                }
            }

            if (m_isManualPrice)
            {
                if (IsUpdate)
                {
                    foreach (var item in soInfo.Items)
                    {
                        SOItemInfo oldSOItem = OriginalSOInfo.Items.Find(oldItem => oldItem.ProductSysNo == item.ProductSysNo);

                        if (oldSOItem != null)
                        {
                            if (!(item.PromotionAmount == 0 && string.IsNullOrEmpty(item.AdjustPriceReason)) && !(item.Price_End != item.Price))
                            {
                                item.OriginalPrice = item.OriginalPrice + oldSOItem.PromotionAmount - item.PromotionAmount;
                            }
                        }
                        item.Price = item.OriginalPrice;
                    }
                }
                else if (soInfo.BaseInfo.SOType == SOType.Gift)
                {
                    //赠品订单做补偿时使用
                    soInfo.Items.ForEach(item =>
                    {
                        item.PromotionAmount = item.OriginalPrice;
                        item.AdjustPriceReason = "赠品订单自动调整";
                        item.OriginalPrice = 0;
                        item.Price = 0;

                    });
                }
            }
        }
    }
}
