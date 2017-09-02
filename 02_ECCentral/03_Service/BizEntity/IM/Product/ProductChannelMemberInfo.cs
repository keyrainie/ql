using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    #region Property
    /// <summary>
    /// 渠道信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductChannelMemberInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [DataMember]
        public string ChannelName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// 插入时间
        /// </summary>
        [DataMember]
        public DateTime InDate { get; set; }

        /// <summary>
        /// 插入用户
        /// </summary>
        [DataMember]
        public string InUser { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 企业短号
        /// </summary>
        [DataMember]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 语言编号
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }
    }

    /// <summary>
    /// 渠道会员价格表
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductChannelMemberPriceInfo : IIdentity
    {
        /// <summary>
        /// 渠道会员价格表编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 渠道会员编号
        /// </summary>
        [DataMember]
        public int ChannelSysNO { get; set; }

        /// <summary>
        /// 渠道会员名称
        /// </summary>
        [DataMember]
        public string ChannelName { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// 泰隆优选价
        /// </summary>
        [DataMember]
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 折扣价
        /// </summary>
        [DataMember]
        public decimal? MemberPrice { get; set; }

        /// <summary>
        /// 折扣比例
        /// </summary>
        [DataMember]
        public decimal? MemberPricePercent { get; set; }

        /// <summary>
        /// 插入时间
        /// </summary>
        [DataMember]
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 插入用户
        /// </summary>
        [DataMember]
        public string InUser { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        [DataMember]
        public string EditUser { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 企业短号
        /// </summary>
        [DataMember]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }
    }

    /// <summary>
    /// 渠道会员变动日志查询
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductChannelMemberPriceLogInfo : IIdentity
    {
        /// <summary>
        /// 渠道会员变动日志查询
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [DataMember]
        public string ChannelName { get; set; }

        /// <summary>
        /// 折扣价格
        /// </summary>
        [DataMember]
        public decimal? MemberPrice { get; set; }

        /// <summary>
        /// 折扣比例
        /// </summary>
        [DataMember]
        public decimal? MemberPricePercent { get; set; }

        /// <summary>
        /// 操作类型 A:新增 E:编辑 D:作废
        /// </summary>
        [DataMember]
        public string OperationType { get; set; }

        /// <summary>
        /// 插入时间
        /// </summary>
        [DataMember]
        public DateTime InDate { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        [DataMember]
        public string InUser { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 企业短号
        /// </summary>
        [DataMember]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        ///  操作类型 A:新增 E:编辑 D:作废
        /// </summary>
        public string DispayOperationType
        {
            get
            {
                return OperationType == "A"
                    ? "新增"
                    : OperationType == "B" 
                        ? "编辑" : "作废";
            }
        }
    }
    #endregion
}
