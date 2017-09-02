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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.UI.PO.Models.PurchaseOrder;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class VirtualStockPurchaseOrderQuery : PageBase
    {

        public VirtualPurchaseOrderQueryVM queryVM;
        public VirtualPurchaseOrderQueryFilter queryFilter;
        private VirtualPurchaseOrderQueryFilter tempFilter;
        private VirtualPurchaseOrderQueryFilter newQueryFilter;

        public VirtualPurchaseOrderFacade serviceFacade;

        public VirtualStockPurchaseOrderQuery()
        {
            InitializeComponent();
            queryVM = new VirtualPurchaseOrderQueryVM();
            queryFilter = new VirtualPurchaseOrderQueryFilter();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            this.DataContext = queryVM;
            base.OnPageLoad(sender, e);
            serviceFacade = new VirtualPurchaseOrderFacade(this);

            LoadComboBoxData();
            this.chkAdvancedSearch.IsChecked = true;
            SwitchAdvancedSearchCondition(Visibility.Visible);
            SetAccessControl();
        }

        private void SetAccessControl()
        {

            //查询虚库采购单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VirtualPO_Query))
            {
                this.btnSearch.IsEnabled = false;
            }
        }

        private void LoadComboBoxData()
        {
            //状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VirtualPurchaseOrderStatus>(ECCentral.Portal.Basic.Utilities.EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
            //销售单状态:
            this.cmbSOStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SOStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbSOStatus.SelectedIndex = 0;
            //采购单状态：
            this.cmbPOStatus.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbPOStatus.SelectedIndex = 0;
            //移仓单状态:
            this.cmbShiftStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ShiftRequestStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbShiftStatus.SelectedIndex = 0;
            //转换单状态:
            this.cmbTransferStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ConvertRequestStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbTransferStatus.SelectedIndex = 0;
            //商品入库状态:
            this.cmbInstockStatus.ItemsSource = EnumConverter.GetKeyValuePairs<InStockStatus>
(EnumConverter.EnumAppendItemType.All);
            this.cmbInstockStatus.SelectedIndex = 0;
            //单据类型:
            this.cmbInStockOrderType.ItemsSource = EnumConverter.GetKeyValuePairs<VirtualPurchaseInStockOrderType>(EnumConverter.EnumAppendItemType.All);
            this.cmbInStockOrderType.SelectedIndex = 0;
        }

        #region [Events]

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //查询操作:
            this.queryFilter = EntityConverter<VirtualPurchaseOrderQueryVM, VirtualPurchaseOrderQueryFilter>.Convert(queryVM, (s, t) =>
            {
                t.Status = (s.Status.HasValue ? (int)s.Status.Value : (int?)null);
                t.POStatus = (s.POStatus.HasValue ? (int)s.POStatus.Value : (int?)null);
                t.SOStatus = (s.SOStatus.HasValue ? (int)s.SOStatus.Value : (int?)null);
                t.TransferStatus = (s.TransferStatus.HasValue ? (int)s.TransferStatus.Value : (int?)null);
                t.ShiftStatus = (s.ShiftStatus.HasValue ? (int)s.ShiftStatus.Value : (int?)null);
                t.InStockStatus = (s.InStockStatus.HasValue ? (int)s.InStockStatus.Value : (int?)null);
                t.InStockOrderType = (s.InStockOrderType.HasValue ? (int)s.InStockOrderType.Value : (int?)null);
            });
            newQueryFilter = queryFilter.DeepCopy();
            if (this.chkAdvancedSearch.IsChecked.Value)
            {
                tempFilter = queryFilter;
            }
            else
            {
                IsNormalQuery();
                tempFilter = newQueryFilter;
            }
            QueryResultGrid.Bind();
        }

        private void IsNormalQuery()
        {
            newQueryFilter.SOStatus = null;
            newQueryFilter.PMLeaderSysNo = null;
            newQueryFilter.PMExecSysNo = null;
            newQueryFilter.POStatus = null;
            newQueryFilter.PayTypeSysNo = null;
            newQueryFilter.ShiftStatus = null;
            newQueryFilter.InStockStatus = null;
            newQueryFilter.TransferStatus = null;
            newQueryFilter.StockSysNo = null;
            newQueryFilter.InStockOrderType = null;
            newQueryFilter.DelayDays = null;
            newQueryFilter.EstimateDelayDays = null;
            newQueryFilter.C1SysNo = newQueryFilter.C2SysNo = newQueryFilter.C3SysNo = null;
            newQueryFilter.IsHasHistory = null;

        }

        private void chkAdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            SwitchAdvancedSearchCondition(this.chkAdvancedSearch.IsChecked == true ? Visibility.Visible : Visibility.Collapsed);
        }

        private void SwitchAdvancedSearchCondition(Visibility visibility)
        {
            this.lblSOStatus.Visibility = visibility;
            this.cmbSOStatus.Visibility = visibility;
            this.lblPM.Visibility = visibility;
            this.ucPM.Visibility = visibility;
            this.lblCurrentPM.Visibility = visibility;
            this.ucCurrentPM.Visibility = visibility;
            this.lblPOStatus.Visibility = visibility;
            this.cmbPOStatus.Visibility = visibility;
            this.lblPayType.Visibility = visibility;
            this.ucPayType.Visibility = visibility;
            this.lblShiftStatus.Visibility = visibility;
            this.cmbShiftStatus.Visibility = visibility;
            this.lblInstockStatus.Visibility = visibility;
            this.cmbInstockStatus.Visibility = visibility;
            this.lblTransferStatus.Visibility = visibility;
            this.cmbTransferStatus.Visibility = visibility;
            this.lblShiftStatus.Visibility = visibility;
            this.cmbShiftStatus.Visibility = visibility;
            this.lblStock.Visibility = visibility;
            this.ucStock.Visibility = visibility;
            this.lblInStockOrderType.Visibility = visibility;
            this.cmbInStockOrderType.Visibility = visibility;
            this.lblEstimateDelayDays.Visibility = visibility;
            this.txtEstimateDelayDays.Visibility = visibility;
            this.lblDelayDays.Visibility = visibility;
            this.txtDelayDays.Visibility = visibility;
            this.lblProductCategory.Visibility = visibility;
            this.ucCategoryPicker.Visibility = visibility;
            this.lblIncludeHistory.Visibility = visibility;
            this.chkIncludeHistory.Visibility = visibility;
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            tempFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            tempFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryVirtualPurchaseOrders(tempFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vspoList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = vspoList;
            });

        }

        private void Hyperlink_VSPOEdit_Click(object sender, RoutedEventArgs e)
        {
            //编辑虚库采购单 ：
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/VirtualStockPurchaseOrderMaintain/{0}", getSelectedItem["SysNo"]), null, true);
        }
        #endregion

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            //权限控制:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VirtualPO_Export))
            {
                Window.Alert("对不起，你没有权限进行此操作! ");
                return;
            }
            //导出全部:
            if (null != queryFilter)
            {
                VirtualPurchaseOrderQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<VirtualPurchaseOrderQueryFilter>(queryFilter);
                exportQueryRequest.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };

                foreach (DataGridColumn col in QueryResultGrid.Columns)
                    if (col.Visibility == Visibility.Collapsed)
                        if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn)
                            (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn).NeedExport = false;
                        else if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn)
                            (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn).NeedExport = false;
                ColumnSet columnSet = new ColumnSet(QueryResultGrid);
                columnSet.Add("Status", ResVirtualStockPurchaseOrderQuery.GridHeader_Status);

                serviceFacade.ExportExcelForVirtualPurchaseOrders(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

    }

}
