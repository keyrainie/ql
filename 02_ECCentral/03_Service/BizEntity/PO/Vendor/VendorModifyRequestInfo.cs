using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{

    /// <summary>
    /// 供应商修改请求信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorModifyRequestInfo
    {

        /// <summary>
        /// 系统编号
        /// </summary>
      [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 修改请求系统编号
        /// </summary>
      [DataMember]
      public int? RequestSysNo { get; set; }

        /// <summary>
        /// 供应商等级
        /// </summary>
      [DataMember]
      public VendorRank? Rank { get; set; }

        /// <summary>
        /// 供应商系统编号
        /// </summary>
      [DataMember]
      public int VendorSysNo { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
      [DataMember]
      public string VendorName { get; set; }

        /// <summary>
        /// 供应商账期信息
        /// </summary>
      [DataMember]
      public VendorPayTermsItemInfo PayPeriodType { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
      [DataMember]
      public DateTime? ValidDate { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
      [DataMember]
      public DateTime? ExpiredDate { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
      [DataMember]
      public decimal? ContractAmt { get; set; }

        /// <summary>
        /// 修改请求状态
        /// </summary>
      [DataMember]
      public VendorModifyRequestStatus? Status { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
      [DataMember]
      public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
      [DataMember]
      public string CompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
      [DataMember]
      public string LanguageCode { get; set; }

        /// <summary>
        /// 货币系统编号
        /// </summary>
      [DataMember]
      public int? CurrencySysNo { get; set; }

        /// <summary>
        /// 供应商修改请求类型
        /// </summary>
      [DataMember]
      public VendorModifyRequestType? RequestType { get; set; }

        /// <summary>
        /// 代理级别
        /// </summary>
      [DataMember]
      public string AgentLevel { get; set; }

        /// <summary>
        /// 生产商系统编号
        /// </summary>
      [DataMember]
      public int? ManufacturerSysNo { get; set; }

        /// <summary>
        /// 2级分类系统编号
        /// </summary>
      [DataMember]
      public int? C2SysNo { get; set; }

        /// <summary>
        /// 3级分类系统编号
        /// </summary>
      [DataMember]
      public int? C3SysNo { get; set; }

        /// <summary>
        /// 供应商结算类型
        /// </summary>
      [DataMember]
      public SettleType? SettleType { get; set; }

        /// <summary>
        /// 供应商财务结算方式
        /// </summary>
      [DataMember]
      public VendorSettlePeriodType? SettlePeriodType { get; set; }

        /// <summary>
        /// 佣金百分比
        /// </summary>
      [DataMember]
      public decimal? SettlePercentage { get; set; }

        /// <summary>
        /// 送货周期
        /// </summary>
      [DataMember]
      public string SendPeriod { get; set; }

        /// <summary>
        /// 品牌系统编号
        /// </summary>
      [DataMember]
      public int? BrandSysNo { get; set; }

        /// <summary>
        /// 请求操作类型
        /// </summary>
      [DataMember]
      public VendorModifyActionType? ActionType { get; set; }

        /// <summary>
        /// 供应商代理系统编号
        /// </summary>
      [DataMember]
      public int? VendorManufacturerSysNo { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
      [DataMember]
      public string Content { get; set; }

        /// <summary>
        /// 备注（财务信息审核不通过理由）
        /// </summary>
      [DataMember]
      public string Memo { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
      [DataMember]
      public string BuyWeekDay { get; set; }

        /// <summary>
        /// 自动审核
        /// </summary>
      [DataMember]
      public bool? AutoAudit { get; set; }

    }
}
