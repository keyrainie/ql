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

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryRelated :PageBase
    {
        CategoryRelatedQueryVM model;
        public CategoryRelated()
        {
            InitializeComponent();
            this.btnDelete.Click += new RoutedEventHandler(btnDelete_Click);
         
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            dynamic viewlist = this.CategoryRelatedResult.ItemsSource as dynamic;
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
                        CategoryRelatedFacade facade = new CategoryRelatedFacade();
                         facade.DeleteCategoryRelated(list, (objs, arg) =>
                        {
                            if (arg.FaultsHandle())
                            {
                                return;

                            }
                            Window.Alert("删除成功");
                            this.CategoryRelatedResult.Bind();
                        });
                    }
                 });
            }
            else
            {
                Window.Alert("请先选择");
            }
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new CategoryRelatedQueryVM();
            this.DataContext = model;
            this.CategoryRelatedResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(CategoryRelatedResult_LoadingDataSource);
        }

        void CategoryRelatedResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            CategoryRelatedFacade facade = new CategoryRelatedFacade();
            facade.GetCategoryRelatedByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                this.CategoryRelatedResult.ItemsSource = arg.Result.Rows.ToList("IsChecked", false);
                this.CategoryRelatedResult.TotalCount = arg.Result.TotalCount;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.CategoryRelatedResult.Bind();

        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.CategoryRelatedResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CategoryRelatedMaintain item = new CategoryRelatedMaintain();
            item.Dialog = Window.ShowDialog("相关类别添加", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    CategoryRelatedResult.Bind();
                }
            }, new Size(700, 200));
        }

    }
}
