using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.IO.IsolatedStorage;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.CommonService;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Models;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Views;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Components;

namespace ControlPanel.SilverlightUI
{
    public partial class App : Application
    {
        #region Fields

        private List<LogRegionMappingModel> m_logRegionMappingCollection;
        private int m_loadCompletedCount;

        #endregion

        public MainPage MainPage
        {
            get;
            set;
        }

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        public void InitApp()
        {
            m_loadCompletedCount = 0;

            InitializeParams(() =>
            {
                VersionControlWrapper.Current.CheckAndDownloadUpdateAsync();

                Interlocked.Increment(ref m_loadCompletedCount);
                if (m_loadCompletedCount == 5)
                {
                    CompanyAndWebChannelInit();
                }
            });
            ComponentFactory.GetComponent<IXapVersionController>().CheckXapVersionAsync(() =>
            {
                Interlocked.Increment(ref m_loadCompletedCount);
                if (m_loadCompletedCount == 5)
                {
                    CompanyAndWebChannelInit();
                }
            });

            ComponentFactory.GetComponent<IConfiguration>().LoadConfig(() =>
            {
                Interlocked.Increment(ref m_loadCompletedCount);
                if (m_loadCompletedCount == 5)
                {
                    CompanyAndWebChannelInit();
                }
            });
            ComponentFactory.GetComponent<IUserProfile>().LoadProfileData(obj =>
            {
                Interlocked.Increment(ref m_loadCompletedCount);
                if (m_loadCompletedCount == 5)
                {
                    CompanyAndWebChannelInit();
                }
            });
            //此时已经是登录后，获取配置的ECCentral的ServiceURL
            ComponentFactory.GetComponent<IConfiguration>().GetECCentralServiceURL(() =>
            {
                Interlocked.Increment(ref m_loadCompletedCount);
                if (m_loadCompletedCount == 5)
                {
                    CompanyAndWebChannelInit();
                }
            });

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Dictionary<string, string> dic;
            if (Application.Current.IsRunningOutOfBrowser)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                dic = (Dictionary<string, string>)IsolatedStoreageHelper.Read("InitParams.bin");
            }
            else
            {
                dic = new Dictionary<string, string>(e.InitParams.Count * 2);
                foreach (var entry in e.InitParams)
                {
                    if (!dic.ContainsKey(entry.Key))
                    {
                        dic.Add(entry.Key, entry.Value);
                    }
                }
                IsolatedStoreageHelper.Write("InitParams.bin", dic);
            }
            // 将sliverlight的default.aspx页面的<param name="initParams" value="...." />的数据放入CPApplication.Current
            // 作为配置数据，供应用程序其他地方使用
            if (dic != null && dic.Count > 0)
            {
                CPApplication.Current.InitParams = new ReadOnlyDirectionary<string, string>(dic);
            }
            else
            {
                CPApplication.Current.InitParams = ReadOnlyDirectionary<string, string>.Empty;
            }

            string defatulCulture = string.Empty;
            if (CPApplication.Current.InitParams["defatulCulture"] != null)
            {
                defatulCulture = CPApplication.Current.InitParams["defatulCulture"];
            }
            InitLanguageAndTheme(defatulCulture);

            if (CPApplication.Current.CommonData == null)
            {
                CPApplication.Current.CommonData = new Dictionary<string, object>();
            }
            CPApplication.Current.CommonData.Add("ECCentralServiceURL_Login", CPApplication.Current.InitParams["login"]);

