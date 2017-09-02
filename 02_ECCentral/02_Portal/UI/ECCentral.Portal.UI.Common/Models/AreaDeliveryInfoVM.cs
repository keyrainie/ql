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
   public class AreaDeliveryInfoVM : ModelBase
   {
       private int? m_SysNo;
       public int? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private int? m_WHArea;
       public int? WHArea
       {
           get { return this.m_WHArea; }
           set { this.SetValue("WHArea", ref m_WHArea, value);  }
       }

       private string m_City;
       [Validate(ValidateType.Required)]
       public string City
       {
           get { return this.m_City; }
           set { this.SetValue("City", ref m_City, value);  }
       }

       private string m_DeliveryScope;
       [Validate(ValidateType.Required)]
       public string DeliveryScope
       {
           get { return this.m_DeliveryScope; }
           set { this.SetValue("DeliveryScope", ref m_DeliveryScope, value);  }
       }

       private string m_Priority;
       [Validate(ValidateType.Required)]
       [Validate(ValidateType.Interger)]
       public string Priority
       {
           get { return this.m_Priority; }
           set { this.SetValue("Priority", ref m_Priority, value);  }
       }

       private string m_Status;
       public string Status
       {
           get { return this.m_Status; }
           set { this.SetValue("Status", ref m_Status, value);  }
       }

       private string m_CompanyCode;
       public string CompanyCode
       {
           get { return this.m_CompanyCode; }
           set { this.SetValue("CompanyCode", ref m_CompanyCode, value);  }
       }

       private DateTime m_InDate;
       public DateTime InDate
       {
           get { return this.m_InDate; }
           set { this.SetValue("InDate", ref m_InDate, value);  }
       }

       private string m_InUser;
       public string InUser
       {
           get { return this.m_InUser; }
           set { this.SetValue("InUser", ref m_InUser, value);  }
       }

       private DateTime m_EditDate;
       public DateTime EditDate
       {
           get { return this.m_EditDate; }
           set { this.SetValue("EditDate", ref m_EditDate, value);  }
       }

       private string m_EditUser;
       public string EditUser
       {
           get { return this.m_EditUser; }
           set { this.SetValue("EditUser", ref m_EditUser, value);  }
       }

       #region 扩展属性

       private bool isEdit;
       /// <summary>
       /// 用于界面控制是否编辑状态
       /// </summary>
       public bool IsEdit
       {
           get { return isEdit; }
           set { SetValue("IsEdit", ref isEdit, value); }
       }

       #endregion

   }
}
