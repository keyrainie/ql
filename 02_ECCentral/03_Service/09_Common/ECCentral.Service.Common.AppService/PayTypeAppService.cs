using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(PayTypeAppService))]
    public class PayTypeAppService
    {
        public virtual PayType Create(PayType entity)
        {
            return ObjectFactory<PayTypeProcessor>.Instance.Create(entity);
        }

        public virtual PayType Update(PayType entity)
        {
            return ObjectFactory<PayTypeProcessor>.Instance.Update(entity);
        }

        public PayType LoadPayType(int sysNo)
        {
            return ObjectFactory<PayTypeProcessor>.Instance.LoadPayType(sysNo);
        }
    }
}
