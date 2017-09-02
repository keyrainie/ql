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
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Common.Models
{
   public class HolidayQueryFilterVM : ModelBase
   {
       public List<KeyValuePair<BlockedServiceType?, string>> ListBlockedServiceType { get; set; }
       public List<KeyValuePair<SYNStatus?, string>> ListIsUnitNow { get; set; }

       public HolidayQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
            ListBlockedServiceType = EnumConverter.GetKeyValuePairs<BlockedServiceType>(EnumConverter.EnumAppendItemType.All);
            ListBlockedServiceType.RemoveRange(5, ListBlockedServiceType.Count-5);

            this.ListIsUnitNow = EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.All);
        }

        public PagingInfo PagingInfo { get; set; }

       private String m_HolidayDate;
       public String HolidayDate
       {
           get { return this.m_HolidayDate; }
           set { this.SetValue("HolidayDate", ref m_HolidayDate, value);  }
       }

       private String m_BlockedService;
       public String BlockedService
       {
           get { return this.m_BlockedService; }
           set { this.SetValue("BlockedService", ref m_BlockedService, value);  }
       }

       private SYNStatus? m_IsUntilNow;
       public SYNStatus? IsUntilNow
       {
           get { return this.m_IsUntilNow; }
           set { this.SetValue("IsUntilNow", ref m_IsUntilNow, value);  }
       }

       private Int32? m_ShipTypeSysNo;
       public Int32? ShipTypeSysNo
       {
           get { return this.m_ShipTypeSysNo; }
           set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value);  }
       }

   }
}
