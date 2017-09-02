using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;
using ECCentral.Portal.UI.Invoice.NeweggCN.UserControls;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Views.InvoiceReport
{
    /// <summary>
    /// 礼品卡发票明细表
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class GiftInvoiceDetailReport : PageBase
    {
        private GiftInvoiceDetaiReportQueryVM _queryVM;
        private GiftInvoiceDetaiReportQueryVM _lastQueryVM;
        private InvoiceReportFacade _facade;

        public GiftInvoiceDetailReport()
        {
            InitializeComponent();
            InitData();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermissions();
            base.OnPageLoad(sender, e);
            _facade = new InvoiceReportFacade(this);
            
        }

        private void VerifyPermissions()
        {
            this.DataGrid.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_GiftInvoiceDetailReport_Export);

        }

        private void InitData()
        {
            _queryVM = new GiftInvoiceDetaiReportQueryVM();
            this.QueryBuilder.DataContext = _lastQueryVM = _queryVM;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_GiftInvoiceDetailReport_Print))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }


        }

        private void btnImportSAP_Click(object sender, RoutedEventArgs e)
        {
            //页面级权限
            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.Invoice_InvoiceDetailReport_Import))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            UCImportTrackingNumber importer = new UCImportTrackingNumber();
            importer.SelectedStockSysNo = 51; //上海仓
            //importer.ShowDialog("导入运单号", null);
            importer.ShowDialog(ResGiftInvoiceDetailReport.Button_ImportSAP, null);
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.DataGrid.ItemsSource;
            if (dataSource != null)
            {
                var ckbAll = sender as CheckBox;
                foreach (dynamic item in dataSource)
                {
                    item.IsChecked = ckbAll.IsChecked ?? false;
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.QueryBuilder);
            if (flag)
            {
                _queryVM.StockSysNo = 51; //上海仓
                _queryVM.SOType = ECCentral.BizEntity.SO.SOType.ElectronicCard; //电子卡订单

                this._lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<GiftInvoiceDetaiReportQueryVM>(_queryVM);

                this.DataGrid.Bind();
            }
        }

        private void DataGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (_lastQueryVM == null || this.DataGrid.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }
            ColumnSet col = new ColumnSet(this.DataGrid);
            _facade.ExportGiftInvoiceDetailReportExcelFile(_lastQueryVM, new ColumnSet[] { col });
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            

            _facade.QueryGiftInvoiceDetailReport(_lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                this.DataGrid.ItemsSource = result.Rows.ToList("IsChecked", false);
                this.DataGrid.TotalCount = result.TotalCount;
            });
        }

        private void Hyperlink_OrderID_Click(object sender, RoutedEventArgs e)
        {
            var data = this.DataGrid.SelectedItem as dynamic;
            if (data != null)
            {
                this.Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, data.OrderID), null, true);
            }
        }
    }
}