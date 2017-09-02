using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Resources;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Resources;
using Newegg.Oversea.Silverlight.Utilities.Compression;
using Newegg.Oversea.Silverlight.Utilities.Serializer;
using System.Threading;
using System.Runtime.InteropServices.Automation;
using System.IO.IsolatedStorage;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Windows.Controls;
using System.Windows.Media;

namespace Newegg.Oversea.Silverlight.Utilities
{
    public static class UtilityHelper
    {
        private const string CONST_GUIDPATTERN = @"^[A-Fa-f0-9]{8}(-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}$";
        private const string s_LanguageCookieKey = "Portal_Language";
        private const string s_ThemeCookieKey = "Portal_Theme";
        private const string s_CompanyCode = "Portal_CurrentCompanyCode";
        private const string s_CompanyName = "Portal_CurrentCompanyName";

        public static List<string> LanguageList = new List<string> { "zh-CN", "en-US", "zh-TW", "ja-JP"};

        public static string GetIsolatedStorage(string key)
        {
            return IsolatedStoreageHelper.Read(key) as string;
        }

        public static void SetIsolatedStorage(string key, string value)
        {
            IsolatedStoreageHelper.Write(key, value);
        }

        public static T DeepClone<T>(T t)
        {
            return XmlDeserialize<T>(XmlSerialize<T>(t));
        }

        public static object DeepClone(object obj, Type type)
        {
            return XmlDeserialize(XmlSerialize(obj), type);
        }

        public static object DeepClone(object obj)
        {
            return BinaryDeserialize(BinarySerialize(obj));
        }

        public static T LoadFromXml<T>(string fileName)
        {
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return (T)serializer.Deserialize(fs);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        public static void SaveToXml<T>(string fileName, T data)
        {
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                serializer.Serialize(fs, data);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        public static string XmlSerialize<T>(T serialObject)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StringWriter(sb))
            {
                ser.Serialize(writer, serialObject);
                return writer.ToString();
            }
        }

        public static string XmlSerialize(object serialObject)
        {
            StringBuilder sb = new StringBuilder(5000);
            XmlSerializer ser = new XmlSerializer(serialObject.GetType());
            using (TextWriter writer = new StringWriter(sb))
            {
                ser.Serialize(writer, serialObject);
                return writer.ToString();
            }
        }

