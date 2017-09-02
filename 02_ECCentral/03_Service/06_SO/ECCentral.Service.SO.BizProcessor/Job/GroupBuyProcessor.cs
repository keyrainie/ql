using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System.Transactions;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 团购订单处理
    /// </summary>
    [VersionExport(typeof(GroupBuySOProcessor))]
    public class GroupBuySOProcessor
    {
        ISODA SODA = ObjectFactory<ISODA>.Instance;

        #region  处理已经完成的团购的订单
        public void ProcessorGroupBuySO(ECCentral.BizEntity.MKT.GroupBuyingInfo gbInfo)
        {
            //处理团购状态为已完成的团购活动
            if (gbInfo.Status == GroupBuyingStatus.Finished || gbInfo.SettlementStatus == GroupBuyingSettlementStatus.MoreThan)
            {
                if (gbInfo.SettlementStatus == GroupBuyingSettlementStatus.No && !gbInfo.SuccessDate.HasValue)
                {
                    FailedGroupBuyProcess(gbInfo);
                }
                else if ((gbInfo.SettlementStatus == GroupBuyingSettlementStatus.No && gbInfo.SuccessDate.HasValue) || gbInfo.SettlementStatus == GroupBuyingSettlementStatus.MoreThan)
                {
                    SuccessfulGroupBuyProcess(gbInfo);
                    SODA.UpdateGroupBuySOItemSettlementStatusByGroupBuySysNo(gbInfo.SysNo.Value, BizEntity.SO.SettlementStatus.Success);
                    SODA.UpdateGroupBuySOSettlementStatusByGroupBuySysNo(gbInfo.SysNo.Value, BizEntity.SO.SettlementStatus.Success);
                }
            }
        }

        private void FailedGroupBuyProcess(GroupBuyingInfo gbInfo)
        {
            TransactionOptions options = new TransactionOptions
              {
                  IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
              };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    SODA.UpdateGroupBuySOItemSettlementStatusByGroupBuySysNo(gbInfo.SysNo.Value, BizEntity.SO.SettlementStatus.PlanFail);
                }
                catch
                {
                    SODA.UpdateGroupBuySOItemSettlementStatusByGroupBuySysNo(gbInfo.SysNo.Value, BizEntity.SO.SettlementStatus.Fail);
                    SODA.UpdateGroupBuySOSettlementStatusByGroupBuySysNo(gbInfo.SysNo.Value, BizEntity.SO.SettlementStatus.Fail);
                }
                ExternalDomainBroker.UpdateGroupBuySettlementStatus(gbInfo.SysNo.Value, GroupBuyingSettlementStatus.Yes);
                scope.Complete();
            }
        }

        private void SuccessfulGroupBuyProcess(GroupBuyingInfo gbInfo)
        {
            List<SOItemInfo> GroupBuyItemList = SODA.GetGroupBuySOItemByGroupBuySysNo(gbInfo.SysNo.Value);
            SOProcessor soProcessor = ObjectFactory<SOProcessor>.Instance;
            foreach (SOItemInfo groupBuyItem in GroupBuyItemList)
            {
                //更新订单
                try
                {
                    //修改团购商品价格
                    SOInfo soInfo = SODA.GetSOBySOSysNo(groupBuyItem.SOSysNo.Value);
                    soProcessor.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Job, SOInfo = soInfo, Parameter = new object[] { groupBuyItem.ProductSysNo } });

                }
                catch (Exception ex)
                {
                    SODA.UpdateGroupBuySOAndItemSettlementStatus(groupBuyItem.SOSysNo.Value, groupBuyItem.ProductSysNo.Value, SettlementStatus.Fail);
                    ExceptionHelper.HandleException(ex);
                    continue;
                }
            }
            ExternalDomainBroker.UpdateGroupBuySettlementStatus(gbInfo.SysNo.Value, GroupBuyingSettlementStatus.Yes);
        }
        #endregion


        public List<int> GetInvalidGroupBuySOSysNoList(string companyCode)
        {
            return SODA.GetInvalidGroupBuySOSysNoList(companyCode);
        }
        /// <summary>
        /// 处理无效的团购订单
        /// </summary>
        /// <returns></returns>
        public void ProcessorInvalidGroupBuySO(SOInfo soInfo)
        {
            if (soInfo == null || !soInfo.SysNo.HasValue)
            {
                return;
            }
            int soSysNo = soInfo.SysNo.Value;
            try
            {
                //获取所有Item
                List<SOItemInfo> items = (from item in soInfo.Items
                                          where item.ActivityType == SOProductActivityType.GroupBuy
                                          select item).ToList();

                if (items == null || items.Count == 0)
                    return;

                //如果含有未处理的团购商品就过
                if (items.Exists(x => x.ReferenceSysNo.HasValue && x.ReferenceSysNo != 0 && !x.SettlementStatus.HasValue))
                {
                    return;
                }
                else if (items.Exists(x => x.SettlementStatus == SettlementStatus.Fail))
                {
                    //更新订单团购处理状态
                    SODA.UpdateGroupBuySOSettlementStatusBySOSysNo(soSysNo, SettlementStatus.Fail);
                }
                else if (items.Exists(x => x.SettlementStatus == SettlementStatus.PlanFail))
                {
                    //只含P,作废
                    if (!items.Exists(x => x.SettlementStatus != SettlementStatus.PlanFail))
                    {
                        ECCentral.BizEntity.Invoice.NetPayInfo netPayInfo = ExternalDomainBroker.GetSOValidNetPay(soSysNo);
                        if (netPayInfo != null)
                        {
                            //ExternalDomainBroker.AuditNetPay(netPayInfo.SysNo.Value);
                            ExternalDomainBroker.AuditNetPay4GroupBuy(netPayInfo.SysNo.Value);
                            ExternalDomainBroker.CreateAOForJob(soSysNo, BizEntity.Invoice.RefundPayType.PrepayRefund, ResourceHelper.Get("Res_SO_Job_GropBuySOAbandon"), null);
                        }

                        ObjectFactory<SOProcessor>.Instance.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Abandon, SOInfo = soInfo });

                    }
                    else
                    {
                        //更新订单团购处理状态
                        SODA.UpdateGroupBuySOSettlementStatusBySOSysNo(soSysNo, SettlementStatus.PlanFail);
                    }
                }
                else if (!items.Exists(x => x.ReferenceSysNo.HasValue && x.ReferenceSysNo != 0 && x.SettlementStatus != SettlementStatus.Success))
                {
                    ECCentral.BizEntity.Invoice.NetPayInfo netPayInfo = ExternalDomainBroker.GetSOValidNetPay(soSysNo);
                    //审核NetPay
                    if (netPayInfo != null)
                    {
                        //ExternalDomainBroker.AuditNetPay(netPayInfo.SysNo.Value);
                        ExternalDomainBroker.AuditNetPay4GroupBuy(netPayInfo.SysNo.Value);
                    }

                    SODA.UpdateGroupBuySOSettlementStatusBySOSysNo(soSysNo, SettlementStatus.Success);
                }
            }
            catch (Exception ex)
            {
                SODA.UpdateGroupBuySOSettlementStatusBySOSysNo(soSysNo, SettlementStatus.Fail);
                ExceptionHelper.HandleException(ex);
            }

        }

        public List<int> GetNotPayGroupBuySOSysNoList(string companyCode)
        {
            return SODA.GetNotPayGroupBuySOSysNoList(companyCode);
        }


        //public void 
    }

    [VersionExport(typeof(SOAction), new string[] { "GroupBuy", "Job" })]
    public class GroupBuySOUpdater : SOAction
    {
        #region Parameter参数
        /* Parameter说明:
         * Parameter[0] : int , 团购商品编号 
         *
         *
         */
        /// <summary>
        /// 团购商品编号,由 Parameter[0] 传入
        /// </summary>
        protected int ProductSysNo
        {
            get
            {
                return GetParameterByIndex<int>(0);
            }
        }
        #endregion
        private ISODA _soDA;
        protected ISODA SODA
        {
            get
            {
                _soDA = _soDA ?? ObjectFactory<ISODA>.Instance;
                return _soDA;
            }
        }
        public override void Do()
        {
            Update();

        }
        private void ValidationGroupBuyingRules()
        {
            if (!CurrentSO.BaseInfo.SOType.HasValue || CurrentSO.BaseInfo.SOType.Value != SOType.GroupBuy)
            {
                BizExceptionHelper.Throw("Res_SO_NoGroupOrder", CurrentSO.SysNo.ToString());
            }

            if (!CurrentSO.BaseInfo.Status.HasValue || CurrentSO.BaseInfo.Status != SOStatus.Origin)
            {
                BizExceptionHelper.Throw("Res_SO_GroupOrderOrigin", CurrentSO.SysNo.ToString());
            }

            if (!CurrentSO.BaseInfo.ReferenceSysNo.HasValue)
            {
                BizExceptionHelper.Throw("Res_SO_NotGroupReferenceSysNo", CurrentSO.SysNo.ToString());
            }
        }
        private void ValidationSOEntity()
        {
            if (CurrentSO.Items == null || CurrentSO.Items.Count <= 0)
            {
                BizExceptionHelper.Throw("Res_GroupSO_NoItems", CurrentSO.SysNo.ToString());
            }

            if (!CurrentSO.ShippingInfo.ShipTypeSysNo.HasValue || CurrentSO.ShippingInfo.ShipTypeSysNo <= 0)
            {
                BizExceptionHelper.Throw("Res_GroupSO_NoShipTypeSysNo", CurrentSO.SysNo.ToString());
            }

            if (!CurrentSO.BaseInfo.PayTypeSysNo.HasValue || CurrentSO.BaseInfo.PayTypeSysNo <= 0)
            {
                BizExceptionHelper.Throw("Res_GroupSO_NoPayTypeSysNo", CurrentSO.SysNo.ToString());
            }

            if (!CurrentSO.BaseInfo.CustomerSysNo.HasValue || CurrentSO.BaseInfo.CustomerSysNo <= 0)
            {
                BizExceptionHelper.Throw("Res_GroupSO_NoCustomerSysNo", CurrentSO.SysNo.ToString());
            }
        }
        /// <summary>
        /// 更新团购订单
        /// </summary>
        /// <param name="CurrentSO"></param>
        /// <returns></returns>
        public void Update()
        {
            ValidationSOEntity();
            ValidationGroupBuyingRules();

            SOItemInfo groupBuyProduct = CurrentSO.Items.Find(item => item.ProductSysNo == ProductSysNo);
            if (groupBuyProduct == null)
            {
                return;
            }
            //可能有赠品,但是只会有一个主商品
            int groupBuySysNo = groupBuyProduct.ReferenceSysNo.Value;

            GroupBuyingInfo gbInfo = ExternalDomainBroker.GetGroupBuyInfoBySysNo(groupBuySysNo);
            decimal dealPrice = gbInfo.GBPrice.HasValue ? gbInfo.GBPrice.Value : -1;

            if (dealPrice < 0)
            {
                throw new BizException(ResourceHelper.Get("SO_Audit_GroupNotDealPrice", CurrentSO.SysNo));
            }

            if (groupBuyProduct != null && dealPrice < groupBuyProduct.Price)
            {
                SOInfo newSOInfo = new SOInfo();
                //newSOInfo.SOItemList = SerializeHelper.DeepClone(soInfo.SOItemList);   

                List<SOItemInfo> otherItems = CurrentSO.Items.FindAll(x => x.ProductSysNo != ProductSysNo);

                decimal oldSOAmt = 0.0m;

                if (otherItems != null && otherItems.Count > 0)
                    oldSOAmt = otherItems.Sum(x => x.OriginalPrice.Value * x.Quantity.Value);



                decimal newSOAmt = oldSOAmt;


                if (groupBuyProduct.ProductType == SOProductType.Product)
                {
                    groupBuyProduct.Price = dealPrice;
                    groupBuyProduct.OriginalPrice = dealPrice;
                    groupBuyProduct.SettlementStatus = SettlementStatus.Success;
                    //团购订单不能用优惠卷
                    newSOAmt += dealPrice * groupBuyProduct.Quantity.Value;
                }


                int refundPoint = 0;
                decimal refundPrepay = 0.0m;
                decimal refundGiftCard = 0.0m;
                decimal difference = CurrentSO.BaseInfo.SOAmount.Value - newSOAmt;

                decimal newPrepayAmt = CurrentSO.BaseInfo.PrepayAmount.Value;
                decimal newGiftCardPay = CurrentSO.BaseInfo.GiftCardPay.Value;
                decimal newPremiumAmt = CurrentSO.BaseInfo.PremiumAmount.Value;
                int newPointPay = CurrentSO.BaseInfo.PointPay.Value;
                decimal newCashPay = newSOAmt - Math.Abs(CurrentSO.BaseInfo.PointPayAmount.Value);

                ECCentral.BizEntity.Common.ShippingType shippingType = ExternalDomainBroker.GetShippingTypeBySysNo(CurrentSO.ShippingInfo.ShipTypeSysNo.Value);
                if (CurrentSO.BaseInfo.IsPremium.Value)
                {
                    if (shippingType != null && newSOAmt > shippingType.PremiumBase)
                    {
                        newPremiumAmt = UtilityHelper.ToMoney(newSOAmt * shippingType.PremiumRate.Value);
                    }
                }
                decimal pointToMoneyRatio = ExternalDomainBroker.GetPointToMoneyRatio();
                #region 退款逻辑
                //退款优先级：1.先退现金支付（注意：不在这里退还），2.退余额支付，3.退礼品卡支付，4.退积分支付
                //  1.  需退还的：余额支付金额，退礼支付品金额，积分支付金额的总合
                decimal refundPayAmount = CurrentSO.BaseInfo.PrepayAmount.Value + CurrentSO.BaseInfo.GiftCardPay.Value + CurrentSO.BaseInfo.PointPayAmount.Value -
                    (newSOAmt + newPremiumAmt + CurrentSO.BaseInfo.ShipPrice.Value);
                //  2.  如果使用余额支付，退到余额
                if (refundPayAmount > 0)
                {
                    refundPrepay = refundPayAmount > CurrentSO.BaseInfo.PrepayAmount ? CurrentSO.BaseInfo.PrepayAmount.Value : refundPayAmount;
                    newPrepayAmt = CurrentSO.BaseInfo.PrepayAmount.Value - refundPrepay;
                    refundPayAmount = refundPayAmount - refundPrepay;

                    //  3.  如果使用礼品卡支付，退到礼品卡
                    if (refundPayAmount > 0)
                    {
                        refundGiftCard = refundPayAmount > CurrentSO.BaseInfo.GiftCardPay ? CurrentSO.BaseInfo.GiftCardPay.Value : refundPayAmount;
                        newGiftCardPay = CurrentSO.BaseInfo.GiftCardPay.Value - refundGiftCard;
                        refundPayAmount = refundPayAmount - refundGiftCard;

                        //  4.  如果使用积分支付，退积分
                        if (refundPayAmount > 0)
                        {
                            decimal refundPointAmount = refundPayAmount > CurrentSO.BaseInfo.PointPayAmount ? CurrentSO.BaseInfo.PointPayAmount.Value : refundPayAmount;
                            refundPayAmount = refundPayAmount - refundPointAmount;
                            decimal newPontPayAmount = CurrentSO.BaseInfo.PointPayAmount.Value - refundPointAmount;
                            newPointPay = (int)(newPontPayAmount * pointToMoneyRatio);
                            refundPoint = (int)(refundPointAmount * pointToMoneyRatio);
                            newCashPay = newSOAmt - newPontPayAmount;
                        }
                    }
                }

                #endregion
                ECCentral.BizEntity.Common.PayType payType = ExternalDomainBroker.GetPayTypeBySysNo(CurrentSO.BaseInfo.PayTypeSysNo.Value);
                decimal newPayPrice = Math.Max(UtilityHelper.ToMoney(payType.PayRate.Value *
                    (newCashPay + CurrentSO.BaseInfo.ShipPrice.Value + newPremiumAmt - newPrepayAmt - newGiftCardPay)), 0M);

                CurrentSO.BaseInfo.SOAmount = newSOAmt;
                CurrentSO.BaseInfo.PremiumAmount = newPremiumAmt;
                CurrentSO.BaseInfo.PayPrice = newPayPrice;
                CurrentSO.BaseInfo.GiftCardPay = newGiftCardPay;
                CurrentSO.BaseInfo.PointPay = newPointPay;
                CurrentSO.BaseInfo.PointPayAmount = CurrentSO.BaseInfo.PointPay.Value / pointToMoneyRatio;
                CurrentSO.BaseInfo.PrepayAmount = newPrepayAmt;

                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    SODA.UpdateGroupBuySOAmount(CurrentSO.BaseInfo);

                    SODA.UpdateGroupBuyProduct(groupBuyProduct);


                    if (refundPrepay > 0)
                    {
                        ExternalDomainBroker.AdjustPrePay(new BizEntity.Customer.CustomerPrepayLog
                        {
                            AdjustAmount = refundPrepay,
                            CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo,
                            Note = "Update SO For GroupBuying",
                            PrepayType = ECCentral.BizEntity.Customer.PrepayType.SOPay,
                            SOSysNo = CurrentSO.SysNo,
                        });
                    }

                    if (refundGiftCard > 0)
                    {
                        ExternalDomainBroker.GiftCardVoidForSOUpdate(newGiftCardPay, CurrentSO.SOGiftCardList, CurrentSO.CompanyCode);
                    }

                    if (refundPoint > 0)
                    {
                        ExternalDomainBroker.AdjustPoint(new BizEntity.Customer.AdjustPointRequest
                        {
                            CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo,
                            Memo = "Update Group Buying SO",
                            OperationType = ECCentral.BizEntity.Customer.AdjustPointOperationType.AddOrReduce,
                            Point = refundPoint,
                            PointType = (int)ECCentral.BizEntity.Customer.AdjustPointType.UpdateSO,
                            SOSysNo = CurrentSO.SysNo,
                            Source = SOConst.DomainName
                        });
                    }
                    scope.Complete();
                }
            }
            WriteLog(BizEntity.Common.BizLogType.Sale_SO_Update, "IPP更改团购订单");
        }

    }
}
