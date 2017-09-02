using ECCentral.BizEntity.Invoice.Refund;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IRefundPointDA
    {
        int Insert(RefundPointInfo info);

        void Update(RefundPointInfo info);

        void UpdateSOIncome(RefundPointInfo info);
    }
}
