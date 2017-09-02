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

namespace ECCentral.Portal.Basic.Components.Models
{
    public class WebChannelVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }

        private string channelID;
        public string ChannelID
        {
            get
            {
                return channelID;
            }
            set
            {
                base.SetValue("ChannelID", ref channelID, value);
            }
        }
        private string channelName;
        public string ChannelName {
            get
            {
                return channelName;
            }
            set
            {
                base.SetValue("ChannelName", ref channelName, value);
            }
        }

        private string channelType;
        public string ChannelType {
            get
            {
                return channelType;
            }
            set
            {
                base.SetValue("ChannelType", ref channelType, value);
            }
        
        }

    }
}
