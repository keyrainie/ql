using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 顾客信息，该信息对应到现实中某个人
    /// </summary>
    [Serializable]
    [DataContract]
    public class PhysicsCustomer
    {
        [DataMember]
        public int? PhysicsCustomerSysNo { get; set; }

        [DataMember]
        List<CustomerInfo> CustomerInfoList { get; set; }
    }
    /// <summary>
    /// 客户信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomerInfo : IIdentity, IWebChannel
    {
        private int? sysNo;
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                sysNo = value;
            }
        }
        public CustomerInfo()
        {
            BasicInfo = new CustomerBasicInfo();
            PasswordInfo = new PasswordInfo();
            AccountPeriodInfo = new AccountPeriodInfo();
            ShippingAddressList = new List<ShippingAddressInfo>();
            ValueAddedTaxInfoList = new List<ValueAddedTaxInfo>();
            AgentInfo = new AgentInfo();
            RightList = new List<CustomerRight>();
        }
        #region 详细信息

        /// <summary>
        /// 总积分
        /// </summary>        
        [DataMember]
        public int? TotalScore { get; set; }
        /// <summary>
        /// 有效积分
        /// </summary>        
        [DataMember]
        public int? ValidScore { get; set; }
        /// <summary>
        /// 总余额
        /// </summary>
        [DataMember]
        public decimal? ValidPrepayAmt { get; set; }
        /// <summary>
        /// 总经验值
        /// </summary>
        [DataMember]
        public decimal? TotalExperience { get; set; }
        /// <summary>
        /// 顾客级别
        /// </summary>        
        [DataMember]
        public CustomerRank? Rank { get; set; }

        /// <summary>
        /// VIP等级
        /// </summary>
        [DataMember]
        public VIPRank? VIPRank { get; set; }
        /// <summary>
        /// 拍卖级别
        /// </summary>        
        [DataMember]
        public AuctionRank? AuctionRank { get; set; }
        /// <summary>
        /// 贡献等级
        /// </summary>        
        [DataMember]
        public string ContributeRank { get; set; }
        /// <summary>
        /// 发送顾客等级邮件时间
        /// </summary>
        [DataMember]
        public DateTime? SendCustomerRankEmailDate { get; set; }
        /// <summary>
        /// 最近登陆时间
        /// </summary>        
        [DataMember]
        public DateTime? LastLoginDate { get; set; }
        /// <summary>
        /// 购买总次数
        /// </summary>
        [DataMember]
        public int? BuyCount { get; set; }
        /// <summary>
        /// 购买总金额
        /// </summary>
        [DataMember]
        public decimal? TotalSOMoney { get; set; }
        /// <summary>
        /// 确认购买总金额
        /// </summary>
        [DataMember]
        public decimal? ConfirmedTotalAmt { get; set; }
        /// <summary>
        /// 是否是系统帐号，系统账户可以发送邮件
        /// </summary>
        [DataMember]
        public bool? IsSystemUser { get; set; }
        /// <summary>
        /// 是否订阅邮件
        /// </summary>
        [DataMember]
        public bool? IsSubscribe { get; set; }
        /// <summary>
        /// 是否为恶意用户
        /// </summary>
        [DataMember]
        public bool? IsBadCustomer { get; set; }
        /// <summary>
        /// 绑定的VIP卡号
        /// </summary>        
        [DataMember]
        public string VipCardNo { get; set; }
        /// <summary>
        /// 上次购买信息
        /// </summary>
        [DataMember]
        public int? LastReceiveAreaSysNo { get; set; }
        /// <summary>
        /// 上次购买时使用的配送方式
        /// </summary>
        [DataMember]
        public int? LastShipTypeSysNo { get; set; }
        /// <summary>
        /// 上次购买使用的支付方式
        /// </summary>
        [DataMember]
        public int? LastPayTypeSysNo { get; set; }
        /// <summary>
        /// 上传购买时间
        /// </summary>
        [DataMember]
        public DateTime? LastBuyDate { get; set; }
        /// <summary>
        /// 积分过期时间
        /// </summary>
        [DataMember]
        public DateTime? PointExpiringDate { get; set; }
        /// <summary>
        /// 顾客类型
        /// </summary>        
        [DataMember]
        public CustomerType CustomersType { get; set; }

        #endregion
        /// <summary>
        /// 顾客基本信息
        /// </summary>
        [DataMember]
        public CustomerBasicInfo BasicInfo { get; set; }
        /// <summary>
        /// 顾客密码信息
        /// </summary>
        [DataMember]
        public PasswordInfo PasswordInfo { get; set; }
        /// <summary>
        /// 顾客账期信息
        /// </summary>
        [DataMember]
        public AccountPeriodInfo AccountPeriodInfo { get; set; }
        /// <summary>
        /// 顾客代理信息
        /// </summary>
        [DataMember]
        public AgentInfo AgentInfo { get; set; }
        /// <summary>
        /// 顾客配送地址清单
        /// </summary>
        [DataMember]
        public List<ShippingAddressInfo> ShippingAddressList { get; set; }
        /// <summary>
        /// 顾客增值税信息清单
        /// </summary>
        [DataMember]
        public List<ValueAddedTaxInfo> ValueAddedTaxInfoList { get; set; }
        /// <summary>
        /// 顾客拥有的权限列表
        /// </summary>
        [DataMember]
        public List<CustomerRight> RightList { get; set; }
        /// <summary>
        /// 顾客预付款日志
        /// </summary>
        [DataMember]
        public List<CustomerPrepayLog> PrepayLogList { get; set; }
        /// <summary>
        /// 顾客经验值日志
        /// </summary>
        [DataMember]
        public List<CustomerExperienceLog> ExperienceLogList { get; set; }
        /// <summary>
        /// 顾客的奖品列表
        /// </summary>
        [DataMember]
        public List<CustomerGift> GiftList { get; set; }
        /// <summary>
        /// 顾客加积分申请列表
        /// </summary>
        [DataMember]
        public List<CustomerPointsAddRequest> PointsAddRequestList { get; set; }
        /// <summary>
        /// 顾客积分日志列表
        /// </summary>
        [DataMember]
        public List<Point> PointList { get; set; }
        /// <summary>
        /// 对顾客的操作记录列表
        /// </summary>
        [DataMember]
        public CustomerOperateLog OperateLog { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        [DataMember]
        public Common.WebChannel WebChannel { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

    }
    /// <summary>
    /// 客户基础信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomerBasicInfo
    {
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        [DataMember]
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 顾客业务ID
        /// </summary>        
        [DataMember]
        public string CustomerID { get; set; }
        /// <summary>
        /// 顾客姓名
        /// </summary>        
        [DataMember]
        public string CustomerName { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>        
        [DataMember]
        public string IdentityCard { get; set; }
        /// <summary>
        /// 顾客昵称
        /// </summary>        
        //[DataMember]
        //public string NickName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>        
        [DataMember]
        public CustomerStatus? Status { get; set; }
        /// <summary>
        /// 头像路径
        /// </summary>        
        [DataMember]
        public string AvtarImage { get; set; }
        /// <summary>
        /// 头像状态
        /// </summary>        
        [DataMember]
        public AvtarShowStatus? AvtarImageStatus { get; set; }

        /// <summary>
        /// 顾客来源
        /// </summary>        
        [DataMember]
        public string FromLinkSource { get; set; }
        /// <summary>
        /// 用户喜好语言
        /// </summary>
        [DataMember]
        public string FavoriteLanguageCode { get; set; }
        /// <summary>
        /// 性别
        /// </summary>        
        [DataMember]
        public Gender? Gender { get; set; }
        /// <summary>
        /// 邮件地址
        /// </summary>        
        [DataMember]
        public string Email { get; set; }
        /// <summary>
        /// 邮箱是否验证
        /// </summary>
        [DataMember]
        public bool? IsEmailConfirmed { get; set; }
        /// <summary>
        /// 电话
        /// </summary>        
        [DataMember]
        public string Phone { get; set; }
        /// <summary>
        /// 手机
        /// </summary>        
        [DataMember]
        public string CellPhone { get; set; }
        /// <summary>
        /// 是否电话验证
        /// </summary>
        [DataMember]
        public bool? CellPhoneConfirmed { get; set; }
        /// <summary>
        /// 传真
        /// </summary>        
        [DataMember]
        public string Fax { get; set; }
        /// <summary>
        /// 居住地SysNo
        /// </summary>
        [DataMember]
        public int? DwellAreaSysNo { get; set; }
        /// <summary>
        /// 居住地地址
        /// </summary>        
        [DataMember]
        public string DwellAddress { get; set; }
        /// <summary>
        /// 居住地邮编
        /// </summary>        
        [DataMember]
        public string DwellZip { get; set; }
        /// <summary>
        /// 注册IP地址
        /// </summary>
        [DataMember]
        public string RegisterIPAddress { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>        
        [DataMember]
        public DateTime? RegisterTime { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>        
        [DataMember]
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 推荐用户系统编号
        /// </summary>
        [DataMember]
        public int? RecommendedByCustomerSysNo { get; set; }
        /// <summary>
        /// 推荐用户系统账号
        /// </summary>
        [DataMember]
        public string RecommendedByCustomerID { get; set; }
        [DataMember]
        public String Memo { get; set; }
        /// <summary>
        /// 是否是学生
        /// </summary>
        [DataMember]
        public bool? StudentFlag { get; set; }
        /// <summary>
        /// 所在区域
        /// </summary>
        [DataMember]
        public string RegionName { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public int? CompanyCustomer { get; set; }

        [DataMember]
        public int? ChannelSysNo { get; set; }

    }
    /// <summary>
    /// 客户密码信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class PasswordInfo
    {
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        [DataMember]
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 加密密码的key
        /// </summary>
        [DataMember]
        public string PasswordSalt { get; set; }
        /// <summary>
        /// 顾客密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }
    }
    /// <summary>
    /// 账期信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class AccountPeriodInfo
    {
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        [DataMember]
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 是否赋予发帖特权
        /// </summary>
        [DataMember]
        public bool? IsAllowComment { get; set; }
        /// <summary>
        /// 是否使用支票
        /// </summary>
        [DataMember]
        [Obsolete("此字段已弃用")]
        public bool? IsUseChequesPay { get; set; }
        /// <summary>
        /// 累计购买金额
        /// </summary>
        [DataMember]
        public decimal? ConfirmedTotalAmt { get; set; }
        /// <summary>
        /// 账期天数
        /// </summary>
        [DataMember]
        [Obsolete("此字段已弃用")]
        public int? AccountPeriodDays { get; set; }
        /// <summary>
        /// 账期额度
        /// </summary>
        [DataMember]
        [Obsolete("此字段已弃用")]
        public decimal? TotalCreditLimit { get; set; }
        /// <summary>
        /// 可用账期额度
        /// </summary>
        [DataMember]
        [Obsolete("此字段已弃用")]
        public decimal? AvailableCreditLimit { get; set; }
    }
}