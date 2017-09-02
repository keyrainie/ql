using System;
using System.Windows;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Containers;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.Controls.Containers
{
    public class Window : IWindow
    {
        public event EventHandler<StatusChangedEventArgs> WindowStatusChanged;

        private PageBrowser m_browser;
        private PageTab m_pageTab;
        private IMessageBox m_messageBox;
        private ILoadingSpin m_loadingSpin;
        private IFaultHandle m_handle;
        private INotificationBox m_NotificationBox;
        private IEventTracker m_EventTracker;


        public Window(PageTab tab)
        {
            m_pageTab = tab;
            m_browser = tab.Parent as PageBrowser;
        }

        #region IWindow Members
        public double DocumentHeight
        {
            get
            {
                double height = double.NaN;
                if (this.m_pageTab != null && this.m_pageTab.View != null)
                {
                    height = (this.m_pageTab.View as UserControl).ActualHeight;
                }

                return height;
            }

            set
            {
                if (this.m_pageTab != null && this.m_pageTab.View != null)
                {
                    (this.m_pageTab.View as UserControl).Height = value;
                }
            }
        }

        public double DocumentWidth
        {
            get
            {
                double width = double.NaN;
                if (this.m_pageTab != null && this.m_pageTab.View != null)
                {
                    width = (this.m_pageTab.View as UserControl).ActualWidth;
                }

                return width;
            }

            set
            {
                if (this.m_pageTab != null && this.m_pageTab.View != null)
                {
                    (this.m_pageTab.View as UserControl).Width = value;
                }
            }
        }

        public double WindowHeight
        {
            get
            {
                return m_browser.WindowHeight;
            }
        }

        public double WindowWidth
        {
            get
            {
                return m_browser.WindowWidth;
            }
        }

        public bool Status
        {
            get
            {
                if (m_pageTab == null || !m_pageTab.IsSelected)
                {
                    return false;
                }

                return true;
            }

        }

        public ScrollBarVisibility DocumentHorizontalScrollBar
        {
            get
            {
                return PageBrowser.GetDocumentHorizontalScrollBar(this.m_pageTab);
            }
            set
            {
                PageBrowser.SetDocumentHorizontalScrollBar(this.m_pageTab, value);
            }
        }

        public ScrollBarVisibility DocumentVerticalScrollBar
        {
            get
            {
                return PageBrowser.GetDocumentVerticalScrollBar(this.m_pageTab);
            }
            set
            {
                PageBrowser.SetDocumentVerticalScrollBar(this.m_pageTab, value);
            }
        }

        public bool AllowAutoResetScrollBar
        {
            get
            {
                return PageBrowser.GetAllowAutoResetScrollBar(this.m_pageTab);
            }
            set
            {
                PageBrowser.SetAllowAutoResetScrollBar(this.m_pageTab, value);
            }
        }

        public ComponentCollection ComponentCollection
        {
            get { return this.m_browser.ComponentCollection; }
        }

        public IMessageBox MessageBox
        {
            get
            {
                if (m_messageBox == null)
                {
                    m_messageBox = m_browser.GetComponent<IMessageComponent>().GetInstance(m_pageTab) as IMessageBox;
                }

                return m_messageBox;
            }
        }

        public IAuth AuthManager
        {
            get
            {
                return ComponentFactory.GetComponent<IAuth>();
            }
        }

        public ILoadingSpin LoadingSpin
        {
            get
            {
                if (m_loadingSpin == null)
                {
                    m_loadingSpin = m_browser.GetComponent<ILoadingSpin>().GetInstance(m_pageTab) as ILoadingSpin;
                }

                return m_loadingSpin;
            }
        }

        public ILog Logger
        {
            get
            {
                return ComponentFactory.GetComponent<ILog>();
            }
        }

        public IMail Mailer
        {
            get
            {
                return ComponentFactory.GetComponent<IMail>();
            }
        }

        public IConfiguration Configuration
        {
            get
            {
                return ComponentFactory.GetComponent<IConfiguration>();
            }
        }

        public ICache Cacher
        {
            get
            {
                return ComponentFactory.GetComponent<ICache>();
            }
        }

        public IFaultHandle FaultHandle
        {
            get
            {
                if (m_handle == null)
                {
                    m_handle = m_browser.GetComponent<IFaultHandle>().GetInstance(m_pageTab) as IFaultHandle;
                }

                return m_handle;
            }
        }


        public IEventTracker EventTracker
        {
            get
            {
                return ComponentFactory.GetComponent<IEventTracker>();
            }
        }

        public IUserProfile Profile
        {
            get
            {
                return ComponentFactory.GetComponent<IUserProfile>();
            }
        }

        public INotificationBox NotificationBox
        {
            get
            {
                if (m_NotificationBox == null)
                {
                    m_NotificationBox = m_browser.GetComponent<INotificationBox>().GetInstance(m_pageTab) as INotificationBox;
                }

                return m_NotificationBox;
            }
        }

        public void Navigate(string url)
        {
            Navigate(url, null);
        }

        public void Navigate(string url, object args)
        {
            Navigate(url, args, false);
        }

        public void Navigate(string url, object args, bool isNewTab)
        {
            (m_browser as PageBrowser).Navigate(url, args, isNewTab, m_pageTab);
        }

        public void Navigate(Request request, bool isNewTab)
        {
            (m_browser as PageBrowser).Navigate(request, isNewTab, m_pageTab);
        }

        public void Back()
        {
            (m_browser as PageBrowser).Back(m_pageTab);
        }

        public void Forward()
        {
            (m_browser as PageBrowser).Forward(m_pageTab);
        }

        public void Close()
        {
            if (m_pageTab != null)
            {
                (m_browser as PageBrowser).Close(m_pageTab);
            }
        }

        public void Close(bool isForce)
        {
            if (m_pageTab != null)
            {
                (m_browser as PageBrowser).Close(isForce);
            }
        }

        public void Refresh()
        {
            if (m_pageTab != null)
            {
                (m_browser as PageBrowser).Refresh(m_pageTab);
            }
        }

        public void Alert(string content)
        {
            Alert(content, MessageType.Information);
        }

        public void Alert(string content, MessageType type)
        {
            Alert(null, content, type);
        }

        public void Alert(string title, string content)
        {
            Alert(null, content, MessageType.Information);
        }

        public void Alert(string title, string content, MessageType type)
        {
            Alert(title, content, type, null);
        }

        public void Alert(string title, string content, MessageType type, ResultHandler handle)
        {
            Alert(title, content, type, handle, null);
        }

        public void Alert(string title, string content, MessageType type, ResultHandler handle, System.Windows.Controls.Panel container)
        {
            if (container == null && m_pageTab != null)
            {
                container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            }

            m_browser.Alert(title, content, type, handle, container);
        }

        public void Confirm(string content, ResultHandler callback)
        {
            Confirm(null, content, callback);
        }

        public void Confirm(string title, string content, ResultHandler callback)
        {
            Confirm(null, content, callback, ButtonType.OKCancel);
        }

        public void Confirm(string title, string content, ResultHandler callback, ButtonType type)
        {
            Confirm(title, content, callback, type, null);
        }

        public void Confirm(string title, string content, ResultHandler callback, ButtonType type, System.Windows.Controls.Panel container)
        {
            if (container == null && m_pageTab != null)
            {
                container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            }

            m_browser.Confirm(title, content, callback, type, container);
        }

        public IDialog ShowDialog(string title, FrameworkElement content)
        {
            return ShowDialog(title, content, null);
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback)
        {
            return ShowDialog(title, content, callback, Size.Empty);
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size)
        {
            return ShowDialog(title, content, callback, size, null);
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size, System.Windows.Controls.Panel container)
        {
            if (container == null && m_pageTab != null)
            {
                container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            }

            return m_browser.ShowDialog(title, content, callback, size, container);
        }

        public IDialog ShowDialog(string title, string url, ResultHandler callback)
        {
            return ShowDialog(title, url, callback, Size.Empty);
        }

        public IDialog ShowDialog(string title, string url, ResultHandler callback, Size size)
        {
            return ShowDialog(title,url, callback, size, null);
        }

        public IDialog ShowDialog(string title, string url, ResultHandler callback, Size size, System.Windows.Controls.Panel container)
        {
            if (container == null && m_pageTab != null)
            {
                container = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
            }

            return m_browser.ShowDialog(title,url, callback, size, container);
        }

        public void CloseDialog(object UserState)
        {
            if (DialogClosed != null)
            {
                DialogClosed(this, new ResultEventArgs { Data = UserState });
            }
        }

        public void CloseDialog(object UserState,bool isForce)
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

        public IComponent GetComponentByName(string name)
        {
            return m_browser.GetComponentByName(name);
        }

        public T GetComponent<T>() where T : IComponent
        {
            return m_browser.GetComponent<T>();
        }

        public void UpdatePageTitle()
        {
            m_browser.UpdatePageTitle(m_pageTab);
        }

        public void ResetPageViewer()
        {
            m_browser.ResetPageViewer(this.m_pageTab);
        }

        #endregion

        internal virtual void OnWindowStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (WindowStatusChanged != null)
            {
                WindowStatusChanged(sender, e);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_pageTab != null)
            {
                this.LoadingSpin.Dispose();
                this.MessageBox.Clear();
                m_pageTab.ClearValue(PageBrowser.DocumentVerticalScrollBarProperty);
                m_pageTab.ClearValue(PageBrowser.DocumentHorizontalScrollBarProperty);
               // List<Panel> list = m_pageTab.GetValue(PageBrowser.PopupBoxListProperty) as List<Panel>;
               // Panel container;

                //if (list != null)
                //{
                //    foreach (Panel panel in list)
                //    {
                //        if (panel.Parent != null && (container = panel.Parent as Panel) != null)
                //        {
                //            container.Children.Remove(panel);
                //        }
                //    }
                //}
              //  m_pageTab.ClearValue(PageBrowser.PopupBoxListProperty);
            }
        }

        #endregion


    }
}
