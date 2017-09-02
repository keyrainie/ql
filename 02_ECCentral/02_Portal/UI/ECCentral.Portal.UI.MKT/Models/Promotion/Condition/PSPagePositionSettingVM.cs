using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSPagePositionSettingViewModel : ModelBase
   {
       private Int32? m_PageTypeSysNo;
       public Int32? PageTypeSysNo
       {
           get { return this.m_PageTypeSysNo; }
           set { this.SetValue("PageTypeSysNo", ref m_PageTypeSysNo, value);  }
       }

       private Int32? m_PageTypeReferenceSysNo;
       public Int32? PageTypeReferenceSysNo
       {
           get { return this.m_PageTypeReferenceSysNo; }
           set { this.SetValue("PageTypeReferenceSysNo", ref m_PageTypeReferenceSysNo, value);  }
       }

       private Int32? m_PositionSysNo;
       public Int32? PositionSysNo
       {
           get { return this.m_PositionSysNo; }
           set { this.SetValue("PositionSysNo", ref m_PositionSysNo, value);  }
       }

   }
}
