using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
using ECommerce.DataAccess.Invoice;
using ECommerce.DataAccess.Product;
using ECommerce.DataAccess.SO;
using ECommerce.Entity.Common;
using ECommerce.Entity.Customer;
using ECommerce.Entity.Invoice;
using ECommerce.Entity.Product;
using ECommerce.Entity.SO;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Enums;
using ECommerce.Service.ControlPannel;
using ECommerce.Utility;
using ECommerce.WebFramework;
using ECommerce.Service.Invoice;
using ECommerce.Service.Customer;
using TransactionManager = ECommerce.Utility.TransactionManager;
using ECommerce.Entity.Inventory;
using ECommerce.Service.Product;
using ECommerce.DataAccess.Inventory;
using ECommerce.Service.Inventory;
using ECommerce.DataAccess;
using ECommerce.Service.Common;
using System.Data;
using ECommerce.WebFramework.Mail;
using StockInfo = ECommerce.Entity.ControlPannel.StockInfo;
using System.Collections;
using ECommerce.DataAccess.Customer;

namespace ECommerce.Service.SO
{
    public class SOService
    {
        #region private methods

        /// <summary>
        /// 重新计算订单的金额。
        /// </summary>
        /// <param name="soUpdateInfo"></param>
        private static void CalculateSOAmounts(SOUpdateInfo soUpdateInfo)
        {
            SOInfo orgSO = GetSOInfo(soUpdateInfo.SOSysNo);

            decimal soAmt = 0, soTariffAmt = 0;

            //根据单价的调整，计算关税
            foreach (var orgItem in orgSO.SOItemList)
            {
                var updateItem = soUpdateInfo.Items.Find(o => o.SysNo == orgItem.SysNo);
                if (updateItem != null)
                {
                    //计算OriginalPrice与TariffAmt（不处理优惠卷）
                    if (orgItem.ProductType != SOItemType.Coupon)
                    {
                        //如没有更新Price，就不重算关税（尽量使原始数据保持少变动为原则）
                        if (orgItem.Price != updateItem.Price)
                        {
                            // 通过OriginalPrice的差值来计算提到Price值的变化，好处是可以不用关心与DiscountAmt的计算方法。
                            updateItem.Price = orgItem.Price + (updateItem.OriginalPrice - orgItem.OriginalPrice);
                            //计算关税（海关需要按原始价格计算，所以会包含优惠卷和折扣）
                            updateItem.TariffAmt = decimal.Round(updateItem.OriginalPrice * orgItem.Quantity * orgItem.TariffRate,
                                2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            updateItem.Price = orgItem.Price;
                            updateItem.TariffAmt = orgItem.TariffAmt;
                        }
                    }

                    soTariffAmt += updateItem.TariffAmt;

                    //累加计算商品总金额（优惠卷商品此时不能参与计算）
                    if (orgItem.ProductType != SOItemType.Coupon)
                    {
                        soAmt += orgItem.Quantity * updateItem.OriginalPrice;
                    }
                }
            }

            //一票货物的关税税额在人民币50元以下的免税。 
            if (soTariffAmt <= 50)
            {
                soTariffAmt = 0; //关税
                soUpdateInfo.Items.ForEach(item => item.TariffAmt = 0);
            }

            // 最后的支付金额
            decimal realPayAmt = 0;
            realPayAmt = soAmt //商品总金额
                  + soUpdateInfo.ShipPrice //运费
                  - Math.Abs(orgSO.PromotionAmt) //优惠金额
                  - Math.Abs(orgSO.Amount.PrepayAmt) //余额支付
                  - Math.Abs(orgSO.Amount.PointPayAmt) //积分支付
                  - Math.Abs(orgSO.Amount.GiftCardPay) //礼品卡金额
                  - Math.Abs(orgSO.Amount.DiscountAmt) //折扣金额
                  + soTariffAmt;//关税


            //根据单价的调整，计算商品金额
            soUpdateInfo.SOAmt = soAmt;
            soUpdateInfo.CashPay = soUpdateInfo.SOAmt - Math.Abs(orgSO.PromotionAmt) - Math.Abs(orgSO.Amount.PointPayAmt);
            soUpdateInfo.TariffAmt = soTariffAmt;
            soUpdateInfo.RealPayAmt = realPayAmt;

            if (soUpdateInfo.SOAmt < 0)
                soUpdateInfo.SOAmt = 0;
            if (soUpdateInfo.CashPay < 0)
                soUpdateInfo.CashPay = 0;
        }

        /// <summary>
        /// 给VoidSo用的，创建负应收款及退款单。
        /// </summary>
        /// <param name="currentSO"></param>
        /// <param name="refundInfo"></param>
        /// <param name="user"></param>
        private static void CreateRefundAO(SOInfo currentSO, SOIncomeRefundInfo refundInfo, LoginUser user = null)
        {
            refundInfo.RefundCashAmt = currentSO.Amount.CashPay;
            SOIncomeInfo currentSOIncome;
            using (var ts = TransactionManager.SuppressTransaction())
            {
                currentSOIncome = SOIncomeDA.GetValidSOIncomeInfo(currentSO.SOSysNo, SOIncomeOrderType.SO);
            }

            if (currentSOIncome == null)
            {
                throw new BusinessException(LanguageHelper.GetText("该订单找不到财务收款单，是否为未支付？"));
            }

            var soIncomeInfo = new SOIncomeInfo
            {
                OrderAmt = -currentSOIncome.OrderAmt,
                OrderType = SOIncomeOrderType.AO,
                Note = LanguageHelper.GetText("作废订单,创建财务负收款单"),
                ReferenceID = "",
                Status = SOIncomeStatus.Origin,
                OrderSysNo = currentSO.SOSysNo,
                IncomeAmt = -(currentSOIncome.OrderAmt - currentSOIncome.PrepayAmt - currentSOIncome.GiftCardPayAmt),
                PayAmount = -(currentSOIncome.OrderAmt - currentSOIncome.PrepayAmt - currentSOIncome.GiftCardPayAmt),
                IncomeStyle = currentSOIncome.IncomeStyle,
                PrepayAmt = -currentSOIncome.PrepayAmt,
                GiftCardPayAmt = -currentSOIncome.GiftCardPayAmt,
                PointPay = -currentSOIncome.PointPay,
                CompanyCode = currentSO.CompanyCode,
                IncomeUserSysNo = user == null ? 0 : user.UserSysNo,
                IncomeUserName = user == null ? "" : user.UserDisplayName
            };

            var soIncomeRefundInfo = new SOIncomeRefundInfo
            {
                OrderSysNo = currentSO.SOSysNo,
                OrderType = RefundOrderType.AO,
                SOSysNo = currentSO.SOSysNo,
                RefundPayType = refundInfo.RefundPayType,
                BankName = refundInfo.BankName,
                BranchBankName = refundInfo.BranchBankName,
                CardNumber = refundInfo.CardNumber,
                CardOwnerName = refundInfo.CardOwnerName,
                PostAddress = refundInfo.PostAddress,
                PostCode = refundInfo.PostCode,
                ReceiverName = refundInfo.ReceiverName,
                Note = refundInfo.Note,
                HaveAutoRMA = false,
                RefundPoint = 0,
                RefundReason = refundInfo.RefundReason,
                CompanyCode = currentSO.CompanyCode,
                CreateUserSysNo = user == null ? 0 : user.UserSysNo,
                CreateUserName = user == null ? "" : user.UserDisplayName
            };

            soIncomeRefundInfo.Status = RefundStatus.Origin;
            soIncomeRefundInfo.RefundCashAmt = currentSOIncome.OrderAmt - currentSOIncome.GiftCardPayAmt;
            soIncomeRefundInfo.RefundGiftCard = currentSOIncome.GiftCardPayAmt;

            //ValidateAbandonSO(false);

            // 省略的：Hold检查：如果还没有出库（打印运单），需要先Hold定单。

            SOIncomeDA.CreateSOIncome(soIncomeInfo);
            //记录操作日志
            //ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
            //    GetMessageString("SOIncome_Log_Create", ServiceContext.Current.UserSysNo, entity.SysNo)
            //    , BizEntity.Common.BizLogType.Finance_SOIncome_Add
            //    , entity.SysNo.Value
            //    , entity.CompanyCode);

            SOIncomeRefundDA.Create(soIncomeRefundInfo);

        }

        private static void ValidateAbandonSO(bool isCheckSOIncome)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// 获取SO及商品清单
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="isFromWriteServer"></param>
        /// <returns></returns>
        public static SOInfo GetSOInfo(int soSysNo, bool isFromWriteServer = false)
        {
            //using (var ts = TransactionManager.Create(TransactionScopeOption.Suppress))
            using (var ts = TransactionManager.Create())
            {
                return SODA.GetSOBySysNo(soSysNo);
            }
        }

        /// <summary>
        /// 获取SO的商品清单
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static List<SOItemInfo> GetSOItemInfoList(int soSysNo)
        {
            var soInfo = SODA.GetSOBySysNo(soSysNo);
            if (soInfo != null)
            {
                return soInfo.SOItemList;
            }
            else
            {
                return new List<SOItemInfo>();
            }
        }

        /// <summary>
        /// 查询SO
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<SOInfo> SOQuery(SOQueryFilter queryFilter)
        {
            return SODA.SOQuery(queryFilter);
        }

        /// <summary>
        /// 订单审核通过
        /// </summary>
        /// <param name="soSysNos"></param>
        public static void AuditAccept(int[] soSysNos)
        {
            #region Precheck

            // check 是否为待审核状态
            var warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo.Status != SOStatus.Original)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            string warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有订单都符合条件, 某些订单不是待审核状态：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单不是待审核状态，不允许审核：{0}"), warningMsg.TrimEnd(',', ' '));
            }

