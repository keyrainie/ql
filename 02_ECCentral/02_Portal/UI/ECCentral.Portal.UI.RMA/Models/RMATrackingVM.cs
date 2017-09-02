using System;
using System.Collections.Generic;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RMATrackingVM : ModelBase
    {
        public RMATrackingVM()
        {
            this.InternalMemoStatusList = EnumConverter.GetKeyValuePairs<InternalMemoStatus>();
            this.publicMemoSourceTypes = new List<CodeNamePair>();
            this.Status = InternalMemoStatus.Open;
            this.SourceSysNo = 0;
        }

        #region 查询得到的属性
        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }

        public int? RegisterSysNo { get; set; }

        public int? SOSysNo { get; set; }

        private string content;
        [Validate(ValidateType.Required)]
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                SetValue("Content", ref content, value);
            }
        }

        private InternalMemoStatus? status;
        public InternalMemoStatus? Status
        {
            get
            {
                return status.HasValue ? status.Value : InternalMemoStatus.Open;
            }
            set
            {
                SetValue("Status", ref status, value);
            }
        }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime? RemindTime { get; set; }

        public string Note { get; set; }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }

        public int? ReasonCodeSysNo { get; set; }

        private string reasonCodePath;
        public string ReasonCodePath
        {
            get
            {
                return reasonCodePath;
            }
            set
            {
                SetValue("ReasonCodePath", ref reasonCodePath, value);
            }
        }

        private string source;
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                SetValue("Source", ref source, value);
            }
        }

        private int? sourceSysNo;
        public int? SourceSysNo
        {
            get
            {
                return sourceSysNo;
            }
            set
            {
                SetValue("SourceSysNo", ref sourceSysNo, value);
            }
        }

        //public string RelevantDepartment { get; set; }
        //public TrackingUrgency? Urgency { get; set; }

        #endregion

        #region 转换为界面展示用
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                base.SetValue("IsChecked", ref isChecked, value);
            }
        }
        private bool isEnable;
        public bool IsEnable
        {
            get
            {
                return isEnable = this.Status == InternalMemoStatus.Open ? true : false;
            }
        }

        public List<KeyValuePair<Nullable<InternalMemoStatus>, string>> InternalMemoStatusList { get; set; }
        public List<CodeNamePair> publicMemoSourceTypes { get; set; }
        #endregion
    }
}
