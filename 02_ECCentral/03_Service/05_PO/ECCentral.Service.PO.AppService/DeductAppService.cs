using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(DeductAppService))]
    public class DeductAppService
    {
        public Deduct  CreateDeduct(Deduct info)
        {
            return ObjectFactory<DeductProcessor>.Instance.CreateDeductInfo(info);
        }

        public Deduct MaintainDeduct(Deduct info)
        {
            return ObjectFactory<DeductProcessor>.Instance.MaintainDeductInfo(info);
        }
    }
}
