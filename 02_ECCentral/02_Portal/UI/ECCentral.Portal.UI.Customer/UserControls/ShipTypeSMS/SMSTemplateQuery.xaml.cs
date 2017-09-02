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
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.QueryFilter.Customer;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class SMSTemplateQuery : UserControl
    {
        ShipTypeSMSTemplateQueryFilter filter;
        ShipTypeSMSTemplateFacade facade;
        public IDialog Dialog { get; set; }
        public SMSTemplateQuery()
        {
            filter = new ShipTypeSMSTemplateQueryFilter();
            facade = new ShipTypeSMSTemplateFacade(CPApplication.Current.CurrentPage);
            InitializeComponent();
            tbKeywords.KeyDown += new KeyEventHandler(tbKeywords_KeyDown);
        }

        void tbKeywords_KeyDown(object sender, KeyEventArgs e)
        {
            dataGrid1.Bind();
        }


        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.Bind();
        }

        private void dataGrid1_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.Keywords = tbKeywords.Text.Trim();
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
                dataGrid1.ItemsSource = args.Result.Rows.ToList("IsCheck",false);
                dataGrid1.TotalCount = args.Result.TotalCount;

            });
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            SMSTemplateEdit dialogWindow = new SMSTemplateEdit();
            dialogWindow.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResSMSTemplateQuery.windowTitle_AddTemplate, dialogWindow, (a, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    dataGrid1.Bind();
            }, new Size(400, 300));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                SMSTemplateEdit dialogWindow = new SMSTemplateEdit();
                dialogWindow.viewModel = DynamicConverter<ShipTypeSMSTemplateVM>.ConvertToVM(dataGrid1.SelectedItem);
                dialogWindow.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResSMSTemplateQuery.windowTitle_EditTemplate, dialogWindow, (a, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                        dataGrid1.Bind();
                }, new Size(400, 300));
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSMSTemplateQuery.msg_SelectTemplate);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close();
        }

        private void btnUse_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.ResultArgs.Data = (dataGrid1.SelectedItem as dynamic).Template;
                Dialog.Close();
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSMSTemplateQuery.msg_SelectTemplate);
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.ResultArgs.Data = (dataGrid1.SelectedItem as dynamic).Template;
                Dialog.Close();
            }
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic current = dataGrid1.SelectedItem as dynamic;
            if (current != null)
            {
                current.IsCheck = true;
            }
        }
    }
}
