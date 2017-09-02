//************************************************************************
// 用户名				泰隆优选
// 系统名				类别配件管理
// 子系统名		        类别配件管理Models
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryAccessoriesQueryVM : ModelBase
    {

        public List<KeyValuePair<CategoryAccessoriesStatus?, string>> StatusList { get; set; }
        public List<KeyValuePair<IsDefault?, string>> IsDefaultList { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryAccessoriesQueryVM()
        {

            this.StatusList = EnumConverter.GetKeyValuePairs<CategoryAccessoriesStatus>(EnumConverter.EnumAppendItemType.All);
            this.IsDefaultList = EnumConverter.GetKeyValuePairs<IsDefault>(EnumConverter.EnumAppendItemType.All);

        }
        /// <summary>
        /// 三级类
        /// </summary>
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int? Category2SysNo { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? Category1SysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CategoryAccessoriesStatus? Status { get; set; }

        /// <summary>
        /// 配件名称
        /// </summary>
        private string _accessoriesName;
        [Validate(ValidateType.MaxLength, 100)]
        public string AccessoriesName
        {
            get { return _accessoriesName; }
            set { SetValue("AccessoriesName", ref _accessoriesName, value); }
        }

        /// <summary>
        /// 顺序
        /// </summary>
        private string _accessoryOrder;
        [Validate(ValidateType.Regex, new object[] { "^0$|^[1-9]\\d{0,5}$" }, ErrorMessageResourceType = typeof(ResProductAttachmentMaintain), ErrorMessageResourceName = "AccessoryOrder_ErrorInfo")]
        public string AccessoryOrder
        {
            get { return _accessoryOrder; }
            set { SetValue("AccessoryOrder", ref _accessoryOrder, value); }
        }

        /// <summary>
        /// 是否默认
        /// </summary>
        public IsDefault? IsDefault { get; set; }

    }
}
