using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.Restful.ResponseMsg
{
    public class MKTProductDetailMsg
    {
        public int? ProductSysNo { get; set; }
        public string ProductName { get; set; }
        public int? AvailableQty { get; set; }
        public int? AccountQty { get; set; }
        public int? ReservedQty { get; set; }
        public int? CurrentReservedQty { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? Price { get; set; }
        public decimal? Point { get; set; }
        public decimal? GrossMarginRate { get; set; }
        public decimal? JDPrice { get; set; }
        public int  W1 { get; set; }
        public int  W2 { get; set; }
        public int  W3 { get; set; }
        public int  W4 { get; set; }
        public int  M1 { get; set; }
        public int  M2 { get; set; }
        public int  M3 { get; set; }
        public DateTime? LastPurchaseDate { get; set; }


    }
}
