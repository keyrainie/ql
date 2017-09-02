using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Inventory
{
    public class ProductShiftDetail
    {

        public int? StItemSysNo { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public DateTime? OutTime { get; set; }

        public string OutTimeString
        {
            get
            {
                if (OutTime != null)
                {
                    return OutTime.Value.ToString("yyyy-MM");
                }

                return string.Empty;
            }
        }

        public DateTime? InTime { get; set; }

        /// <summary>
        /// 移仓单号
        /// </summary>
        public int? ShiftSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? ShiftQty { get; set; }

        /// <summary>
        /// 单位成本(*)
        /// </summary>
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// 商品总金额(*)
        /// </summary>
        public decimal? AmtCount { get; set; }

        /// <summary>
        /// 商品税金(*)
        /// </summary>
        public decimal? AmtTaxItem { get; set; }

        /// <summary>
        /// 商品总金额
        /// </summary>
        public decimal? AmtProductCost { get; set; }

        /// <summary>
        /// 移仓单总金额
        /// </summary>
        public decimal? AtTotalAmt { get; set; }

        /// <summary>
        /// 移出地
        /// </summary>
        public string StockNameA { get; set; }

        /// <summary>
        /// 移入地
        /// </summary>
        public string StockNameB { get; set; }

        /// <summary>
        /// 金税号
        /// </summary>
        public string GoldenTaxNo { get; set; }

        public string InvoiceNo { get; set; }

        public int? StockSysNoA { get; set; }

        public int? StockSysNoB { get; set; }

        public int? ShiftType { get; set; }

        public decimal? AjustRate { get; set; }

        //////////////crl17402//////////////////////
        public string SapCoCodeFrom { get; set; }

        public string SapCoCodeTo { get; set; }

        public string AmtCompanyCountInfo { get; set; }

        public bool NeedManual { get; set; }

    }

    public class ProductShiftDetailAmtInfo
    {
        public int? CountType { get; set; }
        public decimal? AmtCount { get; set; }
        public decimal? AmtTaxItem { get; set; }
        public decimal? AmtProductCost { get; set; }
    }

    public class ProductShiftCheck
    {
        public int? ShiftSysNo { get; set; }
        public int? ProductSysNo { get; set; }
        public decimal? UnitCost { get; set; }
    }

    public class ShiftSysnoProduct
    {
        public int? ProductSysNo { get; set; }

        public int? ShiftSysNo { get; set; }
        public decimal? UnitCost { get; set; }

        public DateTime? OutTime { get; set; }
        public string StockNameB { get; set; }

        public int? StockSysNoA { get; set; }
        public int? StockSysNoB { get; set; }
    }
}
