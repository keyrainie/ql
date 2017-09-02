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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryExtendWarrantyMaintain : PageBase
    {
        CategoryExtendWarrantyQueryVM model;
        CategoryExtendWarrantyDisuseBrandQueryVM modelBrand;

        public CategoryExtendWarrantyMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new CategoryExtendWarrantyQueryVM();
            this.expander1.DataContext = model;
            modelBrand = new CategoryExtendWarrantyDisuseBrandQueryVM();
            this.expander2.DataContext = modelBrand;
        }


        #region 查询绑定

        private void Button_CategorySearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            this.dgCategoryExtendWarrantyQueryResult.Bind();
        }

        private void dgCategoryExtendWarrantyQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            CategoryExtendWarrantyQueryFacade facade = new CategoryExtendWarrantyQueryFacade(this);
            model = (CategoryExtendWarrantyQueryVM)this.expander1.DataContext;
            facade.QueryCategoryExtendWarranty(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dgCategoryExtendWarrantyQueryResult.ItemsSource = args.Result.Rows;
                this.dgCategoryExtendWarrantyQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_BrandSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            this.dgCategoryExtendBrandQueryResult.Bind();
        }

        private void dgCategoryExtendBrandQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            CategoryExtendWarrantyQueryFacade facade = new CategoryExtendWarrantyQueryFacade(this);
            modelBrand = (CategoryExtendWarrantyDisuseBrandQueryVM)this.expander2.DataContext;
            facade.QueryCategoryExtendWarrantyDisuseBrand(modelBrand, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dgCategoryExtendBrandQueryResult.ItemsSource = args.Result.Rows;
                this.dgCategoryExtendBrandQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void hyperlinkCategoryEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgCategoryExtendWarrantyQueryResult.SelectedItem as dynamic;
            if (item == null) return;
            var sysNo = Convert.ToInt32(item.SysNo);
            //类别延保编辑
            CategoryExtendWarrantyCategoryDetail detail = new CategoryExtendWarrantyCategoryDetail();
            detail.SysNo = sysNo;
            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryExtendWarrantyMaintainDetail.Dialog_EditCategory, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgCategoryExtendWarrantyQueryResult.Bind();
                }
            }, new Size(650, 300));
        }

        private void hyperlinkBrandEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgCategoryExtendBrandQueryResult.SelectedItem as dynamic;
            if (item == null) return;
            var sysNo = Convert.ToInt32(item.SysNo);
            //类别延保编辑
            CategoryExtendWarrantyBrandDetail detail = new CategoryExtendWarrantyBrandDetail();
            detail.SysNo = sysNo;
            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryExtendWarrantyMaintainDetail.Dialog_EditCategory, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgCategoryExtendBrandQueryResult.Bind();
                }
            }, new Size(600, 160));
        }

        #endregion

        #region 跳转

        private void btnCategoryNew_Click(object sender, RoutedEventArgs e)
        {
            //类别延保添加
            CategoryExtendWarrantyCategoryDetail mainUC = new CategoryExtendWarrantyCategoryDetail();

            mainUC.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryExtendWarrantyMaintainDetail.Dialog_AddCategory, mainUC, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgCategoryExtendWarrantyQueryResult.Bind();
                }
            }, new Size(650, 300));
        }

        private void btnBrandNew_Click(object sender, RoutedEventArgs e)
        {
            //类别延保添加
            CategoryExtendWarrantyBrandDetail mainUC = new CategoryExtendWarrantyBrandDetail();

            mainUC.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryExtendWarrantyMaintainDetail.Dialog_AddBrand, mainUC, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.dgCategoryExtendBrandQueryResult.Bind();
                }
            }, new Size(600, 160));
        }
        #endregion

        #endregion


    }
}
