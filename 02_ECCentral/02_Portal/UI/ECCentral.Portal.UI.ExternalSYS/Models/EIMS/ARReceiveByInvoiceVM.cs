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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class ARReceiveByInvoiceVM : ModelBase
    {
        /// <summary>
        /// 供应商编号
        /// </summary>
        public string VendorNumber { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

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
        public decimal DueIn30 { get; set; }

        /// <summary>
        /// 开票31-60天
        /// </summary>
        public decimal DueBetween31And60 { get; set; }

        /// <summary>
        /// 开票61-90天
        /// </summary>
        public decimal DueBetween61And90 { get; set; }

        /// <summary>
        /// 开票91-120天
        /// </summary>
        public decimal DueBetween91And120 { get; set; }

        /// <summary>
        /// 开票121-150天
        /// </summary>
        public decimal DueBetween121And150 { get; set; }

        /// <summary>
        /// 开票151-180天
        /// </summary>
        public decimal DueBetween151And180 { get; set; }

        /// <summary>
        /// 开票180天天以上
        /// </summary>
        public decimal DueOver180 { get; set; }
    }
}
