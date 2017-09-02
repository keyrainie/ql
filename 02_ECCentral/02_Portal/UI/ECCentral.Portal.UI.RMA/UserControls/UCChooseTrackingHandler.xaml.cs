using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Service.RMA.Restful.RequestMsg;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCChooseTrackingHandler : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }
        public RMADispatchTrackingVM vm;
        List<ValidationEntity> Validation;
        public UCChooseTrackingHandler()
        {
            LoadComboBoxData();
            this.DataContext = vm = new RMADispatchTrackingVM();
            InitializeComponent();
            BuildValidate();
        }
        private void BuildValidate()
        {
            Validation = new List<ValidationEntity>();
            Validation.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRMATracking.Msg_UnSelectHandler));
        }
        private void LoadComboBoxData()
        {
            RMATrackingFacade facade = new RMATrackingFacade(CPApplication.Current.CurrentPage);
            facade.GetRMATrackingHandleUsers(true, (obj, args) =>
            {
                this.Combo_TrackingHandlers.ItemsSource = args.Result;
            });
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            this.Dialog.Close(true);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.Combo_TrackingHandlers, Validation))
                return;
            RMATrackingBatchActionReq request = new RMATrackingBatchActionReq();
            request.HandlerSysNo = vm.HandlerSysNo;
            request.SysNoList = vm.SysNoList;
            RMATrackingFacade facade = new RMATrackingFacade(CPApplication.Current.CurrentPage);
            facade.Dispatch(request, (obj, args) =>
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
}
