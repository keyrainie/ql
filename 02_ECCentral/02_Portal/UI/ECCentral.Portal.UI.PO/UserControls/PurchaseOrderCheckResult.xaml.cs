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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderCheckResult : UserControl
    {
        public string CheckResultString { get; set; }

        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public PurchaseOrderCheckResult(string checkResultString)
        {
            this.CheckResultString = checkResultString;
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PurchaseOrderCheckResult_Loaded);
        }

        void PurchaseOrderCheckResult_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= PurchaseOrderCheckResult_Loaded;
            //this.lblCheckResult.HtmlText = CheckResultString;
            this.lblCheckResult.HtmlText = TransformIPPToEC(CheckResultString);
        }

        //private string TransformIPPToEC(string ippStr)
        //{
        //    if (ippStr.IndexOf("<table>") >= 0)
        //    {
        //        string tmpEC = string.Empty;//临时存储解析过来的EC字符串
        //        string tmpStr = string.Empty;//临时存储
        //        int trLastIndex = 0;
        //        ippStr = ippStr.Replace("<table>", string.Empty).Replace("</table>", string.Empty).Replace("&nbsp;", " ");
        //        Regex reg = new Regex("<[^>]+>");
        //        while (ippStr.IndexOf("<tr>") >= 0)
        //        {
        //            trLastIndex = ippStr.IndexOf("</tr>");
        //            tmpEC += reg.Replace(ippStr.Substring(0, trLastIndex), "") + Environment.NewLine;
        //            ippStr = ippStr.Substring(trLastIndex + 5);
        //        }
        //        ippStr = tmpEC.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
        //    }
        //    return ippStr;
        //}

        private string TransformIPPToEC(string ippStr)
        {
            int column1With = 8;
            int column2With = 30;

            string tmpECOut = string.Empty;
            Regex reg = new Regex("<[^>]+>");

            ippStr = ippStr.Replace("</tr>", "*&&^@")
                .Replace("</td>", "$@$$")
                .Replace("&nbsp;", string.Empty)
                .Replace(Environment.NewLine, string.Empty);

            ippStr = reg.Replace(ippStr, string.Empty);

            string[] tmpRowList = ippStr.Split(new string[] { "*&&^@" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string ss in tmpRowList)
            {
                string[] tmpCellList = ss.Split(new string[] { "$@$$" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < tmpCellList.Length; i++)
                {
                    int tmpLength = 0;
                    if (i == 0) tmpLength = column1With;
                    else if (i == 1) tmpLength = column2With;
                    tmpECOut += GetFixLengthString(tmpCellList[i], tmpLength);
                }
                tmpECOut += Environment.NewLine;
            }

            return tmpECOut;
        }

        /// <summary>
        /// 加工固定长度字符串
        /// 如果字符串小于固定长度：用空格补足
        /// 如果字符串大于固定长度：直接返回字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="fixLength">默认长度(不足用空格补全)</param>
        /// <returns></returns>
        private string GetFixLengthString(string sourceStr,int fixLength)
        {
            int sourceLength = System.Text.Encoding.Unicode.GetByteCount(sourceStr);
            if (sourceLength >= fixLength)
                return sourceStr;

            string tmpStr = string.Empty;
            int tmpNum = fixLength - sourceLength;
            while (tmpNum > 0)
            {
                tmpNum--;
                tmpStr += " ";
            }
            return sourceStr + tmpStr;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //关闭:
            this.Dialog.Close(true);
        }
    }
}
