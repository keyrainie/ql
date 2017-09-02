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
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.SO.Views
{
    [View]
    public partial class SODeliveryHistorySearch : PageBase
    {
        SODeliveryHistorySearchVM exportSODeliveryHistorySearchVM = null;
        SODeliveryHistorySearchVM queryVM = null;
        OtherDomainQueryFacade otherFacade;
        SOQueryFacade soQueryFacade;

        public SODeliveryHistorySearch()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
        }

        private void IniPageData()
        {
            soQueryFacade = new SOQueryFacade(this);
            queryVM = new SODeliveryHistorySearchVM();
            exportSODeliveryHistorySearchVM = new SODeliveryHistorySearchVM();

            otherFacade = new OtherDomainQueryFacade(this);

            otherFacade.GetFreightManList(true, freightManList =>
            {
                queryVM.FreightMenList = freightManList;
            });

            gridConditions.DataContext = queryVM;

            if (cmbOrderType.DataContext != null)
            {
                cmbOrderType.SelectedIndex = 0;
            }
        }

        private void QuerySODeliveryHistory()
        {
            soQueryFacade.SODeliveryHistoryQuery(queryVM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                dataGridSODeliveryHistory.TotalCount = args.Result.TotalCount;
                dataGridSODeliveryHistory.ItemsSource = args.Result.Rows;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dataGridSODeliveryHistory.Bind();
        }

        private void dataGridSODeliveryHistory_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            queryVM.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            exportSODeliveryHistorySearchVM = queryVM.DeepCopy();
            QuerySODeliveryHistory();
        }

        private void dataGridSODeliveryHistory_ExportAllClick(object sender, EventArgs e)
        {
            if (exportSODeliveryHistorySearchVM != null && exportSODeliveryHistorySearchVM.PageInfo != null)
            {
                ColumnSet col = new ColumnSet(dataGridSODeliveryHistory);
                exportSODeliveryHistorySearchVM.PageInfo.PageSize = dataGridSODeliveryHistory.TotalCount;
                soQueryFacade.ExportSODeliveryHistory(exportSODeliveryHistorySearchVM, new ColumnSet[] { col });
            }
        }

        private void hyperlinkButton_OrderSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.SOMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }
    }

}
