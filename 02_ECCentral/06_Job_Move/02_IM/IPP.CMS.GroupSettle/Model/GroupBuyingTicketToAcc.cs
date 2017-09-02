using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace GroupSettle.Model
{
    public class GroupBuyingTicketToAcc
    {

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("GroupBuyingSysNo", DbType.Int32)]      
        public int GroupBuyingSysNo { get; set; }      

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }             

        [DataMapping("SettlementItemSysNo", DbType.Int32)]
        public int? SettlementItemSysNo { get; set; }

        [DataMapping("UsedStoreSysNo", DbType.Int32)]
        public int UsedStoreSysNo { get; set; }

        [DataMapping("UsedDate", DbType.DateTime)]
        public DateTime UsedDate { get; set; }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int VendorSysNo { get; set; }

        [DataMapping("CostAmt", DbType.Decimal)]
        public decimal CostAmt { get; set; }

        [DataMapping("PayPeriodType", DbType.Int32)]
        public int PayPeriodType { get; set; }
     
    }  
    public enum SettleStatus
    {
        /// <summary>
        /// 未结算
        /// </summary>
        UnSettle=0,
        /// <summary>
        /// 已结算
        /// </summary>
        Settled=1
    }
}
