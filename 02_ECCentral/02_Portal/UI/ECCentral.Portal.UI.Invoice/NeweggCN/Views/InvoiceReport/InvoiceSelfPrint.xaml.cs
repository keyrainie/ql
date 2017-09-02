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
    /// 自印发票查询
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class InvoiceSelfPrint : PageBase
    {
        private InvoiceSelfPrintQueryVM _queryVM;
        private InvoiceSelfPrintQueryVM _lastQueryVM;
        private InvoiceReportFacade _facade;

        public InvoiceSelfPrint()
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
            this.DataGrid.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceSelfPrint_Export);
        }

        private void LoadComboBoxData()
        {
            _facade.QueryInvoiceSelfStock((obj, args) =>
            {
                if (args.FaultsHandle() || args.Result==null || args.Result.Count == 0)
                    return;

                args.Result.Insert(0, new CodeNamePair() { Code="0", Name="--请选择--"});
                this.cmbStock.ItemsSource = args.Result;
                this.cmbStock.SelectedIndex = 0;
            });
        }

        private void InitData()
        {
            _queryVM = new InvoiceSelfPrintQueryVM();
            this.QueryBuilder.DataContext = _lastQueryVM = _queryVM;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.QueryBuilder);
            if (flag)
            {
                this._lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InvoiceSelfPrintQueryVM>(_queryVM);

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
            _facade.ExportInvoiceSelfExcelFile(_lastQueryVM, new ColumnSet[] { col });
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _facade.QueryInvoiceSelf(_lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                this.DataGrid.ItemsSource = result.Rows.ToList();
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
