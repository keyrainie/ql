//************************************************************************
// 用户名				泰隆优选
// 系统名				类别管理
// 子系统名		        类别管理Models
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryQueryVM : ModelBase
    {

        public List<KeyValuePair<CategoryStatus?, string>> CategoryStatusList { get; set; }
        public List<KeyValuePair<CategoryType?, string>> CategoryTypeList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryQueryVM()
        {

            this.CategoryStatusList = EnumConverter.GetKeyValuePairs<CategoryStatus>(EnumConverter.EnumAppendItemType.All);
            this.CategoryTypeList = EnumConverter.GetKeyValuePairs<CategoryType>(EnumConverter.EnumAppendItemType.None);

        }

        private int? category1SysNo;
        public int? Category1SysNo {
            get { return category1SysNo; }
            set { SetValue("Category1SysNo", ref category1SysNo, value); }
        }
        private int? category2SysNo;
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { SetValue("Category2SysNo", ref category2SysNo, value); }
        }
        private CategoryType type = CategoryType.CategoryType1;
        public CategoryType Type { 
            get { return type; }
            set { SetValue("Type", ref type, value); }
        }

      
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 类别状态
        /// </summary>
        public CategoryStatus? Status { get; set; }

       
    }
}
