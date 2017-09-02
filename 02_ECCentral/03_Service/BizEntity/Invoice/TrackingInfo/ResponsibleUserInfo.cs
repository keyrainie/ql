using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class ResponsibleUserInfo : IIdentity
    {
        /// <summary>
        /// 付款方式系统编号
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
        /// 顾客系统编号
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
        /// 来源方式
        /// </summary>
        public ResponsibleSource? SourceType
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单类型
        /// </summary>
        public ResponseUserOrderStyle? IncomeStyle
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