using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
   public class CommissionRuleVM : ModelBase
   {
       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private Int32 m_BrandSysNo;
       public Int32 BrandSysNo
       {
           get { return this.m_BrandSysNo; }
           set { this.SetValue("BrandSysNo", ref m_BrandSysNo, value);  }
       }

       private Int32 m_ManufacturerSysNo;
       public Int32 ManufacturerSysNo
       {
           get { return this.m_ManufacturerSysNo; }
           set { this.SetValue("ManufacturerSysNo", ref m_ManufacturerSysNo, value);  }
       }

       private Decimal m_CostValue;
       public Decimal CostValue
       {
           get { return this.m_CostValue; }
           set { this.SetValue("CostValue", ref m_CostValue, value);  }
       }

       private String m_RuleType;
       public String RuleType
       {
           get { return this.m_RuleType; }
           set { this.SetValue("RuleType", ref m_RuleType, value);  }
       }

       private String m_IsDefaultRule;
       public String IsDefaultRule
       {
           get { return this.m_IsDefaultRule; }
           set { this.SetValue("IsDefaultRule", ref m_IsDefaultRule, value);  }
       }

       private String m_InUser;
       public String InUser
       {
           get { return this.m_InUser; }
           set { this.SetValue("InUser", ref m_InUser, value);  }
       }

       private String m_EditUser;
       public String EditUser
       {
           get { return this.m_EditUser; }
           set { this.SetValue("EditUser", ref m_EditUser, value);  }
       }

       private String m_CurrencyCode;
       public String CurrencyCode
       {
           get { return this.m_CurrencyCode; }
           set { this.SetValue("CurrencyCode", ref m_CurrencyCode, value);  }
       }

       private String m_ManufacturerName;
       public String ManufacturerName
       {
           get { return this.m_ManufacturerName; }
           set { this.SetValue("ManufacturerName", ref m_ManufacturerName, value);  }
       }

       private String m_BrandNameCH;
       public String BrandNameCH
       {
           get { return this.m_BrandNameCH; }
           set { this.SetValue("BrandNameCH", ref m_BrandNameCH, value);  }
       }

       private String m_BrandNameEN;
       public String BrandNameEN
       {
           get { return this.m_BrandNameEN; }
           set { this.SetValue("BrandNameEN", ref m_BrandNameEN, value);  }
       }

       private Int32? m_C1SysNo;
       public Int32? C1SysNo
       {
           get { return this.m_C1SysNo; }
           set { this.SetValue("C1SysNo", ref m_C1SysNo, value);  }
       }

       private String m_C1Name;
       public String C1Name
       {
           get { return this.m_C1Name; }
           set { this.SetValue("C1Name", ref m_C1Name, value);  }
       }

       private Int32? m_C2SysNo;
       public Int32? C2SysNo
       {
           get { return this.m_C2SysNo; }
           set { this.SetValue("C2SysNo", ref m_C2SysNo, value);  }
       }

       private String m_C2Name;
       public String C2Name
       {
           get { return this.m_C2Name; }
           set { this.SetValue("C2Name", ref m_C2Name, value);  }
       }

       private Int32? m_C3SysNo;
       public Int32? C3SysNo
       {
           get { return this.m_C3SysNo; }
           set { this.SetValue("C3SysNo", ref m_C3SysNo, value);  }
       }

       private String m_C3Name;
       public String C3Name
       {
           get { return this.m_C3Name; }
           set { this.SetValue("C3Name", ref m_C3Name, value);  }
       }

       private String m_Level;
       public String Level
       {
           get { return this.m_Level; }
           set { this.SetValue("Level", ref m_Level, value);  }
       }

   }
}
