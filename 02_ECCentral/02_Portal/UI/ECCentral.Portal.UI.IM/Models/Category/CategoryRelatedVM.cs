using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;
namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryRelatedVM : ModelBase
    {
        private string c3SysNo1;
        [Validate(ValidateType.Required)]
        public string C3SysNo1
        {
            get { return c3SysNo1; }
            set { base.SetValue("C3SysNo1", ref c3SysNo1, value); }
        }
        private string c3SysNo2;
        [Validate(ValidateType.Required)]
        public string C3SysNo2
        {
            get { return c3SysNo2; }
            set { base.SetValue("C3SysNo2", ref c3SysNo2, value); }
        }
        private string priority;
        [Validate(ValidateType.Regex, @"^[0-9]*[1-9][0-9]*$", ErrorMessageResourceType=typeof(ResBrandQuery),ErrorMessageResourceName="Error_ValidateIntHint")]
        [Validate(ValidateType.Required)]
        public string Priority
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }
        public int? CreateUserSysNo { get; set; }

        public bool HasRelativeCategoryMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_RelativeCategoryMaintain); }

        }
    }
}
