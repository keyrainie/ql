using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.EventMessage.Invoice;
using ECCentral.Service.EventMessage.Customer;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.WPMessage.BizProcessor
{
    public class AuditRMARefundMessageTask : WPMessageCreator<AuditCustomerRMARefundMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 601;
        }

        protected override string GetBizSysNo(AuditCustomerRMARefundMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(AuditCustomerRMARefundMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(AuditCustomerRMARefundMessage msg)
        {
            return "审核通过RMA退款申请单";
        }

        protected override int GetCurrentUserSysNo(AuditCustomerRMARefundMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

        protected override bool NeedProcess(AuditCustomerRMARefundMessage msg)
        {
            throw new NotImplementedException();
            ////筛选是否审核过了
            //DataTable dt = ObjectFactory<ICustomerBizInteract>.Instance.GetRefundRequestBySysNo(msg.SysNo,RefundRequestStatus.A);
            ////没有审核过
            //if(dt.Rows.Count<=0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
    }

    public class RejectRMARefundMessageTask:WPMessageCreator<RejectCustomerRMARefundMessage>
    {

        protected override int GetCategorySysNo()
        {
            return 601;
        }

        protected override string GetBizSysNo(RejectCustomerRMARefundMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetUrlParameter(RejectCustomerRMARefundMessage msg)
        {
            return msg.SysNo.ToString();
        }

        protected override string GetMemo(RejectCustomerRMARefundMessage msg)
        {
            return "审核通过RMA退款申请单";
        }

        protected override int GetCurrentUserSysNo(RejectCustomerRMARefundMessage msg)
        {
            return msg.CurrentUserSysNo;
        }

        protected override bool NeedProcess(RejectCustomerRMARefundMessage msg)
        {
            throw new NotImplementedException();
            ////筛选是否审核不通过
            //DataTable dt = ObjectFactory<ICustomerBizInteract>.Instance.GetRefundRequestBySysNo(msg.SysNo, RefundRequestStatus.R);
            ////没有审核不通过
            //if (dt.Rows.Count <= 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
    }
}
