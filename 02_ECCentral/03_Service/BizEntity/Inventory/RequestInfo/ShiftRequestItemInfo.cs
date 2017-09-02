using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 移仓商品列表
    /// </summary>
    public class ShiftRequestItemInfo : IIdentity, ICompany
    {
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 移仓商品
        /// </summary>
        public ProductInfo ShiftProduct { get; set; }

        /// <summary>
        /// 移出渠道仓库
        /// </summary>
        public StockInfo SourceStock { get; set; }

        /// <summary>
        /// 移入渠道仓库
        /// </summary>
        public StockInfo TargetStock { get; set; }

        /// <summary>
        /// 移仓数量
        /// </summary>
        public int ShiftQuantity { get; set; }

        /// <summary>
        /// 移仓数量
        /// </summary>
        public int? InStockQuantity { get; set; }

        /// <summary>
        /// 移仓总重量
        /// </summary>
        public decimal? ShiftWeight { get; set; }

        /// <summary>
        /// 移仓成本
        /// </summary>
        public decimal? ShiftUnitCost { get; set; }


        /// <summary>
        /// 移仓去税成本
        /// </summary>
        public decimal? ShiftUnitCostWithoutTax { get; set; }

        /// <summary>
        /// 配送成本
        /// </summary>
        public decimal? ShippingCost { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public int TotalWeight { get; set; }

        /// <summary>
        /// 产品线
        /// </summary>
        public int ProductLineSysno { get; set; }

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        #endregion
    }
}
