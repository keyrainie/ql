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
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PayInvoiceMaintainVM : ModelBase
    {
        public PayInvoiceMaintainVM()
        {
            this.InvoiceStatusList = EnumConverter.GetKeyValuePairs<PayableInvoiceStatus>(EnumConverter.EnumAppendItemType.All);
            this.InvoiceFactStatusList = EnumConverter.GetKeyValuePairs<PayableInvoiceFactStatus>(EnumConverter.EnumAppendItemType.All);
        }
        public string SysNos
        {
            get;
            set;
        }
        /// <summary>
        /// 单据编号
        /// </summary>
        public string OrderSysNos
        {
            get;
            set;
        }
        /// <summary>
        /// 发票状态
        /// </summary>
        private PayableInvoiceStatus? invoiceStatus;
        [Validate(ValidateType.Required)]
        public PayableInvoiceStatus? InvoiceStatus
        {
            get { return invoiceStatus; }
            set { base.SetValue("InvoiceStatus", ref invoiceStatus, value); }
        }
        /// <summary>
        /// 发票实际状态
        /// </summary>
        private PayableInvoiceFactStatus? invoiceFactStatus;
        [Validate(ValidateType.Required)]
        public PayableInvoiceFactStatus? InvoiceFactStatus
        {
            get { return invoiceFactStatus; }
            set { base.SetValue("InvoiceFactStatus", ref invoiceFactStatus, value); }
        }
        /// <summary>
        /// 注解
        /// </summary>
        private string note;
        [Validate(ValidateType.Required)]
        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }

        public List<KeyValuePair<PayableInvoiceStatus?, string>> InvoiceStatusList { get; set; }
        public List<KeyValuePair<PayableInvoiceFactStatus?, string>> InvoiceFactStatusList { get; set; }
    }
}
