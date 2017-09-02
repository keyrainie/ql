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

namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class CommissionToCashAuditComplete : UserControl
    {
        public IDialog Dialog { get; set; }
        public CommissionToCashVM Data { private get; set; }
        private CommissionToCashFacade facade;
        public CommissionToCashAuditComplete()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new CommissionToCashFacade();
                Data.NewPayAmt = (Convert.ToDecimal(string.IsNullOrEmpty(Data.OldPayAmt) ? "0" : Data.OldPayAmt) + Convert.ToDecimal(string.IsNullOrEmpty(Data.Bonus) ? "0" : Data.Bonus)).ToString();
                this.DataContext = Data;
            };
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            CommissionToCashVM vm = this.DataContext as CommissionToCashVM;
            vm.SysNo = Data.SysNo;
            facade.AuditCommisonToCash(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("审核成功!");
                CloseDialog(DialogResultType.OK);
            });
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
