using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.CommonService;
using System.Threading;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class XapVersionController : IXapVersionController
    {
        public event EventHandler<CheckXapVersionCompletedEventArgs> XapVersionChangedCompleted;

        private bool m_EnableAutoCheck;
        private TimeSpan m_AutoCheckIntervalTime;
        private Timer m_Timer;
        private CommonServiceV40Client m_ServiceClient;

        public List<XapVersion> XapVersionList { get; private set; }

        public bool EnableAutoCheck
        {
            get
            {
                return m_EnableAutoCheck;
            }
            set
            {
                m_EnableAutoCheck = value;
                if (value)
                {
                    m_Timer = new Timer((object o) =>
                    {
                        CheckXapVersionAsync(null);
                    }, null, TimeSpan.FromMinutes(1), AutoCheckIntervalTime);
                }
                else
                {
                    if (m_Timer != null)
                    {
                        m_Timer = null;
                    }
                }
            }
        }

        public TimeSpan AutoCheckIntervalTime
        {
            get
            {
                return m_AutoCheckIntervalTime;
            }
            set
            {
                m_AutoCheckIntervalTime = value;
                if (m_Timer != null)
                {
                    m_Timer.Change(TimeSpan.FromMinutes(1), AutoCheckIntervalTime);
                }
            }
        }

        public XapVersionController()
        {
            XapVersionList = new List<XapVersion>();
            AutoCheckIntervalTime = new TimeSpan(0, 5, 0);
            EnableAutoCheck = true;

            m_ServiceClient = new CommonServiceV40Client();
            m_ServiceClient.CheckAppVersionCompleted += new EventHandler<CheckAppVersionCompletedEventArgs>(m_ServiceClient_CheckAppVersionCompleted);
        }

        public void CheckXapVersionAsync(Action callback)
        {
            m_ServiceClient.CheckAppVersionAsync(callback);
        }

        void m_ServiceClient_CheckAppVersionCompleted(object sender, CheckAppVersionCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Faults != null && e.Result.Faults.Count > 0)
                {
                    throw new Exception(e.Result.Faults[0].ErrorDescription);
                }

                //1. 获得老版本的Xap包的信息
                var oldXapVersionList = new List<XapVersion>();
                foreach (var xapVersion in this.XapVersionList)
                {
                    var any = oldXapVersionList.Any(item => string.Equals(xapVersion.XapName, item.XapName, StringComparison.OrdinalIgnoreCase));

                    if (!any)
                    {
                        oldXapVersionList.Add(xapVersion);
                    }
                }

                //2. 获得新版本的Xap包的信息
                this.XapVersionList.Clear();
                foreach (var xapVersion in e.Result.Body)
                {
                    var any = this.XapVersionList.Any(item => string.Equals(xapVersion.XapName, item.XapName, StringComparison.OrdinalIgnoreCase));

                    if (!any)
                    {
                        this.XapVersionList.Add(xapVersion);
                    }
                }

                //3. 比较新老版本的版本号，并返回版本号变化的Xap包信息
                if (oldXapVersionList.Count > 0 && this.XapVersionList.Count > 0)
                {
                    var changedXapVersionList = new List<XapVersion>();

                    #region 框架包的版本比较

                    var fmXapName = CPApplication.Current.FrameworkXapName;
                    var latestXapInfo = this.XapVersionList.FirstOrDefault(xap => string.Equals(xap.XapName,fmXapName,StringComparison.OrdinalIgnoreCase));
                    var preXapInfo = oldXapVersionList.FirstOrDefault(xap => string.Equals(xap.XapName, fmXapName, StringComparison.OrdinalIgnoreCase));

                    if (latestXapInfo != null && preXapInfo != null
                        && latestXapInfo.Version != preXapInfo.Version)
                    {
                        changedXapVersionList.Add(new XapVersion
                        {
                            Title = latestXapInfo.Title,
                            XapName = latestXapInfo.XapName,
                            PublishDate = latestXapInfo.PublishDate,
                            Description = latestXapInfo.Description,
                            Version = latestXapInfo.Version,
                            PreVersion = preXapInfo.Version,
                            UpdateLevel = latestXapInfo.UpdateLevel
                        });
                    }

                    #endregion

                    #region 业务Domain的版本比较

                    var modules = ComponentFactory.GetComponent<IModuleManager>().GetAllModuleInfo();
                    if (modules != null)
                    {
                        foreach (var module in modules)
                        {
                            latestXapInfo = this.XapVersionList.FirstOrDefault(xap => string.Equals(xap.XapName, (module.Name + ".xap"), StringComparison.OrdinalIgnoreCase));
                            preXapInfo = oldXapVersionList.FirstOrDefault(xap => string.Equals(xap.XapName, (module.Name + ".xap"), StringComparison.OrdinalIgnoreCase));

                            if (latestXapInfo != null && preXapInfo != null
                                && latestXapInfo.Version != preXapInfo.Version)
                            {
                                changedXapVersionList.Add(new XapVersion
                                {
                                    Title = latestXapInfo.Title,
                                    XapName = latestXapInfo.XapName,
                                    PublishDate = latestXapInfo.PublishDate,
                                    Description = latestXapInfo.Description,
                                    Version = latestXapInfo.Version,
                                    PreVersion = preXapInfo.Version,
                                    UpdateLevel = latestXapInfo.UpdateLevel
                                });
                            }

                            if (preXapInfo == null && latestXapInfo != null)
                            {
                                changedXapVersionList.Add(new XapVersion
                                {
                                    Title = latestXapInfo.Title,
                                    XapName = latestXapInfo.XapName,
                                    PublishDate = latestXapInfo.PublishDate,
                                    Description = latestXapInfo.Description,
                                    Version = latestXapInfo.Version,
                                    PreVersion = preXapInfo.Version,
                                    UpdateLevel = latestXapInfo.UpdateLevel
                                });
                            }
                        }
                    }

                    #endregion

                    if (changedXapVersionList.Count > 0 && XapVersionChangedCompleted != null)
                    {
                        XapVersionChangedCompleted(sender, new CheckXapVersionCompletedEventArgs(ToXapVersionInfo(changedXapVersionList), e.Error, false, e.UserState));
                    }
                }

            }
            catch (Exception ex)
            {
                ComponentFactory.GetComponent<ILog>().LogError(ex, null);
            }
            finally
            {
                if (e.UserState != null)
                {
                    ((Action)e.UserState)();
                }
            }

        }

        void m_Timer_Tick(object sender, EventArgs e)
        {
            CheckXapVersionAsync(null);
        }

        public string GetXapVersion(string xapName)
        {
            var version = string.Empty;

            var xapInfo = this.XapVersionList.SingleOrDefault(xapVersion => xapVersion.XapName.ToLower() == xapName.ToLower());
            if (xapInfo != null)
            {
                version = xapInfo.Version;
            }

            return version;
        }

        private List<XapVersionInfo> ToXapVersionInfo(List<XapVersion> list)
        {
            var resut = new List<XapVersionInfo>();

            foreach (var item in list)
            {
                resut.Add(new XapVersionInfo
                {
                    Title = item.Title,
                    XapName = item.XapName,
                    Version = item.Version,
                    PreVersion = item.PreVersion,
                    PublishDate = item.PublishDate,
                    UpdateLevel = item.UpdateLevel,
                    Description = item.Description
                });
            }

            return resut;
        }


        #region IComponent Members

        public string Name
        {
            get { return "XapVersionController"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(Controls.IPageBrowser browser)
        {

        }

        public object GetInstance(TabItem tab)
        {
            return new XapVersionController();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion


    }
}
