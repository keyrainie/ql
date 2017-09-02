using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Common.Models
{
    public class ShipTypeAreaUnInfoVM:ModelBase
    {
        public ShipTypeAreaUnInfoVM()
        {
            this.ListWebChannel = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.ListWebChannel.Insert(0, new UIWebChannel { ChannelID = null, ChannelName = ResCommonEnum.Enum_Select });
            
        }
        public int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo",ref _sysNo,value);}
        }
        public int? _shipTypeSysNo;
        [Validate(ValidateType.Required)]
        public int? ShipTypeSysNo
        {
            get { return _shipTypeSysNo; }
            set { SetValue("ShipTypeSysNo", ref _shipTypeSysNo, value); }
        }
        public List<int?> _areaSysNoList;
        public List<int?> AreaSysNoList
        {
            get { return _areaSysNoList; }
            set { SetValue("AreaSysNoList", ref _areaSysNoList, value); }
        }
        public string _companyCode;
        public string CompanyCode
        {
            get { return _companyCode; }
            set { SetValue("CompanyCode",ref _companyCode,value);}
        }
        [Validate(ValidateType.Required)]
        public List<UIWebChannel> ListWebChannel { get; set; }
    }
}
