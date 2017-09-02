using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.SO;
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOStatusChangeInfoVM : ModelBase
   {
       /// <summary>
       /// 订单编号
       /// </summary>    
       private Int32? m_SOSysNo;
       public Int32? SOSysNo
       {
           get { return this.m_SOSysNo; }
           set { this.SetValue("SOSysNo", ref m_SOSysNo, value);  }
       }

       /// <summary>
       /// 操作业类型
       /// </summary>
       private SOOperatorType m_OperatorType;
       public SOOperatorType OperatorType
       {
           get { return this.m_OperatorType; }
           set { this.SetValue("OperatorType", ref m_OperatorType, value);  }
       }

       /// <summary>
       /// 操作者编号
       /// </summary>
       private Int32? m_OperatorSysNo;
       public Int32? OperatorSysNo
       {
           get { return this.m_OperatorSysNo; }
           set { this.SetValue("OperatorSysNo", ref m_OperatorSysNo, value);  }
       }

       /// <summary>
       ///  订单原来状态
       /// </summary>
       private SOStatus? m_OldStatus;
       public SOStatus? OldStatus
       {
           get { return this.m_OldStatus; }
           set { this.SetValue("OldStatus", ref m_OldStatus, value); }
       }

       /// <summary>
       /// 订单当前状态，要更改到的状态
       /// </summary>
       private SOStatus? m_Status;
       public SOStatus? Status
       {
           get { return this.m_Status; }
           set { this.SetValue("Status", ref m_Status, value);  }
       }

       /// <summary>
       /// 状态更改时间
       /// </summary>
       private DateTime? m_ChangeTime;
       public DateTime? ChangeTime
       {
           get { return this.m_ChangeTime; }
           set { this.SetValue("ChangeTime", ref m_ChangeTime, value);  }
       }

       /// <summary>
       /// 是否发送邮件给客户
       /// </summary>
       private Boolean? m_IsSendMailToCustomer;
       public Boolean? IsSendMailToCustomer
       {
           get { return this.m_IsSendMailToCustomer; }
           set { this.SetValue("IsSendMailToCustomer", ref m_IsSendMailToCustomer, value);  }
       }

       /// <summary>
       /// 状态更改备注
       /// </summary>
       private String m_Note;
       public String Note
       {
           get { return this.m_Note; }
           set { this.SetValue("Note", ref m_Note, value);  }
       }
   }
}
