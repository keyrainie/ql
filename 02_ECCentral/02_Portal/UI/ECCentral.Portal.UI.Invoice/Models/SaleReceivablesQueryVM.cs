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
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class SaleReceivablesQueryVM : ModelBase
    {
        public SaleReceivablesQueryVM()
        {
            QueryDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
        }

        /// <summary>
        /// 支付类型
        /// </summary>
        private string payTypeSysNo;
        [Validate(ValidateType.Required)]
        public string PayTypeSysNo
        {
            get { return this.payTypeSysNo; }
            set { this.SetValue("PayTypeSysNo", ref payTypeSysNo, value); }
        }

        /// <summary>
        /// 统计日期
        /// </summary>
        private DateTime? _queryDate;
        public DateTime? QueryDate
        {
            get { return this._queryDate; }
            set { this.SetValue("QueryDate", ref _queryDate, value); }
        }

        /// <summary>
        /// 货币单位
        /// </summary>
        private SaleCurrency? _currency;
        [Validate(ValidateType.Required)]
        public SaleCurrency? Currency
        {
            get { return this._currency; }
            set { this.SetValue("Currency", ref _currency, value); }
        }

    }
}