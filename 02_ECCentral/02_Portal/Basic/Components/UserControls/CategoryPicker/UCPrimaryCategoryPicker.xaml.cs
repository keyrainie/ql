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
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Windows.Data;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Components.UserControls.CategoryPicker
{
    public partial class UCPrimaryCategoryPicker : UserControl
    {

        public string CategorySysNo
        {
            get { return (string)GetValue(CategorySysNoProperty); }
            set { SetValue(CategorySysNoProperty, value); }
        }

        public string CategoryName
        {
            get
            {
                CategoryInfo categoryInfo = this.cmbCategory.SelectedItem as CategoryInfo;
                if (null != categoryInfo)
                {
                    return categoryInfo.CategoryName.ToString() == ResCategoryPicker.ComboBox_SelectItem_DefaultText ? string.Empty : categoryInfo.CategoryName.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

        }

        
        public static readonly DependencyProperty CategorySysNoProperty =
            DependencyProperty.Register("CategorySysNo", typeof(string), typeof(UCPrimaryCategoryPicker), new PropertyMetadata(null));

        private CategoryFacade categoryFacade;
        public CategoryQueryFilter queryFilter;

        public List<CategoryInfo> CategoryList;        

        public UCPrimaryCategoryPicker()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCPrimaryCategoryPicker_Loaded);
            //var categoryCmbBindingExp = this.GetBindingExpression(UCPrimaryCategoryPicker.CategorySysNoProperty);
            //if (categoryCmbBindingExp != null && categoryCmbBindingExp.ParentBinding != null)
            //{
            //    cmbCategory.SetBinding(ComboBox.SelectedValueProperty, categoryCmbBindingExp.ParentBinding);
            //}
        }

        void UCPrimaryCategoryPicker_Loaded(object sender, RoutedEventArgs e)
        {
            CategoryList = new List<CategoryInfo>();

            categoryFacade = new CategoryFacade(CPApplication.Current.CurrentPage);
            queryFilter = new CategoryQueryFilter()
            {
                CompanyCode = "8601"
            };
            InitializeCategoryComboBox();

            //Loaded -= new RoutedEventHandler(UCPrimaryCategoryPicker_Loaded);
            //var exp = this.GetBindingExpression(UCPrimaryCategoryPicker.CategorySysNoProperty);
            //if (exp != null && exp.ParentBinding != null)
            //{
            //    string path = exp.ParentBinding.Path.Path;
            //    Binding binding = new Binding();
            //    binding.Path = new PropertyPath(path);
            //    binding.Mode = BindingMode.TwoWay;
            //    binding.NotifyOnValidationError = true;
            //    cmbCategory.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            //}


        }

        private void InitializeCategoryComboBox()
        {
            #region [加载所有分类]
            categoryFacade.QueryAllPrimaryCategory(queryFilter, (obj, args) =>
            {

                if (args.FaultsHandle())
                {
                    return;
                }
                List<CategoryInfo> categoryInfo = args.Result;
                categoryInfo.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });
                List<CategoryVM> vmList = EntityConverter<List<CategoryInfo>, List<CategoryVM>>.Convert(categoryInfo, (s, t) =>
                {
                    for (int i = 0; i < s.Count; i++)
                    {
                        t[i].CategoryDisplayName = s[i].CategoryName.Content;
                    }
                });

                CategoryList = categoryInfo;

                this.cmbCategory.ItemsSource = CategoryList;
                this.cmbCategory.SelectedIndex = 0;
            });

            

            #endregion

            //List<CategoryInfo> initList = new List<CategoryInfo>();
            //initList.Add(new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });

        }

    }
}
