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

namespace ECCentral.Portal.UI.MKT.Models.Floor
{
    public class FloorSectionLinkVM : FloorSectionItemVM
    {
        private string text;
        public string Text
        {
            get { return text; }
            set { base.SetValue("Text", ref text, value); }
        }

        private string linkUrl;
        public string LinkUrl
        {
            get { return linkUrl; }
            set { base.SetValue("LinkUrl", ref linkUrl, value); }
        }

        private bool isHot;
        public bool IsHot
        {
            get { return isHot; }
            set { base.SetValue("IsHot", ref isHot, value); }
        }
    }
}
