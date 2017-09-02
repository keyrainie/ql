using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 系统账户加积分
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class SysAccountAddPoint : PageBase
    {
        private AdjustSysAccountPointVM vm;
        private SysAccountAjustPointFacade facade;

        public SysAccountAddPoint()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new SysAccountAjustPointFacade(this);
            LoadComboBoxData();
            this.DataContext = vm = new AdjustSysAccountPointVM();
        }

        private void LoadComboBoxData()
        {
            facade.LoadSysAccountList("newegg", true, (obj, args) =>
            {
                this.vm.SysAccountList = args.Result;
                Combox_SysAccount.SelectedIndex = 0;
            });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SysAccountAddPoint_AddPoint))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            ValidationManager.Validate(this.LayoutRoot);
            if (vm.HasValidationErrors)
            {
                return;
            }
            facade.AjustPoint(vm, (obj, args) =>
            {
                Window.Alert(ResSysAccountAddPoint.Msg_AdjustSuccess);
                facade.LoadSysAccountValidScore(int.Parse(vm.CustomerSysNo), (obj1, args1) =>
                {
                    Text_CurrentPoint.Visibility = Visibility.Visible;
                    Text_CurrentPoint.Text = string.Format(ResSysAccountAddPoint.Message_CurrentAccountPoint, args1.Result.ToString());
                    

                    vm.Memo = string.Empty;
                    vm.Point = string.Empty;
                    
                    
                });
            });
        }

        private void Combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Combox_SysAccount.SelectedValue != null)
            {
                facade.LoadSysAccountValidScore(int.Parse(vm.CustomerSysNo), (obj, args) =>
                {
                    Text_CurrentPoint.Visibility = Visibility.Visible;
                    Text_CurrentPoint.Text = string.Format(ResSysAccountAddPoint.Message_CurrentAccountPoint, args.Result.ToString());
                });
            }
            else
            {
                Text_CurrentPoint.Visibility = Visibility.Collapsed;
                Text_CurrentPoint.Text = string.Empty;
            }
        }
    }
}