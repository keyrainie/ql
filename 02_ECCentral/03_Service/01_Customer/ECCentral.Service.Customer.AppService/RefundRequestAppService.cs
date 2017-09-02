using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.BizProcessor;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(RefundRequestAppService))]
    public class RefundRequestAppService
    {
        public virtual void Audit(List<int> list, RefundRequestStatus refundRequestStatus, string memo)
        {
            list.ForEach(item =>
            {
                ObjectFactory<RefundRequestProcessor>.Instance.Audit(item, refundRequestStatus, memo);
            });
        }
    }
}
