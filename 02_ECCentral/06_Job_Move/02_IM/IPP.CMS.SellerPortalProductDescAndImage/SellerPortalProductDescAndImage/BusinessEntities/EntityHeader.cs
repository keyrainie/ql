using Newegg.Oversea.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
{
    /// <summary>
    /// 消息头
    /// </summary>
    [Serializable]
    public class EntityHeader
    {
        /// <summary>
        /// 命名空间,命名策略见《WCF Policy，策略2.4》
        /// </summary>
        public string NameSpace
        {
            get;
            set;
        }
        /// <summary>
        /// 动作,例如：Void,Create,Hold,Update,Batch,UnHold,Dispatch,Verify,UnVerify
        /// </summary>
        public string Action
        {
            get;
            set;
        }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version
        {
            get;
            set;
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type
        {
            get;
            set;
        }
        /// <summary>
        /// 发起者
        /// </summary>
        public string Sender
        {
            get;
            set;
        }
        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMapping("CompanyCode", DbType.AnsiStringFixedLength)]
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 关键字
        /// </summary>
        public string Tag
        {
            get;
            set;
        }
        /// <summary>
        /// 语言,如：ENU、CH
        /// </summary>
        public string Language
        {
            get;
            set;
        }
        /// <summary>
        /// 时区
        /// </summary>
        public string TimeZone
        {
            get;
            set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// 原始发起者
        /// </summary>
        public string OriginalSender
        {
            get;
            set;
        }
        /// <summary>
        /// 原始消息号
        /// </summary>
        public string OriginalGUID
        {
            get;
            set;
        }
        /// <summary>
        /// 返回地址
        /// </summary>
        public string CallbackAddress
        {
            get;
            set;
        }

        public string From
        {
            get;
            set;
        }

        public string To
        {
            get;
            set;
        }

        public string FromSystem
        {
            get;
            set;
        }

        public string ToSystem
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }

        public GlobalBusinessType GlobalBusinessType
        {
            get;
            set;
        }

        /// <summary>
        /// Store Company Code
        /// </summary>
        public string StoreCompanyCode
        {
            get;
            set;
        }

        public string TransactionCode
        {
            get;
            set;
        }

        public OperationUserEntity OperationUser
        {
            get;
            set;
        }
    }

    /// <summary>
    /// GlobalBusinessType of Enum type
    /// </summary>
    [Serializable]
    public enum GlobalBusinessType
    {
        VF,
        Normal,
        Listing
    }

    public class OperationUserEntity
    {
        public string CompanyCode { get; set; }
        public string FullName { get; set; }
        public string LogUserName { get; set; }
        public string SourceDirectoryKey { get; set; }
        public string SourceUserName { get; set; }
        public string UniqueUserName { get; set; }
        public int UserSysNo { get; set; }
    }
}
