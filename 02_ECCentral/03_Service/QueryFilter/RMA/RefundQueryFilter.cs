using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;

namespace ECCentral.QueryFilter.RMA
{
    public class RefundQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }
        public string RefundID { get; set; }
        public int? SOSysNo { get; set; }
        public string SOSysNoString { get; set; }// 支持多张 SO 单，SO 单编号以 . 号分隔（现在支持 ,（逗号） .（点号）  （空格）分隔）
        public int? CustomerSysNo { get; set; }
        public RMARefundStatus? Status { get; set; }
        public int? ProductSysNo { get; set; }
        public DateTime? CreateTimeFrom { get; set; }
        public DateTime? CreateTimeTo { get; set; }
        public DateTime? RefundTimeFrom { get; set; }
        public DateTime? RefundTimeTo { get; set; }
        public bool? IsVIP { get; set; }
        public string WarehouseCreated { get; set; }
        /// <summary>
        /// 发票所在地
        /// </summary>
        public string InvoiceLocation { get; set; }

        /// <summary>
        /// 出货仓库
        /// </summary>
        public string WarehouseShipped { get; set; }
        public RefundStatus? AuditStatus { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string WebChannelID { get; set; }
        public string CompanyCode { get; set; }
    }
}
