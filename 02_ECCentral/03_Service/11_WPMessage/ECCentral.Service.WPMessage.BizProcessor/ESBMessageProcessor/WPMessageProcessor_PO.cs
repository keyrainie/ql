using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.PO;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.WPMessage.BizProcessor
{
    #region 代收结算单
    
    //Category 706&707

    /// <summary>
    /// 新建代收结算单-创建审核待办事项
    /// </summary>
    public class CreateGatherSettlement_CreateAuditTask : WPMessageCreator<CreateGatherSettlementMessage>
    {
        protected override bool NeedProcess(CreateGatherSettlementMessage msg)
        {
            if (msg.SettlementSysNo <= 0)
            {
                return false;
            }

            GatherSettlementInfo gatherSettlementInfo = ObjectFactory<IPOBizInteract>.Instance.GetGatherSettlementInfo(msg.SettlementSysNo);
            if (gatherSettlementInfo == null)
            {
                return false;
            }

            return gatherSettlementInfo.SettleStatus == GatherSettleStatus.ORG;
        }

        protected override int GetCategorySysNo()
        {
            //审核代收结算单待办事项categorySysno
            return 706;
        }

        protected override string GetBizSysNo(CreateGatherSettlementMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateGatherSettlementMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetMemo(CreateGatherSettlementMessage msg)
        {
            return "新建-创建审核待办事项";
        }

        protected override int GetCurrentUserSysNo(CreateGatherSettlementMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核代收结算单-创建结算待办事项
    /// </summary>
    public class AuditedGatherSettlement_CreateSettleTask : WPMessageCreator<GatherSettlementAuditedMessage>
    {
        protected override bool NeedProcess(GatherSettlementAuditedMessage msg)
        {
            if (msg.SettlementSysNo <= 0)
            {
                return false;
            }

            GatherSettlementInfo gatherSettlementInfo = ObjectFactory<IPOBizInteract>.Instance.GetGatherSettlementInfo(msg.SettlementSysNo);
            if (gatherSettlementInfo == null)
            {
                return false;
            }

            return gatherSettlementInfo.SettleStatus == GatherSettleStatus.AUD;
        }

        protected override int GetCategorySysNo()
        {
            //结算代收结算单待办事项categorySysno
            return 707;
        }

        protected override string GetBizSysNo(GatherSettlementAuditedMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetUrlParameter(GatherSettlementAuditedMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetMemo(GatherSettlementAuditedMessage msg)
        {
            return "审核-创建结算待办事项";
        }

        protected override int GetCurrentUserSysNo(GatherSettlementAuditedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 审核代收结算单-完成审核待办事项
    /// </summary>
    public class AuditedGatherSettlement_CompleteAuditTask : WPMessageCompleter<GatherSettlementAuditedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //审核代收结算单待办事项categorySysno
            return 706;
        }

        protected override string GetBizSysNo(GatherSettlementAuditedMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetMemo(GatherSettlementAuditedMessage msg)
        {
            return "审核-完成审核待办事项";
        }

        protected override int GetCurrentUserSysNo(GatherSettlementAuditedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 取消审核代收结算单-创建审核待办事项
    /// </summary>
    public class AuditCanceledGatherSettlement_CreateAuditTask : WPMessageCreator<GatherSettlementAuditCanceledMessage>
    {
        protected override bool NeedProcess(GatherSettlementAuditCanceledMessage msg)
        {
            if (msg.SettlementSysNo <= 0)
            {
                return false;
            }

            GatherSettlementInfo gatherSettlementInfo = ObjectFactory<IPOBizInteract>.Instance.GetGatherSettlementInfo(msg.SettlementSysNo);
            if (gatherSettlementInfo == null)
            {
                return false;
            }

            return gatherSettlementInfo.SettleStatus == GatherSettleStatus.ORG;
        }

        protected override int GetCategorySysNo()
        {
            //审核代收结算单待办事项categorySysno
            return 706;
        }

        protected override string GetBizSysNo(GatherSettlementAuditCanceledMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetUrlParameter(GatherSettlementAuditCanceledMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetMemo(GatherSettlementAuditCanceledMessage msg)
        {
            return "取消审核-创建审核待办事项";
        }

        protected override int GetCurrentUserSysNo(GatherSettlementAuditCanceledMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 取消审核代收结算单-完成结算待办事项
    /// </summary>
    public class AuditCanceledGatherSettlement_CompleteSettleTask : WPMessageCompleter<GatherSettlementAuditCanceledMessage>
    {
        protected override int GetCategorySysNo()
        {
            //结算代收结算单待办事项categorySysno
            return 707;
        }

        protected override string GetBizSysNo(GatherSettlementAuditCanceledMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetMemo(GatherSettlementAuditCanceledMessage msg)
        {
            return "取消审核-完成结算待办事项";
        }

        protected override int GetCurrentUserSysNo(GatherSettlementAuditCanceledMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 结算代收结算单-完成结算待办事项
    /// </summary>
    public class SettledGatherSettlement_CompleteSettleTask : WPMessageCompleter<GatherSettlementSettledMessage>
    {
        protected override int GetCategorySysNo()
        {
            //结算代收结算单待办事项categorySysno
            return 707;
        }

        protected override string GetBizSysNo(GatherSettlementSettledMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetMemo(GatherSettlementSettledMessage msg)
        {
            return "结算-完成结算待办事项";
        }

        protected override int GetCurrentUserSysNo(GatherSettlementSettledMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 取消结算代收结算单-创建结算待办事项
    /// </summary>
    public class SettleCanceledGatherSettlement_CreateSettleTask : WPMessageCreator<GatherSettlementSettleCanceledMessage>
    {
        protected override bool NeedProcess(GatherSettlementSettleCanceledMessage msg)
        {
            if (msg.SettlementSysNo <= 0)
            {
                return false;
            }

            GatherSettlementInfo gatherSettlementInfo = ObjectFactory<IPOBizInteract>.Instance.GetGatherSettlementInfo(msg.SettlementSysNo);
            if (gatherSettlementInfo == null)
            {
                return false;
            }

            return gatherSettlementInfo.SettleStatus == GatherSettleStatus.AUD;
        }

        protected override int GetCategorySysNo()
        {
            //结算代收结算单待办事项categorySysno
            return 707;
        }

        protected override string GetBizSysNo(GatherSettlementSettleCanceledMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetUrlParameter(GatherSettlementSettleCanceledMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetMemo(GatherSettlementSettleCanceledMessage msg)
        {
            return "取消结算-创建结算待办事项";
        }

        protected override int GetCurrentUserSysNo(GatherSettlementSettleCanceledMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 作废代收结算单-完成审核待办事项
    /// </summary>
    public class AbandonedGatherSettlement_CompleteAuditTask : WPMessageCompleter<GatherSettlementAbandonedMessage>
    {
        protected override int GetCategorySysNo()
        {
            //审核代收结算单待办事项categorySysno
            return 706;
        }

        protected override string GetBizSysNo(GatherSettlementAbandonedMessage msg)
        {
            return msg.SettlementSysNo.ToString();
        }

        protected override string GetMemo(GatherSettlementAbandonedMessage msg)
        {
            return "作废-完成审核待办事项";
        }

        protected override int GetCurrentUserSysNo(GatherSettlementAbandonedMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 供应商退款单

    /// <summary>
    /// 创建供应商退款单审核待办事项
    /// </summary>
    public class CreateVendorRefundInfo_CreateTask : WPMessageCreator<VendorRefundInfoSubmitMessage>
    {
        protected override bool NeedProcess(VendorRefundInfoSubmitMessage msg)
        {
            if (msg.SysNo <= 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //供应商退款单审核待办事项CategorySysNo
            return 701;
        }

        protected override string GetBizSysNo(VendorRefundInfoSubmitMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(VendorRefundInfoSubmitMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(VendorRefundInfoSubmitMessage msg)
        {
            return "创建供应商退款单审核待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorRefundInfoSubmitMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
    }
    /// <summary>
    /// 审核通过供应商退款单审核-完成待办事项
    /// </summary>
    public class AuditedVendorRefundInfo_CompleteTask : WPMessageCompleter<VendorRefundInfoAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            //供应商退款单审核待办事项CategorySysNo
            return 701;
        }

        protected override string GetBizSysNo(VendorRefundInfoAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(VendorRefundInfoAuditMessage msg)
        {
            return "审核通过供应商退款单审核-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorRefundInfoAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
    }
    /// <summary>
    /// 审核拒绝供应商退款单审核-完成待办事项
    /// </summary>
    public class RejectedVendorRefundInfo_CompleteTask : WPMessageCompleter<VendorRefundInfoRejectMessage>
    {
        protected override int GetCategorySysNo()
        {
            //供应商退款单审核待办事项CategorySysNo
            return 701;
        }

        protected override string GetBizSysNo(VendorRefundInfoRejectMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(VendorRefundInfoRejectMessage msg)
        {
            return "审核拒绝供应商退款单审核-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorRefundInfoRejectMessage msg)
        {
            return msg.RejectUserSysNo;
        }
    }

    #endregion

    #region 供应商等级申请

    /// <summary>
    /// 供应商等级申请提交审核-创建待办事项
    /// </summary>
    public class SubmitVendorRankRequest_CreateTask : WPMessageCreator<VendorRankRequestSubmitMessage>
    {
        protected override bool NeedProcess(VendorRankRequestSubmitMessage msg)
        {
            if (msg.RequestSysNo <= 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //供应商等级申请审核待办事项CategorySysNo
            return 702;
        }

        protected override string GetBizSysNo(VendorRankRequestSubmitMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetUrlParameter(VendorRankRequestSubmitMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(VendorRankRequestSubmitMessage msg)
        {
            return "供应商等级申请提交审核-创建待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorRankRequestSubmitMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
    }
    /// <summary>
    /// 供应商等级申请审核通过-完成待办事项
    /// </summary>
    public class AuditedVendorRankRequest_CompleteTask : WPMessageCompleter<VendorRankRequestAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            //供应商等级申请审核待办事项CategorySysNo
            return 702;
        }

        protected override string GetBizSysNo(VendorRankRequestAuditMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(VendorRankRequestAuditMessage msg)
        {
            return "供应商等级申请审核通过-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorRankRequestAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
    }
    /// <summary>
    /// 供应商等级申请取消审核-完成待办事项
    /// </summary>
    public class CanceledVendorRefundInfo_CompleteTask : WPMessageCompleter<VendorRankRequestCancelMessage>
    {
        protected override int GetCategorySysNo()
        {
            //供应商等级申请审核待办事项CategorySysNo
            return 702;
        }

        protected override string GetBizSysNo(VendorRankRequestCancelMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(VendorRankRequestCancelMessage msg)
        {
            return "供应商等级申请取消审核-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorRankRequestCancelMessage msg)
        {
            return msg.CancelUserSysNo;
        }
    }

    #endregion

    #region 供应商财务信息申请

    /// <summary>
    /// 供应商财务信息申请提交审核-创建待办事项
    /// </summary>
    public class SubmitVendorFinanceInfoRequest_CreateTask : WPMessageCreator<VendorFinanceInfoRequestSubmitMessage>
    {
        protected override bool NeedProcess(VendorFinanceInfoRequestSubmitMessage msg)
        {
            if (msg.RequestSysNo <= 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //供应商财务信息申请待办事项CategorySysNo
            return 703;
        }

        protected override string GetBizSysNo(VendorFinanceInfoRequestSubmitMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetUrlParameter(VendorFinanceInfoRequestSubmitMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(VendorFinanceInfoRequestSubmitMessage msg)
        {
            return "供应商财务信息申请提交审核-创建待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorFinanceInfoRequestSubmitMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
    }
    /// <summary>
    /// 供应商财务信息申请审核通过-完成待办事项
    /// </summary>
    public class AuditedVendorFinanceInfoRequest_CompleteTask : WPMessageCompleter<VendorFinanceInfoRequestAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            //供应商财务信息申请待办事项CategorySysNo
            return 703;
        }

        protected override string GetBizSysNo(VendorFinanceInfoRequestAuditMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(VendorFinanceInfoRequestAuditMessage msg)
        {
            return "供应商财务信息申请审核通过-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorFinanceInfoRequestAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
    }
    /// <summary>
    /// 供应商财务信息申请审核拒绝-完成待办事项
    /// </summary>
    public class RejectedVendorFinanceInfoRequest_CompleteTask : WPMessageCompleter<VendorFinanceInfoRequestRejectMessage>
    {
        protected override int GetCategorySysNo()
        {
            //供应商财务信息申请待办事项CategorySysNo
            return 703;
        }

        protected override string GetBizSysNo(VendorFinanceInfoRequestRejectMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(VendorFinanceInfoRequestRejectMessage msg)
        {
            return "供应商财务信息申请审核拒绝-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorFinanceInfoRequestRejectMessage msg)
        {
            return msg.RejectUserSysNo;
        }
    }
    /// <summary>
    /// 供应商财务信息申请取消审核-完成待办事项
    /// </summary>
    public class CanceledVendorFinanceInfoRequest_CompleteTask : WPMessageCompleter<VendorFinanceInfoRequestCancelMessage>
    {
        protected override int GetCategorySysNo()
        {
            //供应商财务信息申请待办事项CategorySysNo
            return 703;
        }

        protected override string GetBizSysNo(VendorFinanceInfoRequestCancelMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(VendorFinanceInfoRequestCancelMessage msg)
        {
            return "供应商财务信息申请取消审核-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(VendorFinanceInfoRequestCancelMessage msg)
        {
            return msg.CancelUserSysNo;
        }
    }

    #endregion

    #region PO单审核

    /// <summary>
    /// PO单审核提交审核-创建待办事项
    /// </summary>
    public class SubmitPurchaseOrderAudit_CreateTask : WPMessageCreator<PurchaseOrderSubmitAuditMessage>
    {
        protected override bool NeedProcess(PurchaseOrderSubmitAuditMessage msg)
        {
            if (msg.SysNo <= 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //PO单审核待办事项CategorySysNo
            return 704;
        }

        protected override string GetBizSysNo(PurchaseOrderSubmitAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(PurchaseOrderSubmitAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(PurchaseOrderSubmitAuditMessage msg)
        {
            return "PO单审核提交审核-创建待办事项";
        }

        protected override int GetCurrentUserSysNo(PurchaseOrderSubmitAuditMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
    }
    /// <summary>
    /// PO单审核确认-完成待办事项
    /// </summary>
    public class PurchaseOrderAuditConfirm_CompleteTask : WPMessageCompleter<PurchaseOrderConfirmMessage>
    {
        protected override int GetCategorySysNo()
        {
            //PO单审核待办事项CategorySysNo
            return 704;
        }

        protected override string GetBizSysNo(PurchaseOrderConfirmMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(PurchaseOrderConfirmMessage msg)
        {
            return "PO单审核确认-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(PurchaseOrderConfirmMessage msg)
        {
            return msg.ConfirmUserSysNo;
        }
    }
    /// <summary>
    /// PO单审核拒绝-完成待办事项
    /// </summary>
    public class PurchaseOrderAuditReject_CompleteTask : WPMessageCompleter<PurchaseOrderRejectMessage>
    {
        protected override int GetCategorySysNo()
        {
            //PO单审核待办事项CategorySysNo
            return 704;
        }

        protected override string GetBizSysNo(PurchaseOrderRejectMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(PurchaseOrderRejectMessage msg)
        {
            return "PO单审核拒绝-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(PurchaseOrderRejectMessage msg)
        {
            return msg.RejectUserSysNo;
        }
    }
    
    #endregion

    #region 采购单预计到货时间

    /// <summary>
    /// 采购单预计到货时间提交审核-创建待办事项
    /// </summary>
    public class SubmitPurchaseOrderETATimeInfo_CreateTask : WPMessageCreator<PurchaseOrderETATimeInfoSubmitMessage>
    {
        protected override bool NeedProcess(PurchaseOrderETATimeInfoSubmitMessage msg)
        {
            if (msg.ETATimeSysNo <= 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //采购单预计到货时间提交审核CategorySysNo
            return 705;
        }

        protected override string GetBizSysNo(PurchaseOrderETATimeInfoSubmitMessage msg)
        {
            return msg.ETATimeSysNo.ToString();
        }

        protected override string GetUrlParameter(PurchaseOrderETATimeInfoSubmitMessage msg)
        {
            return msg.ETATimeSysNo.ToString();
        }

        protected override string GetMemo(PurchaseOrderETATimeInfoSubmitMessage msg)
        {
            return "采购单预计到货时间提交审核-创建待办事项";
        }

        protected override int GetCurrentUserSysNo(PurchaseOrderETATimeInfoSubmitMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
    }
    /// <summary>
    /// 采购单预计到货时间审核通过-完成待办事项
    /// </summary>
    public class PurchaseOrderETATimeInfoAudit_CompleteTask : WPMessageCompleter<PurchaseOrderETATimeInfoAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            //采购单预计到货时间审核待办事项CategorySysNo
            return 705;
        }

        protected override string GetBizSysNo(PurchaseOrderETATimeInfoAuditMessage msg)
        {
            return msg.ETATimeSysNo.ToString();
        }

        protected override string GetMemo(PurchaseOrderETATimeInfoAuditMessage msg)
        {
            return "采购单预计到货时间审核通过-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(PurchaseOrderETATimeInfoAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
    }
    /// <summary>
    /// 采购单预计到货时间取消审核-完成待办事项
    /// </summary>
    public class PurchaseOrderETATimeInfoCancel_CompleteTask : WPMessageCompleter<PurchaseOrderETATimeInfoCancelMessage>
    {
        protected override int GetCategorySysNo()
        {
            //采购单预计到货时间审核待办事项CategorySysNo
            return 705;
        }

        protected override string GetBizSysNo(PurchaseOrderETATimeInfoCancelMessage msg)
        {
            return msg.ETATimeSysNo.ToString();
        }

        protected override string GetMemo(PurchaseOrderETATimeInfoCancelMessage msg)
        {
            return "采购单预计到货时间取消审核-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(PurchaseOrderETATimeInfoCancelMessage msg)
        {
            return msg.CancelUserSysNo;
        }
    }
    
    #endregion

    #region 代销商品规则

    /// <summary>
    /// 创建代销商品规则-创建待办事项
    /// </summary>
    public class SettlementRuleCreate_CreateTask : WPMessageCreator<SettlementRuleCreateMessage>
    {
        protected override bool NeedProcess(SettlementRuleCreateMessage msg)
        {
            if (string.IsNullOrEmpty(msg.SettleRulesCode))
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //代销商品规则待办事项CategorySysNo
            return 708;
        }

        protected override string GetBizSysNo(SettlementRuleCreateMessage msg)
        {
            return msg.SettleRulesCode;
        }

        protected override string GetUrlParameter(SettlementRuleCreateMessage msg)
        {
            return msg.SettleRulesCode;
        }

        protected override string GetMemo(SettlementRuleCreateMessage msg)
        {
            return "创建代销商品规则-创建待办事项";
        }

        protected override int GetCurrentUserSysNo(SettlementRuleCreateMessage msg)
        {
            return msg.CreateUserSysNo;
        }
    }
    /// <summary>
    /// 审核代销商品规则-完成待办事项
    /// </summary>
    public class SettlementRuleAudit_CompleteTask : WPMessageCompleter<SettlementRuleAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            //代销商品规则待办事项CategorySysNo
            return 708;
        }

        protected override string GetBizSysNo(SettlementRuleAuditMessage msg)
        {
            return msg.SettleRulesCode;
        }

        protected override string GetMemo(SettlementRuleAuditMessage msg)
        {
            return "审核代销商品规则-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(SettlementRuleAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
    }
    /// <summary>
    /// 作废代销商品规则-完成待办事项
    /// </summary>
    public class SettlementRuleAbandon_CompleteTask : WPMessageCompleter<SettlementRuleAbandonMessage>
    {
        protected override int GetCategorySysNo()
        {
            //代销商品规则待办事项CategorySysNo
            return 708;
        }

        protected override string GetBizSysNo(SettlementRuleAbandonMessage msg)
        {
            return msg.SettleRulesCode;
        }

        protected override string GetMemo(SettlementRuleAbandonMessage msg)
        {
            return "作废代销商品规则-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(SettlementRuleAbandonMessage msg)
        {
            return msg.AbandonUserSysNo;
        }
    }

    #endregion

    #region 代销结算单

    /// <summary>
    /// 创建代销结算单-创建待办事项
    /// </summary>
    public class ConsignSettlementCreate_CreateTask : WPMessageCreator<ConsignSettlementCreateMessage>
    {
        protected override bool NeedProcess(ConsignSettlementCreateMessage msg)
        {
            if (msg.SysNo < 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //代销结算单待办事项CategorySysNo
            return 709;
        }

        protected override string GetBizSysNo(ConsignSettlementCreateMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ConsignSettlementCreateMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsignSettlementCreateMessage msg)
        {
            return "创建代销结算单-创建待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsignSettlementCreateMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 取消审核代销结算单-完成待办事项
    /// </summary>
    public class ConsignSettlementCancel_CompleteTask : WPMessageCompleter<ConsignSettlementCancelMessage>
    {
        protected override int GetCategorySysNo()
        {
            //代销结算单待办事项CategorySysNo
            return 709;
        }

        protected override string GetBizSysNo(ConsignSettlementCancelMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsignSettlementCancelMessage msg)
        {
            return "取消审核代销结算单-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsignSettlementCancelMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 作废代销结算单-完成待办事项
    /// </summary>
    public class ConsignSettlementAbandon_CompleteTask : WPMessageCompleter<ConsignSettlementAbandonMessage>
    {
        protected override int GetCategorySysNo()
        {
            //代销结算单待办事项CategorySysNo
            return 709;
        }

        protected override string GetBizSysNo(ConsignSettlementAbandonMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsignSettlementAbandonMessage msg)
        {
            return "作废代销结算单-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsignSettlementAbandonMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 审核代销结算单-完成待办事项
    /// </summary>
    public class ConsignSettlementAudit_CompleteTask : WPMessageCompleter<ConsignSettlementAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            //代销结算单待办事项CategorySysNo
            return 709;
        }

        protected override string GetBizSysNo(ConsignSettlementAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsignSettlementAuditMessage msg)
        {
            return "审核代销结算单-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsignSettlementAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 审核代销结算单-创建结算待办事项
    /// </summary>
    public class ConsignSettlementAuditCreate_CreateTask : WPMessageCreator<ConsignSettlementAuditMessage>
    {
        protected override bool NeedProcess(ConsignSettlementAuditMessage msg)
        {
            if (msg.SysNo < 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //结算代销结算单待办事项CategorySysNo
            return 710;
        }

        protected override string GetBizSysNo(ConsignSettlementAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(ConsignSettlementAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsignSettlementAuditMessage msg)
        {
            return "审核代销结算单-创建结算待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsignSettlementAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 结算代销结算单-完成结算待办事项
    /// </summary>
    public class ConsignSettlementSettlement_CompleteTask : WPMessageCompleter<ConsignSettlementSettlementMessage>
    {
        protected override int GetCategorySysNo()
        {
            //结算代销结算单待办事项CategorySysNo
            return 710;
        }

        protected override string GetBizSysNo(ConsignSettlementSettlementMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsignSettlementSettlementMessage msg)
        {
            return "结算代销结算单-完成结算待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsignSettlementSettlementMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 取消结算代销结算单-完成结算待办事项
    /// </summary>
    public class ConsignSettlementCancelSettlement_CompleteTask : WPMessageCompleter<ConsignSettlementCancelSettlementMessage>
    {
        protected override int GetCategorySysNo()
        {
            //结算代销结算单待办事项CategorySysNo
            return 710;
        }

        protected override string GetBizSysNo(ConsignSettlementCancelSettlementMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(ConsignSettlementCancelSettlementMessage msg)
        {
            return "取消结算代销结算单-完成结算待办事项";
        }

        protected override int GetCurrentUserSysNo(ConsignSettlementCancelSettlementMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 代收代付结算单

    /// <summary>
    /// 创建代收代付结算单-创建待办事项
    /// </summary>
    public class CollectionPaymentCreate_CreateTask : WPMessageCreator<CollectionPaymentCreateMessage>
    {
        protected override bool NeedProcess(CollectionPaymentCreateMessage msg)
        {
            if (msg.SysNo < 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //代收代付结算单待办事项CategorySysNo
            return 711;
        }

        protected override string GetBizSysNo(CollectionPaymentCreateMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(CollectionPaymentCreateMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(CollectionPaymentCreateMessage msg)
        {
            return "创建代收代付结算单-创建待办事项";
        }

        protected override int GetCurrentUserSysNo(CollectionPaymentCreateMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 取消审核代收代付结算单-完成待办事项
    /// </summary>
    public class CollectionPaymentCancel_CompleteTask : WPMessageCompleter<CollectionPaymentCancelMessage>
    {
        protected override int GetCategorySysNo()
        {
            //代收代付结算单待办事项CategorySysNo
            return 711;
        }

        protected override string GetBizSysNo(CollectionPaymentCancelMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(CollectionPaymentCancelMessage msg)
        {
            return "取消审核代收代付结算单-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(CollectionPaymentCancelMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 作废代收代付结算单-完成待办事项
    /// </summary>
    public class CollectionPaymentAbandon_CompleteTask : WPMessageCompleter<CollectionPaymentAbandonMessage>
    {
        protected override int GetCategorySysNo()
        {
            //代收代付结算单待办事项CategorySysNo
            return 711;
        }

        protected override string GetBizSysNo(CollectionPaymentAbandonMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(CollectionPaymentAbandonMessage msg)
        {
            return "作废代收代付结算单-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(CollectionPaymentAbandonMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 审核代收代付结算单-完成待办事项
    /// </summary>
    public class CollectionPaymentAudit_CompleteTask : WPMessageCompleter<CollectionPaymentAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            //代收代付结算单待办事项CategorySysNo
            return 711;
        }

        protected override string GetBizSysNo(CollectionPaymentAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(CollectionPaymentAuditMessage msg)
        {
            return "审核代收代付结算单-完成待办事项";
        }

        protected override int GetCurrentUserSysNo(CollectionPaymentAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 审核代收代付结算单-创建结算待办事项
    /// </summary>
    public class CollectionPaymentAuditCreate_CreateTask : WPMessageCreator<CollectionPaymentAuditMessage>
    {
        protected override bool NeedProcess(CollectionPaymentAuditMessage msg)
        {
            if (msg.SysNo < 0)
            {
                return false;
            }
            return true;
        }

        protected override int GetCategorySysNo()
        {
            //结算代收代付结算单待办事项CategorySysNo
            return 712;
        }

        protected override string GetBizSysNo(CollectionPaymentAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(CollectionPaymentAuditMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(CollectionPaymentAuditMessage msg)
        {
            return "审核代收代付结算单-创建结算待办事项";
        }

        protected override int GetCurrentUserSysNo(CollectionPaymentAuditMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 结算代收代付结算单-完成结算待办事项
    /// </summary>
    public class CollectionPaymentSettlement_CompleteTask : WPMessageCompleter<CollectionPaymentSettlementMessage>
    {
        protected override int GetCategorySysNo()
        {
            //结算代收代付结算单待办事项CategorySysNo
            return 712;
        }

        protected override string GetBizSysNo(CollectionPaymentSettlementMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(CollectionPaymentSettlementMessage msg)
        {
            return "结算代收代付结算单-完成结算待办事项";
        }

        protected override int GetCurrentUserSysNo(CollectionPaymentSettlementMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }
    /// <summary>
    /// 取消结算代收代付结算单-完成结算待办事项
    /// </summary>
    public class CollectionPaymentCancelSettlement_CompleteTask : WPMessageCompleter<CollectionPaymentCancelSettlementMessage>
    {
        protected override int GetCategorySysNo()
        {
            //结算代收代付结算单待办事项CategorySysNo
            return 712;
        }

        protected override string GetBizSysNo(CollectionPaymentCancelSettlementMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(CollectionPaymentCancelSettlementMessage msg)
        {
            return "取消结算代收代付结算单-完成结算待办事项";
        }

        protected override int GetCurrentUserSysNo(CollectionPaymentCancelSettlementMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion
}
