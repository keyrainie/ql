using System;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.SO.Views
{
    [View]
    public partial class WHSOOutStockSearch : PageBase
    {
        WHSOOutStockSearchVM exportWHSOOutStockSearchInfo = null;
        WHSOOutStockSearchView pageView;
        SOQueryFacade soQueryFacade;

        public WHSOOutStockSearch()
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
            pageView = new WHSOOutStockSearchView();
            soQueryFacade = new SOQueryFacade(this);
            exportWHSOOutStockSearchInfo = new WHSOOutStockSearchVM();
       
            CodeNamePairHelper.GetList(ConstValue.DomainName_Common, ConstValue.Key_TimeRange, CodeNamePairAppendItemType.Custom_All, (sender, e) =>
            {
                pageView.QueryInfo.TimeRangeList = e.Result;
            });
           
            gridConditions.DataContext = pageView.QueryInfo;

            if (cmbShipTypeConditionType.DataContext != null)
            {
                cmbShipTypeConditionType.SelectedIndex = 0;
            }
        }

        private void hyperlinkButton_SOID_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.SOMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }
        
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dataGridWHSOOutStock.Bind();
        }
   
        private void QueryWHSOOutStock()
        {
            soQueryFacade.WHSOOutStockQuery(pageView.QueryInfo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                dataGridWHSOOutStock.TotalCount = args.Result.TotalCount;
                dataGridWHSOOutStock.ItemsSource = args.Result.Rows;
            });
        }

        private void dataGridWHSOOutStock_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            pageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            pageView.QueryInfo.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            exportWHSOOutStockSearchInfo = pageView.QueryInfo.DeepCopy();
            QueryWHSOOutStock();
        }

        private void dataGridWHSOOutStock_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.SO_ExcelExport))
            {
                Window.Alert(ResSO.Msg_Error_Right, MessageType.Error);
                return;
            }

            if (exportWHSOOutStockSearchInfo != null && exportWHSOOutStockSearchInfo.PageInfo != null)
            {
                ColumnSet col = new ColumnSet(dataGridWHSOOutStock);
                exportWHSOOutStockSearchInfo.PageInfo.PageSize = dataGridWHSOOutStock.TotalCount;
                soQueryFacade.ExportWHSOOutStock(exportWHSOOutStockSearchInfo, new ColumnSet[] { col });
            }
        }
    }

}
