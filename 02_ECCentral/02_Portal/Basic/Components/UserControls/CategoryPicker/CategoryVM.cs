using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.Basic.Components.UserControls.CategoryPicker
{
    public class CategoryVM : ModelBase
    {
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private bool? isChecked;
        public bool? IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        /// <summary>
        /// 父级类别SysNo
        /// </summary>
        private int? parentSysNumber;

        public int? ParentSysNumber
        {
            get { return parentSysNumber; }
            set { base.SetValue("ParentSysNumber", ref parentSysNumber, value); }
        }
        /// <summary>
        /// 类别名称
        /// </summary>
        private string categoryDisplayName;

        public string CategoryDisplayName
        {
            get { return categoryDisplayName; }
            set { base.SetValue("CategoryDisplayName", ref categoryDisplayName, value); }
        }

        /// <summary>
        /// 类别状态
        /// </summary>
        private CategoryStatus status;

        public CategoryStatus Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }



    }
}
