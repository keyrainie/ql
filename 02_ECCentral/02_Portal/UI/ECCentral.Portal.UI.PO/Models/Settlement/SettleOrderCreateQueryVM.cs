using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.PO.Models.Settlement
{
    public class SettleOrderCreateQueryVM : ModelBase
    {
        private int? _VendorSysNo;
        /// <summary>
        /// 商家编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public int? VendorSysNo
        {
            get { return _VendorSysNo; }
            set { SetValue("VendorSysNo", ref _VendorSysNo, value); }
        }

        private bool? _POPositive = false;
        /// <summary>
        /// 进货单
        /// </summary>
        public bool? POPositive
        {
            get { return _POPositive; }
            set { SetValue("POPositive", ref _POPositive, value); }
        }

        private bool? _PONegative = false;

        /// <summary>
        /// 返厂单
        /// </summary>
        public bool? PONegative
        {
            get { return _PONegative; }
            set { SetValue("PONegative", ref _PONegative, value); }
        }

        private bool? _ChangePrice = false;
        /// <summary>
        /// 进价变价单
        /// </summary>
        public bool? ChangePrice
        {
            get
            {
                return _ChangePrice;
            }
            set
            {
                SetValue("ChangePrice", ref _ChangePrice, value);
            }
        }

        private DateTime? _CreateDateFrom;
        /// <summary>
        /// 单据生成时间From
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get
            {
                return _CreateDateFrom;
            }
            set
            {
                SetValue("CreateDateFrom", ref _CreateDateFrom, value);
            }
        }


        private DateTime? _CreateDateTo;
        /// <summary>
        /// 单据生成时间To
        /// </summary>
        public DateTime? CreateDateTo
        {
            get
            {

                return _CreateDateTo;
            }
            set
            {
                SetValue("CreateDateTo", ref _CreateDateTo, value);
            }
        }

        private string _OrderSysNoStrs;
        [Validate(ValidateType.Regex, @"^[,\. ]*\d+[\d,\. ]*$",ErrorMessage="编号只能由数字组成，多个编号间用英文的空格、逗号或点隔开")]
        public string OrderSysNoStrs
        {
            get { return _OrderSysNoStrs; }
            set { SetValue("OrderSysNoStrs", ref _OrderSysNoStrs, value); }
        }

    }
}
