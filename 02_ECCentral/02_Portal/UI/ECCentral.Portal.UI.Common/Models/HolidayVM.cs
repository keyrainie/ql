using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Common.Models
{
   public class HolidayVM : ModelBase
   {
       public List<KeyValuePair<BlockedServiceType?, string>> ListBlockedServiceType { get; set; }
       public HolidayVM()
       {
           ListBlockedServiceType = EnumConverter.GetKeyValuePairs<BlockedServiceType>(EnumConverter.EnumAppendItemType.Select);
           ListBlockedServiceType.RemoveRange(5, ListBlockedServiceType.Count - 5);
       }

       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private Int32? m_CreateUserSysNo;
       public Int32? CreateUserSysNo
       {
           get { return this.m_CreateUserSysNo; }
           set { this.SetValue("CreateUserSysNo", ref m_CreateUserSysNo, value);  }
       }

       private DateTime? m_CreateDate;
       public DateTime? CreateDate
       {
           get { return this.m_CreateDate; }
           set { this.SetValue("CreateDate", ref m_CreateDate, value);  }
       }

       private String m_CreateUserName;
       public String CreateUserName
       {
           get { return this.m_CreateUserName; }
           set { this.SetValue("CreateUserName", ref m_CreateUserName, value);  }
       }

       private String m_ShipTypeName;
       public String ShipTypeName
       {
           get { return this.m_ShipTypeName; }
           set { this.SetValue("ShipTypeName", ref m_ShipTypeName, value);  }
       }

       private DateTime? m_HolidayDate;
       [Validate(ValidateType.Required)]
       public DateTime? HolidayDate
       {
           get { return this.m_HolidayDate; }
           set { this.SetValue("HolidayDate", ref m_HolidayDate, value);  }
       }

       private BlockedServiceType? m_BlockedService;
       [Validate(ValidateType.Required)]
       public BlockedServiceType? BlockedService
       {
           get { return this.m_BlockedService; }
           set { this.SetValue("BlockedService", ref m_BlockedService, value);  }
       }

       private string m_ShipTypeSysNo;
       [Validate(ValidateType.Required)]
       public string ShipTypeSysNo
       {
           get { return this.m_ShipTypeSysNo; }
           set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value);  }
       }

       private Int32 m_DeliveryType;
       public Int32 DeliveryType
       {
           get { return this.m_DeliveryType; }
           set { this.SetValue("DeliveryType", ref m_DeliveryType, value);  }
       }

       private String m_CompanyCode;
       public String CompanyCode
       {
           get { return this.m_CompanyCode; }
           set { this.SetValue("CompanyCode", ref m_CompanyCode, value);  }
       }

       private bool m_IsCheck;
       /// <summary>
       /// 用于界面批量操作的选择
       /// </summary>
       public bool IsChecked
       {
           get { return m_IsCheck; }
           set { SetValue("IsCheck", ref m_IsCheck, value); }
       }

   }
}
