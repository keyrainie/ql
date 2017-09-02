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
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOInvoiceInfoVM : ModelBase
   {

       public SOInvoiceInfoVM()
       {
            VATInvoiceInfoVM = new SOVATInvoiceInfoVM();
       }

       private Int32? m_SOSysNo;
       public Int32? SOSysNo
       {
           get { return this.m_SOSysNo; }
           set { this.SetValue("SOSysNo", ref m_SOSysNo, value);  }
       }

       private String m_Header;
       public String Header
       {
           get { return this.m_Header; }
           set { this.SetValue("Header", ref m_Header, value);  }
       }

       private Boolean? m_IsVAT;
       public Boolean? IsVAT
       {
           get { return this.m_IsVAT; }
           set { this.SetValue("IsVAT", ref m_IsVAT, value);  }
       }

       private String m_InvoiceNote;
       public String InvoiceNote
       {
           get { return this.m_InvoiceNote; }
           set { this.SetValue("InvoiceNote", ref m_InvoiceNote, value);  }
       }

       private String m_InvoiceNo;
       public String InvoiceNo
       {
           get { return this.m_InvoiceNo; }
           set { this.SetValue("InvoiceNo", ref m_InvoiceNo, value);  }
       }

       private String m_FinanceNote;
       public String FinanceNote
       {
           get { return this.m_FinanceNote; }
           set { this.SetValue("FinanceNote", ref m_FinanceNote, value);  }
       }

       private Boolean? m_IsPrintPackageCover;
       public Boolean? IsPrintPackageCover
       {
           get { return this.m_IsPrintPackageCover; }
           set { this.SetValue("IsPrintPackageCover", ref m_IsPrintPackageCover, value);  }
       }

       private SOVATInvoiceInfoVM m_VATInvoiceInfoVM;
       public SOVATInvoiceInfoVM VATInvoiceInfoVM
       {
           get { return this.m_VATInvoiceInfoVM; }
           set { this.SetValue("VATInvoiceInfoVM", ref m_VATInvoiceInfoVM, value); }
       }

       private InvoiceType? m_InvoiceType;
       public InvoiceType? InvoiceType
       {
           get { return this.m_InvoiceType; }
           set { this.SetValue("InvoiceType", ref m_InvoiceType, value);  }
       }


       private Int32? m_SplitUserSysNo;
       public Int32? SplitUserSysNo
       {
           get { return this.m_SplitUserSysNo; }
           set { this.SetValue("SplitUserSysNo", ref m_SplitUserSysNo, value); }
       }

       private DateTime? m_SplitDateTime;
       public DateTime? SplitDateTime
       {
           get { return this.m_SplitDateTime; }
           set { this.SetValue("SplitDateTime", ref m_SplitDateTime, value); }
       }

       private Int32? m_IsMultiInvoice;
       public Int32? IsMultiInvoice
       {
           get { return this.m_IsMultiInvoice; }
           set { this.SetValue("IsMultiInvoice", ref m_IsMultiInvoice, value);  }
       }

       private Boolean? m_IsVATPrinted;
       public Boolean? IsVATPrinted
       {
           get { return this.m_IsVATPrinted; }
           set { this.SetValue("IsVATPrinted", ref m_IsVATPrinted, value);  }
       }

       private Boolean? m_IsRequireShipInvoice;
       public Boolean? IsRequireShipInvoice
       {
           get { return this.m_IsRequireShipInvoice; }
           set { this.SetValue("IsRequireShipInvoice", ref m_IsRequireShipInvoice, value);  }
       }

   }
}
