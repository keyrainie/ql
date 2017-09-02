using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    /// <summary>
    /// 用户实名认证信息
    /// </summary>
    public class CustomerAuthenticationInfo
    {
        public int? SysNo { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public IDCardType? IDCardType { get; set; }
        /// <summary>
        /// 证件号
        /// </summary>
        public string IDCardNumber { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public ECustomerGender? Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
    }
}
