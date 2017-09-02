using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOPendingProcessor))]
    public class SOPendingProcessor
    {
        ISOPendingDA m_da = ObjectFactory<ISOPendingDA>.Instance;

        //打开订单
        /// <summary>
        /// 打开订单
        /// </summary>
        /// <param name="soSysNo">订单</param>
        public virtual void Open(int soSysNo)
        {
            SOPending oldEntity = ObjectFactory<ISOPendingDA>.Instance.GetBySysNo(soSysNo);

            if (oldEntity == null)
            {
                //不存在的单据
                BizExceptionHelper.Throw("SO_Pending_UnknowOrder");
            }
            //无需重复提交，节省资源
            if (oldEntity.Status != SOPendingStatus.Origin)
            {
                m_da.UpdateSOPendingStatus(soSysNo, SOPendingStatus.Origin);
                ExternalDomainBroker.WriteBizLog(ResourceHelper.Get("SO_Pending_OpenLogFormat", oldEntity.SOSysNo)
                                                , BizLogType.Sale_SO_Update
                                                , oldEntity.SOSysNo.Value
                                                , oldEntity.CompanyCode);
            }
        }

        //关闭订单
        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public virtual void Close(int soSysNo)
        {
            SOPending oldEntity = ObjectFactory<ISOPendingDA>.Instance.GetBySysNo(soSysNo);

            if (oldEntity == null)
            {
                //不存在的单据
                BizExceptionHelper.Throw("SO_Pending_UnknowOrder");
            }
            if (oldEntity.Status != SOPendingStatus.Complete)
            {
                m_da.UpdateSOPendingStatus(soSysNo, SOPendingStatus.Complete);
                ExternalDomainBroker.WriteBizLog(ResourceHelper.Get("SO_Pending_CloseLogFormat", oldEntity.SOSysNo)
                                                , BizLogType.Sale_SO_Update
                                                , oldEntity.SOSysNo.Value
                                                , oldEntity.CompanyCode);
            }
        }

        //改单
        /// <summary>
        /// 改单
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public virtual void Update(int soSysNo)
        {
            var soProcessor = ObjectFactory<SOProcessor>.Instance;

            var soInfo = soProcessor.GetSOBySOSysNo(soSysNo);
            if (soInfo == null)
            {
                BizExceptionHelper.Throw("SO_SOIsNotExist");
            }

            //是否货到付款
            bool isPayWhenRecv = soProcessor.IsPayWhenReceived(soInfo.BaseInfo.PayTypeSysNo.Value);

            //查询订单出库记录
            string outStock = m_da.GetOutStockString(soSysNo);
            //还没有出仓记录
            if (string.IsNullOrEmpty(outStock))
            {
                //直接作废
                soProcessor.ProcessSO(new SOAction.SOCommandInfo
                {
                    SOInfo = soInfo,
                    Command = SOAction.SOCommand.Abandon
                });
                return;
            }
            //获取出库收款信息
            var invoiceMasterList = ExternalDomainBroker.GetSOInvoiceMaster(soSysNo);
            //计算总出库金额相关数据

            //保价费
            decimal premiumAmt = invoiceMasterList
                                    .Where(p => p.PremiumAmt.HasValue)
                                    .Sum(p => p.PremiumAmt.Value);

            //运费
            decimal shippingCharge = invoiceMasterList
                                        .Where(p => p.ShippingCharge.HasValue)
                                        .Sum(p => p.ShippingCharge.Value);

            //附加费
            decimal extraAmt = invoiceMasterList
                                    .Where(p => p.ExtraAmt.HasValue)
                                    .Sum(p => p.ExtraAmt.Value);

            //折扣金额
            decimal discountAmt = invoiceMasterList
                                 .Where(p => p.DiscountAmt.HasValue)
                                 .Sum(p => p.DiscountAmt.Value);

            //优惠卷抵扣
            decimal promotionAmt = invoiceMasterList
                                .Where(p => p.PromotionAmt.HasValue)
                                .Sum(p => p.PromotionAmt.Value);

            //获得积分？
            int pointAmt = invoiceMasterList
                            .Where(p => p.GainPoint.HasValue)
                            .Sum(p => p.GainPoint.Value);

            //出库发票额
            decimal sumExtendPrice = invoiceMasterList
                                        .Where(p => p.InvoiceAmt.HasValue)
                                        .Sum(p => p.InvoiceAmt.Value);

            //礼品卡支付总额
            decimal sumGiftCardPay = invoiceMasterList
                                        .Where(p => p.GiftCardPayAmt.HasValue)
                                        .Sum(p => p.GiftCardPayAmt.Value);

            //积分支付总额
            decimal pointPay = invoiceMasterList
                                    .Where(p => p.PointPaid.HasValue)
                                    .Sum(p => p.PointPaid.Value);
            //计算多余额应退金额
            decimal returnAmt = GetReturnAmt(soSysNo, sumExtendPrice, soInfo.BaseInfo.GiftCardPay.Value);

            //计算应退积分
            int returnPoint = GetReturnPoint(soInfo, pointPay);

            //已预付款
            decimal prePayAmount = invoiceMasterList
                                    .Where(p => p.PrepayAmt.HasValue)
                                    .Sum(p => p.PrepayAmt.Value);

            //预退款总额
            decimal preReturnAmount = soInfo.BaseInfo.PrepayAmount.Value
                                + prePayAmount;

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //删除未出库分仓的所有Item
                DeleteOrderItem4UpdatePending(soInfo.Items, soInfo.SysNo.Value, outStock);

                #region 更新so单据信息

                //更新so单据信息
                //删除后的SOInfo需要重新读取
                //注意这里虽然事务没有提交，但是可以读取脏数据的方法获取
                soInfo = soProcessor.GetSOBySOSysNo(soSysNo);
                soInfo.BaseInfo.PremiumAmount = premiumAmt;
                soInfo.BaseInfo.ShipPrice = shippingCharge;
                soInfo.BaseInfo.PayPrice = extraAmt;
                soInfo.BaseInfo.PointPay = 0;
                soInfo.BaseInfo.PromotionAmount = discountAmt;
                soInfo.BaseInfo.CouponAmount = promotionAmt;
                soInfo.BaseInfo.GainPoint = pointAmt;

                soInfo.BaseInfo.SOAmount = 0.0M;
                soInfo.Items.ForEach(x =>
                {
                    if (x.ProductType.HasValue
                        && x.ProductType.Value != SOProductType.Coupon)
                    {
                        soInfo.BaseInfo.SOAmount += x.OriginalPrice * x.Quantity;
                    }
                });

                //现金支付为只读
                //soInfo.BaseInfo.CashPay = soInfo.SOMaster.SOAmt + soInfo.SOMaster.PromotionValue + soMP.PointPay;
                soInfo.BaseInfo.PointPay = Convert.ToInt32(-1 * pointPay * ExternalDomainBroker.GetPointToMoneyRatio());

                if (prePayAmount < 0.0M)
                {
                    soInfo.BaseInfo.PrepayAmount = (-1) * prePayAmount;
                }

                soInfo.BaseInfo.GiftCardPay = (-1) * sumGiftCardPay;
                soProcessor.ProcessSO(new SOAction.SOCommandInfo
                {
                    Command = SOAction.SOCommand.Update,
                    SOInfo = soInfo
                });

                #endregion 更新so单据信息

                //更新改单状态
                m_da.UpdateSOPendingStatus(soSysNo, SOPendingStatus.ChangeOrder);

#warning 需要重构 重新计算是否并单
                //重新计算是否并单
                ObjectFactory<ISODA>.Instance.UpdateSOCombineInfo(soInfo.BaseInfo.SysNo.Value);

                //金额拆分
                SOPriceSpliter priceSpliter = ObjectFactory<SOPriceSpliter>.Instance;
                priceSpliter.CurrentSO = soInfo;
                priceSpliter.SplitSO();

                //重发消息
                Resend_ShippingMessage(soSysNo);

                #region 调整积分

                var pointAdjustReq = new ECCentral.BizEntity.Customer.AdjustPointRequest();
                pointAdjustReq.CustomerSysNo = soInfo.BaseInfo.CustomerSysNo;
                pointAdjustReq.OperationType = ECCentral.BizEntity.Customer.AdjustPointOperationType.Abandon;
                pointAdjustReq.Point = returnPoint;
                pointAdjustReq.SOSysNo = soInfo.SysNo;
                pointAdjustReq.Source = "OrderMgmt";
                pointAdjustReq.PointType = (int)ECCentral.BizEntity.Customer.AdjustPointType.UpdateSO;
                pointAdjustReq.Memo = ResourceHelper.Get("SO_Pending_ReturnPointMemo");
                ExternalDomainBroker.AdjustPoint(pointAdjustReq);

                #endregion 调整积分

                #region 退款(退余额)

                if (isPayWhenRecv)
                {
                    //支持货到付款的改单
                    if (preReturnAmount > 0.0M) //(prepayAmt > sumExtendPrice) //预付款大于已出库总金额,要将多余的钱退回客户
                    {
                        var customerPrepayReq = new CustomerPrepayLog();
                        customerPrepayReq.CustomerSysNo = soInfo.BaseInfo.CustomerSysNo;
                        customerPrepayReq.SOSysNo = soInfo.SysNo;
                        customerPrepayReq.AdjustAmount = preReturnAmount;
                        customerPrepayReq.PrepayType = PrepayType.RemitReturn;
                        customerPrepayReq.Note = ResourceHelper.Get("SO_Pending_PreReturnMemo", preReturnAmount);
                        ExternalDomainBroker.AdjustPrePay(customerPrepayReq);
                    }
                }
                else
                {//生成对应的多付款退款记录(invoiceservice)更新财务收款单中的OrderAmt金额(invoiceservice)
                    if (returnAmt > 0)
                    {
                        //查询收款单
                        var incomeOrg = ExternalDomainBroker.GetValidSOIncomeInfo(soSysNo, SOIncomeOrderType.SO);
                        if (incomeOrg == null)
                        {
                            BizExceptionHelper.Throw("SO_Income_Unknow", soSysNo.ToString());
                        }

                        //修改原始的订单金额
                        incomeOrg.OrderAmt = incomeOrg.OrderAmt - returnAmt;

                        ExternalDomainBroker.UpdateSOIncomeOrderAmount(incomeOrg.SysNo.Value, incomeOrg.OrderAmt.Value);
                        //更新

                        //创建付款收支单
                        var income = new SOIncomeInfo
                        {
                            OrderSysNo = soInfo.SysNo
                            ,
                            OrderAmt = -returnAmt
                            ,
                            IncomeAmt = -returnAmt
                            ,
                            OrderType = SOIncomeOrderType.OverPayment
                            ,
                            Note = ResourceHelper.Get("SO_Pending_ReturnMemo")
                            ,
                            Status = SOIncomeStatus.Origin
                            ,
                            IncomeStyle = SOIncomeOrderStyle.Advanced
                            ,
                            CompanyCode = soInfo.CompanyCode
                        };
                        ExternalDomainBroker.CreateSOIncome(income);

                        //创建银行收支单
                        SOIncomeRefundInfo refundInfo = new SOIncomeRefundInfo
                        {
                            SOSysNo = soInfo.SysNo
                         ,
                            OrderSysNo = soInfo.SysNo
                         ,
                            OrderType = RefundOrderType.OverPayment
                         ,
                            RefundPayType = RefundPayType.PrepayRefund
                         ,
                            RefundReason = 5
                         ,
                            Status = RefundStatus.Origin
                         ,
                            Note = ResourceHelper.Get("SO_Pending_ReturnMemo")
                         ,
                            RefundCashAmt = returnAmt
                         ,
                            RefundPoint = 0
                         ,
                            ToleranceAmt = 0
                         ,
                            CompanyCode = soInfo.CompanyCode
                        };
                        ExternalDomainBroker.CreateSOIncomeRefundInfo(refundInfo);
                    }
                }

                #endregion 退款(退余额)

                #region 退礼品卡

                //退礼品卡
                if (sumGiftCardPay < 0.0M)
                {
                    if (soInfo.SOGiftCardList != null)
                    {
                        List<GiftCard> reqList = new List<GiftCard>();
                        decimal needToPayAmt = sumGiftCardPay * (-1);
                        for (int i = 0; i < soInfo.SOGiftCardList.Count; i++)
                        {
                            if (needToPayAmt <= 0)
                            {
                                soInfo.SOGiftCardList.RemoveAt(i);
                                i--;
                                continue;
                            }

                            soInfo.SOGiftCardList[i].AvailAmount = soInfo.SOGiftCardList[i].Amount.HasValue
                                                                        ? soInfo.SOGiftCardList[i].Amount.Value : 0;

                            if (soInfo.SOGiftCardList[i].AvailAmount >= needToPayAmt)
                            {
                                soInfo.SOGiftCardList[i].Amount = needToPayAmt;
                                soInfo.SOGiftCardList[i].AvailAmount -= needToPayAmt;
                                needToPayAmt = 0;
                            }
                            else
                            {
                                soInfo.SOGiftCardList[i].Amount = soInfo.SOGiftCardList[i].AvailAmount;
                                soInfo.SOGiftCardList[i].AvailAmount = 0;
                                needToPayAmt -= soInfo.SOGiftCardList[i].Amount.Value;
                            }
                            reqList.Add(new GiftCard
                            {
                                Code = soInfo.SOGiftCardList[i].Code,
                                ReferenceSOSysNo = soSysNo,
                                CustomerSysNo = soInfo.SOGiftCardList[i].CustomerSysNo.Value,
                                ConsumeAmount = soInfo.SOGiftCardList[i].Amount.Value
                            });
                        }
                        ExternalDomainBroker.GiftCardDeduction(reqList, soInfo.CompanyCode);
                    }
                }

                #endregion 退礼品卡

                scope.Complete();
            }

            ExternalDomainBroker.WriteBizLog(ResourceHelper.Get("SO_Pending_UpdateLogFormat", soSysNo)
                                            , BizLogType.Sale_SO_Update
                                            , soSysNo
                                            , soInfo.CompanyCode);
        }

        /// <summary>
        /// 改单通知仓库
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        private void Resend_ShippingMessage(int soSysNo)
        {
            //原来逻辑
            /*1.根据SOSysNo查询Dropship.dbo.ShippingServiceLog获取MessageBody
             * 2.没有查到将抛出异常，程序终止
             * 3.将获取到得MessageBody传入调用sp的SSB.dbo.Up_ReSendShippingMsg
             * 4.然后将MessageBody插入到Dropship.dbo.ShippingMessageLog_ReSend
             */
        }

        ///// 计算积分返回
        /// <summary>
        /// 计算积分返回
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <param name="pointPay">已支付的积分款</param>
        /// <returns>应退积分</returns>
        private int GetReturnPoint(SOInfo soInfo, decimal pointPay)
        {
            pointPay = pointPay * ExternalDomainBroker.GetPointToMoneyRatio();
            int usedPoint = Convert.ToInt32(pointPay);
            return soInfo.BaseInfo.PointPay.Value + usedPoint;//用点数支付的是负值，所以这块用+
        }

        //计算多余额应退
        /// <summary>
        /// 计算多余额应退
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="orderAmt">订单总额</param>
        /// <param name="giftcardPay">积分支付额</param>
        /// <returns>多余退款额</returns>
        private decimal GetReturnAmt(int soSysNo, decimal orderAmt, decimal giftcardPay)
        {
            var validIncome = ExternalDomainBroker.GetValidSOIncomeInfo(soSysNo, SOIncomeOrderType.SO);
            if (validIncome == null) return 0.0M;

            decimal incomeAmt = validIncome.OrderAmt.Value;
            if (giftcardPay > 0.0M)
                return (incomeAmt > 0.0M) ? (incomeAmt - giftcardPay - orderAmt) : 0.0M;
            return (incomeAmt > 0.0M) ? (incomeAmt - orderAmt) : 0.0M;
        }

        //删除未出库的Item
        /// <summary>
        /// 删除未出库的Item
        /// </summary>
        /// <param name="soItems">订单子项</param>
        /// <param name="outStock">已出库仓库集合，以逗号隔开</param>
        private void DeleteOrderItem4UpdatePending(List<SOItemInfo> soItems, int soSysNo, string outStock)
        {
            if (null == soItems)
                return;
            //调整库存请求
            InventoryAdjustContractInfo req = new InventoryAdjustContractInfo();
            req.ReferenceSysNo = soSysNo.ToString();
            req.SourceBizFunctionName = InventoryAdjustSourceBizFunction.SO_Order;
            req.SourceActionName = InventoryAdjustSourceAction.Pending;

            req.AdjustItemList = new List<InventoryAdjustItemInfo>();

            soItems.ForEach(x =>
            {
                if (x.ProductType.HasValue
                    && x.ProductType.Value != SOProductType.Coupon
                    && x.ProductType.Value != SOProductType.ExtendWarranty
                    && outStock.IndexOf(x.StockSysNo.ToString()) < 0)
                {
                    req.AdjustItemList.Add(new InventoryAdjustItemInfo()
                    {
                        ProductSysNo = x.ProductSysNo.Value
                        ,
                        StockSysNo = x.StockSysNo.Value
                        ,
                        AdjustQuantity = -x.Quantity.Value
                    });
                    //删除未出库的SOItem
                    ObjectFactory<ISODA>.Instance.DeleteSOItemBySysNo(x.SysNo.Value);
                }
            });

            //更新仓库信息
            try
            {
                ExternalDomainBroker.AdjustProductInventory(req);
            }
            catch
            {
                BizExceptionHelper.Throw("SO_Pending_AdjustInventoryFail");
            }
        }
    }
}