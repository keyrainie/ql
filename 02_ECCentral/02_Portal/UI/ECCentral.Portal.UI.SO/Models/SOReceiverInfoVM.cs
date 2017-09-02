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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Resources;
namespace ECCentral.Portal.UI.SO.Models
{
   public class SOReceiverInfoVM : ModelBase
   {
       private Int32? m_sysNo;
       public Int32? SysNo
       {
           get { return this.m_sysNo; }
           set { this.SetValue("SysNo", ref m_sysNo, value); }
       }

       private Int32? m_CustomerSysNo;
       public Int32? CustomerSysNo
       {
           get { return this.m_CustomerSysNo; }
           set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
       }

       /// <summary>
       /// 收货地址标题
       /// </summary>
       private String m_AddressTitle;
       public String AddressTitle
       {
           get { return this.m_AddressTitle; }
           set { this.SetValue("ReceiveAddressBreif", ref m_AddressTitle, value); }
       }

       private Int32? m_ReceiveAreaSysNo;
       [Validate(ValidateType.Required)]
       public Int32? ReceiveAreaSysNo
       {
           get { return this.m_ReceiveAreaSysNo; }
           set { this.SetValue("ReceiveAreaSysNo", ref m_ReceiveAreaSysNo, value); }
       }

       private Int32? m_ReceiveCitySysNo;
       public Int32? ReceiveCitySysNo
       {
           get { return this.m_ReceiveCitySysNo; }
           set { this.SetValue("ReceiveCitySysNo", ref m_ReceiveCitySysNo, value); }
       }

       private String m_ReceiveName;
       public String ReceiveName
       {
           get { return this.m_ReceiveName; }
           set { this.SetValue("ReceiveName", ref m_ReceiveName, value); }
       }

       private String m_ReceiveContact;
       [Validate(ValidateType.Required)]
       public String ReceiveContact
       {
           get { return this.m_ReceiveContact; }
           set { this.SetValue("ReceiveContact", ref m_ReceiveContact, value);  }
       }

       /// <summary>
       /// 收货地址简称
       /// </summary>
       private String m_ReceiveAddressBreif;
       public String ReceiveAddressBreif
       {
           get { return this.m_ReceiveAddressBreif; }
           set { this.SetValue("ReceiveAddressBreif", ref m_ReceiveAddressBreif, value); }
       }

       private String m_ReceiveAddress;
       public String ReceiveAddress
       {
           get { return this.m_ReceiveAddress; }
           set { this.SetValue("ReceiveAddress", ref m_ReceiveAddress, value);  }
       }
        
       private String m_ReceiveCellPhone;
       [Validate(ValidateType.Interger)]
       [Validate(ValidateType.MaxLength)]
       public String ReceiveCellPhone
       {
           get { return this.m_ReceiveCellPhone; }
           set { this.SetValue("ReceiveCellPhone", ref m_ReceiveCellPhone, value);  }
       }

       private String m_ReceivePhone;
       public String ReceivePhone
       {
           get { return this.m_ReceivePhone; }
           set { this.SetValue("ReceivePhone", ref m_ReceivePhone, value);  }
       }

       private String m_ReceiveZip;
       [Validate(ValidateType.Required)]
       [Validate(ValidateType.Regex, RegexHelper.ZIP, ErrorMessageResourceName = "Msg_ZIP_Format", ErrorMessageResourceType = typeof(ResSO))]
       public String ReceiveZip
       {
           get { return this.m_ReceiveZip; }
           set { this.SetValue("ReceiveZip", ref m_ReceiveZip, value);  }
       }

        /// <summary>
        /// 收货人传真
        /// </summary>
       private String m_ReceiveFax;
       public String ReceiveFax
       {
           get { return this.m_ReceiveFax; }
           set { this.SetValue("ReceiveFax", ref m_ReceiveFax, value); }
       }

       /// <summary>
       /// 是否是默认的收货地址
       /// </summary>
       private Boolean? m_IsDefault;
       public Boolean? IsDefault
       {
           get { return this.m_IsDefault; }
           set { this.SetValue("IsDefault", ref m_IsDefault, value); }
       }

       /// <summary>
       /// 是否收要保存货地址
       /// </summary>
       private Boolean? m_IsSaveAddress;
       public Boolean? IsSaveAddress
       {
           get { return this.m_IsSaveAddress; }
           set { this.SetValue("IsSaveAddress", ref m_IsSaveAddress, value); }
       }

       /// <summary>
       /// 客户反馈的 收货日期 （定单创建积分补偿时用到）
       /// </summary>
       private DateTime? m_ReceiveDate;
       public DateTime? ReceiveDate
       {
           get { return this.m_ReceiveDate; }
           set { this.SetValue("ReceiveDate", ref m_ReceiveDate, value); }
       }
   }
}
