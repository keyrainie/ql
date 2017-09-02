using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商财务信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorFinanceInfo
    {
        /// <summary>
        /// 开户行
        /// </summary>
       [DataMember]
        public string BankName { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
       [DataMember]
       public string TaxNumber { get; set; }

        /// <summary>
        /// 财务联系人
        /// </summary>
       [DataMember]
       public string AccountContact { get; set; }

        /// <summary>
        /// 财务联系电话
        /// </summary>
       [DataMember]
       public string AccountPhone { get; set; }

        /// <summary>
        /// 财务联系人电子邮箱
        /// </summary>
       [DataMember]
       public string AccountContactEmail { get; set; }


        /// <summary>
        /// 账号
        /// </summary>
       [DataMember]
       public string AccountNumber { get; set; }

        /// <summary>
        /// 账期
        /// </summary>
       [DataMember]
       public int? PayPeriod { get; set; }

        /// <summary>
        /// 账期类型
        /// </summary>
       [DataMember]
       public VendorPayTermsItemInfo PayPeriodType { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
       [DataMember]
       public VendorSettlePeriodType? SettlePeriodType { get; set; }

        /// <summary>
        /// 是否自动审核
        /// </summary>
       [DataMember]
       public bool? IsAutoAudit { get; set; }

        /// <summary>
        /// 是否代销
        /// </summary>
       [DataMember]
       public VendorConsignFlag? ConsignFlag { get; set; }

        /// <summary>
        /// 合作生效日期
        /// </summary>
       [DataMember]
       public DateTime? CooperateValidDate { get; set; }

        /// <summary>
        /// 合作过期日期
        /// </summary>
       [DataMember]
       public DateTime? CooperateExpiredDate { get; set; }

        /// <summary>
        /// 合作金额
        /// </summary>
       [DataMember]
       public decimal? CooperateAmt { get; set; }

        /// <summary>
        /// 累计金额
        /// </summary>
       [DataMember]
       public decimal? TotalPOAmt { get; set; }

        /// <summary>
        /// 财务信息审核状态
        /// </summary>
       [DataMember]
       public VendorModifyRequestStatus? VerifyStatus { get; set; }

        /// <summary>
        /// 财务申请信息
        /// </summary>
       [DataMember]
       public VendorModifyRequestInfo FinanceRequestInfo { get; set; }

       /// <summary>
       /// 付款结算公司
       /// </summary>
       [DataMember]
       public int PaySettleCompany { get; set; }

    }
}
