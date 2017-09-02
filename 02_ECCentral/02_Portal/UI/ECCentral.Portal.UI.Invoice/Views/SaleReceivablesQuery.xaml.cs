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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.Invoice.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 应收单
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page)]
    public partial class SaleReceivablesQuery : PageBase
    {
        public SaleReceivablesQuery()
        {
            InitializeComponent();
        }

        private SaleReceivablesQueryVM queryVM;
        private SaleIncomeFacade soIncomeFacade;

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryVM = new SaleReceivablesQueryVM();
            soIncomeFacade = new SaleIncomeFacade(this);
            this.DataContext = queryVM;
            cmbCurrency.ItemsSource = EnumConverter.GetKeyValuePairs<SaleCurrency>(EnumConverter.EnumAppendItemType.None);
            cmbCurrency.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.dgQueryResult.Bind();
        }

        private void dgQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            if (!ValidationManager.Validate(this)) return;

            soIncomeFacade.QuerySaleReceivables(queryVM, e.PageSize, e.PageIndex, e.SortField, args =>
            {
                this.dgQueryResult.ItemsSource = args.Rows;
                this.dgQueryResult.TotalCount = args.TotalCount;
                this.dgQueryResult.Columns[0].Header = ucPayType.SelectedPayTypeItem.PayTypeName;
            });
        }

        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            Export();
        }


        private void btnExportAll_Click(object sender, RoutedEventArgs e)
        {
            Export();
        }

        private void Export()
        {
            if (!ValidationManager.Validate(this)) return;

            ColumnSet col = new ColumnSet(this.dgQueryResult);
            soIncomeFacade.ExportSaleReceivablesExcelFile(queryVM, new ColumnSet[] { col });
        }
    }
}
