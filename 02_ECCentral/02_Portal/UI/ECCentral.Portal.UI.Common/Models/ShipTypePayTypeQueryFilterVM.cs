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

namespace ECCentral.Portal.UI.Common.Models
{
   public class ShipTypePayTypeQueryFilterVM : ModelBase
   {
       public ShipTypePayTypeQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
        }

        public PagingInfo PagingInfo { get; set; }

       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private Int32? m_ShipTypeSysNo;
       public Int32? ShipTypeSysNo
       {
           get { return this.m_ShipTypeSysNo; }
           set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value);  }
       }

       private Int32? m_PayTypeSysNo;
       public Int32? PayTypeSysNo
       {
           get { return this.m_PayTypeSysNo; }
           set { this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value);  }
       }

   }
}
