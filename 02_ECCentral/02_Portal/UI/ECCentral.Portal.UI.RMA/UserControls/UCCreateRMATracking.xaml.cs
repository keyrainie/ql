using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCCreateRMATracking : UserControl
    {
        public IDialog Dialog { get; set; }

        public UCCreateRMATracking()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                var vm = this.DataContext as RMATrackingVM;
                new RMATrackingFacade(CPApplication.Current.CurrentPage).Create(vm, (obj, args) =>
                {
                    if (Dialog != null)
                    {
                        Dialog.ResultArgs.Data = args;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    }
                });
            }
        }

        private void btnChooseReasonCode_Click(object sender, RoutedEventArgs e)
        {
            UCReasonCodePicker uc = new UCReasonCodePicker();
            uc.ReasonCodeType = BizEntity.Common.ReasonCodeType.RMA;

            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResRMATracking.Dialog_ReasonCode, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var vm = this.DataContext as RMATrackingVM;
                    if (args.Data != null)
                    {
                        var data = (KeyValuePair<string, string>)args.Data;
                        vm.ReasonCodePath = data.Value;
                        vm.ReasonCodeSysNo = int.Parse(data.Key);
                    }
                    else
                    {
                        vm.ReasonCodePath = string.Empty;
                        vm.ReasonCodeSysNo = null;
                    }
                }
            });
            uc.Dialog = dialog;
        }
    }
}
