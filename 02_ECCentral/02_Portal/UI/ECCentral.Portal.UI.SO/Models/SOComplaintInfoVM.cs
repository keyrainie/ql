using System;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.SO.Models
{
   public class SOComplaintInfoVM : ModelBase
   {
       public SOComplaintInfoVM()
       {
           ComplaintCotentInfo = new SOComplaintCotentInfoVM();
           ProcessInfo = new SOComplaintProcessInfoVM();
           ReplyHistory = new List<SOComplaintReplyInfoVM>();
       }

       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private SOComplaintCotentInfoVM m_ComplaintInfo;
       public SOComplaintCotentInfoVM ComplaintCotentInfo
       {
           get { return this.m_ComplaintInfo; }
           set { this.SetValue("ComplaintInfo", ref m_ComplaintInfo, value);  }
       }

       private SOComplaintProcessInfoVM m_ProcessInfo;
       public SOComplaintProcessInfoVM ProcessInfo
       {
           get { return this.m_ProcessInfo; }
           set { this.SetValue("ProcessInfo", ref m_ProcessInfo, value);  }
       }

       private List<SOComplaintReplyInfoVM> m_ReplyHistory;
       public List<SOComplaintReplyInfoVM> ReplyHistory
       {
           get { return this.m_ReplyHistory; }
           set { this.SetValue("ReplyHistory", ref m_ReplyHistory, value);  }
       }

   }
}
