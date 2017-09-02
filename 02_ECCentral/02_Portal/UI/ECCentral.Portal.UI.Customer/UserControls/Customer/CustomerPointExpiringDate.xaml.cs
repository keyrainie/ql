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
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerPointExpiringDate : UserControl
    {
        #region 属性
        private CustomerPointExpiringDateVM m_ViewModel;
        public CustomerPointExpiringDateVM ViewModel
        {
            get
            {
                return m_ViewModel;
            }
            set
            {

                m_ViewModel = value;
                this.DataContext = m_ViewModel;
            }
        }

        /// <summary>
        /// 父窗口
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        public IDialog DialogHandle { get; set; }

        #endregion

        #region 初始化加载

        public CustomerPointExpiringDate()
        {
            InitializeComponent();
            dpPointExpiringDate.Focus();
        }


        #endregion

        #region 按钮事件

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (this.DialogHandle != null)
            {
                this.DialogHandle.ResultArgs.DialogResult = DialogResultType.Cancel;
                this.DialogHandle.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.PointExpiringDate == null)
            {
                this.dpPointExpiringDate.Validation("请输入有效期！");
                return;
            }
            CustomerFacade facade = new CustomerFacade(CPApplication.Current.CurrentPage);
            facade.UpdateCustomerPointExpiringDate(ViewModel, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CurrentWindow.Alert(ResCustomerPointExpiringDate.Msg_SaveOk, MessageType.Information);
                this.DialogHandle.ResultArgs.DialogResult = DialogResultType.OK;
                this.DialogHandle.Close();
            });

        }

        #endregion
    }
}