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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Portal.Basic.Utilities;
using System.Threading;

namespace ECCentral.Portal.Basic.Components.UserControls.CategoryPicker
{
    public partial class UCCategoryQuery : UserControl
    {
        private CategoryFacade categoryFacade;

        private List<CategoryInfo> Category1List=new List<CategoryInfo>();
        private List<CategoryInfo> Category2List=new List<CategoryInfo>();
        private List<CategoryVM> Category3VMList = new List<CategoryVM>();
        private CategoryQueryFilter queryFilter = new CategoryQueryFilter();

        private List<CategoryVM> filterC3List = new List<CategoryVM>();

        private List<CategoryVM> selectedList = new List<CategoryVM>();
        public UCCategoryQuery()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCCategoryQuery_Loaded);
        }

        void UCCategoryQuery_Loaded(object sender, RoutedEventArgs e)
        {  
            categoryFacade = new CategoryFacade(CPApplication.Current.CurrentPage);
            queryFilter.CompanyCode = "8601";
            InitializeCategoryComboBox();
        }

        /// <summary>
        /// 应用Window.ShowDialog弹出时，会返回一个DialogHandle,根据这个DialogHandle可以做关闭对话框等操作
        /// </summary>
        public IDialog DialogHandler { get; set; }


        /// <summary>
        /// 当前打开的Tab页面
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }


        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {            
            if (queryFilter.Category2SysNo.HasValue)
            {
                filterC3List = Category3VMList.Where(item => item.ParentSysNumber == queryFilter.Category2SysNo).ToList();
            }
            else
            {
                filterC3List = Category3VMList;
            }
            gridC3.ItemsSource = filterC3List;
            gridC3.Bind();
        }

        private void cmbCategory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CategoryInfo c1 = (CategoryInfo)cmbCategory1.SelectedItem;
            queryFilter.Category1SysNo = c1.SysNo;
            //cmbCategory2.SelectionChanged -= cmbCategory2_SelectionChanged;
            if (queryFilter.Category1SysNo.HasValue)
            {
                List<CategoryInfo> targetList = Category2List.Where(item => item.ParentSysNumber == queryFilter.Category1SysNo).ToList();
                if (null == targetList.SingleOrDefault(i => i.CategoryName.ToString() == ResCategoryPicker.ComboBox_SelectItem_DefaultText))
                {
                    targetList.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });
                }

                this.cmbCategory2.ItemsSource = targetList;

                this.cmbCategory2.SelectedIndex = 0;
            }
            else
            {
                this.cmbCategory2.ItemsSource = Category2List;
                this.cmbCategory2.SelectedIndex = 0;
            }
        }
        private void cmbCategory2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CategoryInfo c1 = (CategoryInfo)cmbCategory1.SelectedItem;

            CategoryInfo c2 = (CategoryInfo)cmbCategory2.SelectedItem;
            if (c2 != null)
            {
                queryFilter.Category2SysNo = c2.SysNo;


                if (c2.SysNo.HasValue && !c1.SysNo.HasValue)
                {

                        cmbCategory1.SelectionChanged -= cmbCategory1_SelectionChanged;
                        cmbCategory1.SelectedValue = c2.ParentSysNumber;
                        cmbCategory1.SelectionChanged += cmbCategory1_SelectionChanged;

                    }
                if (c2.SysNo.HasValue && c1.SysNo.HasValue)
                {
                    if (c1.SysNo.Value != c2.ParentSysNumber)
                    {
                        cmbCategory1.SelectionChanged -= cmbCategory1_SelectionChanged;
                        cmbCategory1.SelectedValue = c2.ParentSysNumber;
                        cmbCategory1.SelectionChanged += cmbCategory1_SelectionChanged;
                    }
                }
                }

        }
        
        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked.Value;
            foreach (CategoryVM cate in Category3VMList)
            {
                cate.IsChecked = isChecked;
            }

        }

        private void ButtonConfirmSelected_Click(object sender, RoutedEventArgs e)
        {
            //关闭对话框并返回数据
            selectedList = new List<CategoryVM>();
            foreach (CategoryVM cate in filterC3List)
            {
                if (cate.IsChecked.Value)
                {
                    selectedList.Add(cate);
                }
            }
            this.DialogHandler.ResultArgs.Data = selectedList;
            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.OK;
            this.DialogHandler.Close();
        }

        private void ButtonCloseDialog_Click(object sender, RoutedEventArgs e)
        {
            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.DialogHandler.Close();
        }

        private void InitializeCategoryComboBox()
        {
            #region [加载所有1,2,3级分类]

            categoryFacade.QueryCategoryLevel("1", queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Category1List = args.Result;
                Category1List.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });

                categoryFacade.QueryAllCategory2(queryFilter, (obj2, args2) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Category2List = args2.Result;
                    Category2List.Insert(0, new CategoryInfo() { ParentSysNumber = 0, CategoryName = new BizEntity.LanguageContent(ResCategoryPicker.ComboBox_SelectItem_DefaultText) });
                    
                    categoryFacade.QueryAllCategory3(queryFilter, (obj3, args3) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        List<CategoryInfo> category3List = args3.Result;

                        category3List.ForEach(f => Category3VMList.Add(new CategoryVM()
                        {
                            CategoryDisplayName = f.CategoryName.Content,
                            IsChecked = false,
                            ParentSysNumber = f.ParentSysNumber,
                            SysNo = f.SysNo,
                            Status = f.Status
                        }));


                        this.cmbCategory1.ItemsSource = Category1List;
                        this.cmbCategory2.ItemsSource = Category2List;

                        this.cmbCategory1.SelectedIndex = 0;
                        this.cmbCategory2.SelectedIndex = 0;

                    });
                });
            });


            #endregion



        }

        


       
      
    }
}
