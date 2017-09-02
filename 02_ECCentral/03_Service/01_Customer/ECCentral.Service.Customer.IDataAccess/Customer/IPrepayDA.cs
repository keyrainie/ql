using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IPrepayDA
    {
        void UpdatePrepay(int customerSysNo, decimal prepay);
        void CreatePrepayLog(CustomerPrepayLog log);
    }
}
