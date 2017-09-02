using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductShow : PageBase
    {
        ProductShowQueryVM model;
        public ProductShow()
        {
            InitializeComponent();
            this.ProductShowResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ProductShowResult_LoadingDataSource);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductShowQueryVM();
            this.DataContext = model;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (model.EditDateFrom != null && model.EditDateTo != null)
            {
                if (model.EditDateFrom.Value.CompareTo(model.EditDateTo) > 0)
                {
                    Window.MessageBox.Show("更新的结束时间小于了开始时间,会查不出数据",Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Warning);

                }
            }
            if (model.FirstOnlineTimeFrom != null && model.FirstOnlineTimeTo != null)
            {
                if (model.FirstOnlineTimeFrom.Value.CompareTo(model.FirstOnlineTimeTo) > 0)
                {
                    Window.MessageBox.Show("首次上架的结束时间小于了开始时间,会查不出数据",Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Warning);
                }
            }
           
            this.ProductShowResult.Bind();
        }

        void ProductShowResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductShowFacade facade = new ProductShowFacade();
            facade.GetProductRelatedByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                this.ProductShowResult.ItemsSource = arg.Result.Rows;
                this.ProductShowResult.TotalCount = arg.Result.TotalCount;

            });
            
        }

        private void hyperlinkProductSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic product = this.ProductShowResult.SelectedItem as dynamic;
            if (product != null)
            {
                string websiteProductUrl = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductUrl);
                //ocean.20130514,Move to ControlPanelConfiguration
                Window.Navigate(string.Format(websiteProductUrl, product.SysNo));

            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
         var lnk = sender as FrameworkElement;
            if (null != lnk)
            {
                Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, lnk.Tag.ToString()), null, true);
            }
        }

        private void ProductShowResult_ExportAllClick(object sender, EventArgs e)
        {
            ProductShowFacade facade = new ProductShowFacade();
            ColumnSet col = new ColumnSet();
            col.Insert(0, "ProductID", "产品号");
            col.Insert(1, "ProductTitle", "产品描述");
            col.Insert(2, "Category1Name", "产品一级类");
            col.Insert(3, "Category2Name", "产品二级类");
            col.Insert(4, "Category3Name", "产品三级类");
            col.Insert(5, "FirstOnlineTime", "上架时间");
            col.Insert(6, "EditDate", "更新时间");
            col.Insert(7, "Status", "状态");
            facade.ExprotExecl(model, new ColumnSet[] {col });
            

        }

    }
}
