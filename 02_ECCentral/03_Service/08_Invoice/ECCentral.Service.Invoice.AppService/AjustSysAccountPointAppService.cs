using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(AjustSysAccountPointAppService))]
    public class AjustSysAccountPointAppService
    {
        ICustomerBizInteract CustomerBizInteract = ObjectFactory<ICustomerBizInteract>.Instance;
        public List<CustomerBasicInfo> LoadSysAccountList(string channelID)
        {
            return CustomerBizInteract.GetSystemAccount(channelID);
        }

        public int GetSysAccountValidScore(int customerSysNo)
        {
            return CustomerBizInteract.GetCustomerVaildScore(customerSysNo);

        }

        public void AjustSysAccountPoint(AdjustPointRequest entity)
        {
            CustomerBizInteract.AdjustPoint(entity);
        }
    }
}
