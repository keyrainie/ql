using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.Basic.Utilities
{
    public static class UtilityHelper
    {
        private class HyperlinkButtonHelper : System.Windows.Controls.HyperlinkButton
        {
            public void ClickMe()
            {
                this.OnClick();
            }
        }

        public static void OpenWebPage(string uri)
        {
            OpenWebPage(new Uri(uri));
        }

        public static void OpenWebPage(Uri uri)
        {
            if (System.Windows.Application.Current.IsRunningOutOfBrowser)
            {
                HyperlinkButtonHelper btn = new HyperlinkButtonHelper();
                btn.NavigateUri = uri;
                btn.TargetName = "_blank";
                btn.ClickMe();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Navigate(uri, "blank");
            }
        }

        public static void OpenWebPage(Uri uri, Point? topLeft, Size? size, bool? showBars, bool? resizable)
        {
            OpenWebPageByPost(uri, new Dictionary<string, string>(0), topLeft, size, showBars, resizable);
        }

        public static void OpenWebPageByPost(string uri, Dictionary<string, string> postData)
        {
            OpenWebPageByPost(new Uri(uri), postData);
        }

        public static void OpenWebPageByPost(Uri uri, Dictionary<string, string> postData)
        {
            OpenWebPageByPost(uri, postData, null, null, null, null);
        }

        public static void OpenWebPageByPost(Uri uri, Dictionary<string, string> postData, Point? topLeft, Size? size, bool? showBars, bool? resizable)
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                using (dynamic ie = AutomationFactory.CreateObject("InternetExplorer.Application"))
                {
                    ie.Visible = true;
                    if (size.HasValue)
                    {
                        ie.Width = size.Value.Width;
                        ie.Height = size.Value.Height;
                    }
                    if (showBars.HasValue)
                    {
                        ie.StatusBar = showBars.Value;
                        ie.MenuBar = showBars.Value;
                        ie.ToolBar = showBars.Value;
                    }
                    if (topLeft.HasValue)
                    {
                        ie.Left = topLeft.Value.X;
                        ie.Top = topLeft.Value.Y;
                    }
                    if (resizable.HasValue)
                    {
                        ie.Resizable = resizable.Value;
                    }
                    string header = "Content-Type: application/x-www-form-urlencoded\r\n";
                    StringBuilder postStr = new StringBuilder();
                    if (postData != null && postData.Count > 0)
                    {
                        int index = 0;
                        foreach (var entry in postData)
                        {
                            if (index > 0)
                            {
                                postStr.Append("&");
                            }
                            postStr.AppendFormat("{0}={1}", entry.Key, HttpUtility.UrlEncode(HttpUtility.UrlEncode(HttpUtility.HtmlEncode(entry.Value))));
                            index++;
                        }
                    }

                    ie.Navigate(uri.ToString(), 0, null, Encoding.UTF8.GetBytes(postStr.ToString()), header);
                }
            }
            else
            {
                string tmp = Guid.NewGuid().ToString().Split('-')[0];
                StringBuilder script = new StringBuilder();
                script.Append(string.Format(@"var form1 = document.createElement('form');
form1.id='f1';
form1.action='{0}';
form1.target='{1}';
form1.method='post';
document.body.appendChild(form1);", uri, tmp));
                if (postData != null && postData.Count > 0)
                {
                    int index = 0;
                    foreach (var entry in postData)
                    {
                        index++;
                        script.AppendFormat(@"
var input{0}=document.createElement('input');
input{0}.type='hidden';
input{0}.name='{1}';
input{0}.value='{2}';
form1.appendChild(input{0});", index, entry.Key, HttpUtility.UrlEncode(HttpUtility.HtmlEncode(entry.Value)));
                    }
                }
                StringBuilder param = new StringBuilder();
                if (size.HasValue)
                {
                    param.AppendFormat("height={0},", size.Value.Height);
                    param.AppendFormat("width={0},", size.Value.Width);
                }
                if (topLeft.HasValue)
                {
                    param.AppendFormat("top={0},", topLeft.Value.Y);
                    param.AppendFormat("left={0},", topLeft.Value.X);
                }
                if (showBars.HasValue)
                {
                    param.AppendFormat("toolbar={0},menubar={0},location={0},status={0},", (showBars.Value ? "yes" : "no"));
                }
                if (resizable.HasValue)
                {
                    param.AppendFormat("resizable={0},", (resizable.Value ? "yes" : "no"));
                }
                if (param.Length > 0)
                {
                    param.Remove(param.Length - 1, 1);
                }
                script.AppendFormat(@"
window.open('about:blank','{0}','{1}');
form1.submit();
document.body.removeChild(form1);", tmp, param);
                System.Windows.Browser.HtmlPage.Window.Eval(script.ToString());
            }
        }

        /// <summary>
        /// 检查字符串是否包含脚本
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool CheckScript(string content)
        {
            Regex regex = null;
            Match match = null;
            string str = "<script.*?/.*?script.*?>";
            if (!string.IsNullOrEmpty(content))
            {
                regex = new Regex(str, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                match = regex.Match(content.Trim());
                if (match.Success)
                {
                    return false;
                }
            }
            return true;
        }

        public static void WordSegment(string text, EventHandler<RestClientEventArgs<List<string>>> callBack)
        {
            const string relativeUrl = "/WordSegment";
            RestClient restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Common, ConstValue.Key_ServiceBaseUrl) + "/UtilityService");
            restClient.Query<List<string>>(relativeUrl, text, callBack);
        }

        public static void SetReadOnly(this TextBox tb, bool isReadOnly)
        {
            if (isReadOnly)
            {
                tb.IsReadOnly = true;
                tb.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                tb.IsReadOnly = false;
                tb.Background = new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// 设置控件为只读
        /// </summary>
        /// <param name="obj">控件名称</param>
        /// <param name="childrenCount">控件.Children.Count</param>
        public static void ReadOnlyControl(this DependencyObject obj, int childrenCount, bool status)
        {
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject control = VisualTreeHelper.GetChild(obj, i);
                if (control is TextBox)
                {
                    ((TextBox)control).IsReadOnly = status;
                }
                else if (control is ComboBox)
                {
                    ((ComboBox)control).IsEnabled = !status;
                }
                else if (control is HyperlinkButton)
                {
                    ((HyperlinkButton)control).IsEnabled = !status;
                }
                else if (control is CheckBox)
                {
                    ((CheckBox)control).IsEnabled = !status;
                }
                else if (control is RadioButton)
                {
                    ((RadioButton)control).IsEnabled = !status;
                }
                else if (control is System.Windows.Controls.DataGrid)
                {
                    //((System.Windows.Controls.DataGrid)control).IsEnabled = !status;
                }
                else
                {
                    int innerCount = VisualTreeHelper.GetChildrenCount(control);
                    if (innerCount > 0)
                    {
                        ReadOnlyControl(control, innerCount, status);
                    }
                }
            }
        }

        /// <summary>
        /// 查询父类控件类型
        /// </summary>
        /// <typeparam name="T">父类控件类型</typeparam>
        /// <param name="obj">当前控件</param>
        /// <param name="name">父类控件名称</param>
        /// <returns>父类控件</returns>
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        /// <summary>
        /// 查询子类控件类型
        /// </summary>
        /// <typeparam name="T">子类控件类型</typeparam>
        /// <param name="obj">当前控件</param>
        /// <param name="name">子类控件名称</param>
        /// <returns>子类控件</returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取类基累积型泛
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static Type GetGenericType(object source, Type sourceType)
        {
            Type targe = sourceType;
            if (sourceType == null) return null;
            while (source != null && targe.IsGenericType
                && targe.GetGenericTypeDefinition() == typeof(Nullable<>)
                && targe.GetGenericArguments().Length == 1)
            {
                targe = targe.GetGenericArguments()[0];
            }
            return targe;
        }

        public static void ThrowExcetion(string content, params object[] args)
        {
            throw new Exception(string.Format(content, args));
        }
    }
}
