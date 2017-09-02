using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Invoice
{
    public class CommissionMasterInfo : EntityBase
    {

        public int SysNo { get; set; }

        public string InUser { get; set; }

        public DateTime InDate { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public SettleOrderStatus Status { get; set; }

        public string MerchantSysNo { get; set; }

        public string Memo { get; set; }

    }
}
