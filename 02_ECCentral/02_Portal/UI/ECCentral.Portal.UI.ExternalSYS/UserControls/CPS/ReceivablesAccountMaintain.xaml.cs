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

    public partial class ReceivablesAccountMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public CpsUserVM Data { private get; set; } //编辑的数据源
        private CpsUserFacade facade;
        public ReceivablesAccountMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new CpsUserFacade();
                List<BankType> tempdata = new List<BankType>() {new BankType(){SelectValue=null,Description= "--请选择--"} };
                facade.GetBankType((obj, arg) => 
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    foreach (var item in arg.Result.Rows)
                    {
                        tempdata.Add(new BankType() { SelectValue = item.value, Description = item.Description });
                    }
                    Data.ReceivablesAccount.BankTypeList = tempdata;
                    Data.ReceivablesAccount.Bank = (from p in Data.ReceivablesAccount.BankTypeList where p.SelectValue == Data.ReceivablesAccount.BrankCode select p).ToList().FirstOrDefault();
                    if (Data.ReceivablesAccount.Bank == null)
                    {
                        Data.ReceivablesAccount.Bank = (from p in Data.ReceivablesAccount.BankTypeList where p.SelectValue ==null select p).ToList().FirstOrDefault();
                    }
                    this.DataContext = Data;
                 });
              
            };
        }

        private void Btnok_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            CpsUserVM vm = this.DataContext as CpsUserVM;
            vm.SysNo = Data.SysNo;
            if (vm.ReceivablesAccount.Bank.SelectValue == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择银行!", MessageType.Error);
                return;
            }
            vm.ReceivablesAccount.BrankCode = vm.ReceivablesAccount.Bank.SelectValue;
            facade.UpdateCpsReceivablesAccount(vm, (obj, arg) => 
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
