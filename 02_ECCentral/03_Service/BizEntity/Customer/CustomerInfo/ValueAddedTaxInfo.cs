using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 顾客增值税信息
    /// </summary>
    public class ValueAddedTaxInfo :IIdentity
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
        /// 顾客ID
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName
        {
            get;
            set;
        }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNum
        {
            get;
            set;
        }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string CompanyAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 公司电话
        /// </summary>
        public string CompanyPhone
        {
            get;
            set;
        }

        /// <summary>
        /// 开户行账号
        /// </summary>
        public string BankAccount
        {
            get;
            set;
        }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Memo
        {
            get;
            set;
        }

        /// <summary>
        /// 证书文件路径
        /// </summary>
        public string CertificateFileName
        {
            get;
            set;
        }

        /// <summary>
        /// 发证时间
        /// </summary>
        public DateTime? ReceivedCertificatesDate
        {
            get;
            set;
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastEditDate
        {
            get;
            set;
        }

        /// <summary>
        /// 是否默认增值税信息
        /// </summary>
        public bool? IsDefault
        {
            get;
            set;
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
 

      
    }
}
