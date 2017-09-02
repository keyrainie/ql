using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IRefundRequestDA))]
    public class RefundRequestDA : IRefundRequestDA
    {
        public virtual void Audit(int refundRequestSysNo, BizEntity.Customer.RefundRequestStatus refundRequestStatus, string memo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("RefundRequestAudit");
            cmd.SetParameterValue("@SysNo", refundRequestSysNo);
            cmd.SetParameterValue("@Status", refundRequestStatus);
            cmd.SetParameterValue("@RejectionReason", memo);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUserName");
            cmd.ExecuteNonQuery();
        }
    }
}
