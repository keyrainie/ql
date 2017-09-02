using System;
using System.Net;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 顾客回访信息
    /// </summary>
    public class VisitCustomer : IIdentity, ICompany
    {
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }
        /// <summary>
        /// 是否被激活
        /// </summary>
        public bool? IsActive
        {
            get;
            set;
        }
        /// <summary>
        /// 上次购买时间
        /// </summary>
        public DateTime? LastBuyTime
        {
            get;
            set;
        }
        /// <summary>
        /// 上次激活时间
        /// </summary>
        public DateTime? LastActiveTime
        {
            get;
            set;
        }
        /// <summary>
        /// 上次通话时间
        /// </summary>
        public VisitDealStatus? LastCallStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 联系次数
        /// </summary>
        public int? ContactCount
        {
            get;
            set;
        }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal? OrderAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 订单次数
        /// </summary>
        public int? OrderCount
        {
            get;
            set;
        }
        /// <summary>
        /// 是否有退换货
        /// </summary>
        public bool? IsRMA
        {
            get;
            set;
        }
        /// <summary>
        /// 是否有购买意向
        /// </summary>
        public YNStatusThree? ConsumeDesire
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status
        {
            get;
            set;
        }
        /// <summary>
        /// 上次通话时间
        /// </summary>
        public DateTime? LastCallTime
        {
            get;
            set;
        }
        /// <summary>
        /// 退换货金额
        /// </summary>
        public decimal? RMAAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 上次维护状态
        /// </summary>
        public VisitDealStatus? LastMaintenanceStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 回访结果
        /// </summary>
        public VisitCallResult? LastCallResult
        {
            get;
            set;
        }

        #region IIdentity Members
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        #endregion

        #region ICompany Members

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

        #endregion
    }
    /// <summary>
    /// 待回访顾客信息
    /// </summary>
    public class WaitingVisitCustomer : IIdentity
    {
        /// <summary>
        /// 客户系统编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status
        {
            get;
            set;
        }
        /// <summary>
        /// 锁定用户系统编号
        /// </summary>
        public int? LockUserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone
        {
            get;
            set;
        }
        /// <summary>
        /// 上次购买时间
        /// </summary>
        public DateTime LastBuyDate
        {
            get;
            set;
        }
        /// <summary>
        /// 上次呼叫时间
        /// </summary>
        public DateTime LastCallTime
        {
            get;
            set;
        }
        /// <summary>
        /// 回访系统编号
        /// </summary>
        public int? VisitSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisterTime
        {
            get;
            set;
        }

        #region IIdentity Members
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion
    }
    /// <summary>
    /// 回访日志
    /// </summary>
    public class VisitLog : IIdentity, ICompany
    {
        /// <summary>
        /// 客户系统编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }
        /// <summary>
        /// 回访系统编号
        /// </summary>
        public int? VisitSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 处理状态
        /// </summary>
        public VisitDealStatus? DealStatus //CallStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 回访结果
        /// </summary>
        public VisitCallResult? CallResult //TelType
        {
            get;
            set;
        }
        /// <summary>
        /// 提醒时间
        /// </summary>
        public DateTime? RemindDate
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
        /// QQ
        /// </summary>
        public string QQ
        {
            get;
            set;
        }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string MSN
        {
            get;
            set;
        }
        /// <summary>
        /// 购买意向
        /// </summary>
        public YNStatusThree? ConsumeDesire
        {
            get;
            set;
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 添加用户的系统账户名称
        /// </summary>
        public string InUserAcct { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? InDate { get; set; }

        #region IIdentity Members
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion

        #region ICompany Members
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyCode { get; set; }

        #endregion
    }

    /// <summary>
    /// 顾客维护信息
    /// </summary>
    public class VisitOrder : IIdentity
    {
        public int? SysNo
        {
            get;
            set;
        }
        public int CustomerSysNo
        {
            get;
            set;
        }
        public int VisitSysNo
        {
            get;
            set;
        }
        public int SoSysNo
        {
            get;
            set;
        }
        public string RMASysNo
        {
            get;
            set;
        }
        public int CreateUserSysNo
        {
            get;
            set;
        }
        public int LastEditUserSysNo
        {
            get;
            set;
        }
        public string LanguageCode
        {
            get;
            set;
        }
        public string StoreCompanyCode
        {
            get;
            set;
        }
    }

}
