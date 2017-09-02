using System;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 审核订单
    /// </summary>
    [VersionExport(typeof(SOPriceSpliter))]
    public class SOPriceSpliter
    {

        private ISODA _soDA;
        protected ISODA SODA
        {
            get
            {
                _soDA = _soDA ?? ObjectFactory<ISODA>.Instance;
                return _soDA;
            }
        }
        private ISOPriceDA _soPriceDA;
        protected ISOPriceDA SOPriceDA
        {
            get
            {
                _soPriceDA = _soPriceDA ?? ObjectFactory<ISOPriceDA>.Instance;
                return _soPriceDA;
            }
        }


        public SOInfo CurrentSO
        {
            get;
            set;
        }

        /// <summary>
        /// 当前订单编号
        /// </summary>
        protected int SOSysNo
        {
            get
            {
                return CurrentSO.SysNo.Value;
            }
        }

        public List<SOPriceMasterInfo> SplitSO()
        {
            bool isCombine = CurrentSO.ShippingInfo.IsCombine.HasValue ? CurrentSO.ShippingInfo.IsCombine.Value : false;
            decimal currenSOInvocieAmount = SOCommon.CalculateInvoiceAmount(CurrentSO.BaseInfo.CashPay, CurrentSO.BaseInfo.PremiumAmount.Value, CurrentSO.BaseInfo.ShipPrice.Value, CurrentSO.BaseInfo.PayPrice.Value, CurrentSO.BaseInfo.PromotionAmount.Value, CurrentSO.BaseInfo.GiftCardPay.Value, CurrentSO.BaseInfo.PayWhenReceived.Value);
            decimal currenSOReceivableAmount = CurrentSO.BaseInfo.ReceivableAmount;
            List<SOPriceMasterInfo> soPriceList = new List<SOPriceMasterInfo>();
            if (CurrentSO.Items == null || CurrentSO.Items.Count == 0)
            {
                return soPriceList;//BackOrder订单存在0 item的情况
            }

            //排序的目的是为了最后一个商品的价格是最大的，不会因为减去折扣失败，导致重新计算
            List<SOItemInfo> soItemList = CurrentSO.Items.OrderBy(item => item.Price)
                                                .Select(item => item)
                                                .ToList();

            SOItemInfo soLastSOItem = soItemList.Last();
            SOItemInfo soLastSOItemNoExtend = soItemList.Where(item => (item.ProductType != SOProductType.ExtendWarranty
                                                            && item.ProductType != SOProductType.Coupon)).Last();

            //拆分逻辑
            decimal itemSumValue = CurrentSO.Items.Sum(x => x.Price.Value * x.Quantity.Value) + CurrentSO.BaseInfo.PromotionAmount.Value;

            //计算Item总重量，因为延保和优惠券Weight为0，这里不用排除
            decimal totalWeight = CurrentSO.Items.Sum(x => x.Weight.Value * x.Quantity.Value);

            decimal residual_CashPay = CurrentSO.BaseInfo.CashPay;
            decimal residual_PayPrice = CurrentSO.BaseInfo.PayPrice.Value;
            decimal residual_PointPay = CurrentSO.BaseInfo.PointPayAmount.Value;
            decimal residual_PremiumAmt = CurrentSO.BaseInfo.PremiumAmount.Value;
            decimal residual_PrepayAmt = CurrentSO.BaseInfo.PrepayAmount.Value;
            decimal residual_ShippingCharge = CurrentSO.BaseInfo.ShipPrice.Value;
            decimal residual_GiftCardPay = CurrentSO.BaseInfo.GiftCardPay.Value;

            //找到所有优惠券
            List<SOPriceItemInfo> couponProductPriceInfoList = new List<SOPriceItemInfo>();

            //找到所有延保
            List<SOPriceItemInfo> extendWarrentyProductPriceInfoList = new List<SOPriceItemInfo>();

            soItemList.ForEach(item =>
            {
                SOPriceItemInfo priceItem = new SOPriceItemInfo();

                decimal itemRate = (item.Price.Value * item.Quantity.Value + item.PromotionAmount.Value) / (itemSumValue <= 0 ? 1 : itemSumValue);
                decimal itemRateForShippingCharge = item.Weight.Value * item.Quantity.Value / (totalWeight <= 0 ? 1 : totalWeight);

                priceItem.MasterProductSysNo = item.MasterProductSysNo;
                priceItem.PromotionAmount = item.PromotionAmount;
                priceItem.Price = item.Price;
                priceItem.OriginalPrice = item.OriginalPrice;
                priceItem.ProductSysNo = item.ProductSysNo;
                priceItem.ProductType = item.ProductType;
                priceItem.Quantity = item.Quantity;
                priceItem.CouponAmount = item.CouponAmount;
                priceItem.GainPoint = item.GainPoint;
                priceItem.MasterSysNo = 0;//暂时放仓库编号,作为分仓的中间变量

                //对非优惠券项进行分摊
                if (item.ProductType != SOProductType.Coupon)
                {
                    if (item != soLastSOItem)
                    {
                        priceItem.PayPrice = UtilityHelper.ToMoney(CurrentSO.BaseInfo.PayPrice.Value * itemRate);
                        residual_PayPrice -= priceItem.PayPrice.Value;

                        priceItem.PointPayAmount = UtilityHelper.ToMoney(CurrentSO.BaseInfo.PointPayAmount.Value * itemRate);
                        residual_PointPay -= priceItem.PointPayAmount.Value;

                        priceItem.PremiumAmount = UtilityHelper.ToMoney(CurrentSO.BaseInfo.PremiumAmount.Value * itemRate);
                        residual_PremiumAmt -= priceItem.PremiumAmount.Value;
                    }
                    else
                    {
                        priceItem.PayPrice = UtilityHelper.ToMoney(residual_PayPrice);
                        priceItem.PointPayAmount = UtilityHelper.ToMoney(residual_PointPay);
                        priceItem.PremiumAmount = UtilityHelper.ToMoney(residual_PremiumAmt);
                    }

                    if (item != soLastSOItemNoExtend)
                    {
                        priceItem.ShipPrice = UtilityHelper.ToMoney(CurrentSO.BaseInfo.ShipPrice.Value * itemRateForShippingCharge);
                        residual_ShippingCharge -= priceItem.ShipPrice.Value;
                    }
                    else
                    {
                        priceItem.ShipPrice = UtilityHelper.ToMoney(residual_ShippingCharge);
                    }

                    //计算现金支付
                    priceItem.CashPay = UtilityHelper.ToMoney(CalculateSOPriceItemCashPay(priceItem));

                    decimal priceRate_Prepay = (priceItem.CashPay.Value
                        + priceItem.PayPrice.Value
                        + priceItem.ShipPrice.Value
                        + priceItem.PromotionAmount.Value
                        + priceItem.PremiumAmount.Value)
                        / (CurrentSO.BaseInfo.SOTotalAmount <= 0 ? 1 : CurrentSO.BaseInfo.SOTotalAmount);

                    if (item != soLastSOItem)
                    {
                        priceItem.PrepayAmount = UtilityHelper.ToMoney(CurrentSO.BaseInfo.PrepayAmount.Value * priceRate_Prepay);
                        residual_PrepayAmt -= priceItem.PrepayAmount.Value;

                        priceItem.GiftCardPay = UtilityHelper.ToMoney(CurrentSO.BaseInfo.GiftCardPay.Value * priceRate_Prepay);
                        residual_GiftCardPay -= priceItem.GiftCardPay.Value;

                    }
                    else
                    {
                        priceItem.PrepayAmount = UtilityHelper.ToMoney(residual_PrepayAmt);
                        priceItem.GiftCardPay = UtilityHelper.ToMoney(residual_GiftCardPay);
                    }

                    priceItem.ExtendPrice = priceItem.OriginalPrice * priceItem.Quantity;
                    priceItem.InvoiceAmount = item.OriginalPrice * item.Quantity;
                }

                // 根据仓库编号将订单商品价格信息分组
                switch (item.ProductType.Value)
                {
                    case SOProductType.Product:
                    case SOProductType.Gift:
                    case SOProductType.Award:
                    case SOProductType.Accessory:
                    case SOProductType.SelfGift:
                        {
                            SOPriceMasterInfo soPriceInfo = soPriceList.Find((p) => { return p.StockSysNo == item.StockSysNo; });
                            if (soPriceInfo == null)
                            {
                                soPriceInfo = new SOPriceMasterInfo();
                                soPriceInfo.StockSysNo = item.StockSysNo;
                                soPriceList.Add(soPriceInfo);
                            }
                            soPriceInfo.Items.Add(priceItem);
                            break;
                        }
                    case SOProductType.Coupon:
                        couponProductPriceInfoList.Add(priceItem);
                        break;
                    case SOProductType.ExtendWarranty:
                        extendWarrentyProductPriceInfoList.Add(priceItem);
                        break;
                }
            });

            int i = 0;
            foreach (SOPriceMasterInfo soPriceInfo in soPriceList)
            {
                i++;
                //添加优惠券到每个分仓
                soPriceInfo.Items.AddRange(couponProductPriceInfoList);

                //延保跟随主商品
                extendWarrentyProductPriceInfoList.ForEach(item =>
                {
                    if (soPriceInfo.Items.Exists(o => o.ProductSysNo == int.Parse(item.MasterProductSysNo)))
                    {
                        soPriceInfo.Items.Add(item);
                    }
                });

                soPriceInfo.CashPay = soPriceInfo.Items.Sum(item => item.CashPay);
                soPriceInfo.PromotionAmount = soPriceInfo.Items.Sum(item => item.PromotionAmount);
                soPriceInfo.PayPrice = soPriceInfo.Items.Sum(item => item.PayPrice);
                soPriceInfo.PointPayAmount = soPriceInfo.Items.Sum(item => item.PointPayAmount);
                soPriceInfo.PremiumAmount = soPriceInfo.Items.Sum(item => item.PremiumAmount);
                soPriceInfo.PrepayAmount = soPriceInfo.Items.Sum(item => item.PrepayAmount);
                soPriceInfo.CouponAmount = soPriceInfo.Items.Sum(item => item.CouponAmount);
                soPriceInfo.ShipPrice = soPriceInfo.Items.Sum(item => item.ShipPrice);
                soPriceInfo.GainPoint = soPriceInfo.Items.Sum(item => item.GainPoint);
                soPriceInfo.GiftCardPay = soPriceInfo.Items.Sum(item => item.GiftCardPay);
                soPriceInfo.SOAmount = UtilityHelper.ToMoney(soPriceInfo.Items
                                                .Where(item => item.ProductType == SOProductType.Product
                                                            || item.ProductType == SOProductType.ExtendWarranty)
                                                .Sum(item => item.OriginalPrice.Value * item.Quantity.Value));

                decimal substactAmt = 0;

                //模式4，发票金额为0
                if (CurrentSO.InvoiceInfo.InvoiceType == InvoiceType.MET
                    && CurrentSO.ShippingInfo.StockType == StockType.SELF
                    && CurrentSO.ShippingInfo.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.SELF)
                {
                    foreach (SOPriceItemInfo subItem in soPriceInfo.Items)
                    {
                        substactAmt += subItem.Price.Value * subItem.Quantity.Value;
                        subItem.InvoiceAmount = 0;
                    }
                }


                //商家仓储，商家开票
                if (CurrentSO.ShippingInfo.StockType == StockType.MET
                    && CurrentSO.InvoiceInfo.InvoiceType == InvoiceType.MET)
                {
                    foreach (SOPriceItemInfo subItem in soPriceInfo.Items)
                    {
                        substactAmt += subItem.Price.Value * subItem.Quantity.Value;
                        subItem.InvoiceAmount = 0;
                    }
                }

                //代收代付的商品需减去InvocieAmt
                currenSOInvocieAmount -= substactAmt;

                if (i == soPriceList.Count)
                {
                    soPriceInfo.InvoiceAmount = CalculateInvoiceAmount(soPriceInfo, CurrentSO.BaseInfo.PayWhenReceived.Value, isCombine) - substactAmt;
                    currenSOInvocieAmount -= soPriceInfo.InvoiceAmount.Value;
                    soPriceInfo.ReceivableAmount = CalculateReceivableAmount(soPriceInfo, CurrentSO.BaseInfo.PayWhenReceived.Value, isCombine);
                    currenSOReceivableAmount -= soPriceInfo.ReceivableAmount.Value;
                }
                else
                {
                    soPriceInfo.InvoiceAmount = currenSOInvocieAmount;
                    soPriceInfo.ReceivableAmount = currenSOReceivableAmount;
                }

                soPriceInfo.InvoiceAmount = soPriceInfo.InvoiceAmount < 0 ? 0 : soPriceInfo.InvoiceAmount;

                soPriceInfo.SOSysNo = SOSysNo;
                soPriceInfo.Status = SOPriceStatus.Original;
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //初始化数据，如果存在则删除
                SOPriceDA.DeleteSOPriceBySOSysNo(SOSysNo);
                foreach (SOPriceMasterInfo priceInfo in soPriceList)
                {
                    SOPriceDA.InsertSOPrice(priceInfo, CurrentSO.CompanyCode);
                    foreach (SOPriceItemInfo priceItem in priceInfo.Items)
                    {
                        priceItem.MasterSysNo = priceInfo.SysNo;
                        SOPriceDA.InsertSOPriceItem(priceItem, SOSysNo, CurrentSO.CompanyCode);
                    }
                }
                scope.Complete();
            }

            return soPriceList;
        }

        /// <summary>
        /// 作废订单价格信息,这里仅作废订单价格信息。如果作废后要重新添加价格信息请直接调用方法：SplitSO()
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public void AbandonSOPrice()
        {
            SOPriceDA.AbandonSOPriceBySOSysNo(SOSysNo);
        }

        private decimal CalculateSOPriceItemCashPay(SOPriceItemInfo subSOItem)
        {
            decimal cash = (subSOItem.OriginalPrice.Value * subSOItem.Quantity.Value - subSOItem.CouponAmount.Value - subSOItem.PointPayAmount.Value);
            if (cash <= 0M)
            {
                cash = 0M;
            }
            return cash;
        }

        private decimal CalculateInvoiceAmount(SOPriceMasterInfo soInfo, bool isPayWhenReceived, bool isCombine)
        {
            decimal result = soInfo.CashPay.Value
                + soInfo.PremiumAmount.Value
                + soInfo.ShipPrice.Value
                + soInfo.PayPrice.Value
                + soInfo.PromotionAmount.Value
                - soInfo.GiftCardPay.Value;

            if (isPayWhenReceived && !isCombine)
            {
                result = UtilityHelper.TruncMoney(result);
            }
            return result;
        }

        private decimal CalculateReceivableAmount(SOPriceMasterInfo soInfo, bool isPayWhenReceived, bool isCombine)
        {
            decimal result = soInfo.CashPay.Value
                + soInfo.PremiumAmount.Value
                + soInfo.ShipPrice.Value
                + soInfo.PayPrice.Value
                + soInfo.PromotionAmount.Value
                - soInfo.GiftCardPay.Value
                - soInfo.PrepayAmount.Value;
            if (isPayWhenReceived && !isCombine)
            {
                result = UtilityHelper.TruncMoney(result);
            }
            return result;
        }
    }
}