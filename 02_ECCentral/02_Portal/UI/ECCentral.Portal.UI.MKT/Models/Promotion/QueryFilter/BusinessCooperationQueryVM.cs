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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class BusinessCooperationQueryVM : ModelBase
    {
        private int? _groupBuyingType;     
        public int? GroupBuyingType
        {
            get { return _groupBuyingType; }
            set
            {
                base.SetValue("GroupBuyingType", ref _groupBuyingType, value);
            }
        }

        private BusinessCooperationStatus? _status;      
        public BusinessCooperationStatus? Status
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
       
        private DateTime? _handleDateFrom;
        /// <summary>
        /// 结束时间范围从
        /// </summary>
        public DateTime? HandleDateFrom
        {
            get { return _handleDateFrom; }
            set
            {
                base.SetValue("HandleDateFrom", ref _handleDateFrom, value);
            }
        }
        private DateTime? _handleDateTo;
        /// <summary>
        /// 结束时间范围到
        /// </summary>
        public DateTime? HandleDateTo
        {
            get { return _handleDateTo; }
            set
            {
                base.SetValue("HandleDateTo", ref _handleDateTo, value);
            }
        }

        private string _telephone;      
        public string Telephone
        {
            get { return _telephone; }
            set
            {
                base.SetValue("Telephone", ref _telephone, value);
            }
        }

        private string _vendorName;
        public string VendorName
        {
            get { return _vendorName; }
            set
            {
                base.SetValue("VendorName", ref _vendorName, value);
            }
        }

        private int? areaSysNo;
        public int? AreaSysNo
        {
            get { return areaSysNo; }
            set
            {
                base.SetValue("AreaSysNo", ref areaSysNo, value);
            }
        }

        public List<KeyValuePair<BusinessCooperationStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<BusinessCooperationStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        public List<CodeNamePair> GroupBuyingTypeList { get; set; }

    }
}
