using System;
using System.Collections.Generic;
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

namespace ECCentral.Portal.UI.MKT.Models.Floor
{
    public class FloorSectionBannerVM : FloorSectionItemVM
    {
        private string imageSrc;
        public string ImageSrc
        {
            get { return imageSrc; }
            set { base.SetValue("ImageSrc", ref imageSrc, value); }
        }

        private string bannerText;
        public string BannerText
        {
            get { return bannerText; }
            set { base.SetValue("BannerText", ref bannerText, value); }
        }

        private string linkUrl;
        public string LinkUrl
        {
            get { return linkUrl; }
            set { base.SetValue("LinkUrl", ref linkUrl, value); }
        }

        
    }
}
