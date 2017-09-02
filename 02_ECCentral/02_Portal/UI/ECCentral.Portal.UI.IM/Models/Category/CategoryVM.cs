using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryVM : ModelBase
    {
        public List<KeyValuePair<CategoryStatus?, string>> CategoryStatusList { get; set; }
        public List<KeyValuePair<CategoryType?, string>> CategoryTypeList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryVM()
        {

            this.CategoryStatusList = EnumConverter.GetKeyValuePairs<CategoryStatus>();
            CategoryTypeList = EnumConverter.GetKeyValuePairs<CategoryType>();
        }

        /// <summary>
        /// 类别名称
        /// </summary>
        private string categoryName;
        [Validate(ValidateType.Required)]
        public string CategoryName
        {
            get { return categoryName; }
            set { base.SetValue("CategoryName", ref categoryName, value); }
        }


        /// <summary>
        /// 类别ID
        /// </summary>
        private string _categoryID;
        public string CategoryID
        {
            get { return _categoryID; }
            set { SetValue("CategoryID", ref _categoryID, value); }
        }

        /// <summary>
        /// 类别状态
        /// </summary>
        private CategoryStatus status = CategoryStatus.DeActive;//默认无效
        public CategoryStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }
        private string reansons;
        [Validate(ValidateType.Required)]
        public string Reansons
        {
            get { return reansons; }
            set { SetValue("Reansons", ref reansons, value); }
        }
        private CategoryType type = CategoryType.CategoryType1;
        public CategoryType Type
        {
            get { return type; }
            set { SetValue("Type", ref type, value); }
        }
        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        public bool HasCategoryRequestApplyPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_CategoryRequestApply); }
        }

        private string _C3Code;
        [Validate(ValidateType.Required)]
        public string C3Code
        {
            get { return _C3Code; }
            set { SetValue("C3Code", ref _C3Code, value); }
        }
    }
}
