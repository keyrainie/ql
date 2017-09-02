using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCTrackingInfoMaintain : PopWindow
    {
        private ARWindowFacade trackingInfoFacade;
        private TrackingInfoVM trackingInfoVM;

        private UCTrackingInfoMaintain()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(UCTrackingInfoMaintain_Loaded);
        }

        private void UCTrackingInfoMaintain_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.BaseInfo.DataContext = trackingInfoVM;

            trackingInfoFacade.GetResponsibleUsersForList(false, responsibleUserList =>
            {
                cmbResponsibleUser.ItemsSource = responsibleUserList;
            });
            CodeNamePairHelper.GetList(ConstValue.DomainName_Invoice, ConstValue.Key_LoseType, CodeNamePairAppendItemType.None, (obj, args) =>
            {
                cmbLossType.ItemsSource = args.Result;
            });
        }

        public UCTrackingInfoMaintain(ARWindowFacade facade, TrackingInfoVM vm)
            : this()
        {
            trackingInfoFacade = facade;
            trackingInfoVM = vm;
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.BaseInfo);
            if (flag)
            {
                trackingInfoVM.Note = trackingInfoVM.AppendNote;
                trackingInfoFacade.UpdateTrackingInfo(trackingInfoVM, () => CloseDialog(DialogResultType.OK));
            }
        }

        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}