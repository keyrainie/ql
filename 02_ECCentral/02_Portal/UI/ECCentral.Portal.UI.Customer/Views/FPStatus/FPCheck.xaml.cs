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
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views.FPStatus
{
    [View]
    public partial class FPCheck : PageBase
    {
        FPCheckVM viewModel;
        CommonDataFacade commFacade;
        FPCheckFacade fpCheckFacade;
        FPCheckQueryFilter queryRequest;
        List<FPCheckEntityVM> oraginList;
        public FPCheck()
        {
            viewModel = new FPCheckVM();
            queryRequest = new FPCheckQueryFilter();
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            commFacade = new CommonDataFacade(this);
            fpCheckFacade = new FPCheckFacade(this);
            this.DataContext = viewModel;
            base.OnPageLoad(sender, e);
            cbChannelSysNo.SelectedIndex = 0;
            gridFPMaster.Bind();
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_FPCheck_Save))
                this.ButtonSave.IsEnabled = false;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            gridFPMaster.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryRequest.ChannelID = viewModel.WebChannel;
            queryRequest.PagingInfo = new QueryFilter.Common.PagingInfo();
            queryRequest.PagingInfo.PageSize = e.PageSize;
            queryRequest.PagingInfo.PageIndex = e.PageIndex;
            fpCheckFacade.Query(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                oraginList = DynamicConverter<FPCheckEntityVM>.ConvertToVMList(args.Result.Rows);
                gridFPMaster.ItemsSource = oraginList.DeepCopy();
                gridFPMaster.TotalCount = args.Result.TotalCount;
            });
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<ECCentral.BizEntity.Customer.FPCheck>();
            foreach (var item in gridFPMaster.ItemsSource)
            {
                var vm = item as FPCheckEntityVM;
                if (oraginList.Where(p => p.Status == vm.Status && p.SysNo == vm.SysNo).Count() <= 0)
                {
                    list.Add(new ECCentral.BizEntity.Customer.FPCheck() { SysNo = vm.SysNo, FPCheckStatus = (FPCheckStatus)vm.Status });
                }
            }
            fpCheckFacade.BatchUpdate(list, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                Window.Alert(ResFPCheck.msg_SaveSuccess);
                gridFPMaster.Bind();
            });
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            FPCheckEntityVM vm = this.gridFPMaster.SelectedItem as FPCheckEntityVM;
            if (!string.IsNullOrEmpty(vm.Url))
            {
                Window.Navigate(string.Format(vm.Url, vm.ChannelID), null, true);
            }
        }
    }

}
