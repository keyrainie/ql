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
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Linq;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class FPCheckVM : ModelBase
    {
        public FPCheckVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
         //   this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            EntityList = new ObservableCollection<FPCheckEntityVM>();
        }

        private string _webChannel;

        public string WebChannel
        {
            get { return _webChannel; }
            set { base.SetValue("WebChannel", ref _webChannel, value); }
        }

        public List<UIWebChannel> WebChannelList { get; set; }

        ObservableCollection<FPCheckEntityVM> EntityList { get; set; }

    }

    public class FPCheckEntityVM : ModelBase
    {
        public int SysNo { get; set; }
        private int _status;

        public int Status
        {
            get { return _status; }
            set { base.SetValue("Status", ref _status, value); }
        }
        public string Description { get; set; }
        public string LastEditUserName { get; set; }
        public string LastEditDate { get; set; }
        public string CheckType { get; set; }
        private string _url;

        public string Url
        {
            get
            {
                switch (CheckType.Trim())
                {
                    case "CH":
                        return ConstValue.Customer_FPCheck_CH;
                    case "CC":
                        return ConstValue.Customer_FPCheck_CC;
                    default:
                        return string.Empty;
                }
            }
            set { _url = value; }
        }
        public Visibility HyperlinkButtonVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Url))
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        public Visibility TextBlockVisibility
        {
            get
            {
                if (!string.IsNullOrEmpty(Url))
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        public string ChannelID { get; set; }
    }

}
