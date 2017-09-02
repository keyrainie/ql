using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;
using ECCentral.Portal.UI.Invoice.NeweggCN.UserControls;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Views.InvoiceReport
{
    /// <summary>
    /// 发票明细表
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class InvoiceDetailReport : PageBase
    {
        private InvoiceDetailReportQueryVM _queryVM;
        private InvoiceDetailReportQueryVM _lastQueryVM;
        private InvoiceReportFacade _facade;

        public InvoiceDetailReport()
        {
            InitializeComponent();
            InitData();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermissions();
            base.OnPageLoad(sender, e);
            _facade = new InvoiceReportFacade(this);
            
            LoadComboBoxData();
        }

        private void VerifyPermissions()
        {
            this.DataGrid.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceDetailReport_Export);

        }

        private void InitData()
        {
            _queryVM = new InvoiceDetailReportQueryVM();
            this.QueryBuilder.DataContext = _lastQueryVM = _queryVM;
        }

        private void LoadComboBoxData()
        {
            string key = "InvoiceDetailReportStockList-" + CPApplication.Current.CompanyCode;
            CodeNamePairHelper.GetList("Invoice", key, CodeNamePairAppendItemType.None, (obj, args) =>
            {
                cmbStock.ItemsSource = args.Result;
                cmbStock.SelectedIndex = 0;
            });

            key = "InvoiceDetailReportOrderType";
            CodeNamePairHelper.GetList("Invoice", key, CodeNamePairAppendItemType.Select, (obj, args) =>
            {
                cmbOrderType.ItemsSource = args.Result;
                cmbOrderType.SelectedIndex = 0;
            });
        }

        private void DataGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (_lastQueryVM == null || this.DataGrid.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }
            ColumnSet col = new ColumnSet(this.DataGrid);
            _facade.ExportInvoiceDetailReportExcelFile(_lastQueryVM, new ColumnSet[] { col });
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _facade.QueryInvoiceDetailReport(_lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                this.DataGrid.ItemsSource = result.Rows.ToList();
                this.DataGrid.TotalCount = result.TotalCount;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.QueryBuilder);
            if (flag)
            {
                this._lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InvoiceDetailReportQueryVM>(_queryVM);

                this.DataGrid.Bind();
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
            //importer.ShowDialog("导入运单号", null);
            importer.ShowDialog(ResInvoiceDetailReport.Button_ImportSAP, null);
        }

        private void Hyperlink_OrderID_Click(object sender, RoutedEventArgs e)
        {
            var data = this.DataGrid.SelectedItem as dynamic;
            if (data != null)
            {
                if (data.OrderType == "SO")
                {
                    if (data.HaveSoInfo > 0)
                    {
                        this.Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, data.OrderID), null, true);
                    }
                    else
                    {
                        if (data.RefundBalance != null)
                        {
                            this.Window.Navigate(string.Format(ConstValue.RMA_RefundBalanceQueryUrl, data.RefundBalance), null, true);
                        }
                    }
                }
                else if (data.OrderType == "RO")
                {
                    if (data.IsRO_Balance > 0)
                    {
                        this.Window.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, data.OrderID), null, true);
                    }
                    else
                    {
                        if (data.RefundBalance != null)
                        {
                            this.Window.Navigate(string.Format(ConstValue.RMA_RefundBalanceQueryUrl, data.RefundBalance), null, true);
                        }
                    }
                }
            }
        }
    }
}