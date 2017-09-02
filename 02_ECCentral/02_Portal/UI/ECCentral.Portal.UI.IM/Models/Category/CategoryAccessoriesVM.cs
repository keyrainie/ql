//************************************************************************
// 用户名				泰隆优选
// 系统名				分类属性管理
// 子系统名		        分类属性管理Models
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryAccessoriesVM : ModelBase
    {
        public List<KeyValuePair<CategoryAccessoriesStatus?, string>> StatusList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryAccessoriesVM()
        {

            this.StatusList = EnumConverter.GetKeyValuePairs<CategoryAccessoriesStatus>(EnumConverter.EnumAppendItemType.None);
        }

        /// <summary>
        /// 配件
        /// </summary>
        public AccessoryVM Accessory { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CategoryAccessoriesStatus Status { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }


        /// <summary>
        /// 三级分类
        /// </summary>
        public CategoryVM CategoryInfo { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public IsDefault IsDefault { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        public int? SysNo { get; set; }

        public bool HasAccessoryMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_AccessoryMaintain); }

        }
    }

}
