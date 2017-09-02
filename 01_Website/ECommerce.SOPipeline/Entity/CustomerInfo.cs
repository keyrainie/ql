using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 下单的注册会员信息
    /// </summary>
    public class CustomerInfo : ExtensibleObject
    {
        /// <summary>
        /// 注册会员系统主键
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 注册会员登录账号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 注册会员名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 会员帐户可用余额
        /// </summary>
        public decimal AccountBalance { get; set; }

        /// <summary>
        /// 会员账户可用积分
        /// </summary>
        public int AccountPoint { get; set; }

        /// <summary>
        /// 网银账户可用积分
        /// </summary>
        public int BankAccountPoint { get; set; }

        /// <summary>
        /// 会员等级
        /// </summary>
        public int CustomerRank { get; set; }

        /// <summary>
        /// 手机是否验证
        /// </summary>
        public int IsPhoneValided { get; set; }
        /// <summary>
        /// 邮件是否验证
        /// </summary>
        public int IsEmailConfirmed { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string CellPhone { get; set; }

        public override ExtensibleObject CloneObject()
        {
            return new CustomerInfo()
            {
                SysNo = this.SysNo,
                ID = this.ID,
                Name = this.Name,
                AccountBalance = this.AccountBalance,
                CustomerRank = this.CustomerRank,
                IsEmailConfirmed = this.IsEmailConfirmed,
                IsPhoneValided = this.IsPhoneValided,
                CellPhone = this.CellPhone,
            };
        }
    }
}
