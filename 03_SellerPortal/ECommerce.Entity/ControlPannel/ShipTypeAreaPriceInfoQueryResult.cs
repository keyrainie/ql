using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;
using ECommerce.WebFramework;

namespace ECommerce.Entity.ControlPannel
{
    public class ShipTypeAreaPriceInfoQueryResult : ShipTypeAreaPriceInfo
    {
        public string ShipTypeName { get; set; }
        public string AreaName { get; set; }
        public string VendorName { get; set; }
        public string UICompanyCustomer
        {
            get
            {
                return this.CompanyCustomer.GetDescription();
            }
        }
        public string UIUnitPrice
        {
            get
            {
                return Formatting.FormatMoney(this.UnitPrice);
            }
        }
        public string UIMaxPrice
        {
            get
            {
                return Formatting.FormatMoney(this.MaxPrice);
            }
        }
    }
}
