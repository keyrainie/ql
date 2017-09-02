using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class CurrencyInfo
    {
        public int? SysNo { get; set; }

        public int? CurrencyID { get; set; }

        public bool IsLocal { get; set; }

        public string CurrencySymbol { get; set; }

        public string CurrencyName { get; set; }

        public decimal ExchangeRate { get; set; }
    }
}
