using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.WPMessage.BizProcessor
{        
    #region 订单拆分
    /// <summary>
    /// 订单拆分，创建订单审核待办事项
    /// </summary>
    //public class SOSplit_CreateAuditTask : WPMessageCreator<SOSplitMessage>
    //{
    //    /// <summary>
    //    /// 创建订单审核待办事项
    //    /// </summary>
    //    /// <returns></returns>
    //    protected override int GetCategorySysNo()
    //    {
    //        return 201;//创建订单审核待办事项
    //    }

    //    protected override string GetBizSysNo(SOSplitMessage msg)
    //    {
    //        return msg.SOSysNo.ToString();
    //    }

    //    protected override string GetUrlParameter(SOSplitMessage msg)
    //    {
    //        return (msg.SplitType == SOSplitType.SubSO ? msg.MasterSOSysNo.Value : msg.SOSysNo).ToString();
    //    }

    //    protected override string GetMemo(SOSplitMessage msg)
    //    {
    //        return string.Format("系统处理订单完成，请审核订单[{0}]。", msg.SplitType == SOSplitType.SubSO ? msg.MasterSOSysNo.Value : msg.SOSysNo);
    //    }

    //    protected override int GetCurrentUserSysNo(SOSplitMessage msg)
    //    {
    //        return msg.SplitUserSysNo;
    //    }
    //    protected override bool NeedProcess(SOSplitMessage msg)
    //    {
    //        throw new NotImplementedException();
    //        //ECCentral.BizEntity.SO.SOInfo soInfo = ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SOSysNo);
    //        //if (soInfo == null)
    //        //{
    //        //    return false;
    //        //}
    //        //// 只有在ECCentral 或SellerPortal 处理的订单才需要完成待办事项，到票务系统和酒店系统的没有待办事项。
    //        //return soInfo.BaseInfo.Status == ECCentral.BizEntity.SO.SOStatus.Origin && soInfo.BaseInfo.ProcessSystem == BizEntity.Common.OrderProcessSystem.ECCentral || soInfo.BaseInfo.ProcessSystem == BizEntity.Common.OrderProcessSystem.SellerPortal;
    //    }
    //}
    #endregion

    #region 订单审核消息

    /// <summary>
    /// 订单审核,生成出库的待办事项
    /// </summary>
    public class SOAudited_CreateOutStockTask : WPMessageCreator<SOAuditedMessage>
    {
        private ECCentral.BizEntity.SO.SOInfo soInfo;
        public SOAudited_CreateOutStockTask(ECCentral.BizEntity.SO.SOInfo soInfo)
        {
            this.soInfo = soInfo;
        }
        public SOAudited_CreateOutStockTask()
            : this(null)
        { }
        protected override int GetCategorySysNo()
        {
            return 202;//添加订单出库待办事项类型
        }

        protected override string GetBizSysNo(SOAuditedMessage msg)
        {
            return msg.SOSysNo.ToString();
        }

        protected override string GetUrlParameter(SOAuditedMessage msg)
        {
            return msg.MasterSOSysNo.HasValue && msg.MasterSOSysNo.Value > 0 ? msg.MasterSOSysNo.Value.ToString() : msg.SOSysNo.ToString();
        }

        protected override string GetMemo(SOAuditedMessage msg)
        {
            return string.Format("订单[{0}]已经审核，请出库。", msg.MasterSOSysNo.HasValue && msg.MasterSOSysNo.Value > 0 ? msg.MasterSOSysNo.Value.ToString() : msg.SOSysNo.ToString());
        }

        protected override int GetCurrentUserSysNo(SOAuditedMessage msg)
        {
            return msg.AuditedUserSysNo;
        }

        protected override bool NeedProcess(SOAuditedMessage msg)
        {
            soInfo = soInfo ?? ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SOSysNo);
            if (soInfo == null)
            {
                return false;
            }
            return soInfo.BaseInfo.SOType.Value == (BizEntity.SO.SOType)SOType.PPackageCard && soInfo.BaseInfo.Status == ECCentral.BizEntity.SO.SOStatus.WaitingOutStock;
        }
    }
    /// <summary>
    /// 订单审核,完成订单审核待办事项
    /// </summary>
    public class SOAudited_CompleteAuditTask : WPMessageCompleter<SOAuditedMessage>
    {
        private ECCentral.BizEntity.SO.SOInfo soInfo;
        public SOAudited_CompleteAuditTask(ECCentral.BizEntity.SO.SOInfo soInfo)
        {
            this.soInfo = soInfo;
        }

        public SOAudited_CompleteAuditTask()
            : this(null)
        {
        }

        protected override int GetCategorySysNo()
        {
            return 201;//完成订单审核待办事项类型
        }

        protected override string GetBizSysNo(SOAuditedMessage msg)
        {
            return msg.SOSysNo.ToString();
        }

        protected override int GetCurrentUserSysNo(SOAuditedMessage msg)
        {
            return msg.AuditedUserSysNo;
        }
        protected override string GetMemo(SOAuditedMessage msg)
        {
            return "订单审核通过，等待出库。";
        }
        protected override bool NeedProcess(SOAuditedMessage msg)
        {
            throw new NotImplementedException();
            //soInfo = soInfo ?? ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SOSysNo);
            //if (soInfo == null)
            //{
            //    return false;
            //}
            //// 只有在ECCentral 或SellerPortal 处理的订单才需要完成待办事项，到票务系统和酒店系统的没有待办事项。
            //return soInfo.BaseInfo.ProcessSystem == BizEntity.Common.OrderProcessSystem.ECCentral || soInfo.BaseInfo.ProcessSystem == BizEntity.Common.OrderProcessSystem.SellerPortal;
        }
    }

    public class SOAuditedMessageProcessor : IESBMessageProcessor
    {
        public void Process(ESBMessage message)
        {
            if (message == null)
            {
                return;
            }
            SOAuditedMessage data = message.GetData<SOAuditedMessage>();
            if (data == null)
            {
                return;
            }
            ECCentral.BizEntity.SO.SOInfo soInfo = ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(data.SOSysNo);
            if (soInfo == null)
            {
                return;
            }

            new SOAudited_CreateOutStockTask(soInfo).Process(message);
            new SOAudited_CompleteAuditTask(soInfo).Process(message);
        }
    }


    #endregion

    #region 订单出库
    public class SOOutStock_CompleteOutStockTask : WPMessageCompleter<SOOutStockMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 202;//完成订单出库待办事项
        }

        protected override string GetBizSysNo(SOOutStockMessage msg)
        {
            return msg.SOSysNo.ToString();
        }

        protected override int GetCurrentUserSysNo(SOOutStockMessage msg)
        {
            return msg.OutStockUserSysNo;
        }
        protected override string GetMemo(SOOutStockMessage msg)
        {
            return "订单已出库。";
        }
        protected override bool NeedProcess(SOOutStockMessage msg)
        {
            ECCentral.BizEntity.SO.SOInfo soInfo = ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SOSysNo);
            if (soInfo == null)
            {
                return false;
            }

            return soInfo.BaseInfo.SOType.Value == (BizEntity.SO.SOType)SOType.PPackageCard;
        }
    }
    #endregion

    #region 订单作废

    public class SOAbandoned_CompleteAuditTask : WPMessageCompleter<SOAbandonedMessage>
    {
        private ECCentral.BizEntity.SO.SOInfo soInfo;
        public SOAbandoned_CompleteAuditTask(ECCentral.BizEntity.SO.SOInfo soInfo)
        {
            this.soInfo = soInfo;
        }
        public SOAbandoned_CompleteAuditTask()
            : this(null)
        { }
        protected override int GetCategorySysNo()
        {
            return 201;//完成订单审核待办事项类型
        }

        protected override string GetBizSysNo(SOAbandonedMessage msg)
        {
            return msg.SOSysNo.ToString();
        }

        protected override int GetCurrentUserSysNo(SOAbandonedMessage msg)
        {
            return msg.AbandonedUserSysNo;
        }

        protected override string GetMemo(SOAbandonedMessage msg)
        {
            return "订单已作废，不用再审核。";
        }

        protected override bool NeedProcess(SOAbandonedMessage msg)
        {
            throw new NotImplementedException();
            //soInfo = soInfo ?? ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SOSysNo);
            //if (soInfo == null)
            //{
            //    return false;
            //}
            //// 只有在ECCentral 或SellerPortal 处理的订单才需要完成待办事项，到票务系统和酒店系统的没有待办事项。
            //return soInfo.BaseInfo.ProcessSystem == BizEntity.Common.OrderProcessSystem.ECCentral || soInfo.BaseInfo.ProcessSystem == BizEntity.Common.OrderProcessSystem.SellerPortal;
        }
    }

    public class SOAbandoned_CompleteOutStockTask : WPMessageCompleter<SOAbandonedMessage>
    {
        private ECCentral.BizEntity.SO.SOInfo soInfo;
        public SOAbandoned_CompleteOutStockTask(ECCentral.BizEntity.SO.SOInfo soInfo)
        {
            this.soInfo = soInfo;
        }
        public SOAbandoned_CompleteOutStockTask()
            : this(null)
        { }

        protected override int GetCategorySysNo()
        {
            return 201;//完成订单审核待办事项类型
        }

        protected override string GetBizSysNo(SOAbandonedMessage msg)
        {
            return msg.SOSysNo.ToString();
        }

        protected override int GetCurrentUserSysNo(SOAbandonedMessage msg)
        {
            return msg.AbandonedUserSysNo;
        }

        protected override string GetMemo(SOAbandonedMessage msg)
        {
            return "订单已作废，不用出库。";
        }

        protected override bool NeedProcess(SOAbandonedMessage msg)
        {
            soInfo = soInfo ?? ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SOSysNo);
            if (soInfo == null)
            {
                return false;
            }
            return soInfo.BaseInfo.SOType.Value == (BizEntity.SO.SOType)SOType.PPackageCard;
        }
    }

    public class SOAbandoned_CompleteConfirmTask : WPMessageCompleter<SOAbandonedMessage>
    {
        private ECCentral.BizEntity.SO.SOInfo soInfo;
        public SOAbandoned_CompleteConfirmTask(ECCentral.BizEntity.SO.SOInfo soInfo)
        {
            this.soInfo = soInfo;
        }
        public SOAbandoned_CompleteConfirmTask()
            : this(null)
        { }

        protected override int GetCategorySysNo()
        {
            return 200;
        }

        protected override string GetBizSysNo(SOAbandonedMessage msg)
        {
            return msg.SOSysNo.ToString();
        }

        protected override int GetCurrentUserSysNo(SOAbandonedMessage msg)
        {
            return msg.AbandonedUserSysNo;
        }

        protected override string GetMemo(SOAbandonedMessage msg)
        {
            return "订单作废，不用再确认库存。";
        }

        protected override bool NeedProcess(SOAbandonedMessage msg)
        {
            throw new NotImplementedException();
            //soInfo = soInfo ?? ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(msg.SOSysNo);
            //if (soInfo == null)
            //{
            //    return false;
            //}
            ////订单状态为待确认状态
            //return soInfo.Items != null && soInfo.Items.Exists(item => item.IsConfirmStock);
        }
    }

    public class SOAbandonedMessageProcessor : IESBMessageProcessor
    {
        public void Process(ESBMessage message)
        {
            if (message == null)
            {
                return;
            }
            SOAbandonedMessage data = message.GetData<SOAbandonedMessage>();
            if (data == null)
            {
                return;
            }
            ECCentral.BizEntity.SO.SOInfo soInfo = ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(data.SOSysNo);
            if (soInfo == null)
            {
                return;
            }

            new SOAbandoned_CompleteAuditTask(soInfo).Process(message);
            new SOAbandoned_CompleteOutStockTask(soInfo).Process(message);
            new SOAbandoned_CompleteConfirmTask(soInfo).Process(message);
        }
    }

    #endregion

    #region 客户作废订单申请

    //public class SOCreatedCustomerAbandonRequest_CreateAbandonRequestTask : WPMessageCreator<SOCreatedCustomerAbandonRequestMessage>
    //{
    //    ECCentral.BizEntity.SO.SOInfo _soInfo;
    //    ECCentral.BizEntity.SO.SOInfo GetSOInfo(int soSysNo)
    //    {
    //        _soInfo = _soInfo ?? ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(soSysNo);
    //        return _soInfo;

    //    }
    //    protected override int GetCategorySysNo()
    //    {
    //        return 203;
    //    }

    //    protected override string GetBizSysNo(SOCreatedCustomerAbandonRequestMessage msg)
    //    {
    //        return msg.RequestSysNo.ToString();
    //    }

    //    protected override string GetUrlParameter(SOCreatedCustomerAbandonRequestMessage msg)
    //    {
    //        return "?SOSysNo=" + msg.SOSysNo.ToString();
    //    }

    //    protected override string GetMemo(SOCreatedCustomerAbandonRequestMessage msg)
    //    {
    //        return String.Format("客户创建作废订单[{0}]申请。", msg.SOSysNo);
    //    }

    //    protected override int GetCurrentUserSysNo(SOCreatedCustomerAbandonRequestMessage msg)
    //    {
    //        return msg.CreateUserSysNo;
    //    }
    //    protected override bool NeedProcess(SOCreatedCustomerAbandonRequestMessage msg)
    //    {
    //        throw new NotImplementedException();
    //        //ECCentral.BizEntity.SO.SOInfo soInfo = GetSOInfo(msg.SOSysNo);
    //        //if (soInfo == null)
    //        //{
    //        //    return false;
    //        //}
    //        //return soInfo.BaseInfo.Status == ECCentral.BizEntity.SO.SOStatus.Origin
    //        //    || soInfo.BaseInfo.Status == ECCentral.BizEntity.SO.SOStatus.WaitingConfirm
    //        //    || soInfo.BaseInfo.Status == ECCentral.BizEntity.SO.SOStatus.WaitingOutStock;
    //    }
    //}

    //public class SOCanceledCustomerAbandonRequest_CompleteAbandonRequestTask : WPMessageCompleter<SOCanceledCustomerAbandonRequestMessage>
    //{
    //    protected override int GetCategorySysNo()
    //    {
    //        return 203;
    //    }

    //    protected override string GetBizSysNo(SOCanceledCustomerAbandonRequestMessage msg)
    //    {
    //        return msg.RequestSysNo.ToString();
    //    }

    //    protected override string GetMemo(SOCanceledCustomerAbandonRequestMessage msg)
    //    {
    //        return "客户取消作废订单申请。";
    //    }
    //    protected override int GetCurrentUserSysNo(SOCanceledCustomerAbandonRequestMessage msg)
    //    {
    //        return msg.CancelUserSysNo;
    //    }
    //}

    //public class SOCompletedCustomerAbandonRequest_CompleteAbandonRequestTask : WPMessageCompleter<SOCompletedCustomerAbandonRequestMessage>
    //{
    //    protected override int GetCategorySysNo()
    //    {
    //        return 203;
    //    }

    //    protected override string GetBizSysNo(SOCompletedCustomerAbandonRequestMessage msg)
    //    {
    //        return msg.RequestSysNo.ToString();
    //    }

    //    protected override int GetCurrentUserSysNo(SOCompletedCustomerAbandonRequestMessage msg)
    //    {
    //        return msg.CompleteUserSysNo;
    //    }
    //    protected override string GetMemo(SOCompletedCustomerAbandonRequestMessage msg)
    //    {
    //        return "完成客户作废订单申请。";
    //    }
    //}

    #endregion
}
