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
using System.Linq;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Common.Models
{
    public class ShipTypeAreaUnQueryFilterVM:ModelBase
    {
        public ShipTypeAreaUnQueryFilterVM()
        {
            this.PageInfo = new PagingInfo();
            this.ListWebChannel = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.ListWebChannel.Insert(0, new UIWebChannel { ChannelID = null, ChannelName = ResCommonEnum.Enum_All });             
        }
        public PagingInfo PageInfo { get; set; }
        public int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }
        public int? _shipTypeSysNo;
        public int? ShipTypeSysNo
        {
            get { return _shipTypeSysNo; }
            set { SetValue("ShipTypeSysNo", ref _shipTypeSysNo, value); }

        }
        public int? _provinceSysNo;
        public int? ProvinceSysNo
        {
            get { return _provinceSysNo; }
            set { SetValue("ProvinceSysNo", ref _provinceSysNo, value); }
        }
        public int? _citySysNo;
        public int? CitySysNo
        {
            get { return _citySysNo; }
            set { SetValue("CitySysNo", ref _citySysNo, value); }

        }
        public int? _districtSysNo;
        public int? DistrictSysNo
        {
            get { return _districtSysNo; }
            set { SetValue("DistrictSysNo", ref _districtSysNo, value); }
        }
        public int? _areaSysNo;
        public int? AreaSysNo
        {
            get { return _areaSysNo; }
            set { SetValue("AreaSysNo",ref _areaSysNo,value);}
        }
        public string _areaName;
        public string AreaName
        {
            get { return _areaName; }
            set { SetValue("AreaName",ref _areaName,value);}
        }
        public List<UIWebChannel> ListWebChannel { get; set; }
    }
}
