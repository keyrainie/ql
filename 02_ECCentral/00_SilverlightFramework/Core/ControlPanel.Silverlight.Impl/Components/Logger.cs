using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.LoggingService;
using Newegg.Oversea.Silverlight.Utilities;
using System.Windows.Browser;
using System.Windows;


namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class Logger : ILog
    {
        WriteLogClient m_LogServiceClient = new WriteLogClient();

        #region ILog Members

        public string WriteLog(string content, string category)
        {
            return WriteLog(content, category, CPApplication.Current.LocalRegionName, CPApplication.Current.GlobalRegionName, null, CPApplication.Current.LoginUser.ID, null);
        }

        public string WriteLog(string content, string category, string localName, string globalName)
        {
            return WriteLog(content, category, localName, globalName, null, CPApplication.Current.LoginUser.ID, null);
        }

        public string WriteLog(string content, string category, string referenceKey)
        {
            return WriteLog(content, category, CPApplication.Current.LocalRegionName, CPApplication.Current.GlobalRegionName, referenceKey, CPApplication.Current.LoginUser.ID, null);
        }

        public string WriteLog(string content, string category, string referenceKey, string logUserName, Dictionary<string, object> extendedProperties)
        {
            return WriteLog(content, category, CPApplication.Current.LocalRegionName, CPApplication.Current.GlobalRegionName, referenceKey, logUserName, extendedProperties);
        }

        public string WriteLog(string content, string category, string localName, string globalName, string referenceKey, string logUserName, Dictionary<string, object> extendedProperties)
        {
            LogEntryContract logMsg = new LogEntryContract();
            logMsg.Body = new LogEntryBody();
            logMsg.Body.ID = Guid.NewGuid().ToString();
            logMsg.Body.GlobalName = globalName;
            logMsg.Body.LocalName = localName;
            logMsg.Body.LogServerIP = CPApplication.Current.ClientIPAddress;
            logMsg.Body.LogServerName = CPApplication.Current.ServerComputerName;
            logMsg.Body.CategoryName = category;
            logMsg.Body.Content = content;
            logMsg.Body.LogUserName = logUserName;
            logMsg.Body.ReferenceKey = referenceKey;
            logMsg.Body.LogCreateDate = DateTime.Now;


            if (extendedProperties != null && extendedProperties.Count > 0)
            {
                int i = 0;
                Dictionary<string, string> pp = new Dictionary<string, string>();
                foreach (KeyValuePair<string, object> pair in extendedProperties)
                {
                    #region Serialize propertyValue

                    string propertyValue;
                    if (pair.Value == null)
                    {
                        propertyValue = "NULL";
                    }
                    else if (pair.Value is string)
                    {
                        propertyValue = Convert.ToString(pair.Value).Replace("]]>", "]] >");
                    }
                    else if (pair.Value.GetType().IsPrimitive)
                    {
                        propertyValue = pair.Value.ToString();
                    }
                    else
                    {
                        try
                        {
                            propertyValue = UtilityHelper.XmlSerialize(pair.Value);
                        }
                        catch (Exception ex)
                        {
                            propertyValue = string.Format("Serialize {0} failed, Reason: {1}.", pair.Value.GetType().ToString(), ex.ToString());
                        }
                    }
                    #endregion

                    pp.Add(pair.Key, propertyValue);

                    i++;
                }
                logMsg.Body.ExtendedProperties = pp;
            }

            m_LogServiceClient.LogAsyncAsync(logMsg);

            return logMsg.Body.ID;
        }


        public string LogError(Exception ex)
        {
            return LogError(ex, CPApplication.Current.LocalRegionName, CPApplication.Current.GlobalRegionName, null, null);
        }

        public string LogError(Exception ex, object[] methodArguments)
        {
            return LogError(ex, CPApplication.Current.LocalRegionName, CPApplication.Current.GlobalRegionName, null, methodArguments);
        }

        public string LogError(Exception ex, string message, object[] methodArguments)
        {
            return LogError(ex, CPApplication.Current.LocalRegionName, CPApplication.Current.GlobalRegionName, message, methodArguments);
        }

        public string LogError(Exception ex, string localName, string globalName, string message, object[] methodArguments)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                BusinessException bizEx = ex as BusinessException;
                if (bizEx == null || bizEx.NeedLog)
                {
                    string error = GetExceptionDetail(ex);
                    Dictionary<string, object> extendedProperties = new Dictionary<string, object>();
                    extendedProperties.Add("Method Arguments Type", BuildArgumentTypeDescription(methodArguments));
                    extendedProperties.Add("Method Arguments Value", BuildArgumentValueDescription(methodArguments));
                    extendedProperties.Add("Browser Information", BuildBrowserVersion());
                    if (CPApplication.Current.CurrentPage != null)
                    {
                        extendedProperties.Add("Page Url", CPApplication.Current.CurrentPage.Context.Request.URL);
                    }

                    if (message != null)
                    {
                        extendedProperties.Add("Error Message", message);
                    }

                    WriteLog(error, "ExceptionLog", localName, globalName, null, CPApplication.Current.LoginUser == null ? string.Empty : CPApplication.Current.LoginUser.ID, extendedProperties);
                }
            });

            return string.Empty;
        }


        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "Logger"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
        }

        public object GetInstance(TabItem tab)
        {
            return this;
        }

        public void Dispose()
        {
        }
        #endregion

        private string BuildBrowserVersion()
        {
            if (!Application.Current.IsRunningOutOfBrowser)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Platform: " + HtmlPage.BrowserInformation.Platform);
                sb.AppendLine("ProductName: " + HtmlPage.BrowserInformation.ProductName);
                sb.AppendLine("ProductVersion: " + HtmlPage.BrowserInformation.ProductVersion);
                sb.AppendLine("UserAgent: " + HtmlPage.BrowserInformation.UserAgent);
                return sb.ToString();
            }
            else
            {
                return "Out of Browser";
            }
        }

        private string GetExceptionDetail(Exception ex)
        {
            if (ex == null)
                return "";
            StringBuilder sb = new StringBuilder(1000);
            if (ex.Message != null)
            {
                sb.AppendFormat("Message: {0}.\r\n", ex.Message);
            }
            sb.AppendFormat("Exception Type: {0}.\r\n", ex.GetType().FullName);
            if (ex.StackTrace != null)
            {
                sb.AppendFormat("Stack Trace: {0}.\r\n", ex.StackTrace);
            }
            if (ex.InnerException != null)
            {
                AppendInnerException(ex.InnerException, sb);
            }

            return sb.ToString();
        }

        private void AppendInnerException(Exception ex, StringBuilder sb)
        {
            sb.Append("\r\n");
            sb.AppendFormat("Inner Exception:\r\n");
            sb.AppendFormat("\tMessage: {0}. \r\n", ex.Message);
            sb.AppendFormat("\tException Type: {0}.\r\n", ex.GetType().FullName);
            if (ex.StackTrace != null)
            {
                sb.AppendFormat("\tStack Trace: {0}.\r\n", ex.StackTrace);
            }
            if (ex.InnerException != null)
            {
                AppendInnerException(ex.InnerException, sb);
            }
        }

        private string BuildArgumentValueDescription(object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return "N/A";
            }

            string result = string.Empty;
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] != null)
                {
                    if (arguments[i] is string)
                    {
                        result += (string)arguments[i];
                    }
                    else if (arguments[i].GetType().IsPrimitive)
                    {
                        result += arguments[i].ToString();
                    }
                    else
                    {
                        try
                        {
                            result += UtilityHelper.XmlSerialize(arguments[i]);
                        }
                        catch (Exception ex)
                        {
                            result += string.Format("Serialize {0} failed, Reason: {1}.", arguments[i].GetType().ToString(), ex.ToString());
                        }
                    }
                    if (i != arguments.Length - 1)
                    {
                        result += ", ";
                    }
                }
            }
            return result;
        }

        private string BuildArgumentTypeDescription(object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return "N/A";
            }
            string result = string.Empty;

            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == null)
                {
                    result += "NULL";
                }
                else
                {
                    result += arguments[i].GetType().ToString();
                }

                if (i != arguments.Length - 1)
                {
                    result += ", ";
                }
            }
            return result;
        }
    }

}
