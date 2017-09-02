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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.Basic;
namespace ECCentral.Portal.UI.Customer.Views.SMS
{
    [View]
    public partial class EmailQuery : PageBase
    {
        EmailQueryFilter filter;
        EmailFacade facade;
        EmailQueryVM viewModel;

        public EmailQuery()
        {
            filter = new EmailQueryFilter();
            viewModel = new EmailQueryVM();
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new EmailFacade(this);
            InitVM();
            this.DataContext = viewModel;
            base.OnPageLoad(sender, e);
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_InfoMgmt_SendMail))
                this.btnSendEmail.IsEnabled = false;
            Grid.KeyDown += new KeyEventHandler(Grid_KeyDown);
        }

        void Grid_KeyDown(object sender, KeyEventArgs e)
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
                    DataGrid.Bind();
                    e.Handled = true;
                }
            }
        }

        private void InitVM()
        {
            viewModel.Source = "MailDB";
            cbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<EmailSendStatus>(EnumConverter.EnumAppendItemType.All);
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter = viewModel.ConvertVM<EmailQueryVM, EmailQueryFilter>();
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
                DataGrid.ItemsSource = args.Result.Rows;
                DataGrid.TotalCount = args.Result.TotalCount;
            });

        }


        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            SendEmail DialogPage = new SendEmail();
            DialogPage.Dialog = Window.ShowDialog(ECCentral.Portal.UI.Customer.Resources.ResSMSQuery.PopTitle_SendEmail, DialogPage);
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.Bind();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem != null)
            {
                facade.GetMailContent((DataGrid.SelectedItem as dynamic).SysNo, viewModel.Source, callback);
            }
        }
        private EventHandler<RestClientEventArgs<string>> callback = new EventHandler<RestClientEventArgs<string>>((obj, args) =>
        {
            if (args.FaultsHandle())
                return;
            HtmlViewHelper.ViewHtmlInBrowser("Customer", args.Result);
        });


    }
}
