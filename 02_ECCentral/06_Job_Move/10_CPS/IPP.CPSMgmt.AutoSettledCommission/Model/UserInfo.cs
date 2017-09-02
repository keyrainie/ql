using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Model
{
    /// <summary>
    /// 联盟用户信息实体类
    /// </summary>
    public class UserInfo
    {
        #region Table Field 对应表字段
        /// <summary>
        /// 获取或设置系统编号
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        /// <summary>
        /// 获取或设置关联IPP用户信息表
        /// </summary>
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 获取或设置用户类型
        ///  1.'P'：个人 Personal
        ///  2.'E'：企业 Enterprise"
        /// </summary>
        [DataMapping("UserType", DbType.String)]
        public string UserType { get; set; }

        /// <summary>
        /// 获取或设置站点类型Code
        /// </summary>
        [DataMapping("SiteTypeCode", DbType.String)]
        public string SiteTypeCode { get; set; }

        /// <summary>
        /// 获取或设置网址
        /// </summary>
        [DataMapping("WebSiteUrl", DbType.String)]
        public string WebSiteUrl { get; set; }

        /// <summary>
        /// 获取或设置网站名称
        /// </summary>
        [DataMapping("WebSiteName", DbType.String)]
        public string WebSiteName { get; set; }

        /// <summary>
        /// 获取或设置邮箱
        /// </summary>
        [DataMapping("Email", DbType.String)]
        public string Email { get; set; }

        [DataMapping("NewEggEmail", DbType.String)]
        public string NewEggEmail { get; set; }

        /// <summary>
        /// 获取或设置联系人
        /// </summary>
        [DataMapping("ContactName", DbType.String)]
        public string ContactName { get; set; }

        /// <summary>
        /// 获取或设置联系手机
        /// </summary>
        [DataMapping("ContactPhone", DbType.String)]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 获取或设置通讯地址
        /// </summary>
        [DataMapping("Address", DbType.String)]
        public string Address { get; set; }

        /// <summary>
        /// 获取或设置邮政编码
        /// </summary>
        [DataMapping("ZipCode", DbType.String)]
        public string ZipCode { get; set; }

        /// <summary>
        /// 获取或设置用户银行代码
        /// </summary>
        [DataMapping("BankCode", DbType.String)]
        public string BankCode { get; set; }

        /// <summary>
        /// 获取或设置用户银行名称
        /// </summary>
        [DataMapping("BankName", DbType.String)]
        public string BankName { get; set; }

        /// <summary>
        /// 获取或设置开户支行信息
        /// </summary>
        [DataMapping("BranchBank", DbType.String)]
        public string BranchBank { get; set; }

        /// <summary>
        /// 获取或设置银行卡卡号码
        /// </summary>
        [DataMapping("BankCardNumber", DbType.String)]
        public string BankCardNumber { get; set; }

        /// <summary>
        /// 获取或设置收款人名称
        /// </summary>
        [DataMapping("ReceivableName", DbType.String)]
        public string ReceivableName { get; set; }

        /// <summary>
        /// 获取或设置账户余额
        /// </summary>
        [DataMapping("BalanceAmt", DbType.Decimal)]
        public decimal BalanceAmt { get; set; }

        /// <summary>
        /// 会员ID
        /// </summary>
        [DataMapping("CustomerID", DbType.String)]
        public string CustomerID { get; set; }

        #endregion Table Field

        #region Relevance Table Field 关联其他表
        /// <summary>
        /// 获取或设置用户名称
        /// </summary>
        [DataMapping("UserName", DbType.String)]
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置用户密码
        /// </summary>
        [DataMapping("Password", DbType.String)]
        public string Password { get; set; }

        /// <summary>
        /// 用户图片名称
        /// </summary>
        [DataMapping("AvtarImage", DbType.String)]
        public string AvtarImage { get; set; }

        /// <summary>
        /// 用户图片状态
        /// </summary>
        [DataMapping("AvtarImageStatus", DbType.String)]
        public string AvtarImageStatus { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        [DataMapping("Rank", DbType.Int32)]
        public int Rank { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        [DataMapping("IsAvailable", DbType.String)]
        public string IsAvailable { get; set; }

        /// <summary>
        /// 用户登录时 EC 服务生成的 ID
        /// </summary>
        public string CustomerLoginID { get; set; }

        /// <summary>
        /// 用户登录时 EC 服务生成的验证码
        /// </summary>
        public string AuthenticationCode { get; set; }

        /// <summary>
        /// 是否提供发票
        /// </summary>
        [DataMapping("CanProvideInvoice", DbType.String)]
        public string CanProvideInvoice { get; set; }
        #endregion Relevance Table Field 关联其他表


        #region Common Field 公用字段
        /// <summary>
        /// 获取或设置创建人
        /// </summary>
        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        /// <summary>
        /// 获取或设置创建时间
        /// </summary>
        [DataMapping("InDate", DbType.DateTime)]
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 获取或设置编辑人
        /// </summary>
        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        /// <summary>
        /// 获取或设置编辑时间
        /// </summary>
        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 获取或设置公司代码
        /// </summary>
        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 获取或设置公司代码
        /// </summary>
        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 获取或设置语言编码
        /// </summary>
        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }
        #endregion Common Field
    }
}
