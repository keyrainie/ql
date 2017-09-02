using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class TrackingInfoQueryFilter
    {
        /// <summary>
        /// 单据ID
        /// </summary>
        public string OrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        public SOIncomeOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 收款类型
        /// </summary>
        public SOIncomeOrderStyle? IncomeStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 出库日期（从）
        /// </summary>
        public DateTime? OutFromDate
        {
            get;
            set;
        }

        /// <summary>
        /// 出库日期（到）
        /// </summary>
        public DateTime? OutToDate
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        public int? ShipTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 损失类型
        /// </summary>
        public int? LossType
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单责任人姓名
        /// </summary>
        public string ResponsibleUserName
        {
            get;
            set;
        }

        /// <summary>
        /// 单据处理进度：业务跟进
        /// </summary>
        public bool HasStatusFollow
        {
            get;
            set;
        }

        /// <summary>
        /// 单据处理进度：提交报损
        /// </summary>
        public bool HasStatusSubmit
        {
            get;
            set;
        }

        /// <summary>
        /// 单据处理进度：核销完毕
        /// </summary>
        public bool HasStatusConfirm
        {
            get;
            set;
        }

        /// <summary>
        /// 单据添加方式
        /// </summary>
        public TrackingInfoStyle? InType
        {
            get;
            set;
        }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public string ChannelID
        {
            get;
            set;
        }
    }
}