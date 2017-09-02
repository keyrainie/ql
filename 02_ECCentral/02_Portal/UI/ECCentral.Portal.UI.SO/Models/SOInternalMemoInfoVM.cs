using System;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.Models
{
   public class SOInternalMemoInfoVM : ModelBase
   {
       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private Int32 m_SOSysNo;
       public Int32 SOSysNo
       {
           get { return this.m_SOSysNo; }
           set { this.SetValue("SOSysNo", ref m_SOSysNo, value);  }
       }

       private Int32 m_ReasonCodeSysNo;
       public Int32 ReasonCodeSysNo
       {
           get { return this.m_ReasonCodeSysNo; }
           set { this.SetValue("ReasonCodeSysNo", ref m_ReasonCodeSysNo, value);  }
       }

       private Int32 m_SourceSysNo;
       public Int32 SourceSysNo
       {
           get { return this.m_SourceSysNo; }
           set { this.SetValue("SourceSysNo", ref m_SourceSysNo, value);  }
       }

       private String m_Content;
       [Validate(ValidateType.Required)]
       public String Content
       {
           get { return this.m_Content; }
           set { this.SetValue("Content", ref m_Content, value);  }
       }

       private SOInternalMemoStatus m_Status;
       public SOInternalMemoStatus Status
       {
           get { return this.m_Status; }
           set { this.SetValue("Status", ref m_Status, value);  }
       }

       private DateTime? m_RemindTime;
       public DateTime? RemindTime
       {
           get { return this.m_RemindTime; }
           set { this.SetValue("RemindTime", ref m_RemindTime, value);  }
       }

       private String m_Note;
       [Validate(ValidateType.Required)]
       public String Note
       {
           get { return this.m_Note; }
           set { this.SetValue("Note", ref m_Note, value);  }
       }

       private Int32? m_CallType;
       public Int32? CallType
       {
           get { return this.m_CallType; }
           set { this.SetValue("CallType", ref m_CallType, value);  }
       }

       private Int32? m_Importance;
       public Int32? Importance
       {
           get { return this.m_Importance; }
           set { this.SetValue("Importance", ref m_Importance, value);  }
       }

       private String m_DepartmentCode;
       public String DepartmentCode
       {
           get { return this.m_DepartmentCode; }
           set { this.SetValue("DepartmentCode", ref m_DepartmentCode, value);  }
       }

       private DateTime? m_LogTime;
       public DateTime? LogTime
       {
           get { return this.m_LogTime; }
           set { this.SetValue("LogTime", ref m_LogTime, value);  }
       }

       private Int32? m_AssignerSysNo;
       public Int32? AssignerSysNo
       {
           get { return this.m_AssignerSysNo; }
           set { this.SetValue("AssignerSysNo", ref m_AssignerSysNo, value);  }
       }

       private DateTime? m_AssignDate;
       public DateTime? AssignDate
       {
           get { return this.m_AssignDate; }
           set { this.SetValue("AssignDate", ref m_AssignDate, value);  }
       }

       private Int32? m_OperatorSysNo;
       public Int32? OperatorSysNo
       {
           get { return this.m_OperatorSysNo; }
           set { this.SetValue("OperatorSysNo", ref m_OperatorSysNo, value);  }
       }

   }
}
