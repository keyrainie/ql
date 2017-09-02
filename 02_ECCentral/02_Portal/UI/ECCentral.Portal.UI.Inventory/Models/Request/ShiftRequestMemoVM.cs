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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.Basic.Utilities;


namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ShiftRequestMemoVM : ModelBase
    {
        public ShiftRequestMemoVM()
        {
        }
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }


        private int? requestSysNo;
        public int? RequestSysNo
        {
            get { return requestSysNo; }
            set { SetValue("RequestSysNo", ref requestSysNo, value); }
        }

        private string content;
        [Validate(ValidateType.Required)]
        public string Content
        {
            get { return content; }
            set { SetValue("Content", ref content, value); }
        }

        private ShiftRequestMemoStatus? memoStatus;
        public ShiftRequestMemoStatus? MemoStatus
        {
            get { return memoStatus; }
            set { 
                SetValue("MemoStatus", ref memoStatus, value);
                this.ReminderVisibility = value == ShiftRequestMemoStatus.FollowUp ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private int? createUserSysNo;
        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        private string createUserName;
        public string CreateUserName
        {
            get { return createUserName; }
            set { SetValue("CreateUserName", ref createUserName, value); }
        }

        private DateTime? createDate;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
        }

        private int? editUserSysNo;
        /// <summary>
        /// 更新人
        /// </summary>
        public int? EditUserSysNo
        {
            get { return editUserSysNo; }
            set { base.SetValue("EditUserSysNo", ref editUserSysNo, value); }
        }
        private string editUserName;
        public string EditUserName
        {
            get { return editUserName; }
            set { SetValue("EditUserName", ref editUserName, value); }
        }


        private DateTime? editDate;
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? EditDate
        {
            get { return editDate; }
            set { base.SetValue("EditDate", ref editDate, value); }
        }

        private string note;
        [Validate(ValidateType.Required)]
        public string Note
        {
            get { return note; }
            set { SetValue("Note", ref note, value); }
        }

        private DateTime? remindTime;
        /// <summary>
        /// 提醒日期
        /// </summary>
        public DateTime? RemindTime
        {
            get { return remindTime; }
            set { base.SetValue("RemindTime", ref remindTime, value); }
        }
        public List<KeyValuePair<ShiftRequestMemoStatus?, string>> shiftRequestMemoStatusList;
        public List<KeyValuePair<ShiftRequestMemoStatus?, string>> ShiftRequestMemoStatusList
        {
            get
            {
                shiftRequestMemoStatusList = shiftRequestMemoStatusList ?? EnumConverter.GetKeyValuePairs<ShiftRequestMemoStatus>();
                return shiftRequestMemoStatusList;
            }
        }
    
        #region UI Model

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetValue("IsChecked", ref isChecked, value); }
        }

        private Visibility reminderVisibility;
        public Visibility ReminderVisibility
        {
            get 
            {
                return reminderVisibility;
            }
            set
            {
                SetValue("ReminderVisibility", ref reminderVisibility, value); 
            }
        }
        #endregion UI Model
    }
}
