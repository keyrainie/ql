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
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views.SMS
{
    [View]
    public partial class SMSQuery : PageBase
    {
        SMSQueryFilter filter;
        SMSFacade facade;
        SMSVM viewModel;
        public SMSQuery()
        {
            filter = new SMSQueryFilter();
            viewModel = new SMSVM();

            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new SMSFacade(this);
            this.DataContext = viewModel;
            base.OnPageLoad(sender, e);
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_SMS_SendSMS))
                this.btnSendSMS.IsEnabled = false;
            SearchBuilder.KeyDown += new KeyEventHandler(SearchBuilder_KeyDown);
        }

        void SearchBuilder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                var txtBinding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (txtBinding != null)
                {
                    txtBinding.UpdateSource();
                    dataGrid1.Bind();
                    e.Handled = true;
                }
            }
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.Bind();
        }

        private void dataGrid1_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.Tel = tbTel.Text.Trim();
            filter.FromDate = DatePicker_ApplyDateRange.SelectedDateStart;
            filter.ToDate = DatePicker_ApplyDateRange.SelectedDateEnd;
            filter.WebChannel = viewModel.ChannelID;
            filter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            facade.Query(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGrid1.ItemsSource = args.Result.Rows;
                dataGrid1.TotalCount = args.Result.TotalCount;
            });

        }

        private void btnSendSMS_Click(object sender, RoutedEventArgs e)
        {
            SendSMS DialogPage = new SendSMS();
            DialogPage.Dialog = Window.ShowDialog(ECCentral.Portal.UI.Customer.Resources.ResSMSQuery.PopTitle_SendSMS, DialogPage, (obj, args) =>
            {

            });
        }


    }

}
