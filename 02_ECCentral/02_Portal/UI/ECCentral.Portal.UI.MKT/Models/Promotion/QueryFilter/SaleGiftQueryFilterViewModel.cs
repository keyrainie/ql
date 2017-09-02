using System;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class SaleGiftQueryFilterViewModel : ModelBase
   {
       private PagingInfo m_PageInfo;
       public PagingInfo PageInfo
       {
           get { return this.m_PageInfo; }
           set { this.SetValue("PageInfo", ref m_PageInfo, value); }
       }

       private string m_MasterProductSysNo;
       public string MasterProductSysNo
       {
           get { return this.m_MasterProductSysNo; }
           set { this.SetValue("MasterProductSysNo", ref m_MasterProductSysNo, value);  }
       }

       private string m_GiftProductSysNo;
       public string GiftProductSysNo
       {
           get { return this.m_GiftProductSysNo; }
           set { this.SetValue("GiftProductSysNo", ref m_GiftProductSysNo, value);  }
       }

       private string m_Category3SysNo;
       public string Category3SysNo
       {
           get { return this.m_Category3SysNo; }
           set { this.SetValue("Category3SysNo", ref m_Category3SysNo, value);  }
       }

       
       private string m_BrandSysNo;
       public string BrandSysNo
       {
           get { return this.m_BrandSysNo; }
           set { this.SetValue("BrandSysNo", ref m_BrandSysNo, value);  }
       }

       private string m_PMUser;
       public string PMUser
       {
           get { return this.m_PMUser; }
           set { this.SetValue("PMUser", ref m_PMUser, value); }
       }

       private string m_SysNo;
       [Validate(ValidateType.Regex, @"^[1-9]*[1-9][0-9]*$", ErrorMessage = "请输入大于0的整数!")]
       public string SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private SaleGiftStatus? m_Status;
       public SaleGiftStatus? Status
       {
           get { return this.m_Status; }
           set { this.SetValue("Status", ref m_Status, value);  }
       }

       private String m_PromotionName;
       public String PromotionName
       {
           get { return this.m_PromotionName; }
           set { this.SetValue("PromotionName", ref m_PromotionName, value);  }
       }

       private SaleGiftType? m_Type;
       public SaleGiftType? Type
       {
           get { return this.m_Type; }
           set { this.SetValue("Type", ref m_Type, value);  }
       }

       private String m_ChannelID;
       public String ChannelID
       {
           get { return this.m_ChannelID; }
           set { this.SetValue("ChannelID", ref m_ChannelID, value);  }
       }

       private DateTime? m_ActivityDateFrom;
       public DateTime? ActivityDateFrom
       {
           get { return this.m_ActivityDateFrom; }
           set { this.SetValue("ActivityDateFrom", ref m_ActivityDateFrom, value);  }
       }

       private DateTime? m_ActivityDateTo;
       public DateTime? ActivityDateTo
       {
           get { return this.m_ActivityDateTo; }
           set { this.SetValue("ActivityDateTo", ref m_ActivityDateTo, value);  }
       }
 
        
       private String m_CompanyCode;
       public string CompanyCode
       {
           get { return this.m_CompanyCode; }
           set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
       }

       private int m_VendorSysNo;
       public int VendorSysNo
       {
           get { return this.m_VendorSysNo; }
           set { this.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
       }

       private string m_Category2SysNo;
       public string Category2SysNo
       {
           get { return this.m_Category2SysNo; }
           set { this.SetValue("Category2SysNo", ref m_Category2SysNo, value); }
       }
       private string m_Category1SysNo;
       public string Category1SysNo
       {
           get { return this.m_Category1SysNo; }
           set { this.SetValue("Category1SysNo", ref m_Category1SysNo, value); }
       }

       public bool HasGiftApprovePermission
       {
           get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Gift_Approve); }
       }
   }

   public class SaleGiftLogQueryFilterViewModel : ModelBase
   {
       private PagingInfo m_PageInfo;
       public PagingInfo PageInfo
       {
           get { return this.m_PageInfo; }
           set { this.SetValue("PageInfo", ref m_PageInfo, value); }
       }

       public int? ProductSysNo
       {
           get;
           set;
       }
   
   }
}
