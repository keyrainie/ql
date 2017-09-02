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

namespace ECCentral.Portal.UI.MKT.Models.PageType
{
    public class PageTypeQueryVM:ModelBase
    {
        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }
        private string _pageTypeName;
        /// <summary>
        /// 页面类型名称
        /// </summary>
        public string PageTypeName
        {
            get { return _pageTypeName; }
            set
            {
                base.SetValue("PageTypeName", ref _pageTypeName, value);
            }
        }

    }
}
