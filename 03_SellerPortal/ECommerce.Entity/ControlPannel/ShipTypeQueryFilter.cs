using ECommerce.Enums;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class ShipTypeQueryFilter : QueryFilter
    {
        public int? SysNo { get; set; }
        public int? MerchantSysNo { get; set; }
        public string ShipTypeID { get; set; }
        public string ShipTypeName { get; set; }
        public HYNStatus? IsOnlineShow { get; set; }
        public CommonYesOrNo? IsWithPackFee { get; set; }
    }
}
