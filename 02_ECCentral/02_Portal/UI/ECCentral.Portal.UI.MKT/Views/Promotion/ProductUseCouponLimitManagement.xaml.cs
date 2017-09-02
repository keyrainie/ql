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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.BizEntity.MKT;


namespace ECCentral.Portal.UI.MKT.Views
{
     [View(IsSingleton = true, SingletonType = SingletonTypes.Url,NeedAccess=false)]
    public partial class ProductUseCouponLimitManagement : PageBase
    {
         private ProductUseCouponLimitQueryVM model;
         private ProductUseCouponLimitFacade facade;
        public ProductUseCouponLimitManagement()
        {
            InitializeComponent();
            this.ProductUseCouponResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ProductUseCouponResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
                model = new ProductUseCouponLimitQueryVM();
                facade = new ProductUseCouponLimitFacade();
                this.DataContext = model;
            };
        }

        void ProductUseCouponResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetProductUseCouponLimitByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.ProductUseCouponResult.ItemsSource = arg.Result.Rows.ToList("IsChecked", false);
                this.ProductUseCouponResult.TotalCount = arg.Result.TotalCount;

            });
        }


        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ProductUseCouponResult.Bind();
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.ProductUseCouponResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ProductUseCouponLimitMaintain item = new ProductUseCouponLimitMaintain();
       
            item.Dialog = Window.ShowDialog("商品添加", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ProductUseCouponResult.Bind();
                }
            });
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<ProductUseCouponLimitInfo> list = new List<ProductUseCouponLimitInfo>();

            dynamic viewlist = this.ProductUseCouponResult.ItemsSource as dynamic;
            if (viewlist != null)
            {
                foreach (var item in viewlist)
                {
                    if (item.IsChecked == true)
                    {
                        list.Add(new ProductUseCouponLimitInfo() { CouponLimitType = item.Type, SysNo =item.SysNo });
                    }
                }
            }
            if (list.Count > 0)
            {
              Window.Confirm("是否删除!", (objs, args) =>
              {
                  if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                  {
                      facade.DeleteProductUseCouponLimit(list, (obj, arg) =>
                      {
                          if (arg.FaultsHandle())
                          {
                              return;
                          }
                          Window.MessageBox.Show("删除成功!", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Success);
                          this.ProductUseCouponResult.Bind();
                      });
                  }
              });
            }
            else
            {
                Window.Alert("请先选择！");
            }
        }

    }
}
