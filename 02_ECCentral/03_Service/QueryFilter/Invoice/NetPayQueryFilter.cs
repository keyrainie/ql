using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// NetPay查询过滤条件
    /// </summary>
    public class NetPayQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public SOStatus? SOStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式
        /// </summary>
        public int? ShipTypeCode
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public int? PayTypeCode
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public NetPayStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间从
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间到
        /// </summary>
        public DateTime? CreateDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 来源
        /// </summary>
        public NetPaySource? Source
        {
            get;
            set;
        }

        /// <summary>
        /// 分仓
        /// </summary>
        public string StockID
        {
            get;
            set;
        }

        /// <summary>
        /// 配送日期
        /// </summary>
        public DateTime? DeliveryDate
        {
            get;
            set;
        }

        /// <summary>
        /// 配送时间范围
        /// </summary>
        public int? DeliveryTimeRange
        {
            get;
            set;
        }

        /// <summary>
        /// 金额范围从
        /// </summary>
        public decimal? AmtFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 金额范围到
        /// </summary>
        public decimal? AmtTo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单类型
        /// </summary>
        public SOType? SOType
        {
            get;
            set;
        }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID
        {
            get;
            set;
        }

        /// <summary>
        /// 团购状态
        /// </summary>
        public GroupBuyingSettlementStatus? SettlementStatus
        {
            get;
            set;
        }
    }
}