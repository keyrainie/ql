using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IRefundRequestDA
    {
          void Audit(int refundRequestSysNo, BizEntity.Customer.RefundRequestStatus refundRequestStatus, string memo);
    }
}
