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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views.FPStatus
{

    [View]
    public partial class CHSet : PageBase
    {
        private CHSetVM viewModel;
        private CHQueryFilter queryRequest;
        private FPCheckFacade fpCheckFacade;

        public CHSet()
        {
            viewModel = new CHSetVM();
            this.DataContext = viewModel;
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            fpCheckFacade = new FPCheckFacade(this);
            queryRequest = new CHQueryFilter();
            viewModel.ChannelID = this.Request.Param;
            base.OnPageLoad(sender, e);
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CHSet_Add))
                this.Button_New.IsEnabled = false;
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            CHSetMaintain window = new CHSetMaintain(viewModel.IsSearchCategory.Value);
            window.Dialog = Window.ShowDialog(ResCHSetMaintain.title, window, (org, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    DataGrid_ResultList.Bind();
                }
            }, new Size(550, 200));
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            DataGrid_ResultList.Bind();
        }

        private void DataGrid_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryRequest = viewModel.ConvertVM<CHSetVM, CHQueryFilter>();
            queryRequest.PagingInfo = new QueryFilter.Common.PagingInfo();
            queryRequest.PagingInfo.PageSize = e.PageSize;
            queryRequest.PagingInfo.PageIndex = e.PageIndex;
            queryRequest.PagingInfo.SortBy = e.SortField;
            fpCheckFacade.Query(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                DataGrid_ResultList.ItemsSource = args.Result.Rows;
                DataGrid_ResultList.TotalCount = args.Result.TotalCount;
            });
        }

        private void Hyperlink_ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CHSet_Active))
            {
                Window.Alert(ResCHSet.Msg_NoRight_ChangeStatus);
                return;
            }
            int id = ((dynamic)DataGrid_ResultList.SelectedItem).SysNo;
            fpCheckFacade.UpdateCHItemStatus(id, (obj, args) =>
          {
              if (args.FaultsHandle())
              {
                  return;
              }
              DataGrid_ResultList.Bind();
          });
        }
    }

}
