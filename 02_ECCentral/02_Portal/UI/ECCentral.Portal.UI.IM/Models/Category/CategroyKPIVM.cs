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
using System.Collections.Generic;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategroyKPIVM : ModelBase
    {

        public CategroyKPIVM()
        {
        }

        public int SysNo { get; set; }

        /// <summary>
        /// 三级类别
        /// </summary>
        public string C3Name { get; set; }

        /// <summary>
        /// 二级类别
        /// </summary>
        public string C2Name { get; set; }   
        /// <summary>
        /// 跌价风险
        /// </summary>
        public int CheapenRisk { get; set; }

        public string CheapenRiskDisplay
        {
            get
            {
                if (CheapenRisk == 1)
                {
                    return ResCategoryKPIMaintain.CheapenRiskDisplay1;
                }
                else if (CheapenRisk == 2)
                {
                    return ResCategoryKPIMaintain.CheapenRiskDisplay2;
                }
                else if (CheapenRisk == 3)
                {
                    return ResCategoryKPIMaintain.CheapenRiskDisplay3;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 是否贵重物品
        /// </summary>
        public int IsValuableProduct { get; set; }

        public string IsValueableProductDisplay
        {
            get
            {
                if (IsValuableProduct == 1)
                {
                    return ResCategoryKPIMaintain.IsValueableProductDisplay1;
                }
                else if (IsValuableProduct == 0)
                {
                    return ResCategoryKPIMaintain.IsValueableProductDisplay2;
                }
                else
                {
                    return ResCategoryKPIMaintain.IsValueableProductDisplay3;
                }
            }
        }

        /// <summary>
        /// 产品账期
        /// </summary>
        public int PayPeriodType { get; set; }


        public string PayPeriodTypeDisplay
        {
            get
            {
                if (PayPeriodType == 1)
                {
                    return ResCategoryKPIMaintain.PayPeriodTypeDisplay1;
                }
                else if (PayPeriodType == 2)
                {
                    return ResCategoryKPIMaintain.PayPeriodTypeDisplay2;
                }
                else if (PayPeriodType == 3)
                {
                    return ResCategoryKPIMaintain.PayPeriodTypeDisplay3;
                }
                else if (PayPeriodType == 4)
                {
                    return ResCategoryKPIMaintain.PayPeriodTypeDisplay4;
                }
                else if (PayPeriodType == 5)
                {
                    return ResCategoryKPIMaintain.PayPeriodTypeDisplay5;
                }
                else if (PayPeriodType == 6)
                {
                    return ResCategoryKPIMaintain.PayPeriodTypeDisplay6;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 缺货率
        /// </summary>
        public decimal OOSRate { get; set; }

        public decimal OOSRatePercent
        {
            get { return OOSRate * 100m; }
            set { OOSRate = value / 100m; }
        }

        /// <summary>
        /// 缺货数量
        /// </summary>
        public int OOSQty { get; set; }

        /// <summary>
        /// 混合虚库率
        /// </summary>
        public decimal VirtualRate { get; set; }

        public decimal VirtualRatePercent
        {
            get { return VirtualRate * 100m; }
            set { VirtualRate = value / 100m; }
        }

        /// <summary>
        /// 纯虚库数量
        /// </summary>
        public int VirtualCount { get; set; }

        /// <summary>
        /// 毛利下限
        /// </summary>
        public decimal MinMargin { get; set; }

        /// <summary>
        /// DMS率
        /// </summary>
        public decimal DMSRate { get; set; }

        /// <summary>
        /// 附件约束
        /// </summary>
        public int IsRequired { get; set; }

        public string IsRequiredDisplay
        {
            get
            {
                return IsRequired == 1 ? ResCategoryKPIMaintain.IsValueableProductDisplay1 : ResCategoryKPIMaintain.IsValueableProductDisplay2;
            }
        }
        
    }
}
