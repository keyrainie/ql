using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoClose.Model
{
    public class POItem
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("POSysNo", DbType.Int32)]
        public int POSysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("BriefName", DbType.String)]
        public string BriefName { get; set; }

        [DataMapping("Quantity", DbType.Int32)]
        public int Quantity { get; set; }

        [DataMapping("Weight", DbType.Int32)]
        public int Weight { get; set; }

        [DataMapping("OrderPrice", DbType.Decimal)]
        public Decimal? OrderPrice { get; set; }

        [DataMapping("ApportionAddOn", DbType.Decimal)]
        public Decimal? ApportionAddOn { get; set; }

        [DataMapping("UnitCost", DbType.Decimal)]
        public Decimal? UnitCost { get; set; }

        [DataMapping("ReturnCost", DbType.Decimal)]
        public Decimal? ReturnCost { get; set; }

        [DataMapping("PurchaseQty", DbType.Int32)]
        public int PurchaseQty { get; set; }

        [DataMapping("CheckStatus", DbType.Int32)]
        public int? CheckStatus { get; set; }

        [DataMapping("CheckReasonMemo", DbType.String)]
        public string CheckReasonMemo { get; set; }

        [DataMapping("lastOrderPrice", DbType.Decimal)]
        public Decimal? lastOrderPrice { get; set; }

        [DataMapping("ExecptStatus", DbType.Int32)]
        public int ExecptStatus { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("UnitCostWithoutTax", DbType.Decimal)]
        public decimal? UnitCostWithoutTax { get; set; }

        [DataMapping("JDPrice", DbType.Decimal)]
        public decimal? JDPrice { get; set; }

        [DataMapping("AvailableQty", DbType.Int32)]
        public int? AvailableQty { get; set; }

        [DataMapping("m1", DbType.Int32)]
        public int? m1 { get; set; }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("ProductMode", DbType.String)]
        public string ProductMode { get; set; }

        [DataMapping("ExchangeRate", DbType.Decimal)]
        public decimal? ExchangeRate { get; set; }

        [DataMapping("CurrencySysmbol", DbType.String)]
        public string CurrencySysmbol { get; set; }           //货币标记

        [DataMapping("PMUserSysNo", DbType.Int32)]
        public int PMUserSysNo { get; set; }

        [DataMapping("CurrentPrice", DbType.Decimal)]
        public decimal? CurrentPrice { get; set; }           //当前价格

        [DataMapping("CurrentUnitCost", DbType.Decimal)]
        public decimal? CurrentUnitCost { get; set; }

        [DataMapping("LastAdjustPriceDate", DbType.DateTime)]
        public DateTime? LastAdjustPriceDate { get; set; }

        [DataMapping("LastInTime", DbType.DateTime)]
        public DateTime? LastInTime { get; set; }

        [DataMapping("Tax", DbType.Decimal)]
        public decimal? Tax { get; set; }                    //毛利率

        [DataMapping("LineCost", DbType.Decimal)]
        public decimal? LineCost { get; set; }               //总价

        [DataMapping("LineReturnedPointCost", DbType.Decimal)]
        public decimal? LineReturnedPointCost { get; set; }  //抵扣后总价

        [DataMapping("CheckStatusStr", DbType.String)]
        public string CheckStatusStr { get; set; }           //检查状态

        [DataMapping("JDTax", DbType.Decimal)]
        public decimal? JDTax { get; set; }                  //京东价毛利率

        [DataMapping("Point", DbType.Int32)]
        public int Point { get; set; }

        [DataMapping("IsVirtualStockProduct", DbType.String)]
        public string IsVirtualStockProduct { get; set; }               //虚库商品

        [DataMapping("LocalCurrencySymbol", DbType.String)]
        public string LocalCurrencySymbol { get; set; } //本地指定的货币类型

        //实际总价（=已入库总额）
        [DataMapping("ActualPrice", DbType.Decimal)]
        public decimal? ActualPrice { get; set; }

        public string ActualPriceStr
        {
            get
            {
                if (this.ActualPrice.HasValue)
                {
                    return this.ActualPrice.Value.ToString("0.00");
                }
                return "0.00";
            }
            set { }
        }
    }
}
