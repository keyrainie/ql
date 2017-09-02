using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using ECCentral.BizEntity.PO;

namespace POASNMgmt.AutoCreateVendorSettle.Entities
{
   
    public class GatherSettleInfo
    {
        [DataMapping("ItemType", DbType.String)]
        public string ItemType { get; set; }
        [DataMapping("InvoiceNumber", DbType.Int32)]
        public int? InvoiceNumber { get; set; }  //单据编号
        [DataMapping("OrderDate", DbType.DateTime)]
        public DateTime? OrderDate { get; set; }  //    --创建日期
        [DataMapping("WarehouseNumber", DbType.Int32)]
        public int? WarehouseNumber { get; set; }//--仓库
        [DataMapping("SettleStatus", DbType.String)]
        public string SettleStatus { get; set; } // --状态
        [DataMapping("VendorSysno", DbType.Int32)]
        public int? VendorSysno { get; set; }
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int? ProductSysNo { get; set; }
        [DataMapping("OriginalPrice", DbType.Decimal)]
        public decimal? OriginalPrice { get; set; }//--销售价格
        [DataMapping("Quantity", DbType.Int32)]
        public int? Quantity { get; set; }     //--数量
        [DataMapping("VendorName", DbType.String)]
        public string VendorName { get; set; }  //--供应商
        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; } //--商品编号
        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }//--商品名称
        [DataMapping("StockName", DbType.String)]
        public string StockName { get; set; }
        [DataMapping("SettleType", DbType.String)]
        public string SettleType { get; set; }
        [DataMapping("TransactionNumber", DbType.Int32)]
        public int? TransactionNumber { get; set; }

        [DataMapping("SONumber", DbType.Int32)]
        public int? SONumber { get; set; }
        [DataMapping("SoItemSysno", DbType.Int32)]
        public int? SoItemSysno { get; set; }
        [DataMapping("Point", DbType.Decimal)]
        public decimal? Point { get; set; }
        [DataMapping("SettleSysNo", DbType.Int32)]
        public int? SettleSysNo { get; set; }
        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }

        [DataMapping("PromotionDiscount", DbType.Decimal)]
        public decimal? PromotionDiscount { get; set; }

        
        public GatherSettleType? EnumSettleType
        {
            get
            {
                GatherSettleType? resault = null;
                switch (SettleType)
                {
                    case "SO":
                        resault = GatherSettleType.SO;
                        break;
                    case "RMA":
                        resault = GatherSettleType.RMA;
                        break;
                    //case "RO_Adjust":
                    //    resault = GatherSettleType.RO_Adjust;
                    //    break;
                    default:
                        resault = null;
                        break;
                }
                return resault;
            }
        }
    }
}
