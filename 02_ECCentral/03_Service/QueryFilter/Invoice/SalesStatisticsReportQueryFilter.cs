using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class SalesStatisticsReportQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public DateTime? SODateFrom { get; set; }
        public DateTime? SODateTo { get; set; }
        public string ProductID { get; set; }
        public string BrandName { get; set; }
        public int? C1SysNo { get; set; }
        public int? C2SysNo { get; set; }
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public List<int> VendorSysNoList { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        public List<string> WarehouseNumberList { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public List<int> SOStatusList { get; set; }
    }
}
