using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;
using System.Windows.Resources;
using System.Xml.Linq;


using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Utilities;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI
{
    [View("Home",true,NeedAccess = false)]
    public partial class HomePage : PageBase
    {
        public HomePage()
        {
            InitializeComponent();

            this.Title = PageResource.LbPageTitle;

            InitMostVisited();

            InitClosedTabs();

        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.Window.DocumentHorizontalScrollBar = ScrollBarVisibility.Disabled;
            this.Window.DocumentVerticalScrollBar = ScrollBarVisibility.Auto;

        }


        public void InitMostVisited()
        {
            stackPanelMostUsed.Children.Clear();

            var mgr = CPApplication.Current.Browser.ComponentCollection.First(p => p.Name == "PageRecordManager") as PageRecordManager;
            var data = mgr.GetHotHitPages(15);
            if (data == null)
            {
                return;
            }

            foreach (var item in data)
            {
                var hyperlinkButton = new HyperlinkButton
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = item.Title,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 0, 7),
                    DataContext = item
                };
                hyperlinkButton.Click += (sender, args) =>
                {
                    HyperlinkButton link = sender as HyperlinkButton;
                    if (link != null)
                    {
                        PageRecordInfo info = link.DataContext as PageRecordInfo;
                        if (info != null)
                        {
                            this.Window.Navigate(info.Url, null,true);            
                            this.Window.EventTracker.TraceEvent(CPApplication.Current.DefaultPage, "Click", "Most Used");
                        }
                    }

                };
                stackPanelMostUsed.Children.Add(hyperlinkButton);
            }
        }


        public void InitClosedTabs()
        {
            stackRecentlyClosed.Children.Clear();

            var mgr = CPApplication.Current.Browser.ComponentCollection.First(p => p.Name == "PageRecordManager") as PageRecordManager;
            var data = mgr.GetClosedTabList(15);
            if (data == null)
            {
                return;
            }

            foreach (var item in data)
            {
                var hyperlinkButton = new HyperlinkButton
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = item.Title,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 0, 7),
                    DataContext = item
                };
                hyperlinkButton.Click += (sender, args) =>
                {
                    HyperlinkButton link = sender as HyperlinkButton;
                    if (link != null)
                    {
                        ClosedTabInfo info = link.DataContext as ClosedTabInfo;
                        if (info != null)
                        {
                            this.Window.Navigate(info.Url, null, true);
                            this.Window.EventTracker.TraceEvent(CPApplication.Current.DefaultPage, "Click", "Recently Closed");
                        }
                    }

                };
                stackRecentlyClosed.Children.Add(hyperlinkButton);
            }
        }

    }
}
