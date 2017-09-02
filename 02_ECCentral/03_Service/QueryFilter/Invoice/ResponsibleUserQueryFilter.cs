using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// 跟踪单责任人查询条件
    /// </summary>
    public class ResponsibleUserQueryFilter
    {
        /// <summary>
        /// 收款单类型
        /// </summary>
        public ResponseUserOrderStyle? IncomeStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        public int? ShipTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerSysNo
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
        /// 责任人邮件地址
        /// </summary>
        public string EmailAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 责任人状态
        /// </summary>
        public ResponseUserStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是特殊模式责任人
        /// </summary>
        public bool? IsSpecialMode
        {
            get;
            set;
        }

        /// <summary>
        /// 责任人来源方式
        /// </summary>
        public ResponsibleSource? SourceType
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
        /// 分页信息
        /// </summary>
        public ECCentral.QueryFilter.Common.PagingInfo PagingInfo
        {
            get;
            set;
        }
    }
}