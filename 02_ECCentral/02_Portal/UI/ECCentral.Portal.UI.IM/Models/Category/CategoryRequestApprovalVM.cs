using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryRequestApprovalVM : ModelBase
    {
        public OperationType? OperationType { get; set; }
        public string OriginalCategory1Name { get; set; }
        public string OriginalCategory2Name { get; set; }
        public string OriginalCategory3Name { get; set; }
        public CategoryStatus CategoryStatus { get; set; }
        public string Category1Name { get; set; }
        public string Category2Name { get; set; }
        public string Category3Name { get; set; }
        public CategoryStatus OriginalStatus { get; set; }
        public int? Status { get; set; }
        public string Reansons { get; set; }
        public int? CategorySysNo { get; set; }
        public int SysNo { get; set; }
        public string CategoryName { get; set; }
        public CategoryType CategoryType { get; set; }
        public int? ParentSysNumber { get; set; }
        public string CategoryID { get; set; }
        public string C3Code { get; set; }
        public string OriginalC3Code { get; set; }

        public bool HasCategoryRequestApprovalPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_CategoryRequestApproval); }
        }

        public bool HasCategoryRequestApplyPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_CategoryRequestApply); }
        }
    }
}
