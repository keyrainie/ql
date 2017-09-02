using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.Inventory;

namespace ECCentral.Service.WPMessage.BizProcessor
{
    #region 损益单

    /// <summary>
    /// 创建损益发送待办项
    /// </summary>
    public class CreateAdjustRequestInfoMessage_Creator : WPMessageCreator<CreateAdjustRequestInfoMessage>
    {
        protected override bool NeedProcess(CreateAdjustRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 700;
        }

        protected override string GetBizSysNo(CreateAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CreateAdjustRequestInfoMessage msg)
        {
            return "创建损益发送待办项Message";
        }

        protected override int GetCurrentUserSysNo(CreateAdjustRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    /// <summary>
    /// 审核损益单
    /// </summary>
    public class AuditAdjustRequestInfoMessage_Completor : WPMessageCompleter<AuditAdjustRequestInfoMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 700;
        }

        protected override string GetBizSysNo(AuditAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(AuditAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(AuditAdjustRequestInfoMessage msg)
        {
            return "审核损益单";
        }

        protected override int GetCurrentUserSysNo(AuditAdjustRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }


    /// <summary>
    /// 取消审核损益单
    /// </summary>
    public class CancelAuditAdjustRequestInfoMessage_Completor : WPMessageCompleter<CancelAuditAdjustRequestInfoMessage>
    {

        protected override int GetCategorySysNo()
        {
            return 700;
        }

        protected override string GetBizSysNo(CancelAuditAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelAuditAdjustRequestInfoMessage msg)
        {
            return "取消审核损益单";
        }

        protected override int GetCurrentUserSysNo(CancelAuditAdjustRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }


    /// <summary>
    /// 损益单出库
    /// </summary>
    public class OutStockAdjustRequestInfoMessage_Compeletor : WPMessageCompleter<OutStockAdjustRequestInfoMessage>
    {

        protected override int GetCategorySysNo()
        {
            return 700;
        }

        protected override string GetBizSysNo(OutStockAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(OutStockAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(OutStockAdjustRequestInfoMessage msg)
        {
            return "取消审核损益单";
        }

        protected override int GetCurrentUserSysNo(OutStockAdjustRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }


    /// <summary>
    /// 作废损益单
    /// </summary>
    public class VoidAdjustRequestInfoMessage_Compeletor : WPMessageCompleter<VoidAdjustRequestInfoMessage>
    {

        protected override int GetCategorySysNo()
        {
            return 700;
        }

        protected override string GetBizSysNo(VoidAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(VoidAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(VoidAdjustRequestInfoMessage msg)
        {
            return "作废损益单";
        }

        protected override int GetCurrentUserSysNo(VoidAdjustRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }


    /// <summary>
    /// 取消作废损益单
    /// </summary>
    public class CancelVoidAdjustRequestInfoMessage_Compeletor : WPMessageCreator<CancelVoidAdjustRequestInfoMessage>
    {

        protected override int GetCategorySysNo()
        {
            return 700;
        }

        protected override string GetBizSysNo(CancelVoidAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CancelVoidAdjustRequestInfoMessage msg)
        {
            return msg.AdjustRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelVoidAdjustRequestInfoMessage msg)
        {
            return "作废损益单";
        }

        protected override int GetCurrentUserSysNo(CancelVoidAdjustRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    #endregion

    #region 移仓单

    /// <summary>
    /// 创建移仓单
    /// </summary>
    public class CreateShiftRequestInfoMessage_Creator : WPMessageCreator<CreateShiftRequestInfoMessage>
    {
        protected override bool NeedProcess(CreateShiftRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 701;
        }

        protected override string GetBizSysNo(CreateShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CreateShiftRequestInfoMessage msg)
        {
            return "创建移仓单发送Message";
        }

        protected override int GetCurrentUserSysNo(CreateShiftRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    /// <summary>
    /// 审核移仓单
    /// </summary>
    public class AuditShiftRequestInfoMessage_Completor : WPMessageCompleter<AuditShiftRequestInfoMessage>
    {
        protected override bool NeedProcess(AuditShiftRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 701;
        }

        protected override string GetBizSysNo(AuditShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(AuditShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(AuditShiftRequestInfoMessage msg)
        {
            return "审核移仓单";
        }

        protected override int GetCurrentUserSysNo(AuditShiftRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    /// <summary>
    /// 取消审核移仓单
    /// </summary>
    public class CancelAuditShiftRequestInfoMessage_Completor : WPMessageCompleter<CancelAuditShiftRequestInfoMessage>
    {
        protected override bool NeedProcess(CancelAuditShiftRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 701;
        }

        protected override string GetBizSysNo(CancelAuditShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CancelAuditShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelAuditShiftRequestInfoMessage msg)
        {
            return "取消审核移仓单";
        }

        protected override int GetCurrentUserSysNo(CancelAuditShiftRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }



    /// <summary>
    ///作废移仓单
    /// </summary>
    public class VoidShiftRequestInfoMessage_Complete : WPMessageCompleter<VoidShiftRequestInfoMessage>
    {
        protected override bool NeedProcess(VoidShiftRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 701;
        }

        protected override string GetBizSysNo(VoidShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(VoidShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(VoidShiftRequestInfoMessage msg)
        {
            return "作废移仓单";
        }

        protected override int GetCurrentUserSysNo(VoidShiftRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    ///取消作废移仓单
    /// </summary>
    public class CancelVoidShiftRequestInfoMessage_Completor : WPMessageCompleter<CancelVoidShiftRequestInfoMessage>
    {
        protected override bool NeedProcess(CancelVoidShiftRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 701;
        }

        protected override string GetBizSysNo(CancelVoidShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CancelVoidShiftRequestInfoMessage msg)
        {
            return msg.ShiftRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelVoidShiftRequestInfoMessage msg)
        {
            return "取消作废移仓单";
        }

        protected override int GetCurrentUserSysNo(CancelVoidShiftRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 借货单

    /// <summary>
    /// 创建借货单
    /// </summary>
    public class CreateLendRequestInfoMessage_Creator : WPMessageCreator<CreateLendRequestInfoMessage>
    {
        protected override bool NeedProcess(CreateLendRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 702;
        }

        protected override string GetBizSysNo(CreateLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CreateLendRequestInfoMessage msg)
        {
            return "创建借货单发送Message";
        }

        protected override int GetCurrentUserSysNo(CreateLendRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }


    /// <summary>
    /// 审核借货单
    /// </summary>
    public class AuditLendRequestInfoMessage_Completor : WPMessageCompleter<AuditLendRequestInfoMessage>
    {
        protected override bool NeedProcess(AuditLendRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 702;
        }

        protected override string GetBizSysNo(AuditLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(AuditLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(AuditLendRequestInfoMessage msg)
        {
            return "审核借货单";
        }

        protected override int GetCurrentUserSysNo(AuditLendRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    /// <summary>
    /// 取消审核借货单
    /// </summary>
    public class CancelAuditLendRequestInfoMessage_Completor : WPMessageCompleter<CancelAuditLendRequestInfoMessage>
    {
        protected override bool NeedProcess(CancelAuditLendRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 702;
        }

        protected override string GetBizSysNo(CancelAuditLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CancelAuditLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelAuditLendRequestInfoMessage msg)
        {
            return "取消审核借货单";
        }

        protected override int GetCurrentUserSysNo(CancelAuditLendRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }


    /// <summary>
    /// 作废借货单
    /// </summary>
    public class VoidLendRequestInfoMessage_Completor : WPMessageCompleter<VoidLendRequestInfoMessage>
    {
        protected override bool NeedProcess(VoidLendRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 702;
        }

        protected override string GetBizSysNo(VoidLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(VoidLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(VoidLendRequestInfoMessage msg)
        {
            return "作废借货单";
        }

        protected override int GetCurrentUserSysNo(VoidLendRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    /// <summary>
    /// 取消作废借货单
    /// </summary>
    public class CancelVoidLendRequestInfoMessage_Completor : WPMessageCompleter<CancelVoidLendRequestInfoMessage>
    {
        protected override bool NeedProcess(CancelVoidLendRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 702;
        }

        protected override string GetBizSysNo(CancelVoidLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CancelVoidLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelVoidLendRequestInfoMessage msg)
        {
            return "取消作废借货单";
        }

        protected override int GetCurrentUserSysNo(CancelVoidLendRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    /// <summary>
    /// 借货单出库
    /// </summary>
    public class OutStockLendRequestInfoMessage_Completor : WPMessageCompleter<OutStockLendRequestInfoMessage>
    {
        protected override bool NeedProcess(OutStockLendRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 702;
        }

        protected override string GetBizSysNo(OutStockLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(OutStockLendRequestInfoMessage msg)
        {
            return msg.LendRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(OutStockLendRequestInfoMessage msg)
        {
            return "借货单出库";
        }

        protected override int GetCurrentUserSysNo(OutStockLendRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion

    #region 转换单

    /// <summary>
    /// 创建转换单
    /// </summary>
    public class CreateConvertRequestInfoMessage_Creator : WPMessageCreator<CreateConvertRequestInfoMessage>
    {
        protected override bool NeedProcess(CreateConvertRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 703;
        }

        protected override string GetBizSysNo(CreateConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CreateConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CreateConvertRequestInfoMessage msg)
        {
            return "创建转换单发送Message";
        }

        protected override int GetCurrentUserSysNo(CreateConvertRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

    }

    /// <summary>
    /// 转换单审核
    /// </summary>
    public class AuditConvertRequestInfoMessage_Completor : WPMessageCompleter<AuditConvertRequestInfoMessage>
    {
        protected override bool NeedProcess(AuditConvertRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 703;
        }

        protected override string GetBizSysNo(AuditConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(AuditConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(AuditConvertRequestInfoMessage msg)
        {
            return "转换单审核";
        }

        protected override int GetCurrentUserSysNo(AuditConvertRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 转换单取消审核
    /// </summary>
    public class CancelAuditConvertRequestInfoMessage_Completor : WPMessageCompleter<CancelAuditConvertRequestInfoMessage>
    {
        protected override bool NeedProcess(CancelAuditConvertRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 703;
        }

        protected override string GetBizSysNo(CancelAuditConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CancelAuditConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelAuditConvertRequestInfoMessage msg)
        {
            return "转换单审核";
        }

        protected override int GetCurrentUserSysNo(CancelAuditConvertRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }


    /// <summary>
    /// 转换单-作废
    /// </summary>
    public class VoidConvertRequestInfoMessage_Completor : WPMessageCompleter<VoidConvertRequestInfoMessage>
    {
        protected override bool NeedProcess(VoidConvertRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 703;
        }

        protected override string GetBizSysNo(VoidConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(VoidConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(VoidConvertRequestInfoMessage msg)
        {
            return "转换单-作废";
        }

        protected override int GetCurrentUserSysNo(VoidConvertRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }


    /// <summary>
    /// 转换单-取消作废
    /// </summary>
    public class CancelVoidConvertRequestInfoMessage_Completor : WPMessageCompleter<CancelVoidConvertRequestInfoMessage>
    {
        protected override bool NeedProcess(CancelVoidConvertRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 703;
        }

        protected override string GetBizSysNo(CancelVoidConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(CancelVoidConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(CancelVoidConvertRequestInfoMessage msg)
        {
            return "转换单-取消作废";
        }

        protected override int GetCurrentUserSysNo(CancelVoidConvertRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    /// <summary>
    /// 转换单-出库
    /// </summary>
    public class OutStockConvertRequestInfoMessage_Completor : WPMessageCompleter<OutStockConvertRequestInfoMessage>
    {
        protected override bool NeedProcess(OutStockConvertRequestInfoMessage msg)
        {
            throw new NotImplementedException();
        }

        protected override int GetCategorySysNo()
        {
            return 703;
        }

        protected override string GetBizSysNo(OutStockConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetUrlParameter(OutStockConvertRequestInfoMessage msg)
        {
            return msg.ConvertRequestInfoSysNo.ToString();
        }

        protected override string GetMemo(OutStockConvertRequestInfoMessage msg)
        {
            return "转换单-出库";
        }

        protected override int GetCurrentUserSysNo(OutStockConvertRequestInfoMessage msg)
        {
            return msg.CurrentUserSysNo;
        }
    }

    #endregion
}
