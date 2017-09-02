using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class FloorSectionBrandVM : FloorSectionItemVM
    {
        private int brandSysNo;
        public int BrandSysNo
        {
            get { return brandSysNo; }
            set { base.SetValue("BrandSysNo", ref brandSysNo, value); }
        }

        private string imageSrc;
        public string ImageSrc
        {
            get { return imageSrc; }
            set { base.SetValue("ImageSrc", ref imageSrc, value); }
        }

        private string brandText;
        public string BrandText
        {
            get { return brandText; }
            set { base.SetValue("BrandText", ref brandText, value); }
        }

        private string linkUrl;
        public string LinkUrl
        {
            get { return linkUrl; }
            set { base.SetValue("LinkUrl", ref linkUrl, value); }
        }
    }
}
