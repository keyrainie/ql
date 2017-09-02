using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.CommonService;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;


namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI
{
    public class VersionControlWrapper
    {
        private bool m_isFirstCheck = true;
        private List<XapVersionInfo> m_xapVersionList;

        public VersionControlWrapper()
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                Application.Current.CheckAndDownloadUpdateCompleted += new CheckAndDownloadUpdateCompletedEventHandler(Application_CheckAndDownloadUpdateCompleted);
            }
            Application.Current.InstallStateChanged += new EventHandler(Application_InstallStateChanged);
        }

        private static VersionControlWrapper m_current;
        public static VersionControlWrapper Current
        {
            get
            {
                if (m_current == null)
                {
                    m_current = new VersionControlWrapper();
                }
                return m_current;
            }
        }


        public void CheckAndDownloadUpdateAsync()
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
               Application.Current.CheckAndDownloadUpdateAsync();
            }
        }

        public void InitializeVersionController()
        {
            ComponentFactory.GetComponent<IXapVersionController>().AutoCheckIntervalTime = TimeSpan.FromMinutes(8);
            ComponentFactory.GetComponent<IXapVersionController>().XapVersionChangedCompleted += (sender, args) =>
            {
                m_xapVersionList = args.XapVersionList;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!Application.Current.IsRunningOutOfBrowser)
                    {
                            var box = new UpdateWindow(m_xapVersionList);
                            box.ShowWindow();
                    }
                    else
                    {
                        //在OOB模式，判断是否存在主框架XAP包更新
                        var b = m_xapVersionList.Any(item => item.XapName.Equals(CPApplication.Current.FrameworkXapName, StringComparison.OrdinalIgnoreCase));

                        //如果主框架包没有更新，则显示提示框
                        if (!b)
                        {
                            var box = new UpdateWindow(m_xapVersionList);
                            box.ShowWindow();

                        }
                        else
                        {
                            m_isFirstCheck = false;
                            Application.Current.CheckAndDownloadUpdateAsync();
                        }
                    }
                });
            };
        }

        void Application_InstallStateChanged(object sender, EventArgs e)
        {
            if (Application.Current.InstallState == System.Windows.InstallState.Installed)
            {
                var version = ComponentFactory.GetComponent<IXapVersionController>().GetXapVersion(CPApplication.Current.FrameworkXapName);
                UtilityHelper.SetIsolatedStorage(Constants.Key_Framework_Version, version);
            }
        }

        void Application_CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (m_isFirstCheck)
            {
                ProcessWhenCompleted(e);
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var box = new UpdateWindow(m_xapVersionList);
                    box.ShowWindow();
                });
            }
        }

        private void ProcessWhenCompleted(CheckAndDownloadUpdateCompletedEventArgs e)
        {
            //如果有某个应用程序更新可用，但是使用了用户尚未安装的 Silverlight 的新版本，将不下载更新。在此情况下，UpdateAvailable 属性值为 false，并且 Error 属性值为 PlatformNotSupportedException 实例。
            //出现这种情况时，您可以提醒用户打开应用程序的宿主网站，触发基于 HTML 的 Silverlight 升级体验。
            if (e.Error != null &&
                e.Error is PlatformNotSupportedException)
            {
                MessageBox.Show("An application update is available, " +
                    "but it requires a new version of Silverlight. " +
                    "Visit the application home page to upgrade.");

                return;
            }

            XapVersionInfo xapVersionInfo;
            var client = new CommonServiceV40Client();
            client.GetFrameworkVersionAsync();
            client.GetFrameworkVersionCompleted += (obj, args) =>
            {
                if (args.Error != null)
                {
                    throw args.Error;
                }
                if (args.Result.Faults != null && args.Result.Faults.Count > 0)
                {
                    throw new Exception(args.Result.Faults[0].ErrorDetail);
                }

                if (args.Result.Body != null)
                {
                    xapVersionInfo = ToXapVersionInfo(args.Result.Body);

                    if (e.UpdateAvailable)
                    {
                        var version = UtilityHelper.GetIsolatedStorage(Constants.Key_Framework_Version);

                        if (string.IsNullOrEmpty(version))
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                var box = new UpdateWindow(xapVersionInfo);
                                box.ShowWindow();
                            });
                        }
                        else if (!xapVersionInfo.Version.Equals(version, StringComparison.OrdinalIgnoreCase))
                        {
                            xapVersionInfo.PreVersion = version;

                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                var box = new UpdateWindow(xapVersionInfo);
                                box.ShowWindow();
                            });
                        }
                    }

                    UtilityHelper.SetIsolatedStorage(Constants.Key_Framework_Version, xapVersionInfo.Version);
                }
            };
        }

        private XapVersionInfo ToXapVersionInfo(XapVersion xapVersion)
        {
            return new XapVersionInfo
            {
                Title = xapVersion.Title,
                XapName = xapVersion.XapName,
                Version = xapVersion.Version,
                PreVersion = xapVersion.PreVersion,
                PublishDate = xapVersion.PublishDate,
                UpdateLevel = xapVersion.UpdateLevel,
                Description = xapVersion.Description
            };
        }
    }
}
