using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;
using ECommerce.Entity.Product;

namespace ECommerce.Entity.Member
{
    /// <summary>
    /// customer 基本信息【[IPP3].[dbo].[Customer]】
    /// 这是一个基本信息Contract,如果需要其他信息，请继承或者修改
    /// 该类。
    /// </summary>
    public class CustomerInfo : EntityBase
    {
        private string nickName;
        public int SysNo { get; set; }

        /// <summary>
        /// customer ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// customer name
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// customer password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// customer email
        /// </summary>
        public string Email { get; set; }

        public int? SocietyID { get; set; }

        public ECustomerGender? Gender { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public CustomerRankType CustomerRank { get; set; }

        /// <summary>
        /// 是否可发表评论
        /// </summary>
        private EYesNo IsAllowComment { get; set; }

        /// <summary>
        /// 用户卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 用户来源
        /// </summary>
        public string FromLinkSource { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.nickName) && this.nickName.Length >= 10)
                {
                    return this.nickName.Substring(0, 8) + "...";
                }
                else
                {
                    return this.nickName;
                }
            }

            set { this.nickName = value; }
        }

        /// <summary>
        /// Gets the name of the mask nickname.
        /// </summary>
        /// <value>
        /// The name of the mask nickname.
        /// </value>
        public string MaskNickName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(this.nickName))
                {
                    if (this.nickName.Length <= 2)
                    {
                        return "**";
                    }

                    char[] array = this.nickName.ToCharArray();
                    sb.Append(array[0]);
                    //int starNumbers = System.Text.Encoding.Default.GetBytes(this.nickName).Length;
                    int starNumbers = array.Length - 2;
                    if (starNumbers > 12)
                    {
                        starNumbers = 12;
                    }

                    for (int index = 0; index < starNumbers; index++)
                    {
                        sb.Append('*');
                    }
                    sb.Append(array[array.Length - 1]);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 用户状态
        /// </summary>
        public CustomerStatusType Status { get; set; }

        /// <summary>
        /// CompanyCustomer
        /// </summary>
        public CompanyType CompanyCustomer { get; set; }

        /// <summary>
        /// 用户下单时的IP
        /// </summary>
        public string CustomerIPAddress { get; set; }

        /// <summary>
        /// 用户下单时的GUID
        /// </summary>
        public string ClientGUID { get; set; }

        /// <summary>
        /// 校园代理
        /// </summary>
        public string AgentCustomerName { get; set; }

        public DateTime? Birthday { get; set; }

        /// <summary>
        /// RegisterTime
        /// </summary>
        public DateTime RegisterTime { get; set; }

        /// <summary>
        /// 创建customer时需要初始化的经验值
        /// </summary>
        public decimal InitTotalSOMoney { get; set; }

        /// <summary>
        /// customer Encrypted password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// customer Encrypted password
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// 当前用户是否是支付宝金账户用户
        /// </summary>
        public bool IsGoldAccount { get; set; }

        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MembershipCard { get; set; }

        /// <summary>
        /// Crm会员ID
        /// </summary>
        public string CrmMemberID { get; set; }

        /// <summary>
        /// 订阅优惠信息
        /// </summary>
        public int IsSubscribe { get; set; }

        /// <summary>
        /// 用户来源
        /// </summary>
        public CustomerSourceType SourceType { get; set; }

        /// <summary>
        /// DB sourceType
        /// </summary>
        public string DBSourceType { get; set; }

        /// <summary>
        /// 获取或设置手机号
        /// </summary>
        public string CellPhone { get; set; }
        public string Phone { get; set; }
        public string DwellAddress { get; set; }
        public int DwellAreaSysNo { get; set; }
        public string DwellZip { get; set; }
        public int ReceiveAreaSysNo { get; set; }

        /// <summary>
        /// 手机验证的积分是否赠送
        /// </summary>
        public int IsPhonePointsSend { get; set; }
        /// <summary>
        /// 邮件验证的积分是否赠送
        /// </summary>
        public int IsEmailPointsSend { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int InitRank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CustomersType { get; set; }
        /// <summary>
        /// 总积分
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal ValidPrepayAmt { get; set; }

        /// <summary>
        /// 当前有效积分
        /// </summary>
        public int ValidScore { get; set; }

        /// <summary>
        /// 网银账户可用积分
        /// </summary>
        public int BankAccountPoint { get; set; }

        /// <summary>
        /// 手机是否验证
        /// </summary>
        public int IsPhoneValided { get; set; }
        /// <summary>
        /// 邮件是否验证
        /// </summary>
        public int IsEmailConfirmed { get; set; }

        public CustomerExtendInfo ExtendInfo { get; set; }

        public int CustomerSysNo { get; set; }
    }
}
