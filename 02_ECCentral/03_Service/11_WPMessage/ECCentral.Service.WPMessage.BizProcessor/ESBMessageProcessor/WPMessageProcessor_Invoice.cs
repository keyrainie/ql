using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.EventMessage.Invoice;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.WPMessage.BizProcessor
{
    #region 退款审核

    /// <summary>
    /// 创建会计审核待办事项
    /// </summary>
    public class CreateRefundAccountantAudit_CreateTask : WPMessageCreator<CreateRefundAccountantAuditMessage>
    {
        protected override bool NeedProcess(CreateRefundAccountantAuditMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.RefundSysNo <= 0)
            //{
            //    return false;
            //}

            //SOIncomeRefundInfo soincomeRefundInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetSOIncomeRefund(msg.RefundSysNo);
            //if (soincomeRefundInfo == null)
            //    return false;

            //return soincomeRefundInfo.Status == RefundStatus.WaitingAccountantAudit;
        }

        protected override int GetCategorySysNo()
        {
            //会计审核待办事项categorySysno
            return 300;
        }

        protected override string GetBizSysNo(CreateRefundAccountantAuditMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateRefundAccountantAuditMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(CreateRefundAccountantAuditMessage msg)
        {
            return "创建会计审核待办事项";
        }

        protected override int GetCurrentUserSysNo(CreateRefundAccountantAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 创建出纳退款待办事项
    /// </summary>
    public class CreateRefundCashierRefund_CreateTask : WPMessageCreator<CreateRefundCashierRefundMessage>
    {
        protected override bool NeedProcess(CreateRefundCashierRefundMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.RefundSysNo <= 0)
            //{
            //    return false;
            //}

            //SOIncomeRefundInfo soincomeRefundInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetSOIncomeRefund(msg.RefundSysNo);
            //if (soincomeRefundInfo == null)
            //    return false;

            //return soincomeRefundInfo.Status == RefundStatus.WaitingCashierRefund;
        }

        protected override int GetCategorySysNo()
        {
            //出纳退款待办事项categorySysno
            return 308;
        }

        protected override string GetBizSysNo(CreateRefundCashierRefundMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateRefundCashierRefundMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(CreateRefundCashierRefundMessage msg)
        {
            return "创建出纳退款待办事项";
        }

        protected override int GetCurrentUserSysNo(CreateRefundCashierRefundMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 出纳退款-关闭RO单提交审核待办事项
    /// </summary>
    public class RefundAuditCashierRefunded_CompleteROAuditTask : WPMessageCompleter<RefundAuditCashierRefundedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //RO单提交审核待办事项categorySysno
            return 300;
        }

        protected override string GetBizSysNo(RefundAuditCashierRefundedMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RefundAuditCashierRefundedMessage msg)
        {
            return "出纳退款-关闭RO单提交审核待办事项";
        }

        protected override int GetCurrentUserSysNo(RefundAuditCashierRefundedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 出纳退款-关闭出纳退款待办事项
    /// </summary>
    public class RefundAuditCashierRefunded_CompleteAuditTask : WPMessageCompleter<RefundAuditCashierRefundedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //出纳退款待办事项categorySysno
            return 308;
        }

        protected override string GetBizSysNo(RefundAuditCashierRefundedMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RefundAuditCashierRefundedMessage msg)
        {
            return "出纳退款-关闭出纳退款待办事项";
        }

        protected override int GetCurrentUserSysNo(RefundAuditCashierRefundedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 会计审核通过-创建出纳退款待办事项
    /// </summary>
    public class RefundAuditAccountantPassed_CreateAuditTask : WPMessageCreator<RefundAuditAccountantPassedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //出纳退款待办事项categorySysno
            return 308;
        }

        protected override string GetBizSysNo(RefundAuditAccountantPassedMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetUrlParameter(RefundAuditAccountantPassedMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RefundAuditAccountantPassedMessage msg)
        {
            return "审核通过-创建出纳退款待办事项";
        }

        protected override int GetCurrentUserSysNo(RefundAuditAccountantPassedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
        protected override bool NeedProcess(RefundAuditAccountantPassedMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.RefundSysNo <= 0)
            //{
            //    return false;
            //}

            //SOIncomeRefundInfo soincomeRefundInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetSOIncomeRefund(msg.RefundSysNo);
            //if (soincomeRefundInfo == null)
            //    return false;

            //return soincomeRefundInfo.Status == RefundStatus.WaitingCashierRefund;
        }
    }
    /// <summary>
    /// 会计审核通过-关闭会计审核待办事项
    /// </summary>
    public class RefundAuditAccountantPassed_CompleteAuditTask : WPMessageCompleter<RefundAuditAccountantPassedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //会计审核待办事项categorySysno
            return 300;
        }

        protected override string GetBizSysNo(RefundAuditAccountantPassedMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RefundAuditAccountantPassedMessage msg)
        {
            return "审核通过-关闭会计审核待办事项";
        }

        protected override int GetCurrentUserSysNo(RefundAuditAccountantPassedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 会计审核拒绝-关闭会计审核待办事项
    /// </summary>
    public class RefundAuditAccountantRejected_CompleteAuditTask : WPMessageCompleter<RefundAuditAccountantRejectedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //会计审核待办事项categorySysno
            return 300;
        }

        protected override string GetBizSysNo(RefundAuditAccountantRejectedMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RefundAuditAccountantRejectedMessage msg)
        {
            return "审核拒绝-关闭会计审核待办事项";
        }

        protected override int GetCurrentUserSysNo(RefundAuditAccountantRejectedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    #endregion

    #region 销售收款单

    /// <summary>
    /// 新建销售收款单-创建确认收款单待办事项
    /// </summary>
    public class CreateSOIncome_CreateConfirmTask : WPMessageCreator<CreateSOIncomeMessage>
    {
        protected override bool NeedProcess(CreateSOIncomeMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.SOIncomeSysNo <= 0)
            //{
            //    return false;
            //}

            //SOIncomeInfo soincomeInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetSOIncome(msg.SOIncomeSysNo);
            //if (soincomeInfo == null)
            //{
            //    return false;
            //}

            //return soincomeInfo.Status == SOIncomeStatus.Origin;
        }

        protected override int GetCategorySysNo()
        {
            //确认收款单待办事项categorySysno
            return 301;
        }

        protected override string GetBizSysNo(CreateSOIncomeMessage msg)
        {
            return msg.SOIncomeSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateSOIncomeMessage msg)
        {
            return msg.SOIncomeSysNo.ToString();
        }

        protected override string GetMemo(CreateSOIncomeMessage msg)
        {
            return "新建-创建确认收款单待办事项";
        }

        protected override int GetCurrentUserSysNo(CreateSOIncomeMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 取消确认销售收款单-创建确认收款单待办事项
    /// </summary>
    public class SOIncomeConfirmCanceled_CreateConfirmTask : WPMessageCreator<SOIncomeConfirmCanceledMessage>
    {
        protected override bool NeedProcess(SOIncomeConfirmCanceledMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.SOIncomeSysNo <= 0)
            //{
            //    return false;
            //}

            //SOIncomeInfo soincomeInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetSOIncome(msg.SOIncomeSysNo);
            //if (soincomeInfo == null)
            //{
            //    return false;
            //}

            //return soincomeInfo.Status == SOIncomeStatus.Origin;
        }

        protected override int GetCategorySysNo()
        {
            //确认收款单待办事项categorySysno
            return 301;
        }

        protected override string GetBizSysNo(SOIncomeConfirmCanceledMessage msg)
        {
            return msg.SOIncomeSysNo.ToString();
        }

        protected override string GetUrlParameter(SOIncomeConfirmCanceledMessage msg)
        {
            return msg.SOIncomeSysNo.ToString();
        }

        protected override string GetMemo(SOIncomeConfirmCanceledMessage msg)
        {
            return "取消-创建确认收款单待办事项";
        }

        protected override int GetCurrentUserSysNo(SOIncomeConfirmCanceledMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 确认销售收款单-完成确认收款单待办事项
    /// </summary>
    public class SOIncomeConfirmed_CompleteConfirmTask : WPMessageCompleter<SOIncomeConfirmedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //确认收款单待办事项categorySysno
            return 301;
        }

        protected override string GetBizSysNo(SOIncomeConfirmedMessage msg)
        {
            return msg.SOIncomeSysNo.ToString();
        }

        protected override string GetMemo(SOIncomeConfirmedMessage msg)
        {
            return "确认-完成确认收款单待办事项";
        }

        protected override int GetCurrentUserSysNo(SOIncomeConfirmedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废销售收款单-完成确认收款单待办事项
    /// </summary>
    public class SOIncomeAbandoned_CompleteConfirmTask : WPMessageCompleter<SOIncomeAbandonedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //确认收款单待办事项categorySysno
            return 301;
        }

        protected override string GetBizSysNo(SOIncomeAbandonedMessage msg)
        {
            return msg.SOIncomeSysNo.ToString();
        }

        protected override string GetMemo(SOIncomeAbandonedMessage msg)
        {
            return "作废-完成确认收款单待办事项";
        }

        protected override int GetCurrentUserSysNo(SOIncomeAbandonedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 拆分销售收款单，关闭主单待办事项，新建子单待办事项
    /// </summary>
    public class SOIncomeSplited_NewAndCompleteConfirmTask : IESBMessageProcessor
    {
        public void Process(ESBMessage message)
        {
            if (message == null)
            {
                return;
            }
            SOIncomeSplitedMessage data = message.GetData<SOIncomeSplitedMessage>();
            if (data == null)
            {
                return;
            }
            if (data.MasterSOIncomeSysNo > 0)
            {
                CompleteTask(ConvertToTask(data.MasterSOIncomeSysNo, "拆分主单-完成主单确认收款单待办事项"
                    , data.CurrentUserSysNo));
            }
            if (data.SubSOIncomeSysNoList != null && data.SubSOIncomeSysNoList.Count > 0)
            {
                for (int i = 0; i < data.SubSOIncomeSysNoList.Count && data.SubSOIncomeSysNoList[i] > 0; i++)
                {
                    NewTask(ConvertToTask(data.SubSOIncomeSysNoList[i], "拆分主单-创建子单确认收款单待办事项"
                        , data.CurrentUserSysNo));
                }
            }
        }

        private void NewTask(Task task)
        {
            ObjectFactory<WPMessage.BizProcessor.WPMessageProcessor>.Instance.AddWPMessage(task.CategorySysNo,
                task.BizSysNo, task.UrlParameter, task.CurrentUserSysNo, task.Memo);
        }

        private void CompleteTask(Task task)
        {
            ObjectFactory<WPMessage.BizProcessor.WPMessageProcessor>.Instance.CompleteWPMessage(task.CategorySysNo,
                task.BizSysNo, task.CurrentUserSysNo, task.Memo);
        }

        private Task ConvertToTask(int sysno, string memo, int userSysno)
        {
            return new Task
            {
                CategorySysNo = 301,
                BizSysNo = sysno.ToString(),
                UrlParameter = sysno.ToString(),
                Memo = memo,
                CurrentUserSysNo = userSysno
            };
        }

    }
    #endregion

    #region 用户余额退款审核

    /// <summary>
    /// 创建CS审核用户余额退款待办事项
    /// </summary>
    public class CreateBalanceRefund_CreateCSAuditTask : WPMessageCreator<CreateBalanceRefundMessage>
    {
        protected override bool NeedProcess(CreateBalanceRefundMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.ReturnPrepaySysNo <= 0)
            //{
            //    return false;
            //}

            //BalanceRefundInfo refundInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetBalanceRefund(msg.ReturnPrepaySysNo);
            //if (refundInfo == null)
            //    return false;

            //return refundInfo.Status == BalanceRefundStatus.Origin;
        }

        protected override int GetCategorySysNo()
        {
            //CS审核用户余额退款待办事项categorySysno
            return 302;
        }

        protected override string GetBizSysNo(CreateBalanceRefundMessage msg)
        {
            return msg.ReturnPrepaySysNo.ToString();
        }

        protected override string GetUrlParameter(CreateBalanceRefundMessage msg)
        {
            return msg.ReturnPrepaySysNo.ToString();
        }

        protected override string GetMemo(CreateBalanceRefundMessage msg)
        {
            return "创建CS审核用户余额退款代办事项";
        }

        protected override int GetCurrentUserSysNo(CreateBalanceRefundMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 创建财务审核用户余额退款待办事项
    /// </summary>
    public class BalanceRefundCSConfirmed_CreateFinAuditTask : WPMessageCreator<BalanceRefundCSConfirmedMessage>
    {
        protected override bool NeedProcess(BalanceRefundCSConfirmedMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.ReturnPrepaySysNo <= 0)
            //{
            //    return false;
            //}
            //BalanceRefundInfo refundInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetBalanceRefund(msg.ReturnPrepaySysNo);
            //if (refundInfo == null)
            //{
            //    return false;
            //}
            //return refundInfo.Status == BalanceRefundStatus.CSConfirmed;
        }

        protected override int GetCategorySysNo()
        {
            //财务审核用户余额退款待办事项categorySysno
            return 303;
        }

        protected override string GetBizSysNo(BalanceRefundCSConfirmedMessage msg)
        {
            return msg.ReturnPrepaySysNo.ToString();
        }

        protected override string GetUrlParameter(BalanceRefundCSConfirmedMessage msg)
        {
            return msg.ReturnPrepaySysNo.ToString();
        }

        protected override string GetMemo(BalanceRefundCSConfirmedMessage msg)
        {
            return "创建财务审核用户余额退款代办事项";
        }

        protected override int GetCurrentUserSysNo(BalanceRefundCSConfirmedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 关闭CS审核用户余额退款代办事项
    /// </summary>
    public class BalanceRefundCSConfirmed_CompleteCSAuditTask : WPMessageCompleter<BalanceRefundCSConfirmedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //CS审核用户余额退款待办事项categorySysno
            return 302;
        }

        protected override string GetBizSysNo(BalanceRefundCSConfirmedMessage msg)
        {
            return msg.ReturnPrepaySysNo.ToString();
        }

        protected override string GetMemo(BalanceRefundCSConfirmedMessage msg)
        {
            return "完成CS审核用户余额退款代办事项";
        }

        protected override int GetCurrentUserSysNo(BalanceRefundCSConfirmedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 关闭财务审核用户余额退款代办事项
    /// </summary>
    public class BalanceRefundFinConfirmed_CompleteFinAuditTask : WPMessageCompleter<BalanceRefundFinConfirmedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //财务审核用户余额退款待办事项categorySysno
            return 303;
        }

        protected override string GetBizSysNo(BalanceRefundFinConfirmedMessage msg)
        {
            return msg.ReturnPrepaySysNo.ToString();
        }

        protected override string GetMemo(BalanceRefundFinConfirmedMessage msg)
        {
            return "完成财务审核用户余额退款代办事项";
        }

        protected override int GetCurrentUserSysNo(BalanceRefundFinConfirmedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废消息处理器
    /// </summary>
    public class BalanceRefundAbandoned_CompleteAuditTask : WPMessageCompleter<BalanceRefundAbandonedMessage>
    {
        private static Dictionary<int, dynamic> Dic = new Dictionary<int, dynamic>()
        {
            {
                (int)BalanceRefundStatus.Origin,new { CategorySysNo = 302, Memo = "作废用户余额退款关闭CS审核待办事项" }
            },
            {
                (int)BalanceRefundStatus.CSConfirmed,new { CategorySysNo = 303, Memo = "作废用户余额退款关闭财务审核待办事项" }
            }
        };

        private BalanceRefundAbandonedMessage _abandonedMsg;
        protected override bool NeedProcess(BalanceRefundAbandonedMessage msg)
        {
            _abandonedMsg = msg;
            return msg.ReturnPrepaySysNo > 0;
        }

        protected override int GetCategorySysNo()
        {
            dynamic val = null;
            if (_abandonedMsg != null && Dic.TryGetValue(_abandonedMsg.LastRefundStatus, out val))
            {
                return val.CategorySysNo;
            }
            return 0;
        }

        protected override string GetBizSysNo(BalanceRefundAbandonedMessage msg)
        {
            return msg.ReturnPrepaySysNo.ToString();
        }

        protected override string GetMemo(BalanceRefundAbandonedMessage msg)
        {
            dynamic val = null;
            if (_abandonedMsg != null && Dic.TryGetValue(_abandonedMsg.LastRefundStatus, out val))
            {
                return val.Memo;
            }
            return string.Empty;
        }

        protected override int GetCurrentUserSysNo(BalanceRefundAbandonedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    #endregion

    #region netpay审核

    /// <summary>
    /// 添加审核netpay待办事项
    /// </summary>
    public class CreateNetpay_CreateAuditTask : WPMessageCreator<CreateNetpayMessage>
    {
        protected override bool NeedProcess(CreateNetpayMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.NetpaySysNo <= 0)
            //{
            //    return false;
            //}

            //NetPayInfo netpayInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetNetPay(msg.NetpaySysNo);
            //if (netpayInfo == null)
            //{
            //    return false;
            //}
            //return netpayInfo.Status == NetPayStatus.Origin;
        }

        protected override int GetCategorySysNo()
        {
            //审核netpay待办事项categorySysno
            return 304;
        }

        protected override string GetBizSysNo(CreateNetpayMessage msg)
        {
            return msg.NetpaySysNo.ToString();
        }

        protected override string GetUrlParameter(CreateNetpayMessage msg)
        {
            return msg.NetpaySysNo.ToString();
        }

        protected override string GetMemo(CreateNetpayMessage msg)
        {
            return "创建审核netpay代办事项";
        }

        protected override int GetCurrentUserSysNo(CreateNetpayMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// netpay审核通过关闭netpay审核待办事项
    /// </summary>
    public class NetpayAudited_CompleteAuditTask : WPMessageCompleter<InvoiceNetpayAuditedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //审核netpay待办事项categorySysno
            return 304;
        }

        protected override string GetBizSysNo(InvoiceNetpayAuditedMessage msg)
        {
            return msg.NetpaySysNo.ToString();
        }

        protected override string GetMemo(InvoiceNetpayAuditedMessage msg)
        {
            return "审核通过netpay关闭审核netpay待办事项";
        }

        protected override int GetCurrentUserSysNo(InvoiceNetpayAuditedMessage msg)
        {
            return msg.AuditUserSysNo;
        }
    }

    /// <summary>
    /// 创建订单审核待办事项
    /// </summary>
    public class NetpayAudited_CreateSOAuditTask : WPMessageCreator<InvoiceNetpayAuditedMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 201;
        }

        protected override string GetBizSysNo(InvoiceNetpayAuditedMessage msg)
        {
            return msg.SoSysNo.ToString();
        }

        protected override string GetUrlParameter(InvoiceNetpayAuditedMessage msg)
        {
            return msg.SoSysNo.ToString();
        }

        protected override string GetMemo(InvoiceNetpayAuditedMessage msg)
        {
            return null;
        }

        protected override int GetCurrentUserSysNo(InvoiceNetpayAuditedMessage msg)
        {
            return msg.AuditUserSysNo;
        }
        protected override bool NeedProcess(InvoiceNetpayAuditedMessage msg)
        {
            ECCentral.BizEntity.SO.SOInfo soInfo = ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SoSysNo);
            if (soInfo == null)
            {
                return false;
            }
            //订单不需要拆分
            return soInfo.BaseInfo.SplitType == ECCentral.BizEntity.SO.SOSplitType.Normal;
        }
    }

    /// <summary>
    /// netpay作废 关闭netpay审核待办事项
    /// </summary>
    public class NetpayAbandoned_CompleteAuditTask : WPMessageCompleter<NetpayAbandonedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //审核netpay待办事项categorySysno
            return 304;
        }

        protected override string GetBizSysNo(NetpayAbandonedMessage msg)
        {
            return msg.NetpaySysNo.ToString();
        }

        protected override string GetMemo(NetpayAbandonedMessage msg)
        {
            return "作废netpay关闭审核netpay待办事项";
        }

        protected override int GetCurrentUserSysNo(NetpayAbandonedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }


    public class NetpayAudited_CreateAssignCustomerConsultTask : WPMessageCreator<InvoiceNetpayAuditedMessage>
    {

        protected override int GetCategorySysNo()
        {
            return 104;
        }

        protected override string GetBizSysNo(InvoiceNetpayAuditedMessage msg)
        {
            return msg.ReferenceSysNo.ToString();
        }

        protected override string GetUrlParameter(InvoiceNetpayAuditedMessage msg)
        {
            return msg.ReferenceSysNo.ToString();
        }

        protected override string GetMemo(InvoiceNetpayAuditedMessage msg)
        {
            return "客户行程咨询已支付，请分配。";
        }

        protected override int GetCurrentUserSysNo(InvoiceNetpayAuditedMessage msg)
        {
            return msg.AuditUserSysNo;
        }
        protected override bool NeedProcess(InvoiceNetpayAuditedMessage msg)
        {
            throw new NotImplementedException();
            //ECCentral.BizEntity.SO.SOInfo soInfo = ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SoSysNo);
            //if (soInfo == null)
            //{
            //    return false;
            //}
            //if (soInfo.BaseInfo.Status != ECCentral.BizEntity.SO.SOStatus.Abandon && soInfo.BaseInfo.SOType == ECCentral.BizEntity.SO.SOType.TravelRequest && soInfo.BaseInfo.ReferenceSysNo.HasValue)
            //{
            //    ECCentral.BizEntity.IM.DIYCustomerConsult consult = ObjectFactory<IIMBizInteract>.Instance.GetDIYCustomerConsultByID(msg.ReferenceSysNo.Value);
            //    return consult != null && (!consult.IsAssigned.HasValue || consult.IsAssigned == ECCentral.BizEntity.IM.DIYConsultIsAssigned.No) && consult.Status == ECCentral.BizEntity.IM.DIYConsultStatus.Submit;
            //}
            //return false;
        }
    }
    #endregion

    #region 用户余额充值
    /// <summary>
    /// 创建用户余额充值审核待办事项
    /// </summary>
    public class CreateBalanceRechargeRequest_CreateAuditTask : WPMessageCreator<CreateBalanceRechargeRequestMessage>
    {
        protected override bool NeedProcess(CreateBalanceRechargeRequestMessage msg)
        {
            throw new NotImplementedException();
            //if (msg.RequestSysNo <= 0)
            //{
            //    return false;
            //}

            //BalanceRechargeRequestInfo requestInfo = ObjectFactory<IInvoiceBizInteract>.Instance.GetBalanceRechargeRequest(msg.RequestSysNo);
            //if (requestInfo == null)
            //{
            //    return false;
            //}

            //return requestInfo.Status == BalanceRechargeStatus.WaitingAudit;
        }

        protected override int GetCategorySysNo()
        {
            //用户余额充值审核待办事项categorySysno
            return 305;
        }

        protected override string GetBizSysNo(CreateBalanceRechargeRequestMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateBalanceRechargeRequestMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(CreateBalanceRechargeRequestMessage msg)
        {
            return "创建用户余额充值审核待办事项";
        }

        protected override int GetCurrentUserSysNo(CreateBalanceRechargeRequestMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 用户余额充值申请“审核通过”，关闭充值审核待办事项
    /// </summary>
    public class BalanceRechargeRequestAudited_CompleteAuditTask : WPMessageCompleter<BalanceRechargeRequestAuditedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //用户余额充值审核待办事项categorySysno
            return 305;
        }

        protected override string GetBizSysNo(BalanceRechargeRequestAuditedMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(BalanceRechargeRequestAuditedMessage msg)
        {
            return "审核通过关闭用户余额充值审核待办事项";
        }

        protected override int GetCurrentUserSysNo(BalanceRechargeRequestAuditedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 用户余额充值申请“作废”，关闭充值审核待办事项
    /// </summary>
    public class BalanceRechargeRequestAbandoned_CompleteAuditTask : WPMessageCompleter<BalanceRechargeRequestAbandonedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //用户余额充值审核待办事项categorySysno
            return 305;
        }

        protected override string GetBizSysNo(BalanceRechargeRequestAbandonedMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(BalanceRechargeRequestAbandonedMessage msg)
        {
            return "作废关闭用户余额充值审核待办事项";
        }

        protected override int GetCurrentUserSysNo(BalanceRechargeRequestAbandonedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    #endregion


    #region 电汇邮局收款单 306

    /// <summary>
    /// 创建用户余额充值审核待办事项
    /// </summary>
    public class CreatePostIncomeInfoMessage_CreateAuditTask : WPMessageCreator<CreatePostIncomeInfoMessage>
    {
        protected override bool NeedProcess(CreatePostIncomeInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {

            return 306;
        }

        protected override string GetBizSysNo(CreatePostIncomeInfoMessage msg)
        {
            return msg.PostIncomeInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CreatePostIncomeInfoMessage msg)
        {
            return msg.PostIncomeInfoSysNo.ToString();
        }

        protected override string GetMemo(CreatePostIncomeInfoMessage msg)
        {
            return "创建电汇邮局收款单";
        }

        protected override int GetCurrentUserSysNo(CreatePostIncomeInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }


    /// <summary>
    /// 确认电汇邮局收款单
    /// </summary>
    public class ConfirmPostIncomeInfoMessagee_ConfirmTask : WPMessageCompleter<ConfirmPostIncomeInfoMessage>
    {
        protected override int GetCategorySysNo()
        {

            return 306;
        }



        protected override string GetBizSysNo(ConfirmPostIncomeInfoMessage msg)
        {
            return msg.PostIncomeInfoSysNo.ToString();
        }

        protected override string GetMemo(ConfirmPostIncomeInfoMessage msg)
        {
            return "确认电汇邮局收款单";
        }

        protected override int GetCurrentUserSysNo(ConfirmPostIncomeInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 取消确认电汇邮局收款单
    /// </summary>
    public class CancelConfirmPostIncomeMessage_CancelConfirmTask : WPMessageCompleter<CancelConfirmPostIncomeMessage>
    {


        protected override int GetCategorySysNo()
        {

            return 306;
        }

        protected override string GetBizSysNo(CancelConfirmPostIncomeMessage msg)
        {
            return msg.PostIncomeInfoSysNo.ToString();
        }


        protected override string GetMemo(CancelConfirmPostIncomeMessage msg)
        {
            return "取消确认电汇邮局收款单";
        }

        protected override int GetCurrentUserSysNo(CancelConfirmPostIncomeMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }


    /// <summary>
    ///作废电汇邮局收款单
    /// </summary>
    public class VoidPostIncomeInfoMessage_Completor : WPMessageCompleter<VoidPostIncomeInfoMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 306;
        }

        protected override string GetBizSysNo(VoidPostIncomeInfoMessage msg)
        {
            return msg.PostIncomeInfoSysNo.ToString();
        }

        protected override string GetMemo(VoidPostIncomeInfoMessage msg)
        {
            return "作废电汇邮局收款单";
        }

        protected override int GetCurrentUserSysNo(VoidPostIncomeInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    /// <summary>
    ///取消作废电汇邮局收款单
    /// </summary>
    public class CancelVoidPostIncomeInfoMessage_CancelVoid : WPMessageCompleter<CancelVoidPostIncomeInfoMessage>
    {


        protected override int GetCategorySysNo()
        {

            return 306;
        }

        protected override string GetBizSysNo(CancelVoidPostIncomeInfoMessage msg)
        {
            return msg.PostIncomeInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelVoidPostIncomeInfoMessage msg)
        {
            return "取消作废电汇邮局收款单";
        }

        protected override int GetCurrentUserSysNo(CancelVoidPostIncomeInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    #endregion


    #region CS审核客户余额退款

    /// <summary>
    /// CS审核客户余额退款
    /// </summary>
    public class RefundCSAuditMessage_Audit : WPMessageCompleter<RefundCSAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 302;
        }

        protected override string GetBizSysNo(RefundCSAuditMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RefundCSAuditMessage msg)
        {
            return "CS审核客户余额退款";
        }

        protected override int GetCurrentUserSysNo(RefundCSAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// CS取消审核客户余额退款
    /// </summary>
    public class RefundCSCancelAuditMessage_CancelAudit : WPMessageCompleter<RefundCSCancelAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 302;
        }

        protected override string GetBizSysNo(RefundCSCancelAuditMessage msg)
        {
            return msg.RefundSysNo.ToString();
        }

        protected override string GetMemo(RefundCSCancelAuditMessage msg)
        {
            return "CS取消审核客户余额退款";
        }

        protected override int GetCurrentUserSysNo(RefundCSCancelAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    #endregion
}
