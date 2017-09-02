using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.RMA;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IRefundQueryDA
    {
        DataTable QueryRefund(RefundQueryFilter filter, out int totalCount);

        DataTable GetWaitingRegisters(int? soSysNo);

        DataTable GetRefundPrintDetail(int sysNo);

        DataTable GetRefundPrintItems(int refundSysNo);
    }
}
