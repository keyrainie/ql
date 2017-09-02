using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Res = ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.Models
{
    /// <summary>
    /// 增值税信息视图模型
    /// </summary>
    public class ValueAddedTaxInfoVM : ModelBase
    {
        private int? m_SysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get
            {
                return m_SysNo;
            }
            set
            {
                base.SetValue("SysNo", ref m_SysNo, value);
            }
        }

        private string m_CustomerID;
        public string CustomerID
        {
            get
            {
                return m_CustomerID;
            }
            set
            {
                base.SetValue("CustomerID", ref m_CustomerID, value);
            }
        }

        private int? m_CustomerSysNo;
        public int? CustomerSysNo
        {
            get
            {
                return m_CustomerSysNo;
            }
            set
            {
                base.SetValue("CustomerSysNo", ref m_CustomerSysNo, value);
            }
        }

        public string m_CompanyName;
        /// <summary>
        /// 公司名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CompanyName
        {
            get
            {
                return m_CompanyName;
            }
            set
            {
                base.SetValue("CompanyName", ref m_CompanyName, value);
            }
        }

        public string m_TaxNum;
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string TaxNum
        {
            get
            {
                return m_TaxNum;
            }
            set
            {
                base.SetValue("TaxNum", ref m_TaxNum, value);
            }
        }

        private string m_CompanyAddress;
        /// <summary>
        /// 公司地址
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CompanyAddress
        {
            get
            {
                return m_CompanyAddress;
            }
            set
            {
                base.SetValue("CompanyAddress", ref m_CompanyAddress, value);
            }
        }

        private string m_CompanyPhone;
        /// <summary>
        /// 公司电话
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, (@"^((\(?0\d{2}\)?-?\d{8}))(-\d+)?$|^(\(?0\d{3}\)?-?\d{7})(-\d+)?$|^[0]?[1][3|5|8][0-9]{9}$"), ErrorMessageResourceName = "Valid_CompanyPhoneRegex", ErrorMessageResourceType = typeof(Res.ResValueAddedTaxInvoiceDetail))]
        public string CompanyPhone
        {
            get
            {
                return m_CompanyPhone;
            }
            set
            {
                base.SetValue("CompanyPhone", ref m_CompanyPhone, value);
            }
        }

        private string m_BankAccount;
        /// <summary>
        /// 开户行账号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string BankAccount
        {
            get
            {
                return m_BankAccount;
            }
            set
            {
                base.SetValue("BankAccount", ref m_BankAccount, value);
            }
        }

        private string m_Memo;
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Memo
        {
            get
            {
                return m_Memo;
            }
            set
            {
                base.SetValue("Memo", ref m_Memo, value);
            }
        }

        private string m_CertificateFileName;
        /// <summary>
        /// 证书文件路径
        /// </summary>
        public string CertificateFileName
        {
            get
            {
                return m_CertificateFileName;
            }
            set
            {
                base.SetValue("CertificateFileName", ref m_CertificateFileName, value);
            }
        }

        private DateTime? m_ReceivedCertificatesDate;
        /// <summary>
        /// 发证时间
        /// </summary>
        public DateTime? ReceivedCertificatesDate
        {
            get
            {
                return m_ReceivedCertificatesDate;
            }
            set
            {
                base.SetValue("ReceivedCertificatesDate", ref m_ReceivedCertificatesDate, value);
            }
        }

        private DateTime? m_LastEditDate;
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastEditDate
        {
            get
            {
                return m_LastEditDate;
            }
            set
            {
                base.SetValue("LastEditDate", ref m_LastEditDate, value);
            }
        }

        private bool? m_IsDefault;
        /// <summary>
        /// 是否默认增值税信息
        /// </summary>
        public bool? IsDefault
        {
            get
            {
                return m_IsDefault;
            }
            set
            {
                base.SetValue("IsDefault", ref m_IsDefault, value);
            }
        }
    }
}