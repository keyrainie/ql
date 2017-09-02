using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
//using System.Windows.Interactivity;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Collections.Specialized;

namespace Newegg.Oversea.Silverlight.Controls.Containers
{
    [ScriptableType]
    public class PageBrowser : TabControl, IPageBrowser
    {
        public event EventHandler<StatusChangedEventArgs> WindowStatusChanged;
        public event EventHandler<LoadedMoudleEventArgs> Navigating;
        public event ApplyTemplateEventHandler ApplyTemplateHandle;
        public event EventHandler NavigateCompleted;
        public event EventHandler TabClosing;

        protected TabPanel m_panel;
        protected ScrollViewer m_pageScroll;
        internal ScrollViewer m_contentViewer;
        protected ContentPresenter m_content;
        protected StackPanel m_panelNavgigation;
        private ScrollBar h_scrollbar;
        private ScrollBar v_scrollbar;
        private DropTarget m_contentDropTarget;
        protected ContentPresenter m_contentPageDescription;

        public static readonly DependencyProperty ScrollStepProperty;
        public static readonly DependencyProperty DefaultPageProperty;
        public static readonly DependencyProperty ErrorPageProperty;
        public static readonly DependencyProperty LoadingPageProperty;
        public static readonly DependencyProperty DocumentHorizontalScrollBarProperty;
        public static readonly DependencyProperty DocumentVerticalScrollBarProperty;
        public static readonly DependencyProperty MaxPageTabTotalProperty;
        public static readonly DependencyProperty ComponentCollectionProperty;
        public static readonly DependencyProperty AllowAutoResetScrollBarProperty;
        public static readonly DependencyProperty ModelProperty;
        //  internal static readonly DependencyProperty PopupBoxListProperty;
        internal static readonly DependencyProperty IsLayoutSetPageProperty;

        static PageBrowser()
        {
            ScrollStepProperty = DependencyProperty.Register("ScrollStep", typeof(double), typeof(PageBrowser), new PropertyMetadata(20D));
            DefaultPageProperty = DependencyProperty.RegisterAttached("DefaultPage", typeof(string), typeof(PageBrowser), new PropertyMetadata(CPApplication.Current.DefaultPage));
            ErrorPageProperty = DependencyProperty.RegisterAttached("ErrorPage", typeof(string), typeof(PageBrowser), new PropertyMetadata("/PageBrowser/Error"));
            DocumentHorizontalScrollBarProperty = DependencyProperty.RegisterAttached("DocumentHorizontalScrollBar", typeof(ScrollBarVisibility), typeof(PageBrowser), new PropertyMetadata(ScrollBarVisibility.Auto));
            DocumentVerticalScrollBarProperty = DependencyProperty.RegisterAttached("DocumentVerticalScrollBar", typeof(ScrollBarVisibility), typeof(PageBrowser), new PropertyMetadata(ScrollBarVisibility.Auto));
            MaxPageTabTotalProperty = DependencyProperty.Register("MaxPageTab", typeof(int), typeof(PageBrowser), new PropertyMetadata(20));
            ComponentCollectionProperty = DependencyProperty.Register("ComponentCollection", typeof(ComponentCollection), typeof(PageBrowser), null);
            AllowAutoResetScrollBarProperty = DependencyProperty.RegisterAttached("AllowAutoResetScrollBar", typeof(bool), typeof(PageBrowser), new PropertyMetadata(false));
            //PopupBoxListProperty = DependencyProperty.RegisterAttached("PopupBoxList", typeof(List<Panel>), typeof(PageBrowser), null);
            ModelProperty = DependencyProperty.Register("Model", typeof(PageBrowserModel), typeof(PageBrowser), new PropertyMetadata(PageBrowserModel.MultiPage));
            IsLayoutSetPageProperty = DependencyProperty.Register("IsLayoutSetPage", typeof(bool), typeof(IPage), new PropertyMetadata(false));
        }

        public PageBrowser()
            : base()
        {
            this.DefaultStyleKey = typeof(PageBrowser);
            this.SelectionChanged += new SelectionChangedEventHandler(PageBrowser_SelectionChanged);
            this.ComponentCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ComponentCollection_CollectionChanged);
            ComponentFactory.GetComponent<IModuleManager>().Add(new ModuleInfo("PageBrowser", typeof(PageBrowser).Assembly));
            Application.Current.UnhandledException += new EventHandler<ApplicationUnhandledExceptionEventArgs>(Current_UnhandledException);
            Configure();

            Register();

            if (!Application.Current.IsRunningOutOfBrowser)
                HtmlPage.RegisterScriptableObject("PageBrowser", this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_panel = this.GetTemplateChild("TabPanelTop") as TabPanel;
            m_pageScroll = this.GetTemplateChild("PageScroll") as ScrollViewer;
            m_contentViewer = this.GetTemplateChild("ContentViewer") as ScrollViewer;
            m_content = this.GetTemplateChild("ContentTop") as ContentPresenter;
            m_panelNavgigation = this.GetTemplateChild("panelNavigation") as StackPanel;
            m_contentDropTarget = this.GetTemplateChild("ContentDropTarget") as DropTarget;
            //if (this.m_contentDropTarget != null)
            //{
            //    m_contentDropTarget.DragSourceDropped += ContentDropTarget_DragSourceDropped;
            //}

            ComboBox cmbTabs = this.GetTemplateChild("cmbTabs") as ComboBox;
            m_contentPageDescription = this.GetTemplateChild("contentPageDescription") as ContentPresenter;

            if (cmbTabs != null)
            {
                cmbTabs.DropDownOpened += new EventHandler(delegate(object sender, EventArgs e)
                {
                    ContentControl item;

                    for (int i = cmbTabs.Items.Count - 1; i > 0; i--)
                    {
                        cmbTabs.Items.RemoveAt(i);
                    }

                    cmbTabs.SelectedItem = null;

                    foreach (PageTab tab in this.Items)
                    {
                        item = new ContentControl();
                        item.Foreground = new SolidColorBrush(Colors.White);
                        item.Content = tab.Header.ToString().Length <= 20
                                           ? tab.Header
                                           : tab.Header.ToString().Substring(0, 20) + "...";

                        if (tab.Header.ToString().Length > 20)
                            ToolTipService.SetToolTip(item, tab.Header);

                        cmbTabs.Items.Add(item);
                    }
                });

                cmbTabs.SelectionChanged += new SelectionChangedEventHandler(delegate(object sender, SelectionChangedEventArgs e)
                {
                    if (e.AddedItems.Count > 0)
                    {
                        int index = cmbTabs.Items.IndexOf(e.AddedItems[0]);

                        if (index > 0)
                        {
                            this.SelectedItem = this.Items[index - 1];

                        }
                        else
                        {
                            //close all tabs,then navigate to defalut page
                            Confirm(null, MessageResource.PageBrowser_Confirm_CloseAllTabs, new ResultHandler(delegate(object o, ResultEventArgs args)
                            {
                                if (args.DialogResult == DialogResultType.OK)
                                {
                                    CloseAllTabs();
                                }
                            }), ButtonType.YesNo, null);
                            return;
                        }
                    }
                });
            }

            if (m_contentViewer != null)
            {
                m_contentViewer.Loaded += new RoutedEventHandler(m_contentViewer_Loaded);
                m_contentViewer.SizeChanged += new SizeChangedEventHandler(m_contentViewer_SizeChanged);
            }
            if (m_content != null)
            {
                m_content.HorizontalAlignment = HorizontalAlignment.Stretch;
                m_content.VerticalAlignment = VerticalAlignment.Stretch;
            }

            if (this.SelectedItem != null)
            {
                SetContentViewer(this.SelectedItem as PageTab);
            }

            if (this.ApplyTemplateHandle != null)
            {
                this.ApplyTemplateHandle(this, new ApplyTemplateEventArgs(this.GetTemplateChild));
            }

            if (!Application.Current.IsRunningOutOfBrowser)
            {
                if (string.IsNullOrEmpty(HtmlPage.Window.CurrentBookmark) ||
                    (!string.IsNullOrEmpty(HtmlPage.Window.CurrentBookmark) && HtmlPage.Window.CurrentBookmark.ToLower() == "/pagebrowser/error"))
                {
                    this.Navigate(this.DefaultPage);
                }
                else
                {
                    this.Navigate(HtmlPage.Window.CurrentBookmark);
                }
                HtmlPage.Window.Eval("$(window).hashchange();");
            }
            else
            {
                this.Navigate(this.DefaultPage);
            }

            PageBrowserTabPanel thePBTabPanel = null;
            thePBTabPanel = m_panel as PageBrowserTabPanel;
            if (thePBTabPanel != null)
            {
                thePBTabPanel.PageBrowser = this;
            }

        }

