using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.OrderMgmt.ServiceJob.BusinessEntities.SecKill
{
    public class ItemEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("BasicPrice", DbType.Decimal)]
        public decimal BasicPrice { get; set; }

        [DataMapping("CashRebate", DbType.Decimal)]
        public decimal CashRebate { get; set; }

        [DataMapping("CurrentPrice", DbType.Decimal)]
        public decimal CurrentPrice { get; set; }

        [DataMapping("Discount", DbType.Decimal)]
        public decimal Discount { get; set; }

        [DataMapping("UnitCost", DbType.Decimal)]
        public decimal UnitCost { get; set; }

        [DataMapping("UnitCostWithoutTax", DbType.Decimal)]
        public decimal UnitCostWithoutTax { get; set; }

        [DataMapping("Point", DbType.Int32)]
        public int Point { get; set; }

        [DataMapping("PointType", DbType.Int32)]
        public int PointType { get; set; }

        [DataMapping("ClearanceSale", DbType.Int32)]
        public int ClearanceSale { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataMapping("IsExistRankPrice", DbType.Int32)]
        public int IsExistRankPrice { get; set; }

        [DataMapping("IsWholeSale", DbType.Int32)]
        public int IsWholeSale { get; set; }

        [DataMapping("MaxPerOrder", DbType.Int32)]
        public int MaxPerOrder { get; set; }

        [DataMapping("P1", DbType.Decimal)]
        public decimal P1 { get; set; }

        [DataMapping("P2", DbType.Decimal)]
        public decimal P2 { get; set; }

        [DataMapping("P3", DbType.Decimal)]
        public decimal P3 { get; set; }

        [DataMapping("Q1", DbType.Int32)]     
        public int Q1 { get; set; }

        [DataMapping("Q2", DbType.Int32)]
        public int Q2 { get; set; }

        [DataMapping("Q3", DbType.Int32)]
        public int Q3 { get; set; }

        [DataMapping("TLRequestMemo", DbType.String)]
        public string TLRequestMemo { get; set; }

        [DataMapping("PMRequestMemo", DbType.String)]
        public string PMRequestMemo { get; set; }       

    }
}
