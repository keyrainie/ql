using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCOrderPicker : PopWindow
    {
        private OrderQueryVM queryVM;
        private ARWindowFacade trackingInfoFacade;

        private UCOrderPicker()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(UCOrderPicker_Loaded);
        }

        public UCOrderPicker(ARWindowFacade facade)
            : this()
        {
            trackingInfoFacade = facade;
        }

        private void UCOrderPicker_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= new System.Windows.RoutedEventHandler(UCOrderPicker_Loaded);
            InitData();
        }

        private void InitData()
        {
            queryVM = new OrderQueryVM()
            {
                OrderType = SOIncomeOrderType.SO
            };
            this.SearchBuilder.DataContext = queryVM;
        }

        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.SearchBuilder);
            if (flag)
            {
                this.dgOrderQueryResult.Bind();
            }
        }

        private void btnAddTrackingInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedOrderList = GetSelectedOrderList();
            if (selectedOrderList.Count <= 0)
            {
                AlertInformationDialog(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            trackingInfoFacade.BatchCreateTrackingInfo(selectedOrderList, msg =>
            {
                CloseDialog(msg, DialogResultType.OK);
            });
        }

        private void dgOrderQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            trackingInfoFacade.QueryOrder(queryVM.OrderSysNo, queryVM.OrderType,
                result =>
                {
                    this.dgOrderQueryResult.ItemsSource = result.ResultList;
                    this.dgOrderQueryResult.TotalCount = result.TotalCount;
                });
        }

        private List<OrderVM> GetSelectedOrderList()
        {
            var selectedOrderList = new List<OrderVM>();
            var dataSource = this.dgOrderQueryResult.ItemsSource as List<OrderVM>;
            if (dataSource != null)
            {
                selectedOrderList = dataSource.Where(w => w.IsChecked)
                    .Select(s => s).ToList();
            }
            return selectedOrderList;
        }

        private void ckbSelectAllRow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dataSource = this.dgOrderQueryResult.ItemsSource as List<OrderVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }
    }
}