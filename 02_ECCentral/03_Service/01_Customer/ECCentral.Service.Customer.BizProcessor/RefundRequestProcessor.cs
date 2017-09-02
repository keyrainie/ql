using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(RefundRequestProcessor))]
    public class RefundRequestProcessor
    {

        public virtual void Audit(int refundRequestSysNo, RefundRequestStatus refundRequestStatus, string memo)
        {
            ObjectFactory<IRefundRequestDA>.Instance.Audit(refundRequestSysNo, refundRequestStatus, memo);
        }
    }
}
