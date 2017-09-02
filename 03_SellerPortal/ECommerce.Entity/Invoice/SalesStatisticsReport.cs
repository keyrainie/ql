using ECommerce.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Invoice
{
    public class SalesStatisticsReport
    {
        /// <summary>
        /// 查询结果列表
        /// </summary>
        public QueryResult<SalesStatistics> SalesStatisticsResult { get; set; }

        /// <summary>
        /// 统计信息
        /// </summary>
        public List<IncomeCostReportStatistic> CostReportStatisticList { get; set; }

        public SalesStatisticsReport()
        {
            this.SalesStatisticsResult = new QueryResult<SalesStatistics>();

            this.CostReportStatisticList = new List<IncomeCostReportStatistic>();
        }

    }


    public class SalesStatistics
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }
        public int ProductGroupSysno { get; set; }
        public DateTime BeginDate { get; set; }

		public DateTime EndDate	{ get; set; }
		public int Quantity { get; set; }
		public decimal? ProductCost { get; set; }
		public decimal? ProductPriceAmount { get; set; }
		public decimal? PromotionDiscountAmount { get; set; }
		public decimal? ProductSaleAmount { get; set; }
		public decimal? ProductGrossMargin { get; set; }
        public int VendorSysNo { get; set; }
        public int WarehouseNumber { get; set; }
        public string VendorName { get; set; }
        public string StockName { get; set; }
		public string ProductID { get; set; }
		public string ProductName { get; set; }
		public string C1Name { get; set; }
		public string C2Name { get; set; }
		public string C3Name { get; set; }
		public int BrandSysNo { get; set; }
		public string BrandName { get; set; }
		public string BrandName_En { get; set; }
		public int PayTypeSysNo { get; set; }
		public string PayTypeName { get; set; }
        public string BMCode { get; set; }
		public string ProductProperty1 { get; set; }
        public string ProductProperty2 { get; set; }


    }

    public class IncomeCostReportStatistic
    {
        /// <summary>
        /// 销售成本
        /// </summary>
        public decimal? ProductCost { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? ProductPriceAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? PromotionDiscountAmount { get; set; }

        /// <summary>
        /// 实际销售金额
        /// </summary>
        public decimal? ProductSaleAmount { get; set; }

        /// <summary>
        /// 商品毛利
        /// </summary>
        public decimal? ProductGrossMargin { get; set; }
    }
}
