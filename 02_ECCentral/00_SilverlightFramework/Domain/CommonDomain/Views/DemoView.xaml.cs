using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media.Animation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace Newegg.Oversea.Silverlight.CommonDomain.Views
{
    [View(NeedAccess=false,IsSingleton=false)]
    public partial class DemoView : PageBase
    {
        public DemoView()
        {
            InitializeComponent();
            this.OnLoad += new EventHandler(DemoView_OnLoad);
        }

        void DemoView_OnLoad(object sender, EventArgs e)
        {
            OVSSample1.Page = this;
        }

        //private void TabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (TabControl1 != null && TabControl1.SelectedIndex == 3)
        //    {
        //        GridUnPinContainer.Visibility = System.Windows.Visibility.Visible;
        //        HyperlinkButtonAnchor1.Visibility = System.Windows.Visibility.Visible;
        //        HyperlinkButtonAnchor2.Visibility = System.Windows.Visibility.Visible;
        //        GridPinContainer.Visibility = System.Windows.Visibility.Visible;
        //    }
        //    else if (GridUnPinContainer != null)
        //    {
        //        GridUnPinContainer.Visibility = System.Windows.Visibility.Collapsed;
        //        HyperlinkButtonAnchor1.Visibility = System.Windows.Visibility.Collapsed;
        //        HyperlinkButtonAnchor2.Visibility = System.Windows.Visibility.Collapsed;
        //        GridPinContainer.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //}
    }
}
