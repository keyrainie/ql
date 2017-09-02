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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using System.Linq;
namespace ECCentral.Portal.UI.MKT.Models
{
    public class SysConfigVM : ModelBase
    {
        public SysConfigVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
        }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        private string _ConfigType;

        public string ConfigType
        {
            get { return _ConfigType; }
            set { base.SetValue("ConfigType", ref _ConfigType, value); }
        }

    }
    public class SysConfigItemVM : ModelBase
    {
        public string SysConfigType { get; set; }
        public string Key { get; set; }
        private string _Value;

        public string Value
        {
            get { return _Value; }
            set { base.SetValue("Value", ref _Value, value); }
        }
        private bool _IsChecked = false;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { base.SetValue("IsChecked", ref _IsChecked, value); }
        }
    }
}
