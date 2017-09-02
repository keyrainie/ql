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
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class CommissionToCashConfirmPay : UserControl
    {
        public IDialog Dialog { get; set; }
        private CommissionToCashFacade facade;
        public CommissionToCashVM Data { private get; set; }
        public CommissionToCashConfirmPay()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new CommissionToCashFacade();
                this.DataContext = new CommissionToCashVM();
            };
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            CommissionToCashVM vm = this.DataContext as CommissionToCashVM;
            vm.NewPayAmt = Data.NewPayAmt;
            vm.SysNo = Data.SysNo;
            vm.AfterTaxAmt = Data.AfterTaxAmt;
            vm.ConfirmToCashAmt = Data.ConfirmToCashAmt;
            facade.ConfirmCommisonToCash(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("确认成功!");
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
