using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace ProductSaleInfoBiz.Model
{
    public class ProductSaleInfo
    {
        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }
        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }
        [DataMapping("Category1Name", DbType.String)]
        public string Category1Name { get; set; }
        [DataMapping("Category2Name", DbType.String)]
        public string Category2Name { get; set; }
        [DataMapping("Category3Name", DbType.String)]
        public string Category3Name { get; set; }
        [DataMapping("Manufacturername", DbType.String)]
        public string Manufacturername { get; set; }
        [DataMapping("LastPMName", DbType.String)]
        public string LastPMName { get; set; }
        [DataMapping("CurrentPrice", DbType.Decimal)]
        public decimal? CurrentPrice { get; set; }
        [DataMapping("UnitCost", DbType.Decimal)]
        public decimal? UnitCost { get; set; }
        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime? CreateTime { get; set; }
        [DataMapping("FirstOnlineTime", DbType.DateTime)]
        public DateTime? FirstOnlineTime { get; set; }
        [DataMapping("VirtualQty", DbType.Int32)]
        public int? VirtualQty { get; set; }
        [DataMapping("OnlineAccountQty", DbType.Int32)]
        public int? OnlineAccountQty { get; set; }
        [DataMapping("QuantityW1", DbType.Int32)]
        public int? QuantityW1 { get; set; }
        [DataMapping("ProductAmtW1", DbType.Decimal)]
        public decimal? ProductAmtW1 { get; set; }
        [DataMapping("QuantityM1", DbType.Int32)]
        public int? QuantityM1 { get; set; }
        [DataMapping("ProductAmtM1", DbType.Decimal)]
        public decimal? ProductAmtM1 { get; set; }
         //,begin14days.Quantity AS Quantity14days 
         //  ,begin14days.ProductAmt AS ProductAmt14days
        [DataMapping("Quantity14days", DbType.Int32)]
        public int? Quantity14days { get; set; }
        [DataMapping("ProductAmt14days", DbType.Decimal)]
        public decimal? ProductAmt14days { get; set; }

        [DataMapping("Quantity60days", DbType.Int32)]
        public int? Quantity60days { get; set; }
        [DataMapping("ProductAmt60days", DbType.Decimal)]
        public decimal? ProductAmt60days { get; set; }
        [DataMapping("Domain", DbType.String)]
        public string Domain { get; set; }
        [DataMapping("DetailCount", DbType.Int32)]
        public int? DetailCount { get; set; }

        [DataMapping("AllAmt", DbType.Decimal)]
        public decimal? AllAmt { get; set; }

        [DataMapping("AllQuantity", DbType.Int32)]
        public int? AllQuantity { get; set; }
        [DataMapping("LastInTime", DbType.DateTime)]
        public DateTime? LastInTime { get; set; }

    }
}