        #region Property
        public int MaxPageTabTotal
        {
            get
            {
                return (int)this.GetValue(PageBrowser.MaxPageTabTotalProperty);
            }

            set
            {
                this.SetValue(PageBrowser.MaxPageTabTotalProperty, value);
            }
        }

        public string DefaultPage
        {
            get
            {
                return this.GetValue(PageBrowser.DefaultPageProperty) as string;
            }
            set
            {
                this.SetValue(PageBrowser.DefaultPageProperty, value);
            }
        }

        public string ErrorPage
        {
            get
            {
                return this.GetValue(PageBrowser.ErrorPageProperty) as string;
            }
            set
            {
                this.SetValue(PageBrowser.ErrorPageProperty, value);
            }
        }

        public double ScrollStep
        {
            get
            {
                return (double)this.GetValue(PageBrowser.ScrollStepProperty);
            }

            set
            {
                this.SetValue(PageBrowser.ScrollStepProperty, value);
            }
        }

        public IMessageBox MessageBox
        {
            get
            {
                return GetComponent<IMessageComponent>().GetInstance(this.SelectedItem as TabItem) as IMessageBox;
            }
        }

        public IAuth AuthManager
        {
            get
            {
                return GetComponent<IAuth>();
            }
        }

        public ILoadingSpin LoadingSpin
        {
            get
            {
                return this.GetComponent<ILoadingSpin>();
            }
        }

        public ILog Logger
        {
            get
            {
                return this.GetComponent<ILog>();
            }
        }

        public IConfiguration Configuration
        {
            get
            {
                return this.GetComponent<IConfiguration>();
            }
        }

        public ICache Cacher
        {
            get
            {
                return this.GetComponent<ICache>();
            }
        }

        public IFaultHandle FaultHandle
        {
            get
            {
                return GetComponent<IFaultHandle>().GetInstance(this.SelectedItem as TabItem) as IFaultHandle;
            }
        }

        public INotificationBox NotificationBox
        {
            get
            {
                return GetComponent<INotificationBox>().GetInstance(this.SelectedItem as TabItem) as INotificationBox;
            }
        }

        public IUserProfile Profile
        {
            get
            {
                return GetComponent<IUserProfile>();
            }
        }

        public IMail Mailer
        {
            get
            {
                return GetComponent<IMail>();
            }
        }

        public double DocumentHeight
        {
            get
            {
                double height = double.NaN;

                if (this.SelectedPage != null)
                {
                    height = (this.SelectedPage as UserControl).ActualHeight;
                }

                return height;
            }

            set
            {
                if (this.SelectedPage != null)
                {
                    (this.SelectedPage as UserControl).Height = value;
                }
            }
        }

        public double DocumentWidth
        {
            get
            {
                double width = double.NaN;

                if (this.SelectedPage != null)
                {
                    width = (this.SelectedPage as UserControl).ActualWidth;
                }

                return width;
            }

            set
            {
                if (this.SelectedPage != null)
                {
                    (this.SelectedPage as UserControl).Width = value;
                }
            }
        }

        public double WindowHeight
        {
            get
            {
                double height = double.NaN;
                if (m_contentViewer != null)
                {
                    height = m_contentViewer.ViewportHeight;
                }

                return height;
            }
        }

        public double WindowWidth
        {
            get
            {
                double width = double.NaN;
                if (m_contentViewer != null)
                {
                    width = m_contentViewer.ViewportWidth;
                }

                return width;
            }
        }

        public bool Status
        {
            get { throw new NotImplementedException(); }
        }

        public ScrollBarVisibility DocumentHorizontalScrollBar
        {
            get
            {
                return GetDocumentHorizontalScrollBar(this.SelectedItem as PageTab);
            }

            set
            {
                SetDocumentHorizontalScrollBar(this.SelectedItem as PageTab, value);
            }
        }

        public ScrollBarVisibility DocumentVerticalScrollBar
        {
            get
            {
                return GetDocumentVerticalScrollBar(this.SelectedItem as PageTab);
            }

            set
            {
                SetDocumentVerticalScrollBar(this.SelectedItem as PageTab, value);
            }
        }

        public ComponentCollection ComponentCollection
        {
            get
            {
                ComponentCollection collection = this.GetValue(ComponentCollectionProperty) as ComponentCollection;

                if (collection == null)
                {
                    collection = new ComponentCollection();
                    this.SetValue(ComponentCollectionProperty, collection);
                }

                return collection;
            }

            set
            {
                this.SetValue(ComponentCollectionProperty, value);
            }
        }

        internal IModuleManager ModuleManger
        {
            get
            {
                return ComponentFactory.GetComponent<IModuleManager>();
            }
        }

        public IPage SelectedPage
        {
            get
            {
                PageTab tab;
                if (this.SelectedItem != null && (tab = this.SelectedItem as PageTab) != null)
                {
                    return tab.View as IPage;
                }

                return null;
            }
        }

        public IList<IPage> OpenPages
        {
            get
            {
                IList<IPage> list = new List<IPage>();
                foreach (var item in this.Items)
                {
                    PageTab tab = item as PageTab;
                    if (tab != null)
                    {
                        IPage page = tab.View as IPage;
                        if (page != null)
                        {
                            list.Add(page);
                        }
                    }
                }
                return list;
            }
        }

        public bool AllowAutoResetScrollBar
        {
            get
            {
                if (this.SelectedPage != null)
                {
                    return this.SelectedPage.Context.Window.AllowAutoResetScrollBar;
                }
                return true;
            }
            set
            {
                if (this.SelectedPage != null)
                {
                    this.SelectedPage.Context.Window.AllowAutoResetScrollBar = value;
                }
            }
        }

        public PageBrowserModel Model
        {
            get
            {
                return (PageBrowserModel)this.GetValue(ModelProperty);
            }
            set
            {
                if (PageBrowserModel.SinglePage == value)
                {
                    Queue<PageTab> list = new Queue<PageTab>();
                    PageTab tab = null;
                    foreach (PageTab item in this.Items)
                    {
                        if (!item.IsSelected)
                        {
                            list.Enqueue(item);
                        }
                        else
                        {
                            tab = item;
                        }
                    }

                    if (tab != null && tab.View != null)
                    {
                        SetPageTab(tab, tab.View as IPage);
                    }

                    while (list.Count > 0)
                    {
                        tab = list.Dequeue();
                        this.Close(tab);
                    }
                }
                VisualStateManager.GoToState(this, value.ToString(), false);
                this.SetValue(ModelProperty, value);
            }
        }

        public bool EnableConfirmOnClose { get; set; }

        public string ConfirmContent { get; set; }

        #endregion

        #region IPageBrowser Members

        public void Navigate(string url)
        {
            this.Navigate(url, null);
        }

        public void Navigate(string url, object args)
        {
            this.Navigate(url, args, this.Model == PageBrowserModel.MultiPage ? true : false);
        }

        public void Navigate(string url, object args, bool isNewTab)
        {
            Navigate(url, args, isNewTab, null);
        }

        public void Navigate(Request request, bool isNewTab)
        {
            Navigate(request, isNewTab, null);
        }

        public void Navigate(Request request, bool isNewTab, TabItem targetTab)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            //Update by Aaron: 如果传入的URL地址前后包含空格, 会导致打开多个相同的页面；
            if (!string.IsNullOrEmpty(request.URL))
            {
                request.URL = request.URL.Trim();
            }

            PageTab tab = (targetTab != null) ? targetTab as PageTab : this.SelectedItem as PageTab;

