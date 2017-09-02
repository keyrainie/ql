using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(ExperienceProcessor))]

    public class ExperienceProcessor
    {
        #region IExperienceProcess Members

        public virtual void Adjust(BizEntity.Customer.CustomerExperienceLog adjustInfo)
        {
            var totalExperience = GetValidExperience(adjustInfo.CustomerSysNo.Value);
            totalExperience = totalExperience + adjustInfo.Amount.Value;
            if (totalExperience < 0)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerInfo", "CustomerExpericeMustMoreThanZero"));
            }
            ObjectFactory<ICustomerInfoDA>.Instance.UpdateExperience(adjustInfo.CustomerSysNo.Value, totalExperience);
            if (adjustInfo.Type != ExperienceLogType.MerchantSOOutbound)
            {
                CustomerExperienceLog cuslog = new CustomerExperienceLog();
                cuslog.CustomerSysNo = adjustInfo.CustomerSysNo;
                cuslog.Amount = adjustInfo.Amount;
                cuslog.Memo = string.Format(ResouceManager.GetMessageString("Customer.CustomerInfo", "ManualAdjustUserExperience_Log_Memo"),
                     adjustInfo.CustomerSysNo, totalExperience - cuslog.Amount, adjustInfo.Memo);
                cuslog.Type = adjustInfo.Type;
                ObjectFactory<ICustomerInfoDA>.Instance.InsertExperienceLog(cuslog);
            }
            else
            {
                ObjectFactory<RankProcessor>.Instance.SetVIPRank(adjustInfo.CustomerSysNo.Value);
            }

            ObjectFactory<RankProcessor>.Instance.SetRank(adjustInfo.CustomerSysNo.Value);

        }

        public virtual decimal GetValidExperience(int customerSysNo)
        {
            CustomerInfo customer = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBySysNo(customerSysNo);
            return customer.TotalExperience.Value;
        }

        #endregion
    }
}
