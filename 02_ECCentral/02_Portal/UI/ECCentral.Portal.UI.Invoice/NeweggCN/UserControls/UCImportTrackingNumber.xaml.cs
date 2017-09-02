using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Controls.Uploader;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.UI.Invoice.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.UserControls
{
    /// <summary>
    /// 导入运单号
    /// </summary>
    public partial class UCImportTrackingNumber : PopWindow
    {
        private InvoiceReportFacade _facade;
        private ImportTrackingNumberVM _viewModel;

        public UCImportTrackingNumber()
        {
            InitializeComponent();
            RegisterEvents();
            _viewModel = new ImportTrackingNumberVM();
            this.LayoutRoot.DataContext = _viewModel;
        }

        public int? SelectedStockSysNo
        {
            get
            {
                return this._viewModel.StockSysNo;
            }
            set
            {
                this._viewModel.StockSysNo = value;
                this.cmbStock.IsEnabled = false;
            }
        }

        private void RegisterEvents()
        {
            this.Loaded += new RoutedEventHandler(UCImportTrackingNumber_Loaded);
            this.uploader.AllFileUploadCompleted += new Basic.Controls.Uploader.AllUploadCompletedEvent(uploader_AllFileUploadCompleted);
        }

        private void UCImportTrackingNumber_Loaded(object sender, RoutedEventArgs e)
        {
            string key = "InvoiceDetailReportStockList-" + CPApplication.Current.CompanyCode;
            CodeNamePairHelper.GetList("Invoice", key, CodeNamePairAppendItemType.None, (obj, args) =>
            {
                _viewModel.StockList = args.Result;
                cmbStock.SelectedIndex = 0;
            });

            _facade = new InvoiceReportFacade(this.CurrentPage);
        }

        private void uploader_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            uploader.Clear();
            if (args.UploadInfo[0].UploadResult == SingleFileUploadStatus.Failed)
            {
                this.AlertErrorDialog(ResInvoiceDetailReport.Message_UploadFail);
                return;
            }

            int stockSysNo = Convert.ToInt32(this.cmbStock.SelectedValue);
            _facade.ImportTrackingNumber(args.UploadInfo[0].ServerFilePath, stockSysNo, result =>
            {
                this.DataGrid_SuccessList.ItemsSource = result.SuccessList;
                this.DataGrid_FailedList.ItemsSource = result.FailedList;

                this.tbStatisticInfo.Text = string.Format(ResInvoiceDetailReport.Message_StatisticInfo
                    , result.SuccessList.Count + result.FailedList.Count
                    , result.SuccessList.Count
                    , result.FailedList.Count);
            });
        }

        private void btnDownloadTemp_Click(object sender, RoutedEventArgs e)
        {
            AppSettingHelper.GetSetting(ConstValue.DomainName_Invoice, "ImportTrackingNumberTemplate", (obj, args) =>
            {
                string url = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Invoice, "ServiceBaseUrl") + args.Result;

                UtilityHelper.OpenWebPage(url);
            });
        }
    }
}