using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Windows;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class PageRecordManager : IComponent
    {
        private IPageBrowser m_browser;
        private const string m_PageRecord_FileName = "FM.PageRecord";
        private IUserProfile m_profile;
        private List<PageRecordInfo> m_PageRecordList;

        private const string m_ClosedTab_FileName = "FM.ClosedTabs";
        private List<ClosedTabInfo> m_ClosedTabList;

        #region IComponent Members

        public string Name
        {
            get { return "PageRecordManager"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            m_browser = browser;
            m_browser.NavigateCompleted += new EventHandler(m_browser_Navigated);
            m_browser.TabClosing += new EventHandler(m_browser_TabClosing);
            m_browser = browser;
            m_profile = ComponentFactory.GetComponent<IUserProfile>() as IUserProfile;            
            if (m_profile == null)
            {
                throw new Exception("can not find IUserProfile component!");
            }

            try
            {
                m_PageRecordList = FilterPage(m_profile.Get<List<PageRecordInfo>>(m_PageRecord_FileName));

                m_ClosedTabList = FilterPage(m_profile.Get<List<ClosedTabInfo>>(m_ClosedTab_FileName));
            }
            catch
            {
                //don't do anything
            }

            if (m_PageRecordList == null)
            {
                m_PageRecordList = new List<PageRecordInfo>();
            }
            if (m_ClosedTabList == null)
            {
                m_ClosedTabList = new List<ClosedTabInfo>();
            }

            Application.Current.Exit += new EventHandler(Current_Exit);
        }

        void Current_Exit(object sender, EventArgs e)
        {
            Save();
        }

        void m_browser_TabClosing(object sender, EventArgs e)
        {
            PageTab tab = sender as PageTab;

            if (tab != null && tab.View != null && tab.View.Context != null)
            {
                Request request = tab.View.Context.Request;
                string url = string.Format("/{0}/{1}", request.ModuleName, request.ViewName);

                ClosedTabInfo closedTabInfo = null;
                foreach (ClosedTabInfo i in m_ClosedTabList)
                {
                    if (i.Url.ToLower() == url.ToLower())
                    {
                        closedTabInfo = i;
                        break;
                    }
                }
                if (closedTabInfo == null)
                {
                    ClosedTabInfo info = new ClosedTabInfo();
                    info.Url = url;
                    info.Title = tab.Header == null ? request.ViewName : tab.Header.ToString();
                    info.ClosedTime = DateTime.Now;
                    if (ValidPage(info))
                    {
                        m_ClosedTabList.Add(info);
                    }
                }
                else
                {
                    closedTabInfo.ClosedTime = DateTime.Now;
                }
            }

        }


        void m_browser_Navigated(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => 
            {
                PageTab tab = sender as PageTab;
                if (tab != null && tab.View != null)
                {
                    Request request = tab.View.Context.Request;
                    string url = string.Format("/{0}/{1}", request.ModuleName, request.ViewName);

                    PageRecordInfo pageRecordInfo = null;
                    foreach (PageRecordInfo i in m_PageRecordList)
                    {
                        if (i.Url.ToLower() == url.ToLower())
                        {
                            pageRecordInfo = i;
                            break;
                        }
                    }

                    if (pageRecordInfo != null)
                    {
                        if (pageRecordInfo.Quantity == int.MaxValue)
                        {
                            pageRecordInfo.Quantity = 1;
                        }
                        else
                        {
                            pageRecordInfo.Quantity += 1;
                        }
                    }
                    else
                    {
                        pageRecordInfo = new PageRecordInfo()
                        {
                            Title = tab.Header == null ? request.ViewName : tab.Header.ToString(),
                            Url = url,
                            Quantity = 1
                        };

                        if (ValidPage(pageRecordInfo))
                        {
                            m_PageRecordList.Add(pageRecordInfo);
                        }
                    }
                }
            });
        }


        private bool ValidPage(PageBasicInfo pageInfo)
        {                
            //过滤 error page && Home page
            if (pageInfo.Url.ToLower() == CPApplication.Current.ErrorPage.ToLower() 
                || pageInfo.Url.ToLower() == CPApplication.Current.DefaultPage.ToLower())
            {
                return false;
            }

            var menuItems = ComponentFactory.GetComponent<IAuth>().AuthorizedNavigateToList();
            foreach (AuthMenuItem menuItem in menuItems)
            {
                if (menuItem.Type == AuthMenuItemType.Page && menuItem.IsDisplay)
                {
                    if (menuItem.URL.Trim().ToLower() == pageInfo.Url.Trim().ToLower())
                    {
                        pageInfo.Title = menuItem.Name;
                        pageInfo.Url = menuItem.URL;
                        return true;
                    }
                }
            }
            return false;
        }

        private List<T> FilterPage<T>(List<T> data) where T: PageBasicInfo
        {
            for (int i = data.Count - 1; i >= 0; i--)
            {
                var pageInfo = data[i] as T;
                if (pageInfo != null)
                {
                    if (!ValidPage(pageInfo))
                    {
                        data.Remove(pageInfo);
                    }
                }
            }    
            return data;
        }


        public List<PageRecordInfo> GetHotHitPages(int maxCount)
        {
            return (from a in m_PageRecordList
                    orderby a.Quantity descending
                    select a).Take(maxCount).ToList();

        }

        public List<ClosedTabInfo> GetClosedTabList(int maxCount)
        {
            return (from a in m_ClosedTabList
                    orderby a.ClosedTime descending
                    select a).Take(maxCount).ToList();
        }


        private void Save()
        {
            m_profile.Set(m_PageRecord_FileName, m_PageRecordList);

            m_profile.Set(m_ClosedTab_FileName, m_ClosedTabList);
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
    
    public class PageBasicInfo
    {       
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class PageRecordInfo : PageBasicInfo
    {
        public int Quantity { get; set; }
    }

    public class ClosedTabInfo : PageBasicInfo
    {
        public DateTime ClosedTime { get; set; }
    }


}
