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
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Common.Models
{
   public class ControlPanelUserVM : ModelBase
   {
       public List<KeyValuePair<ControlPanelUserStatus?, string>> ListStatus { get; set; }

       public ControlPanelUserVM()
       {
           this.ListStatus = EnumConverter.GetKeyValuePairs<ControlPanelUserStatus>();
       }

       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private String m_LoginName;
       [Validate(ValidateType.Required)]
       public String LoginName
       {
           get { return this.m_LoginName; }
           set { this.SetValue("LoginName", ref m_LoginName, value); }
       }

       private String m_DisplayName;
       [Validate(ValidateType.Required)]
       public String DisplayName
       {
           get { return this.m_DisplayName; }
           set { this.SetValue("DisplayName", ref m_DisplayName, value);  }
       }

       private String m_DepartmentCode;
       public String DepartmentCode
       {
           get { return this.m_DepartmentCode; }
           set { this.SetValue("DepartmentCode", ref m_DepartmentCode, value);  }
       }

       private String m_DepartmentName;
       public String DepartmentName
       {
           get { return this.m_DepartmentName; }
           set { this.SetValue("DepartmentName", ref m_DepartmentName, value);  }
       }

       private String m_PhoneNumber;
       [Validate(ValidateType.Regex, @"^1\d{10}$")]
       //[Validate(ValidateType.Phone)]
       public String PhoneNumber
       {
           get { return this.m_PhoneNumber; }
           set { this.SetValue("PhoneNumber", ref m_PhoneNumber, value);  }
       }

       private String m_EmailAddress;
       [Validate(ValidateType.Email)]
       [Validate(ValidateType.Required)]
       public String EmailAddress
       {
           get { return this.m_EmailAddress; }
           set { this.SetValue("EmailAddress", ref m_EmailAddress, value);  }
       }

       private ControlPanelUserStatus? m_Status;
       [Validate(ValidateType.Required)]
       public ControlPanelUserStatus? Status
       {
           get { return this.m_Status; }
           set { this.SetValue("Status", ref m_Status, value);  }
       }

       private String m_InUser;
       public String InUser
       {
           get { return this.m_InUser; }
           set { this.SetValue("InUser", ref m_InUser, value);  }
       }

       private DateTime? m_InDate;
       public DateTime? InDate
       {
           get { return this.m_InDate; }
           set { this.SetValue("InDate", ref m_InDate, value);  }
       }

       private String m_EditUser;
       public String EditUser
       {
           get { return this.m_EditUser; }
           set { this.SetValue("EditUser", ref m_EditUser, value);  }
       }

       private DateTime? m_EditDate;
       public DateTime? EditDate
       {
           get { return this.m_EditDate; }
           set { this.SetValue("EditDate", ref m_EditDate, value);  }
       }

       private String m_SourceDirectory;
       [Validate(ValidateType.Required)]
       public String SourceDirectory
       {
           get { return this.m_SourceDirectory; }
           set { this.SetValue("SourceDirectory", ref m_SourceDirectory, value);  }
       }

       private String m_LogicUserId;
       [Validate(ValidateType.Required)]
       public String LogicUserId
       {
           get { return this.m_LogicUserId; }
           set { this.SetValue("LogicUserId", ref m_LogicUserId, value);  }
       }

       private String m_PhysicalUserId;
       [Validate(ValidateType.Required)]
       public String PhysicalUserId
       {
           get { return this.m_PhysicalUserId; }
           set { this.SetValue("PhysicalUserId", ref m_PhysicalUserId, value);  }
       }

       private String m_CompanyCode;
       public String CompanyCode
       {
           get { return this.m_CompanyCode; }
           set { this.SetValue("CompanyCode", ref m_CompanyCode, value);  }
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
