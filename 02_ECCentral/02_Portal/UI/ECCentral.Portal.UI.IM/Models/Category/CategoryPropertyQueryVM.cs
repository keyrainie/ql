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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryPropertyQueryVM : ModelBase
    {

        public List<KeyValuePair<PropertyType?, string>> PropertyTypeList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryPropertyQueryVM()
        {

            this.PropertyTypeList = EnumConverter.GetKeyValuePairs<PropertyType>(EnumConverter.EnumAppendItemType.None);


        }

        /// <summary>
        /// 三级分类
        /// </summary>
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 属性组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        ///类型
        /// </summary>
        private int? categorySysNp;
          [Validate(ValidateType.Required)]
        public int? CategorySysNo {
            get { return categorySysNp; }
            set { SetValue("CategorySysNo", ref categorySysNp, value); }
        }

        private string _propertyName;
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
            set { SetValue("PropertyName", ref _propertyName, value); }
        }

        /// <summary>
        /// 属性
        /// </summary>
        public int? PropertySysNo { get; set; }


        public List<KeyValuePair<int?, string>> PropertyList { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        ///类型
        /// </summary>
        public PropertyType PropertyType { get; set; }

        /// <summary>
        ///复制源类别SysNo
        /// </summary>
        private int? sourceCategorySysNo;
        [Validate(ValidateType.Required)]
        public int? SourceCategorySysNo
        {
            get { return sourceCategorySysNo; }
            set { SetValue("SourceCategorySysNo", ref sourceCategorySysNo, value); }
        }

    }
}
