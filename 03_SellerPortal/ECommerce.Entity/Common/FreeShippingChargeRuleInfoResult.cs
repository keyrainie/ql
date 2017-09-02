using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class FreeShippingChargeRuleInfoResult : FreeShippingChargeRuleInfo
    {
        public string UIStartDate
        {
            get
            {
                return this.StartDate.Value.ToString("yyyy-MM-dd");
            }
        }

        public string UIEndDate
        {
            get
            {
                return this.EndDate.Value.ToString("yyyy-MM-dd");
            }
        }
    }
}
