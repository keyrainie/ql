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

namespace ECCentral.Portal.Basic.Components.UserControls.AreaPicker
{
    public class AreaQueryVM : ModelBase
    {
        public int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }
        public int? _provinceSysNumber;
        public int? ProvinceSysNumber
        {
            get { return _provinceSysNumber; }
            set { SetValue("ProvinceSysNumber", ref _provinceSysNumber, value); }
        }
        public string _provinceName;
        public string ProvinceName
        {
            get { return _provinceName; }
            set { SetValue("ProvinceName", ref _provinceName, value); }
        }
         public int? _citySysNumber;
        public int? CitySysNumber
        {
            get { return _citySysNumber; }
            set { SetValue("CitySysNumber", ref _citySysNumber, value); }
        }
        public string _cityName;
        public string CityName
        {
            get { return _cityName; }
            set { SetValue("CityName", ref _cityName, value); }
        }
        public int? _districtSysNumber;
        public int? DistrictSysNumber
        {
            get { return _districtSysNumber; }
            set { SetValue("DistrictSysNumber", ref _districtSysNumber, value); }
        }
        public string  _districtName;
        public string  DistrictName
        {
            get { return _districtName; }
            set { SetValue("DistrictName", ref _districtName, value); }
        }


    }
}