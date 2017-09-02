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

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CHSetVM : ModelBase
    {
        public CHSetVM()
        {
            StatusList = EnumConverter.GetKeyValuePairs<FPCheckItemStatus>(EnumConverter.EnumAppendItemType.All);
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


        public FPCheckItemStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }

        private int? _CategorySysNo;

        public int? CategorySysNo
        {
            get
            {
                if (IsSearchCategory.HasValue && IsSearchCategory.Value)
                    return _CategorySysNo;
                else
                    return null;
            }
            set { base.SetValue("CategorySysNo", ref _CategorySysNo, value); }
        }
        private string _ProductID;

        public string ProductID
        {
            get
            {
                if (!(IsSearchCategory.HasValue&&IsSearchCategory.Value))
                    return _ProductID;
                else
                    return null;
            }
            set { base.SetValue("ProductID", ref _ProductID, value); }
        }
        private bool? _IsSearchCategory = true;

        public bool? IsSearchCategory
        {
            get { return _IsSearchCategory; }
            set { base.SetValue("IsSearchCategory", ref _IsSearchCategory, value); }
        }
    }
}
