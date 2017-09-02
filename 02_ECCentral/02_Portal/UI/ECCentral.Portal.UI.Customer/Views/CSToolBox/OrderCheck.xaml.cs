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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Portal.Basic.Components;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class OrderCheck : PageBase
    {
        public OrderCheck()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.dataGridOrderCheckList.Bind();
            LoadCompanyList();
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_Save))
                this.Button_Save.IsEnabled = false;
        }
        private void LoadCompanyList()
        {
            this.listChannel.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.listChannel.SelectedIndex = 0;
        }
        private void DataGrid_Result_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            OrderCheckMasterQueryFilter request = new OrderCheckMasterQueryFilter();
            request.PagingInfo = new PagingInfo
            {
                PageSize = int.MaxValue,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            request.CompanyCode = CPApplication.Current.CompanyCode;
            OrderCheckMasterFacade facade = new OrderCheckMasterFacade(this);
            facade.QueryOrderCheckMaster(request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<OrderCheckMasterVM> list = DynamicConverter<OrderCheckMasterVM>.ConvertToVMList(args.Result.Rows);
                dataGridOrderCheckList.ItemsSource = list;
                dataGridOrderCheckList.TotalCount = list.Count();
            });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckMasterFacade facade = new OrderCheckMasterFacade();
            OrderCheckMasterReq req = new OrderCheckMasterReq();
            req.orderCheckMaster = new OrderCheckMaster();
            req.orderCheckMaster.CompanyCode = CPApplication.Current.CompanyCode;
            req.SysNoList = GetSelectedSysNoList();

            facade.BatchUpdateOrderCheckMasterStatus(req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                else
                {
                    Window.Alert(ResOrderCheck.Msg_UpdateSuccess);
                    this.dataGridOrderCheckList.Bind();
                }
            });
        }
        private List<int> GetSelectedSysNoList()
        {
            List<int> sysNoList = new List<int>();
            if (this.dataGridOrderCheckList.ItemsSource != null)
            {
                dynamic viewList = this.dataGridOrderCheckList.ItemsSource as dynamic;

                foreach (var view in viewList)
                {
                    if (view.IsCheck)
                    {
                        sysNoList.Add(view.SysNo);
                    }
                }
            }
            return sysNoList;
        }

        private void Hyperlink_CheckType_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckMasterVM vm = dataGridOrderCheckList.SelectedItem as OrderCheckMasterVM;
            if (vm != null)
            {
                Window.Navigate(string.Format(vm.Url, this.listChannel.SelectedValue), null, true);
            }
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.dataGridOrderCheckList.ItemsSource as dynamic;
                foreach (var view in viewList)
                {
                    view.IsCheck = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                }
                this.dataGridOrderCheckList.ItemsSource = viewList;
            }
        }

        private void listChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.dataGridOrderCheckList.Bind();
        }


    }
}