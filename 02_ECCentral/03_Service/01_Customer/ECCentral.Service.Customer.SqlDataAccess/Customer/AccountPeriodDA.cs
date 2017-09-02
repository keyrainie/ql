using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IAccountPeriodDA))]
    public class AccountPeriodDA : IAccountPeriodDA
    {
        #region IAccountPeriodDA Members

        public virtual void CreateAccountPeriodInfo(AccountPeriodInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateAccountPeriodInfo");
            cmd.SetParameterValue<AccountPeriodInfo>(entity);
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdateAccountPeriodInfo(AccountPeriodInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateAccountPeriodInfo");
            cmd.SetParameterValue<AccountPeriodInfo>(entity);
            cmd.ExecuteNonQuery();
        }

        public virtual BizEntity.Customer.AccountPeriodInfo GetAccountPeriodInfoByCustomerSysNo(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerAccountPeriodInfo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteEntity<AccountPeriodInfo>();
        }

        public virtual void UpdateAvailableCreditLimit(int customerSysNo, decimal? availableCreditLimit)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateAvailableCreditLimit");
            cmd.SetParameterValue("@AvailableCreditLimit", availableCreditLimit);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.ExecuteNonQuery();      
        }

        #endregion
    }
}
