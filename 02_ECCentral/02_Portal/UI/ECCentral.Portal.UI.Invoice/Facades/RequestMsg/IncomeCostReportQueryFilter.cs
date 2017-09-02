using System;
using System.Collections.Generic;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.Invoice.Facades.RequestMsg
{
    public class IncomeCostReportQueryFilter
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

        /// <summary>
        /// 支付方式
        /// </summary>
        public int? PayTypeSysNo { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID
        {
            get;
            set;
        }

        public int? SOSysNo { get; set; }

        public List<int> SOStatusList { get; set; }
        ///// <summary>
        ///// 希望成本统计可以按照商户划分
        ///// </summary>
        //public int? MerchentSysNo { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public List<int> VendorSysNoList { get; set; }
    }
}