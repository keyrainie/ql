using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class ShiftProductCompanyEntity : ICompany, ILanguage
    {
        public int? StItemSysNo { get; set; }
        /// <summary>
        /// 月份
        /// </summary>

        public DateTime? OutTime { get; set; }

        public DateTime? InTime { get; set; }

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
        public decimal? UnitCostCount { get; set; }

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

        //////////////crl17402//////////////////////
        public string SapCoCodeFrom { get; set; }
        public string SapCoCodeTo { get; set; }

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        public string LanguageCode
        {
            get;
            set;
        }

        #endregion
    }
}
