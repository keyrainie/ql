using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.Models
{
   public class SOComplaintProcessInfoVM : ModelBase
   {
       private Int32? m_ComplainSysNo;
       public Int32? ComplainSysNo
       {
           get { return this.m_ComplainSysNo; }
           set { this.SetValue("ComplainSysNo", ref m_ComplainSysNo, value);  }
       }

       private SOComplainStatus m_Status;
       public SOComplainStatus Status
       {
           get { return this.m_Status; }
           set { this.SetValue("Status", ref m_Status, value);  }
       }

       private DateTime? m_ProcessTime;
       public DateTime? ProcessTime
       {
           get { return this.m_ProcessTime; }
           set { this.SetValue("ProcessTime", ref m_ProcessTime, value);  }
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

       private String m_ProductID;
       public String ProductID
       {
           get { return this.m_ProductID; }
           set { this.SetValue("ProductID", ref m_ProductID, value);  }
       }

       private Int32? m_DomainSysNo;
       public Int32? DomainSysNo
       {
           get { return this.m_DomainSysNo; }
           set { this.SetValue("DomainSysNo", ref m_DomainSysNo, value);  }
       }

       private Int32? m_ReasonCodeSysNo;
       public Int32? ReasonCodeSysNo
       {
           get { return this.m_ReasonCodeSysNo; }
           set { this.SetValue("ReasonCodeSysNo", ref m_ReasonCodeSysNo, value); }
       }

       private String m_ComplainNote;
       public String ComplainNote
       {
           get { return this.m_ComplainNote; }
           set { this.SetValue("ComplainNote", ref m_ComplainNote, value);  }
       }

       private string m_CSConfirmComplainType;
       public string CSConfirmComplainType
       {
           get { return this.m_CSConfirmComplainType; }
           set { this.SetValue("CSConfirmComplainType", ref m_CSConfirmComplainType, value);  }
       }

       private String m_CSConfirmComplainTypeDetail;
       public String CSConfirmComplainTypeDetail
       {
           get { return this.m_CSConfirmComplainTypeDetail; }
           set { this.SetValue("CSConfirmComplainTypeDetail", ref m_CSConfirmComplainTypeDetail, value);  }
       }

       private String m_ResponsibleDepartment;
       public String ResponsibleDepartment
       {
           get { return this.m_ResponsibleDepartment; }
           set { this.SetValue("ResponsibleDepartment", ref m_ResponsibleDepartment, value);  }
       }

       private String m_ResponsibleUser;
       public String ResponsibleUser
       {
           get { return this.m_ResponsibleUser; }
           set { this.SetValue("ResponsibleUser", ref m_ResponsibleUser, value);  }
       }

       private Boolean? m_IsSure;
       public Boolean? IsSure
       {
           get { return this.m_IsSure; }
           set { this.SetValue("IsSure", ref m_IsSure, value);  }
       }

       private SOComplainReplyType m_ReplyType;
       public SOComplainReplyType ReplyType
       {
           get { return this.m_ReplyType; }
           set { this.SetValue("ReplyType", ref m_ReplyType, value);  }
       }

       private String m_ReplyContent;
       public String ReplyContent
       {
           get { return this.m_ReplyContent; }
           set { this.SetValue("ReplyContent", ref m_ReplyContent, value);  }
       }

       private String m_ProcessedNote;
       public String ProcessedNote
       {
           get { return this.m_ProcessedNote; }
           set { this.SetValue("ProcessedNote", ref m_ProcessedNote, value);  }
       }

       private Int32 m_SpendHours;
       public Int32 SpendHours
       {
           get { return this.m_SpendHours; }
           set { this.SetValue("SpendHours", ref m_SpendHours, value);  }
       }

       private Int32 m_ReopenCount;
       public Int32 ReopenCount
       {
           get { return this.m_ReopenCount; }
           set { this.SetValue("ReopenCount", ref m_ReopenCount, value); }
       }

   }
}
