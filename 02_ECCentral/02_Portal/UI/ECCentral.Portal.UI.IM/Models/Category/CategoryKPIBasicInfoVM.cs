using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models.Category
{
    public class CategoryKPIBasicInfoVM : ModelBase
    {


        public List<KeyValuePair<CheapenRisk?, string>> CheapenRiskList { get; set; }

        public List<KeyValuePair<IsDefault?, string>> IsDefaultList { get; set; }

        public List<KeyValuePair<PayPeriodType?, string>> PayPeriodTypeList { get; set; }

        public List<KeyValuePair<IsLarge?, string>> LargeFlagList { get; set; }
        public List<KeyValuePair<int, string>> InStockDaysList { get; set; }

        public CategoryKPIBasicInfoVM()
        {
            CheapenRiskList = EnumConverter.GetKeyValuePairs<CheapenRisk>(EnumConverter.EnumAppendItemType.None);
            IsDefaultList = EnumConverter.GetKeyValuePairs<IsDefault>(EnumConverter.EnumAppendItemType.None);
            PayPeriodTypeList = EnumConverter.GetKeyValuePairs<PayPeriodType>(EnumConverter.EnumAppendItemType.None);
            LargeFlagList = EnumConverter.GetKeyValuePairs<IsLarge>(EnumConverter.EnumAppendItemType.None);
            InStockDaysList = new List<KeyValuePair<int, string>>();
            InStockDaysList.Add(new KeyValuePair<int, string>(60, "60"));
            InStockDaysList.Add(new KeyValuePair<int, string>(90, "90"));
            InStockDaysList.Add(new KeyValuePair<int, string>(120, "120"));

        }
        /// <summary>
        /// 跌价风险
        /// </summary>
        private CheapenRisk cheapenRiskInfo = CheapenRisk.A; //默认A类(0-2)周
        public CheapenRisk CheapenRiskInfo
        {
            get { return cheapenRiskInfo; }
            set
            {
                if (value == 0)
                {
                    cheapenRiskInfo = CheapenRisk.A;
                }
                else
                {
                    cheapenRiskInfo = value;
                }
            }
        }

        /// <summary>
        /// 是否贵重物品
        /// </summary>
        public IsDefault IsValuableProduct { get; set; }

        /// <summary>
        /// 附件约束
        /// </summary>
        public IsDefault IsRequired { get; set; }

        /// <summary>
        /// 帐期选择
        /// </summary>
        private PayPeriodType payPeriodTypeInfo = PayPeriodType.AdvancePayment; //默认预付款
        public PayPeriodType PayPeriodTypeInfo
        {
            get { return payPeriodTypeInfo; }
            set
            {
                if (value == 0)
                {
                    payPeriodTypeInfo = PayPeriodType.AdvancePayment;
                }
                else
                {
                    payPeriodTypeInfo = value;
                }
            }
        }

        /// <summary>
        /// 是否大货
        /// </summary>
        private IsLarge isLargeInfo = IsLarge.Undefined; //默认未定义
        public IsLarge IsLargeInfo
        {
            get { return isLargeInfo; }
            set
            {
                if (value == 0)
                {
                    isLargeInfo = IsLarge.Undefined;
                }
                else
                {
                    isLargeInfo = value;
                }
            }
        }

        /// <summary>
        /// 缺货率
        /// </summary>
        private string oOSRate;
        [Validate(ValidateType.Regex, @"^([1-9]\d?(\.\d{1,2})?|0\.\d{1,2}|100|0|100.00)$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string OOSRate
        {
            get { return oOSRate; }
            set { base.SetValue("OOSRate", ref oOSRate, value); }
        }

        /// <summary>
        /// 缺货数量
        /// </summary>
        private string oOSQty;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessageResourceType=typeof(ResCategoryExtendWarrantyMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string OOSQty
        {
            get { return oOSQty; }
            set { base.SetValue("OOSQty", ref oOSQty, value); }
        }

        /// <summary>
        /// 混合虚库率
        /// </summary>
        private string virtualRate;
        [Validate(ValidateType.Regex, @"^([1-9]\d?(\.\d{1,2})?|0\.\d{1,2}|100|0|100.00)$", ErrorMessageResourceType=typeof(ResCategoryExtendWarrantyMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string VirtualRate
        {
            get { return virtualRate; }
            set { base.SetValue("VirtualRate", ref virtualRate, value); }
        }

        /// <summary>
        /// 纯虚库数量
        /// </summary>
        private string virtualCount;
        [Validate(ValidateType.Regex, @"^(([1-9][0-9]{0,8})(\.[0-9]{1,2})?)|([0]\.[0-9]{1,2})$|^0$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputDouble")]
        public string VirtualCount
        {
            get { return virtualCount; }
            set { base.SetValue("VirtualCount", ref virtualCount, value); }
        }

        /// <summary>
        /// 天数
        /// </summary>
        private string safetyInventoryDay;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntMessage")]
        public string SafetyInventoryDay
        {
            get { return safetyInventoryDay; }
            set { base.SetValue("SafetyInventoryDay", ref safetyInventoryDay, value); }
        }

        /// <summary>
        /// 数量
        /// </summary>
        private string safetyInventoryQty;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntMessage")]
        public string SafetyInventoryQty
        {
            get { return safetyInventoryQty; }
            set { base.SetValue("SafetyInventoryQty", ref safetyInventoryQty, value); }
        }

        /// <summary>
        /// 额度
        /// </summary>
        private string quota;
        [Validate(ValidateType.Regex, @"^0\.\d{1,2}$|^[1-9][0-9]{0,8}\.\d{1,2}$|^[1-9][0-9]{1,8}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputDoubleIntMessage")]
        public string Quota
        {
            get { return quota; }
            set
            {
                if (value != null)
                {
                    decimal currentValue;
                    if (decimal.TryParse(value, out currentValue))
                    {
                        var tempValue = Math.Round(currentValue, 2);
                        value = tempValue.ToString("0.00");
                    }

                }
                base.SetValue("Quota", ref quota, value);
            }
        }

        private int avgDailySalesCycle;
        public int AvgDailySalesCycle
        {
            get { return avgDailySalesCycle; }
            set { SetValue("AvgDailySalesCycle", ref avgDailySalesCycle, value); }
        }

        /// <summary>
        /// 最低佣金限额
        /// </summary>
        private string minCommission;

        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,5}\.\d{1,2}$|^[1-9][0-9]{1,5}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string MinCommission
        {
            get { return minCommission; }
            set { base.SetValue("MinCommission", ref minCommission, value); }
        }
        /// <summary>
        /// 滞销库存天数信息
        /// </summary>
        public int InStockDays { get; set; }
        public int PropertySysNO { get; set; }
        /// <summary>
        /// 三级分类编号
        /// </summary>
        public int CategorySysNo { get; set; }
        public int Category1SysNo { get; set; }
        public int Category2SysNo { get; set; }

        public bool HasCategoryMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_CategoryMaintain); }
        }

        public bool HasCategoryKpiQuotaMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Category_CategoryKpiQuotaMaintain); }
        }

    }
}
