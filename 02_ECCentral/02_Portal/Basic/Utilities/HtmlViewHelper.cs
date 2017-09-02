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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using System.IO;
using System.Windows.Browser;
using System.Threading;

namespace ECCentral.Portal.Basic.Utilities
{
    public static class HtmlViewHelper
    {
        public static void ViewHtmlInBrowser(string domainName, string htmlContent, Point? topLeft, Size? size, bool? showBars, bool? resizable)
        {
            string url = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl") + "/HtmlView.ashx?random=" + Guid.NewGuid().ToString(); // 生成一个Guid在url里，保证每次的url都不一样，防止浏览器缓存
            UtilityHelper.OpenWebPageByPost(new Uri(url), new Dictionary<string, string>
            {
                { "Content", htmlContent }
            }, topLeft, size, showBars, resizable);
        }

        public static void ViewHtmlInBrowser(string domainName, string htmlContent)
        {
            ViewHtmlInBrowser(domainName, htmlContent, null, null, null, null);
        }
        
        public static void WebPrintPreview(string domainName, string printerName, Dictionary<string, string> postData)
        {
            WebPrintPreview(domainName, printerName, postData, null, null, null, null);
        }

        public static void WebPrintPreview(string domainName, string printerName, Dictionary<string, string> postData,
            Point? topLeft, Size? size, bool? showBars, bool? resizable)
        {
            string sessionID = Guid.NewGuid().ToString();
            string url = string.Format("{0}/WebPrinter.ashx?sessionID={1}&ECCentral_WebPrinter_Name={2}&ECCentral_WebPrinter_languageCode={3}",
                CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl"),
                sessionID, printerName, Thread.CurrentThread.CurrentCulture.Name);
            UtilityHelper.OpenWebPageByPost(new Uri(url), postData, topLeft, size, showBars, resizable);
        }
        public static void PreviewPageShow(string jsFunctionName, string content)
        {
            HtmlPage.Window.Invoke(jsFunctionName, content);
        }
    }
}
