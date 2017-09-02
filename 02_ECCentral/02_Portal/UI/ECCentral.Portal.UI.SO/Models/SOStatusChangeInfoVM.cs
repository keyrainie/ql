using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.SO;
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOStatusChangeInfoVM : ModelBase
   {
       /// <summary>
       /// �������
       /// </summary>    
       private Int32? m_SOSysNo;
       public Int32? SOSysNo
       {
           get { return this.m_SOSysNo; }
           set { this.SetValue("SOSysNo", ref m_SOSysNo, value);  }
       }

       /// <summary>
       /// ����ҵ����
       /// </summary>
       private SOOperatorType m_OperatorType;
       public SOOperatorType OperatorType
       {
           get { return this.m_OperatorType; }
           set { this.SetValue("OperatorType", ref m_OperatorType, value);  }
       }

       /// <summary>
       /// �����߱��
       /// </summary>
       private Int32? m_OperatorSysNo;
       public Int32? OperatorSysNo
       {
           get { return this.m_OperatorSysNo; }
           set { this.SetValue("OperatorSysNo", ref m_OperatorSysNo, value);  }
       }

       /// <summary>
       ///  ����ԭ��״̬
       /// </summary>
       private SOStatus? m_OldStatus;
       public SOStatus? OldStatus
       {
           get { return this.m_OldStatus; }
           set { this.SetValue("OldStatus", ref m_OldStatus, value); }
       }

       /// <summary>
       /// ������ǰ״̬��Ҫ���ĵ���״̬
       /// </summary>
       private SOStatus? m_Status;
       public SOStatus? Status
       {
           get { return this.m_Status; }
           set { this.SetValue("Status", ref m_Status, value);  }
       }

       /// <summary>
       /// ״̬����ʱ��
       /// </summary>
       private DateTime? m_ChangeTime;
       public DateTime? ChangeTime
       {
           get { return this.m_ChangeTime; }
           set { this.SetValue("ChangeTime", ref m_ChangeTime, value);  }
       }

       /// <summary>
       /// �Ƿ����ʼ����ͻ�
       /// </summary>
       private Boolean? m_IsSendMailToCustomer;
       public Boolean? IsSendMailToCustomer
       {
           get { return this.m_IsSendMailToCustomer; }
           set { this.SetValue("IsSendMailToCustomer", ref m_IsSendMailToCustomer, value);  }
       }

       /// <summary>
       /// ״̬���ı�ע
       /// </summary>
       private String m_Note;
       public String Note
       {
           get { return this.m_Note; }
           set { this.SetValue("Note", ref m_Note, value);  }
       }
   }
}
