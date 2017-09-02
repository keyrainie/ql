using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface ICustomerContactDA
    {
        CustomerContactInfo Insert(CustomerContactInfo entity);

        bool Update(CustomerContactInfo entity);

        bool UpdateByRequestSysNo(CustomerContactInfo entity);

        CustomerContactInfo Load(int sysNo);

        CustomerContactInfo LoadOrigin(int sysNo);

        CustomerContactInfo LoadByRegisterSysNo(int sysNo);
    }
}
