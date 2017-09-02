using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models.Category
{

    public class CategoryKPIMinMarginVM : ModelBase
    {
        /// <summary>
        /// 最小毛利率
        /// </summary>
        public Dictionary<MinMarginDays, MinMarginKPIVM> Margin { get; set; }

        /// <summary>
        /// 三级分类编号
        /// </summary>
        public int CategorySysNo { get; set; }

        public MinMarginKPIVM M1 { get; set; }

        public MinMarginKPIVM M2 { get; set; }

        public MinMarginKPIVM M3 { get; set; }

        public MinMarginKPIVM M4 { get; set; }

        public MinMarginKPIVM M5 { get; set; }

        public MinMarginKPIVM M6 { get; set; }

        /// <summary>
        /// 初始化毛利率
        /// </summary>
        public void InitMargin()
        {
            if (Margin == null) Margin = new Dictionary<MinMarginDays, MinMarginKPIVM>();
            if (Margin.ContainsKey(MinMarginDays.Thirty))
            {
                M1 = Margin[MinMarginDays.Thirty];
            }
            else if (M1 == null)
            {
                M1 = new MinMarginKPIVM();
            }
            if (Margin.ContainsKey(MinMarginDays.Sixty))
            {
                M2 = Margin[MinMarginDays.Sixty];
            }
            else if (M2 == null)
            {
                M2 = new MinMarginKPIVM();
            }
            if (Margin.ContainsKey(MinMarginDays.Ninety))
            {
                M3 = Margin[MinMarginDays.Ninety];
            }
            else if (M3 == null)
            {
                M3 = new MinMarginKPIVM();
            }
            if (Margin.ContainsKey(MinMarginDays.OneHundredAndTwenty))
            {
                M4 = Margin[MinMarginDays.OneHundredAndTwenty];
            }
            else if (M4 == null)
            {
                M4 = new MinMarginKPIVM();
            }
            if (Margin.ContainsKey(MinMarginDays.OneHundredAndEighty))
            {
                M5 = Margin[MinMarginDays.OneHundredAndEighty];
            }
            else if (M5 == null)
            {
                M5 = new MinMarginKPIVM();
            }
            if (Margin.ContainsKey(MinMarginDays.Other))
            {
                M6 = Margin[MinMarginDays.Other];
            }
            else if (M6 == null)
            {
                M6 = new MinMarginKPIVM();
            }
        }

        /// <summary>
        /// 获取毛利率下限
        /// </summary>
        public void GetMargin()
        {
            Margin = new Dictionary<MinMarginDays, MinMarginKPIVM>();
            if (M1 == null)
            {
                M1 = new MinMarginKPIVM();
            }
            AddMargin(MinMarginDays.Thirty, M1);
            if (M2 == null)
            {
                M2 = new MinMarginKPIVM();
            }
            AddMargin(MinMarginDays.Sixty, M2);
            if (M3 == null)
            {
                M3 = new MinMarginKPIVM();
            }
            AddMargin(MinMarginDays.Ninety, M3);
            if (M4 == null)
            {
                M4 = new MinMarginKPIVM();
            }
            AddMargin(MinMarginDays.OneHundredAndTwenty, M4);
            if (M5 == null)
            {
                M5 = new MinMarginKPIVM();
            }
            AddMargin(MinMarginDays.OneHundredAndEighty, M5);
            if (M6 == null)
            {
                M6 = new MinMarginKPIVM();
            }
            AddMargin(MinMarginDays.Other, M6);
        }

        private void AddMargin(MinMarginDays days, MinMarginKPIVM value)
        {
            if (Margin.ContainsKey(days))
            {
                Margin[days].MinMargin = value.MinMargin;
                Margin[days].MaxMargin = value.MaxMargin;
            }
            else
            {
                var tempValue = new MinMarginKPIVM { MinMargin = value.MinMargin, MaxMargin = value.MaxMargin };
                Margin.Add(days, tempValue);
            }
        }

        public bool HasCategory3MinMarginMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_Category3MinMarginMaintain); }
        }
    }

    /// <summary>
    /// 指标数据
    /// </summary>
    public class MinMarginKPIVM : ModelBase
    {
        public MinMarginDays MinMarginDays { get; set; }

        //2013-1-5 update Bug 95190 
        //给属性加默认值，不然会序列化错误
        private string minMargin = "0";
        [Validate(ValidateType.Regex, @"^-?(100|[1-9]?\d(\.\d+)?)$|^-100.00|^100.00", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputBetweenNumber")]    
        public string MinMargin 
        {
            get { return minMargin; }
            set { base.SetValue("MinMargin", ref minMargin, value); }
        }

        private string maxMargin="0";
        [Validate(ValidateType.Regex, @"^-?(100|[1-9]?\d(\.\d+)?)$|^-100.00|^100.00", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputBetweenNumber")]    
        public string MaxMargin
        {
            get { return maxMargin; }
            set { base.SetValue("MaxMargin", ref maxMargin, value); }
        }
    }

}
