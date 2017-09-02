using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSCustomerConditionViewModel : ModelBase
   {
       private RelCustomerRankViewModel m_RelCustomerRanks;
       public RelCustomerRankViewModel RelCustomerRanks
       {
           get { return this.m_RelCustomerRanks; }
           set { this.SetValue("RelCustomerRanks", ref m_RelCustomerRanks, value);  }
       }

       private RelCustomerViewModel m_RelCustomers;
       public RelCustomerViewModel RelCustomers
       {
           get { return this.m_RelCustomers; }
           set { this.SetValue("RelCustomers", ref m_RelCustomers, value);  }
       }

       private RelAreaViewModel m_RelAreas;
       public RelAreaViewModel RelAreas
       {
           get { return this.m_RelAreas; }
           set { this.SetValue("RelAreas", ref m_RelAreas, value);  }
       }

       private Boolean? m_NeedMobileVerification = false;
       public Boolean? NeedMobileVerification
       {
           get { return this.m_NeedMobileVerification; }
           set { this.SetValue("NeedMobileVerification", ref m_NeedMobileVerification, value);  }
       }

       private Boolean? m_NeedEmailVerification = false;
       public Boolean? NeedEmailVerification
       {
           get { return this.m_NeedEmailVerification; }
           set { this.SetValue("NeedEmailVerification", ref m_NeedEmailVerification, value);  }
       }

       private Boolean? m_InvalidForAmbassador = false;
       public Boolean? InvalidForAmbassador
       {
           get { return this.m_InvalidForAmbassador; }
           set { this.SetValue("InvalidForAmbassador", ref m_InvalidForAmbassador, value);  }
       }

   }


   public class RelAreaViewModel : ModelBase
   {
       private bool? m_IsIncludeRelation;
       public bool? IsIncludeRelation
       {
           get { return this.m_IsIncludeRelation; }
           set { this.SetValue("IsIncludeRelation", ref m_IsIncludeRelation, value); }
       }

       private bool? m_IsExcludeRelation;
       public bool? IsExcludeRelation
       {
           get { return this.m_IsExcludeRelation; }
           set { this.SetValue("IsExcludeRelation", ref m_IsExcludeRelation, value); }
       }

       private List<SimpleObjectViewModel> m_AreaList;
       public List<SimpleObjectViewModel> AreaList
       {
           get { return this.m_AreaList; }
           set { this.SetValue("AreaList", ref m_AreaList, value); }
       }

   }

   public class RelCustomerRankViewModel : ModelBase
   {
       private bool? m_IsIncludeRelation;
       public bool? IsIncludeRelation
       {
           get { return this.m_IsIncludeRelation; }
           set { this.SetValue("IsIncludeRelation", ref m_IsIncludeRelation, value); }
       }


       private List<SimpleObjectViewModel> m_CustomerRankList;
       public List<SimpleObjectViewModel> CustomerRankList
       {
           get { return this.m_CustomerRankList; }
           set { this.SetValue("CustomerRankList", ref m_CustomerRankList, value); }
       }

   }

   public class RelCustomerViewModel : ModelBase
   {
       private bool? m_IsIncludeRelation;
       public bool? IsIncludeRelation
       {
           get { return this.m_IsIncludeRelation; }
           set { this.SetValue("IsIncludeRelation", ref m_IsIncludeRelation, value); }
       }

       private bool? m_IsExcludeRelation;
       public bool? IsExcludeRelation
       {
           get { return this.m_IsExcludeRelation; }
           set { this.SetValue("IsExcludeRelation", ref m_IsExcludeRelation, value); }
       }

       private List<CustomerAndSendViewModel> m_CustomerIDList;
       public List<CustomerAndSendViewModel> CustomerIDList
       {
           get { return this.m_CustomerIDList; }
           set { this.SetValue("CustomerIDList", ref m_CustomerIDList, value); }
       }

   }

   public class CustomerAndSendViewModel : ModelBase
   {
       private String m_SendStatus;
       public String SendStatus
       {
           get { return this.m_SendStatus; }
           set { this.SetValue("SendStatus", ref m_SendStatus, value); }
       }

       private Int32? m_CustomerSysNo;
       public Int32? CustomerSysNo
       {
           get { return this.m_CustomerSysNo; }
           set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
       }

       private String m_CustomerID;
       public String CustomerID
       {
           get { return this.m_CustomerID; }
           set { this.SetValue("CustomerID", ref m_CustomerID, value); }
       }

       private String m_CustomerName;
       public String CustomerName
       {
           get { return this.m_CustomerName; }
           set { this.SetValue("CustomerName", ref m_CustomerName, value); }
       }

       private bool? isChecked;
       public bool? IsChecked
       {
           get { return isChecked; }
           set { base.SetValue("IsChecked", ref isChecked, value); }
       }
   }
}
