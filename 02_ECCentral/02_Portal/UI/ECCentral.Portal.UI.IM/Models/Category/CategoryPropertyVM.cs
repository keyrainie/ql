//************************************************************************
// 用户名				泰隆优选
// 系统名				分类属性管理
// 子系统名		        分类属性管理Models
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Windows;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryPropertyVM : ModelBase
    {
        public List<KeyValuePair<WebDisplayStyle?, string>> DisplayStyleList { get; set; }
        public List<KeyValuePair<PropertyType?, string>> PropertyTypeList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryPropertyVM()
        {

            this.DisplayStyleList = EnumConverter.GetKeyValuePairs<WebDisplayStyle>();
            this.PropertyTypeList = EnumConverter.GetKeyValuePairs<PropertyType>();
        }

        /// <summary>
        /// 属性
        /// </summary>
        public PropertyVM Property { get; set; }


        /// <summary>
        /// 属性组
        /// </summary>
        public PropertyGroupInfoVM PropertyGroup { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        private int priority;

        [IntRangeCustomValidation(0, 999999, ErrorMessageResourceType = typeof(ResCategoryPropertyMaintainDetail), ErrorMessageResourceName = "PriorityInvalid")]
        public int Priority 
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }

        /// <summary>
        /// 搜索优先级
        /// </summary>
        private int searchPriority;

        [IntRangeCustomValidation(0, 999999, ErrorMessageResourceType = typeof(ResCategoryPropertyMaintainDetail), ErrorMessageResourceName = "PriorityInvalid")]
        public int SearchPriority 
        {
            get { return searchPriority; }
            set { base.SetValue("SearchPriority", ref searchPriority, value); }
        }

        /// <summary>
        ///类型
        /// </summary>
        public PropertyType PropertyType { get; set; }

        /// <summary>
        /// 前台展示样式
        /// </summary>
        public WebDisplayStyle DisplayStyle { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 必选
        /// </summary>
        public CategoryPropertyStatus IsMustInput { get; set; }

        /// <summary>
        /// 商品管理查询
        /// </summary>
        public CategoryPropertyStatus IsItemSearch { get; set; }

        /// <summary>
        /// 高级搜索
        /// </summary>
        public CategoryPropertyStatus IsInAdvSearch { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        public CategoryVM CategoryInfo { get; set; }

        public int? SysNo { get; set; }

        #region "批量更新需要的属性"

        public bool IsMustInputBat { get; set; }
        public bool IsItemSearchBat { get; set; }

        public bool IsInAdvSearchBat { get; set; 
        }
        public string PropertyName{get;set;}
        public string GroupName { get; set; }

        public string UserName { get; set; }

        public DateTime LastDate { get; set; }
        #endregion


    }

    /// <summary>
    /// 属性信息
    /// </summary>
    public class PropertyGroupInfoVM : ModelBase
    {

        /// <summary>
        ///  属性组标题（前台使用）
        /// </summary>
        public string PropertyGroupName { get; set; }

        public int? SysNo { get; set; }

    }
}
