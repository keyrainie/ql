using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECCentral.BizEntity.PO.Vendor;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorInfo : IIdentity, ICompany
    {

        public VendorInfo()
        {
            VendorAgentInfoList = new List<VendorAgentInfo>();
            VendorAttachInfo = new VendorAttachInfo();
            VendorBasicInfo = new VendorBasicInfo() { ExtendedInfo = new VendorExtendInfo() };
            VendorFinanceInfo = new VendorFinanceInfo() { PayPeriodType = new VendorPayTermsItemInfo(), FinanceRequestInfo = new VendorModifyRequestInfo() };
            VendorCustomsInfo = new VendorCustomsInfo();
            VendorHistoryLog = new List<VendorHistoryLog>();
            VendorServiceInfo = new VendorServiceInfo();
            VendorDeductInfo = new VendorDeductInfo();
            VendorStoreInfoList = new List<VendorStoreInfo>();
            VendorAttachmentForApplyFor = new List<AttachmentForApplyFor>();
            ApplyInfo = new List<ApplyInfo>();
        }

        /// <summary>
        /// 供应商系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 创建人系统编号
        /// </summary>
        [DataMember]
        public int? CreateUserSysNo { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        [DataMember]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 供应商基本信息
        /// </summary>
        [DataMember]
        public VendorBasicInfo VendorBasicInfo { get; set; }

        /// <summary>
        /// 供应商售后服务信息
        /// </summary>
        [DataMember]
        public VendorServiceInfo VendorServiceInfo { get; set; }

        /// <summary>
        /// 供应商财务信息
        /// </summary>
        [DataMember]
        public VendorFinanceInfo VendorFinanceInfo { get; set; }

        /// <summary>
        /// 关务对接相关信息
        /// </summary>
        [DataMember]
        public VendorCustomsInfo VendorCustomsInfo { get; set; }
        /// <summary>
        /// 供应商附件信息
        /// </summary>
        [DataMember]
        public VendorAttachInfo VendorAttachInfo { get; set; }

        /// <summary>
        /// 供应商历史信息
        /// </summary>
        [DataMember]
        public List<VendorHistoryLog> VendorHistoryLog { get; set; }

        /// <summary>
        /// 供应商代理信息
        /// </summary>
        [DataMember]
        public List<VendorAgentInfo> VendorAgentInfoList { get; set; }

        [DataMember]
        public List<VendorStoreInfo> VendorStoreInfoList { get; set; }

        [DataMember]
        public VendorDeductInfo VendorDeductInfo { get; set; }

        [DataMember]
        public List<AttachmentForApplyFor> VendorAttachmentForApplyFor { get; set; }

        [DataMember]
        public List<ApplyInfo> ApplyInfo { get; set; }
    }
}
