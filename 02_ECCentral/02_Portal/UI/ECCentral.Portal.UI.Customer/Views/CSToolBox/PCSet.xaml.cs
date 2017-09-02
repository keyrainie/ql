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
    public partial class PCSet : PageBase
    {
        public PCSet()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.dataGridOrderCheckList.Bind();       
        }
       
        private void DataGrid_Result_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {          
            OrderCheckItemQueryFilter queryFilter = new OrderCheckItemQueryFilter();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = int.MaxValue,
                PageIndex = 0,
                SortBy = ""
            };
            queryFilter.ReferenceType = "PC";
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                List<OrderCheckItemVM> list = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                dataGridOrderCheckList.ItemsSource = list;
                dataGridOrderCheckList.TotalCount = list.Count();
            });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckItemVM orderCheckItemVM = new OrderCheckItemVM();
            if (null != orderCheckItemVM)
            {
                OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
                orderCheckItemVM.Status = OrderCheckStatus.Valid;
                orderCheckItemVM.SysNos = GetSelectedSysNos();
                orderCheckItemVM.ReferenceType = "PC";
                facade.UpdateOrderCheckItem(orderCheckItemVM, (obj, args) =>
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
        }

        private string GetSelectedSysNos()
        {
            string sysNos = "";
            if (this.dataGridOrderCheckList.ItemsSource != null)
            {
                dynamic viewList = this.dataGridOrderCheckList.ItemsSource as dynamic;

                foreach (var view in viewList)
                {
                    if (view.IsCheck)
                    {
                        sysNos += view.SysNo + ",";
                    }
                }
            }
            if (sysNos.Length > 0)
            {
                sysNos = sysNos.Substring(0, sysNos.Length - 1);
            }
            return sysNos;
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
    }
}
