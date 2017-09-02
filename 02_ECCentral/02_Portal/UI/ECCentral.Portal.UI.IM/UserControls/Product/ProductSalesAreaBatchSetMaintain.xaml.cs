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
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM.Product;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductSalesAreaBatchSetMaintain : UserControl
    {


        private ProductSalesAreaBatchFacade facade;
        private ProductSalesAreaBatchQueryVM model;
        public List<ProductInfo> productList; //存放勾选的商品
        
        public ProductSalesAreaBatchSetMaintain()
        {
            model = new ProductSalesAreaBatchQueryVM();
            InitializeComponent();
            ProductSalesAreaBatchResult.LoadingDataSource += new EventHandler<Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs>(ProductSalesAreaBatchResult_LoadingDataSource);
            this.Loaded += (sender, e) =>
            {
                this.DataContext = model;
                facade = new ProductSalesAreaBatchFacade();
            };
        }

        void ProductSalesAreaBatchResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetProductByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.ProductSalesAreaBatchResult.ItemsSource = arg.Result.Rows.ToList("IsChecked", false);
                this.ProductSalesAreaBatchResult.TotalCount = arg.Result.TotalCount;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.ProductSalesAreaBatchResult.Bind();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            productList = new List<ProductInfo>();
            List<ProductSalesAreaInfo> productSalesAreaInfoList;
            ProductInfo info;
            bool flag = false;

            //先找出勾选的仓库和省份 避免下面多次循环
            var listStock = from s in myProductSalesAreaBatchWarehouse.listStockVM where s.IsChecked == true && s.StockID!="none" select s; 

            var listProvince = from p in myProductSalesAreaBatchWarehouse.listProvinceVM where p.IsChecked == true && p.SysNo!=-1 select p;

             var viewlist = this.ProductSalesAreaBatchResult.ItemsSource as dynamic;
             if (viewlist != null)
             {
                 foreach (var item in viewlist)
                 {
                     if (item.IsChecked == true)
                     {
                         info = new ProductInfo();
                         info.SysNo = item.SysNo;
                        info.ProductID = item.productId;
                         productSalesAreaInfoList = new List<ProductSalesAreaInfo>();
                         foreach (var stock in listStock)
                         {
                             foreach (var province in listProvince)
                             {
                                
                                 productSalesAreaInfoList.Add(new ProductSalesAreaInfo()
                                 {
                                     CompanyCode= CPApplication.Current.CompanyCode,
                                     LanguageCode=CPApplication.Current.LanguageCode,
                                     Province = new AreaInfo() { ProvinceName = province.ProvinceName, ProvinceSysNo = province.SysNo },
                                     Stock = new StockInfo() { StockID = stock.StockID, StockName = stock.StockName, StockStatus = stock.StockStatus, SysNo = stock.SysNo }
                                 });
                               
                                 flag = true;
                            }
                         }
                         info.ProductSalesAreaInfoList = productSalesAreaInfoList;
                         info.OperateUser = new UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo,UserDisplayName=CPApplication.Current.LoginUser.DisplayName,UserID=CPApplication.Current.LoginUser.ID};
                         productList.Add(info);
                     }

                 }
             }
            if (productList.Count == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("没有选择商品");
                return;
            }
            else if (!flag)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("没有选择销售区域");
                return;
            }
            
            
            facade.UpdateProductSalesAreaInfo(productList, (obj, arg) => {
                if (arg.FaultsHandle())
                {
                    return;
                }

                CPApplication.Current.CurrentPage.Context.Window.Alert("批量设置成功");
            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.ProductSalesAreaBatchResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }
       
    }
}
