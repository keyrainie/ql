
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Windows;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Category
{
    public class CategoryKPIRMAInfoVM : ModelBase
    {
        /// <summary>
        /// 保修天数
        /// </summary>

        private string warrantyDays;

        //[IntRangeCustomValidation(0, 999999, ErrorMessageResourceType = typeof(ResCategoryKPIDetail), ErrorMessageResourceName = "WarrantyDaysInvalid")]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntMessage")]
        public string WarrantyDays
        {
            get { return warrantyDays; }
            set { base.SetValue("WarrantyDays", ref warrantyDays, value); }
        }

        /// <summary>
        /// RMA 率标准
        /// </summary>

        private string rmaRateStandard;

        //[DoubleRangeCustomValidation(0, 100, ErrorMessageResourceType = typeof(ResCategoryKPIDetail), ErrorMessageResourceName = "RMARateStandardInvalid")]
        [Validate(ValidateType.Regex, @"^([1-9]\d?(\.\d{1,2})?|0\.\d{1,2}|100|0)$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string RMARateStandard
        {
            get { return rmaRateStandard; }
            set { base.SetValue("RMARateStandard", ref rmaRateStandard, value); }
        }


        /// <summary>
        /// 三级分类编号
        /// </summary>
        public int CategorySysNo { get; set; }

        public bool HasCategory3RMAMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_Category3RMAMaintain); }
        }

    }
}
