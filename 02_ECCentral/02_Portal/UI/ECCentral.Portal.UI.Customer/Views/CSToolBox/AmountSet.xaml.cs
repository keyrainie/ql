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
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;


namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class AmountSet : PageBase
    {
        private List<CodeNamePair> amountType;
        public AmountSet()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            CodeNamePairHelper.GetList("Customer", "AmountType", (obj2, args2) =>
            {
                amountType = args2.Result;
                this.dataGridAmount.Bind();
            });
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_AMT_Add))
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
            amountType.ForEach(item =>
            {
                queryFilter.ReferenceTypeIn += string.Format("'{0}',", item.Code);
            });
            queryFilter.ReferenceTypeIn = queryFilter.ReferenceTypeIn.TrimEnd(',');
            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<OrderCheckItemVM> list = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                foreach (CodeNamePair pair in amountType)
                {
                    foreach (OrderCheckItemVM itemVM in list)
                    {
                        if (itemVM.ReferenceType.ToString() == pair.Code)
                        {
                            itemVM.ReferenceTypeName = pair.Name;
                        }
                    }
                }
                dataGridAmount.ItemsSource = list;
                dataGridAmount.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckItemVM orderCheckItemVM = new OrderCheckItemVM();
            CSToolAmountMaintain uctlAmountMaintain = new CSToolAmountMaintain();
            uctlAmountMaintain.OrderCheckItemVM = orderCheckItemVM;
            uctlAmountMaintain.Dialog = Window.ShowDialog(ResOrderCheck.DialogTitle_AmountMaintain, uctlAmountMaintain, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    this.dataGridAmount.Bind();
                }
            });
        }

        private void Hyperlink_ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_AMT_Active))
            {
                Window.Alert(ResOrderCheck.Msg_NoRights_AMT_ChangeStatus);
                return;
            }
            OrderCheckItemVM orderCheckItemVM = dataGridAmount.SelectedItem as OrderCheckItemVM;
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
                    this.dataGridAmount.Bind();
                });
            }
        }
    }
}
