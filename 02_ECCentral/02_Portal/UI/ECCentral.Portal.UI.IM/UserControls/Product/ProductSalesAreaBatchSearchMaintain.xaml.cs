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
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM.Product;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductSalesAreaBatchSearchMaintain : UserControl
    {
        private ProductSalesAreaBatchFacade facade;
        private ProductSalesAreaBatchQueryVM model;
       
        //初始化数据
        public ProductSalesAreaBatchSearchMaintain()
        {
            InitializeComponent();
            model = new ProductSalesAreaBatchQueryVM();
            this.SalesAreaBatchByProductResult.LoadingDataSource += new EventHandler<Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs>(SalesAreaBatchByProductResult_LoadingDataSource);
            this.SalesAreaBatchResult.LoadingDataSource += new EventHandler<Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs>(SalesAreaBatchResult_LoadingDataSource);
            this.Loaded += (sender, e) => {
                if (this.cboResultType.Items.Count == 0)
                {
                    this.cboResultType.Items.Add("商品");
                    this.cboResultType.Items.Add("记录");
                }
                this.cboResultType.SelectedIndex = 0;
                this.DataContext = model;
                facade = new ProductSalesAreaBatchFacade();
            };
        }
        //记录结果类型的bing
        void SalesAreaBatchResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.GetProductSalesAreaBatchList(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) =>
            {
                this.SalesAreaBatchResult.ItemsSource = arg.Result.Rows.ToList("IsChecked", false);
                this.SalesAreaBatchResult.TotalCount = arg.Result.TotalCount;

            });
        }
       //商品结果类型的bing
        void SalesAreaBatchByProductResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.GetProductSalesAreaBatchList(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => {
                this.SalesAreaBatchByProductResult.ItemsSource = arg.Result.Rows;
                this.SalesAreaBatchByProductResult.TotalCount = arg.Result.TotalCount;

            });
        }
        //选择不同的结果类型查询bing不同的结果
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string StockSysno = "";
            string ProvinceSysno = "";
            var listStock = from s in myProductSalesAreaBatchWarehouse.listStockVM where s.IsChecked == true select s;
            if (listStock.Count() > 0)
            {
                foreach (var item in listStock)
                {
                    if (item.SysNo != null)
                    {
                        StockSysno = item.SysNo + "," + StockSysno;
                    }
                }
                model.StockSysNos = StockSysno.Substring(0, StockSysno.Length - 1); //选中的仓库SysNo
            }
            else
            {
                model.StockSysNos =null;
            }
        
            var listProvince = from p in myProductSalesAreaBatchWarehouse.listProvinceVM where p.IsChecked == true select p;
            if (listProvince.Count() > 0)
            {
                foreach (var item in listProvince)
                {
                    if (item.SysNo != null)
                    {
                        ProvinceSysno = item.SysNo + "," + ProvinceSysno;
                    }
                }
                model.ProvinceSysNos = ProvinceSysno.Substring(0, ProvinceSysno.Length - 1); //选中的省份Sysno
            }
            else
            {
                model.ProvinceSysNos = null;
            }
           
            if (cboResultType.SelectedItem == "商品")
            {
                model.IsSearchProduct = true;
                this.btnInvalid.Visibility = Visibility.Collapsed;
                this.SalesAreaBatchResult.Visibility = Visibility.Collapsed;
                this.SalesAreaBatchByProductResult.Visibility = Visibility.Visible;
                this.SalesAreaBatchByProductResult.Bind();
            }
            if (cboResultType.SelectedItem == "记录")
            {
               
                model.IsSearchProduct = false;
                this.btnInvalid.Visibility = Visibility.Visible;
                this.SalesAreaBatchResult.Visibility = Visibility.Visible;
                this.SalesAreaBatchByProductResult.Visibility = Visibility.Collapsed;
                this.SalesAreaBatchResult.Bind();
            }
            
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.SalesAreaBatchResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<ProductSalesAreaBatchInfo> listInfo = new List<ProductSalesAreaBatchInfo>();
            dynamic viewlist = this.SalesAreaBatchResult.ItemsSource as dynamic;
            if (viewlist != null)
            {
                foreach (var item in viewlist)
                {
                    if (item.IsChecked == true)
                    {
                        listInfo.Add(new ProductSalesAreaBatchInfo()
                        {
                            ProductSysNo = item.ProductSysNo,
                            StockSysNo = item.StockSysNo
                        });
                    }
                }
            }
            if (listInfo.Count > 0)
            {
                facade.RemoveItemSalesAreaListBatch(listInfo, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("作废成功");
                    this.SalesAreaBatchResult.Bind();
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请先选择");
            }
            
        }

        private void linkBtnClose_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = SalesAreaBatchResult.SelectedItem as dynamic;
            string provinceStr = d.Name;
            string[] provinceArr = provinceStr.Split(' ');

          // List<ProductSalesAreaBatchInfo> list = new List<ProductSalesAreaBatchInfo>();
           
            var data = (from p in myProductSalesAreaBatchWarehouse.listProvinceVM
                       join s in provinceArr
                       on p.ProvinceName equals s
                       select p).ToList();


            ProductSetSalesProvince detail = new ProductSetSalesProvince();
            detail.Data = data;
            detail.ProductSysNo = d.ProductSysNo;
            detail.StockSysNo = d.StockSysNo;
            detail.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("设置销售省份", detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.Cancel)
                {
                    this.SalesAreaBatchResult.Bind();
                }
            }, new Size(450,600));
                       
        }
    }
}
