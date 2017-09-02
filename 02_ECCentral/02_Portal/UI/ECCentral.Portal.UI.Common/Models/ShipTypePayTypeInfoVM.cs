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
   public class ShipTypePayTypeInfoVM : ModelBase
   {
       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private string m_PayTypeSysNo;
       [Validate(ValidateType.Required)]
       public string PayTypeSysNo
       {
           get { return this.m_PayTypeSysNo; }
           set { this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value);  }
       }

       private string m_ShipTypeSysNo;
       [Validate(ValidateType.Required)]
       public string ShipTypeSysNo
       {
           get { return this.m_ShipTypeSysNo; }
           set { this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value);  }
       }

       private String m_PayTypeName;
       public String PayTypeName
       {
           get { return this.m_PayTypeName; }
           set { this.SetValue("PayTypeName", ref m_PayTypeName, value);  }
       }

       private String m_ShipTypeName;
       public String ShipTypeName
       {
           get { return this.m_ShipTypeName; }
           set { this.SetValue("ShipTypeName", ref m_ShipTypeName, value);  }
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
