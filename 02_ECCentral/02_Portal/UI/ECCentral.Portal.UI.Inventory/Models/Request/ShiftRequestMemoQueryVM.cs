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
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ShiftRequestMemoQueryVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }


        private string requestSysNo;
        [Validate(ValidateType.Interger, ErrorMessageResourceName = "ValidateMsg_NumberOnly", ErrorMessageResourceType = typeof(ResInventoryCommon))]
        public string RequestSysNo
        {
            get { return requestSysNo; }
            set { SetValue("RequestSysNo", ref requestSysNo, value); }
        }

        private string content;
        public string Content
        {
            get { return content; }
            set { SetValue("Content", ref content, value); }
        }

        private ShiftRequestMemoStatus? memoStatus;
        public ShiftRequestMemoStatus? MemoStatus
        {
            get { return memoStatus; }
            set { SetValue("MemoStatus", ref memoStatus, value); }
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

        private DateTime? createDateFrom;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get { return createDateFrom; }
            set { base.SetValue("CreateDateFrom", ref createDateFrom, value); }
        }

        private DateTime? createDateTo;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDateTo
        {
            get { return createDateTo; }
            set { base.SetValue("CreateDateTo", ref createDateTo, value); }
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

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { SetValue("CompanyCode", ref companyCode, value); }
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
                shiftRequestMemoStatusList = shiftRequestMemoStatusList ?? EnumConverter.GetKeyValuePairs<ShiftRequestMemoStatus>(EnumConverter.EnumAppendItemType.All);
                return shiftRequestMemoStatusList;
            }
        }

        #region UI Model
   
    
        #endregion UI Model        

    }

    public class ShiftRequestMemoQueryView : ModelBase
    {
        public ShiftRequestMemoQueryVM QueryInfo
        {
            get;
            set;
        }

        private List<dynamic> result;
        public List<dynamic> Result
        {
            get { return result; }
            set
            {
                SetValue("Result", ref result, value);
            }
        }

        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                SetValue<int>("TotalCount", ref totalCount, value);
            }
        }

        public ShiftRequestMemoQueryView()
        {
            QueryInfo = new ShiftRequestMemoQueryVM();
        }
    }
}
