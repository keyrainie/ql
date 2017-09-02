using System;
using System.Windows;

using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class InvoiceChangeLog : PageBase
    {
        CommonDataFacade CommonDataFacade;
        SOInvoiceChangeLogQueryFilter m_query;
        public InvoiceChangeLog()
        {
            InitializeComponent();
            CommonDataFacade = new Basic.Components.Facades.CommonDataFacade(this);
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            spConditions.DataContext = m_query = new SOInvoiceChangeLogQueryFilter();
            IntiPageData();
        }
        private void IntiPageData()
        {
            CommonDataFacade.GetStockList(true, (sender, e) =>
            {
                if (e.FaultsHandle()) return;
                cmbStock.ItemsSource = e.Result;
                if (e.Result.Count > 0)
                {
                    cmbStock.SelectedIndex = 0;
                }
            });

            this.cmbChangeType.ItemsSource = EnumConverter.GetKeyValuePairs<InvoiceChangeType>(EnumConverter.EnumAppendItemType.All);
            this.cmbChangeType.SelectedIndex = 0;

        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
           dataGridInvoiceChangLog.Bind();
        }

        private void dataGridInvoiceChangLog_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QuerySOInvoiceChangeLog(m_query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridInvoiceChangLog.TotalCount = args.Result.TotalCount;
                dataGridInvoiceChangLog.ItemsSource = args.Result.Rows;
            });
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml info = this.dataGridInvoiceChangLog.SelectedItem as DynamicXml;
            if (info != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, info["SONumber"]), null, true);
            }
        }
    }

}
