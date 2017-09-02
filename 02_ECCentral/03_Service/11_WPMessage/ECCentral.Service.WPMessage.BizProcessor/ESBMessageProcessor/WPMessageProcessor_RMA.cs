using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.RMA;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.WPMessage.BizProcessor
{

    //20130604 Victor Added MessageClass:
    /// <summary>
    /// 创建RMA申请单待确认  - 待办事项
    /// </summary>
    public class RMARequestWaitingForAuditCreator : WPMessageCreator<RMACreateRequestWaitingForAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 500;
        }

        protected override string GetBizSysNo(RMACreateRequestWaitingForAuditMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetUrlParameter(RMACreateRequestWaitingForAuditMessage msg)
        {
            return string.Format("{0}&{1}", msg.SOSysNo.ToString(), msg.RequestSysNo.ToString());
        }

        protected override string GetMemo(RMACreateRequestWaitingForAuditMessage msg)
        {
            return "创建RMA申请单待审核待办事项.";
        }

        protected override int GetCurrentUserSysNo(RMACreateRequestWaitingForAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 完成RMA申请单待确认 - 待办事项
    /// </summary>
    public class RMARequestWaitingForAuditCompleter : WPMessageCompleter<RMACompleteRequestWaitingForAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 500;
        }

        protected override string GetBizSysNo(RMACompleteRequestWaitingForAuditMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(RMACompleteRequestWaitingForAuditMessage msg)
        {
            return "完成RMA申请单待审核待办事项.";
        }

        protected override int GetCurrentUserSysNo(RMACompleteRequestWaitingForAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 创建RMA退款单待提交审核 - 待办事项
    /// </summary>
    public class RMARefundWaitingForSubmitCreator : WPMessageCreator<RMACreateRefundWaitingForSubmitMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 501;
        }

        protected override string GetBizSysNo(RMACreateRefundWaitingForSubmitMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetUrlParameter(RMACreateRefundWaitingForSubmitMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RMACreateRefundWaitingForSubmitMessage msg)
        {
            return "创建RMA退款单待审核待办事项.";
        }

        protected override int GetCurrentUserSysNo(RMACreateRefundWaitingForSubmitMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 完成RMA退款单待提交审核 - 待办事项
    /// </summary>
    public class RMARefundWaitingForSubmitCompleter : WPMessageCompleter<RMACompleteRefundWaitingForSubmitMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 501;
        }

        protected override string GetBizSysNo(RMACompleteRefundWaitingForSubmitMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RMACompleteRefundWaitingForSubmitMessage msg)
        {
            return "完成RMA退款单待审核待办事项.";
        }

        protected override int GetCurrentUserSysNo(RMACompleteRefundWaitingForSubmitMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 创建RMA退款调整单待提交审核 - 待办事项
    /// </summary>
    public class RMARefundBalanceWaitingForAuditCreator : WPMessageCreator<RMACreateRefundBalanceWaitingForAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 502;
        }

        protected override string GetBizSysNo(RMACreateRefundBalanceWaitingForAuditMessage msg)
        {
            return msg.RefundBalanceSysNo.ToString();
        }

        protected override string GetUrlParameter(RMACreateRefundBalanceWaitingForAuditMessage msg)
        {
            return string.Format("{0}&{1}", msg.RefundSysNo, msg.RefundBalanceSysNo.ToString());
        }

        protected override string GetMemo(RMACreateRefundBalanceWaitingForAuditMessage msg)
        {
            return "创建RMA退款单调整单待审核待办事项.";
        }

        protected override int GetCurrentUserSysNo(RMACreateRefundBalanceWaitingForAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 完成RMA退款调整单待提交审核 - 待办事项
    /// </summary>
    public class RMARefundBalanceWaitingForAuditCompleter : WPMessageCompleter<RMACompleteRefundBalanceWaitingForAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 502;
        }

        protected override string GetBizSysNo(RMACompleteRefundBalanceWaitingForAuditMessage msg)
        {
            return msg.RefundBalanceSysNo.ToString();
        }

        protected override string GetMemo(RMACompleteRefundBalanceWaitingForAuditMessage msg)
        {
            return "完成RMA退款单调整单待审核待办事项.";
        }

        protected override int GetCurrentUserSysNo(RMACompleteRefundBalanceWaitingForAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// RMA-RO单提交审核 - 待办事项
    /// </summary>
    public class RMAROSubmitCreator : WPMessageCreator<RMAROSubmitMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 300;
        }

        protected override string GetBizSysNo(RMAROSubmitMessage msg)
        {
            return msg.SOIncomeRefundSysNo.ToString();
        }

        protected override string GetUrlParameter(RMAROSubmitMessage msg)
        {
            return msg.SOIncomeRefundSysNo.ToString();
        }

        protected override string GetMemo(RMAROSubmitMessage msg)
        {
            return "RO单提交审核.";
        }

        protected override int GetCurrentUserSysNo(RMAROSubmitMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// RMA-RO单取消提交审核  - 待办事项
    /// </summary>
    public class RMAROCancelSubmitCreator : WPMessageCompleter<RMAROCancelSubmitMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 300;
        }

        protected override string GetBizSysNo(RMAROCancelSubmitMessage msg)
        {
            return msg.SOIncomeRefundSysNo.ToString();
        }

        protected override string GetMemo(RMAROCancelSubmitMessage msg)
        {
            return "RO单取消提交审核.";
        }

        protected override int GetCurrentUserSysNo(RMAROCancelSubmitMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// ROt退款调整单 - 取消审核 待办事项
    /// </summary>
    public class RMAROBalanceCancelAuditCompletor : WPMessageCompleter<RMAROBalanceCancelAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 300;
        }

        protected override string GetBizSysNo(RMAROBalanceCancelAuditMessage msg)
        {
            return msg.SOIncomeRefundSysNo.ToString();
        }

        protected override string GetMemo(RMAROBalanceCancelAuditMessage msg)
        {
            return "取消RO退款调整单退款审核代办事项";
        }

        protected override int GetCurrentUserSysNo(RMAROBalanceCancelAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

}
