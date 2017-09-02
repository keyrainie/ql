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
using System.Collections.Generic;
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOVATInvoiceInfoVM : ModelBase
   {
       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private Int32? m_SOSysNo;
       public Int32? SOSysNo
       {
           get { return this.m_SOSysNo; }
           set { this.SetValue("SOSysNo", ref m_SOSysNo, value);  }
       }

       private Int32? m_CustomerSysNo;
       public Int32? CustomerSysNo
       {
           get { return this.m_CustomerSysNo; }
           set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value);  }
       }

       private String m_CompanyName;
       public String CompanyName
       {
           get { return this.m_CompanyName; }
           set { this.SetValue("CompanyName", ref m_CompanyName, value);  }
       }

       private String m_TaxNumber;
       public String TaxNumber
       {
           get { return this.m_TaxNumber; }
           set { this.SetValue("TaxNumber", ref m_TaxNumber, value);  }
       }

       private String m_CompanyAddress;
       public String CompanyAddress
       {
           get { return this.m_CompanyAddress; }
           set { this.SetValue("CompanyAddress", ref m_CompanyAddress, value);  }
       }

       private String m_BankName;
       public String BankName
       {
           get { return this.m_BankName; }
           set { this.SetValue("BankName", ref m_BankName, value);  }
       }

       private String m_BankAccount;
       public String BankAccount
       {
           get { return this.m_BankAccount; }
           set { this.SetValue("BankAccount", ref m_BankAccount, value);  }
       }

       private String m_CompanyPhone;
       public String CompanyPhone
       {
           get { return this.m_CompanyPhone; }
           set { this.SetValue("CompanyPhone", ref m_CompanyPhone, value);  }
       }

       private String m_Memo;
       public String Memo
       {
           get { return this.m_Memo; }
           set { this.SetValue("Memo", ref m_Memo, value);  }
       }

       private Boolean? m_ExistTaxpayerCertificate;
       public Boolean? ExistTaxpayerCertificate
       {
           get { return this.m_ExistTaxpayerCertificate; }
           set { this.SetValue("ExistTaxpayerCertificate", ref m_ExistTaxpayerCertificate, value);  }
       }

   }
}
