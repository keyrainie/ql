using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class EIMSInvoiceEntryInfo : ICompany, IIdentity
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public string AssignedCode { get; set; }

        /// <summary>
        /// IPP#
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// 单据名称
        /// </summary>
        public string InvoiceName { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string RuleAssignedCode { get; set; }

        /// <summary>
        /// 供应商#
        /// </summary>
        public int VendorNumber { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 收款类型
        /// </summary>
        public ReceiveType ReceiveType { get; set; }

        /// <summary>
        /// PM
        /// </summary>
        public string PM { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public EIMSType EIMSType { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal InvoiceAmount { get; set; }

        /// <summary>
        /// 单据状态        
        /// </summary>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 审批通过时间
        /// </summary>
        public DateTime? ApproveDate { get; set; }

        /// <summary>
        /// 发票录入状态
        /// 已录入、未录入
        /// </summary>
        public string InvoiceInputStatue { get; set; }

        /// <summary>
        /// 上传SAP状态      
        /// </summary>
        public string IsSAPImported { get; set; }

        /// <summary>
        /// 录入的发票号
        /// 多个发票号以","隔开
        /// </summary>
        public string InvoiceInputSysNo
        {
            get;
            set;
        }

        #region IIdentity Members

        public int? SysNo
        {
            get;
            set;
        }

        #endregion

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        #endregion
    }
    
}
