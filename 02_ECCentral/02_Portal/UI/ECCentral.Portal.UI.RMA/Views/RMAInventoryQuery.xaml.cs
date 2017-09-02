using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Resources;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View]
    public partial class RMAInventoryQuery : PageBase
    {
        private ReportFacade facade;
        private RMAInventoryQueryVM queryVM;
        private CommonDataFacade commonFacade;
        private RMAInventoryQueryVM lastQueryVM;


        public RMAInventoryQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryVM = new RMAInventoryQueryVM();
            facade = new ReportFacade(this);
            commonFacade = new CommonDataFacade(this);
            LoadComboBoxData();
            this.QueryFilter.DataContext = queryVM;
            base.OnPageLoad(sender, e);
        }
        private void LoadComboBoxData()
        {
            commonFacade.GetStockList(true, (obj, args) =>
            {
                this.Combox_LocationWarehouse.ItemsSource = this.Combox_OwnByWarehous.ItemsSource = args.Result;
            });
            Combox_SearchType.SelectedIndex = 0;
        }

        private void DataGrid_Product_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryRMAProductInventory(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.DataGrid_Product_ResultList.ItemsSource = args.Result[0].Rows;
                this.DataGrid_Product_ResultList.TotalCount = args.Result[0].TotalCount;
                decimal totalMisCost = Math.Round(Convert.ToDecimal(args.Result[1].Rows[0]["TotleMisCost"]), 2);
                if (args.Result[0].TotalCount > 0)
                {
                    Text_TotalMisCost.Visibility = Visibility.Visible;
                    Text_TotalMisCost.Text = ResRMAReports.Label_StatisticalData + totalMisCost.ToString();
                }
                else
                {
                    Text_TotalMisCost.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void DataGrid_RMAItem_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryRMAItemInventory(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.DataGrid_RMAItem_ResultList.ItemsSource = args.Result.Rows;
                this.DataGrid_RMAItem_ResultList.TotalCount = args.Result.TotalCount;

            });
        }

        private void Combox_SearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (queryVM.SearchType == (int)RMAInventorySearchType.Product)
            {
                Tab_Product.IsSelected = true;
                Tab_RMA.IsSelected = false;
            }
            else
            {
                Tab_Product.IsSelected = false;
                Tab_RMA.IsSelected = true;
            }
            Text_RMASysNo.IsEnabled = queryVM.SearchType == RMAInventorySearchType.RMA ? true : false;
            Text_RMASysNo.Text = Text_RMASysNo.IsEnabled == true ? Text_RMASysNo.Text : string.Empty;
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.QueryFilter);
            if (queryVM.HasValidationErrors)
            {
                return;
            }
            lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RMAInventoryQueryVM>(queryVM);
            if (queryVM.SearchType == RMAInventorySearchType.Product)
            {
                this.DataGrid_Product_ResultList.Bind();
            }
            else
            {
                this.DataGrid_RMAItem_ResultList.Bind();
            }

        }

        private void btnSysNo_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            string url = string.Format(ConstValue.RMA_EditRegisterUrl, vm.SysNo);
            Window.Navigate(url, null, true);
        }

        private void btnProductId_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            string url = string.Format(ConstValue.RMA_RegisterQueryUrl,"?ProductSysNo="+vm.SysNo+"&ProductID="+ vm.ProductId);
            Window.Navigate(url, null, true);
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                var txtBinding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (txtBinding != null)
                {
                    txtBinding.UpdateSource();
                    if (queryVM.SearchType == RMAInventorySearchType.Product)
                    {
                        this.DataGrid_Product_ResultList.Bind();
                    }
                    else
                    {
                        this.DataGrid_RMAItem_ResultList.Bind();
                    }
                }
            }
        }

        private void DataGrid_Product_ResultList_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.DataGrid_Product_ResultList.TotalCount < 1)
            {
                Window.Alert(ResRMAReports.Msg_ExportError);
                return;
            }
            if (this.DataGrid_Product_ResultList.TotalCount > 10000)
            {
                Window.Alert(ResRMAReports.Msg_ExportExceedsLimitCount);
                return;
            }

            ColumnSet col = new ColumnSet(this.DataGrid_Product_ResultList, true);
            col.SetWidth("ProductID", 15)
                .SetWidth("ProductName", 48);
            facade.ExportProductExcelFile(lastQueryVM, new ColumnSet[] { col });
        }

        private void DataGrid_RMAItem_ResultList_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.DataGrid_RMAItem_ResultList.TotalCount < 1)
            {
                Window.Alert(ResRMAReports.Msg_ExportError);
                return;
            }
            if (this.DataGrid_RMAItem_ResultList.TotalCount > 10000)
            {
                Window.Alert(ResRMAReports.Msg_ExportExceedsLimitCount);
                return;
            }

            ColumnSet col = new ColumnSet(this.DataGrid_RMAItem_ResultList);
            col.Insert(0, "SysNo", ResRMAReports.Excel_RegisterSysNo, 12)
                .SetWidth("ProductID", 15)
                .SetWidth("ProductName", 48);
            facade.ExportRMAItemExcelFile(lastQueryVM, new ColumnSet[] { col });

        }
    }
}