        public static T XmlDeserialize<T>(string str)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(str))
            {
                return (T)mySerializer.Deserialize(reader);
            }
        }

        public static object XmlDeserialize(string str, Type type)
        {
            XmlSerializer mySerializer = new XmlSerializer(type);
            using (TextReader reader = new StringReader(str))
            {
                return mySerializer.Deserialize(reader);
            }
        }

        public static byte[] BinarySerialize(object serialObject)
        {
            return BinarySerializer.Serialize(serialObject);
        }

        public static void BinarySerialize(object serialObject, Stream outputStream)
        {
            BinarySerializer.Serialize(serialObject, outputStream);
        }

        public static T BinaryDeserialize<T>(byte[] bytes) where T : class
        {
            return BinarySerializer.Deserialize<T>(bytes);
        }

        public static T BinaryDeserialize<T>(Stream inputStream) where T : class
        {
            return BinarySerializer.Deserialize<T>(inputStream);
        }

        public static object BinaryDeserialize(byte[] bytes)
        {
            return BinarySerializer.Deserialize(bytes);
        }

        public static object BinaryDeserialize(Stream inputStream)
        {
            return BinarySerializer.Deserialize(inputStream);
        }

        public static byte[] DeflateCompress(byte[] bytes)
        {
            return DeflateStream.CompressBuffer(bytes);
        }

        public static byte[] DeflateUnCompress(byte[] bytes)
        {
            return DeflateStream.UncompressBuffer(bytes);
        }

        public static byte[] GZipCompress(byte[] bytes)
        {
            return GZipStream.CompressBuffer(bytes);
        }

        public static byte[] GZipUnCompress(byte[] bytes)
        {
            return GZipStream.UncompressBuffer(bytes);
        }

        /// <summary>
        /// 由于系统提供比较字符串只有一个空格时，会认为比较的字符串不为空。
        /// 该方法是对系统方法的一个补充，即传入字符串有且只有一个空格时，验证字符串为空；
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string input)
        {
            if (input == null)
            {
                return true;
            }

            return input.Trim().Length == 0;
        }

        /// <summary>
        /// 比较2个字符串对象是否相等,区分大小写，去掉首尾空格。
        /// </summary>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <returns>若相等，则为True；反之为False</returns>
        public static bool AreEqualWithTrim(string input1, string input2)
        {
            if (input1 == input2)
            {
                return true;
            }

            if ((input1 == null && input2 != null)
                || (input1 != null && input2 == null))
            {
                return false;
            }

            return input1.Trim() == input2.Trim();
        }

        /// <summary>
        /// 返回一个布尔值，指定两个字符串是否相等，不区分大小写。
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns>若相等，则为True；反之为False。</returns>
        public static bool AreEqualIgnoreCase(string input1, string input2)
        {
            return (input1 == input2) || StringComparer.OrdinalIgnoreCase.Compare(input1, input2) == 0;
        }

        /// <summary>
        /// 将指定字符串的首字母转换为大写字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UpperFirstChar(string s)
        {
            if (TrimString(s) == null || s.Length == 0)
                return s;
            string result = string.Empty;
            string[] tmp = s.Split(' ');
            for (int i = 0; i < tmp.Length; i++)
            {
                result += Upper(tmp[i]);
                if (tmp.Length == 1 || i == tmp.Length - 1)
                {
                }
                else
                {
                    result += " ";
                }
            }
            return result;
        }

        private static string Upper(string s)
        {
            if (s == null || s.Length == 0)
                return s;
            char[] array = s.ToCharArray();
            string result = string.Empty;
            for (int i = 0; i < s.Length; i++)
            {
                if (i == 0)
                {
                    result += array[i].ToString().ToUpper();
                }
                else
                {
                    result += array[i].ToString().ToLower();
                }
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TrimString(string s)
        {
            return s == null ? null : s.Trim();
        }

        public static string FormatShortDate(this DateTime dateTime)
        {
            return dateTime.ToString(MessageResource.ShortDateFormat);
        }

        /// <summary>
        /// 用于服务端返回过来的Code，来判断是业务异常，还是服务端异常
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static bool IsBusinessException(string errorCode)
        {
            if (!string.IsNullOrEmpty(errorCode) && !Regex.IsMatch(errorCode, CONST_GUIDPATTERN))
            {
                return true;
            }

            return false;
        }

        public static ObservableCollection<T> ConvertTo<T>(this IList list) where T : ModelBase, new()
        {
            if (list != null)
            {
                ObservableCollection<T> result = new ObservableCollection<T>();

                foreach (ModelBase item in list)
                {
                    result.Add(item.ConvertTo<T>());
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Url地址导航
        /// Browser模式下，开启新的浏览器Tab打开页面； OOB模式下，开启IE浏览器打开页面；
        /// </summary>
        /// <param name="url"></param>
        public static void OpenPage(string url)
        {
            OpenPage(url, null);
        }

        /// <summary>
        /// Url地址导航
        /// Browser模式下，开启新的浏览器Tab打开页面； OOB模式下，开启IE浏览器打开页面；
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        public static void OpenPage(string url, WindowOptions options)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (Application.Current.IsRunningOutOfBrowser)
                {
                    var uri = new Uri(url, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri)
                    {
                        uri = new Uri(new Uri(CPApplication.Current.PortalBaseAddress, UriKind.RelativeOrAbsolute), new Uri(url, UriKind.RelativeOrAbsolute));
                    }

                    HyperlinkOperation.OpenWebPage(uri, "_blank");
                }
                else if (!System.Windows.Application.Current.IsRunningOutOfBrowser)
                {
                    if (options != null)
                    {
                        var opt = GenerateOptions(options);
                        HtmlPage.Window.Navigate(new Uri(url, UriKind.RelativeOrAbsolute), "_blank", opt);
                    }
                    else
                    {
                        HtmlPage.Window.Navigate(new Uri(url, UriKind.RelativeOrAbsolute), "_blank");
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            });
        }

        /// <summary>
        /// 获取当前控件是否可用
        /// </summary>
        /// <param name="relativeTo">相关的控件</param>
        /// <returns>是否可用</returns>
        public static bool IsAvailable(this Control relativeTo)
        {
            return IsAvailable(relativeTo, Application.Current.RootVisual);
        }

        /// <summary>
        /// 获取当前控件是否可用
        /// </summary>
        /// <param name="relativeTo">相关的控件</param>
        /// <param name="container">相关控件的所在的容器</param>
        /// <returns>是否可用</returns>
        public static bool IsAvailable(this Control relativeTo, UIElement container)
        {
            Point p = new Point();

            if (relativeTo != null)
            {
                if (!relativeTo.IsEnabled)
                {
                    return false;
                }

                try
                {
                    GeneralTransform transform = relativeTo.TransformToVisual(container);
                    p = transform.Transform(new Point(0, 0));
                    UIElement element = VisualTreeHelper.FindElementsInHostCoordinates(p, container).FirstOrDefault();
                    return element != null;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }


        /// <summary>
        /// 重启当前应用程序
        /// 1. 在Browser模式下，为刷新页面
        /// 2. 在OOB模式下，为重启应用程序
        /// </summary>
        public static void RestartApplication()
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                if (AutomationFactory.IsAvailable)
                {
                    try
                    {
                        var slLauncherCommand = GetSLLauncherCommand();

                        if (!string.IsNullOrEmpty(slLauncherCommand))
                        {
                            using (dynamic shell = AutomationFactory.CreateObject("WScript.Shell"))
                            {
                                shell.Run(slLauncherCommand);
                            }
                            Application.Current.MainWindow.Close();
                        }
                        else
                        {
                            if (CPApplication.Current.CurrentPage != null)
                            {
                                CPApplication.Current.CurrentPage.Context.Window.Alert(MessageResource.LbInfoTitle, MessageResource.LbRestartApplication, Controls.Components.MessageType.Information, (sender, e) =>
                                {
                                    Application.Current.MainWindow.Close();
                                });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (CPApplication.Current.CurrentPage != null)
                        {
                            //CPApplication.Current.CurrentPage.Context.Window.Logger.LogError(e, new object[] { CPApplication.Current.LoginUser });
                            CPApplication.Current.CurrentPage.Context.Window.Alert(MessageResource.LbInfoTitle, MessageResource.LbRestartApplication, Controls.Components.MessageType.Information, (sender, args) =>
                            {
                                Application.Current.MainWindow.Close();
                            });
                        }
                    }
                }
                else
                {
                    if (CPApplication.Current.CurrentPage != null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(MessageResource.LbInfoTitle, MessageResource.LbRestartApplication, Controls.Components.MessageType.Information, (sender, e) =>
                        {
                            Application.Current.MainWindow.Close();
                        });
                    }
                }
            }
            else
            {
                if (CPApplication.Current.CurrentPage != null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.LoadingSpin.Show();
                }

                var script = (ScriptObject)HtmlPage.Window.GetProperty("reloadPage");
                script.InvokeSelf();
            }
        }

        public static string GetCurrentLanguageCode(string defatulCulture)
        {
            string language = UtilityHelper.GetIsolatedStorage(s_LanguageCookieKey);

            if (!UtilityHelper.IsNullOrEmpty(language))
            {
                language = language.Trim();
            }
            else
            {
                string localLanguage = Thread.CurrentThread.CurrentCulture.Name;
                if (LanguageList.FirstOrDefault(f => f.ToUpper() == localLanguage.ToUpper()) != null)
                {
                    language = localLanguage;
                }
                else
                {
                    if (string.IsNullOrEmpty(defatulCulture))
                    {
                        language = LanguageList[0];
                    }
                    else
                    {
                        language = defatulCulture;
                    }
                }
            }


            switch (language.ToLower())
            {
                case "chs":
                case "zh-chs":
                case "zh-cn":
                    language = "zh-CN";
                    break;

                case "cht":
                case "zh-cht":
                case "zh-tw":
                    language = "zh-TW";
                    break;

                case "ja":
                case "ja-jp":
                    language = "ja-JP";
                    break;

                case "en":
                case "en-us":
                default:
                    language = "zh-CN";
                    break;
            }

            return language;
        }




        public static void SetCurrentLanguageCode(string code)
        {
            string language = code;

            if (!UtilityHelper.IsNullOrEmpty(language))
            {
                language = language.Trim().ToLower();
            }

            switch (language)
            {
                case "chs":
                case "zh-chs":
                case "zh-cn":
                    language = "zh-CN";
                    break;
                case "cht":
                case "zh-cht":
                case "zh-tw":
                    language = "zh-TW";
                    break;
                case "ja":
                case "ja-jp":
                    language = "ja-JP";
                    break;           
                case "en":
                case "en-us":
                default:
                    language = "zh-CN";
                    break;
            }

            UtilityHelper.SetIsolatedStorage(s_LanguageCookieKey, language);
        }


        public static string GetCurrentCompanyCode()
        {
            string company = UtilityHelper.GetIsolatedStorage(s_CompanyCode);

            return company;
        }

        public static void SetCurrentCompanyCode(string code)
        {
            UtilityHelper.SetIsolatedStorage(s_CompanyCode, code);
        }

        public static string GetCurrentCompanyName()
        {
            string name = UtilityHelper.GetIsolatedStorage(s_CompanyName);
            return name;
        }

        public static void SetCurrentCompanyName(string name)
        {
            UtilityHelper.SetIsolatedStorage(s_CompanyName, name);
        }


        public static string GetCurrentThemeCode()
        {
            string theme = UtilityHelper.GetIsolatedStorage(s_ThemeCookieKey);

            if (!UtilityHelper.IsNullOrEmpty(theme))
            {
                theme = theme.Trim().ToLower();
            }

            switch (theme)
            {
                case "default":
                default:
                    theme = "Default";
                    break;
            }

            return theme;
        }

        public static void SetCurrentThemeCode(string code)
        {
            string theme = code;

            if (!UtilityHelper.IsNullOrEmpty(theme))
            {
                theme = theme.Trim().ToLower();
            }

            switch (theme)
            {
                case "default":
                default:
                    theme = "Default";
                    break;
            }

            UtilityHelper.SetIsolatedStorage(s_ThemeCookieKey, theme);
        }

        #region Private Methods

        private static string GetSLLauncherCommand()
        {
            var desktopPath = string.Empty;
            var startMenuPath = string.Empty;
            var slLauncherCmmand = string.Empty;

            using (dynamic wShell = AutomationFactory.CreateObject("WScript.Shell"))
            {
                desktopPath = wShell.SpecialFolders("Desktop");
                startMenuPath = wShell.SpecialFolders("Programs");
            }

            using (dynamic shell = AutomationFactory.CreateObject("Shell.Application"))
            {
                dynamic desktopItems = shell.NameSpace(desktopPath).Items();
                dynamic startMenuItems = shell.NameSpace(startMenuPath).Items();


                FindApplicationInFolder(ref slLauncherCmmand, desktopItems);

                if (string.IsNullOrEmpty(slLauncherCmmand))
                {
                    FindApplicationInFolder(ref slLauncherCmmand, startMenuItems);
                }
            }

            return slLauncherCmmand;
        }

        private static void FindApplicationInFolder(ref string slLauncherCmmand, dynamic items)
        {
            var uri = new Uri(CPApplication.Current.PortalBaseAddress);

            foreach (dynamic item in items)
            {
                if (item.IsLink)
                {
                    dynamic link = item.GetLink();
                    try
                    {
                        if (link.Arguments != null && (link.Arguments.Contains(uri.Host) || link.Arguments.Contains(CPApplication.Current.ServerIPAddress)))
                        {
                            slLauncherCmmand = "\"" + link.Path + "\" " + link.Arguments;
                            break;
                        }
                    }
                    catch (NotImplementedException)
                    {
                        //do nothing here, that's mean in link dynamic type, not found Arguments property.
                    }
                }
            }
        }


        private static string GenerateOptions(WindowOptions options)
        {
            var sb = new StringBuilder();
            sb.Append("scrollbars=yes");

            if (options.Resizable.HasValue)
            {
                sb.Append(",");
                sb.Append("resizable=" + (options.Resizable.Value ? "yes" : "no"));
            }

            if (options.Directories.HasValue)
            {
                sb.Append(",");
                sb.Append("directories=" + (options.Directories.Value ? "yes" : "no"));
            }
            if (options.Location.HasValue)
            {
                sb.Append(",");
                sb.Append("location=" + (options.Location.Value ? "yes" : "no"));
            }
            if (options.Menubar.HasValue)
            {
                sb.Append(",");
                sb.Append("menubar=" + (options.Menubar.Value ? "yes" : "no"));
            }
            if (options.Status.HasValue)
            {
                sb.Append(",");
                sb.Append("status=" + (options.Status.Value ? "yes" : "no"));
            }
            if (options.Toolbar.HasValue)
            {
                sb.Append(",");
                sb.Append("toolbar=" + (options.Toolbar.Value ? "yes" : "no"));
            }


            if (!options.FullScreen)
            {
                if (options.Size.HasValue)
                {
                    sb.Append(",");
                    sb.Append("width=" + options.Size.Value.Width + ",height=" + options.Size.Value.Height);
                }
            }
            else
            {
                sb.Append(",");
                sb.Append("fullscreen=1");
            }

            return sb.ToString();
        }

        #endregion
    }


    public class WindowOptions
    {
        public bool? Directories { get; set; }

        public bool? Location { get; set; }

        public bool? Menubar { get; set; }

        public bool? Status { get; set; }

        public bool? Toolbar { get; set; }

        public bool? Resizable { get; set; }

        public Size? Size { get; set; }

        public bool FullScreen { get; set; }


    }

    public class HyperlinkOperation : HyperlinkButton
    {
        void DoClick()
        {
            this.OnClick();
        }

        public static void OpenWebPage(Uri uri, string targetName)
        {
            HyperlinkOperation btn = new HyperlinkOperation();
            btn.NavigateUri = uri;
            btn.TargetName = targetName;
            btn.DoClick();
        }
    }
}