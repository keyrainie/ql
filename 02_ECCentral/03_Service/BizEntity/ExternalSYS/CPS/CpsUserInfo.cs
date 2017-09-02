using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.ExternalSYS
{
    /// <summary>
    /// CPS用户信息
    /// </summary>
   public class CpsUserInfo:ICompany,ILanguage
    {
       public int SysNo { get; set; }
        /// <summary>
       /// 基本信息
       /// </summary>
       public CpsBasicUserInfo UserBasicInfo { get; set; }

       /// <summary>
       /// 账户余额
       /// </summary>
       public decimal BalanceAmt { get; set; }

       /// <summary>
       /// 是否提供发票
       /// </summary>
       public CanProvideInvoice CanProvideInvoice { get; set; }

       /// <summary>
       /// 收款账户信息
       /// </summary>
       public CpsReceivablesAccount ReceivablesAccount { get; set; }

       /// <summary>
       /// Source
       /// </summary>
       public CpsUserSource Source { get; set; }

       public UserInfo User { get; set; }

       /// <summary>
       /// 审核状态
       /// </summary>
       public AuditStatus Status { get; set; }
       /// <summary>
       /// 拒绝原因
       /// </summary>
       public string AuditNoClearanceInfo { get; set; }
       public string CompanyCode
       {
           get;
           set;
       }

       public string LanguageCode
       {
           get;
           set;
       }
    }

    /// <summary>
    /// 用户基本信息
    /// </summary>
   public class CpsBasicUserInfo 
   {
       /// <summary>
       /// 用户ID
       /// </summary>
       public string CPSCustomerID { get; set; }

       /// <summary>
       /// 账户类型
       /// </summary>
       public UserType? UserType { get; set; }

       /// <summary>
       /// 是否可用
       /// </summary>
       public IsActive? IsActive { get; set; }

       /// <summary>
       /// 联盟账号
       /// </summary>
       public string AllianceAccount { get; set; }

       /// <summary>
       /// 网站类型
       /// </summary>
       public string WebSiteCode { get; set; }
       /// <summary>
       /// 网址
       /// </summary>
       public string WebSiteAddress { get; set; }

       /// <summary>
       /// 网站名称
       /// </summary>
       public string WebSiteName { get; set; }

       /// <summary>
       /// 邮箱
       /// </summary>
       public string Email { get; set; }

       /// <summary>
       /// 联系人
       /// </summary>
       public string Contact { get; set; }

       /// <summary>
       /// 联系手机
       /// </summary>
       public string ContactPhone { get; set; }

       /// <summary>
       /// 通讯地址
       /// </summary>
       public string ContactAddress { get; set; }

       /// <summary>
       /// 邮政编码
       /// </summary>
       public string Zipcode { get; set; }
   }
    /// <summary>
    /// 收款账户信息
    /// </summary>
   public class CpsReceivablesAccount 
   {
       /// <summary>
       /// 收款账户类型
       /// </summary>
       public UserType? ReceivablesAccountType { get; set; }

       /// <summary>
       /// 收款人姓名
       /// </summary>
       public string ReceiveablesName { get; set; }

       /// <summary>
       /// 开户银行Code
       /// </summary>
       public string BrankCode { get; set; }

       /// <summary>
       /// 开户银行名称
       /// </summary>
       public string BrankName { get; set; }

       /// <summary>
       ///开户银行支行
       /// </summary>
       public string BranchBank { get; set; }

       /// <summary>
       /// 开户卡号
       /// </summary>
       public string BrankCardNumber { get; set; }

       /// <summary>
       /// 是否锁定
       /// </summary>
       public IsLock? IsLock { get; set; }
   }

   public class CpsUserSource 
   {

       public int SysNo { get; set; }
       /// <summary>
       ///账户类型
       /// </summary>
       public UserType? UserType { get; set; }

       /// <summary>
       /// 渠道名称
       /// </summary>
       public string ChanlName { get; set; }

       /// <summary>
       /// Source
       /// </summary>
       public string Source { get; set; }
   }
}
