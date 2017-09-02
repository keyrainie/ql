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
    public partial class SODeliveryScoreSearch : PageBase
    {
        SODeliveryScoreSearchVM exportSODeliveryScoreSearchVM = null;
        SODeliveryScoreSearchVM queryVM = null;
        OtherDomainQueryFacade otherFacade;
        SOQueryFacade soQueryFacade;

        public SODeliveryScoreSearch()
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
            queryVM = new SODeliveryScoreSearchVM();
            exportSODeliveryScoreSearchVM = new SODeliveryScoreSearchVM();

            otherFacade = new OtherDomainQueryFacade(this);

            otherFacade.GetFreightManList(true, freightManList =>
            {
                queryVM.FreightMenList = freightManList;
            });

            gridConditions.DataContext = queryVM;
        }

        private void QuerySODeliveryScore()
        {
            soQueryFacade.SODeliveryScoreQuery(queryVM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                dataGridDeliveryScore.TotalCount = args.Result.TotalCount;
                dataGridDeliveryScore.ItemsSource = args.Result.Rows;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dataGridDeliveryScore.Bind();
        }

        private void dataGridDeliveryScore_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.SO_ExcelExport))
            {
                Window.Alert(ResSO.Msg_Error_Right, MessageType.Error);
                return;
            }

            if (exportSODeliveryScoreSearchVM != null && exportSODeliveryScoreSearchVM.PageInfo != null)
            {
                ColumnSet col = new ColumnSet(dataGridDeliveryScore);
                exportSODeliveryScoreSearchVM.PageInfo.PageSize = dataGridDeliveryScore.TotalCount;
                soQueryFacade.ExportSODeliveryScore(exportSODeliveryScoreSearchVM, new ColumnSet[] { col });
            }
        }

        private void dataGridDeliveryScore_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            queryVM.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            exportSODeliveryScoreSearchVM = queryVM.DeepCopy();
            QuerySODeliveryScore();
        }

        private void hyperlinkButton_OrderSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.SOMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }
    }

}
