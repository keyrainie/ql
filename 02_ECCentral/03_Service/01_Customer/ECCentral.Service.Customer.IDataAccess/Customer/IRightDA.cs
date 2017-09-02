using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{

    public interface IRightDA
    {
        List<CustomerRight> LoadAllCustomerRight(int customerSysNo);

        bool CreateCustomerRight(int customerSysNo, int right);

        bool UpdateCustomerRight(int customerSysNo, int right);

    }
}
