using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Service.Utility;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Text;

namespace ECCentral.Portal.UI.MKT.UserControls.SaleGift
{
    public partial class UCSaleGiftLimitScope : UserControl
    {
        bool isLoaded = false;


        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCSaleGiftLimitScope()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCSaleGiftLimitScope_Loaded);
        }

        void UCSaleGiftLimitScope_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }

            List<KeyValuePair<PSRelationType?, string>> statusList = EnumConverter.GetKeyValuePairs<PSRelationType>();
            cmbRelation.ItemsSource = statusList;
            cmbRelation.SelectedIndex = 0;

            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            dgBrandC3.ItemsSource = vm.BrandC3ScopeList;
            dgBrandC3.Bind();
            dgProduct.ItemsSource = vm.ProductScopeList;
            dgProduct.Bind();
            if (vm.IsGlobalProduct)
            {
                cmbRelation.SelectedValue = PSRelationType.Exclude;
                cmbRelation.IsEnabled = false;
                rdoProductExclude.IsEnabled = false;
                rdoProductExclude.IsChecked = true;
                rdoProductInclude.IsEnabled = false;
                rdoProductInclude.IsChecked = false;
            }
            else
            {
                cmbRelation.IsEnabled = true;
                rdoProductExclude.IsEnabled = true;
                rdoProductExclude.IsChecked = false;
                rdoProductInclude.IsEnabled = true;
                rdoProductInclude.IsChecked = true;
            }

            isLoaded = true;
        }

        private void chkDGProductCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            CheckBox chk = (CheckBox)sender;
            vm.ProductScopeList.ForEach(f => f.IsChecked = chk.IsChecked.Value);
            dgProduct.ItemsSource = vm.ProductScopeList;
            dgProduct.Bind();
        }

        private void chkDGBrandC3CheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            CheckBox chk = (CheckBox)sender;
            vm.BrandC3ScopeList.ForEach(f => f.IsChecked = chk.IsChecked.Value);
            dgBrandC3.ItemsSource = vm.BrandC3ScopeList;
            dgBrandC3.Bind();
        }

        private void btnAddBrandC3_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ucBrand.SelectedBrandSysNo) && !ucCategory.ChooseCategory3SysNo.HasValue)
            {
                CurrentWindow.Alert("品牌和分类至少选择一项且分类至少是三级分类!");
                return;
            }

            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;

            SaleGift_RuleSettingViewModel settingVM = new SaleGift_RuleSettingViewModel();
            if (vm.IsGlobalProduct)
            {
                settingVM.ComboType = AndOrType.Not;
            }
            else
            {   
                settingVM.ComboType = ((PSRelationType)cmbRelation.SelectedValue) == PSRelationType.Include ? AndOrType.Or : AndOrType.Not;
            }

            if (!string.IsNullOrEmpty(ucBrand.SelectedBrandSysNo))
            {
                settingVM.RelBrand.SysNo = int.Parse(ucBrand.SelectedBrandSysNo);
                settingVM.RelBrand.Name = ucBrand.SelectedBrandName;
            }
            if (ucCategory.ChooseCategory3SysNo.HasValue)
            {
                settingVM.RelC3.SysNo = ucCategory.ChooseCategory3SysNo;
                settingVM.RelC3.Name = ucCategory.Category3Name;
            }

            if (!string.IsNullOrEmpty(ucBrand.SelectedBrandSysNo) && ucCategory.ChooseCategory3SysNo.HasValue)
            {
                if (vm.BrandC3ScopeList.FirstOrDefault(f => f.RelBrand.SysNo == settingVM.RelBrand.SysNo && f.RelC3.SysNo == settingVM.RelC3.SysNo) != null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("相同的数据已经存在!");
                    return;
                }

                settingVM.Type = SaleGiftSaleRuleType.BrandC3Combo;
            }
            else
            {
                if (!string.IsNullOrEmpty(ucBrand.SelectedBrandSysNo))
                {
                    if (vm.BrandC3ScopeList.FirstOrDefault(f => f.RelBrand.SysNo == settingVM.RelBrand.SysNo && f.RelC3.SysNo == null) != null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("相同的数据已经存在!");
                        return;
                    }
                    settingVM.Type = SaleGiftSaleRuleType.Brand;
                }
                else if (ucCategory.ChooseCategory3SysNo.HasValue)
                {
                    if (vm.BrandC3ScopeList.FirstOrDefault(f => f.RelC3.SysNo == settingVM.RelC3.SysNo && f.RelBrand.SysNo == null) != null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("相同的数据已经存在!");
                        return;
                    }
                    settingVM.Type = SaleGiftSaleRuleType.C3;
                }
            }
            settingVM.IsChecked = false;



            #region
            //非整网规则，不能单独添加排斥类
            string errorStr = ValidateDataForAdd(settingVM);
            if (!string.IsNullOrEmpty(errorStr))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(errorStr);
                return;
            }
            #endregion
            

            vm.BrandC3ScopeList.Add(settingVM);

            dgBrandC3.ItemsSource = vm.BrandC3ScopeList;
            dgBrandC3.Bind();

        }

        private void btnRemoveBrandC3_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            List<SaleGift_RuleSettingViewModel> needRemoveList = new List<SaleGift_RuleSettingViewModel>();
            if (vm.BrandC3ScopeList.Count > 0)
            {
                foreach (SaleGift_RuleSettingViewModel setting in vm.BrandC3ScopeList)
                {
                    if (setting.IsChecked)
                    {
                        needRemoveList.Add(setting);
                    }
                }

                #region
                //非整网规则，不能单独添加排斥类
                string errorStr = ValidateDataForRemove();
                if (!string.IsNullOrEmpty(errorStr))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(errorStr);
                    return;
                }
                #endregion

                needRemoveList.ForEach(f => vm.BrandC3ScopeList.Remove(f));
                dgBrandC3.ItemsSource = vm.BrandC3ScopeList;
                dgBrandC3.Bind();
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch ucPicker = new UCProductSearch();
            ucPicker.SelectionMode = SelectionMode.Multiple;
            ucPicker.DialogHandler = CurrentWindow.ShowDialog("选择商品", ucPicker, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
                    List<ProductVM> selectedList = args.Data as List<ProductVM>;
                    StringBuilder message = new StringBuilder();

                    foreach (ProductVM product in selectedList)
                    {
                        //if (product.Status.Value != BizEntity.IM.ProductStatus.Active)
                        //{
                        //    message.AppendLine(string.Format("商品{0}必须为上架商品！", product.ProductID));
                        //    continue;
                        //}

                        SaleGift_RuleSettingViewModel settingVM = new SaleGift_RuleSettingViewModel();
                        settingVM.IsChecked = false;

                        settingVM.RelProduct.ProductSysNo = product.SysNo;
                        settingVM.RelProduct.ProductID = product.ProductID;
                        settingVM.RelProduct.ProductName = product.ProductName;
                        settingVM.RelProduct.AvailableQty = product.AvailableQty;
                        settingVM.RelProduct.ConsignQty = product.ConsignQty;
                        settingVM.RelProduct.UnitCost = product.UnitCost;
                        settingVM.RelProduct.CurrentPrice = product.CurrentPrice;

                        if (vm.ProductScopeList.FirstOrDefault(f => f.RelProduct.ProductSysNo == settingVM.RelProduct.ProductSysNo) != null)
                        {
                             message.AppendLine(string.Format("商品{0}已经存在!", product.ProductID));
                            continue;
                        }

                        //获取商品的毛利
                        //new CouponsFacade(CPApplication.Current.CurrentPage).GetCouponGrossMarginRate(product.SysNo.Value, (s, a) =>
                        //{
                        //    settingVM.RelProduct.GrossMarginRate = a.Result;
                        //});

                        settingVM.Type = SaleGiftSaleRuleType.Item;
                        if (vm.IsGlobalProduct)
                        {
                            settingVM.ComboType = AndOrType.Not;
                        }
                        else
                        {
                            if (rdoProductInclude.IsChecked.HasValue && rdoProductInclude.IsChecked.Value)
                            {
                                settingVM.ComboType = AndOrType.Or;
                            }
                            else
                            {
                                settingVM.ComboType = AndOrType.Not;
                            }
                        }

                        #region
                        //非整网规则，不能单独添加排斥类
                        string errorStr = ValidateDataForAdd(settingVM);
                        if (!string.IsNullOrEmpty(errorStr))
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert(errorStr);
                            return;
                        }
                        #endregion

                        vm.ProductScopeList.Add(settingVM);
                    }

                    if (message.Length > 0)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(message.ToString(), MessageType.Warning);
                    }

                    dgProduct.ItemsSource = vm.ProductScopeList;
                }
            });
        }

        private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            List<SaleGift_RuleSettingViewModel> needRemoveList = new List<SaleGift_RuleSettingViewModel>();
            if (vm.ProductScopeList.Count > 0)
            {
                foreach (SaleGift_RuleSettingViewModel setting in vm.ProductScopeList)
                {
                    if (setting.IsChecked)
                    {
                        needRemoveList.Add(setting);
                    }
                }

                #region
                //非整网规则，不能单独添加排斥类
                string errorStr = ValidateDataForRemove();
                if (!string.IsNullOrEmpty(errorStr))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(errorStr);
                    return;
                }
                #endregion

                needRemoveList.ForEach(f => vm.ProductScopeList.Remove(f));
                dgProduct.ItemsSource = vm.ProductScopeList;
                dgProduct.Bind();
            }
        }

        private void chkIsGlobalProduct_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            if ((vm.ProductScopeList != null && vm.ProductScopeList.Count > 0)
            || (vm.BrandC3ScopeList != null && vm.BrandC3ScopeList.Count > 0))
            {

                CurrentWindow.Confirm("当前已有设置的数据，改变\"整网商品\"的选择后，现有设置将清除！请确认是否要清除？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        vm.ProductScopeList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
                        vm.BrandC3ScopeList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
                        dgBrandC3.ItemsSource = vm.BrandC3ScopeList;
                        dgBrandC3.Bind();
                        dgProduct.ItemsSource = vm.ProductScopeList;
                        dgProduct.Bind();
                    }
                    else
                    {
                        if (chkIsGlobalProduct.IsChecked.Value)
                        {
                            chkIsGlobalProduct.IsChecked = false;
                        }
                        else
                        {
                            chkIsGlobalProduct.IsChecked = true;
                        }

                    }
                });
            }

            if (chkIsGlobalProduct.IsChecked.Value)
            {
                cmbRelation.SelectedValue = PSRelationType.Exclude;
                cmbRelation.IsEnabled = false;
                rdoProductExclude.IsEnabled = false;
                rdoProductExclude.IsChecked = true;
                rdoProductInclude.IsEnabled = false;
                rdoProductInclude.IsChecked = false;



            }
            else
            {
                cmbRelation.SelectedValue = PSRelationType.Include;
                cmbRelation.IsEnabled = true;
                rdoProductExclude.IsEnabled = true;
                rdoProductExclude.IsChecked = false;
                rdoProductInclude.IsEnabled = true;
                rdoProductInclude.IsChecked = true;
            }
        }

        private string ValidateDataForAdd(SaleGift_RuleSettingViewModel addData)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            bool isGlobalProduct = vm.IsGlobalProduct;


            ObservableCollection<SaleGift_RuleSettingViewModel> dataList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            vm.BrandC3ScopeList.ForEach(x => dataList.Add(x));
            vm.ProductScopeList.ForEach(x => dataList.Add(x));

            string errorStr = string.Empty;
            //非整网规则，不能单独添加排斥类
            ObservableCollection<SaleGift_RuleSettingViewModel> tempData = dataList.DeepCopy();
            tempData.Add(addData);

            if (isGlobalProduct == false && tempData.Count > 0)
            {
                bool isPass = false;
                tempData.ForEach(x =>
                {
                    if (x.ComboType != AndOrType.Not) { isPass = true; };
                });
                if (isPass == false)
                {
                    errorStr = "未勾选“整网商品”时，请确保列表中存在“包含”类型的销售规则!";
                }
            }

            return errorStr;
        }

        private string ValidateDataForRemove()
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            bool isGlobalProduct = vm.IsGlobalProduct;


            ObservableCollection<SaleGift_RuleSettingViewModel> dataList = new ObservableCollection<SaleGift_RuleSettingViewModel>();
            vm.BrandC3ScopeList.ForEach(x => dataList.Add(x));
            vm.ProductScopeList.ForEach(x => dataList.Add(x));

            string errorStr = string.Empty;
            ObservableCollection<SaleGift_RuleSettingViewModel> tempData = dataList.DeepCopy();
            List<SaleGift_RuleSettingViewModel> tempDataRemove = new List<SaleGift_RuleSettingViewModel>();
            foreach (SaleGift_RuleSettingViewModel item in tempData)
            {
                if (item.IsChecked) { tempDataRemove.Add(item); }
            }
            tempDataRemove.ForEach(x => tempData.Remove(x));

            if (isGlobalProduct == false && tempData.Count > 0)
            {
                bool isPass = false;
                tempData.ForEach(x =>
                {
                    if (x.ComboType != AndOrType.Not) { isPass = true; };
                });
                if (isPass == false)
                {
                    errorStr = "未勾选“整网商品”时，请确保列表中存在“包含”类型的销售规则!";
                }
            }

            return errorStr;
        }
    }
}
