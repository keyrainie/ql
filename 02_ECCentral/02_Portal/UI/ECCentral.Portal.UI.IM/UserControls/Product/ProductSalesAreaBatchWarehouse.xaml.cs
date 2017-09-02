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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductSalesAreaBatchWarehouse : UserControl
    {
        public List<ProductSalesAreaBatchStockVM> listStockVM { get; set; }
        public List<ProductSalesAreaBatchProvinceVM> listProvinceVM { get; set; }
      
        /// <summary>
        /// 是否显示省份
        /// </summary>
        private bool isDisPlayProvince=true; //默认显示
        public bool IsDisPlayProvince { private get { return isDisPlayProvince; }
            set { isDisPlayProvince = value; }
        }
        private ProductSalesAreaBatchFacade facade;
        public ProductSalesAreaBatchWarehouse()
        {
            InitializeComponent();
            listStockVM = new List<ProductSalesAreaBatchStockVM>();
            listProvinceVM = new List<ProductSalesAreaBatchProvinceVM>();
            this.Loaded += (sender, e) =>
            {
                facade = new ProductSalesAreaBatchFacade();
                this.cbWebChanne.ItemsSource = CPApplication.Current.CurrentWebChannelList;
                this.cbWebChanne.DisplayMemberPath = "ChannelName";
                this.cbWebChanne.SelectedValuePath = "ChannelID";
                this.cbWebChanne.SelectedIndex = 0;
                if (IsDisPlayProvince == true) //不显示省份
                {
                      spProvince.Visibility = Visibility.Visible;
                }
               
            };
        }

        private void cbWebChanne_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string webChannelID =(string)this.cbWebChanne.SelectedValue;
            facade.GetWarehouseList(CPApplication.Current.CompanyCode, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                foreach (var item in arg.Result)
                {
                    listStockVM.Add(new ProductSalesAreaBatchStockVM() {IsChecked=false,StockID=item.WarehouseID,StockName=item.WarehouseName,StockStatus=item.WarehouseStatus,SysNo=item.SysNo});
                }
                listStockVM.Insert(0, new ProductSalesAreaBatchStockVM() { StockName = "全选",StockID="none",SysNo=-1});
                this.listStock.ItemsSource = listStockVM;
            
            });

            if (IsDisPlayProvince) //显示省份 
            {
                facade.GetAllProvince((objs, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    dynamic d = args.Result.Rows as dynamic;
                    foreach (var item in d)
                    {
                        listProvinceVM.Add(new ProductSalesAreaBatchProvinceVM() { ProvinceName = item.ProvinceName, SysNo = item.ProvinceSysNo, IsChecked = false });
                    }
                    listProvinceVM.Insert(0, new ProductSalesAreaBatchProvinceVM() { ProvinceName = "全选", SysNo = -1 });
                    this.listProvince.ItemsSource = listProvinceVM;
                });
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {

            CheckBox cb = (CheckBox)sender;
            if (cb.Content.ToString() == "全选") //全选
            {
                  foreach (var item in listStockVM)
                    {
                        item.IsChecked = cb.IsChecked == true ? true : false;
                    }
                
            }
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Content.ToString() == "全选") //全选
            {
                foreach (var item in listProvinceVM)
                {
                    item.IsChecked = cb.IsChecked == true ? true : false;
                }

            }
        }      
    }
}
