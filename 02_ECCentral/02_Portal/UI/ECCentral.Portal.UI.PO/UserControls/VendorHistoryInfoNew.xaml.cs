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
using ECCentral.Portal.UI.PO.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class VendorHistoryInfoNew : UserControl
    {
        public IDialog Dialog { get; set; }
        public VendorFacade serviceFacade;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public VendorHistoryLogVM vendorHistoryVM;
        public int VendorSysNo;

        public VendorHistoryInfoNew(int vendorSysNo)
        {
            VendorSysNo = vendorSysNo;
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(VendorHistoryInfoNew_Loaded);
            vendorHistoryVM = new VendorHistoryLogVM();
        }

        void VendorHistoryInfoNew_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(VendorHistoryInfoNew_Loaded);
            serviceFacade = new VendorFacade(CPApplication.Current.CurrentPage);
            this.DataContext = vendorHistoryVM;
        }

        private void btnAddVendorHistoryInfo_Click(object sender, RoutedEventArgs e)
        {

            if (!ValidationManager.Validate(LayoutRoot))
            {
                return;
            }
            //添加供应商历史记录:
            VendorHistoryLog newhistoryInfo = new VendorHistoryLog()
            {
                HistoryReason = this.txtHistoryReason.Text,
                HistoryDate = this.datepicker_CreateTime.SelectedDate,
                VendorSysNo = this.VendorSysNo
            };
            serviceFacade.CreateVendorHistoryLog(newhistoryInfo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CurrentWindow.Alert(ResVendorMaintain.AlertMsg_AlertTitle, ResVendorMaintain.InfoMsg_History_AddSuc, MessageType.Information, (obj2, args2) =>
                {
                    if (args2.DialogResult == DialogResultType.Cancel)
                    {
                        this.Dialog.ResultArgs.Data = vendorHistoryVM;
                        this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        this.Dialog.Close(true);
                    }
                });
            });


        }
    }
}
