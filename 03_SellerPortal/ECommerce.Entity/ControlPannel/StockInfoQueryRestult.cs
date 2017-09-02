using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.ControlPannel
{
    public class StockInfoQueryRestult : StockInfo
    {
        public bool CanEdit
        {
            get
            {
                return this.MerchantSysNo != 1;
            }
        }

        public string UIStatus
        {
            get
            {
                return this.Status.GetDescription();
            }
        }

        public string UIStockType
        {
            get
            {
                return this.StockType.GetDescription();
            }
        }
    }
}
