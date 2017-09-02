using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductExt : PageBase
    {
        ProductExtQueryVM model;
        public ProductExt()
        {
            InitializeComponent();
            itemExtGrid.ItemExtQueryResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ItemExtQueryResult_LoadingDataSource);
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductExtQueryVM();
            this.DataContext = model;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            itemExtGrid.ItemExtQueryResult.Bind();

        }

        void ItemExtQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductExtFacade facade = new ProductExtFacade();
            facade.GetProductExtByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                itemExtGrid.ItemExtQueryResult.ItemsSource = arg.Result.Rows.ToList("IsChecked", false); 
                itemExtGrid.ItemExtQueryResult.TotalCount = arg.Result.TotalCount;

            });
        }

        private void btn_CanRefund_Click(object sender, RoutedEventArgs e)
        {
            ProductExtFacade facade = new ProductExtFacade();
            List<ProductExtVM> list = new List<ProductExtVM>();
            dynamic viewlist = this.itemExtGrid.ItemExtQueryResult.ItemsSource as dynamic;
            foreach (var item in viewlist)
            {
                if (item.IsChecked == true)
                {
                    list.Add(new ProductExtVM() { IsPermitRefund = 1, SysNo = item.sysno });
                }
            }
            if (list.Count > 0)
            {
                facade.UpdatePermitRefund(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert("设置成功");
                    itemExtGrid.ItemExtQueryResult.Bind();
                    itemExtGrid.cbtemp.IsChecked = false;
                });
            }
            else
            {
                Window.Alert("请先选择");
            }
        }

        private void btn_CanNotRefund_Click(object sender, RoutedEventArgs e)
        {
            ProductExtFacade facade = new ProductExtFacade();
            List<ProductExtVM> list = new List<ProductExtVM>();
            dynamic viewlist = this.itemExtGrid.ItemExtQueryResult.ItemsSource as dynamic;
            foreach (var item in viewlist)
            {
                if (item.IsChecked == true)
                {
                    list.Add(new ProductExtVM() { IsPermitRefund = 0, SysNo = item.sysno });
                }
            }
            if (list.Count > 0)
            {
                facade.UpdatePermitRefund(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert("设置成功");
                    itemExtGrid.ItemExtQueryResult.Bind();
                    itemExtGrid.cbtemp.IsChecked = false;
                });
            }
            else
            {
                Window.Alert("请先选择");
            }
        }
    }
}
