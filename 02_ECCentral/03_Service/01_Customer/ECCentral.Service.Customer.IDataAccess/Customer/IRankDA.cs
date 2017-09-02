using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IRankDA
    {
        void SetRank(int customerSysNo);
        void SetRank(int customerSysNo, CustomerRank rank);
        void SetVIPRank(int customerSysNo);
        void SetVIPRank(int CustomerSysNo, VIPRank rank);
    }
}
