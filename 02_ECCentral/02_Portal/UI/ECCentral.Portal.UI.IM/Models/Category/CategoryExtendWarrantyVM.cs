//************************************************************************
// 用户名				泰隆优选
// 系统名				分类延保管理
// 子系统名		        分类延保管理Models
// 作成者				Kevin
// 改版日				2012.6.5
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryExtendWarrantyVM : ModelBase
    {
        public List<KeyValuePair<CategoryExtendWarrantyStatus?, string>> StatusList { get; set; }
        public List<KeyValuePair<CategoryExtendWarrantyYears?, string>> YearsList { get; set; }
        public List<KeyValuePair<BooleanEnum?, string>> ECSelectedList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryExtendWarrantyVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<CategoryExtendWarrantyStatus>(EnumConverter.EnumAppendItemType.None);
            this.YearsList = EnumConverter.GetKeyValuePairs<CategoryExtendWarrantyYears>(EnumConverter.EnumAppendItemType.None);
            this.ECSelectedList = EnumConverter.GetKeyValuePairs<BooleanEnum>(EnumConverter.EnumAppendItemType.None);

        }

        public int? SysNo { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        public CategoryVM CategoryInfo { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public BrandVM Brand { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CategoryExtendWarrantyStatus Status { get; set; }

        private string productCode = "";
        /// <summary>
        /// 延保编号
        /// </summary>
        [Validate(ValidateType.Regex, @"[a-zA-Z0-9]", ErrorMessageResourceType=typeof(ResManufacturerRequest),ErrorMessageResourceName="Error_InputCharAndNumber")]
        public string ProductCode
        {
            get { return productCode; }
            set { base.SetValue("ProductCode", ref productCode, value); }
        }

        /// <summary>
        ///  延保年限
        /// </summary>
        public CategoryExtendWarrantyYears Years { get; set; }

        /// <summary>
        /// 价格下限
        /// </summary>
        private string minUnitPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryExtendWarrantyMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string MinUnitPrice
        {
            get { return minUnitPrice; }
            set { base.SetValue("MinUnitPrice", ref minUnitPrice, value); }
        }

        /// <summary>
        /// 价格上限
        /// </summary>
        private string maxUnitPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryExtendWarrantyMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string MaxUnitPrice
        {
            get { return maxUnitPrice; }
            set { base.SetValue("MaxUnitPrice", ref maxUnitPrice, value); }
        }

        /// <summary>
        /// 延保价格
        /// </summary>
        private string unitPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryExtendWarrantyMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string UnitPrice
        {
            get { return unitPrice; }
            set { base.SetValue("UnitPrice", ref unitPrice, value); }
        }

        /// <summary>
        /// 延保成本
        /// </summary>
        private string cost;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryExtendWarrantyMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string Cost
        {
            get { return cost; }
            set { base.SetValue("Cost", ref cost, value); }
        }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo InUser { get; set; }

        /// <summary>
        ///  是否前台展示
        /// </summary>
        public BooleanEnum IsECSelected { get; set; }

        public bool HasCategoryExtendWarrantyMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_CategoryExtendWarrantyMaintain); }
        }

    }

    public class CategoryExtendWarrantyDisuseBrandVM : ModelBase
    {
        public List<KeyValuePair<CategoryExtendWarrantyDisuseBrandStatus?, string>> StatusList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryExtendWarrantyDisuseBrandVM()
        {

            this.StatusList = EnumConverter.GetKeyValuePairs<CategoryExtendWarrantyDisuseBrandStatus>(EnumConverter.EnumAppendItemType.None);
        }

        public int? SysNo { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        public CategoryVM CategoryInfo { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public BrandVM Brand { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CategoryExtendWarrantyDisuseBrandStatus Status { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo InUser { get; set; }

        public bool HasExtendWarrantyDisuseBrandMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_ExtendWarrantyDisuseBrandMaintain); }

        }
    }

}
