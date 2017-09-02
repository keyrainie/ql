using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using System.Windows.Media;

namespace ECCentral.Portal.UI.Invoice.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class Reconciliation : PageBase
    {
        private InvoiceFacade facade;
        private ReconciliationVM reconciliationVM;
        private ReconciliationQueryVM queryVM;

        public Reconciliation()
        {
            InitializeComponent();
            InitData();
            Loaded += new RoutedEventHandler(Reconciliation_OnOnLoad);
            dgInvoiceQueryResult.LoadingRow += dgInvoiceQueryResult_LoadingRow;
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermissions();
            base.OnPageLoad(sender, e);
        }

        private void Reconciliation_OnOnLoad(object sender, EventArgs e)
        {

            facade = new InvoiceFacade(this);
        }

        private void InitData()
        {
            queryVM = new ReconciliationQueryVM();
            SeachBuilder.DataContext = queryVM;
        }

        private void VerifyPermissions()
        {
            this.dgInvoiceQueryResult.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceQuery_Export);
        }

        private void dgInvoiceQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryReconciliation(queryVM, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle()) return;
                var gridVM = DynamicConverter<ReconciliationVM>.ConvertToVMList<List<ReconciliationVM>>(args.Result.Rows);
                foreach (ReconciliationVM temp in gridVM)
                {
                    if (temp.IncomeAmt.HasValue)
                        temp.IncomeAmt = decimal.Round(temp.IncomeAmt.Value, 2);
                    if (temp.TotalAmount.HasValue)
                        temp.TotalAmount = decimal.Round(temp.TotalAmount.Value, 2);
                    if (temp.OrderAmt.HasValue)
                        temp.OrderAmt = decimal.Round(temp.OrderAmt.Value, 2);
                }
                dgInvoiceQueryResult.ItemsSource = gridVM;
                dgInvoiceQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.dgInvoiceQueryResult.ItemsSource as List<ReconciliationVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }

        void dgInvoiceQueryResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var dataSource = this.dgInvoiceQueryResult.ItemsSource as List<ReconciliationVM>;
            var dataRow = dataSource[e.Row.GetIndex()];

            //支付金额有值，网关支付金额无值
            if (dataRow.IncomeAmt.HasValue&&!dataRow.TotalAmount.HasValue)
            {
                e.Row.Background = new SolidColorBrush(Colors.Orange);
            }

            //支付金额有值，网关支付金额有值，两值不相等
            if (dataRow.IncomeAmt.HasValue && dataRow.TotalAmount.HasValue && dataRow.IncomeAmt != dataRow.TotalAmount)
            {
                e.Row.Background = new SolidColorBrush(Colors.Red);
            }

            //支付金额有值，网关支付金额有值，两值相等
            if (dataRow.IncomeAmt.HasValue && dataRow.TotalAmount.HasValue && dataRow.IncomeAmt == dataRow.TotalAmount)
            {
                e.Row.Background = new SolidColorBrush(Colors.Green);
            }

            //支付金额无值，网关支付金额有值
            if (!dataRow.IncomeAmt.HasValue && dataRow.TotalAmount.HasValue)
            {
                e.Row.Background = new SolidColorBrush(Colors.Black);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            bool flag = ValidationManager.Validate(this.SeachBuilder);
            if (flag)
            {
                this.dgInvoiceQueryResult.Bind();
            }
        }

        private void dgInvoiceQueryResult_ExportAllClick(object sender, EventArgs e)
        {
           
        }

    }
}