            // check非货到付款的订单是否已经支付
            warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo.IsPayWhenRecv != 1 && soInfo.NetPayStatus != NetPayStatusType.Verified)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有在线支付已完成, 某些订单没有有效的支付记录，请支付后（如果已支付请审核支付后）再审核订单：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单还没有有效的支付记录，审核失败，请支付后（如果已支付请审核支付后）再审核订单：{0}"), warningMsg.TrimEnd(',', ' '));
            }
            #endregion

            foreach (int soSysNo in soSysNos)
            {
                AuditAccept(soSysNo);
            }
        }

        /// <summary>
        /// 订单审核通过
        /// </summary>
        /// <param name="soSysNo"></param>
        private static void AuditAccept(int soSysNo)
        {
            using (var ts = TransactionManager.Create())
            {
                SOStatus toStatus = SOStatus.WaitingOutStock;
                SODA.UpdateSOStatus(soSysNo, toStatus, DateTime.Now);
                SOInfo soInfo = GetSOInfo(soSysNo);
                //  审核订单后重新判断订单是否并单
                UpdateSOIsCombine(soSysNo);
                //  根据仓库拆分订单价格
                SplitPrice(soSysNo);
                SODA.WriteLog(soInfo, BizLogType.Sale_SO_Audit, "订单审核通过");
                ts.Complete();
            }
        }

        private static void SplitPrice(int soSysNo)
        {
            var CurrentSO = GetSOInfo(soSysNo, true);

            bool isCombine = CurrentSO.IsCombine.HasValue && CurrentSO.IsCombine.Value;
            decimal currenSOInvocieAmount = InvoicePriceService.CalculateInvoiceAmount(CurrentSO.Amount.CashPay,
                CurrentSO.Amount.PremiumAmt,
                CurrentSO.Amount.ShipPrice,
                CurrentSO.Amount.PayPrice,
                CurrentSO.PromotionAmt,
                CurrentSO.Amount.GiftCardPay,
                CurrentSO.IsPayWhenRecv == 1);
            decimal currenSOReceivableAmount = CurrentSO.ReceivableAmount;
            List<SOPriceMasterInfo> soPriceList = new List<SOPriceMasterInfo>();
            if (CurrentSO.SOItemList == null || CurrentSO.SOItemList.Count == 0)
            {
                return;//BackOrder订单存在0 item的情况
            }

            //排序的目的是为了最后一个商品的价格是最大的，不会因为减去折扣失败，导致重新计算
            List<SOItemInfo> soItemList = CurrentSO.SOItemList.OrderBy(item => item.Price)
                                                .Select(item => item)
                                                .ToList();

            SOItemInfo soLastSOItem = soItemList.Last();
            SOItemInfo soLastSOItemNoExtend = soItemList.Last(item => item.ProductType != SOItemType.ExtendWarranty && item.ProductType != SOItemType.Coupon);

            //拆分逻辑
            decimal itemSumValue = CurrentSO.SOItemList.Sum(x => x.Price * x.Quantity) + CurrentSO.PromotionAmt;

            //计算Item总重量，因为延保和优惠券Weight为0，这里不用排除
            decimal totalWeight = CurrentSO.SOItemList.Sum(x => x.Weight * x.Quantity);

            decimal residual_CashPay = CurrentSO.Amount.CashPay;
            decimal residual_PayPrice = CurrentSO.Amount.PayPrice;
            decimal residual_PointPay = CurrentSO.Amount.PointPayAmt;
            decimal residual_PremiumAmt = CurrentSO.Amount.PremiumAmt;
            decimal residual_PrepayAmt = CurrentSO.Amount.PrepayAmt;
            decimal residual_ShippingCharge = CurrentSO.Amount.ShipPrice;
            decimal residual_GiftCardPay = CurrentSO.Amount.GiftCardPay;

            //找到所有优惠券
            List<SOPriceItemInfo> couponProductPriceInfoList = new List<SOPriceItemInfo>();

            //找到所有延保
            List<SOPriceItemInfo> extendWarrentyProductPriceInfoList = new List<SOPriceItemInfo>();

            soItemList.ForEach(item =>
            {
                SOPriceItemInfo priceItem = new SOPriceItemInfo();

                decimal itemRate = (item.Price * item.Quantity + item.PromotionDiscount) / (itemSumValue <= 0 ? 1 : itemSumValue);
                decimal itemRateForShippingCharge = item.Weight * item.Quantity / (totalWeight <= 0 ? 1 : totalWeight);

                priceItem.MasterProductSysNo = item.MasterProductSysNo;
                priceItem.PromotionAmount = item.DiscountAmt;
                priceItem.Price = item.Price;
                priceItem.OriginalPrice = item.OriginalPrice;
                priceItem.ProductSysNo = item.ProductSysNo;
                priceItem.ProductType = item.ProductType;
                priceItem.Quantity = item.Quantity;
                priceItem.CouponAmount = item.PromotionDiscount;
                priceItem.GainPoint = item.Point * item.Quantity;
                priceItem.MasterSysNo = 0;//暂时放仓库编号,作为分仓的中间变量

                //对非优惠券项进行分摊
                if (item.ProductType != SOItemType.Coupon)
                {
                    if (item != soLastSOItem)
                    {
                        priceItem.PayPrice = UnifiedHelper.ToMoney(CurrentSO.Amount.PayPrice * itemRate);
                        residual_PayPrice -= priceItem.PayPrice.Value;

                        priceItem.PointPayAmount = UnifiedHelper.ToMoney(CurrentSO.Amount.PointPayAmt * itemRate);
                        residual_PointPay -= priceItem.PointPayAmount.Value;

                        priceItem.PremiumAmount = UnifiedHelper.ToMoney(CurrentSO.Amount.PremiumAmt * itemRate);
                        residual_PremiumAmt -= priceItem.PremiumAmount.Value;
                    }
                    else
                    {
                        priceItem.PayPrice = UnifiedHelper.ToMoney(residual_PayPrice);
                        priceItem.PointPayAmount = UnifiedHelper.ToMoney(residual_PointPay);
                        priceItem.PremiumAmount = UnifiedHelper.ToMoney(residual_PremiumAmt);
                    }

                    if (item != soLastSOItemNoExtend)
                    {
                        priceItem.ShipPrice = UnifiedHelper.ToMoney(CurrentSO.Amount.ShipPrice * itemRateForShippingCharge);
                        residual_ShippingCharge -= priceItem.ShipPrice.Value;
                    }
                    else
                    {
                        priceItem.ShipPrice = UnifiedHelper.ToMoney(residual_ShippingCharge);
                    }

                    //计算现金支付
                    priceItem.CashPay = UnifiedHelper.ToMoney(CalculateSOPriceItemCashPay(priceItem));

                    decimal priceRate_Prepay = (priceItem.CashPay.Value
                        + priceItem.PayPrice.Value
                        + priceItem.ShipPrice.Value
                        + priceItem.PromotionAmount.Value
                        + priceItem.PremiumAmount.Value)
                        / (CurrentSO.RealPayAmt <= 0 ? 1 : CurrentSO.RealPayAmt);

                    if (item != soLastSOItem)
                    {
                        priceItem.PrepayAmount = UnifiedHelper.ToMoney(CurrentSO.Amount.PrepayAmt * priceRate_Prepay);
                        residual_PrepayAmt -= priceItem.PrepayAmount.Value;

                        priceItem.GiftCardPay = UnifiedHelper.ToMoney(CurrentSO.Amount.GiftCardPay * priceRate_Prepay);
                        residual_GiftCardPay -= priceItem.GiftCardPay.Value;

                    }
                    else
                    {
                        priceItem.PrepayAmount = UnifiedHelper.ToMoney(residual_PrepayAmt);
                        priceItem.GiftCardPay = UnifiedHelper.ToMoney(residual_GiftCardPay);
                    }

                    priceItem.ExtendPrice = priceItem.OriginalPrice * priceItem.Quantity;
                    priceItem.InvoiceAmount = item.OriginalPrice * item.Quantity;
                }

                // 根据仓库编号将订单商品价格信息分组
                switch (item.ProductType)
                {
                    case SOItemType.Product:
                    case SOItemType.Gift:
                    case SOItemType.Award:
                    case SOItemType.Accessory:
                    case SOItemType.SelfGift:
                        {
                            SOPriceMasterInfo soPriceInfo = soPriceList.Find((p) => p.StockSysNo == item.WarehouseNumber.ToString());
                            if (soPriceInfo == null)
                            {
                                soPriceInfo = new SOPriceMasterInfo();
                                soPriceInfo.StockSysNo = item.WarehouseNumber.ToString();
                                soPriceList.Add(soPriceInfo);
                            }
                            soPriceInfo.Items.Add(priceItem);
                            break;
                        }
                    case SOItemType.Coupon:
                        couponProductPriceInfoList.Add(priceItem);
                        break;
                    case SOItemType.ExtendWarranty:
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
                soPriceInfo.SOAmount = UnifiedHelper.ToMoney(soPriceInfo.Items
                                                .Where(item => item.ProductType == SOItemType.Product
                                                            || item.ProductType == SOItemType.ExtendWarranty)
                                                .Sum(item => item.OriginalPrice.Value * item.Quantity.Value));

                decimal substactAmt = 0;

                //模式4，发票金额为0
                if (CurrentSO.InvoiceType == VendorInvoiceType.MET
                    && CurrentSO.StockType == ShippingStockType.NEG
                    && CurrentSO.ShippingType == VendorShippingType.NEG)
                {
                    foreach (SOPriceItemInfo subItem in soPriceInfo.Items)
                    {
                        substactAmt += subItem.Price.Value * subItem.Quantity.Value;
                        subItem.InvoiceAmount = 0;
                    }
                }


                //商家仓储，商家开票
                if (CurrentSO.StockType == ShippingStockType.MET
                    && CurrentSO.InvoiceType == VendorInvoiceType.MET)
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
                    soPriceInfo.InvoiceAmount = CalculateInvoiceAmount(soPriceInfo, CurrentSO.IsPayWhenRecv == 1, isCombine) - substactAmt;
                    currenSOInvocieAmount -= soPriceInfo.InvoiceAmount.Value;
                    soPriceInfo.ReceivableAmount = CalculateReceivableAmount(soPriceInfo, CurrentSO.IsPayWhenRecv == 1, isCombine);
                    currenSOReceivableAmount -= soPriceInfo.ReceivableAmount.Value;
                }
                else
                {
                    soPriceInfo.InvoiceAmount = currenSOInvocieAmount;
                    soPriceInfo.ReceivableAmount = currenSOReceivableAmount;
                }

                soPriceInfo.InvoiceAmount = soPriceInfo.InvoiceAmount < 0 ? 0 : soPriceInfo.InvoiceAmount;

                soPriceInfo.SOSysNo = soSysNo;
                soPriceInfo.Status = SOPriceStatus.Original;
            }

            using (var scope = TransactionManager.Create())
            {
                //初始化数据，如果存在则删除
                SOPriceDA.DeleteSOPriceBySOSysNo(soSysNo);
                foreach (SOPriceMasterInfo priceInfo in soPriceList)
                {
                    SOPriceDA.InsertSOPrice(priceInfo, CurrentSO.CompanyCode);
                    foreach (SOPriceItemInfo priceItem in priceInfo.Items)
                    {
                        priceItem.MasterSysNo = priceInfo.SysNo;
                        SOPriceDA.InsertSOPriceItem(priceItem, soSysNo, CurrentSO.CompanyCode);
                    }
                }
                scope.Complete();
            }
        }

        private static decimal CalculateInvoiceAmount(SOPriceMasterInfo soInfo, bool isPayWhenReceived, bool isCombine)
        {
            decimal result = soInfo.CashPay.Value
                + soInfo.PremiumAmount.Value
                + soInfo.ShipPrice.Value
                + soInfo.PayPrice.Value
                + soInfo.PromotionAmount.Value
                - soInfo.GiftCardPay.Value;

            if (isPayWhenReceived && !isCombine)
            {
                result = UnifiedHelper.TruncMoney(result);
            }
            return result;
        }

        private static decimal CalculateReceivableAmount(SOPriceMasterInfo soInfo, bool isPayWhenReceived, bool isCombine)
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
                result = UnifiedHelper.TruncMoney(result);
            }
            return result;
        }

        private static decimal CalculateSOPriceItemCashPay(SOPriceItemInfo subSOItem)
        {
            decimal cash = (subSOItem.OriginalPrice.Value * subSOItem.Quantity.Value - subSOItem.CouponAmount.Value - subSOItem.PointPayAmount.Value);
            if (cash <= 0M)
            {
                cash = 0M;
            }
            return cash;
        }

        private static void UpdateSOIsCombine(int soSysNo)
        {
            SODA.UpdateSOIsCombine(soSysNo);
        }

        /// <summary>
        /// check订单必须为非泰隆优选自贸仓（直邮或商家自贸仓）的订单。
        /// </summary>
        /// <param name="soSysNos"></param>
        private static void CheckIsNotKJTFTAStock(params int[] soSysNos)
        {
            var warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                if (IsStockFTAOfKJT(soSysNo))
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            string warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有订单都符合条件, 某些订单是泰隆优选自贸出库订单，不允许在此操作：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单是泰隆优选自贸出库订单，不允许在此操作：{0}"), warningMsg.TrimEnd(',', ' '));
            }
        }

        /// <summary>
        /// 订单批量作废
        /// </summary>
        /// <param name="soSysNos"></param>
        /// <param name="user"></param>
        public static void BatchVoidSO(int[] soSysNos, LoginUser user = null)
        {
            #region Precheck

            // check 是否已经出库
            var warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo.Status >= SOStatus.OutStock)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            string warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有订单都符合条件, 某些订单可能已经出库：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单已经出库，不能作废：{0}"), warningMsg.TrimEnd(',', ' '));
            }

            // check 是否已经作废
            warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if ((soInfo.Status >= SOStatus.SystemCancel && soInfo.Status <= SOStatus.Abandon)
                    || soInfo.Status == SOStatus.Reject
                    || soInfo.Status == SOStatus.CustomsReject)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有订单都符合条件, 某些订单可能已经作废：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单已经作废，不能重复作废：{0}"), warningMsg.TrimEnd(',', ' '));
            }

            // check 是否已经支付
            warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo.IsNetPayed == 1)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                throw new BusinessException(LanguageHelper.GetText("不允许对已有支付记录的订单进行批量作废，请进入订单详情页面进行作废，它们是：{0}"), warningMsg.TrimEnd(',', ' '));
            }
            #endregion

            using (var ts = TransactionManager.Create())
            {
                foreach (int soSysNo in soSysNos)
                {
                    VoidSO(soSysNo, null, loginUser: user);
                }

                ts.Complete();
            }
        }

        public static void BatchOutStock(int[] soSysNos)
        {
            SOStatus toStatus = SOStatus.OutStock;

            #region Precheck

            // check 是否为待出库状态
            var warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo.Status != SOStatus.WaitingOutStock)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            string warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有订单都符合条件, 某些订单不是待出库状态：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单不是待出库状态，不允许出库操作：{0}"), warningMsg.TrimEnd(',', ' '));
            }

            // check订单必须为非泰隆优选自贸仓（直邮或商家自贸仓）的订单。
            CheckIsNotKJTFTAStock(soSysNos);
            #endregion

            using (var ts = TransactionManager.Create())
            {
                foreach (int soSysNo in soSysNos)
                {
                    SOOutStockWaitReportRequest outStockWaitReportRequest = new SOOutStockWaitReportRequest { SOSysNo = soSysNo };
                    OutStockWaitReport(outStockWaitReportRequest);
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// 是否为泰隆优选自贸仓
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        private static bool IsStockFTAOfKJT(int soSysNo)
        {
            var soInfo = SODA.GetSOBySysNo(soSysNo);
            var stockSysNo = soInfo.StockSysNo;
            var stockInfo = stockSysNo.HasValue ? StockService.LoadStock(stockSysNo.Value) : null;

            if (stockInfo != null// && stockInfo.StockType == TradeType.FTA
                && (stockInfo.SellerSysNo == null || stockInfo.SellerSysNo == 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void BatcReported(int[] soSysNos)
        {
            SOStatus toStatus = SOStatus.Reported;

            #region Precheck

            // check 是否为已出库待申报状态
            var warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo.Status != SOStatus.OutStock)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            string warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有订单都符合条件, 某些订单不是已出库待申报：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单不是已出库待申报状态，不允许申报操作：{0}"), warningMsg.TrimEnd(',', ' '));
            }

            // check订单必须为非泰隆优选自贸仓（直邮或商家自贸仓）的订单。

            #endregion

            using (var ts = TransactionManager.Create())
            {
                foreach (int soSysNo in soSysNos)
                {
                    SODA.UpdateSOStatus(soSysNo, toStatus);
                    SOInfo currentSOInfo = GetSOInfo(soSysNo);
                    SODA.WriteLog(currentSOInfo, BizLogType.Sale_SO_Reported, EnumHelper.GetDescription(toStatus));
                }
                ts.Complete();
            }
        }

        public static void BatchCustomsPass(int[] soSysNos)
        {
            #region Precheck

            // check 是否为已出库待申报状态
            var warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo.Status != SOStatus.Reported)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            string warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有订单都符合条件, 某些订单不是已申报待通关：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单不是已申报待通关状态，不允许申报操作：{0}"), warningMsg.TrimEnd(',', ' '));
            }

            // check订单必须为非泰隆优选自贸仓（直邮或商家自贸仓）的订单。

            #endregion

            using (var ts = TransactionManager.Create())
            {
                foreach (int soSysNo in soSysNos)
                {
                    SOInfo currentSOInfo = GetSOInfo(soSysNo);
                    OrderOutstockSuccess(currentSOInfo);
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// 订单作废
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="refundInfo"></param>
        public static void VoidSO(int soSysNo, SOIncomeRefundInfo refundInfo, SOStatus targetSOStatus = SOStatus.Abandon, LoginUser loginUser = null)
        {
            SOInfo currentSO = null;
            //using (var ts = TransactionManager.Create(TransactionScopeOption.Suppress))
            using (var ts = TransactionManager.Create())
            {
                currentSO = SODA.GetSOBySysNo(soSysNo);
            }

            #region Precheck

            // check 是否满足作废的状态要求            
            if (targetSOStatus == SOStatus.Abandon)
            {
                if (currentSO.Status >= SOStatus.OutStock)
                {
                    throw new BusinessException(LanguageHelper.GetText(string.Format("该订单已经出库，不能作废:{0}!", soSysNo)));
                }
            }
            else if (targetSOStatus == SOStatus.Reject)
            {
                if (currentSO.Status != SOStatus.OutStock)
                {
                    throw new BusinessException(LanguageHelper.GetText(string.Format("该订单不是已出库待申报，不能做申报失败订单作废：{0}!", soSysNo)));
                }
            }
            else if (targetSOStatus == SOStatus.CustomsReject)
            {
                if (currentSO.Status != SOStatus.Reported)
                {
                    throw new BusinessException(LanguageHelper.GetText(string.Format("该订单不是已申报待通关，不能做通关失败订单作废：{0}!", soSysNo)));
                }
            }
            else
            {
                throw new BusinessException(LanguageHelper.GetText("传入订单非法处理参数！"));
            }

            // check 是否已经作废
            if (currentSO.Status == SOStatus.SystemCancel
                || currentSO.Status == SOStatus.Abandon
                || currentSO.Status == SOStatus.Reject
                || currentSO.Status == SOStatus.CustomsReject)
            {
                throw new BusinessException(LanguageHelper.GetText(string.Format("该订单已经作废，不能重复作废：{0}!", soSysNo)));
            }

            #endregion

            using (var ts = TransactionManager.Create())
            {
                // 更新状态
                SODA.UpdateSOStatus(soSysNo, targetSOStatus);

                // 回滚库存
                SODA.RollbackStockInventoryForVoidSO(soSysNo);    // 回滚分仓库存
                SODA.RollbackTotalInventoryForVoidSO(soSysNo);    // 回滚总库存

                // 优惠券重置

                // 取消使用延保
                CancelExtendWarranty(soSysNo);

                // 客户积分处理，返还客户支付的积分
                CancelPointPay(currentSO);

                // 礼品卡支付部分处理
                CancelGiftCardPay(currentSO);

                // 生成退款单
                if (currentSO.IsNetPayed == 1 && refundInfo != null)
                {
                    CreateRefundAO(currentSO, refundInfo, user: loginUser);
                }

                // 写SOLog:
                SODA.WriteLog(currentSO, BizLogType.Sale_RMA_Abandon2, "订单作废");

                // 发短信: 作废（已支付、申报失败、通关失败）：  
                // 不发短信: 作废（未支付）
                if ((currentSO.IsNetPayed == 1 && refundInfo != null) || targetSOStatus != SOStatus.Abandon)
                {
                    SMSService.SendSMSForSO(soSysNo, SMSType.OrderAbandon);
                }

                ts.Complete();
            }
        }

        private static void CancelExtendWarranty(int soSysNo)
        {
            SODA.CancelSOExtendWarranty(soSysNo);
        }

        private static void CancelGiftCardPay(SOInfo soInfo)
        {

            //xmlMessage
            List<GiftCard> GiftCardsElement = new List<GiftCard>(){
                new GiftCard()
                {                    
                    ReferenceSOSysNo= soInfo.SOSysNo,                    
        }
            };
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "VoidSO",
                    Version = "V1",
                    From = "SellerPortal",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = soInfo.CompanyCode,
                    StoreCompanyCode = soInfo.StoreCompanyCode
                },
                Body = new ECommerce.Entity.Product.Body()
        {
            EditUser = "",
            GiftCard = GiftCardsElement
        }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            GiftCardDA.OperateGiftCard(paramXml);
        }

        private static void CancelPointPay(SOInfo soInfo)
        {
            if (soInfo.PointPay > 0)
            {
                CustomerService.AdjustPoint(new AdjustPointRequest
        {
            CustomerSysNo = soInfo.CustomerSysNo,
            Point = new int?(Convert.ToInt32(soInfo.PointPay)),
            PointType = (int)AdjustPointType.AbandonSO,
            SOSysNo = soInfo.SOSysNo,
            Memo = LanguageHelper.GetText("作废订单返还积分"),
            Source = "SO",
            OperationType = AdjustPointOperationType.Abandon
        }, 0);
            }
        }

        /// <summary>
        /// 获取退款原因列表
        /// </summary>
        /// <returns>退款原因列表</returns>
        public static List<Entity.Common.CodeNamePair> GetRefundReasons()
        {
            return SOIncomeRefundDA.GetRefundReasons();
        }

        /// <summary>
        /// 保存订单的修改。
        /// </summary>
        /// <param name="soUpdateInfo"></param>
        public static void SOUpdate(SOUpdateInfo soUpdateInfo)
        {
            CalculateSOAmounts(soUpdateInfo);
            SODA.SOUpdate(soUpdateInfo);
            SODA.SOCheckShippingUpdateShippingFee(soUpdateInfo);
            //TODO LOG: SODA.SOLogInsert(soLogInfo);
        }

        /// <summary>
        /// 订单在修改过程中，对计算的金额等进行计算并实时反馈给用户
        /// </summary>
        /// <param name="soUpdateInfo"></param>
        /// <returns></returns>
        public static SOUpdateInfo SOUpdatePreview(SOUpdateInfo soUpdateInfo)
        {
            CalculateSOAmounts(soUpdateInfo);
            return soUpdateInfo;
        }

        public static QueryResult AOQuery(AOQueryFilter queryFilter)
        {
            return SODA.AOQuery(queryFilter);
        }

        public static SOIncomeRefundInfo GetAORefundInfo(int soSysNo)
        {
            return SOIncomeRefundDA.GetValidSOIncomeRefundInfo(soSysNo, SOIncomeOrderType.AO);
        }

        public static void AORefund(int soSysNo, int sellerSysNo, int userSysNo)
        {
            SOInfo soInfo = GetSOInfo(soSysNo);
            if (soInfo == null)
            {
                throw new BusinessException(LanguageHelper.GetText("订单不存在"));
            }
            if (soInfo.MerchantSysNo != sellerSysNo)
            {
                throw new BusinessException(LanguageHelper.GetText("此订单不属于当前商家，不允许操作。").ToString());
            }
            SOIncomeInfo soIncomeInfo = SOIncomeDA.GetValidSOIncomeInfo(soInfo.SOSysNo, SOIncomeOrderType.AO);
            if (soIncomeInfo == null)
            {
                throw new BusinessException(LanguageHelper.GetText("订单退款信息不存在"));
            }

            SOIncomeRefundInfo soIncomeRefundInfo = SOIncomeRefundDA.GetValidSOIncomeRefundInfo(soInfo.SOSysNo, SOIncomeOrderType.AO);
            if (soIncomeRefundInfo == null)
            {
                throw new BusinessException(LanguageHelper.GetText("订单退款信息不存在"));
            }

            if (soIncomeInfo.Status == SOIncomeStatus.Confirmed || soIncomeRefundInfo.Status == RefundStatus.Refunded)
            {
                throw new BusinessException(LanguageHelper.GetText("订单已退款，不能再次退款"));
            }

            //取得当前退款状态
            RefundStatus orgRefundStatus = soIncomeRefundInfo.Status.Value;

            if (soIncomeRefundInfo.RefundPayType == RefundPayType.NetWorkRefund)
            {
                if (orgRefundStatus != RefundStatus.Origin && orgRefundStatus != RefundStatus.Processing)
                {
                    throw new BusinessException(LanguageHelper.GetText("网关直退的退款单不是“待退款”或“退款中”状态，不能退款"));
                }
                //发起银行网关退款
                ProcessNetWorkRefund(soIncomeInfo, soInfo);
                soIncomeRefundInfo.Status = RefundStatus.Processing;
                soIncomeInfo.Status = SOIncomeStatus.Processing;
            }
            else
            {
                if (orgRefundStatus != RefundStatus.Origin)
                {
                    throw new BusinessException(LanguageHelper.GetText("退款单不是“待退款”状态，不能退款"));
                }
                soIncomeRefundInfo.Status = RefundStatus.Refunded;
                soIncomeInfo.Status = SOIncomeStatus.Confirmed;
            }

            using (ITransaction trans = TransactionManager.Create())
            {
                if (orgRefundStatus == RefundStatus.Origin)
                {
                    //更新客户累计购买金额
                    if (soIncomeInfo.OrderAmt != 0)
                    {
                        CustomerService.UpdateCustomerOrderedAmt(soIncomeInfo.CustomerSysNo.Value, soIncomeInfo.OrderAmt.Value);
                    }
                }

                if (soIncomeInfo.Status == SOIncomeStatus.Confirmed)
                {
                    soIncomeInfo.ConfirmUserSysNo = userSysNo;
                    soIncomeInfo.ConfirmTime = DateTime.Now;
                }
                else
                {
                    soIncomeInfo.ConfirmUserSysNo = null;
                    soIncomeInfo.ConfirmTime = null;
                }
                SOIncomeDA.UpdateSOIncomeStatus(soIncomeInfo);

                if (soIncomeRefundInfo.Status == RefundStatus.Refunded)
                {
                    soIncomeRefundInfo.AuditUserSysNo = userSysNo;
                    soIncomeRefundInfo.AuditTime = DateTime.Now;
                }
                else
                {
                    soIncomeRefundInfo.AuditUserSysNo = null;
                    soIncomeRefundInfo.AuditTime = null;
                }
                soIncomeRefundInfo.EditUserSysNo = userSysNo;
                soIncomeRefundInfo.EditDate = DateTime.Now;
                SOIncomeRefundDA.UpdateSOIncomeRefundStatus(soIncomeRefundInfo);

                trans.Complete();
            }
        }

        private static RefundResult ProcessNetWorkRefund(SOIncomeInfo entity, SOInfo soInfo)
        {
            var result = new RefundResult();
            var biz = new EasiPayUtils();
            var refundEntity = new RefundEntity
            {
                SOSysNo = soInfo.SOSysNo,
                RefundSysNo = entity.OrderSysNo.Value,
                RefundAmt = Math.Abs(entity.IncomeAmt.Value),
                CompanyCode = entity.CompanyCode,
                OrderDate = soInfo.OrderDate.Value
            };
            refundEntity.ProductAmount = Math.Abs(entity.IncomeAmt.Value)
                                           - soInfo.Amount.ShipPrice
                                           - soInfo.TariffAmt;
            refundEntity.TaxFeeAmount = soInfo.TariffAmt;
            refundEntity.ShippingFeeAmount = soInfo.Amount.ShipPrice;

            result = biz.Refund(refundEntity);

            return result;
        }

        #region [订单出库确认，接口调用相关方法:]

        /// <summary>
        /// 订单出库确认成功
        /// </summary>
        /// <param name="soSysNo"></param>
        public static SOInfo OrderOutstockSuccess(SOInfo currentSOInfo)
        {

            //修改订单状态，调整库存，创建代销转财务记录:
            if (null == currentSOInfo || currentSOInfo.SOSysNo <= 0)
            {
                throw new BusinessException("订单号不存在!");
            }
            if (currentSOInfo.Status != SOStatus.Reported)
            {
                throw new BusinessException(string.Format("SO单{0}的状态不是“已申报待通关”状态，不能出库确认成功操作！", currentSOInfo.SOSysNo));
            }

            int userSysno = 0;
            using (var scope = TransactionManager.Create())
            {

                SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
                {
                    SOSysNo = currentSOInfo.SOSysNo,
                    ChangeTime = DateTime.Now,
                    OldStatus = currentSOInfo.Status,
                    Status = SOStatus.CustomsPass,
                    OperatorSysNo = userSysno,
                    OperatorType = userSysno == 0 ? SOOperatorType.System : SOOperatorType.User
                };
                //修改订单状态
                SODA.UpdateSOStatusToOutStock(statusChangeInfo);
                currentSOInfo.Status = SOStatus.CustomsPass; //设置出库状态


                ////如果是商家配送将不记日志
                ////添加商家出库日志表
                //metShipViaCode = metShipViaCode ?? "";
                //if (!string.IsNullOrEmpty(metShipViaCode))
                //{
                //    if (metShipViaCode.Length > 50)
                //    {
                //        metShipViaCode = metShipViaCode.Substring(0, 50);
                //    }
                //    SOLogDA.InsertMerchantShippingLog(soInfo.SysNo.Value, ServiceContext.Current.UserSysNo, metShipViaCode, metPackageNumber);
                //}

                List<InventoryAdjustItemInfo> adjustItemList = new List<InventoryAdjustItemInfo>();
                foreach (SOItemInfo soItem in currentSOInfo.SOItemList)
                {
                    switch (soItem.ProductType)
                    {
                        case SOItemType.Product:
                        case SOItemType.Gift:
                        case SOItemType.Award:
                        case SOItemType.SelfGift:
                        case SOItemType.Accessory:
                            adjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = soItem.Quantity,
                                ProductSysNo = soItem.ProductSysNo,
                                StockSysNo = currentSOInfo.StockSysNo.Value
                            });
                            break;
                        case SOItemType.Coupon:
                        case SOItemType.ExtendWarranty:
                            break;
                    }
                }
                //调整库存:
                foreach (InventoryAdjustItemInfo adjustItem in adjustItemList)
                {
                    ProductQueryInfo productInfo = ProductService.GetProductBySysNo(adjustItem.ProductSysNo);
                    if (productInfo == null || productInfo.SysNo <= 0)
                    {
                        throw new BusinessException(string.Format("欲调库存的商品不存在，商品编号：{0}", adjustItem.ProductSysNo));
                    }
                    InventoryDA.InitProductInventoryInfo(adjustItem.ProductSysNo, adjustItem.StockSysNo);
                    var inventoryType = InventoryDA.GetProductInventroyType(adjustItem.ProductSysNo);
                    ECommerce.Entity.Inventory.ProductInventoryInfo stockInventoryCurrentInfo = InventoryDA.GetProductInventoryInfoByStock(adjustItem.ProductSysNo, adjustItem.StockSysNo);
                    ECommerce.Entity.Inventory.ProductInventoryInfo totalInventoryCurrentInfo = InventoryDA.GetProductTotalInventoryInfo(adjustItem.ProductSysNo);

                    ECommerce.Entity.Inventory.ProductInventoryInfo stockInventoryAdjustInfo = new Entity.Inventory.ProductInventoryInfo()
                    {
                        ProductSysNo = adjustItem.ProductSysNo,
                        StockSysNo = adjustItem.StockSysNo
                    };

                    ECommerce.Entity.Inventory.ProductInventoryInfo totalInventoryAdjustInfo = new ECommerce.Entity.Inventory.ProductInventoryInfo()
                    {
                        ProductSysNo = adjustItem.ProductSysNo
                    };
                    stockInventoryAdjustInfo.OrderQty = -adjustItem.AdjustQuantity;
                    totalInventoryAdjustInfo.OrderQty = -adjustItem.AdjustQuantity;

                    // 如果为泰隆优选自贸仓，才调整代销库存、可用库存
                    if (IsStockFTAOfKJT(currentSOInfo.SOSysNo))
                    {
                        stockInventoryAdjustInfo.ConsignQty = -adjustItem.AdjustQuantity;
                        totalInventoryAdjustInfo.ConsignQty = -adjustItem.AdjustQuantity;

                        stockInventoryAdjustInfo.AvailableQty = adjustItem.AdjustQuantity;
                        totalInventoryAdjustInfo.AvailableQty = adjustItem.AdjustQuantity;
                    }

                    //预检调整后的商品库存是否合法  
                    Entity.Inventory.ProductInventoryInfo stockInventoryAdjustAfterAdjust = InventoryService.PreCalculateInventoryAfterAdjust(stockInventoryCurrentInfo, stockInventoryAdjustInfo);
                    Entity.Inventory.ProductInventoryInfo totalInventoryAdjustAfterAdjust = InventoryService.PreCalculateInventoryAfterAdjust(totalInventoryCurrentInfo, totalInventoryAdjustInfo);

                    bool isNeedCompareAvailableQtyAndAccountQty = true;
                    InventoryService.PreCheckGeneralRules(stockInventoryAdjustAfterAdjust, ref isNeedCompareAvailableQtyAndAccountQty);
                    InventoryService.
                        PreCheckGeneralRules(totalInventoryAdjustAfterAdjust, ref isNeedCompareAvailableQtyAndAccountQty);

                    //调整商品库存:
                    InventoryDA.AdjustProductStockInventoryInfo(stockInventoryAdjustInfo);
                    InventoryDA.AdjustProductTotalInventoryInfo(totalInventoryAdjustInfo);

                }

                #region 更新客户等级以及积分经验值
                //增加客户经验值
                //更新客户等级            
                //调整客户经验值（内部修改客户等级）
                int customerSysNo = currentSOInfo.CustomerSysNo;
                decimal adjustValue = currentSOInfo.RealPayAmt;// + soInfo.BaseInfo.PayPrice.Value + currentSOInfo.BaseInfo.ShipPrice.Value + currentSOInfo.BaseInfo.PremiumAmount.Value + currentSOInfo.BaseInfo.PromotionAmount.Value;

                string logMemo = string.Format("SO#{0}:购物加经验值。", currentSOInfo.SOSysNo);

                CustomerService.AdjustCustomerExperience(customerSysNo, adjustValue, ExperienceLogType.MerchantSOOutbound, logMemo);

                //给款到发货用户加积分
                //CustomerService.AddPointForCustomer(currentSOInfo);
                #endregion 更新客户等级以及积分经验值

                #region [财务应收 - 创建收款单]

                SOIncomeInfo soIncomeInfo = null;
                using (TransactionScope ts2 = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    soIncomeInfo = SOIncomeService.GetValidSOIncomeInfo(currentSOInfo.SOSysNo, SOIncomeOrderType.SO);
                    if (soIncomeInfo == null)
                    {
                        soIncomeInfo = new SOIncomeInfo();
                        soIncomeInfo.OrderType = SOIncomeOrderType.SO;
                        soIncomeInfo.OrderSysNo = currentSOInfo.SOSysNo;
                        soIncomeInfo.OrderAmt = currentSOInfo.Amount.SOAmt;
                        soIncomeInfo.IncomeAmt = currentSOInfo.RealPayAmt;
                        soIncomeInfo.PrepayAmt = Math.Max(currentSOInfo.Amount.PrepayAmt, 0);
                        soIncomeInfo.IncomeStyle = SOIncomeOrderStyle.Normal;
                        soIncomeInfo.IncomeUserSysNo = 0;
                        soIncomeInfo.Status = SOIncomeStatus.Origin;
                        soIncomeInfo.GiftCardPayAmt = currentSOInfo.Amount.GiftCardPay;
                        soIncomeInfo.PointPay = currentSOInfo.Amount.PointPayAmt;
                        soIncomeInfo.PayAmount = currentSOInfo.RealPayAmt;
                        //if (currentSOInfo..BaseInfo.SOSplitMaster.HasValue)
                        //{
                        //    soIncomeInfo.MasterSoSysNo = currentSOInfo.BaseInfo.SOSplitMaster;  //获取母单号
                        //}
                        SOIncomeService.CreateSOIncome(soIncomeInfo);

                    }
                    ts2.Complete();
                }

                #endregion

                #region [调用InvoiceSync:]
                if (!InvoiceDA.ExistInvoiceMaster(currentSOInfo.SOSysNo))
                {
                    InvoiceDA.SOOutStockInvoiceSync(currentSOInfo.SOSysNo, currentSOInfo.StockSysNo.Value, string.Empty, currentSOInfo.CompanyCode);
                }
                #endregion

                #region 发送邮件:
                SendEmailToCustomerForSOInfo(currentSOInfo, "SO_OutStock");
                #endregion

                #region 发送短信:

                SMSService.SendSMSForSO(currentSOInfo.SOSysNo, SMSType.OrderOutBound);

                //if (soInfo.InvoiceInfo.IsVAT.Value && soInfo.InvoiceInfo.InvoiceType == ECCentral.BizEntity.Invoice.InvoiceType.SELF)
                //{
                //    //增票提醒短信
                //    messageProcessor.SendVATSMS(soInfo);
                //    //发送增值税发票SSB 
                //    EventPublisher.Publish<ECCentral.Service.EventMessage.ImportVATSSBMessage>(new ECCentral.Service.EventMessage.ImportVATSSBMessage
                //    {
                //        SOSysNo = soInfo.SysNo.Value,
                //        StockSysNo = soInfo.Items[0].StockSysNo.Value,
                //        OrderType = EventMessage.ImportVATOrderType.SO
                //    });
                //}

                #endregion

                //写SOLog:
                SODA.WriteLog(currentSOInfo, BizLogType.Sale_SO_CustomsPass, "销售单已通关发往顾客");
                scope.Complete();
            }
            return currentSOInfo;
        }

        /// <summary>
        /// 订单出库确认失败:
        /// </summary>
        /// <param name="soSysNo"></param>
        public static SOInfo OrderOutstockFailed(SOInfo currentSOInfo, string failedMessage)
        {
            //修改订单状态:
            if (null == currentSOInfo || currentSOInfo.SOSysNo <= 0)
            {
                throw new BusinessException("订单号不存在!");
            }
            if (currentSOInfo.Status != SOStatus.Reported)
            {
                throw new BusinessException(string.Format("SO单{0}的状态不是“已申报待通关”状态，不能出库确认失败操作！", currentSOInfo.SOSysNo));
            }

            //生成退款单,生成AO:
            var getSONetPayInfo = NetPayService.GetValidNetPayBySOSysNo(currentSOInfo.SOSysNo);
            if (null != getSONetPayInfo)
            {
                SOIncomeRefundInfo refundInfo = new SOIncomeRefundInfo()
                {
                    SOSysNo = currentSOInfo.SOSysNo,
                    Note = string.Format("订单通关失败,自动作废订单。原因 :{0}", failedMessage),
                    RefundPayType = RefundPayType.NetWorkRefund,
                    RefundReason = 0
                };
                VoidSO(currentSOInfo.SOSysNo, refundInfo, SOStatus.CustomsReject);
                return currentSOInfo;
            }
            else
            {
                throw new BusinessException(string.Format("订单号:{0}没有NetPay记录。", currentSOInfo.SOSysNo));
            }
        }

        private static void SendEmailToCustomerForSOInfo(SOInfo soInfo, string emailTemplateID)
        {

            var customerInfo = CustomerService.GetCustomerInfo(soInfo.CustomerSysNo);
            string customerEmail = customerInfo.Email == null ? null : customerInfo.Email.Trim();

            if (customerEmail == null)
            {
                return;
            }
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            KeyTableVariables keyTableVariables = new KeyTableVariables();

            #region 填充基本属性
            keyValueVariables.Add("SOSysNo", soInfo.SOSysNo.ToString());
            keyValueVariables.Add("SOID", soInfo.SOSysNo.ToString());
            keyValueVariables.Add("CustomerName", customerInfo.CustomerName);
            keyValueVariables.Add("CustomerID", customerInfo.CustomerID);
            keyValueVariables.Add("OrderTime", soInfo.OrderDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            keyValueVariables.Add("InvoiceHeader", soInfo.ReceiveName);
            keyValueVariables.Add("ReceiveName", soInfo.ReceiveName);
            keyValueVariables.Add("ProvinceName", soInfo.ReceiveArea.ProvinceName);
            keyValueVariables.Add("CityName", soInfo.ReceiveArea.CityName);
            keyValueVariables.Add("DistrictName", soInfo.ReceiveArea.DistrictName);
            keyValueVariables.Add("ReceiveAddress", soInfo.ReceiveAddress);
            keyValueVariables.Add("ReceiveZip", soInfo.ReceiveZip);
            keyValueVariables.Add("ReceivePhone", String.IsNullOrEmpty(soInfo.ReceivePhone) ? soInfo.ReceiveCellPhone : soInfo.ReceivePhone);
            keyValueVariables.Add("PayType", soInfo.Payment.PayTypeName);
            keyValueVariables.Add("ShipType", soInfo.ShipType.ShipTypeName);
            var shippingType = CommonService.GetShippingTypeBySysNo(soInfo.ShipType.ShipTypeSysNo);
            keyValueVariables.Add("ShipPeriod", shippingType.Period);
            keyValueVariables.Add("CashPay", soInfo.Amount.CashPay.ToString("#########0.00"));
            keyValueVariables.Add("ShipPrice", soInfo.Amount.ShipPrice.ToString("#########0.00"));
            //保价费:
            keyValueVariables.Add("PremiumAmount", "0.00");
            keyValueVariables.Add("ReceivableAmount", soInfo.RealPayAmt.ToString("#########0.00"));
            keyValueVariables.Add("GainPoint", soInfo.Amount.PointAmt);
            keyValueVariables.Add("Weight", soInfo.Weight);

            string changeAmount = (soInfo.Amount.SOAmt - soInfo.RealPayAmt).ToString("#########0.00");
            keyValueVariables.Add("ChangeAmount", changeAmount);
            keyValueVariables.Add("ChangeAmountDisplay", changeAmount != (0M).ToString("#########0.00"));
            keyValueVariables.Add("GiftCardPay", soInfo.Amount.GiftCardPay.ToString("#########0.00"));
            keyValueVariables.Add("GiftCardDisplay", soInfo.Amount.GiftCardPay != 0m);
            keyValueVariables.Add("PointPay", soInfo.Amount.PointPayAmt);
            keyValueVariables.Add("PointPayDisplay", soInfo.Amount.PointPay != 0);
            keyValueVariables.Add("PrePay", soInfo.Amount.PrepayAmt.ToString("#########0.00"));
            keyValueVariables.Add("PrePayDisplay", soInfo.Amount.PrepayAmt != 0m);
            keyValueVariables.Add("PayPrice", soInfo.Amount.PayPrice.ToString("#########0.00"));
            keyValueVariables.Add("PayPriceDisplay", soInfo.Amount.PayPrice != 0m);

            keyValueVariables.Add("PromotionAmount", Math.Abs(soInfo.PromotionAmt).ToString("#########0.00"));
            keyValueVariables.Add("PromotionDisplay", soInfo.PromotionAmt != 0m);

            keyValueVariables.Add("NowYear", DateTime.Now.Year);
            keyValueVariables.Add("Datetime", DateTime.Now.ToString("yyyy-MM-dd"));
            keyValueVariables.Add("TariffAmt", /*soInfo.BaseInfo.TariffAmount.HasValue ? soInfo.BaseInfo.TariffAmount.Value.ToString(SOConst.DecimalFormat) :*/ 0M.ToString("#########0.00"));
            #endregion

            //int weight = 0;

            #region 替换邮件模板内连接追踪代码
            string now = DateTime.Now.ToString("yyyyMMdd");
            string homePage = string.Format("?cm_mmc=emc-_-tran{0}-_-homepage", now);
            string countDown = string.Format("?cm_mmc=emc-_-tran{0}-_-countdown", now);
            string mobile = string.Format("?cm_mmc=emc-_-tran{0}-_-mobile", now);

            keyValueVariables.Add("CM_MMC_HomePage", homePage);
            keyValueVariables.Add("CM_MMC_CountDown", countDown);
            keyValueVariables.Add("CM_MMC_Mobile", mobile);
            #endregion

            #region 填充商品
            string imgSrc = string.Empty;
            string pagePath = string.Empty;
            soInfo.SOItemList.ForEach(item =>
            {
                //weight += item.Weight.Value * item.Quantity.Value;
                string tbKey = String.Format("Items_{0}", item.ProductType.ToString());
                pagePath = ECommerce.Utility.AppSettingManager.GetSetting("Store", "PreviewBaseUrl") + "product/detail/" + item.ProductSysNo;
                imgSrc = ECommerce.Utility.AppSettingManager.GetSetting("Store", "ImageBaseUrl") + "neg/P60/" + item.ProductID + ".jpg";
                DataTable tableList = null;
                if (!keyTableVariables.ContainsKey(tbKey))
                {
                    tableList = new DataTable();
                    tableList.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("ProductID"),
                        new DataColumn("ProductName"),
                        new DataColumn("Price"),
                        new DataColumn("Quantity"),
                        new DataColumn("Amount"),
                        new DataColumn("PagePath"),
                        new DataColumn("ImgSrc")
                    });
                    keyTableVariables.Add(tbKey, tableList);

                }
                else
                {
                    tableList = keyTableVariables[tbKey];
                }
                //连接增加追踪代码
                string tmp = pagePath;
                if (string.IsNullOrEmpty(tmp))
                {
                    return;
                }

                string param = string.Format("cm_mmc=emc-_-tran{0}-_-Recproduct-_-{1}", DateTime.Now.ToString("yyyyMMdd"), item.ProductID);
                if (tmp.IndexOf('?') > 0)
                {
                    param = "&" + param;
                }
                else
                {
                    param = "?" + param;
                }
                tmp = tmp + param;
                pagePath = tmp;
                tableList.Rows.Add(new string[] 
                { 
                    item.ProductType == SOItemType.Coupon ? null : item.ProductID, 
                    item.ProductName, 
                    item.OriginalPrice.ToString("#########0.00"), 
                    item.Quantity.ToString(), 
                    (item.Quantity * item.OriginalPrice).ToString("#########0.00"),
                    pagePath,
                    imgSrc
                });

            });
            #endregion

            #region 组合销售
            //TODO:
            //List<SOPromotionInfo> comboPromotionList = soInfo.SOPromotions.FindAll(p => { return p.PromotionType == SOPromotionType.Combo; });
            //keyValueVariables.Add("PromotionInfoDisplay", soInfo.BaseInfo.PromotionAmount != 0);
            //if (comboPromotionList != null)
            //{
            //    DataTable comboTable = new DataTable();
            //    comboTable.Columns.AddRange(new DataColumn[]
            //{
            //    new DataColumn("ComboName"),
            //    new DataColumn("ComboDiscount"),
            //    new DataColumn("ComboTime"),
            //    new DataColumn("ComboTotalDiscount")
            //});
            //    keyTableVariables.Add("ComboList", comboTable);
            //    List<ECCentral.BizEntity.MKT.ComboInfo> comboInfoList = ExternalDomainBroker.GetComboList(comboPromotionList.Select<SOPromotionInfo, int>(p => p.PromotionSysNo.Value).ToList<int>());
            //    if (comboInfoList != null)
            //    {
            //        comboPromotionList.ForEach(promotion =>
            //        {
            //            ECCentral.BizEntity.MKT.ComboInfo comboInfo = comboInfoList.FirstOrDefault(cb => cb.SysNo == promotion.PromotionSysNo);
            //            comboTable.Rows.Add(new string[] 
            //        { 
            //            comboInfo == null ? null : comboInfo.Name.Content, 
            //            (promotion.DiscountAmount.Value / promotion.Time.Value).ToString(SOConst.DecimalFormat), 
            //            promotion.Time.Value.ToString(), 
            //            promotion.DiscountAmount.Value.ToString(SOConst.DecimalFormat) 
            //        });
            //        });
            //    }
            //}
            #endregion

            #region 填充推荐商品信息
            string result = string.Empty;
            //测试订单号：388280  
            List<CommendatoryProductsInfo> list = SODA.GetCommendatoryProducts(soInfo.SOSysNo);
            if (list != null && list.Count > 0)
            {
                result = "<br /><table width=\"650px\" style=\"border-collapse: collapse; border: 1px solid #ddd;\" cellspacing=\"0\" cellpadding=\"0\">\n" +
                            "<tr style=\"background:#fff;\">\n" +
                              "<td style=\"width:30px; padding:10px 0 0 10px;\"><img src=\"http://c1.neweggimages.com.cn/NeweggPic2/Marketing/201108/chuhuo/images/icon_4.jpg\" /></td>\n" +
                              "<td style=\"text-align:left; padding:10px 0 0 5px;\"><span style=\"font-family:\"微软雅黑\";font-size:16px; display:inline-block; padding:0; padding-left:5px;\"><strong>我们猜您可能还对下面的商品感兴趣</strong></span></td>\n" +
                            "</tr>\n" +
                        "</table>\n" +
                        "<div style=\"padding-top:10px;border-collapse: collapse; border: 1px solid #ddd;width:648px\">" +
                            "<table cellspacing=\"0\" cellpadding=\"0\" style=\"padding-bottom:10px;border-collapse: collapse; border:0;\">\n" +
                                "<tr style=\" background:#fff;\">\n" +
                                    "[RevommendProductList1]\n" +
                                "</tr>\n" +
                                "<tr style=\"background:#fff;\">\n" +
                                    "[RevommendProductList2]\n" +
                                "</tr>\n" +
                            "</table>\n" +
                        "</div>";
                string item1 = string.Empty,
                item2 = string.Empty;

                IEnumerator<CommendatoryProductsInfo> rator = list.Take(3).GetEnumerator();
                while (rator.MoveNext())
                {
                    CommendatoryProductsInfo entity = rator.Current;
                    item1 += ReplaceCommendatoryProduct(entity, emailTemplateID);
                }
                rator = list.Skip(3).Take(3).GetEnumerator();
                while (rator.MoveNext())
                {
                    CommendatoryProductsInfo entity = rator.Current;
                    item2 += ReplaceCommendatoryProduct(entity, emailTemplateID);
                }

                result = result.Replace("[RevommendProductList1]", item1)
                                .Replace("[RevommendProductList2]", item2);

                keyValueVariables.Add("CommendatoryProducts", result);
            }
            #endregion

            #region 填充备注信息
            string memo = string.Empty;
            if (!string.IsNullOrEmpty(soInfo.MemoForCustomer))
            {
                memo = @"<table border='0' cellpadding='5' cellspacing='0' style='width: 650px; border-collapse: collapse;font-size: 9pt;'>
                                <tr>
                                    <td style='border: 1px solid #ddd; color: #FF4E00; font-size: 10.5pt; font-weight: bold; background: #F2F2F2;'>
                                        备注信息
                                    </td>
                                </tr>
                                <tr>
                                    <td id='Memo' style='border: 1px solid #ddd;'>
                                        " + soInfo.MemoForCustomer + @"
                                    </td>
                                </tr>
                            </table>";
                keyValueVariables.Add("Memo", memo);
            }
            #endregion

            //发送邮件:
            MailTemplate template = MailHelper.BuildMailTemplate(emailTemplateID, keyValueVariables, keyTableVariables);
            CommonService.SendEmail(new AsyncEmail() { MailBody = template.Body, MailSubject = string.Format(template.Subject, soInfo.SOSysNo.ToString()), MailFrom = "service.kjt.com", MailAddress = customerEmail, Priority = 1, Status = 0 });
        }

        private static string ReplaceCommendatoryProduct(CommendatoryProductsInfo entity, string emailTemplateID)
        {
            if (entity == null)
            {
                return string.Empty;
            }
            string item = "<td align=\"left\" valign=\"top\" style=\"padding-left:25px;\">" +
                            "<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"width:180px; overflow:hidden; background:#fff;\">" +
                                "<tr>" +
                                    "<td width=\"86\"><a [ProductItemLink]><img style=\"border:0;width:80px;height:80px;\" src=\"[ProductImageTag]\" /></a></td>" +
                                    "<td width=\"94\" valign=\"middle\";><span style=\"padding-left:10px; color:#ff5100;\">￥<b>[NeweggPrice]</b></span></td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan=\"2\">" +
                                    "<p style=\"line-height:18px; height:36px; width:180px; overflow:hidden; margin-bottom:8px;\"><a [ProductItemLink] style=\"color:#484848; text-decor6ation:none;\">[ProductTitle]</a></p>" +
                                    "</td>" +
                                "</tr>" +
                            "</table>" +
                          "</td>";

            string productLinkUrl = string.Format("{0}product/detail/{1}", ECommerce.Utility.AppSettingManager.GetSetting("Store", "PreviewBaseUrl"), entity.ProductSysNo);

            string tmp = productLinkUrl;
            if (!string.IsNullOrEmpty(tmp))
            {


                string param = string.Format("cm_mmc=emc-_-tran{0}-_-Recproduct-_-{1}", DateTime.Now.ToString("yyyyMMdd"), entity.ProductID);
                if (tmp.IndexOf('?') > 0)
                {
                    param = "&" + param;
                }
                else
                {
                    param = "?" + param;
                }

                switch (emailTemplateID)
                {
                    case "SO_Created":
                    case "SO_Audit_Passed":
                    case "SO_OutStock":
                    case "SO_Splited":
                        break;
                    default:
                        param = string.Empty;
                        break;
                }
                tmp = tmp + param;

                productLinkUrl = tmp;
            }

            productLinkUrl = string.Format("href=\"{0}\" target=\"_blank\"", productLinkUrl);
            string imageUrl = string.Format("{0}neg/P120/{1}?v={2}", ECommerce.Utility.AppSettingManager.GetSetting("Store", "ImageBaseUrl"), entity.DefaultImage, entity.ImageVersion);

            string title = entity.ProductName;

            string price = entity.Price.ToString("##,###0.00");

            item = item.Replace("[ProductItemLink]", productLinkUrl)
                        .Replace("[ProductImageTag]", imageUrl)
                        .Replace("[ProductTitle]", title)
                        .Replace("[NeweggPrice]", price);

            return item;
        }
        #endregion

        public static List<SOLogInfo> GetOrderLogBySOSysNo(int sosysno)
        {
            var list = SODA.GetOrderLogBySOSysNo(sosysno);
            var logisticsLogs = new List<SOLogInfo>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.Note.IndexOf("<ActionName>") > 0)
                    {
                        try
                        {
                            XDocument xmlDoc = XDocument.Parse(item.Note);
                            var actionNameNotes = xmlDoc.Descendants("ActionName");
                            item.Note = actionNameNotes.First().Value;
                        }
                        catch
                        {
                            //简单处理屏蔽异常
                            item.Note = string.Empty;
                        }
                    }
                    else if (item.OptType == BizLogType.Sale_SO_ShippingInfo)
                    {
                        //物流信息 
                        try
                        {
                            if (item.Note.StartsWith("http://www.kuaidi100.com"))
                            {
                                logisticsLogs.Add(new SOLogInfo()
                                {
                                    Note = item.Note
                                });
                            }
                            else
                            {
                                List<SOLogisticsInfo> infos = SerializationUtility.XmlDeserialize<List<SOLogisticsInfo>>(item.Note);
                                if (infos != null && infos.Count > 0)
                                {
                                    DateTime acceptTime;
                                    foreach (var info in infos)
                                    {
                                        logisticsLogs.Add(new SOLogInfo()
                                        {
                                            OptTime = DateTime.TryParse(info.AcceptTime, out acceptTime) ? acceptTime : new Nullable<DateTime>(),
                                            Note = info.Remark
                                        });
                                    }
                                    //DateTime acceptTime;
                                    //logisticsLogs = infos.ConvertAll<SOLogInfo>(c =>
                                    //{
                                    //    var log = item;
                                    //    if (DateTime.TryParse(c.AcceptTime, out acceptTime))
                                    //    {
                                    //        log.OptTime = acceptTime;
                                    //    }
                                    //    log.Note = c.Remark;
                                    //    return log;
                                    //});
                                }
                            }
                        }
                        catch
                        {
                            //简单处理屏蔽异常
                            item.Note = string.Empty;
                        }
                    }
                }
            }
            if (list != null && logisticsLogs != null)
            {
                list.RemoveAll(x => x.OptType == BizLogType.Sale_SO_ShippingInfo);
                list.AddRange(logisticsLogs);
            }
            return list;
        }
        /// <summary>
        /// 撮合交易
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public static List<SOLogMatchedTrading> GetOrderMatchedTradingLogBySOSysNo(int sosysno)
        {
            var list = SODA.GetOrderMatchedTradingLogBySOSysNo(sosysno);
            return list;
        }
        public static void OutStockWaitReport(SOOutStockWaitReportRequest outStockWaitReportRequest)
        {
            // check 是否为待出库状态
            var warningMsgBuilder = new StringBuilder();

            var soInfo = GetSOInfo(outStockWaitReportRequest.SOSysNo);
            if (soInfo.Status != SOStatus.WaitingOutStock)
            {
                warningMsgBuilder.Append(outStockWaitReportRequest.SOSysNo);
                warningMsgBuilder.Append(", ");
            }

            string warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                throw new BusinessException(LanguageHelper.GetText("该订单不是待出库状态，不允许出库操作：{0}"), warningMsg.TrimEnd(',', ' '));
            }

            // check订单必须为非泰隆优选自贸仓（直邮或商家自贸仓）的订单。
            //using (var ts = TransactionManager.Create(TransactionScopeOption.Suppress))
            using (var ts = TransactionManager.Create())
            {
                CheckIsNotKJTFTAStock(outStockWaitReportRequest.SOSysNo);
            }

            SOStatus toStatus = SOStatus.OutStock;
            using (var ts = TransactionManager.Create())
            {
                SODA.UpdateSOStatus(outStockWaitReportRequest.SOSysNo, toStatus, outTime: DateTime.Now);
                SOInfo currentSOInfo = GetSOInfo(outStockWaitReportRequest.SOSysNo);

                string operateName = EnumHelper.GetDescription(toStatus);
                if (outStockWaitReportRequest.Logistics != null && !string.IsNullOrWhiteSpace(outStockWaitReportRequest.Logistics.TrackingNumber))
                {
                    operateName += "，" + LanguageHelper.GetText(string.Format("配送方式：{0}，运单号：{1}", outStockWaitReportRequest.Logistics.ShipTypeName, outStockWaitReportRequest.Logistics.TrackingNumber));
                }
                SODA.WriteLog(currentSOInfo, BizLogType.Sale_SO_OutStock, operateName);
                var trackingInfo = new SOTrackingInfo
                {
                    SONumber = outStockWaitReportRequest.SOSysNo,
                    WarehouseNumber = currentSOInfo.StockSysNo.ToString(),
                    TrackingNumber = outStockWaitReportRequest.Logistics.TrackingNumber,
                    CreateUserID = outStockWaitReportRequest.User.UserID
                };
                SODA.InsertSOTrackingNumber(trackingInfo);
                //创建发票
                CreateInvoice(currentSOInfo);

                //发送短信
                SMSService.SendSMSForSO(currentSOInfo.SOSysNo, SMSType.OrderOutBound);
                ts.Complete();
            }
        }
        public static void CreatMoreSOSysNoAndTrackingNumber(string SoTrackingTableXml, string UserID, int SellerSysNo, int[] soSysNos, ArrayList TrackingNumbers)
        {
            //验证订单是否为待出库状态
            #region 验证订单是否为待出库状态
            StringBuilder warningMsgBuilder = new StringBuilder();
            StringBuilder errorSOno = new StringBuilder();
            List<SOInfo> soList = new List<SOInfo>();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo != null)
                {
                    if (soInfo.Status != SOStatus.WaitingOutStock)
                    {
                        errorSOno.Append(soSysNo);
                        errorSOno.Append(", ");
                    }
                    else
                    {
                        soList.Add(soInfo);
                    }
                }
                else
                {
                    errorSOno.Append(soSysNo);
                    errorSOno.Append(", ");
                }
            }
            if (soList.Count == 0 && errorSOno.Length > 0)
            {
                throw new BusinessException(LanguageHelper.GetText("只有待出库订单才能批量操作，出库失败的订单号：{0}"), errorSOno.ToString().TrimEnd(',', ' '));
            }
            if (soList.Count == 0)
            {
                return;
            }

            #endregion
            // check订单必须为非泰隆优选自贸仓（直邮或商家自贸仓）的订单。
            CheckIsNotKJTFTAStock(soList.Select(c => c.SOSysNo).ToArray());

            using (var ts = TransactionManager.Create())
            {
                SODA.CreatMoreSOSysNoAndTrackingNumber(SoTrackingTableXml, UserID, SellerSysNo);
                foreach (var so in soList)
                {
                    //记录日志
                    SOStatus toStatus = SOStatus.OutStock;
                    string operateName = EnumHelper.GetDescription(toStatus);
                    if (so.ShipType.ShipTypeName != null && !string.IsNullOrWhiteSpace(so.TrackingNumber))
                    {
                        operateName += "，" + LanguageHelper.GetText(string.Format("配送方式：{0}，运单号：{1}", so.ShipType.ShipTypeName, so.TrackingNumber));
                    }

                    CreateInvoice(so);

                    SODA.WriteLog(so, BizLogType.Sale_SO_OutStock, operateName);
                    //发送短信
                    SMSService.SendSMSForSO(so.SOSysNo, SMSType.OrderOutBound);
                }
                ts.Complete();
            }
        }

        private static void CreateInvoice(SOInfo soInfo)
        {
            SOPriceMasterInfo priceInfo = GetSOPriceMasterInfo(soInfo.SOSysNo);
            ShippingAddressInfo shippingAddressInfo = GetShippingAddress(soInfo.CustomerSysNo);
            InvoiceMasterInfo invoiceMaster = GetInvoiceMasterInfo(soInfo, priceInfo, shippingAddressInfo);
            int invoiceSysNo = 0;
            SODA.CreateInvoiceMaster(invoiceMaster, out invoiceSysNo);
            List<InvoiceTransactionInfo> invoiceTransactions = GetInvoiceTransactionInfo(soInfo, priceInfo);
            SODA.CreateInvoiceTransactions(invoiceTransactions, invoiceSysNo);
        }

        private static InvoiceMasterInfo GetInvoiceMasterInfo(SOInfo soInfo, SOPriceMasterInfo priceInfo, ShippingAddressInfo shippingAddressInfo)
        {
            SOItemInfo couponItem = soInfo.SOItemList.Find(item => item.ProductType == SOItemType.Coupon);
            int? couponCodeSysNo = null;
            if (couponItem != null)
            {
                couponCodeSysNo = couponItem.ProductSysNo;
            }

            InvoiceMasterInfo invoiceMaster = new InvoiceMasterInfo()
            {
                CustomerID = soInfo.CustomerID,
                CustomerSysNo = soInfo.CustomerSysNo,
                SONumber = soInfo.SOSysNo,
                InvoiceDate = priceInfo.InDate,
                InvoiceAmt = priceInfo.InvoiceAmount,
                PayTypeSysNo = soInfo.Payment.PayTypeID,
                PayTypeName = soInfo.Payment.PayTypeName,
                RMANumber = 0,
                OriginalInvoiceNumber = 0,
                InvoiceMemo = null,
                ShippingCharge = priceInfo.ShipPrice,
                StockSysNo = Convert.ToInt32(priceInfo.StockSysNo),
                OrderDate = soInfo.OrderDate,
                DeliveryDate = soInfo.DeliveryDate,
                SalesManSysNo = soInfo.MerchantSysNo,
                IsWholeSale = false,
                IsPremium = false,
                PremiumAmt = 0,
                ShipTypeSysNo = soInfo.ShipType.ShipTypeSysNo,
                ExtraAmt = priceInfo.PayPrice,
                SOAmt = priceInfo.SOAmount,
                DiscountAmt = priceInfo.PromotionAmount,
                GainPoint = priceInfo.GainPoint,
                PointPaid = -priceInfo.PointPayAmount,
                PrepayAmt = -priceInfo.PrepayAmount,
                PromotionAmt = priceInfo.CouponAmount,
                ReceiveAreaSysNo = shippingAddressInfo.ReceiveAreaSysNo,
                ReceiveContact = shippingAddressInfo.ReceiveName,
                ReceiveAddress = shippingAddressInfo.ReceiveAddress,
                ReceiveCellPhone = shippingAddressInfo.ReceiveCellPhone,
                ReceivePhone = shippingAddressInfo.ReceivePhone,
                ReceiveZip = shippingAddressInfo.ReceiveZip,
                ReceiveName = shippingAddressInfo.AddressTitle,
                GiftCardPayAmt = -priceInfo.GiftCardPay,
                InvoiceNo = null,
                InvoiceType = soInfo.InvoiceType,
                MerchantSysNo = soInfo.MerchantSysNo,
                CompanyCode = soInfo.CompanyCode,
                PromotionCustomerSysNo = soInfo.CustomerSysNo,
                PromotionCodeSysNo = couponCodeSysNo,
                IsUseChequesPay = false,
                CashPaid = priceInfo.CashPay,
            };
            return invoiceMaster;
        }

        private static List<InvoiceTransactionInfo> GetInvoiceTransactionInfo(SOInfo soInfo, SOPriceMasterInfo priceInfo)
        {
            List<InvoiceTransactionInfo> transactionInfo = new List<InvoiceTransactionInfo>();
            List<SOPriceItemInfo> soPriceItemInfo = SODA.GetSOItemPriceBySOSysNo(soInfo.SOSysNo, priceInfo.SysNo);
            soPriceItemInfo.ForEach(item =>
            {
                SOItemInfo soItem = soInfo.SOItemList.Find(i =>
                {
                    return i.ProductSysNo == item.ProductSysNo && i.ProductType == item.ProductType;
                });
                string itemCode = soItem == null ? String.Empty : soItem.ProductID;
                switch (item.ProductType.Value)
                {
                    case SOItemType.Coupon:
                        itemCode = String.Format("Promot-{0}", item.ProductSysNo);
                        break;
                    case SOItemType.ExtendWarranty:
                        itemCode = String.Format("{0}E", itemCode);
                        break;
                }
                transactionInfo.Add(new InvoiceTransactionInfo
                {
                    ItemCode = itemCode,
                    PrintDescription = item.ProductType.Value.GetEnumDescription(),
                    ItemType = item.ProductType,
                    UnitPrice = item.Price,
                    Quantity = item.Quantity,
                    ExtendPrice = item.Price * item.Quantity,
                    ReferenceSONumber = soInfo.SOSysNo,
                    Weight = soItem.Weight,
                    GainPoint = item.GainPoint,
                    PayType = soInfo.Payment.PayTypeID,
                    PremiumAmt = item.PremiumAmount,
                    ShippingCharge = item.ShipPrice,
                    ExtraAmt = item.PayPrice,
                    CashPaid = item.CashPay,
                    PointPaid = item.PointPayAmount,
                    DiscountAmt = item.PromotionAmount,
                    PrepayAmt = -item.PrepayAmount,
                    Warranty = soItem.Warranty,
                    BriefName = soItem.ProductName,
                    OriginalPrice = soItem.OriginalPrice,
                    PromotionDiscount = item.CouponAmount,
                    MasterProductSysNo = soItem.MasterProductSysNo,
                    UnitCost = soItem.Cost,  //Price?
                    CompanyCode = soInfo.CompanyCode,
                    GiftCardPayAmt = -item.GiftCardPay,
                    UnitCostWithoutTax = soItem.UnitCostWithoutTax,
                    ProductSysNo = soItem.ProductSysNo,
                    PriceType = SOProductPriceType.Normal,
                    ItemDescription = item.ProductType.Value.GetEnumDescription(),
                });
            });

            return transactionInfo;
        }

        private static SOPriceMasterInfo GetSOPriceMasterInfo(int soSysNo)
        {
            List<SOPriceMasterInfo> soPriceList = SODA.GetSOPriceBySOSysNo(soSysNo);
            if (soPriceList != null)
            {
                soPriceList.RemoveAll(priceInfo =>
                {
                    return priceInfo.Status == SOPriceStatus.Deactivate;
                });
            }
            if (soPriceList == null || soPriceList.Count < 1)
            {
                throw new BusinessException(LanguageHelper.GetText("该订单没有进行支付确认:{0}"), soSysNo);
            }
            return soPriceList[0];
        }

        public static ShippingAddressInfo GetShippingAddress(int customerSysNo)
        {
            List<ShippingAddressInfo> shippingAddress = CustomerDA.QueryShippingAddress(customerSysNo);
            if (shippingAddress == null || shippingAddress.Count == 0)
            {
                throw new BusinessException(LanguageHelper.GetText("该客户收货地址为空:{0}"), customerSysNo);
            }
            var defaultShippingAddress = shippingAddress.FirstOrDefault(c => c.IsDefault == true);
            if (defaultShippingAddress == null)
            {
                throw new BusinessException(LanguageHelper.GetText("该客户没有设置默认收货地址:{0}"), customerSysNo);
            }
            return defaultShippingAddress;
        }

        public static void BatcUpdateToPaid(int[] soSysNos)
        {
            #region Precheck

            // check 是否为未支付或者已作废状态
            var warningMsgBuilder = new StringBuilder();
            foreach (int soSysNo in soSysNos)
            {
                var soInfo = SODA.GetSOBySysNo(soSysNo);
                if (soInfo.NetPayStatus != null && soInfo.NetPayStatus != NetPayStatusType.Origin)
                {
                    warningMsgBuilder.Append(soSysNo);
                    warningMsgBuilder.Append(", ");
                }
            }
            string warningMsg = warningMsgBuilder.ToString();
            if (warningMsg.Length > 0)
            {
                if (soSysNos.Length > 1) //批量更新
                    throw new BusinessException(LanguageHelper.GetText("请保证选中的所有订单都符合条件, 某些订单未支付：{0}"), warningMsg.TrimEnd(',', ' '));
                else  //单笔更新
                    throw new BusinessException(LanguageHelper.GetText("该订单不是未支付状态，不允许修改为支付状态：{0}"), warningMsg.TrimEnd(',', ' '));
            }

            #endregion

            using (var ts = TransactionManager.Create())
            {
                foreach (int soSysNo in soSysNos)
                {
                    SODA.UpdateSOPayStatus(soSysNo, 0);
                    //TODO 是否需要记录log
                    //SOInfo currentSOInfo = GetSOInfo(soSysNo);
                    //SODA.WriteLog(currentSOInfo, BizLogType.Sale_SO_Reported, EnumHelper.GetDescription(toStatus));
                }
                ts.Complete();
            }

        }
    }
}
