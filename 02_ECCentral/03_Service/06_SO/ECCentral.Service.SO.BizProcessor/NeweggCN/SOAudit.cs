using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity;

namespace ECCentral.Service.SO.BizProcessor
{
    #region 订单审核

    /// <summary>
    /// 审核SIM卡订单。
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
    /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
    /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "SIM", "Audit" })]
    public class SIMSOAudit : SOAudit
    {
        public override void SendMessage()
        {
            SOSendMessageProcessor messageProcessor = ObjectFactory<SOSendMessageProcessor>.Instance;
            SOStatus soStatus = CurrentSO.BaseInfo.Status.Value;

            if (soStatus == SOStatus.WaitingOutStock)
            {
                // 给内部工作人员发邮件
                messageProcessor.SendSIMCardStatusMail(CurrentSO);
                base.SendMessage();
            }
        }
    }

    /// <summary>
    /// 审核SIM卡订单。
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
    /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
    /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "ContractPhone", "Audit" })]
    public class ContractPhoneSOAudit : SOAudit
    {
        public override void SendMessage()
        {
            SOSendMessageProcessor messageProcessor = ObjectFactory<SOSendMessageProcessor>.Instance;
            SOStatus soStatus = CurrentSO.BaseInfo.Status.Value;

            if (soStatus == SOStatus.WaitingOutStock)
            {
                // 给内部工作人员发邮件
                messageProcessor.SendUnicomSOSIMCardMail(CurrentSO);

                base.SendMessage();
            }
        }
    }

    [VersionExport(typeof(SOAction), new string[] { "BuyMobileSettlement", "Audit" })]
    public class BuyMobileSettlementSOAudit : SOAudit
    {
        protected override void AuditPreCheck()
        {
            if (CurrentSO.BaseInfo.Status != SOStatus.Origin)
            {
                throw new BizException("订单不是待审核状态");
            }

            if (CurrentSO.BaseInfo.SOType.Value != SOType.BuyMobileSettlement)
            {
                throw new BizException("不是购机结算订单类型");
            }
        }
        protected override SOStatus GetAuditStatus()
        {
            return SOStatus.OutStock;
        }

        protected override void SaveAudit(SOStatus nextStatus)
        {
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                base.SaveAudit(nextStatus);
                SODA.UpdateSOOutStockTime(SOSysNo);
                scope.Complete();
            }
        }

        public override void Audit()
        {
            AuditPreCheck();

            SaveAudit(GetAuditStatus());

            SplitPrice();

            CreateSOIncome();

            //创建分公司收款单
            CreateInvoice(CurrentSO);

            WriteLog("IPP审核购机结算订单");
        }

        private void CreateSOIncome()
        {
            SOIncomeInfo soIncomeInfo = CurrentSOIncomeInfo;
            if (soIncomeInfo == null)
            {
                soIncomeInfo = new SOIncomeInfo();
                soIncomeInfo.OrderType = SOIncomeOrderType.SO;
                soIncomeInfo.OrderSysNo = SOSysNo;
                soIncomeInfo.OrderAmt = UtilityHelper.TruncMoney(CurrentSO.BaseInfo.SOTotalAmount);
                soIncomeInfo.IncomeAmt = CurrentSO.BaseInfo.ReceivableAmount;
                soIncomeInfo.PrepayAmt = Math.Max(CurrentSO.BaseInfo.PrepayAmount.Value, 0);
                soIncomeInfo.IncomeStyle = (int)SOIncomeOrderStyle.Normal;
                soIncomeInfo.Status = (int)SOIncomeStatus.Origin;
                soIncomeInfo.GiftCardPayAmt = CurrentSO.BaseInfo.GiftCardPay;
                soIncomeInfo.PointPay = CurrentSO.BaseInfo.PointPay;
                soIncomeInfo.PayAmount = CurrentSO.BaseInfo.ReceivableAmount;
                soIncomeInfo.CompanyCode = CurrentSO.CompanyCode;

                if (CurrentSO.BaseInfo.SOSplitMaster != null)
                {
                    soIncomeInfo.MasterSoSysNo = CurrentSO.BaseInfo.SOSplitMaster.Value;  //获取母单号
                }
                ExternalDomainBroker.CreateSOIncome(soIncomeInfo);
            }
        }
    }

    #endregion 订单审核
}