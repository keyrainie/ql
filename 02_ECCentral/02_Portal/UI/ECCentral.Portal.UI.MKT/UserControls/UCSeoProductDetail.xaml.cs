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
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.UserControls.CategoryPicker;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCSeoProductDetail : UserControl
    {
       

        //每次添加商品或范围时 记录以前的list
        private ObservableCollection<ProductVM> templistProduct;
        private List<CategoryVM> templistCategory;
        private string SelectProduct;
        private string SelectCategory;
   
        //数据源
        public ObservableCollection<ProductVM> listProduct {  get; set; }
        public List<CategoryVM> listCategory {  get; set; }

        //公开属性
        public string ProductResult { get { return productResult.Text; } }
        public string CategoryResult { get { return categoryResult.Text; } }
        public UCSeoProductDetail()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                if (listCategory == null)
                {
                    listCategory = new List<CategoryVM>();
                }
                if (listProduct == null)
                {
                    listProduct = new ObservableCollection<ProductVM>();
                }
                templistCategory = new List<CategoryVM>();
                templistProduct = new ObservableCollection<ProductVM>();

            };
        }

        public void Bind()
        {
            ProductBind();
            CategoryBind();
         }
        private void hyproduct_Click(object sender, RoutedEventArgs e)
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

        private void hycategory_Click(object sender, RoutedEventArgs e)
        {
            listCategory.ForEach(s => { templistCategory.Add(s); });
            UCCategoryQuery item = new UCCategoryQuery();
            bool flag = false;
            item.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择类别", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    var tempData = (List<CategoryVM>)args.Data;
                    if (listCategory.Count == 0)
                    {
                        listCategory = tempData;
                    }
                    else
                    {
                        foreach (var data in tempData)
                        {
                            flag = false;
                            foreach (var category in listCategory) //去重复
                            {
                                if (category.SysNo == data.SysNo)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                listCategory.Add(data);
                            }
                        }
                    }
                    CategoryBind();
                }

            });
        }

      

        private void ProductBind()
        {
            //bool flag = true;
            //foreach (var source in templistProduct)
            //{
            //    if (flag)
            //    {
            //        flag = false;
            //        SelectProduct = source.ProductID;
            //    }
            //    else
            //    {

            //        SelectProduct = SelectProduct + "\r\n" + source.ProductID;
            //    }
            //}
            SelectProduct = this.productResult.Text;
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

            this.productResult.Text = SelectProduct ?? "";
        }

        private void CategoryBind()
        {
        //    bool flag = true;
        //    foreach (var c in listCategory)
        //    {
        //        if (flag)
        //        {
        //            flag = false;
        //            SelectCategory = c.CategoryDisplayName;
        //        }
        //        else
        //        {
                  
        //        }
        //    }
            SelectCategory = this.categoryResult.Text;
            var data = listCategory.Except(templistCategory); //找出新添加的类别
            foreach (var item in data)
            {
                if (string.IsNullOrEmpty(SelectCategory))
                {
                    SelectCategory = item.CategoryDisplayName;
                }
                else
                {
                    SelectCategory = SelectCategory + "\r" + item.CategoryDisplayName;
                }
            }
            this.categoryResult.Text = SelectCategory??"";
        }
    }
}
