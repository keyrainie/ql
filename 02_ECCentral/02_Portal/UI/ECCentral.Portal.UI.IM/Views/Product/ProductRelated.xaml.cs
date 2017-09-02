using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.UserControls;
using System.Windows.Controls;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Data;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductRelated : PageBase
    {
        private ProductRelatedQueryVM model;

        public ProductRelated()
        {
            InitializeComponent();
            this.ItemRelatedQueryResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ItemRelatedQueryResult_LoadingDataSource);
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductRelatedQueryVM();
            this.DataContext = model;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            this.ItemRelatedQueryResult.Bind();
        }

        void ItemRelatedQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductRelatedFacade facade = new ProductRelatedFacade();
            facade.GetProductRelatedByQuery(model, e.PageSize, e.PageIndex, e.SortField, (orj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.ItemRelatedQueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                this.ItemRelatedQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            ProductRelatedMaintain item = new ProductRelatedMaintain();
            item.Dialog = Window.ShowDialog("添加相关商品", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    ItemRelatedQueryResult.Bind();
                }
            }, new Size(600, 200));
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.ItemRelatedQueryResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            dynamic viewlist = this.ItemRelatedQueryResult.ItemsSource as dynamic;
            if (viewlist != null)
            {
                foreach (var item in viewlist)
                {
                    if (item.IsChecked == true)
                    {
                        list.Add(item.SysNo.ToString());

                    }
                }
            }
            if (list.Count > 0)
            {
                Window.Confirm("是否删除?", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        ProductRelatedFacade facade = new ProductRelatedFacade();

                        facade.DeleteItemRelated(list, (objs, arg) =>
                        {
                            if (arg.FaultsHandle())
                            {
                                return;

                            }
                            Window.Alert("删除成功");
                            this.ItemRelatedQueryResult.Bind();
                        });
                    }
                });
            }
            else
            {
                Window.Alert("请先选择");
            }
        }

        private void btn_BatchUpdate_Click(object sender, RoutedEventArgs e)
        {
            List<ProductRelatedVM> list = new List<ProductRelatedVM>();
            dynamic viewlist = this.ItemRelatedQueryResult.ItemsSource as dynamic;
            ProductRelatedFacade facade = new ProductRelatedFacade();
            if (viewlist != null)
            {
                foreach (var item in viewlist)
                {
                    if (item.IsChecked == true)
                    {
                        list.Add(new ProductRelatedVM() { SysNo = item.SysNo, Priority = Convert.ToString(item.Priority) });
                    }
                }
            }
            if (list.Count > 0)
            {
                facade.UpdateProductRelatePriority(list, (orj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert("更新成功");
                    this.ItemRelatedQueryResult.Bind();
                });
            }
            else
            {
                Window.Alert("请先选择");
            }
        }

        private void btn_BatchSet_Click(object sender, RoutedEventArgs e)
        {
            ProductRelateBatSet item = new ProductRelateBatSet();

            item.Dialog = Window.ShowDialog("批量设置", item, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {

                }
            }, new Size(800, 700));
        }
    }

}
