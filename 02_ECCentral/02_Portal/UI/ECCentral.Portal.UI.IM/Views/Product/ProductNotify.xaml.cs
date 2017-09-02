using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.UserControls;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductNotify : PageBase
    {
        private ProductNotifyQueryVM model;
        public ProductNotify()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                model = new ProductNotifyQueryVM();
                if (!string.IsNullOrEmpty(Request.Param))
                {
                    model.ProductSysNo = Request.Param;
                }
                this.DataContext = model;
            };
            this.ProductNotifyQueryResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ProductNotifyQueryResult_LoadingDataSource);
        }

      
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            
            this.ProductNotifyQueryResult.Bind();
        }

        void ProductNotifyQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductNotifyFacade facade = new ProductNotifyFacade();
            facade.GetProductNotifyByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, org) =>
            {
                this.ProductNotifyQueryResult.ItemsSource = org.Result.Rows;
                this.ProductNotifyQueryResult.TotalCount = org.Result.TotalCount;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mybox.Show();
        }

      

    }
}
