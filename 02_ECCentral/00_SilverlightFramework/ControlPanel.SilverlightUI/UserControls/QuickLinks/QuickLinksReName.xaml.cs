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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Utilities;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class QuickLinksReName : UserControl
    {
        public IDialog diglog { get; set; }
        protected QuickLinksModel QuickLink{get;set;}
        private string OriginalQuickLinkName=string.Empty;
        public QuickLinksReName()
        {
            InitializeComponent();
            lbOK.Click += new RoutedEventHandler(lbOK_Click);
            lbCancel.Click += new RoutedEventHandler(lbCancel_Click);
        }
        public QuickLinksReName(QuickLinksModel quickLinksModel)
            : this()
        {
            QuickLink = quickLinksModel;
           OriginalQuickLinkName = quickLinksModel.QuickLinkName;
            this.DataContext = QuickLink;
        }
        

        /// <summary>
        /// 取消事件，把原始的值重新付给当前实体，并执行关闭弹出对话框的动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lbCancel_Click(object sender, RoutedEventArgs e)
        {
            QuickLink.QuickLinkName = OriginalQuickLinkName;
            diglog.Close();       
        }
        /// <summary>
        /// 提交事件，
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lbOK_Click(object sender, RoutedEventArgs e)
        {
            if (!txt_Newname.Text.IsNullOrEmpty())
            {
                QuickLink.QuickLinkName = txt_Newname.Text.Trim();
                QuickLink.IsRename = true;
                diglog.ResultArgs.DialogResult = DialogResultType.OK;
                diglog.Close();
            }
            else
            {
                txt_Newname.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                txt_Newname.Focus();
            }
                


        }
    }
}
