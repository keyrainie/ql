using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 订单付款追踪信息
    /// </summary>
    public class TrackingInfo : IIdentity
    {
        /// <summary>
        /// 单据SysNo
        /// </summary>
        public int? OrderSysNo
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
        /// 单据处理进度状态
        /// </summary>
        public TrackingInfoStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 责任人姓名
        /// </summary>
        public string ResponsibleUserName
        {
            get;
            set;
        }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal? IncomeAmt
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
        /// 备注
        /// </summary>
        public string Note
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
        /// 跟踪的收款单收款类型
        /// </summary>
        public SOIncomeOrderStyle? IncomeStyle
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

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members
    }
}