using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace GroupSettle.Model
{
    public class GroupBuyingSettleMaster
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("SettleAmt", DbType.Decimal)]
        public decimal SettleAmt { get; set; }
      
        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int VendorSysNo { get; set; }

        [DataMapping("BeginDate", DbType.DateTime)]
        public DateTime BeginDate { get; set; }

        [DataMapping("EndDate", DbType.DateTime)]
        public DateTime EndDate { get; set; }

        public List<GroupBuyingSettleItem> ItemList { get; set; }
    }


    public class GroupBuyingSettleItem
    {

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("SettlementSysNo", DbType.Int32)]
        public int SettlementSysNo { get; set; }

        [DataMapping("SettleAmt", DbType.Decimal)]
        public decimal SettleAmt { get; set; }

        [DataMapping("GroupBuyingSysNo", DbType.Int32)]
        public int GroupBuyingSysNo { get; set; }

        public List<GroupBuyingTicketToAcc> AccList { get; set; }
      
    }
}
