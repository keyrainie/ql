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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GroupBuyingSettlementQueryVM : ModelBase
    {
        private string sysNo;
        [Validate(ValidateType.Interger)]
        public string SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }

        private SettlementBillStatus? _status;
        public SettlementBillStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }

        private DateTime? _createDateFrom;      
        public DateTime? CreateDateFrom
        {
            get { return _createDateFrom; }
            set
            {
                base.SetValue("CreateDateFrom", ref _createDateFrom, value);
            }
        }
        private DateTime? _createDateTo;       
        public DateTime? CreateDateTo
        {
            get { return _createDateTo; }
            set
            {
                base.SetValue("CreateDateTo", ref _createDateTo, value);
            }
        }
       
        private DateTime? _settleDateFrom;
        /// <summary>
        /// 结束时间范围从
        /// </summary>
        public DateTime? SettleDateFrom
        {
            get { return _settleDateFrom; }
            set
            {
                base.SetValue("SettleDateFrom", ref _settleDateFrom, value);
            }
        }
        private DateTime? _settleDateTo;
        /// <summary>
        /// 结束时间范围到
        /// </summary>
        public DateTime? SettleDateTo
        {
            get { return _settleDateTo; }
            set
            {
                base.SetValue("SettleDateTo", ref _settleDateTo, value);
            }
        }      

        public List<KeyValuePair<SettlementBillStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SettlementBillStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        private PayItemStatus? _payItemStatus;
        public PayItemStatus? PayItemStatus
        {
            get { return _payItemStatus; }
            set
            {
                base.SetValue("PayItemStatus", ref _payItemStatus, value);
            }
        }
        public List<KeyValuePair<PayItemStatus?, string>> PayItemStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<PayItemStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }      
    }
}
