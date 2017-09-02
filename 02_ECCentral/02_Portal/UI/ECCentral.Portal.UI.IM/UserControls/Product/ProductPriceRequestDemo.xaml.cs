using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.UserControls.Product
{
    public partial class ProductPriceRequestDemo : UserControl
    {
       

        #region 属性

        public IDialog Dialog { get; set; }
     
        /// <summary>
        /// 依赖属性：审核理由,支持Silverlight绑定等特性
        /// </summary>
        public string Text
        {
            get
            {
                var value = this.tb_Reason.Text ?? "";
                return value.Trim();
            }
          
        }
      
        #endregion

        #region 初始化加载

        public ProductPriceRequestDemo()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        #endregion

        #region 按钮事件

        private void btnDeny_Click(object sender, RoutedEventArgs e)
        {
            var value = this.tb_Reason.Text ?? "";
            if (string.IsNullOrEmpty(value.Trim()))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("必须输入审核理由.", MessageType.Warning);
                return;
            }
            CloseDialog(DialogResultType.OK);
        }

      

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.tb_Reason.Text = "";
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        #endregion
    }
}
