using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    public class ConsignAdjustInfo
    {

        public int? SysNo { get; set; }

        public int? SettleSysNo
        {
            get;
            set;
        }


        public string SettleRange
        {
            get;
            set;
        }


        public DateTime? SettleRangeDate
        {
            get;
            set;
        }


        public int? VenderSysNo
        {
            get;
            set;
        }


        public string VenderName
        {
            get;
            set;
        }



        public int? PMSysNo
        {
            get;
            set;
        }


        public List<ConsignAdjustItemInfo> ItemList
        {
            get;
            set;
        }

        public ConsignAdjustStatus? Status { get; set; }


        public decimal? TotalAmt { get; set; }
    }
}
