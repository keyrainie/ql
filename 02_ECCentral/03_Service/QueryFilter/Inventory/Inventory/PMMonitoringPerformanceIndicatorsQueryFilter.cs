using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Inventory
{
    public class PMMonitoringPerformanceIndicatorsQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 是否根据类别查询
        /// </summary>
        public bool SearchByCategory { get; set; }
        /// <summary>
        /// 已选择的 PM 编号
        /// </summary>
        public string SelectedPMSysNo { get; set; }
        /// <summary>
        /// 已选择的  C1 类 编号
        /// </summary>
        public string SelectedCategory1 { get; set; }
        /// <summary>
        /// 已选择的  C2 类 编号     
        /// </summary>
        public string SelectedCategory2 { get; set; }
        /// <summary>
        /// 已选择的  仓库
        /// </summary>
        public int? StockSysNo { get; set; }
        /// <summary>
        /// 可销售天数比较条件
        /// </summary>
        public string AvailableSalesDaysCondition { get; set; }
        /// <summary>
        /// 日均销量比较条件
        /// </summary>
        public string AVGSaledQtyCondition { get; set; }
        /// <summary>
        /// 可销售天数
        /// </summary>
        public string AvailableSaledDays { get; set; }
        /// <summary>
        /// 日均销量
        /// </summary>
        public string AVGSaledQty { get; set; }

    }
}