            this.RootVisual = new RootVisualWrapper();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            //If the app is running outside of the debugger then report the exception using
            //the browser's exception mechanism. On IE this will display it a yellow alert 
            //icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                try
                {
                    if (e.ExceptionObject.StackTrace != null 
                        && e.ExceptionObject.StackTrace.StartsWith("at MS.Internal.XcpImports.CheckHResult(UInt32 hr) at MS.Internal.XcpImports.Control_Raise(Control control, IManagedPeerBase arguments, Byte nDelegate) at System.Windows.Controls.TextBox.OnKeyDown(KeyEventArgs e) at System.Windows.Controls.Control.OnKeyDown(Control ctrl, EventArgs e) at MS.Internal.JoltHelper.FireEvent(IntPtr unmanagedObj, IntPtr unmanagedObjArgs, Int32 argsTypeIndex, Int32 actualArgsTypeIndex, String eventName)"))
                    {
                        e.Handled = true;
                        return;
                    }

                    var exceptionObject = e.ExceptionObject as IsolatedStorageException;
                    int sizeToAdd = 25 * 1024 * 1024;
                    if (exceptionObject != null)
                    {
                        if (exceptionObject.Message == "IsolatedStorage_UsageWillExceedQuota")
                        {
                            e.Handled = true;
                            IncreaseIsolatedStoreage(sizeToAdd);
                            return;
                        }
                        else if (exceptionObject.Message == "There is not enough free space to perform the operation.")
                        {
                            e.Handled = true;
                            IncreaseIsolatedStoreage(sizeToAdd);
                            return;
                        }
                    }

                    if (!(e.ExceptionObject is PageException))
                    {
                        e.Handled = true;
                        ErrorWindow.CreateNew(e.ExceptionObject, StackTracePolicy.Always);
                        TraceExceptionLog(e.ExceptionObject);
                    }
                }
                catch
                {
                }
            }
        }

        private void TraceExceptionLog(Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            if (m_logRegionMappingCollection != null && m_logRegionMappingCollection.Count > 0)
            {
                foreach (var mapping in m_logRegionMappingCollection)
                {
                    foreach (var ns in mapping.NamespaceCollection)
                    {
                        if (!ns.IsNullOrEmpty() && !exception.StackTrace.IsNullOrEmpty())
                        {
                            //根据Stack trace, 决定Log记录至哪一个local region(异常邮件通知至对应的Team)
                            if (exception.StackTrace.ToLower().Contains(ns.ToLower()))
                            {
                                ComponentFactory.GetComponent<ILog>().LogError(exception, mapping.LocalRegion, CPApplication.Current.GlobalRegionName, null, null);
                                return;
                            }
                            //根据当前的Page URL, 决定Log记录至哪一个local region(异常邮件通知至对应的Team)
                            if (CPApplication.Current.CurrentPage != null 
                                && CPApplication.Current.CurrentPage.Context != null
                                && CPApplication.Current.CurrentPage.Context.Request != null
                                && CPApplication.Current.CurrentPage.Context.Request.URL != null
                                && !CPApplication.Current.CurrentPage.Context.Request.URL.IsNullOrEmpty())
                            {
                                if (CPApplication.Current.CurrentPage.Context.Request.URL.ToLower().Contains(ns.ToLower()))
                                {
                                    ComponentFactory.GetComponent<ILog>().LogError(exception, mapping.LocalRegion, CPApplication.Current.GlobalRegionName, null, null);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            ComponentFactory.GetComponent<ILog>().LogError(exception, new object[] { CPApplication.Current.LoginUser });
        }

        //获取当前用户所属公司列表，并获得渠道列表，赋到CPApplication中
        private void CompanyAndWebChannelInit()
        {
            ComponentFactory.GetComponent<ICompanyManager>().GetAllCompanies(CPApplication.Current.LoginUser.UserSysNo.Value, () =>
            {
                //加载ShareDll
                ComponentFactory.GetComponent<IAssemblyLoader>().LoadShareAssembly(() =>
                {
                    InitApplication();
                }); 
            });
        }
        

        private void InitApplication()
        {
            InitLogRegionMappingData();
            UserProfileWrapper.InitProfile();

            VersionControlWrapper.Current.InitializeVersionController();

            ComponentFactory.GetComponent<IModuleManager>().Add(new ModuleInfo("Main", typeof(App).Assembly));
            ComponentFactory.GetComponent<IUserProfile>().SyncIntervalTime = TimeSpan.FromMinutes(new Random().Next(30, 120));

            //Create MainPage
            this.MainPage = new MainPage();
            (Application.Current.RootVisual as RootVisualWrapper).LayoutRoot.Children.Add(this.MainPage);
            (Application.Current.RootVisual as RootVisualWrapper).LoginArea.Visibility = Visibility.Collapsed;
            (Application.Current.RootVisual as RootVisualWrapper).BorderLoadingLayer.Visibility = Visibility.Collapsed;
        }

        private void InitLanguageAndTheme(string defatulCulture)
        {
            //Init ThemeCode
            CPApplication.Current.LanguageCode = UtilityHelper.GetCurrentLanguageCode(defatulCulture);
            CPApplication.Current.ThemeCode = UtilityHelper.GetCurrentThemeCode();

            //to build current language
            CultureInfo cultureInfo = new CultureInfo(CPApplication.Current.LanguageCode);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            //to build
            string themeCode = CPApplication.Current.ThemeCode;

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(String.Format("/Newegg.Oversea.Silverlight.Controls.Theming;component/Themes/{0}/BasicResourcesStyle.xaml", themeCode), UriKind.Relative) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(String.Format("/Newegg.Oversea.Silverlight.Controls.Theming;component/Themes/{0}/ThemeStyle.xaml", themeCode), UriKind.Relative) });
        }

        private void InitializeParams(Action callback)
        {
            var client = new CommonServiceV40Client();
            client.GetAppParamsCompleted += (sender, e) =>
            {
                var param = e.Result.Body;

                CPApplication.Current.ServerComputerName = param.ServerComputerName;

                CPApplication.Current.ServerIPAddress = param.ServerIPAddress;

                CPApplication.Current.GlobalRegionName = param.GlobalRegion;

                CPApplication.Current.LocalRegionName = param.LocalRegion;

                CPApplication.Current.DefaultPage = param.DefaultPage;

                CPApplication.Current.PortalBaseAddress = param.HostAddress;

                CPApplication.Current.FrameworkXapName = param.FrameworkXapName;

                CPApplication.Current.ClientIPAddress = param.ClientIPAddress;

                CPApplication.Current.ClientComputerName = param.ClientComputerName;

                if (callback != null)
                {
                    callback();
                }
            };
            client.GetAppParamsAsync();
        }

        private void InitLogRegionMappingData()
        {
            try
            {
                object obj = ComponentFactory.GetComponent<IConfiguration>().GetConfigValue(Constants.Framework_DomainName, Constants.Key_ExceptionLogMapping);
                string strXmlLogRegionMappingCollection = obj as string;
                if (strXmlLogRegionMappingCollection != null)
                {
                    m_logRegionMappingCollection = UtilityHelper.XmlDeserialize<List<LogRegionMappingModel>>(strXmlLogRegionMappingCollection);
                }

            }
            catch (Exception ex)
            {
                ComponentFactory.GetComponent<ILog>().LogError(ex);
            }
        }

        private void IncreaseIsolatedStoreage(long spaceToAdd) 
        {
            if (IsolatedStorageFile.IsEnabled)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    long curAvail = store.AvailableFreeSpace;
                    // If available space is less than
                    // what is requested, try to increase.
                    if (curAvail < spaceToAdd)
                    {
                        // Request more quota space.
                        if (!store.IncreaseQuotaTo(store.Quota + spaceToAdd))
                        {
                            // The user clicked NO to the
                            // host's prompt to approve the quota increase.
                            CPApplication.Current.CurrentPage.Context.Window.Confirm("System Information", "Your Isolated Storage usage need exceed quota to store user profile data. Do your want to continue?", (obj, args) =>
                            {
                                if (args.DialogResult == DialogResultType.OK)
                                {
                                    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                                    {
                                        isoStore.IncreaseQuotaTo(isoStore.Quota + spaceToAdd);
                                    }
                                }
                            }, ButtonType.YesNo, null);
                        }
                    }
                }
            }
        }
    }

}
