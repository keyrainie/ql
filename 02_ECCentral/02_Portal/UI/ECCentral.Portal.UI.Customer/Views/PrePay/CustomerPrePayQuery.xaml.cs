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
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;

namespace ECCentral.Portal.UI.Customer.Views.PrePay
{
    [View]
    public partial class CustomerPrePayQuery : PageBase
    {
        CustomerPrePayVM viewModel;
        CommonDataFacade commFacade;
        PrePayQueryFilter filter;
        PrePayFacade facade;
        public CustomerPrePayQuery()
        {
            filter = new PrePayQueryFilter();
            viewModel = new CustomerPrePayVM();
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            commFacade = new CommonDataFacade(this);
            facade = new PrePayFacade(this);
            InitVM();
            this.DataContext = viewModel;
            base.OnPageLoad(sender, e);
        }


        private void InitVM()
        {
            CodeNamePairHelper.GetList("Customer", "PrepayType", CodeNamePairAppendItemType.All, (s, arg) =>
          {
              if (arg.FaultsHandle())
                  return;
              foreach (var item in arg.Result)
              {
                  viewModel.PrepayTypeList.Add(item);
              }
          });

        }

        private void dataGridPaymentLog_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter = viewModel.ConvertVM<CustomerPrePayVM, PrePayQueryFilter>();
            filter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            facade.QueryPrePayLogPayment(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridPaymentLog.ItemsSource = args.Result.Rows;
                dataGridPaymentLog.TotalCount = args.Result.TotalCount;
            });
        }

        private void dataGridInComeLog_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter = viewModel.ConvertVM<CustomerPrePayVM, PrePayQueryFilter>();
            filter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            facade.QueryPrePayLogIncome(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                dataGridInComeLog.ItemsSource = args.Result.Rows;
                dataGridInComeLog.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            dataGridInComeLog.Bind();
            dataGridPaymentLog.Bind();
        }

        private void ucCustomerPicker_CustomerSelected(object sender, CustomerSelectedEventArgs e)
        {

        }
    }

}
