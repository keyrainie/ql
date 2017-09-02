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

namespace ECCentral.Portal.Basic.Components.UserControls.DialogBox
{
   
    public partial class UCDialogBox : UserControl
    {
        /// <summary>
        /// 确定按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void BtnClick(object sender, RoutedEventArgs e);
        public event BtnClick SaveAction;
        public UCDialogBox()
        {
            InitializeComponent();
            this.Visibility = System.Windows.Visibility.Collapsed;

            btnClosePopup.Click += (sender, e) =>
            {
                this.Close();
            };
         
            Application.Current.Host.Content.Resized += (s, e) =>
            {
                theBack.Width = Application.Current.Host.Content.ActualWidth;
                theBack.Height = Application.Current.Host.Content.ActualHeight;

            };

            btnOK.Click += (sender, e) => { if (SaveAction != null) { SaveAction(sender, e); Close(); }; };
            txtInputMessage.GotFocus += (sender, e) =>
            {
                txtInputMessage.Foreground = new SolidColorBrush() { Color = Colors.Black };
            };
            txtInputMessage.LostFocus += (sender, e) =>
            {
                txtInputMessage.Foreground = new SolidColorBrush() { Color = Colors.DarkGray };
            };
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            popMessage.IsOpen = false;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }


        /// <summary>
        /// Show
        /// </summary>
        public void Show()
        {
            popMessage.IsOpen = true;
            this.Visibility = System.Windows.Visibility.Visible;
            btnClosePopup.Focus();
        }
         /// <summary>
        /// 提示信息
        /// </summary>
        public string Title
        {
            get { return tbPopupMessage.Text; }
            set { tbPopupMessage.Text = value; }
        }
        /// <summary>
        /// 文本框前的提示文本
        /// </summary>
        public string Message
        {
            get { return txtMessage.Text; }
            set { txtMessage.Text = value; }
        }
        /// <summary>
        /// 输入的文本，调用该控件的地方获取文本框的值用控件实例名.InputMessage获取
        /// </summary>
        public string InputMessage
        {
            get { return txtInputMessage.Text; }
            set { txtInputMessage.Text = value; }
        }
        /// <summary>
        /// 是否显示输入部分
        /// </summary>
        public System.Windows.Visibility IsInput
        {
            get { return spMessage.Visibility; }
            set { spMessage.Visibility = value; }
        }
    }
}
