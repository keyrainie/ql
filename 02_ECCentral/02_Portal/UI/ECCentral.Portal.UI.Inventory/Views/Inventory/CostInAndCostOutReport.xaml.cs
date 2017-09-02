using System;
using System.Linq;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Input;
using ECCentral.Portal.Basic.Components.UserControls.BrandPicker;

namespace ECCentral.Portal.UI.Inventory.Views
{
    /// <summary>
    /// 商品入库报表
    /// </summary>
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Page)]
    public partial class CostInAndCostOutReport : PageBase
    {
        private CostInAndCostOutReportQueryVM PageQueryView;
        private InventoryQueryFacade PageQueryFacade;

        public CostInAndCostOutReport()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            PageQueryFacade = new InventoryQueryFacade(this);
            PageQueryView = new CostInAndCostOutReportQueryVM();
            this.SearchBuilder.DataContext = PageQueryView;
            this.drDateRange.SelectedRangeType = RangeType.Last30Days;
        }

        private void btnChooseVendor_Click(object sender, MouseButtonEventArgs e)
        {
            UCVendorQuery selectDialog = new UCVendorQuery() { SelectionMode = SelectionMode.Multiple };
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("供应商查询", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    List<string> vendorList = new List<string>();
                    List<DynamicXml> getSelectedVendors = args.Data as List<DynamicXml>;
                    if (null != getSelectedVendors)
                    {
                        foreach (DynamicXml getSelectedVendor in getSelectedVendors)
                        {
                            vendorList.Add(getSelectedVendor["VendorName"].ToString());
                            PageQueryView.VendorSysNoList.Add(Convert.ToInt32(getSelectedVendor["SysNo"]));
                        }
                        this.txtVendorName.Text = vendorList.Join(",");
                    }
                }
            }, new Size(750, 650));
        }

        private void btnClearVendor_Click(object sender, MouseButtonEventArgs e)
        {
            PageQueryView.VendorSysNoList.Clear();
            this.txtVendorName.Text = string.Empty;
        }

        private void btnChooseBrand_Click(object sender, MouseButtonEventArgs e)
        {
            UCBrandQuery selectDialog = new UCBrandQuery() { SelectionMode = SelectionMode.Multiple };
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("供应商查询", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    List<string> vendorList = new List<string>();
                    List<DynamicXml> getSelectedBrands = args.Data as List<DynamicXml>;
                    if (null != getSelectedBrands)
                    {
                        foreach (DynamicXml getSelectedBrand in getSelectedBrands)
                        {
                            vendorList.Add(getSelectedBrand["BrandName_Ch"].ToString());
                            PageQueryView.BrandSysNoList.Add(Convert.ToInt32(getSelectedBrand["SysNo"]));
                        }
                        this.txtBrandName.Text = vendorList.Join(",");
                    }
                }
            }, new Size(750, 650));
        }

        private void btnClearBrand_Click(object sender, MouseButtonEventArgs e)
        {
            PageQueryView.BrandSysNoList.Clear();
            this.txtBrandName.Text = string.Empty;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(SearchBuilder) || PageQueryView.HasValidationErrors)
            {
                if (PageQueryView.ValidationErrors.Any(_ => _.MemberNames.Any(x => x.Contains("DateTime"))))
                {
                    this.drDateRange.Focus();
                }
                return;
            }
            PageQueryView.ValidationErrors.Clear();

            QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {

            PageQueryFacade.QueryCostInAndCostOutReport(PageQueryView, e.PageIndex, e.PageSize, e.SortField, (obj, args) =>
             {
                 if (args.Result != null && args.Result.Rows != null)
                 {
                     this.QueryResultGrid.ItemsSource = args.Result.Rows;
                     this.QueryResultGrid.TotalCount = args.Result.TotalCount;
                 }
             });
        }

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {

            if (this.QueryResultGrid == null || this.QueryResultGrid.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageQueryView)
            {
                ColumnSet columnSet = new ColumnSet(QueryResultGrid);
                PageQueryFacade.ExportExcelForCostInAndCostOutReport(PageQueryView, new ColumnSet[] { columnSet });
            }
        }
    }
}
