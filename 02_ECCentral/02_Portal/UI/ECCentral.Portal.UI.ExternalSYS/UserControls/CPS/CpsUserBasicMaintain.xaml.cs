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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class CpsUserBasicMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public CpsUserVM Data { private get; set; } //编辑的数据源
        private CpsUserFacade facade;
        public CpsUserBasicMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new CpsUserFacade();
                this.DataContext = Data;
            };
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            CpsUserVM vm = this.DataContext as CpsUserVM;
            vm.SysNo = Data.SysNo;
            vm.BasicUser.WebSiteCode = vm.WebSiteType.SelectValue;
            vm.BasicUser.UserType = vm.UserType;
            if (string.IsNullOrEmpty(vm.BasicUser.WebSiteCode))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择网站类型!",MessageType.Error);
                return;
            }
            facade.UpdateBasicUser(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功!");
                CloseDialog(DialogResultType.OK);

            });
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
