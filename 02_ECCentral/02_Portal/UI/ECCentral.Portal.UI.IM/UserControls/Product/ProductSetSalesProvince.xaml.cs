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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductSetSalesProvince : UserControl
    {
        public IDialog Dialog { get; set; }
        public int ProductSysNo { private get; set; } //商品SysNo
        public int StockSysNo { private get; set; } //仓库SysNo
        public List<ProductSalesAreaBatchProvinceVM> Data { private get; set; } //数据源

        private ProductSalesAreaBatchFacade facade;
        public ProductSetSalesProvince()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                this.ProductSetSalesProvinceResult.ItemsSource = Data;
                facade = new ProductSalesAreaBatchFacade();
            };
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ProductSalesAreaBatchProvinceVM vm = this.ProductSetSalesProvinceResult.SelectedItem as ProductSalesAreaBatchProvinceVM;
            ProductSalesAreaBatchInfo info = new ProductSalesAreaBatchInfo() 
            {
                ProductSysNo = ProductSysNo,
                ProvinceSysNo=vm.SysNo,
                StockSysNo=StockSysNo
            };
            facade.RemoveProvince(info, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                var data = (from p in Data where p.SysNo != vm.SysNo select p).ToList();
                Data = data;
                this.ProductSetSalesProvinceResult.ItemsSource = Data;
            });
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }
}
