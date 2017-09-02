using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(AreaDeliveryAppService))]
    public class AreaDeliveryAppService
    {
        public virtual AreaDeliveryInfo Create(AreaDeliveryInfo entity)
        {
            return ObjectFactory<AreaDeliveryProcessor>.Instance.Create(entity);
        }

        public virtual AreaDeliveryInfo Update(AreaDeliveryInfo entity)
        {
            return ObjectFactory<AreaDeliveryProcessor>.Instance.Update(entity);
        }

        public virtual void Delete(int transactionNumber)
        {
            ObjectFactory<AreaDeliveryProcessor>.Instance.Delete(transactionNumber);
        }

        public virtual AreaDeliveryInfo GetAreaDeliveryInfoByID(int transactionNumber)
        {
            return ObjectFactory<AreaDeliveryProcessor>.Instance.GetAreaDeliveryInfoByID(transactionNumber);
        }
    }
}
