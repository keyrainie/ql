using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities;


namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class MiniToolBar : UserControl
    {
        private readonly IHistory m_historyComponent = ComponentFactory.GetComponent<IHistory>();
        private ObservableCollection<HistoryModel> m_history = new ObservableCollection<HistoryModel>();
        private Grid m_HistoryCloseHelpGrid = new Grid { Opacity = 0, Background = new SolidColorBrush(Colors.Transparent) };

        private RequestPanel.RequestPanel m_requestPanel;
   
        public MiniToolBar()
       {
            InitializeComponent();

            m_HistoryCloseHelpGrid.SetValue(Canvas.ZIndexProperty, 50);

            MiniButtonHome.Click += new RoutedEventHandler(MiniButtonHome_Click);
            MiniButtonRefresh.Click += new RoutedEventHandler(MiniButtonRefresh_Click);
            MiniButtonRevoked.Click += new RoutedEventHandler(MiniButtonRevoked_Click);
            MiniButtonHistory.Click += new RoutedEventHandler(MiniButtonHistory_Click);
            MiniButtonRequest.Click += new RoutedEventHandler(MiniButtonRequest_Click);

            PopupHistory.Opened += new EventHandler(PopupHistory_Opened);
            PopupHistory.Closed += new EventHandler(PopupHistory_Closed);

            ListBoxHistory.SelectionChanged += new SelectionChangedEventHandler(ListBoxHistory_SelectionChanged);
            m_HistoryCloseHelpGrid.MouseLeftButtonUp += new MouseButtonEventHandler(m_HistoryCloseHelpGrid_MouseLeftButtonUp);

            if (ComponentFactory.GetComponent<IConfiguration>().GetConfigValue("Framework", "RequestPanelConfig") == null)
            {
                MiniButtonRequest.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void MiniButtonHome_Click(object sender, RoutedEventArgs e)
        {
            var url = CPApplication.Current.DefaultPage;

            CPApplication.Current.Browser.Navigate(url);

            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Home", "ToolBar");
        }

        void m_HistoryCloseHelpGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PopupHistory.IsOpen = false;
        }

        void PopupHistory_Closed(object sender, EventArgs e)
        {
            ((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Remove(m_HistoryCloseHelpGrid);
        }

        void PopupHistory_Opened(object sender, EventArgs e)
        {
            ((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Add(m_HistoryCloseHelpGrid);
        }

        void MiniButtonRequest_Click(object sender, RoutedEventArgs e)
        {
            if (this.m_requestPanel == null)
            {
                this.m_requestPanel = new RequestPanel.RequestPanel(PopupRequestPanel);
                PopupRequestPanel.Child = this.m_requestPanel;
            }

            PopupRequestPanel.IsOpen = !PopupRequestPanel.IsOpen;

            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Request", "ToolBar");
        }

        void MiniButtonHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!PopupHistory.IsOpen)
            {
                var visitedTabViews = m_historyComponent.VisitedTabViews as List<TabView>;

                if (visitedTabViews != null && visitedTabViews.Count > 0)
                {
                    this.ListBoxHistory.ItemsSource = visitedTabViews.Take(10);
                    this.PopupHistory.IsOpen = true;
                }
            }
            else
            {
                PopupHistory.IsOpen = !PopupHistory.IsOpen;
            }
            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "History", "ToolBar");
        }

        private void GetNavigateItems(ref ObservableCollection<AuthMenuItem> list, ObservableCollection<AuthMenuItem> items, AuthMenuItem parent)
        {
            foreach (var authMenuItem in items)
            {
                authMenuItem.Parent = parent;
                if (authMenuItem.Type == AuthMenuItemType.Page)
                {
                    list.Add(authMenuItem);
                }
                if (authMenuItem.Items != null && authMenuItem.Items.Count > 0)
                {
                    GetNavigateItems(ref list, authMenuItem.Items, authMenuItem);
                }
            }
        }


        void MiniButtonRevoked_Click(object sender, RoutedEventArgs e)
        {
            var tabs = m_historyComponent.ClosedTabs as List<Request>;

            if (tabs != null && tabs.Count > 0)
            {
                if (CPApplication.Current != null && CPApplication.Current.CurrentPage != null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Navigate(tabs[0], true);
                    m_historyComponent.RemoveRecoveriedTab();
                }
            }
            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Open Closed Tab", "ToolBar");
        }

        void MiniButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (CPApplication.Current != null && CPApplication.Current.CurrentPage != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Refresh();
            }
            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Refresh", "ToolBar");
        }

        void ListBoxHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopupHistory.IsOpen = false;

            var tabView = ListBoxHistory.SelectedItem as TabView;
            if (tabView != null && tabView.Request != null)
            {
                CPApplication.Current.Browser.Navigate(tabView.Request, true);
            }
        }
    }



    public class HistoryModel
    {
        public string DisplayName { get; set; }

        public string Url { get; set; }
    }
}
