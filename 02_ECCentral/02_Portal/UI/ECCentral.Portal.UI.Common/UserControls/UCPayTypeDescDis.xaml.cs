using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.Basic.Utilities;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class UCPayTypeDescDis : UserControl
    {
        private static readonly DependencyProperty DescriptionTextProperty =
           DependencyProperty.Register("DescriptionText", typeof(string), typeof(UCPayTypeDescDis), new PropertyMetadata(string.Empty, (obj, e) =>
           {
               UCPayTypeDescDis tuc = obj as UCPayTypeDescDis;
               if (e.NewValue != null)
               {
                   tuc.ResolveDescription(e.NewValue.ToString());
               }
           }));

        public string DescriptionText
        {
            get
            {
                return GetValue(DescriptionTextProperty).ToString();
            }
            set
            {
                base.SetValue(DescriptionTextProperty, value);
            }
        }

        public UCPayTypeDescDis()
        {
            InitializeComponent();
        }

        private void Hyperlink_Help_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton tLink = sender as HyperlinkButton;
            if (tLink.DataContext != null)
                UtilityHelper.OpenWebPage(tLink.DataContext.ToString());
        }

        string beginTxt = string.Empty, linkTxt = string.Empty, linkUrl = string.Empty, endTxt = string.Empty;
        
        private void ResolveDescription(string desc)
        {
            beginTxt = string.Empty; linkTxt = string.Empty; linkUrl = string.Empty; endTxt = string.Empty;

            ProcessDesc(desc);

            labDescBegin.Text = beginTxt.Trim();
            linkHelp.Content = linkTxt.Trim();
            linkHelp.DataContext = linkUrl.Trim();
            labDescEnd.Text = endTxt.Trim();
            //labDescBegin.Text = desc;
            //linkHelp.Content = desc;
            //linkHelp.DataContext = "http://www.baidu.com";
            //labDescEnd.Text = desc;
        }

        private void ProcessDesc(string desc)
        {
            //去<br>,去空格
            desc = desc.Replace("<br>", string.Empty);
            
            int tBeginIndex = desc.IndexOf("<a");
            int tEndIndex = desc.IndexOf("</a>");
            
            if (tBeginIndex > 0)
            {
                //获取超链接前的字符串
                beginTxt = desc.Substring(0, tBeginIndex);
                //获取<a href="http://union.tenpay.com/act2011/ccb/explain.shtml?ADTAG=CREDIT.HDSJ.GZSM.CCB2"><font color="#499DE6">详情</font></a>
                linkTxt = desc.Substring(tBeginIndex, tEndIndex - tBeginIndex + 4);
                //取url
                Regex reg =new Regex(@"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>(?<text>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mc=reg.Matches(linkTxt);
                linkUrl = mc[0].Groups["href"].Value;
                linkTxt = GetFontTextIfHave(mc[0].Groups["text"].Value);
                //取超链接后的字符串
                if (tEndIndex + 4 != desc.Length - 1)
                    endTxt = desc.Substring(tEndIndex + 4);
            }
            else
                beginTxt = desc;
        }

        private string GetFontTextIfHave(string pstr)
        {
            Regex reg = new Regex(@"<font[^>]+>(?<text>.*?)</font>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match mc = reg.Match(pstr);
            if (mc.Success)
                return mc.Groups["text"].Value;
            return pstr;
        }



    }
}
