using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Service.RMA.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class RegisterMemo : PageBase
    {
        private RegisterMemoVM vm;
        private int registerSysNo = 0;
        public RegisterMemo()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            vm = new RegisterMemoVM();
            string no = this.Request.Param;
            if (!string.IsNullOrEmpty(no) && int.TryParse(no, out registerSysNo))
            {
                LoadRegisterMemo(registerSysNo);
            }
            else
            {
                Window.Alert(ResRegisterMemo.Msg_RegisterSysNoError, ResRegisterMemo.Msg_RegisterSysNoError, MessageType.Warning, (obj, args) =>
                {
                    Window.Close();
                });
                //Window.Alert(ResRegisterMemo.Msg_RegisterSysNoError);
                //Window.Close();
            }

        }

        private void LoadRegisterMemo(int registerSysNo)
        {
            if (registerSysNo > 0)
            {
                new RegisterFacade(this).LoadRegisterMemo(registerSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm = args.Result.Convert<RegisterMemoRsp, RegisterMemoVM>();
                    this.DataContext = vm;
                });
            }
        }

        private void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.LayoutRoot);
            if (vm.HasValidationErrors)
            {
                return;
            }
            RMATrackingVM trackingVM = new RMATrackingVM();
            trackingVM.RegisterSysNo = vm.RegisterSysNo;
            trackingVM.Content = vm.Content;

            new RMATrackingFacade(this).Create(trackingVM, (obj, args) =>
            {
                Window.Alert(ResRegisterMemo.Msg_CreateSuccess);
                Window.Refresh();
            });
        }

    }
}
