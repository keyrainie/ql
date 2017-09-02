using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.UserControls.BrandPicker;
using ECCentral.Portal.Basic.Components.UserControls.CategoryPicker;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.UI.MKT.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using System.Windows.Media;

namespace ECCentral.Portal.UI.MKT.UserControls.Coupon
{
    public partial class UCProductRange : UserControl
    {
        bool isLoaded = false;
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCProductRange()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCProductRange_Loaded);
            
        }

        void UCProductRange_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded) return;

            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.ProductRangeType == CouponsProductRangeType.LimitCategoryBrand)
            {
                //expandeVendor.Visibility = Visibility.Visible;
                expanderBrand.Visibility = System.Windows.Visibility.Visible;
                expanderCategory.Visibility = System.Windows.Visibility.Visible;
                expanderProduct.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (vm.ProductRangeType == CouponsProductRangeType.LimitProduct)
            {
                expandeVendor.Visibility = Visibility.Collapsed;
                expanderBrand.Visibility = System.Windows.Visibility.Collapsed;
                expanderCategory.Visibility = System.Windows.Visibility.Collapsed;
                expanderProduct.Visibility = System.Windows.Visibility.Visible;
            }
             
            if (vm.IsOnlyViewMode)
            {
                OperationControlStatusHelper.SetControlsStatus(expanderCategory, true);
                OperationControlStatusHelper.SetControlsStatus(expanderBrand, true);
                OperationControlStatusHelper.SetControlsStatus(expandeVendor, true);
                OperationControlStatusHelper.SetControlsStatus(expanderProduct, true);
            }

            isLoaded = true;
        }

        private void DataGridCheckBoxAllCategory_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked.Value;
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelCategories.CategoryList != null)
            {
                foreach (SimpleObjectViewModel sim in vm.ProductCondition.RelCategories.CategoryList)
                {
                    sim.IsChecked = isChecked;
                }
            }

        }

        private void DataGridCheckBoxAllBrand_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked.Value;
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelBrands.BrandList != null)
            {
                foreach (SimpleObjectViewModel sim in vm.ProductCondition.RelBrands.BrandList)
                {
                    sim.IsChecked = isChecked;
                }
            }
        }

        private void DataGridCheckBoxAllProduct_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked.Value;
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelProducts.ProductList!= null)
            {
                foreach (RelProductAndQtyViewModel sim in vm.ProductCondition.RelProducts.ProductList)
                {
                    sim.IsChecked = isChecked;
                }
            }
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            UCCategoryQuery ucCategory = new UCCategoryQuery();
            ucCategory.DialogHandler = CurrentWindow.ShowDialog("选择分类", ucCategory, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        List<CategoryVM> returnList = args.Data as List<CategoryVM>;
                        CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
                        if (vm.ProductCondition.RelCategories.CategoryList == null)
                        {
                            vm.ProductCondition.RelCategories.CategoryList = new List<SimpleObjectViewModel>();
                        }
                        if (returnList.Count > 0)
                        {
                            foreach (CategoryVM cate in returnList)
                            {
                                if (vm.ProductCondition.RelCategories.CategoryList.FirstOrDefault(f => f.SysNo == cate.SysNo) == null)
                                {
                                    vm.ProductCondition.RelCategories.CategoryList.Add(new SimpleObjectViewModel()
                                    {
                                        SysNo = cate.SysNo,
                                        Name = cate.CategoryDisplayName,
                                        Relation = vm.ProductCondition.RelCategories.IsIncludeRelation.Value ? PSRelationType.Include : PSRelationType.Exclude,
                                        IsChecked = false
                                    });
                                }
                            }
                            dgCategory.ItemsSource = vm.ProductCondition.RelCategories.CategoryList;                            
                        }
                    }
                }
                , new Size(700, 600));
        }
        
        private void btnAddBrand_Click(object sender, RoutedEventArgs e)
        {
        UCBrandQuery selectDialog = new UCBrandQuery();
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("查询品牌", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    DynamicXml getSelectedBrand = args.Data as DynamicXml;
                    if (null != getSelectedBrand)
                    {
                        //验证品牌是否有效
                        var status = getSelectedBrand["Status"].ToString();
                        if (status != "Active")
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert("只能添加有效的品牌！");
                            return;
                        }
                        CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
                        if (vm.ProductCondition == null) vm.ProductCondition = new PSProductConditionViewModel();
                        if (vm.ProductCondition.RelBrands == null) vm.ProductCondition.RelBrands = new RelBrandViewModel();
                        if (vm.ProductCondition.RelBrands.BrandList == null) vm.ProductCondition.RelBrands.BrandList = new List<SimpleObjectViewModel>();

                        if (vm.ProductCondition.RelBrands.BrandList.FirstOrDefault(f => f.SysNo.ToString() == getSelectedBrand["SysNo"].ToString()) == null)
                        {
                            vm.ProductCondition.RelBrands.BrandList.Add(new SimpleObjectViewModel()
                            {
                                SysNo = int.Parse(getSelectedBrand["SysNo"].ToString()),
                                Name = getSelectedBrand["BrandName_Ch"].ToString(),
                                Relation = vm.ProductCondition.RelBrands.IsIncludeRelation.Value ? PSRelationType.Include : PSRelationType.Exclude,
                                IsChecked = false
                            });
                        }

                        dgBrand.ItemsSource = vm.ProductCondition.RelBrands.BrandList;                 
                    }
                }
            });
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch ucPicker = new UCProductSearch ();
             ucPicker.SelectionMode = SelectionMode.Multiple;
            ucPicker.DialogHandler = CurrentWindow.ShowDialog("选择商品", ucPicker, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    if (args.Data == null) return;
                    CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;                    

                    if (vm.ProductCondition == null) vm.ProductCondition = new PSProductConditionViewModel();
                    if (vm.ProductCondition.RelProducts == null) vm.ProductCondition.RelProducts = new RelProductViewModel();
                    if (vm.ProductCondition.RelProducts.ProductList == null)
                        vm.ProductCondition.RelProducts.ProductList = new ObservableCollection<RelProductAndQtyViewModel>();
                    List<ProductVM> selectedList = args.Data as List<ProductVM>;
                    foreach (ProductVM product in selectedList)
                    {
                        var pr = vm.ProductCondition.RelProducts.ProductList.FirstOrDefault(p => p.ProductSysNo == product.SysNo);
                        if (pr != null)
                        {
                            continue;
                        }
                        //if (product.Status != BizEntity.IM.ProductStatus.Active)
                        //{
                        //    CurrentWindow.Alert(string.Format("商品{0}必须为上架商品!",product.ProductID));
                        //    continue;
                        //}

                        RelProductAndQtyViewModel item=new RelProductAndQtyViewModel();
                        item.ProductSysNo = product.SysNo;
                        item.ProductID = product.ProductID;
                        item.ProductName =  product.ProductName;
                        ////获取商品的毛利率
                        //new CouponsFacade(CPApplication.Current.CurrentPage).GetCouponGrossMarginRate(item.ProductSysNo.Value, (s, a) =>
                        //{
                        //    item.GrossMarginRate = a.Result;
                        //});
                                         
                        item.IsChecked = false;
                        vm.ProductCondition.RelProducts.ProductList.Add(item);
                    }
                    dgProduct.ItemsSource = vm.ProductCondition.RelProducts.ProductList;                   
                }
            });
        }

        private void btnRemoveCategory_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelCategories.CategoryList != null)
            {
                List<SimpleObjectViewModel> removedList = new List<SimpleObjectViewModel>();
                foreach (SimpleObjectViewModel sim in vm.ProductCondition.RelCategories.CategoryList)
                {
                    if (sim.IsChecked.Value)
                    {
                        removedList.Add(sim);
                    }
                }
                if (removedList.Count == 0)
                {
                    this.CurrentWindow.Alert("请先至少选中一条记录!");
                }
                else
                {
                    removedList.ForEach(f => vm.ProductCondition.RelCategories.CategoryList.Remove(f));
                    dgCategory.ItemsSource = vm.ProductCondition.RelCategories.CategoryList;                    
                }
            }           
        }

        private void btnRemoveBrand_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelBrands.BrandList != null)
            {
                List<SimpleObjectViewModel> removedList = new List<SimpleObjectViewModel>();
                foreach (SimpleObjectViewModel sim in vm.ProductCondition.RelBrands.BrandList)
                {
                    if (sim.IsChecked.Value)
                    {
                        removedList.Add(sim);
                    }
                }
                if (removedList.Count == 0)
                {
                    this.CurrentWindow.Alert("请先至少选中一条记录!");
                }
                else
                {
                    removedList.ForEach(f => vm.ProductCondition.RelBrands.BrandList.Remove(f));
                    dgBrand.ItemsSource = vm.ProductCondition.RelBrands.BrandList;                    
                }
            }
        }

        private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelProducts.ProductList != null)
            {
                List<RelProductAndQtyViewModel> removedList = new List<RelProductAndQtyViewModel>();
                foreach (RelProductAndQtyViewModel sim in vm.ProductCondition.RelProducts.ProductList)
                {
                    if (sim.IsChecked.Value)
                    {
                        removedList.Add(sim);
                    }
                }
                if (removedList.Count == 0)
                {
                    this.CurrentWindow.Alert("请先至少选中一条记录!");
                }
                else
                {
                    removedList.ForEach(f => vm.ProductCondition.RelProducts.ProductList.Remove(f));
                    this.dgProduct.ItemsSource = vm.ProductCondition.RelProducts.ProductList;                    
                }               
            }
        }

        private void rdoCategoryInclude_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelCategories.CategoryList != null
                && vm.ProductCondition.RelCategories.CategoryList.Count > 0
                && vm.ProductCondition.RelCategories.CategoryList[0].Relation.Value==PSRelationType.Exclude)
            {
                CurrentWindow.Confirm("请确认是否切换？如果是\"是\"，将删除所有已添加的类别！", (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            vm.ProductCondition.RelCategories.CategoryList.Clear();
                            dgCategory.ItemsSource = vm.ProductCondition.RelCategories.CategoryList;                            
                        }
                        else
                        {
                            vm.ProductCondition.RelCategories.IsExcludeRelation = true;
                            vm.ProductCondition.RelCategories.IsIncludeRelation = false;
                        }
                    });
            }
        }

        private void rdoCategoryExclude_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelCategories.CategoryList != null
                && vm.ProductCondition.RelCategories.CategoryList.Count > 0
                && vm.ProductCondition.RelCategories.CategoryList[0].Relation.Value == PSRelationType.Include)
            {
                CurrentWindow.Confirm("请确认是否切换？如果是\"是\"，将删除所有已添加的类别！", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        vm.ProductCondition.RelCategories.CategoryList.Clear();
                        dgCategory.ItemsSource = vm.ProductCondition.RelCategories.CategoryList;                        
                    }
                    else
                    {
                        vm.ProductCondition.RelCategories.IsIncludeRelation = true;
                        vm.ProductCondition.RelCategories.IsExcludeRelation = false;
                    }                                          
                });
            }
        }

        private void rdoBrandInclude_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelBrands.BrandList != null
                && vm.ProductCondition.RelBrands.BrandList.Count > 0
                && vm.ProductCondition.RelBrands.BrandList[0].Relation.Value == PSRelationType.Exclude)
            {
                CurrentWindow.Confirm("请确认是否切换？如果是\"是\"，将删除所有已添加的品牌！", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        vm.ProductCondition.RelBrands.BrandList.Clear();
                        dgBrand.ItemsSource = vm.ProductCondition.RelBrands.BrandList;                        
                    }
                    else
                    {
                        vm.ProductCondition.RelBrands.IsExcludeRelation = true;
                        vm.ProductCondition.RelBrands.IsIncludeRelation = false;
                    }
                });
            }
        }

        private void rdoBrandExclude_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.RelBrands.BrandList != null
                && vm.ProductCondition.RelBrands.BrandList.Count > 0
                && vm.ProductCondition.RelBrands.BrandList[0].Relation.Value==PSRelationType.Include)
            {
                CurrentWindow.Confirm("请确认是否切换？如果是\"是\"，将删除所有已添加的品牌！", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        vm.ProductCondition.RelBrands.BrandList.Clear();
                        dgBrand.ItemsSource = vm.ProductCondition.RelBrands.BrandList;                        
                    }
                    else
                    {
                        vm.ProductCondition.RelBrands.IsIncludeRelation = true;
                        vm.ProductCondition.RelBrands.IsExcludeRelation = false;
                    }
                    
                });
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddVendor_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            UCVendorQuery item = new UCVendorQuery();
            item.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商家", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dynamic data = args.Data as dynamic;

                    if (vm.ProductCondition.ListRelVendorViewModel == null)
                    {
                        vm.ProductCondition.ListRelVendorViewModel = new List<RelVendorViewModel>();
                    }
                    bool flag = true;
                    foreach (var vendor in vm.ProductCondition.ListRelVendorViewModel)
                    {
                        if (vendor.VendorSysNo == data.SysNo)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        vm.ProductCondition.ListRelVendorViewModel.Add(new RelVendorViewModel() { VendorSysNo = data.SysNo, VendorName = data.VendorName, CouponsStatus = vm.Status });
                        this.dgVendor.ItemsSource = vm.ProductCondition.ListRelVendorViewModel;
                    }
                    else
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(string.Format("已存在{0}", data.VendorName));
                    }
                 }
            });
        }
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteVendor_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            
            if (vm.ProductCondition.ListRelVendorViewModel != null)
            {
                var selectdata=(from p in vm.ProductCondition.ListRelVendorViewModel where p.IsChecked select p).ToList();
                if (selectdata.Count == 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("没有选择记录!");
                    return;
                }
                var tempdata=(from p in vm.ProductCondition.ListRelVendorViewModel where p.IsChecked==false select p).ToList();
                vm.ProductCondition.ListRelVendorViewModel = tempdata;
                this.dgVendor.ItemsSource = vm.ProductCondition.ListRelVendorViewModel;
                
            }
        }
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCheckBoxAllVandor_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked.Value;
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            if (vm.ProductCondition.ListRelVendorViewModel != null)
            {
                foreach (RelVendorViewModel sim in vm.ProductCondition.ListRelVendorViewModel)
                {
                    sim.IsChecked = isChecked;
                }
            }
        }
        /// <summary>
        /// 快捷加泰隆优选商家
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewegg_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            var data = (from p in vm.ProductCondition.ListRelVendorViewModel where p.VendorSysNo == 1 select p).ToList();
            if (data.Count > 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("已存在泰隆优选商家!");
            }
            else
            {
                vm.ProductCondition.ListRelVendorViewModel.Add(new RelVendorViewModel() { VendorSysNo = 1, VendorName = "泰隆优选", CouponsStatus = vm.Status });
                this.dgVendor.ItemsSource = vm.ProductCondition.ListRelVendorViewModel;
            }
            
        }        
    }
}
