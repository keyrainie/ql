using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryRelatedQueryVM:ModelBase
    {
        public int? C1SysNo1
        {
            get;
            set;
        }



        public int? C2SysNo1
        {
            get;
            set;
        }


        public int? C3SysNo1
        {
            get;
            set;
        }


        public int? C1SysNo2
        {
            get;
            set;
        }


        public int? C2SysNo2
        {
            get;
            set;
        }


        public int? C3SysNo2
        {
            get;
            set;
        }

        public bool HasRelativeCategoryMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_RelativeCategoryMaintain); }
        }
    }
}
