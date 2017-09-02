using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility;
using ECCentral.Service.RMA.BizProcessor;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(CustomerContactAppService))]
    public class CustomerContactAppService
    {
        public virtual CustomerContactInfo LoadByRequestSysNo(int sysNo)
        {
            return ObjectFactory<CustomerContactProcessor>.Instance.LoadByRequestSysNo(sysNo);
        }

        public virtual CustomerContactInfo LoadOriginByRequestSysNo(int sysNo)
        {
            return ObjectFactory<CustomerContactProcessor>.Instance.LoadOriginByRequestSysNo(sysNo);
        }

        public virtual CustomerContactInfo Create(CustomerContactInfo contactInfo)
        {
            return ObjectFactory<CustomerContactProcessor>.Instance.Create(contactInfo);
        }

        public virtual void Update(CustomerContactInfo contactInfo)
        {
            ObjectFactory<CustomerContactProcessor>.Instance.Update(contactInfo);
        }
    }
}
