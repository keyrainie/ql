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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls.Inventory;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class InventoryQuery : PageBase
    {

        public InventoryQueryVM queryVM;
        public InventoryQueryFilter queryFilter;
        public InventoryQueryFacade serviceFacade;

        private DataGridColumn unPayOrderQtyColumn;
        public DataGridColumn UnPayOrderQtyColumn
        {
            get
            {
                if (unPayOrderQtyColumn == null)
                {
                    foreach (var col in dgInventoryQueryResult.Columns)
                    {
                        if (col.GetBindingPath().ToUpper().IndexOf("UNPAYORDERQTY") > 0)
                        {
                            unPayOrderQtyColumn = col;
                            break;
                        }
                    }
                }                
                
                return unPayOrderQtyColumn;    
            }     
        }

        private DataGridColumn stockNameColumn;
        public DataGridColumn StockNameColumn
        {
            get
            {
                if (stockNameColumn == null)
                {
                    foreach (var col in dgInventoryQueryResult.Columns)
                    {
                        if (col.GetBindingPath().ToUpper().IndexOf("STOCKNAME") > 0)
                        {
                            stockNameColumn = col;
                            break;
                        }
                    }
                }

                return stockNameColumn;
            }
        }

        private DataGridColumn safeQtyColumn;
        public DataGridColumn SafeQtyColumn
        {
            get
            {
                if (safeQtyColumn == null)
                {
                    foreach (var col in dgInventoryQueryResult.Columns)
                    {
                        if (col.GetBindingPath().ToUpper().IndexOf("SAFEQTY") > 0)
                        {
                            safeQtyColumn = col;
                            break;
                        }
                    }
                }

                return safeQtyColumn;
            }
        }
        #region 初始化加载

        public InventoryQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryFilter = new InventoryQueryFilter();
            queryVM = new InventoryQueryVM();
            serviceFacade = new InventoryQueryFacade(this);

            string getParam = this.Request.Param;
            if (!string.IsNullOrEmpty(getParam))
            {
                queryVM.ProductSysNo = Convert.ToInt32(getParam.Trim());
                this.DataContext = queryVM;
                btnSearch_Click(null, null);
            }

            else
            {
                this.DataContext = queryVM;
            }

        }

        #endregion

        #region 页面内按钮处理事件

        private void dgInventoryQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
           
            queryFilter = EntityConverter<InventoryQueryVM, InventoryQueryFilter>.Convert(queryVM);
            this.queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo();

            this.queryFilter.PagingInfo.PageIndex = e.PageIndex;
            this.queryFilter.PagingInfo.PageSize = e.PageSize;
            this.queryFilter.PagingInfo.SortBy = e.SortField;
            //如果查询总库存，则调用QueryInventory的service,否则调用QueryInventoryStock:
            //if (this.queryFilter.IsShowTotalInventory != null && this.queryFilter.IsShowTotalInventory.Value)
            //{

            #region 获取自己能访问到的PM

            queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;
            queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
            {
                if (queryFilter == null || !queryFilter.ProductSysNo.HasValue)
                {
                    Window.Alert("请先选择商品!");
                    return;
                }
                serviceFacade.CheckOperateRightForCurrentUser(queryFilter, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        if (args.Result)
                        {
                            serviceFacade.QueryInventoryList(queryFilter, (innerObj, innerArgs) =>
                            {
                                if (innerArgs.FaultsHandle())
                                {
                                    return;
                                }
                                var getList = innerArgs.Result.Rows;

                                foreach (var x in getList)
                                {
                                    x["ActivedQty"] = x["ActivedQty"] == null || x["ActivedQty"].ToString() == "" ? "--" : x["ActivedQty"];
                                    x["InactiveQty"] = x["InactiveQty"] == null || x["InactiveQty"].ToString() == "" ? "--" : x["InactiveQty"];
                                    x["InvalidQty"] = x["InvalidQty"] == null || x["InvalidQty"].ToString() == "" ? "--" : x["InvalidQty"];
                                }

                                int totalCount = innerArgs.Result.TotalCount;
                                this.dgInventoryQueryResult.ItemsSource = getList;
                                this.dgInventoryQueryResult.TotalCount = totalCount;
                            });
                            //}
                            //else
                            //{
                            //    serviceFacade.QueryInventoryStockList(queryFilter, (obj, args) =>
                            //    {
                            //        if (args.FaultsHandle())
                            //        {
                            //            return;
                            //        }
                            //        var getList = args.Result.Rows;
                            //        int totalCount = args.Result.TotalCount;
                            //        this.dgInventoryQueryResult.ItemsSource = getList;
                            //        this.dgInventoryQueryResult.TotalCount = totalCount;
                            //    });
                            //}
                            this.UnPayOrderQtyColumn.Visibility = queryVM.IsUnPayShow ? Visibility.Visible : Visibility.Collapsed;
                            this.StockNameColumn.Visibility =
                                queryVM.IsShowTotalInventory ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else
                        {
                            Window.Alert(string.Format("你没有权限查询该商品库存"));
                        }

                    });
            }
            else 
            {
                serviceFacade.QueryInventoryList(queryFilter, (innerObj, innerArgs) =>
                            {
                                if (innerArgs.FaultsHandle())
                                {
                                    return;
                                }
                                var getList = innerArgs.Result.Rows;

                                foreach (var x in getList)
                                {
                                    x["ActivedQty"] = x["ActivedQty"] == null || x["ActivedQty"].ToString() == "" ? "--" : x["ActivedQty"];
                                    x["InactiveQty"] = x["InactiveQty"] == null || x["InactiveQty"].ToString() == "" ? "--" : x["InactiveQty"];
                                    x["InvalidQty"] = x["InvalidQty"] == null || x["InvalidQty"].ToString() == "" ? "--" : x["InvalidQty"];
                                }

                                int totalCount = innerArgs.Result.TotalCount;
                                this.dgInventoryQueryResult.ItemsSource = getList;
                                this.dgInventoryQueryResult.TotalCount = totalCount;

                                this.UnPayOrderQtyColumn.Visibility = queryVM.IsUnPayShow ? Visibility.Visible : Visibility.Collapsed;
                                this.StockNameColumn.Visibility = queryVM.IsShowTotalInventory ? Visibility.Collapsed : Visibility.Visible;
                        });
            }
            #endregion
        }
      

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.queryFilter = EntityConverter<InventoryQueryVM, InventoryQueryFilter>.Convert(this.queryVM);
            //if (queryFilter==null||!queryFilter.ProductSysNo.HasValue)
            //{
            //    Window.Alert(string.Format("请输入商品编号"));
            //    return;
            //}
            this.dgInventoryQueryResult.Bind();
        }

        #endregion

        private void chkTotalInventory_Click(object sender, RoutedEventArgs e)
        {

            queryVM.PositionInWareHouse = string.Empty;
            queryVM.PositionInWareHouse = string.Empty;
            queryVM.IsAccountQtyLargerThanZero = false;
            queryVM.IsUnPayShow = false;

            this.lblPositionInWarehouse.Visibility = this.chkTotalInventory.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
            this.txtPositionInWarehouse.Visibility = this.chkTotalInventory.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
            this.chkIsAccountQtyLargerThan0.Visibility = this.chkTotalInventory.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
            this.chkIsUnPay.Visibility = this.chkTotalInventory.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
            //清空仓库选择
            this.ucStock.SelectedStockSysNo = 0;
            this.ucStock.SelectedStockName = string.Empty;

            this.StockNameColumn.Visibility = 
               queryVM.IsShowTotalInventory ? Visibility.Collapsed : Visibility.Visible;

            this.SafeQtyColumn.Visibility = queryVM.IsShowTotalInventory ? Visibility.Visible : Visibility.Collapsed;
            if (this.chkTotalInventory.IsChecked == true)
            {
                this.queryVM.StockID = null;
                this.ucStock.IsEnabled = false;
            }
            else
            {
                this.ucStock.IsEnabled = true;
                this.chkInventoryWarning.IsChecked = false;
            }
        }

        private void dgInventoryQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_InventoryQuery_ExportExcellAll))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgInventoryQueryResult == null || this.dgInventoryQueryResult.TotalCount==0)
            {
                Window.Alert("没有可供导出的数据！");
                return;
            }

            //导出全部:
            if (null != queryFilter)
            {
                InventoryQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InventoryQueryFilter>(queryFilter);
                exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet()
                    // .Add("WebChannelName", ResInventoryQuery.Grid_WebChannel, 20)
                .Add("StockName", ResInventoryQuery.Grid_Stock, 20)
                .Add("ProductID", ResInventoryQuery.Grid_ProductID, 20)
                .Add("ProductName", ResInventoryQuery.Grid_ProductName, 40)
                .Add("VendorName", ResInventoryQuery.Grid_VendorName, 30)
                .Add("AccountQty", ResInventoryQuery.Grid_Inventory_AccountQty, 20)
                .Add("CostAmount", ResInventoryQuery.Grid_CostAmount, 20)
                .Add("AvailableQty", ResInventoryQuery.Grid_Inventory_AvailableQty, 20)
                .Add("AllocatedQty", ResInventoryQuery.Grid_Inventory_AllocatedQty, 20)
                .Add("OrderQty", ResInventoryQuery.Grid_Inventory_OrderQty, 20)
                .Add("ActivedQty", ResInventoryQuery.Grid_Inventory_ActivedQty, 20)
                .Add("InactiveQty", ResInventoryQuery.Grid_Inventory_InactiveQty, 20)
                .Add("InvalidQty", ResInventoryQuery.Grid_Inventory_InvalidQty, 20)
                .Add("VirtualQty", ResInventoryQuery.Grid_Inventory_VirtualQty, 20)
                .Add("ConsignQty", ResInventoryQuery.Grid_Inventory_ConsignQty, 20)
                .Add("PurchaseQty", ResInventoryQuery.Grid_Inventory_PurchaseQty, 20)
                .Add("ShiftInQty", ResInventoryQuery.Grid_Inventory_ShiftInQty, 20)
                .Add("ShiftOutQty", ResInventoryQuery.Grid_Inventory_ShiftOutQty, 20)
                .Add("RetainQty", "临时保留库存", 20)
                .Add("PositionInWarehouse", ResInventoryQuery.Grid_Inventory_PositionWarehose, 20)
                .Add("AVGDailySales", ResInventoryQuery.Grid_SalesQty_DMS, 20)
                .Add("AvailableSalesDays", ResInventoryQuery.Grid_Inventory_DaysToSell, 20);
                serviceFacade.ExportExcelForInventoryList(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

        private void hpAccountQty_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml sltItem = this.dgInventoryQueryResult.SelectedItem as DynamicXml;
            if (sltItem["StockSysNo"] != null)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.Inventory/BatchDetailMaintain/{0}&{1}",
                    sltItem["ProductSysNo"].ToString(),
                    sltItem["StockSysNo"].ToString()),
                    null,
                    true);
            }
            else
            {
                Window.Alert("请先按分仓进行查询！");
            }
        }

        private void chkInventoryWarning_Click(object sender, RoutedEventArgs e)
        {
            if (chkInventoryWarning.IsChecked.HasValue && chkInventoryWarning.IsChecked.Value == true)
            {
                chkTotalInventory.IsChecked = true;
                chkTotalInventory_Click(this, new RoutedEventArgs());
            }
        }
    }
}
