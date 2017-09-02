using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.Basic.Components.UserControls.CategoryPicker
{
    public partial class UCECCCategoryPicker : UserControl
    {
        /// <summary>
        /// 用于加载完数据，设置默认值
        /// </summary>
        /// <param name="sender">control</param>
        public delegate void SetDefaultValue(params object[] senders);
        public SetDefaultValue SetDefaultValueHandler;

        private bool enableThirdCategory = true;
        #region [依赖属性]
        public int? Category1SysNo
        {
            get { return (int?)GetValue(Category1SysNoProperty); }
            set { SetValue(Category1SysNoProperty, value); }
        }

        public int? Category2SysNo
        {
            get { return (int?)GetValue(Category2SysNoProperty); }
            set { SetValue(Category2SysNoProperty, value); }
        }

        public int? Category3SysNo
        {
            get { return (int?)GetValue(Category3SysNoProperty); }
            set { SetValue(Category3SysNoProperty, value); }
        }

        public string Category1Name
        {
            get
            {
                return (string)GetValue(Category1NameProperty);

            }
            set { SetValue(Category1NameProperty, value); }

        }

        public string Category2Name
        {
            get
            {
                return (string)GetValue(Category2NameProperty);
            }
            set { SetValue(Category2NameProperty, value); }

        }

        public string Category3Name
        {
            get
            {
                return (string)GetValue(Category3NameProperty);
            }
            set { SetValue(Category3NameProperty, value); }
        }

        public bool IsAllowCategorySelect
        {
            get { return (bool)GetValue(IsAllowCategorySelectProperty); }
            set { SetValue(IsAllowCategorySelectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowCategorySelect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowCategorySelectProperty =
            DependencyProperty.Register("IsAllowCategorySelect", typeof(bool), typeof(UCECCCategoryPicker), new PropertyMetadata(true, (s, e) =>
            {
                var uc = s as UCECCCategoryPicker;
                if ((bool)e.NewValue == false)
                {
                    uc.cmbCategory1.IsEnabled = false;
                    uc.cmbCategory2.IsEnabled = false;
                    uc.cmbCategory3.IsEnabled = false;
                }
            }));



        //// Using a DependencyProperty as the backing store for Category3SysNo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Category3SysNoProperty =
            DependencyProperty.Register("Category3SysNo", typeof(int?), typeof(UCECCCategoryPicker), new PropertyMetadata(null, (s, e) =>
            {
                var uc = s as UCECCCategoryPicker;
                if (e.NewValue != null)
                {
                    if (null != uc.categoryFacade && e.NewValue != null)
                    {
                        uc.BindC3SysNo();
                    }
                }
            }));

        // Using a DependencyProperty as the backing store for Category2SysNo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Category2SysNoProperty =
            DependencyProperty.Register("Category2SysNo", typeof(int?), typeof(UCECCCategoryPicker), new PropertyMetadata(null, (s, e) =>
            {
                var uc = s as UCECCCategoryPicker;
                if (e.NewValue != null)
                {
                    if (null != uc.categoryFacade && !uc.Category3SysNo.HasValue)
                    {
                        uc.BindC2SysNo();
                    }
                }

            }));

        // Using a DependencyProperty as the backing store for Category1SysNo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Category1SysNoProperty =
            DependencyProperty.Register("Category1SysNo", typeof(int?), typeof(UCECCCategoryPicker), new PropertyMetadata(null));


        public static readonly DependencyProperty Category1NameProperty =
         DependencyProperty.Register("Category1Name", typeof(string), typeof(UCECCCategoryPicker), new PropertyMetadata(null));

        public static readonly DependencyProperty Category2NameProperty =
         DependencyProperty.Register("Category2Name", typeof(string), typeof(UCECCCategoryPicker), new PropertyMetadata(null));

        public static readonly DependencyProperty Category3NameProperty =
         DependencyProperty.Register("Category3Name", typeof(string), typeof(UCECCCategoryPicker), new PropertyMetadata(null));
        #endregion

        #region 将每个类别的Visibility公开 因为有些地方不需要显示第二级和第三极类别

        public Visibility Category1Visibility
        {
            get { return this.cmbCategory1.Visibility; }
            set { this.cmbCategory1.Visibility = value; }
        }
        public Visibility Category2Visibility
        {
            get { return this.cmbCategory2.Visibility; }
            set { this.cmbCategory2.Visibility = value; }
        }
        public Visibility Category3Visibility
        {
            get { return this.cmbCategory3.Visibility; }
            set { this.cmbCategory3.Visibility = value; }
        }
        #endregion


        #region 将每个类别的无效状态是否加载 公开 因为有些地方不需要加载无效状态的类别

        public bool _Category1LoadDeActive = true;
        public bool Category1LoadDeActive
        {
            get { return _Category1LoadDeActive; }
            set { _Category1LoadDeActive = value; }
        }

        public bool _Category2LoadDeActive = true;
        public bool Category2LoadDeActive
        {
            get { return _Category2LoadDeActive; }
            set { _Category2LoadDeActive = value; }
        }

        public bool _Category3LoadDeActive = true;
        public bool Category3LoadDeActive
        {
            get { return _Category3LoadDeActive; }
            set { _Category3LoadDeActive = value; }
        }
        #endregion


        /// <summary>
        /// 是否允许三级分类
        /// </summary>
        public bool EnableThirdCategory
        {
            get
            {
                return enableThirdCategory;
            }
            set
            {
                enableThirdCategory = value;
            }
        }

        public int? ChooseCategory1SysNo
        {
            get
            {
                CategoryInfo categoryInfo = this.cmbCategory1.SelectedItem as CategoryInfo;
                if (null != categoryInfo)
                {
                    return categoryInfo.SysNo;
                }
                else
                {
                    return null;
                }
            }
        }
        public int? ChooseCategory2SysNo
        {
            get
            {
                CategoryInfo categoryInfo = this.cmbCategory2.SelectedItem as CategoryInfo;
                if (null != categoryInfo)
                {
                    return categoryInfo.SysNo;
                }
                else
                {
                    return null;
                }
            }
        }
        public int? ChooseCategory3SysNo
        {
            get
            {
                CategoryInfo categoryInfo = this.cmbCategory3.SelectedItem as CategoryInfo;
                if (null != categoryInfo)
                {
                    return categoryInfo.SysNo;
                }
                else
                {
                    return null;
                }
            }
        }

        private ECCCategoryFacade categoryFacade;
        public CategoryQueryFilter queryFilter;

        public List<CategoryInfo> Category1List;
        public List<CategoryInfo> Category2List;
        public List<CategoryInfo> Category3List;

        public event EventHandler<EventArgs> LoadCategoryCompleted;

        public event EventHandler<EventArgs> cmbCategory3SelectionChanged;
        public UCECCCategoryPicker()
        {
            InitializeComponent();
            queryFilter = new CategoryQueryFilter()
            {
                CompanyCode = CPApplication.Current.CompanyCode
            };
            Category1List = new List<CategoryInfo>();
            Category2List = new List<CategoryInfo>();
            Category3List = new List<CategoryInfo>();

            this.Loaded += new RoutedEventHandler(UCECCCategoryPicker_Loaded);
        }

        void UCECCCategoryPicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCECCCategoryPicker_Loaded);
            categoryFacade = new  ECCCategoryFacade(CPApplication.Current.CurrentPage);
            BindingControl();
            InitializeCategoryComboBox();
        }
        public void BindingControl()
        {
            #region [Binding Control]
            var exp = this.GetBindingExpression(UCECCCategoryPicker.Category1SysNoProperty);
            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbCategory1.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            var exp2 = this.GetBindingExpression(UCECCCategoryPicker.Category2SysNoProperty);
            if (exp2 != null && exp2.ParentBinding != null)
            {
                string path = exp2.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbCategory2.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }
            var exp3 = this.GetBindingExpression(UCECCCategoryPicker.Category3SysNoProperty);
            if (exp3 != null && exp3.ParentBinding != null)
            {
                string path = exp3.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbCategory3.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }
            #endregion
        }

        public void BindC3SysNo()
        {
            int? getC3SysNo = Category3SysNo;
            int? getC2SysNo = null;
            int? getC1SysNo = null;
            Category1SysNo = null;
            Category2SysNo = null;
            Category3SysNo = getC3SysNo;
            CategoryInfo c3Info = Category3List.SingleOrDefault(i => i.SysNo == getC3SysNo.Value);
            getC2SysNo = c3Info.ParentSysNumber.Value;
            CategoryInfo c2Info = Category2List.SingleOrDefault(i => i.SysNo == getC2SysNo);
            getC1SysNo = c2Info.ParentSysNumber.Value;
            Category1SysNo = getC1SysNo;
            Category2SysNo = getC2SysNo;
            this.cmbCategory1.SelectedValue = Category1SysNo;
            this.cmbCategory2.SelectedValue = Category2SysNo;
            this.cmbCategory3.SelectedValue = Category3SysNo;
        }

        public void BindC2SysNo()
        {
            int? getC2SysNo = Category2SysNo;
            int? getC1SysNo = null;
            Category1SysNo = null;
            Category2SysNo = getC2SysNo;
            CategoryInfo c2Info = Category2List.SingleOrDefault(i => i.SysNo == getC2SysNo);
            if (c2Info!=null&&c2Info.ParentSysNumber.HasValue)
            {
                getC1SysNo = c2Info.ParentSysNumber.Value;
                Category1SysNo = getC1SysNo;
                this.cmbCategory1.SelectedValue = Category1SysNo;
            }

            this.cmbCategory2.SelectedValue = Category2SysNo;
        }

        private void InitializeCategoryComboBox()
        {
            this.cmbCategory3.IsEnabled = enableThirdCategory;

            #region [加载所有1,2，3级分类]
            categoryFacade.QueryCategoryLevel("1", queryFilter, (obj, args) =>
            {

                if (args.FaultsHandle())
                {
                    return;
                }
                List<CategoryInfo> categoryInfo = Category1LoadDeActive ? args.Result : args.Result.Where(x => { return x.Status == CategoryStatus.Active; }).ToList();//不加载无效类别时 只获取有效状态的类型 Edit 不用Ray.L.Xing;               
                categoryInfo.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });
                List<CategoryVM> vmList = EntityConverter<List<CategoryInfo>, List<CategoryVM>>.Convert(categoryInfo, (s, t) =>
                {
                    for (int i = 0; i < s.Count; i++)
                    {
                        t[i].CategoryDisplayName = s[i].CategoryName.Content;
                    }
                });

                Category1List = categoryInfo;


                categoryFacade.QueryAllCategory2(queryFilter, (obj2, args2) =>
                {

                    if (args2.FaultsHandle())
                    {
                        return;
                    }
                    List<CategoryInfo> categoryInfo2 = Category2LoadDeActive ? args2.Result : args2.Result.Where(x => { return x.Status == CategoryStatus.Active; }).ToList();//不加载无效类别时 只获取有效状态的类型 Edit 不用Ray.L.Xing;               
                    categoryInfo2.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });

                    Category2List = categoryInfo2;
                    categoryFacade.QueryAllCategory3(queryFilter, (obj3, args3) =>
                    {

                        if (args3.FaultsHandle())
                        {
                            return;
                        }
                        List<CategoryInfo> categoryInfo3 = Category3LoadDeActive ? args3.Result : args3.Result.Where(x => { return x.Status == CategoryStatus.Active; }).ToList();//不加载无效类别时 只获取有效状态的类型 Edit 不用Ray.L.Xing;               
                        categoryInfo3.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });

                        Category3List = categoryInfo3;

                        List<CategoryInfo> initList = new List<CategoryInfo>();
                        initList.Add(new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });
                        this.cmbCategory2.ItemsSource = initList;

                        this.cmbCategory3.ItemsSource = initList;

                        this.cmbCategory1.ItemsSource = Category1List;

                        OnLoadingCategoryCompleted();


                        if (SetDefaultValueHandler != null)
                            SetDefaultValueHandler(cmbCategory1, cmbCategory2, cmbCategory3);
                    });
                });
            });
            #endregion
        }

        private void cmbCategory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 < Category2List.Count)
            {
                LoadCategory2DataBySysNo();
                var categoryInfo = this.cmbCategory1.SelectedItem as CategoryInfo;
                if (null != categoryInfo)
                {
                    Category1Name = categoryInfo.CategoryName.ToString() == ResCategoryPicker.ComboBox_SelectItem_DefaultText ? string.Empty : categoryInfo.CategoryName.ToString();
                }
                else
                {
                    Category1Name = string.Empty;
                }
            }
        }

        private void cmbCategory2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 < Category3List.Count)
            {
                LoadCategory3DataBySysNo();
                CategoryInfo categoryInfo = this.cmbCategory2.SelectedItem as CategoryInfo;
                if (null != categoryInfo)
                {
                    Category2Name = categoryInfo.CategoryName.ToString() == ResCategoryPicker.ComboBox_SelectItem_DefaultText ? string.Empty : categoryInfo.CategoryName.ToString();
                }
                else
                {
                    Category2Name = string.Empty;
                }
            }
        }

        private void LoadCategory2DataBySysNo()
        {
            queryFilter.Category1SysNo = this.cmbCategory1.SelectedValue == null ? 0 : int.Parse(this.cmbCategory1.SelectedValue.ToString());
            List<CategoryInfo> targetList = Category2List.Where(item => item.ParentSysNumber == queryFilter.Category1SysNo).ToList();
            if (null == targetList.SingleOrDefault(i => i.CategoryName.ToString() == ResCategoryPicker.ComboBox_SelectItem_DefaultText))
            {
                targetList.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });
            }

            this.cmbCategory2.ItemsSource = targetList;
            this.cmbCategory2.SelectedValue = Category2SysNo;
        }

        private void LoadCategory3DataBySysNo()
        {
            queryFilter.Category2SysNo = this.cmbCategory2.SelectedValue == null ? 0 : int.Parse(this.cmbCategory2.SelectedValue.ToString());
            List<CategoryInfo> targetList = Category3List.Where(item => item.ParentSysNumber == queryFilter.Category2SysNo).ToList();
            if (null == targetList.SingleOrDefault(i => i.CategoryName.ToString() == ResCategoryPicker.ComboBox_SelectItem_DefaultText))
            {
                targetList.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });
            }
            this.cmbCategory3.ItemsSource = targetList;
            this.cmbCategory3.SelectedValue = Category3SysNo;
            Category3Name = "";

        }

        private void OnLoadingCategoryCompleted()
        {
            var handler = LoadCategoryCompleted;
            if (handler != null)
            {
                EventArgs args = new EventArgs();
                handler(this, args);
            }

        }

        private void cmbCategory3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var handler = cmbCategory3SelectionChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
            CategoryInfo categoryInfo = this.cmbCategory3.SelectedItem as CategoryInfo;
            if (null != categoryInfo)
            {
                Category3Name = categoryInfo.CategoryName.ToString() == ResCategoryPicker.ComboBox_SelectItem_DefaultText ? string.Empty : categoryInfo.CategoryName.ToString();
            }
            else
            {
                Category3Name = string.Empty;
            }
        }
    }
}
