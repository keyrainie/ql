using System;
using System.Windows;

using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View]
    public partial class ProductCardsQuery : PageBase
    {
        ReportFacade facade;
        public ProductCardsQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new ReportFacade(this);
            base.OnPageLoad(sender, e);
        }
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ucProductPicker.ProductSysNo))
            {
                this.DataGrid_Inventory_ResultList.Bind();
                this.DataGrid_ProductCards_ResultList.Bind();
            }
            else 
            {
                Window.Alert(ResRMAReports.Msg_ProductIsEmpty);
            }
        }

        private void DataGrid_Inventory_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryProductCardInventory(int.Parse(this.ucProductPicker.ProductSysNo), (obj, args) =>
            {
                this.DataGrid_Inventory_ResultList.ItemsSource = args.Result.Rows;
            });
        }

        private void DataGrid_ProductCards_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryProductCards(int.Parse(this.ucProductPicker.ProductSysNo), (obj, args) =>
            {
                this.DataGrid_ProductCards_ResultList.ItemsSource = args.Result.Rows;
            });
        }

    }
}
