using System.Windows;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.UserControls.Product
{
    public partial class ProductProfileTemplateMaintain : UserControl
    {
      
        #region 属性

        public IDialog Dialog { get; set; }

        public string Text
        {
            get 
            { 
                var value = tbTemplateName.Text ?? "";
                return value;
            }
        }
     
        /// <summary>
        /// 依赖属性：审核理由,支持Silverlight绑定等特性
        /// </summary>
        public string Heard
        {
            get
            {
                var value = this.tb_Heard.Text ?? "";
                return value.Trim();
            }
            set { tb_Heard.Text = value; }
          
        }
      
        #endregion

        #region 初始化加载

        public ProductProfileTemplateMaintain()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        #endregion

        #region 按钮事件

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var value = this.tbTemplateName.Text ?? "";
            if (string.IsNullOrEmpty(value.Trim()))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("必须输入模板名称.", MessageType.Warning);
                return;
            }
            CloseDialog(DialogResultType.OK);
        }

      

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.tbTemplateName.Text = "";
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
