using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IAccountPeriodDA
    {
        void CreateAccountPeriodInfo(AccountPeriodInfo entity);
        void UpdateAccountPeriodInfo(AccountPeriodInfo entity);
        void UpdateAvailableCreditLimit(int customerSysNo, decimal? availableCreditLimit);
        AccountPeriodInfo GetAccountPeriodInfoByCustomerSysNo(int customerSysNo);
    }
}
