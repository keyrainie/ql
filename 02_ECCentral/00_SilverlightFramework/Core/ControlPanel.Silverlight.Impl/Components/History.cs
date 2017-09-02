using System;
using System.Linq;
using System.Collections;
using System.Windows.Controls;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.Resources;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class History : IHistory
    {
        private LinkedList<Request> m_queue;
        private LinkedList<Request> m_visitedQueue;
        private LinkedList<Request> m_closedQueue;
        private LinkedList<TabView> m_visitedTabViews;
        private Request m_current;

        public Request Current
        {
            get
            {
                return m_current;
            }
        }

        public History()
        {
            m_queue = new LinkedList<Request>();
            m_visitedQueue = new LinkedList<Request>();
            m_closedQueue = new LinkedList<Request>();

            m_visitedTabViews = new LinkedList<TabView>();
        }

        #region IHistory Members

        public IList ClosedTabs
        {
            get
            {
                var list = m_closedQueue.ToList();
                list.Reverse();
                return list;
            }
        }

        public IList VisitedTabs
        {
            get
            {
                var list = m_visitedQueue.ToList();
                list.Reverse();
                return list;
            }
        }

        public IList VisitedTabViews
        {
            get
            {
                var list = m_visitedTabViews.ToList();
                list.Reverse();

                return list;
            }
        }

        public void Previous()
        {
            if (m_current != null)
            {
                LinkedListNode<Request> node = m_queue.Find(m_current);

                if (node != null && node.Previous != null)
                {
                    m_current = node.Previous.Value;
                }
                else if ((node = m_queue.Last) != null)
                {
                    m_current = node.Value;
                }
            }
        }

        public void Next()
        {
            if (m_current != null)
            {
                LinkedListNode<Request> node = m_queue.Find(m_current);

                if (node != null && node.Next != null)
                {
                    m_current = node.Next.Value;
                }
            }
        }

        public void Clear()
        {
            this.m_queue.Clear();
            this.m_current = null;
        }

        public void RemoveRecoveriedTab()
        {
            if (this.m_closedQueue.Count > 0)
            {
                m_closedQueue.RemoveLast();
            }
        }

        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "History"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            browser.NavigateCompleted += new EventHandler(browser_NavigateCompleted);
            browser.Navigating += new EventHandler<Newegg.Oversea.Silverlight.Core.Components.LoadedMoudleEventArgs>(browser_Navigating);
            browser.TabClosing += new EventHandler(browser_TabClosing);
        }

        void browser_TabClosing(object sender, EventArgs e)
        {
            var tab = sender as PageTab;
            if (!(tab.View is ErrorPage) && tab.View != null && tab.View.Context != null)
            { 
                m_closedQueue.AddLast(tab.View.Context.Request);
            }
        }

        void browser_Navigating(object sender, Newegg.Oversea.Silverlight.Core.Components.LoadedMoudleEventArgs e)
        {
            if (e.Status == Newegg.Oversea.Silverlight.Core.Components.LoadModuleStatus.Begin)
            {
                if (e.Request.RefererRequest != null)
                {
                    if (this.Current == null || !m_queue.Contains(e.Request.RefererRequest))
                    {
                        m_queue.AddLast(e.Request.RefererRequest);
                    }
                }
                var re = m_visitedQueue.FirstOrDefault(p => p.URL.Trim().ToUpper() == e.Request.URL.Trim().ToUpper());
                if (re != null)
                {
                    m_visitedQueue.Remove(re);
                }

                if (e.Request.ViewName.ToLower() != "error")
                {
                    m_visitedQueue.AddLast(e.Request);
                }

                var v = m_visitedTabViews.FirstOrDefault(p => string.Equals(p.Url, e.Request.URL, StringComparison.OrdinalIgnoreCase));
                if (v != null)
                {
                    m_visitedTabViews.Remove(v);
                }

                if (e.Request.ViewName.ToLower() != "error")
                {
                    m_visitedTabViews.AddLast(new TabView { Request = e.Request, Header = e.Request.ViewName, Url = e.Request.URL });
                }
            }
        }

        void browser_NavigateCompleted(object sender, EventArgs e)
        {
            PageTab tab = sender as PageTab;
            if (tab != null && tab.View != null)
            {
                m_current = tab.View.Context.Request;

                if (tab.Header == null)
                {
                    m_visitedTabViews.Last.Value.Header = MessageResource.PageBrowser_Unknown;
                }
                else
                {
                    m_visitedTabViews.Last.Value.Header = tab.Header.ToString();
                }
            }
        }

        public object GetInstance(TabItem tab)
        {
            return this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
