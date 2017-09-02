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
using ECCentral.BizEntity.ExternalSYS;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class CpsUserSourceEdit : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        public UserType UserType { get; set; }
        private CpsUserFacade facade;
        private CpsUserVM model;
        public CpsUserSourceEdit()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new CpsUserFacade();
                model = new CpsUserVM() { UserSource = new CpsUserSourceVM() { UserType = UserType } };
                this.DataContext = model;
            };
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            CpsUserVM vm = this.DataContext as CpsUserVM;
            vm.SysNo = SysNo;
            facade.CreateUserSource(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("创建成功!");
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
