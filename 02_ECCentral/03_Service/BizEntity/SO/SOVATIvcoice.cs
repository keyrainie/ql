using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 增值税发票信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOVATInvoiceInfo 
    {
        /// <summary>
        /// 增值税发票编号
        /// </summary>
        [DataMember]
        public int SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户系统编号
        /// </summary>
        [DataMember]
        public int CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户公司名称 
        /// </summary>
        [DataMember]
        public string CompanyName
        {
            get;
            set;
        }
        /// <summary>
        /// 公司税号
        /// </summary>
        [DataMember]
        public string TaxNumber //TaxNum
        {
            get;
            set;
        }
        /// <summary>
        /// 公司地址
        /// </summary>
        [DataMember]
        public string CompanyAddress
        {
            get;
            set;
        }
        /// <summary>
        /// 公司银行名称
        /// </summary>
        [DataMember]
        public string BankName
        {
            get;
            set;
        }
        /// <summary>
        /// 公司银行账号
        /// </summary>
        [DataMember]
        public string BankAccount
        {
            get;
            set;
        }
        /// <summary>
        /// 公司电话
        /// </summary>
        [DataMember]
        public string CompanyPhone
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo
        {
            get;
            set;
        }
        /// <summary>
        /// 是否有一般纳税人资格证书
        /// </summary>
        [DataMember]
        public bool ExistTaxpayerCertificate
        {
            get;
            set;
        }
    }
}
