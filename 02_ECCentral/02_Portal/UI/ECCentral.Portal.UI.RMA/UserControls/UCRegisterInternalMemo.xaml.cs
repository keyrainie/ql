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
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCRegisterpublicMemo : UserControl
    {
        public int RegisterSysNo { get; set; }

        public UCRegisterpublicMemo()
        {
            InitializeComponent();            
        }

        private void btnNewMemo_Click(object sender, RoutedEventArgs e)
        {
            UCCreateRMATracking uc = new UCCreateRMATracking();
            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "RMAInternalMemoSourceType", (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                RMATrackingVM vm = new RMATrackingVM();
                vm.RegisterSysNo = this.RegisterSysNo;
                vm.publicMemoSourceTypes = args.Result;
                uc.DataContext = vm;

            });
            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResRegisterMaintain.PopTitle_NewpublicMemo, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.dataRMARequestList.Bind();
                }
            });
            uc.Dialog = dialog;
        }

        private void btnCloseMemo_Click(object sender, RoutedEventArgs e)
        {
            var list = this.dataRMARequestList.ItemsSource as List<RMATrackingVM>;
            RMATrackingVM vm = list.FirstOrDefault(p => p.IsChecked);
            if (vm == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResRegisterMaintain.Warning_NoItemSelected, MessageType.Warning);
                return;
            }

            UCCloseRMATracking uc = new UCCloseRMATracking();            
            uc.DataContext = vm;
            uc.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResRegisterMaintain.PopTitle_CloseMemo, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.dataRMARequestList.Bind();
                }
            });
        }

        public void BindData(int registerSysNo)
        {
            this.RegisterSysNo = registerSysNo;
            this.dataRMARequestList.Bind();           
        }      

        private void dataRMARequestList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var queryVM = new RMATrackingQueryVM { RegisterSysNo = this.RegisterSysNo.ToString() };
            new RMATrackingFacade(CPApplication.Current.CurrentPage).Query(queryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.dataRMARequestList.ItemsSource = DynamicConverter<RMATrackingVM>.ConvertToVMList(args.Result.Rows);
                this.dataRMARequestList.TotalCount = args.Result.TotalCount;
            });
        }
    }
}
