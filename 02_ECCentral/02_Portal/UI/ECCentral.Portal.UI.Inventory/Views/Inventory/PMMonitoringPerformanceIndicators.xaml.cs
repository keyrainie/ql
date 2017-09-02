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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Text;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.Views.Inventory
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class PMMonitoringPerformanceIndicators : PageBase
    {
        PMMonitoringPerformanceIndicatorsQueryFilterVM queryFilterVM;
        string tStockCenterCondition;

        public PMMonitoringPerformanceIndicators()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            queryFilterVM = new PMMonitoringPerformanceIndicatorsQueryFilterVM() { AvailableSaledDays = "3" };
            LayoutQuerySection.DataContext = queryFilterVM;
            comAvailableSalesDaysCondition.ItemsSource = queryFilterVM.AvailableSaledDaysConditionList;
            comAVGSaledQtyCondition.ItemsSource = queryFilterVM.AvgSaledQtyConditionList;
            comStock.ItemsSource = queryFilterVM.StockList;

            comAVGSaledQtyCondition.SelectedIndex = 0;
            comAvailableSalesDaysCondition.SelectedIndex = 0;
            btnExportToStockCenter.IsEnabled = false;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridSearchCondition))
            {
                btnExportToStockCenter.IsEnabled = true;
                CalcStockCenterCondition();
                searchResultGrid.Bind();
            }
        }

        private void searchResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.queryFilterVM.PagingInfo.PageIndex = e.PageIndex;
            this.queryFilterVM.PagingInfo.PageSize = e.PageSize;
            this.queryFilterVM.PagingInfo.SortBy = e.SortField;

            new InventoryQueryFacade(this).QueryPMMonitoringPerformanceIndicators(this.queryFilterVM, (obj, args) =>
            {
                this.searchResultGrid.ItemsSource = args.Result.Rows;
                this.searchResultGrid.TotalCount = args.Result.TotalCount;
            });

        }

        private void btnExportToStockCenter_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(string.Format(ConstValue.Inventory_ProductStockingCenter, tStockCenterCondition), null, true);
        }
        /// <summary>
        /// 获取当前过滤条件的值，组合成字符串，供跳转备货中心功能使用
        /// </summary>
        private void CalcStockCenterCondition()
        {
            StringBuilder tmpUrlParmStr = new StringBuilder();
            tmpUrlParmStr.AppendFormat("{0},", queryFilterVM.SelectedCategory1);
            tmpUrlParmStr.AppendFormat("{0},", queryFilterVM.SelectedCategory2);
            tmpUrlParmStr.AppendFormat("{0},", queryFilterVM.SelectedPMSysNo);
            tmpUrlParmStr.AppendFormat("{0},", queryFilterVM.StockSysNo);
            tmpUrlParmStr.AppendFormat("{0},", queryFilterVM.AvailableSalesDaysCondition);
            tmpUrlParmStr.AppendFormat("{0},", queryFilterVM.AvailableSaledDays);
            tmpUrlParmStr.AppendFormat("{0},", queryFilterVM.AVGSaledQtyCondition);
            tmpUrlParmStr.AppendFormat("{0}", queryFilterVM.AVGSaledQty);

            tStockCenterCondition = tmpUrlParmStr.ToString();
        }

        private void searchResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_PMMonitoringPerformanceIndicators_ExportExcellAll))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.searchResultGrid == null || this.searchResultGrid.TotalCount==0)
            {
                Window.Alert("没有可供导出的数据！");
                return;
            }

            //导出全部:
            if (null != queryFilterVM)
            {
                PMMonitoringPerformanceIndicatorsQueryFilterVM exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PMMonitoringPerformanceIndicatorsQueryFilterVM>(queryFilterVM);
                exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet()
                .Add("C1Name", ResPMMonitoringPerformanceIndicators.GridHeader_C1Name, 40)
                .Add("C2Name", ResPMMonitoringPerformanceIndicators.GridHeader_C2Name, 40)
                .Add("PMs", ResPMMonitoringPerformanceIndicators.GridHeader_PMs, 20)
                .Add("Shortage", ResPMMonitoringPerformanceIndicators.GridHeader_Shortage, 20)
                .Add("ShortageRate", ResPMMonitoringPerformanceIndicators.GridHeader_ShortageRate, 20)
                .Add("Losing", ResPMMonitoringPerformanceIndicators.GridHeader_Losing, 20)
                .Add("LSD", ResPMMonitoringPerformanceIndicators.GridHeader_LSD, 20)
                .Add("LSDRate", ResPMMonitoringPerformanceIndicators.GridHeader_LSDRate, 20);
                new InventoryQueryFacade(this).ExportExcelForVendors(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

    }
}