            if (IsSingleton(request, isNewTab))
            {
                return;
            }

            OpenPage(request, isNewTab, tab);
        }

        public void Navigate(string url, object args, bool isNewTab, TabItem targetTab)
        {
            Navigate(url, args, isNewTab, targetTab, true, false);
        }

        private void Navigate(string url, object args, bool isNewTab, TabItem targetTab, bool isSLTransfer, bool isRefresh)
        {
            if ((url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                && !url.StartsWith(CPApplication.Current.PortalBaseAddress, StringComparison.OrdinalIgnoreCase))
            {
                Utilities.UtilityHelper.OpenPage(url);
                return;
            }

            isNewTab = this.Model == PageBrowserModel.SinglePage ? false : isNewTab;
            if (string.IsNullOrEmpty(url))
            {
                this.Navigate("about:blank", null, false);
                return;
            }
            else
            {
                //Update by Aaron: 如果传入的URL地址前后包含空格, 会导致打开多个相同的页面；
                url = url.Trim();
            }

            if (url.ToLower() == "about:blank")
            {
                url = this.DefaultPage;
            }

            //1 get new url
            PageTab tab = (targetTab != null) ? targetTab as PageTab : this.SelectedItem as PageTab;

            Request request = new Request(url)
            {
                UserState = args
            };

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            //1.1 check current view exsit 'isSingleton'
            if (IsSingleton(request, isNewTab) && !isRefresh)
            {
                return;
            }

            bool isOpened = OpenPage(request, isNewTab, tab);

            if (isOpened && isSLTransfer)
            {
                RiseUrlChange(request);
            }
        }

        [ScriptableMember]
        public void NavigateFromScript(string url)
        {
            if (this.SelectedPage == null || this.SelectedPage.Context.Request.URL != url)
            {
                try
                {
                    Navigate(url, null, this.Model == PageBrowserModel.MultiPage ? true : false, null, false, false);
                }
                catch (PageException ex)
                {
                    Current_UnhandledException(this, new ApplicationUnhandledExceptionEventArgs(ex, true));
                }
            }
        }

        private void RiseUrlChange(Request request)
        {
            if (!Application.Current.IsRunningOutOfBrowser)
            {
                try
                {
                    HtmlPage.Window.Eval(string.Format(@"
                $(window).unbind(""hashchange"",window.hashHandle);
                window.location.href = ""{0}"";
                 $(window).bind(""hashchange"",window.hashHandle);
                $(window).hashchange();
            ", request.URL.Replace("\"", "\\\"")));
                }
                catch (Exception ex)
                {
                    ComponentFactory.Logger.LogError(ex, new object[] { string.Format(@"
                $(window).unbind(""hashchange"",window.hashHandle);
                window.location.href = ""{0}"";
                 $(window).bind(""hashchange"",window.hashHandle);
                $(window).hashchange();
            ", request.URL.Replace("\"", "\\\"")) });
                }
            }
        }

        private bool OpenPage(Request request, bool isNewTab, PageTab tab)
        {
            IContainer container;
            //2 open page
            if (isNewTab)
            {
                //2.11 check more than maxpagetab
                if (this.Items.Count + 1 > this.MaxPageTabTotal)
                {
                    Alert(MessageResource.PageBrowser_Error_MoreThanMaxPageTabTotal, MessageType.Warning);
                    return false;
                }
                if (this.SelectedItem != null && (tab = this.SelectedItem as PageTab) != null && tab.View != null)
                {
                    request.RefererRequest = tab.View.Context.Request;
                }
                //2.12 open the page in new tab
                container = new GridContainer();

                container.LoadModule += new EventHandler<LoadedMoudleEventArgs>(OnNavigating);
                tab = new PageTab(string.Empty, container);
                Add(tab);
                this.SelectedItem = tab;
            }
            else
            {
                if (this.Items.Count == 0 && tab == null)
                {
                    tab = new PageTab(string.Empty, new GridContainer());
                    this.Items.Add(tab);
                }
                //2.2 open the page in the current tab
                container = (tab.Content as IContainer);

                if (container.Children.Count > 0)
                {
                    IPage page;
                    foreach (UIElement element in container.Children)
                    {
                        page = element as IPage;

                        if (page != null)
                        {
                            page.Context.OnPageClose(this, new PageCloseEventArgs()); ;
                        }
                    }
                }

                if (tab.View != null && tab.View.Context.Window != null)
                {
                    tab.View.Context.Window.Dispose();
                    request.RefererRequest = tab.View.Context.Request;
                }

                container.LoadModule -= new EventHandler<LoadedMoudleEventArgs>(OnNavigating);
                container.LoadModule += new EventHandler<LoadedMoudleEventArgs>(OnNavigating);
                container.Children.Clear();
            }

            //3 load page
            tab.Tag = request;
            (tab.Content as IContainer).Load(request);
            return true;
        }

        public void Back()
        {
            this.Back(null);
        }

        public void Back(PageTab tab)
        {
            tab = tab == null ? this.SelectedItem as PageTab : tab;
            IHistory history = this.GetComponent<IHistory>().GetInstance(tab) as IHistory;
            Request request;

            history.Previous();
            if ((request = history.Current) != null)
            {
                OpenPage(request, false, tab);
            }
        }

        public void Forward()
        {
            this.Forward(null);
        }

        public void Forward(PageTab tab)
        {
            tab = tab == null ? this.SelectedItem as PageTab : tab;
            IHistory history = this.GetComponent<IHistory>().GetInstance(tab) as IHistory;
            Request request;

            history.Next();
            if ((request = history.Current) != null)
            {
                OpenPage(request, false, tab);
            }
        }

        public void Close()
        {
            if (this.SelectedItem != null)
            {
                this.Close(this.SelectedItem as PageTab);
            }
        }

        public void Refresh()
        {
            Refresh(this.SelectedItem as PageTab);
        }

        public void Refresh(PageTab tab)
        {
            tab = (tab == null) ? this.SelectedItem as PageTab : tab;

            if (tab.View != null)
            {
                tab.View.Context.Window.MessageBox.Clear();
                Navigate(tab.View.Context.Request.URL, tab.View.Context.Request.UserState, false, null, true, true);
            }
        }

        #region Alert, Confirm, ShowDialog

        public void Alert(string content)
        {
            this.Alert(content, MessageType.Information);
        }

        public void Alert(string title, string content)
        {
            this.Alert(title, content, MessageType.Information);
        }

        public void Alert(string content, MessageType type)
        {
            Alert(null, content, type);
        }

        public void Alert(string title, string content, MessageType type)
        {
            Alert(title, content, type, null);
        }

        public void Alert(string title, string content, MessageType type, ResultHandler handle)
        {
            Alert(title, content, type, handle, null);
        }

        public void Alert(string title, string content, MessageType type, ResultHandler handle, Panel container)
        {
            // List<Panel> list = null;
            IAlert form;
            PageTab tab = null;

            if (string.IsNullOrEmpty(title))
            {
                title = null;
            }

            if (container == null)
            {
                container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            }
            else
            {
                if (container.Parent is PageTab)
                {
                    tab = container.Parent as PageTab;
                    //list = GetPopupBoxList(tab);
                    container = GetPanelContainerByPageTabContent(container);
                }
            }

            form = CreateAlert();
            form.Alert(title, content, type, handle, container);

        }

        public void Confirm(string content, ResultHandler callback)
        {
            this.Confirm(string.Empty, content, callback);
        }

        public void Confirm(string title, string content, ResultHandler callback)
        {
            Confirm(title, content, callback, ButtonType.OKCancel);
        }

        public void Confirm(string title, string content, ResultHandler callback, ButtonType type)
        {
            Confirm(title, content, callback, type, null);
        }

        public void Confirm(string title, string content, ResultHandler callback, ButtonType type, Panel container)
        {
            //  List<Panel> list = null;
            IConfirm form;
            PageTab tab = null;

            if (container == null)
            {
                container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            }
            else
            {
                if (container.Parent is PageTab)
                {
                    tab = container.Parent as PageTab;
                    //   list = GetPopupBoxList(tab);
                    container = GetPanelContainerByPageTabContent(container);
                }
            }

            form = CreateConfirm();
            form.Confirm(title, content, callback, type, container);

        }

        public IDialog ShowDialog(string title, FrameworkElement content)
        {
            return this.ShowDialog(title, content, null);
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback)
        {
            return this.ShowDialog(title, content, callback, new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size)
        {
            return ShowDialog(title, content, callback, size, null);
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size, Panel container)
        {
            //  List<Panel> list = null;
            IDialog form, result;
            PageTab tab = null;

            if (container == null)
            {
                container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            }
            else
            {

                if (container.Parent is PageTab)
                {
                    tab = container.Parent as PageTab;
                    //       list = GetPopupBoxList(tab);
                    container = GetPanelContainerByPageTabContent(container);
                }
            }

            form = CreateDialog();
            result = form.ShowDialog(title, content, callback, size, container);

            return result;
        }

        public IDialog ShowDialog(string title, string url, ResultHandler callback)
        {
            return this.ShowDialog(title, url, callback, new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        public IDialog ShowDialog(string title, string url, ResultHandler callback, Size size)
        {
            return ShowDialog(title, url, callback, size, null);
        }

        public IDialog ShowDialog(string title, string url, ResultHandler callback, Size size, Panel container)
        {
            //List<Panel> list = null;
            IDialog form, result;
            PageTab tab = null;

            if (container == null)
            {
                container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            }
            else
            {
                if (container.Parent is PageTab)
                {
                    tab = container.Parent as PageTab;
                    // list = GetPopupBoxList(tab);
                    container = GetPanelContainerByPageTabContent(container);
                }
            }

            form = CreateDialog();
            result = form.ShowDialog(title, url, callback, size, container, this);

            return result;
        }

        #endregion

        public IComponent GetComponentByName(string name)
        {
            foreach (IComponent item in this.ComponentCollection)
            {
                if (item.Name.ToLower() == name.ToLower())
                {
                    return item;
                }
            }

            return null;
        }

        public T GetComponent<T>() where T : IComponent
        {
            Type type = typeof(T);
            Type[] interfaces;
            foreach (IComponent item in this.ComponentCollection)
            {
                interfaces = item.GetType().GetInterfaces();

                if (interfaces != null)
                {
                    foreach (Type interfaceType in interfaces)
                    {
                        if (interfaceType == type)
                        {
                            return (T)item;
                        }
                    }
                }
            }



            return default(T);

        }

        public void Dispose()
        { }

        public void ResetPageViewer()
        {
            this.ResetPageViewer(this.SelectedItem as PageTab);
        }

        internal void ResetPageViewer(PageTab tab)
        {
            if (tab != null && m_contentViewer != null)
            {
                m_contentViewer.ScrollToVerticalOffset(0d);
                m_contentViewer.ScrollToHorizontalOffset(0d);
            }
        }

        public void CloseDialog(object UserState)
        {
            if (DialogClosed != null)
            {
                DialogClosed(this, new ResultEventArgs { Data = UserState });
            }
        }

        public void CloseDialog(object UserState, bool isForce)
        {
            if (DialogClosed != null)
            {
                DialogClosed(this, new ResultEventArgs { Data = UserState, isForce = true });
            }
        }

        public void ClosingDialog(ClosingEventArgs args)
        {
            if (DialogClosing != null)
            {
                DialogClosing(this, args);
            }
        }

        public event EventHandler<ResultEventArgs> DialogClosed;

        public event EventHandler<ClosingEventArgs> DialogClosing;

        #endregion

        #region IWindow Members

        public IEventTracker EventTracker
        {
            get
            {
                return this.GetComponent<IEventTracker>();
            }
        }

        #endregion

        #region Private & Public & Protected Methods

        private void Register()
        {
            Application.Current.RootVisual.KeyDown += new KeyEventHandler(RootVisual_KeyDown);
        }

        private void Close(PageTab tab, bool allowChangeSelectedItem)
        {
            this.Close(tab, allowChangeSelectedItem, false);
        }

        private void Close(PageTab tab, bool allowChangeSelectedItem, bool isForce)
        {
            //1 trigger the page's close event when the tab closing
            if (tab.View != null && !isForce)
            {
                var args = new PageCloseEventArgs();
                tab.View.Context.OnPageClose(this, args);
                if (args.Cancel)
                {
                    return;
                }
                tab.View.Context.Window.Dispose();
            }

            if (this.TabClosing != null)
            {
                TabClosing(tab, new EventArgs());
            }

            //当只有一个TabPage的时候，执行关闭，让它转到DefaultPage去；
            if (this.Items.Count == 1)
            {
                this.Navigate(this.DefaultPage, null, false);
                return;
            }

            var container = tab.Content as IContainer;
            var index = this.Items.IndexOf(tab);


            //2 remove the tab

            container.Children.Clear();
            //ColsePopupBoxByPageTab(tab);
            if (index > 0 && tab.IsSelected)
            {
                base.SelectedIndex = this.Items.IndexOf(tab);
            }
            this.Items.Remove(tab);

            if (allowChangeSelectedItem)
            {
                object selectedItem = this.SelectedItem;
                this.SelectedItem = null;
                this.SelectedItem = selectedItem;
            }
        }

        private void CloseOtherTabs(PageTab tab)
        {
            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                var item = this.Items[i] as PageTab;
                if (item != tab)
                {
                    this.Close(item, false);
                }
            }
        }

        private void CloseAllTabs()
        {
            if (this.Items.Count > 1)
            {
                this.SelectedItem = this.Items[0];
            }

            for (int i = this.Items.Count - 1; i > 0; i--)
            {
                this.Close(this.Items[i] as PageTab, false);
            }

            if (this.Items.Count == 1)
            {
                this.SelectedItem = null;
                this.SelectedItem = this.Items[0];
                this.Navigate(this.DefaultPage, null, false);
            }
        }

        private void SetPage(PageTab tab, bool isCallPageLoad)
        {
            IPage page = null;

            if (tab.Parent != null)
            {
                if (tab.IsSelected)
                {
                    if (tab.View != null &&
                    (page = tab.View as IPage) != null)
                    {
                        if (isCallPageLoad)
                        {
                            page.Context.OnPageLoad(page, new EventArgs());
                            this.OnNavigated(tab, new EventArgs());
                        }
                    }
                    else
                    {
                        DependencyObject element;
                        if ((element = page as DependencyObject) != null)
                        {
                            element.SetValue(IsLayoutSetPageProperty, true);
                        }
                    }
                    SetContentViewer(tab);
                }
                else
                {
                    DependencyObject element;
                    if (tab.View != null &&
                        (page = tab.View as IPage) != null && (element = page as DependencyObject) != null)
                    {
                        element.SetValue(IsLayoutSetPageProperty, true);
                    }
                }
                if (tab.Header == null)
                {
                    tab.Header = MessageResource.PageBrowser_Unknown;
                }
                if (!Application.Current.IsRunningOutOfBrowser)
                {
                    HtmlPage.Document.SetProperty("title", string.Format("{0} - {1}",
                        CPApplication.Current.Application.Name, tab.Header.ToString()));
                }
            }
        }

        private void RegisterDependencyProperty(string propertyName, FrameworkElement element, PropertyChangedCallback callback)
        {
            Binding bind = new Binding(propertyName) { Source = element };
            var prop = DependencyProperty.RegisterAttached("ScrollBar" + propertyName, typeof(object), element.GetType(), new PropertyMetadata(callback));
            element.SetBinding(prop, bind);
        }

        protected virtual void Configure()
        {
            //1 register components
            this.ComponentCollection.Add(new MessageComponent());
            this.ComponentCollection.Add(new PageRecordManager());
            this.ComponentCollection.Add(new LoadingComponent());
            this.ComponentCollection.Add(new FaultHandleComponent());
            this.ComponentCollection.Add(new History());
            this.ComponentCollection.Add(new NotificationBox());
            this.ComponentCollection.Add(new PopupBox());
            //this.ComponentCollection.Add(new Dialog());
            //this.ComponentCollection.Add(new AlertWindow());
            

            //2 load components from xaml
            foreach (KeyValuePair<Type, object> item in ComponentFactory.s_list)
            {
                this.ComponentCollection.Add(item.Value as IComponent);
            }
        }

        protected virtual IAlert CreateAlert()
        {
            return this.GetComponent<IAlert>();
        }

        protected virtual IConfirm CreateConfirm()
        {
            return this.GetComponent<IConfirm>();
        }

        protected virtual IDialog CreateDialog()
        {
            return this.GetComponent<IDialog>();
        }

        protected virtual bool IsSingleton(Request request, bool isNewTab)
        {
            IViewInfo viewInfo;
            PageTab singlePageTab = null;
            Request tagRequest;

            foreach (PageTab item in this.Items)
            {

                if (item.View != null && item.View.Context.Request.ModuleName.ToLower() == request.ModuleName.ToLower())
                {
                    if (request.ViewName.ToLower() == item.View.Context.Request.ViewName.ToLower() &&
                            (viewInfo = item.View.Context.Request.ModuleInfo.GetViewInfoByName(request.ViewName)) != null &&
                                viewInfo.IsSingleton)
                    {
                        if (viewInfo.SingletonType == SingletonTypes.Page ||
                            (viewInfo.SingletonType == SingletonTypes.Url && item.View.Context.Request.URL.ToLower() == request.URL.ToLower()))
                        {
                            singlePageTab = item;
                            break;
                        }
                    }
                }
                else
                {

                    if (item.Tag != null && (tagRequest = item.Tag as Request) != null)
                    {
                        if (tagRequest.URL.Equals(request.URL))
                        {
                            singlePageTab = item;
                            break;
                        }
                    }
                }
            }

            if (singlePageTab != null && singlePageTab.View != null)
            {
                singlePageTab.View.Context.Request.Param = request.Param;
                singlePageTab.View.Context.Request.QueryString = request.QueryString;
                singlePageTab.View.Context.Request.URL = request.URL;
                singlePageTab.View.Context.Request.UserState = request.UserState;

                if (this.SelectedItem == singlePageTab && singlePageTab.View != null)
                {
                    (singlePageTab.View.Context.Window as Window).OnWindowStatusChanged(this, new StatusChangedEventArgs(true));
                }
                else
                {
                    this.SelectedItem = singlePageTab;
                }
            }

            return singlePageTab != null;
        }

        protected virtual void SetPageTab(PageTab tab, IPage page)
        {
            string url = string.Format("/{0}/{1}", page.Context.Request.ModuleName, page.Context.Request.ViewName).ToLower();
            string title = "";
            AuthMenuItem menuItem, currentItem = null;
            TextBlock tbItem;

            System.Collections.Generic.Queue<AuthMenuItem> queue = new Queue<AuthMenuItem>();

            //1 traversal menuitem
            foreach (AuthMenuItem item in ComponentFactory.GetComponent<IAuth>().GetAuthorizedNavigateItems())
            {
                queue.Enqueue(item);
            }

            while (queue.Count > 0)
            {
                menuItem = queue.Dequeue();

                if (!string.IsNullOrEmpty(menuItem.URL) && menuItem.URL.ToLower().Equals(url))
                {
                    currentItem = menuItem;
                    break;
                }

                if (menuItem.Items != null)
                {
                    foreach (AuthMenuItem item in menuItem.Items)
                    {
                        item.Parent = menuItem;
                        queue.Enqueue(item);
                    }
                }
            }

            //2 set page tab
            //Add by ryan, 解决多tab页面并发打开的时候，因为call back是异步的，之前设置的当前选中的pagetab的面包屑会被后面异步回来的页面给覆盖掉。
            if (this.SelectedPage == page)
            {
                m_panelNavgigation.Children.Clear();
            }

            //当前设置页面再导航菜单中能够找到，就是用while循环找到它的父节点，然后去build这个面包屑；
            if (currentItem != null)
            {
                //2.1 set pageTab's header
                tab.Header = string.IsNullOrEmpty(page.Title) ? currentItem.Name : page.Title;
                string pageTitle = string.IsNullOrEmpty(page.Title) ? currentItem.Name : page.Title;
                //2.2 set pageTab's title
                while (currentItem != null)
                {
                    if (title == "")
                    {
                        title = currentItem.Name;
                        //Add by ryan, 解决多tab页面并发打开的时候，因为call back是异步的，之前设置的当前选中的pagetab的面包屑会被后面异步回来的页面给覆盖掉。
                        if (this.SelectedPage == page)
                        {
                            m_panelNavgigation.Children.Add(new TextBlock()
                            {
                                Text = pageTitle,
                                Foreground = m_panelNavgigation.Resources["brushPageTitle"] as SolidColorBrush,
                                TextTrimming = TextTrimming.WordEllipsis
                            });
                            ToolTipService.SetToolTip(m_panelNavgigation.Children[m_panelNavgigation.Children.Count - 1], new ToolTip { Content = pageTitle });
                        }
                    }
                    else
                    {
                        title = currentItem.Name + " > " + title;
                        //Add by ryan, 解决多tab页面并发打开的时候，因为call back是异步的，之前设置的当前选中的pagetab的面包屑会被后面异步回来的页面给覆盖掉。
                        if (this.SelectedPage == page)
                        {
                            m_panelNavgigation.Children.Insert(0, new TextBlock()
                            {
                                Text = " > ",
                                Foreground = m_panelNavgigation.Resources["brushPageTitle"] as SolidColorBrush,
                                TextTrimming = TextTrimming.WordEllipsis
                            });

                            if (!string.IsNullOrEmpty(currentItem.URL))
                            {
                                tbItem = new TextBlock()
                                {
                                    Text = currentItem.Name,
                                    Foreground = m_panelNavgigation.Resources["brushURLLink"] as SolidColorBrush,
                                    Cursor = System.Windows.Input.Cursors.Hand,
                                    Tag = currentItem.URL,
                                    TextTrimming = TextTrimming.WordEllipsis
                                };
                                ToolTipService.SetToolTip(tbItem, new ToolTip { Content = currentItem.Name });
                            }
                            else
                            {
                                tbItem = new TextBlock()
                                {
                                    Text = currentItem.Name,
                                    Foreground = m_panelNavgigation.Resources["brushPageTitle"] as SolidColorBrush,
                                    TextTrimming = TextTrimming.WordEllipsis
                                };
                                ToolTipService.SetToolTip(tbItem, new ToolTip { Content = currentItem.Name });
                            }
                            m_panelNavgigation.Children.Insert(0, tbItem);
                            tbItem.MouseLeftButtonUp += new MouseButtonEventHandler(tbItem_MouseLeftButtonUp);
                        }
                    }

                    currentItem = currentItem.Parent;
                }
            }
            //当前设置页面找不到就只设置自己的title到面包屑中；
            else
            {
                tab.Header = page.Title;
                title = page.Title;
                //Add by ryan, 解决多tab页面并发打开的时候，因为call back是异步的，之前设置的当前选中的pagetab的面包屑会被后面异步回来的页面给覆盖掉。
                if (this.SelectedPage == page)
                {
                    m_panelNavgigation.Children.Add(new TextBlock()
                    {
                        Text = page.Title,
                        Foreground = m_panelNavgigation.Resources["brushPageTitle"] as SolidColorBrush
                    });
                    ToolTipService.SetToolTip(m_panelNavgigation.Children[m_panelNavgigation.Children.Count - 1], new ToolTip { Content = page.Title });
                }
            }

            var toolTip = ToolTipService.GetToolTip(tab) as ToolTip;

            if (toolTip == null)
            {
                toolTip = new ToolTip();
                ToolTipService.SetToolTip(tab, toolTip);
            }

            toolTip.Content = title;

            if (m_contentPageDescription != null)
            {
                m_contentPageDescription.Content = page.Description;
            }
            if (tab.Header != null && page is PageBase)
            {
                (page as PageBase).Title = tab.Header.ToString();
            }
        }

        internal static PageTab GetPageTabByChild(FrameworkElement element)
        {
            PageTab tab = null;
            while (element != null)
            {
                tab = element as PageTab;
                if (tab == null)
                {
                    element = (element.Parent == null ? null : element.Parent as FrameworkElement);
                }
                else
                {
                    break;
                }
            }

            return tab;
        }

        internal void SetContentViewer(PageTab tab)
        {
            if (m_contentViewer != null)
            {
                FrameworkElement content = m_contentViewer.Content as FrameworkElement;
                m_contentViewer.HorizontalScrollBarVisibility = PageBrowser.GetDocumentHorizontalScrollBar(tab as DependencyObject);
                m_contentViewer.VerticalScrollBarVisibility = PageBrowser.GetDocumentVerticalScrollBar(tab as DependencyObject);

                if (content == null)
                {
                    return;
                }

                if (m_contentViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto ||
                   m_contentViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible ||
                   m_contentViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Auto ||
                   m_contentViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                {
                    m_contentViewer.UpdateLayout();
                }
            }
        }

        internal void UpdatePageTitle(PageTab tab)
        {
            if (tab != null && tab.View != null)
            {
                (tab as TabItem).Header = tab.View.Title;
                TextBlock textBlock = m_panelNavgigation.Children[m_panelNavgigation.Children.Count - 1] as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Text = tab.View.Title;
                    ToolTip toolTip = ToolTipService.GetToolTip(textBlock) as ToolTip;
                    if (toolTip != null)
                    {
                        toolTip.Content = tab.View.Title;
                    }
                }
                ToolTip tabToolTip = ToolTipService.GetToolTip(tab) as ToolTip;
                if (tabToolTip != null && tabToolTip.Content != null)
                {
                    if (tabToolTip.Content.ToString().LastIndexOf(">") > 1)
                    {
                        tabToolTip.Content = tabToolTip.Content.ToString().Substring(0, tabToolTip.Content.ToString().LastIndexOf(">") + 2) + tab.View.Title;
                    }
                }
            }
        }

        internal static Panel GetPanelContainerByPageTabContent(DependencyObject pageTabContent)
        {
            if (pageTabContent == null)
            {
                return null;
            }
            FrameworkElement container = System.Windows.Media.VisualTreeHelper.GetParent(pageTabContent) as FrameworkElement;

            while (container != null && !(container is Panel))
            {
                container = System.Windows.Media.VisualTreeHelper.GetParent(container) as FrameworkElement;
            }

            return container as Panel;
        }

        public static void SetAllowAutoResetScrollBar(DependencyObject obj, bool isAllow)
        {
            obj.SetValue(PageBrowser.AllowAutoResetScrollBarProperty, isAllow);
        }

        public static bool GetAllowAutoResetScrollBar(DependencyObject obj)
        {
            return (bool)obj.GetValue(PageBrowser.AllowAutoResetScrollBarProperty);
        }

        public static void SetDocumentHorizontalScrollBar(DependencyObject obj, ScrollBarVisibility scrollBarStatus)
        {
            FrameworkElement element = obj as FrameworkElement;
            PageTab tab = null;

            if (element != null)
            {
                tab = GetPageTabByChild(element);
                if (tab != null)
                {
                    (tab as DependencyObject).SetValue(PageBrowser.DocumentHorizontalScrollBarProperty, scrollBarStatus);
                    if (tab.IsSelected && tab.Parent != null)
                    {
                        (tab.Parent as PageBrowser).SetContentViewer(tab);
                    }
                }
            }
        }

        public static ScrollBarVisibility GetDocumentHorizontalScrollBar(DependencyObject obj)
        {
            FrameworkElement element = obj as FrameworkElement;
            PageTab tab = obj as PageTab;

            if (tab == null)
            {
                tab = GetPageTabByChild(element);
            }

            if (tab != null)
            {
                return (ScrollBarVisibility)(tab as DependencyObject).GetValue(PageBrowser.DocumentHorizontalScrollBarProperty);
            }

            return ScrollBarVisibility.Auto;
        }

        public static void SetDocumentVerticalScrollBar(DependencyObject obj, ScrollBarVisibility scrollBarStatus)
        {
            FrameworkElement element = obj as FrameworkElement;
            PageTab tab = null;

            if (element != null)
            {
                tab = GetPageTabByChild(element);
                if (tab != null)
                {
                    (tab as DependencyObject).SetValue(PageBrowser.DocumentVerticalScrollBarProperty, scrollBarStatus);
                    if (tab.IsSelected && tab.Parent != null)
                    {
                        (tab.Parent as PageBrowser).SetContentViewer(tab);
                    }
                }
            }
        }

        public static ScrollBarVisibility GetDocumentVerticalScrollBar(DependencyObject obj)
        {
            FrameworkElement element = obj as FrameworkElement;
            PageTab tab = obj as PageTab;

            if (tab == null)
            {
                tab = GetPageTabByChild(element);
            }

            if (tab != null)
            {
                return (ScrollBarVisibility)(tab as DependencyObject).GetValue(PageBrowser.DocumentVerticalScrollBarProperty);
            }

            return ScrollBarVisibility.Auto;
        }

        public void Add(PageTab tab)
        {
            //添加MenuContext
            if (tab != null)
            {
                tab.MouseRightButtonDown -= new MouseButtonEventHandler(tab_MouseRightButtonDown);
                tab.MouseRightButtonUp -= new MouseButtonEventHandler(tab_MouseRightButtonUp);
                tab.MouseRightButtonDown += new MouseButtonEventHandler(tab_MouseRightButtonDown);
                tab.MouseRightButtonUp += new MouseButtonEventHandler(tab_MouseRightButtonUp);
            }

            if (this.SelectedItem != null)
            {
                this.Items.Insert(this.Items.IndexOf(this.SelectedItem) + 1, tab);
            }
            else
            {
                this.Items.Add(tab);
            }
        }

        public void Close(PageTab tab)
        {
            Close(tab, true, false);
        }

        public void Close(bool isForce)
        {
            if (this.SelectedItem != null)
            {
                this.Close(this.SelectedItem as PageTab, true, true);
            }
        }

        public void UpdatePageTitle()
        {
            UpdatePageTitle(this.SelectedItem as PageTab);
        }

        #endregion

        #region Event Implements

        void ContentDropTarget_DragSourceDropped(object sender, DropEventArgs args)
        {
            var tab = ((args.DragSource.Content as FrameworkElement).FindName("HeaderTopSelected") as FrameworkElement).Tag as PageTab;

            if (tab != null && tab.Content != null)
            {
                if (!tab.View.Context.Request.URL.Contains(this.DefaultPage))
                {
                    var container = tab.Content as IContainer;
                    if (container != null && container.Children.Count > 0)
                    {
                        var element = container.Children[0] as FrameworkElement;
                        if (element != null)
                        {
                            this.Close(tab);

                            var window = new FloatableWindow();
                            var gridContainer = new GridContainer();
                            gridContainer.Children.Add(element);

                            window.ParentLayoutRoot = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
                            window.Height = this.m_contentViewer.ActualHeight * 0.9;
                            window.Width = this.m_contentViewer.ActualWidth * 0.8;
                            window.Content = gridContainer;
                            window.Title = tab.Header.ToString();
                            window.Show();
                        }
                    }
                }
            }
        }


        void RootVisual_KeyDown(object sender, KeyEventArgs e)
        {
            if (LayoutMask.RootVisualContainerCount <= 0)
            {
                ModifierKeys keys = Keyboard.Modifiers;

                var tab = this.SelectedItem as PageTab;
                //关闭当前标签页
                if ((e.Key == Key.C) && keys == (ModifierKeys.Control | ModifierKeys.Alt))
                {
                    if (tab != null && tab.View != null)
                    {
                        this.Close(tab);
                    }
                }

                //关闭所有标签页
                if ((e.Key == Key.A) && keys == (ModifierKeys.Control | ModifierKeys.Alt) && tab != null && tab.View != null)
                {
                    this.CloseAllTabs();
                }

                //关闭其他页
                if ((e.Key == Key.X) && keys == (ModifierKeys.Control | ModifierKeys.Alt))
                {
                    if (tab != null)
                    {
                        this.CloseOtherTabs(tab);
                    }
                }

                //复制页面
                if ((e.Key == Key.D) && keys == (ModifierKeys.Control | ModifierKeys.Alt))
                {
                    var req = tab.View.Context.Request;

                    this.Navigate(req, true);
                }

                //重新载入页面
                if ((e.Key == Key.R) && keys == (ModifierKeys.Control | ModifierKeys.Alt) && tab != null && tab.View != null)
                {
                    this.Refresh();
                }
            }
        }

        void tab_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void tab_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tab = sender as PageTab;

            //防止在下载XAP包的时候右键报错。
            if (tab.View == null) return;

            var context = new ContextMenu();
            context.IsOpen = true;

            var p = e.GetPosition(null);

            context.VerticalOffset = p.Y + 10;
            context.HorizontalOffset = p.X + 10;

            var item_closeTab = new MenuItem();
            item_closeTab.Header = MessageResource.PageBrowser_CloseTab_Title;
            item_closeTab.Tag = tab;
            item_closeTab.Click += new RoutedEventHandler(item_closeTab_Click);

            var item_colseAll = new MenuItem();
            item_colseAll.Header = MessageResource.PageBrowser_CloseAllTabs_Title;
            item_colseAll.Tag = tab;
            item_colseAll.Click += new RoutedEventHandler(item_colseAll_Click);

            var item_reload = new MenuItem();
            item_reload.Header = MessageResource.PageBrowser_Reload_Title;
            item_reload.Click += new RoutedEventHandler(item_reload_Click);

            var item_duplicate = new MenuItem();
            item_duplicate.Header = MessageResource.PageBrowser_Duplicate_Title;
            item_duplicate.IsEnabled = !this.IsSingleton(tab.View.Context.Request, false);
            item_duplicate.Tag = tab;
            item_duplicate.Click += new RoutedEventHandler(item_duplicate_Click);

            var item_closeOthers = new MenuItem();
            item_closeOthers.Header = MessageResource.PageBrowser_CloseOtherTabs_Title;
            item_closeOthers.IsEnabled = this.Items.Count > 1;
            item_closeOthers.Tag = tab;
            item_closeOthers.Click += new RoutedEventHandler(item_closeOthers_Click);

            context.Items.Add(item_reload);
            context.Items.Add(item_duplicate);
            context.Items.Add(new Separator());
            context.Items.Add(item_closeTab);
            context.Items.Add(item_closeOthers);
            context.Items.Add(item_colseAll);

        }

        void item_closeOthers_Click(object sender, RoutedEventArgs e)
        {
            var tab = ((MenuItem)sender).Tag as PageTab;

            CloseOtherTabs(tab);
        }

        void item_duplicate_Click(object sender, RoutedEventArgs e)
        {
            var tab = ((MenuItem)sender).Tag as PageTab;
            var req = tab.View.Context.Request;

            this.Navigate(req, true);
        }

        void item_reload_Click(object sender, RoutedEventArgs e)
        {
            this.Refresh();
        }

        void item_colseAll_Click(object sender, RoutedEventArgs e)
        {
            this.CloseAllTabs();
        }

        void item_closeTab_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var tab = item.Tag as PageTab;

                this.Close(tab);
            }
        }

        void m_contentViewer_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject grid = System.Windows.Media.VisualTreeHelper.GetChild(System.Windows.Media.VisualTreeHelper.GetChild(m_contentViewer, 0), 0);
            h_scrollbar = System.Windows.Media.VisualTreeHelper.GetChild(grid, 1) as ScrollBar;
            v_scrollbar = System.Windows.Media.VisualTreeHelper.GetChild(grid, 2) as ScrollBar;

            RegisterDependencyProperty("Visibility", h_scrollbar, new PropertyChangedCallback(OnPropertyChanged));
            RegisterDependencyProperty("Visibility", v_scrollbar, new PropertyChangedCallback(OnPropertyChanged));
        }

        void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ScrollBar scrollbar = sender as ScrollBar;
            if (m_contentViewer != null && args.NewValue != args.OldValue)
            {
                if (args.NewValue != null && ((Visibility)args.NewValue) == Visibility.Visible &&
                    this.AllowAutoResetScrollBar && (scrollbar == h_scrollbar || scrollbar == v_scrollbar))
                {
                    if (scrollbar.Orientation == Orientation.Horizontal)
                    {
                        m_contentViewer.ScrollToHorizontalOffset(0D);
                    }
                    else
                    {
                        m_contentViewer.ScrollToVerticalOffset(0D);
                    }
                }
            }
        }

        protected virtual void OnNavigated(object sender, EventArgs e)
        {
            if (this.NavigateCompleted != null)
            {
                this.NavigateCompleted(sender, e);
            }
        }

        protected virtual void OnNavigating(object sender, LoadedMoudleEventArgs e)
        {
            if (e.Status == LoadModuleStatus.Begin)
            {
                PageTab tab = GetPageTabByChild(sender as FrameworkElement);
                tab.Header = MessageResource.PageBrowser_LoadingSpin_Title;
            }

            if (this.Navigating != null)
            {
                this.Navigating(sender, e);
            }

            if (e.Error != null)
            {
                if (e.Error is PageException)
                {
                    throw e.Error;
                }
                else
                {
                    throw new PageException(MessageResource.PageException_PageLoadFailure_Title,
                        MessageResource.PageException_PageLoadFailure_Message, e.Error, e.Request);
                }
            }

            if (e.Status == LoadModuleStatus.End)
            {
                Request request = e.Request as Request;
                IViewInfo viewInfo = request.ModuleInfo.GetViewInfoByName(request.ViewName);
                PageTab tab = GetPageTabByChild(sender as FrameworkElement);
                object view;
                IPage page;

                if (viewInfo == null)
                {
                    throw new PageException(MessageResource.PageException_PageNoFound_Title,
                        MessageResource.PageExcepton_PageNotFound_Message, request);
                }

                try
                {
                    view = viewInfo.GetViewInstance(new PageContext(request, new Window(tab)));
                }
                catch (Exception ex)
                {
                    throw new PageException(MessageResource.PageException_PageInitializeError_Ttitle,
                        MessageResource.PageException_PageInitializeError_Message, ex, request);
                }

                page = view as IPage;

                (tab.Content as IContainer).Children.Add(view as UIElement);
                if (page != null)
                {
                    (page as UserControl).SizeChanged += new SizeChangedEventHandler(Page_SizeChanged);
                    SetPageTab(tab, page);
                    SetPage(tab, true);
                }
                else
                {
                    SetContentViewer(tab);
                    this.OnNavigated(tab, new EventArgs());
                }

                if (tab.IsSelected && page != null)
                {
                    (page.Context.Window as Window).OnWindowStatusChanged(tab, new StatusChangedEventArgs(true));
                }

            }

            if (m_content != null)
            {
                m_content.VerticalAlignment = VerticalAlignment.Stretch;
                m_content.HorizontalAlignment = HorizontalAlignment.Stretch;
            }
        }

        void Current_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null &&
                    (e.ExceptionObject is PageException || (e.ExceptionObject as Exception).InnerException is PageException))
            {
                e.Handled = true;
                Request request = null;
                PageTab tab = null;
                PageException exception = e.ExceptionObject as PageException;

                if (exception.Request is Request)
                {
                    request = exception.Request as Request;
                }
                else if (exception.Request is PageContext)
                {
                    request = (exception.Request as PageContext).Request;
                }

                if (request != null)
                {
                    foreach (PageTab item in this.Items)
                    {
                        if (item.Tag == request)
                        {
                            tab = item;
                            break;
                        }
                    }

                    if (tab != null)
                    {
                        this.Navigate(Request.Host + "#" + this.ErrorPage, exception, false, tab, true, false);
                    }
                    else if (this.SelectedItem != null)
                    {
                        this.Navigate(Request.Host + "#" + this.ErrorPage, exception, false, this.SelectedItem as PageTab, true, false);
                    }
                }
            }
        }

        void PageBrowser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PageTab tab, selectedTab = null;

            if (e.AddedItems.Count > 0 && this.SelectedItem != null)
            {
                //1 calculate tab's width
                tab = this.Items[0] as PageTab;
                if (!double.IsNaN(tab.m_maxWidth))
                {
                    tab.MaxWidth = tab.m_maxWidth;
                }
                selectedTab = this.SelectedItem as PageTab;
                double newWidth = double.NaN;



                if (!double.IsNaN(newWidth))
                {
                    foreach (PageTab item in this.Items)
                    {
                        item.Width = newWidth;
                        if (!double.IsNaN(item.m_maxWidth))
                        {
                            item.MaxWidth = item.m_maxWidth;
                        }
                    }
                }
                else if (!double.IsNaN(tab.Width))
                {
                    foreach (PageTab item in this.Items)
                    {
                        item.Width = tab.Width;
                        if (!double.IsNaN(item.m_maxWidth))
                        {
                            item.MaxWidth = item.m_maxWidth;
                        }
                    }
                }

                //1.0 Change navigation url and title
                if (selectedTab.View != null)
                {
                    SetPageTab(selectedTab, selectedTab.View);
                }

                //1.1 set page
                IPage page;
                DependencyObject element = null;
                bool isLayoutSetPage = false;
                if (tab.View != null &&
                      (page = tab.View as IPage) != null && (element = page as DependencyObject) != null)
                {
                    isLayoutSetPage = (bool)element.GetValue(IsLayoutSetPageProperty);
                }

                //Modify By Ryan:修改当一次性请求多个页面时，不会去执行布局的操作，因为View都还没有来得及产生；
                if (isLayoutSetPage || (selectedTab.View != null && (selectedTab.View is PageBase) && !((PageBase)selectedTab.View).IsLoaded))
                {
                    this.SetPage(selectedTab, true);
                    element.SetValue(IsLayoutSetPageProperty, false);
                }
                else
                {
                    this.SetPage(selectedTab, false);
                }

                //4 set popupbox for tab
                // SetPopupBox(selectedTab);

                //5 trigger onselectchangedevent
                if (selectedTab.View != null)
                {
                    (selectedTab.View.Context.Window as Window).OnWindowStatusChanged(tab, new StatusChangedEventArgs(selectedTab.IsSelected));
                }
            }

            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != selectedTab)
            {
                tab = e.RemovedItems[0] as PageTab;
                // SetPopupBox(tab);
                if (tab.View != null)
                {
                    (tab.View.Context.Window as Window).OnWindowStatusChanged(tab, new StatusChangedEventArgs(tab.IsSelected));
                }
            }

            if (this.SelectedPage != null)
            {
                RiseUrlChange(this.SelectedPage.Context.Request);
            }
        }

        void ComponentCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                    e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (IComponent item in e.NewItems)
                {
                    item.InitializeComponent(this);
                }
            }
        }

        void m_contentViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetContentViewer(this.SelectedItem as PageTab);
        }

        void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            IPage page = sender as IPage;

            if (page != null)
            {
                page.Context.OnPageSizeChanged(page, e);
            }
        }

        void tbItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tbItem = sender as TextBlock;
            if (tbItem.Tag != null)
            {
                this.Navigate(tbItem.Tag.ToString(), false);
            }
        }

        #endregion
    }

    public class PageTab : TabItem
    {
        private Button m_btnSelectedClose;
        private Button m_btnUnSelectedClose;
        private TextBlock m_txtEllipsis;
        private ContentControl m_HeaderTopUnselected;
        internal TextBlock m_content;
        internal double m_maxWidth = double.NaN;
        internal ContextMenu ContextMenu { get; set; }

        public IPage View
        {
            get
            {
                IPage page = null;

                if (this.Content != null)
                {
                    foreach (UIElement element in (this.Content as IContainer).Children)
                    {
                        if ((page = element as IPage) != null)
                        {
                            break;
                        }
                    }
                }

                return page;
            }
        }

        private PageTab()
            : base()
        {
            this.DefaultStyleKey = typeof(PageTab);
            this.Loaded += new RoutedEventHandler(PageTab_Loaded);
        }

        public PageTab(string name, IContainer content)
            : this()
        {
            base.Header = name;
            base.Content = content;
        }


        void PageTab_Loaded(object sender, RoutedEventArgs e)
        {
            m_maxWidth = this.MaxWidth;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_btnSelectedClose = this.GetTemplateChild("btnSelectedClose") as Button;
            m_btnUnSelectedClose = this.GetTemplateChild("btnUnSelectedClose") as Button;
            m_content = this.GetTemplateChild("HeaderTopSelected") as TextBlock;
            m_txtEllipsis = this.GetTemplateChild("txtEllipsis") as TextBlock;
            m_HeaderTopUnselected = this.GetTemplateChild("HeaderTopUnselected") as ContentControl;
            m_content.Tag = this;

            if (m_btnSelectedClose != null)
            {
                m_btnSelectedClose.Click += new RoutedEventHandler(OnPageClose);
            }

            if (m_btnUnSelectedClose != null)
            {
                m_btnUnSelectedClose.Click += new RoutedEventHandler(OnPageClose);
            }

        }


        void OnPageClose(object sender, RoutedEventArgs e)
        {
            if (this.View != null)
            {
                Close(sender);
            }
        }

        private void Close(object sender)
        {
            PageBrowser browser = this.Parent as PageBrowser;

            Button button = sender as Button;

            if (browser != null && button != null)
            {
                button.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    PageTab tab = null;
                    if (!this.IsSelected)
                    {
                        tab = browser.SelectedItem as PageTab;
                    }

                    browser.Close(this);

                    if (tab != null)
                    {
                        tab.IsSelected = true;
                    }
                }));
            }
        }
    }

    public class PageBrowserTabPanel : TabPanel
    {
        private double m_OldSingleTabWidth = 0;
        private PageBrowser m_PageBrowser = null;
        private bool m_MouseInTabBar = false; // Greg: 专门用于记录鼠标是否在Tabbar上，当用户改变浏览器窗口大小时，Tab的大小应该重新计算。

        public PageBrowser PageBrowser
        {
            get { return m_PageBrowser; }
            set
            {
                m_PageBrowser = value;
                if (m_PageBrowser != null)
                {
                    m_PageBrowser.TabClosing -= new EventHandler(PageBrowser_TabClosing);
                    m_PageBrowser.TabClosing += new EventHandler(PageBrowser_TabClosing);
                }
            }
        }

        public PageBrowserTabPanel()
            : base()
        {
            this.MouseLeave += new MouseEventHandler(PageBrowserTabPanel_MouseLeave);
            this.MouseEnter += new MouseEventHandler(PageBrowserTabPanel_MouseEnter);
        }

        void PageBrowserTabPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            m_MouseInTabBar = true;
        }

        void PageBrowser_TabClosing(object sender, EventArgs e)
        {
            if ((sender as PageTab) == ((PageTab)this.Children[this.Children.Count - 1]))
            {
                if (this.Children.Count != 1)
                    m_OldSingleTabWidth = m_OldSingleTabWidth * this.Children.Count / (this.Children.Count - 1);

                RecalculateTabWitdh(this.ActualWidth, true);
            }
        }

        void PageBrowserTabPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            m_OldSingleTabWidth = this.ActualWidth / this.Children.Count;
            RecalculateTabWitdh(this.ActualWidth, false);

            m_MouseInTabBar = false;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            RecalculateTabWitdh(availableSize.Width, false);
            return base.MeasureOverride(availableSize);
        }

        private void RecalculateTabWitdh(double panelWidth, bool useSelfCalc)
        {
            if (this.Children.Count > 0)
            {
                double eachWidth = panelWidth / this.Children.Count;

                if (false == m_MouseInTabBar)
                    m_OldSingleTabWidth = eachWidth;

                if (false == useSelfCalc && (m_OldSingleTabWidth == 0 || m_OldSingleTabWidth > eachWidth || this.Children.Count == 1))
                    m_OldSingleTabWidth = eachWidth;


                // Greg: 换一种比较平均的方法来降低像素的偏移抖动。
                for (int i = 0; i < this.Children.Count; i++)
                {
                    if (i % 2 == 0)
                        ((FrameworkElement)this.Children[i]).Width = Math.Floor(m_OldSingleTabWidth);
                    else
                        ((FrameworkElement)this.Children[i]).Width = m_OldSingleTabWidth;
                }
            }
        }
    }

}

