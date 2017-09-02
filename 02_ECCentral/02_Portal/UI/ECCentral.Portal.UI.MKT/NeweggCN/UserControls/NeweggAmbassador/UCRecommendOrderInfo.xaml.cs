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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.NeweggCN.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.NeweggCN.Resources;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.NeweggCN.UserControls
{
    public partial class UCRecommendOrderInfo : UserControl
    {
        private NeweggAmbassadorOrderQueryVM _CurrentQueryVM = new NeweggAmbassadorOrderQueryVM();


        public UCRecommendOrderInfo()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(UCPurchasingOrderInfo_Loaded);

        }

        void UCPurchasingOrderInfo_Loaded(object sender, RoutedEventArgs e)
        {
            this.Grid_QueryGrid.DataContext = _CurrentQueryVM;
        }

        

       

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;
            this.Grid_OrderInfo.Bind();
        }


        private void Grid_OrderInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            NeweggAmbassadorFacade facade = new NeweggAmbassadorFacade(CPApplication.Current.CurrentPage);

            if (facade != null)
            {

                facade.QueryAmbassadorRecommendOrderInfo(_CurrentQueryVM, p, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    var orderInfoRows = args.Result[0].Rows.ToList();
                    var summaryInfoRows = args.Result[1].Rows.ToList();

                    this.Grid_OrderInfo.ItemsSource = orderInfoRows;
                    this.Grid_OrderInfo.TotalCount = args.Result[0].TotalCount;

                    this.Grid_SummaryInfo.ItemsSource = summaryInfoRows;
                    this.Grid_SummaryInfo.Bind();
                });
            }
        }

        private void Grid_OrderInfo_ExportAllClick(object sender, EventArgs e)
        {
            NeweggAmbassadorFacade facade = new NeweggAmbassadorFacade(CPApplication.Current.CurrentPage);

            ColumnSet columnSet = new ColumnSet(this.Grid_OrderInfo);

            facade.ExportAllRecommendOrderInfoToExcel(_CurrentQueryVM, new ColumnSet[] { columnSet });
        }
    }
}
