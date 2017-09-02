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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class AutoCheckTimeSet : PageBase
    {
        public AutoCheckTimeSet()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.dataGridAutoCheckTimeList.Bind();
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_AT_Add))
                this.Button_New.IsEnabled = false;
        }
        private void DataGrid_Result_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            OrderCheckItemQueryFilter queryFilter = new OrderCheckItemQueryFilter();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.ReferenceType = "SA";
            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<OrderCheckItemVM> list = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                dataGridAutoCheckTimeList.ItemsSource = list;
                dataGridAutoCheckTimeList.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckItemVM orderCheckItemVM = new OrderCheckItemVM();
            CSToolAutoCheckTimeMaintain uctlAutoCheckTimeMaintain = new CSToolAutoCheckTimeMaintain();
            uctlAutoCheckTimeMaintain.OrderCheckItemVM = orderCheckItemVM;
            uctlAutoCheckTimeMaintain.Dialog = Window.ShowDialog(ResOrderCheck.DialogTitle_AutoCheckTime, uctlAutoCheckTimeMaintain, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    this.dataGridAutoCheckTimeList.Bind();
                }
            });
        }

        private void Hyperlink_ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_AT_Active))
            {
                Window.Alert(ResOrderCheck.Msg_NoRigths_AT_ChangeStatus);
                return;
            }
            OrderCheckItemVM orderCheckItemVM = dataGridAutoCheckTimeList.SelectedItem as OrderCheckItemVM;
            if (null != orderCheckItemVM)
            {
                OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
                orderCheckItemVM.Status = orderCheckItemVM.Operator;
                facade.UpdateOrderCheckItem(orderCheckItemVM, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    this.dataGridAutoCheckTimeList.Bind();
                });
            }
        }
    }
}
