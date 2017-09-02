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
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOFPInfoVM : ModelBase
   {
       private Int32? m_SOSysNo;
       public Int32? SOSysNo
       {
           get { return this.m_SOSysNo; }
           set { this.SetValue("SOSysNo", ref m_SOSysNo, value);  }
       }

       private Int32? m_IsFPSO;
       public Int32? IsFPSO
       {
           get { return this.m_IsFPSO; }
           set { this.SetValue("IsFPSO", ref m_IsFPSO, value);  }
       }

       private String m_FPReason;
       public String FPReason
       {
           get { return this.m_FPReason; }
           set { this.SetValue("FPReason", ref m_FPReason, value);  }
       }

       private Boolean? m_IsFPCheck;
       public Boolean? IsFPCheck
       {
           get { return this.m_IsFPCheck; }
           set { this.SetValue("IsFPCheck", ref m_IsFPCheck, value);  }
       }

       private DateTime? m_FPCheckTime;
       public DateTime? FPCheckTime
       {
           get { return this.m_FPCheckTime; }
           set { this.SetValue("FPCheckTime", ref m_FPCheckTime, value);  }
       }

       private String m_FPExtend;
       public String FPExtend
       {
           get { return this.m_FPExtend; }
           set { this.SetValue("FPExtend", ref m_FPExtend, value);  }
       }

   }
}
