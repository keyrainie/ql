using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.SO
{
    /// <summary>
    /// 订单拦截
    /// </summary>
    public class SOInterceptQueryFilter
    {
        /// <summary>
        /// 渠道编号
        /// </summary> 
        public string WebChannelID{get;set;}

        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }

        /// <summary>
        /// 配送方式类别
        /// </summary>
        public string ShipTypeCategory { get; set; }

        /// <summary>
        /// 配送方式枚举
        /// </summary>
        public string ShipTypeEnum { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string StockSysNo { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        public int ShipTypeSysNo { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        public string ShippingTypeID { get; set; }

        /// <summary>
        /// 有无运单号
        /// </summary>
        public string HasTrackingNumber { get; set; }

        /// <summary>
        /// 配送时间类别
        /// </summary>
        public string ShipTimeType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 订单拦截收件人 邮件地址
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// 订单拦截抄送人 邮件地址
        /// </summary>
        public string CCEmailAddress { get; set; }

        /// <summary>
        /// 发票拦截收件人 邮件地址
        /// </summary>
        public string FinanceEmailAddress { get; set; }

        /// <summary>
        /// 发票拦截收件人 邮件地址
        /// </summary>
        public string FinanceCCEmailAddress { get; set; }
    }
}
