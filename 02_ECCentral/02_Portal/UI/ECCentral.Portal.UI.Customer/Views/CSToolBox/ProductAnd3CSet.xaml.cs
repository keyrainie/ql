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
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class ProductAnd3CSet : PageBase
    {
        public OrderCheckItemVM viewModel;
        public ProductAnd3CSet()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            viewModel = new OrderCheckItemVM();
            viewModel.IsProductType = true;
            rbtnProductType1.IsChecked = true;
            this.DataContext = viewModel;
            LoadComboBoxData();
            base.OnPageLoad(sender, e);
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_ProductAnd3C_Add))
                this.Button_New.IsEnabled = false;
        }
        private void LoadComboBoxData()
        {
            List<KeyValuePair<OrderCheckStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<OrderCheckStatus>(EnumConverter.EnumAppendItemType.All);
            this.Combox_Status.ItemsSource = statusList;
            this.Combox_Status.SelectedIndex = 0;
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
            if (rbtnProductSysNo1.IsChecked.Value)
            {
                viewModel.ReferenceType = "PID";
            }
            else if (rbtnProductType1.IsChecked.Value)
            {
                viewModel.ReferenceType = "PC3";
            }
            queryFilter.ReferenceType = viewModel.ReferenceType;
            queryFilter.Status = viewModel.Status;
            queryFilter.ReferenceContent = viewModel.ProductID;
            queryFilter.Description = (viewModel.Category3No.HasValue ? viewModel.Category3No.ToString() : string.Empty);
            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<OrderCheckItemVM> list = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                this.DataGrid_ResultList.ItemsSource = list;
                DataGrid_ResultList.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            CSToolProductAnd3CMaintain uctlProductAnd3CMaintain = new CSToolProductAnd3CMaintain();

            uctlProductAnd3CMaintain.Dialog = Window.ShowDialog(ResOrderCheck.DialogTitle_ProductMaitain, uctlProductAnd3CMaintain, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid_ResultList.Bind();
                }
            }, new Size(500, 250));
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_ResultList.Bind();
        }

        private void Hyperlink_ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_ProductAnd3C_Active))
            {
                Window.Alert(ResOrderCheck.Msg_NoRights_ProductAnd3C_ChangeStatus);
                return;
            }
            OrderCheckItemVM orderCheckItemVM = this.DataGrid_ResultList.SelectedItem as OrderCheckItemVM;
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
                    this.DataGrid_ResultList.Bind();
                });
            }
        }

        private void rbtnProductType_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.ProductID = string.Empty;
        }

        private void rbtnProductSysNo_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.Category3No = null;
        }


    }
}
