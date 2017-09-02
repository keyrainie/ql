using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using System.Collections.Generic;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Components.UserControls.Language;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryQuery : PageBase
    {
        #region 字段以及构造函数
        CategoryQueryVM model;
        private CategoryType catetype;
        public CategoryQuery()
        {
            InitializeComponent();
           
        }

       
        #endregion

        #region 初始化加载
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new CategoryQueryVM();
            this.DataContext = model;
           
        }
        #endregion

        #region 查询绑定
        private void btnCategorySearch_Click(object sender, RoutedEventArgs e)
        {
            catetype = myConnection.CategoryType;
            switch (catetype)
            {
                case CategoryType.CategoryType1:
                    dgCategoryQueryResult.Columns[2].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[3].Visibility = Visibility.Collapsed;
                    dgCategoryQueryResult.Columns[4].Visibility = Visibility.Collapsed;
                    break;
                case CategoryType.CategoryType2:
                     dgCategoryQueryResult.Columns[2].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[3].Visibility = Visibility.Visible;
                    dgCategoryQueryResult.Columns[4].Visibility = Visibility.Collapsed;
                    break;
                case CategoryType.CategoryType3:
                     dgCategoryQueryResult.Columns[2].Visibility = Visibility.Visible;
                     dgCategoryQueryResult.Columns[3].Visibility = Visibility.Visible;
                     dgCategoryQueryResult.Columns[4].Visibility = Visibility.Visible;
                    break;
                default:
                     dgCategoryQueryResult.Columns[2].Visibility = Visibility.Visible;
                     dgCategoryQueryResult.Columns[3].Visibility = Visibility.Visible;
                     dgCategoryQueryResult.Columns[4].Visibility = Visibility.Visible;
                    break;
            }
            dgCategoryQueryResult.Bind();
            
          
        }


        private void dgCategoryQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
           
            CategoryQueryFacade facade = new CategoryQueryFacade(this);
            facade.QueryCategory(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dgCategoryQueryResult.ItemsSource = args.Result.Rows;
                this.dgCategoryQueryResult.TotalCount = args.Result.TotalCount;
            });
        }
        #endregion

        #region 页面事件
        private void btnCategoryNew_Click(object sender, RoutedEventArgs e)
        {
            CategoryEditMaintain item = new CategoryEditMaintain();
            item.IsEdit = false;
            item.Dialog = Window.ShowDialog("添加类别", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgCategoryQueryResult.Bind();
                }
            }, new Size(600, 500));
        }

        private void MultiLanguagelinkCategorySysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgCategoryQueryResult.SelectedItem as dynamic;
            string BizEntityType;

            if (d.Category3SysNo != 0)
            {
                BizEntityType = "Category3";
            }
            else if (d.Category2SysNo != 0)
            {
                BizEntityType = "Category2";
            }
            else
            {
                BizEntityType = "Category1";
            }

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(d.CategorySysNo, BizEntityType);

            item.Dialog = Window.ShowDialog("编辑类别多语言", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgCategoryQueryResult.Bind();
                }
            }, new Size(750, 600));
        }

        private void hyperlinkCategorySysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgCategoryQueryResult.SelectedItem as dynamic;
            CategoryEditMaintain item = new CategoryEditMaintain();
            item.IsEdit = true;
            item.Data = new CategoryVM() 
            {
                Category1SysNo = d.Category1SysNo,
                Category2SysNo = d.Category2SysNo,
                Category3SysNo=d.Category3SysNo,
                CategoryName = d.CategoryName,
                Status = (CategoryStatus)d.CategoryStatus,
                SysNo = d.CategorySysNo,
                Type = catetype
             };
            if (catetype == CategoryType.CategoryType3)
            {
                item.Data.C3Code = d.C3Code;
            }
            item.Dialog = Window.ShowDialog("更新类别", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgCategoryQueryResult.Bind();
                }
            }, new Size(600, 500));
        }
        #endregion

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.IM_CategoryRequestApprovalUrl, null, true);
        }

    }

}
