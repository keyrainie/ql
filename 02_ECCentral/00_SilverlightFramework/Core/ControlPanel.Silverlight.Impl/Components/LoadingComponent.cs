using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Newegg.Oversea.Silverlight.ControlPanel.Impl.Resources;
using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.Core.Components;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class LoadingComponent : ILoadingSpin, IComponent
    {
        private IPageBrowser m_browser;
        private PageTab m_pageTab;
        private BusyIndicator m_loadingSpin;
        private readonly static DependencyProperty LoadingSettingProperty;
        private BusyIndicator Loading
        {
            get
            {
                if (m_loadingSpin == null)
                {
                    m_loadingSpin = new BusyIndicator();
                    m_loadingSpin.DisplayAfter = TimeSpan.FromMilliseconds(400);
                    m_loadingSpin.SetValue(Canvas.ZIndexProperty, 999999);
                }
                return m_loadingSpin;
            }
        }
        private Panel m_defaultContainer;
        private Panel DefaultContainer
        {
            get
            {
                if (LayoutMask.RootVisualContainerCount > 0)
                {
                    return (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
                }
                else
                {
                    if (m_defaultContainer == null)
                    {
                        PageBrowser browser = m_browser as PageBrowser;
                        if (browser.m_contentViewer != null)
                        {
                            m_defaultContainer = VisualTreeHelper.GetParent(browser.m_contentViewer) as Panel;
                        }
                    }

                    return m_defaultContainer;
                }
            }
        }

        static LoadingComponent()
        {
            LoadingSettingProperty = DependencyProperty.Register("LoadingSetting", typeof(LoadingComponent), typeof(DependencyObject), new PropertyMetadata(null));
        }

        public LoadingComponent()
        { }

        internal LoadingComponent(PageTab tab, IPageBrowser browser)
            : this()
        {
            m_pageTab = tab;
            m_browser = browser;
        }

        #region IComponent Members

        public string Name
        {
            get { return "LoadingComponent"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            m_browser = browser;
            m_browser.Navigating += new EventHandler<LoadedMoudleEventArgs>(m_browser_Navigating);
            m_browser.SelectionChanged += new SelectionChangedEventHandler(m_browser_SelectionChanged);
        }

        void m_browser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pageBrowser = sender as FrameworkElement;

            if (e.AddedItems.Count > 0)
            {
                PageTab tab = e.AddedItems[0] as PageTab;
                LoadingComponent component = tab.GetValue(LoadingSettingProperty) as LoadingComponent;

                if (component != null && component.m_loadingSpin != null)
                {
                    component.m_loadingSpin.IsBusy = true;
                }
            }

            if (e.RemovedItems.Count > 0)
            {
                PageTab tab = e.RemovedItems[0] as PageTab;
                LoadingComponent component = tab.GetValue(LoadingSettingProperty) as LoadingComponent;

                if (component != null && component.m_loadingSpin != null)
                {
                    component.m_loadingSpin.IsBusy = false;
                }
            }
        }

        void m_browser_Navigating(object sender, LoadedMoudleEventArgs e)
        {
            var container = sender as IContainer;
            PageTab tab = PageBrowser.GetPageTabByChild(sender as FrameworkElement);
            if (tab != null)
            {
                LoadingComponent component = tab.GetValue(LoadingSettingProperty) as LoadingComponent;

                if (container != null)
                {
                    container.LoadProgress -= new EventHandler<LoadProgressEventArgs>(container_LoadProgress);
                    container.LoadProgress += new EventHandler<LoadProgressEventArgs>(container_LoadProgress);
                }

                if (e.Status == LoadModuleStatus.Begin)
                {
                    if (component == null)
                    {
                        component = this.GetInstance(tab) as LoadingComponent;
                        component.Show();
                    }
                }
                else
                {
                    if (component != null)
                    {
                        component.Hide();
                    }
                }
            }
        }

        void container_LoadProgress(object sender, LoadProgressEventArgs e)
        {
            PageTab tab = PageBrowser.GetPageTabByChild(sender as FrameworkElement);

            if (tab != null && tab.View == null)
            {
                LoadingComponent component = tab.GetValue(LoadingSettingProperty) as LoadingComponent;

                if (component != null && component.m_loadingSpin != null)
                {
                    component.m_loadingSpin.BusyContent = string.Format(MessageResource.LoadingComponent_Loading, e.ProgressPercentage);
                }
            }
        }

        public object GetInstance(TabItem tab)
        {
            return new LoadingComponent(tab as PageTab, m_browser);
        }

        public void Dispose()
        {
            Hide();
        }
        #endregion

        #region ILoadingPin Members

        public void Show()
        {
            Panel container = this.DefaultContainer;
            if (container != null)
            {
                Show(container);

            }
        }

        public void Show(Panel container)
        {
            if (container != null)
            {

                if (m_pageTab != null)
                {
                    m_pageTab.SetValue(LoadingSettingProperty, this);
                }

                if (m_loadingSpin == null)
                {
                    container.Children.Add(this.Loading);
                }
                m_loadingSpin.IsBusy = true;
                m_loadingSpin.Focus();
            }
        }

        public void Hide()
        {
            if (m_loadingSpin != null)
            {
                Panel container = this.DefaultContainer;
                if (container != null)
                {
                    Hide(container);
                }
            }


        }

        public void Hide(Panel container)
        {

            if (container != null && m_loadingSpin != null)
            {
                container.Children.Remove(this.Loading);
                m_loadingSpin.IsBusy = false;
                m_loadingSpin = null;
                if (m_pageTab != null)
                {
                    m_pageTab.ClearValue(LoadingSettingProperty);
                    m_pageTab.Focus();
                }
            }

        }

        public void Clear()
        {
            if (m_loadingSpin != null)
            {
                Panel container = this.DefaultContainer;
                if (container != null)
                {
                    Clear(container);
                }
            }
        }

        public void Clear(Panel container)
        {
            if (container != null && m_loadingSpin != null)
            {
                m_loadingSpin.IsBusy = false;
                m_loadingSpin = null;
                if (m_pageTab != null)
                {
                    m_pageTab.ClearValue(LoadingSettingProperty);
                }
            }
        }
        #endregion

        public bool IsOpen
        {
            get { return this.m_loadingSpin != null; }
        }
    }

    public class LoadingSpin : Control, IDisposable
    {
        protected Border BorderCoverLayer = null;
        private bool m_isStop = false;
        protected Storyboard Storyboard1;
        private object m_lock = new object();

        private List<Ellipse> m_Ellipses;
        private Ellipse m_e1, m_e2, m_e3, m_e4, m_e5, m_e6, m_e7, m_e8, m_e9, m_e10;

        public LoadingSpin()
            : base()
        {
            this.DefaultStyleKey = typeof(LoadingSpin);
            ++Counter;
            this.ApplyTemplate();
            Canvas.SetZIndex(this, 999999);
        }
        public static readonly DependencyProperty SpinColorProperty =
                DependencyProperty.Register("SpinColor", typeof(Brush), typeof(LoadingSpin), null);

        public Brush SpinColor
        {
            get { return (Brush)GetValue(SpinColorProperty); }
            set { SetValue(SpinColorProperty, value); }
        }

        public object SyncObject
        {
            get
            {
                return m_lock;
            }
        }

        public int Counter { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BorderCoverLayer = GetTemplateChild("BorderCoverLayer") as Border;
            Storyboard1 = GetTemplateChild("Storyboard1") as Storyboard;
            m_e1 = GetTemplateChild("E1") as Ellipse;
            m_e2 = GetTemplateChild("E2") as Ellipse;
            m_e3 = GetTemplateChild("E3") as Ellipse;
            m_e4 = GetTemplateChild("E4") as Ellipse;
            m_e5 = GetTemplateChild("E5") as Ellipse;
            m_e6 = GetTemplateChild("E6") as Ellipse;
            m_e7 = GetTemplateChild("E7") as Ellipse;
            m_e8 = GetTemplateChild("E8") as Ellipse;
            m_e9 = GetTemplateChild("E9") as Ellipse;
            m_e10 = GetTemplateChild("E10") as Ellipse;

            m_Ellipses = new List<Ellipse>
            {
                m_e1,m_e2,m_e3,m_e4,m_e5,m_e6,m_e7,m_e8,m_e9,m_e10
            };
            m_Ellipses.ForEach(i => i.Fill = SpinColor);

            if (this.Parent != null && Storyboard1 != null && !m_isStop)
            {
                Storyboard1.Completed += new EventHandler(Storyboard1_Completed);
                Storyboard1.Begin();
            }
        }

        void Storyboard1_Completed(object sender, EventArgs e)
        {
            if (this.Parent != null && Storyboard1 != null && !m_isStop)
            {
                Storyboard1.Begin();
            }
        }

        public void Start()
        {
            if (this.Parent != null && Storyboard1 != null && m_isStop)
            {
                m_isStop = false;
                Storyboard1.Begin();
            }
        }

        public void Stop()
        {
            if (this.Parent != null && Storyboard1 != null && !m_isStop)
            {
                m_isStop = true;
                Storyboard1.Stop();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Storyboard1 != null)
            {
                this.Stop();
                Storyboard1 = null;
            }

            if (this.Parent != null)
            {
                (this.Parent as Panel).Children.Remove(this);
            }
        }
        #endregion
    }
}
