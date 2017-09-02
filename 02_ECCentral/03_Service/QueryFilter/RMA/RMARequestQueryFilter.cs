using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.RMA;

namespace ECCentral.QueryFilter.RMA
{
    public class RMARequestQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public DateTime? ReceivedDateFrom{ get; set; }
        public DateTime? ReceivedDateTo{ get; set; }
        public DateTime? ETakeDateFrom { get; set; }
        public DateTime? ETakeDateTo { get; set; }
        public bool? IsVIP { get; set; }
        public int? CustomerSysNo { get; set; }
        public bool? IsSubmit { get; set; }
        public int? ReceiveUserSysNo { get; set; }
        public RMARequestStatus? Status { get; set; }
        public string SOID { get; set; }
        public string RequestID { get; set; }
        public string CustomerID { get; set; }
        
        public int? SysNo { get; set; }
        public int? SOSysNo { get; set; }

        public bool? IsLabelPrinted { get; set; }

        public SellersType? SellersType { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string WebChannelID { get; set; }
        public string CompanyCode { get; set; }

        public string ServiceCode { get; set; }

        #region 多余字段读取相关读取

        /// <summary>
        /// 是否读取相关单件项SysNo,以某种连接连接成字符串
        /// </summary>
        public bool IsReadRMAItemSysNos
        {
            get;
            set;
        }

        public DateTime? AuditDateFrom { get; set; }
        public DateTime? AuditDateTo { get; set; }
        public int? AuditUserSysNo { get; set; }

        #endregion
    }
}
