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
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CHMaintainVM : ModelBase
    {
        public CHMaintainVM()
        {
            StatusList = EnumConverter.GetKeyValuePairs<FPCheckItemStatus>(EnumConverter.EnumAppendItemType.Select);
        }

        public string ChannelID
        {
            get;
            set;
        }

        public List<KeyValuePair<FPCheckItemStatus?, string>> StatusList
        {
            get;
            set;
        }

        private FPCheckItemStatus? _Status;

        [Validate(ValidateType.Required)]
        public FPCheckItemStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }

        private int? _CategorySysNo;
        [Validate(ValidateType.Required)]
        public int? CategorySysNo
        {
            get
            {
                return _CategorySysNo;
            }
            set
            {
                base.SetValue("CategorySysNo", ref _CategorySysNo, value);
            }
        }
        private string _ProductID;
        [Validate(ValidateType.Required)]
        public string ProductID
        {
            get
            {
                return _ProductID;
            }
            set
            {
                base.SetValue("ProductID", ref _ProductID, value);
            }
        }
        private string _ProductSysNo;

        public string ProductSysNo
        {
            get { return _ProductSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _ProductSysNo, value);
            }
        }
 
 
    }
}
