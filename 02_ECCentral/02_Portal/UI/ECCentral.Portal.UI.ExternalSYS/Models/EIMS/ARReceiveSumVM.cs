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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class ARReceiveSumVM
    {
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal SumAccruedTotal { get; set; }

        /// <summary>
        /// 未开单据金额
        /// </summary>
        public decimal SumUnbilled { get; set; }

        /// <summary>
        /// 已开单据金额
        /// </summary>
        public decimal SumOpenTotal { get; set; }

        /// <summary>
        /// 未收金额
        /// </summary>
        public decimal SumNoReceived { get; set; }

        /// <summary>
        /// 30天内到期
        /// </summary>
        public decimal SumDueIn30 { get; set; }

        /// <summary>
        /// 开票31-60天
        /// </summary>
        public decimal SumDueBetween31And60 { get; set; }

        /// <summary>
        /// 开票61-90天
        /// </summary>
        public decimal SumDueBetween61And90 { get; set; }

        /// <summary>
        /// 开票91-120天
        /// </summary>
        public decimal SumDueBetween91And120 { get; set; }

        /// <summary>
        /// 开票121-150天
        /// </summary>
        public decimal SumDueBetween121And150 { get; set; }

        /// <summary>
        /// 开票151-180天
        /// </summary>
        public decimal SumDueBetween151And180 { get; set; }

        /// <summary>
        /// 开票180天天以上
        /// </summary>
        public decimal SumDueOver180 { get; set; }

        /// <summary>
        /// 未开票金额占应开票金额的比例
        /// </summary>
        public decimal SumUnbilledPercentage { get; set; }

        /// <summary>
        /// 已开票金额占应开票金额的比例
        /// </summary>
        public decimal SumOpenTotalPercentage { get; set; }

        /// <summary>
        /// 开票30天内未收占总未收的比例
        /// </summary>
        public decimal SumDueIn30Percentage { get; set; }

        /// <summary>
        /// 开票31-60天内未收占总未收的比例
        /// </summary>
        public decimal SumDueBetween31And60Percentage { get; set; }

        /// <summary>
        /// 开票61-90天内未收占总未收的比例
        /// </summary>
        public decimal SumDueBetween61And90Percentage { get; set; }

        /// <summary>
        /// 开票91-120天内未收占总未收的比例
        /// </summary>
        public decimal SumDueBetween91And120Percentage { get; set; }

        /// <summary>
        /// 开票121-150天内未收占总未收的比例
        /// </summary>
        public decimal SumDueBetween121And150Percentage { get; set; }

        /// <summary>
        /// 开票151-180天内未收占总未收的比例
        /// </summary>
        public decimal SumDueBetween151And180Percentage { get; set; }

        /// <summary>
        /// 开票180天以上未收占总未收的比例
        /// </summary>
        public decimal SumDueOver180Percentage { get; set; }

        #region Display Member

        string formatStr = "{0}%";
        /// <summary>
        /// 未开票金额占应开票金额的比例
        /// </summary>
        public string SumUnbilledPercentageStr
        {
            get
            {
                decimal result = Math.Round((SumUnbilledPercentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }

        /// <summary>
        /// 已开票金额占应开票金额的比例
        /// </summary>
        public string SumOpenTotalPercentageStr
        {
            get
            {
                decimal result = Math.Round((SumOpenTotalPercentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }

        /// <summary>
        /// 开票30天内未收占总未收的比例
        /// </summary>
        public string SumDueIn30PercentageStr
        {
            get
            {
                decimal result = Math.Round((SumDueIn30Percentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }

        /// <summary>
        /// 开票31-60天内未收占总未收的比例
        /// </summary>
        public string SumDueBetween31And60PercentageStr
        {
            get
            {
                decimal result = Math.Round((SumDueBetween31And60Percentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }

        /// <summary>
        /// 开票61-90天内未收占总未收的比例
        /// </summary>
        public string SumDueBetween61And90PercentageStr
        {
            get
            {
                decimal result = Math.Round((SumDueBetween61And90Percentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }

        /// <summary>
        /// 开票91-120天内未收占总未收的比例
        /// </summary>
        public string SumDueBetween91And120PercentageStr
        {
            get
            {
                decimal result = Math.Round((SumDueBetween91And120Percentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }
        /// <summary>
        /// 开票121-150天内未收占总未收的比例
        /// </summary>
        public string SumDueBetween121And150PercentageStr
        {
            get
            {
                decimal result = Math.Round((SumDueBetween121And150Percentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }
        /// <summary>
        /// 开票151-180天内未收占总未收的比例
        /// </summary>
        public string SumDueBetween151And180PercentageStr
        {
            get
            {
                decimal result = Math.Round((SumDueBetween151And180Percentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }

        /// <summary>
        /// 开票180天以上未收占总未收的比例
        /// </summary>
        public string SumDueOver180PercentageStr
        {
            get
            {
                decimal result = Math.Round((SumDueOver180Percentage * 100), 2);
                return string.Format(formatStr, result);
            }
        }
        #endregion
    }
}
