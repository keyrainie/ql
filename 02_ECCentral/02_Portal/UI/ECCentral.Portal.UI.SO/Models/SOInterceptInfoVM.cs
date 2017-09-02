using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOInterceptInfoVM : ModelBase
   {
       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private String m_Sysnolist;
       public String Sysnolist
       {
           get { return this.m_Sysnolist; }
           set { this.SetValue("Sysnolist", ref m_Sysnolist, value); }
       }

       private String m_CompanyCode;
       public String CompanyCode
       {
           get { return this.m_CompanyCode; }
           set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
       }

       private String m_WebChannelID;
       public String WebChannelID
       {
           get { return this.m_WebChannelID; }
           set { this.SetValue("WebChannelID", ref m_WebChannelID, value); }
       }

       private String m_ShipTypeCategory;
       public String ShipTypeCategory
       {
           get { return this.m_ShipTypeCategory; }
           set { this.SetValue("ShipTypeCategory", ref m_ShipTypeCategory, value); }
       }

       private String m_ShipTypeEnum;
       public String ShipTypeEnum
       {
           get { return this.m_ShipTypeEnum; }
           set { this.SetValue("ShipTypeEnum", ref m_ShipTypeEnum, value); }
       }

       private String m_StockSysNo;
       public String StockSysNo
       {
           get { return this.m_StockSysNo; }
           set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
       }
 
       private String m_StockName;
       public String StockName
       {
           get { return this.m_StockName; }
           set { this.SetValue("StockName", ref m_StockName, value); }
       }

       private Int32? m_ShipTypeSysNo;
       public Int32? ShipTypeSysNo
       {
           get { return this.m_ShipTypeSysNo; }
           set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value); }
       }


       private String m_ShippingTypeID;
       public String ShippingTypeID
       {
           get { return this.m_ShippingTypeID; }
           set { this.SetValue("ShippingTypeID", ref m_ShippingTypeID, value); }
       }

       private String m_ShippingTypeName;
       public String ShippingTypeName
       {
           get { return this.m_ShippingTypeName; }
           set { this.SetValue("ShippingTypeName", ref m_ShippingTypeName, value); }
       }

       private String m_HasTrackingNumber;
       public String HasTrackingNumber
       {
           get { return this.m_HasTrackingNumber; }
           set { this.SetValue("HasTrackingNumber", ref m_HasTrackingNumber, value); }
       }

       private String m_ShipTimeType;
       public String ShipTimeType
       {
           get { return this.m_ShipTimeType; }
           set { this.SetValue("ShipTimeType", ref m_ShipTimeType, value); }

       }

       private String m_ContactName;
       public String ContactName
       {
           get { return this.m_ContactName; }
           set { this.SetValue("ContactName", ref m_ContactName, value); }
       }

       private String m_EmailAddress;
       [Validate(ValidateType.Email)]  
       public String EmailAddress
       {
           get { return this.m_EmailAddress; }
           set { this.SetValue("EmailAddress", ref m_EmailAddress, value); }
       }

       private String m_CCEmailAddress;
       public String CCEmailAddress
       {
           get { return this.m_CCEmailAddress; }
           set { this.SetValue("CCEmailAddress", ref m_CCEmailAddress, value); }
       }

       private String m_FinanceEmailAddress;
       [Validate(ValidateType.Email)]  
       public String FinanceEmailAddress
       {
           get { return this.m_FinanceEmailAddress; }
           set { this.SetValue("FinanceEmailAddress", ref m_FinanceEmailAddress, value); }
       }

       private String m_FinanceCCEmailAddress;
       public String FinanceCCEmailAddress
       {
           get { return this.m_FinanceCCEmailAddress; }
           set { this.SetValue("FinanceCCEmailAddress", ref m_FinanceCCEmailAddress, value); }
       }      
   }
}
