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
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models.Floor
{
    public class FloorSectionVM : ModelBase
    {
        /// <summary>
        ///系统编号
        /// </summary>
        private int? sysNo;
        [Validate(ValidateType.Interger)]
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        ///楼层编号
        /// </summary>
        private int floorMasterSysNo;
        public int FloorMasterSysNo
        {
            get { return floorMasterSysNo; }
            set { base.SetValue("FloorMasterSysNo", ref floorMasterSysNo, value); }
        }

        /// <summary>
        ///分组标签名称
        /// </summary>
        private string sectionName;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 100)]
        public string SectionName
        {
            get { return sectionName; }
            set { base.SetValue("SectionName", ref sectionName, value); }
        }

        /// <summary>
        ///分组标签优先级，数字越小排在前面，第一个分组标签为默认标签
        /// </summary>
        private int priority;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public int Priority
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }

        /// <summary>
        ///状态，通用状态，共两种：有效，无效；
        /// </summary>
        private ADStatus? status;
        [Validate(ValidateType.Required)]
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        #region 前端使用
        /// <summary>
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }
        #endregion 前端使用
    }
}
