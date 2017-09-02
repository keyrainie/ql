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
using System.Collections.Generic;
using System.Threading;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class EventTrackerComponent : IEventTracker
    {
        private IPageBrowser m_browser;
        private List<EventLog> m_EventList = new List<EventLog>();
        private const string m_EventStatistic_FileName = "FM.EventTracker";
        private IUserProfile m_profile;
        private Timer m_Timer;
        private RestClient m_RestClient = new RestClient("/Service/Framework/V50/StatisticService.svc");

        #region IComponent Members

        public string Name
        {
            get { return "EventTrackerComponent"; }
        }

        public string Version
        {
            get { return "1.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            m_browser = browser;
            m_browser.NavigateCompleted += new EventHandler(m_browser_NavigateCompleted);
            m_browser = browser;
            m_profile = ComponentFactory.GetComponent<IUserProfile>() as IUserProfile;
            if (m_profile == null)
            {
                throw new Exception("can not find IUserProfile component!");
            }

            try
            {
                m_EventList = m_profile.Get<List<EventLog>>(m_EventStatistic_FileName);
            }
            catch
            {
            }

            if (m_EventList == null)
            {
                m_EventList = new List<EventLog>();
            }

            m_Timer = new Timer((object o) =>
            {
                try
                {
                    if (m_EventList != null && m_EventList.Count > 0)
                    {
                        m_RestClient.Create<object>("BatchTraceEventLog", m_EventList, (sender, args) =>
                        {
                            try
                            {
                                if (!args.FaultsHandle())
                                {
                                    m_EventList.Clear();
                                }
                            }
                            catch (Exception ex)
                            {
                                ComponentFactory.Logger.LogError(ex);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    ComponentFactory.Logger.LogError(ex);
                }
            }, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10));

            Application.Current.Exit += new EventHandler(Current_Exit);
        }

        void Current_Exit(object sender, EventArgs e)
        {
            m_profile.Set(m_EventStatistic_FileName, m_EventList);
        }

        void m_browser_NavigateCompleted(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                PageTab tab = sender as PageTab;
                if (tab != null && tab.View != null)
                {
                    Request request = tab.View.Context.Request;
                    string url = string.Format("/{0}/{1}", request.ModuleName, request.ViewName);

                    //过滤 error page && Home page
                    if (url.ToLower() == CPApplication.Current.ErrorPage.ToLower())
                    {
                        return;
                    }

                    m_EventList.Add(new EventLog()
                    {
                        Action = "Visit",
                        EventDate = DateTime.Now,
                        IP = CPApplication.Current.ClientIPAddress,
                        Url = url,
                        UserID = CPApplication.Current.LoginUser.ID,
                    });
                }
            });
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


        #region IEventTracker Members

        public void TraceEvent(string action, string label)
        {
            Request request = CPApplication.Current.CurrentPage.Context.Request;
            string url = string.Format("/{0}/{1}", request.ModuleName, request.ViewName);
            TraceEvent(url, action, label);
        }

        #endregion


        public void TraceEvent(string pageURL, string action, string label)
        {
            m_EventList.Add(new EventLog()
            {
                Action = action,
                Label = label,
                EventDate = DateTime.Now,
                IP = CPApplication.Current.ClientIPAddress,
                Url = pageURL,
                UserID = CPApplication.Current.LoginUser.ID,
            });
        }
    }

    public class EventLog
    {
        public string UserID { get; set; }

        public string IP { get; set; }

        public DateTime EventDate { get; set; }

        public string Url { get; set; }

        public string Action { get; set; }

        public string Label { get; set; }
    }
}
