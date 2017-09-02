using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(BuyLimitRuleAppService))]
    public class BuyLimitRuleAppService
    {
        private BuyLimitRuleProcessor _bizBuyLimitRule = ObjectFactory<BuyLimitRuleProcessor>.Instance;

        public virtual BuyLimitRule Load(int sysNo)
        {
            return _bizBuyLimitRule.Load(sysNo);
        }

        public virtual void Insert(BuyLimitRule data)
        {
            _bizBuyLimitRule.Insert(data);
        }

        public virtual void Update(BuyLimitRule data)
        {
            _bizBuyLimitRule.Update(data);
        }
    }
}
