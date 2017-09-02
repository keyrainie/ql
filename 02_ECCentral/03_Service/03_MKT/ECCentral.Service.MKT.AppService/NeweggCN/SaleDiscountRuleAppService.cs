using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.AppService
{
   [VersionExport(typeof(SaleDiscountRuleAppService))]
    public class SaleDiscountRuleAppService
    {
        private SaleDiscountRuleProcessor _bizSaleDiscountRule = ObjectFactory<SaleDiscountRuleProcessor>.Instance;

        public virtual SaleDiscountRule Load(int sysNo)
        {
            return _bizSaleDiscountRule.Load(sysNo);
        }

        public virtual void Insert(SaleDiscountRule data)
        {
            _bizSaleDiscountRule.Insert(data);
        }

        public virtual void Update(SaleDiscountRule data)
        {
            _bizSaleDiscountRule.Update(data);
        }
    }
}
