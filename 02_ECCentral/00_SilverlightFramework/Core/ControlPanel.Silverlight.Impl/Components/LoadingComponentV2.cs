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
using Newegg.Oversea.Silverlight.Controls.Containers;
using System.Windows.Controls.Primitives;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.Resources;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class LoadingComponentV2 : ILoadingSpin
    {
        private readonly static DependencyProperty LoadingProperty;

        private IPageBrowser m_browser;
        private BusyIndicator m_busyIndicator;
        private Popup m_popup;
        private PageTab m_tab;
        private Panel m_container;
        private bool m_isOpen;

        internal Popup Popup
        {
            get
            {
                if (this.m_popup == null)
                {
                    this.m_busyIndicator = new BusyIndicator
                    {
                        DisplayAfter = TimeSpan.FromMilliseconds(400)
                    };
                    this.m_popup = new Popup();
                    this.m_popup.Child = this.m_busyIndicator;
                }
                return this.m_popup;
            }
        }

        internal Panel DefaultContainer
        {
            get
            {
                if (this.m_container == null)
                {
                    var browser = m_browser as PageBrowser;
                    if (browser.m_contentViewer != null)
                    {
                        m_container = VisualTreeHelper.GetParent(browser.m_contentViewer) as Panel;
                    }
                }
                return this.m_container;
            }
        }

        static LoadingComponentV2()
        {
            LoadingProperty = DependencyProperty.Register("LoadingProperty", typeof(LoadingComponentV2), typeof(DependencyObject), new PropertyMetadata(null));
        }

        public LoadingComponentV2() { }

        internal LoadingComponentV2(PageTab tab, IPageBrowser browser)
        {
            this.m_tab = tab;
            this.m_browser = browser;
        }

        #region ILoadingSpin Members

        public bool IsOpen
        {
            get { return this.m_isOpen; }
        }

        public void Show()
        {
            var container = this.DefaultContainer;
            if (container != null)
            {
                this.Show(container);
            }
        }

        public void Show(Panel container)
        {
            if (container != null)
            {
                if (this.m_popup == null)
                {
                    if (!container.Children.Contains(this.Popup))
                    {
                        container.Children.Add(this.Popup);
                    }
                }
                this.m_busyIndicator.Height = container.ActualHeight;
                this.m_busyIndicator.Width = container.ActualWidth;
                this.m_busyIndicator.IsBusy = true;
                this.m_popup.IsOpen = true;

                if (this.m_tab != null)
                {
                    this.m_tab.SetValue(LoadingProperty, this);
                }

                this.m_isOpen = true;
            }
        }

        public void Hide()
        {
            var container = this.DefaultContainer;
            if (container != null)
            {
                this.Hide(container);
            }
        }

        public void Hide(Panel container)
        {
            if (container != null)
            {
                if (container.Children.Contains(this.Popup))
                {
                    container.Children.Remove(this.Popup);
                }

                this.m_popup.IsOpen = false;
                this.m_popup.Child = null;
                this.m_popup = null;

                this.m_busyIndicator.IsBusy = false;
                this.m_busyIndicator = null;

                if (this.m_tab != null)
                {
                    this.m_tab.ClearValue(LoadingProperty);
                }
                this.m_isOpen = false;
            }
        }

        public void Clear()
        {
            var container = this.DefaultContainer;

            if (container != null)
            {
                this.Clear(container);
            }
        }

        public void Clear(Panel container)
        {
            this.Hide(container);
        }

        #endregion



        #region IComponent Members

        public string Name
        {
            get { return "LoadingComponent"; }
        }

        public string Version
        {
            get { return "2.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            this.m_browser = browser;
            this.m_browser.SelectionChanged += Browser_SelectionChanged;
            this.m_browser.Navigating += Browser_Navigating;
        }

        public object GetInstance(TabItem tab)
        {
            return new LoadingComponentV2(tab as PageTab, m_browser);
        }

        public void Dispose()
        {
            this.Hide();
        }

        #endregion

        #region Event Impl

        void Browser_Navigating(object sender, Core.Components.LoadedMoudleEventArgs e)
        {
            var container = sender as IContainer;
            var tab = PageBrowser.GetPageTabByChild(sender as FrameworkElement);

            if (tab != null)
            {
                var component = tab.GetValue(LoadingProperty) as LoadingComponentV2;

                //当下载XAP包的时候，显示下载进度条
                if (container != null)
                {
                    container.LoadProgress -= Contaner_LoadProgress;
                    container.LoadProgress += Contaner_LoadProgress;
                }

                if (e.Status == Core.Components.LoadModuleStatus.Begin)
                {
                    if (component == null)
                    {
                        component = this.GetInstance(tab) as LoadingComponentV2;
                        component.Show(component.DefaultContainer);
                    }
                }
                else if (component != null)
                {
                    component.Hide(component.DefaultContainer);
                }
            }
        }

        void Browser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var browser = sender as FrameworkElement;

            if (e.AddedItems.Count > 0)
            {
                var tab = e.AddedItems[0] as PageTab;
                var component = tab.GetValue(LoadingProperty) as LoadingComponentV2;

                if (component != null)
                {
                    component.Show();
                }
            }

            if (e.RemovedItems.Count > 0)
            {
                var tab = e.RemovedItems[0] as PageTab;
                var component = tab.GetValue(LoadingProperty) as LoadingComponentV2;

                if (component != null)
                {
                    component.Hide();
                }
            }
        }

        void Contaner_LoadProgress(object sender, Core.Components.LoadProgressEventArgs e)
        {
            var tab = PageBrowser.GetPageTabByChild(sender as FrameworkElement);

            if (tab != null && tab.View == null)
            {
                var component = tab.GetValue(LoadingProperty) as LoadingComponentV2;

                if (component != null && component.m_busyIndicator != null)
                {
                    component.m_busyIndicator.BusyContent = string.Format(MessageResource.LoadingComponent_Loading, e.ProgressPercentage);
                }
            }
        }

        #endregion
    }
}
