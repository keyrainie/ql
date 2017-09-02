using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductCommissionQuotaVM : ModelBase
    {
        public ProductCommissionQuotaVM()
        {
            List<KeyValuePair<CategoryType?, string>> categoryTypeList = new List<KeyValuePair<CategoryType?, string>>() 
            { new KeyValuePair<CategoryType?, string>(ECCentral.BizEntity.IM.CategoryType.CategoryType2,ResCategoryKPIMaintain.SelectTextCategory2),
            new KeyValuePair<CategoryType?, string>(ECCentral.BizEntity.IM.CategoryType.CategoryType3,ResCategoryKPIMaintain.SelectTextCategory3)};
            CategoryTypeList = categoryTypeList;
            ComparisonList = EnumConverter.GetKeyValuePairs<Comparison>(EnumConverter.EnumAppendItemType.None);
            ProductStatusList = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
        }

        /// <summary>
        /// 类型
        /// </summary>
        private CategoryType? categoryType = ECCentral.BizEntity.IM.CategoryType.CategoryType3;
        public CategoryType? CategoryType {
            get { return categoryType; }
            set { SetValue("CategoryType", ref categoryType, value); }
        }
        
        /// <summary>
        /// 类别1SysNo
        /// </summary>
        public int? Category1SysNo { get; set; }
        /// <summary>
        /// 类别2SysNo
        /// </summary>
        public int? Category2SysNo { get; set; }
        /// <summary>
        /// 类别3SysNo
        /// </summary>
        public int? Category3SysNo { get; set; }


        /// <summary>
        /// 生产商名称
        /// </summary>
        private string manufacturerName;
         [Validate(ValidateType.Required)]    
        public string ManufacturerName {
            get { return manufacturerName; }
            set { SetValue("ManufacturerName", ref manufacturerName, value); }
        }

        /// <summary>
        /// 生产商SysNo
        /// </summary>
        private string manufacturerSysNo;
         [Validate(ValidateType.Required)]   
        public string ManufacturerSysNo {
            get { return manufacturerSysNo; }
            set { SetValue("ManufacturerSysNo", ref manufacturerSysNo, value); }
        }

        /// <summary>
        /// 等于；不等于
        /// </summary>
        private Comparison? comparison = ECCentral.BizEntity.IM.Comparison.Equal;
        public Comparison? Comparison {
            get { return comparison; }
            set { SetValue("Comparison", ref comparison, value); }
        }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus? ProductStatus { get; set; }

        /// <summary>
        /// 最低佣金限额
        /// </summary>
        public string commissionMin;
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,5}\.\d{1,2}$|^[1-9][0-9]{1,5}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryExtendWarrantyMaintain),ErrorMessageResourceName="Error_InputDouble")]    
        public string CommissionMin 
        {
            get 
            {
                if (string.IsNullOrEmpty(commissionMin))
                {
                    return "0.00";
                }
                else
                {
                    return commissionMin;
                }
                
            }
            set { SetValue("CommissionMin", ref commissionMin, value); }
        }

        /// <summary>
        /// PMSysNo
        /// </summary>
        public int? PMSysNo { get; set; }

        public List<KeyValuePair<CategoryType?, string>> CategoryTypeList { get; set; }
        public List<KeyValuePair<Comparison?, string>> ComparisonList { get; set; }
        public List<KeyValuePair<ProductStatus?, string>> ProductStatusList { get; set; }

    }
}
