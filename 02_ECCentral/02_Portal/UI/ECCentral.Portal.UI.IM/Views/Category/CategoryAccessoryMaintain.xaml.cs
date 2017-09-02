using System;
using System.Linq;
using System.Windows;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryAccessoryMaintain : PageBase
    {

        #region 属性
        CategoryAccessoriesQueryVM model;
        #endregion

        #region 初始化加载

        public CategoryAccessoryMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new CategoryAccessoriesQueryVM();
            this.DataContext = model;
            BindcmbAccessoriesName();
        }

        #endregion

        #region 查询绑定
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }             

            dgCategoryAccessoryQueryResult.Bind();
        }

        private void dgCategoryAccessoryQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            CategoryAccessoriesQueryFacade facade = new CategoryAccessoriesQueryFacade(this);
            model = (CategoryAccessoriesQueryVM)this.DataContext;
            facade.QueryCategoryAccessories(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dgCategoryAccessoryQueryResult.ItemsSource = args.Result.Rows;
                this.dgCategoryAccessoryQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void BindcmbAccessoriesName()
        {
            CategoryAccessoriesQueryFacade facade = new CategoryAccessoriesQueryFacade(this);
            facade.GetAllAccessories((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var source = args.Result.Select(p => p.AccessoryName.Content).ToList();

                this.cmbAccessoriesName.ItemsSource = source;
            });
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void hyperlinkOperationEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgCategoryAccessoryQueryResult.SelectedItem as dynamic;
            if (manufacturer == null) return;
            var sysNo = Convert.ToInt32(manufacturer.SysNo);
            //类别属性编辑
            CategoryAccessoryMaintainDetail detail = new CategoryAccessoryMaintainDetail();
            detail.SysNo = sysNo;
            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryAccessoryMaintainDetail.Dialog_EditCategoryAccessory, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgCategoryAccessoryQueryResult.Bind();
                }
            }, new Size(550, 300));
        }

        private void btnCatetoryAccessoryEdit_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #endregion

        #region 跳转

        private void btnCatetoryAccessoryNew_Click(object sender, RoutedEventArgs e)
        {
            CategoryAccessoryMaintainDetail detail = new CategoryAccessoryMaintainDetail();

            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryAccessoryMaintainDetail.Dialog_EditCategoryAccessory, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgCategoryAccessoryQueryResult.Bind();
                }
            }, new Size(550, 300));
        }
        #endregion

    }

}
