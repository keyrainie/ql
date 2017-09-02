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
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Facades;

namespace ECCentral.Portal.UI.MKT.UserControls
{
   
    public partial class ProductUseCouponLimitMaintain : UserControl
    {
        //每次添加商品或范围时 记录以前的list
        private ObservableCollection<ProductVM> templistProduct;
        private ObservableCollection<ProductVM> listProduct { get; set; }
        public IDialog Dialog { get; set; }
        private ProductUseCouponLimitFacade facade;
        public ProductUseCouponLimitMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                listProduct = new ObservableCollection<ProductVM>();
                templistProduct = new ObservableCollection<ProductVM>();
                facade = new ProductUseCouponLimitFacade();
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(txtProductList.Text))
            {
                foreach (var item in txtProductList.Text.Split('\r'))
                {
                    list.Add(item);
                }
            }
            if (list.Count == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("没有商品", MessageType.Warning);
                return;
            }
            facade.CreateProductUseCouponLimit(list, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
            });
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCanel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void linkSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            listProduct.ToList().ForEach(s => { templistProduct.Add(s); });
            UCProductSearch item = new UCProductSearch();
            bool flag = false;
            item.SelectionMode = SelectionMode.Multiple;
            item.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商品", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    if (listProduct.Count == 0)
                    {
                        listProduct = item._viewModel.SelectedProducts;
                    }
                    else
                    {
                        foreach (var product in item._viewModel.SelectedProducts)
                        {
                            flag = false;
                            foreach (var l in listProduct) //去重复
                            {
                                if (product.SysNo == l.SysNo)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                listProduct.Add(product);
                            }
                        }

                    }
                    ProductBind();

                }

            });
        }

        private void ProductBind()
        {
          
            var  SelectProduct = this.txtProductList.Text;
            var data = listProduct.Except(templistProduct);// 找出新添加的商品ID
            foreach (var item in data)
            {
                if (string.IsNullOrEmpty(SelectProduct))
                {
                    SelectProduct = item.ProductID;
                }
                else
                {
                    SelectProduct = SelectProduct + "\r" + item.ProductID;
                }
            }
             this.txtProductList.Text = SelectProduct ?? "";
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
