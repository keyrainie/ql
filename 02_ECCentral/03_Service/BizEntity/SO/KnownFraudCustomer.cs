using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 欺诈用户
    /// </summary>
    public class KnownFraudCustomer
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 配送联系人
        /// </summary>
        public string ShippingContact { get; set; }

        /// <summary>
        /// 配送地址
        /// </summary>
        public string ShippingAddress { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string BrowseInfo { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public FPStatus? Status { get; set; }

        /// <summary>
        /// 欺诈类型
        /// </summary>
        public KFCType? KFCType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public DateTime? LastEditDate { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastEditUserName { get; set; }
    }
}
