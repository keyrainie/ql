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
using ECCentral.BizEntity.SO;
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOClientInfoVM : ModelBase
   {
       private String m_CustomerIPAddress;
       public String CustomerIPAddress
       {
           get { return this.m_CustomerIPAddress; }
           set { this.SetValue("CustomerIPAddress", ref m_CustomerIPAddress, value);  }
       }

       private String m_CustomerCookie;
       public String CustomerCookie
       {
           get { return this.m_CustomerCookie; }
           set { this.SetValue("CustomerCookie", ref m_CustomerCookie, value);  }
       }

       private PhoneType m_PhoneType;
       public PhoneType PhoneType
       {
           get { return this.m_PhoneType; }
           set { this.SetValue("PhoneType", ref m_PhoneType, value);  }
       }

   }
}
