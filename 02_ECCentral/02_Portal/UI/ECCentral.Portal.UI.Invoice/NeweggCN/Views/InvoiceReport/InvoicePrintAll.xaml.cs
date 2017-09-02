using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Views.InvoiceReport
{
    /// <summary>
    /// 发票打印
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class InvoicePrintAll : PageBase
    {
        private InvoicePrintAllQueryVM _queryVM;
        private InvoicePrintAllQueryVM _lastQueryVM;
        private InvoiceReportFacade _facade;

        public InvoicePrintAll()
        {
            InitializeComponent();
            InitData();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this._facade = new InvoiceReportFacade(this);
            VerifyPermissions();
            LoadComboBoxData();
        }

        private void VerifyPermissions()
        {
            this.DataGrid.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoicePrintAll_Export);

        }


        private void LoadComboBoxData()
        {
            string key = "InvoicePrintStockList-" + CPApplication.Current.CompanyCode;
            CodeNamePairHelper.GetList("Invoice", key, CodeNamePairAppendItemType.All, (obj, args) =>
            {
                cmbStock.ItemsSource = args.Result;
                cmbStock.SelectedIndex = 0;
            });
        }

        private void InitData()
        {
            _queryVM = new InvoicePrintAllQueryVM();
            this.QueryBuilder.DataContext = _lastQueryVM = _queryVM;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoicePrintAll_Print))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.QueryBuilder);
            if (flag)
            {
                this._lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InvoicePrintAllQueryVM>(_queryVM);

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
            col.Insert(2, "InvoiceType", "发票类型");
            _facade.ExportAllInvoiceExcelFile(_lastQueryVM, new ColumnSet[] { col });
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _facade.QueryAllInvoice(_lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                this.DataGrid.ItemsSource = result.Rows.ToList("IsChecked", false);
                this.DataGrid.TotalCount = result.TotalCount;
            });
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

        private void Hyperlink_Priview_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dPrint = new Dictionary<string, string>();

            HyperlinkButton button = e.OriginalSource as HyperlinkButton;

            DynamicXml source = button.DataContext as DynamicXml;

            dPrint["SOSysNo"] = source["SOID"].ToString();
            dPrint["WareHouseNumber"] = source["WareHouseNumber"].ToString();

            HtmlViewHelper.WebPrintPreview("Invoice", "InvoiceDetailPrint", dPrint);
        }

        private void Hyperlink_OrderID_Click(object sender, RoutedEventArgs e)
        {
            var data = this.DataGrid.SelectedItem as dynamic;
            if (data != null)
            {
                this.Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, data.SOID), null, true);
            }
        }
    }
}