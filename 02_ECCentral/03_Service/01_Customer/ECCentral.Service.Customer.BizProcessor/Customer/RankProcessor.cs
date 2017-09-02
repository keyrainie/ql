using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(RankProcessor))]
    public class RankProcessor
    {
        #region IRankProcess Members

        public virtual CustomerRank GetRank(int customerSysNo)
        {
            var rank = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBySysNo(customerSysNo).Rank;
            if (rank.HasValue)
                return rank.Value;
            else
                return CustomerRank.Ferrum;
        }
        public virtual VIPRank GetVIPRank(int customerSysNo)
        {
            var rank = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBySysNo(customerSysNo).VIPRank;
            if (rank.HasValue)
                return rank.Value;
            else
                return VIPRank.NormalManual;

        }

        public virtual void SetRank(int customerSysNo)
        {
            ObjectFactory<IRankDA>.Instance.SetRank(customerSysNo);
        }

        public virtual void SetVIPRank(int CustomerSysNo)
        {
            //现在的逻辑：如果是自动调整的话，那么当经验值大于30000的时候，就调整成VIP
            var viprank = GetVIPRank(CustomerSysNo);
            if (viprank != VIPRank.NormalManual && viprank != VIPRank.VIPManual)
            {
                if (ObjectFactory<ExperienceProcessor>.Instance.GetValidExperience(CustomerSysNo) > int.Parse(AppSettingManager.GetSetting("customer", "TheLeastExperienceOfVIPAuto")))
                {
                    ObjectFactory<IRankDA>.Instance.SetVIPRank(CustomerSysNo, VIPRank.VIPAuto);
                }
            }
        }

        public virtual void SetVIPRank(int CustomerSysNo, VIPRank rank)
        {
            ObjectFactory<IRankDA>.Instance.SetVIPRank(CustomerSysNo, rank);
        }

        #endregion
    }
}
