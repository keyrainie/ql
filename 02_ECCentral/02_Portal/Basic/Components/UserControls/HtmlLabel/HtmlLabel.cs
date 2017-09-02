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
using System.Text;
using System.Windows.Browser;

namespace ECCentral.Portal.Basic.Components.UserControls.HtmlLabel
{
    public class HtmlLabel : RichTextBox
    {
        public static readonly DependencyProperty HtmlTextProperty = DependencyProperty.Register("HtmlText",
            typeof(string), typeof(HtmlLabel), new PropertyMetadata(string.Empty,
                new PropertyChangedCallback(HtmlTextPropertyChangedCallback)));


        public string HtmlText
        {
            get
            {                
                return base.GetValue(HtmlTextProperty).ToString();
            }
            set
            {
                base.SetValue(HtmlTextProperty, value);

            }
        }

        private static void HtmlTextPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            HtmlLabel control = obj as HtmlLabel;

            string newStr = string.Empty;

            if (e.NewValue != null)
            {
                newStr = e.NewValue.ToString().Trim();
            }
            if (!string.IsNullOrEmpty(newStr))
            {
                control.SelectAll();
                control.Selection.Xaml = TransformToXamlFormat(newStr);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.IsReadOnly = true;
            this.Background = new SolidColorBrush() { Color = Colors.Transparent };
            this.BorderBrush = new SolidColorBrush() { Color = Colors.Transparent };
        }

        private static string TransformToXamlFormat(string textString)
        {
            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbFooter = new StringBuilder();
            StringBuilder sbBody = new StringBuilder();
            sbHeader.Append("<Section xml:space=\"preserve\" HasTrailingParagraphBreakOnPaste=\"False\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
            sbFooter.Append("</Section>");

            /*********************************************************************************/
            //集中在这里做处理，对于<BR><P><DIV>使用<Paragraph>来替换(Div样式是无法处理的)
            //其它使用<Run>，在Run中设置各种属性来调整颜色、字体等
            //结构：<Paragraph> -> <Run>


            sbBody.Append("<Paragraph>");
            textString = HttpUtility.HtmlEncode(textString);

            sbBody.Append(string.Format("<Run>{0}</Run>", textString));
            sbBody.Append("</Paragraph>");
            /*********************************************************************************/

            string xamlString = sbHeader.ToString() + sbBody.ToString() + sbFooter.ToString();
            return xamlString;
        }
    }
}
