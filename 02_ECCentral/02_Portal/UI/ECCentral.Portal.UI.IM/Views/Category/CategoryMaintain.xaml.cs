using System;
using System.Windows;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryMaintain : PageBase
    {

        #region 属性
        private CategoryFacade _facade;
        private int _sysNo;
        #endregion

        #region 初始化加载

        public CategoryMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            var brandSysNo = Request.Param;
            BindPage(brandSysNo);
        }
        #endregion

        #region 查询绑定
        private void BindPage(string brandSysNo)
        {
            if (!String.IsNullOrEmpty(brandSysNo))
            {
                _facade = new CategoryFacade();
                _sysNo = int.Parse(brandSysNo);
                _facade.GetCategoryBySysNo(int.Parse(brandSysNo), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        Window.MessageBox.Show("没有获得分类信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<CategoryInfo, CategoryVM>();

                    DataContext = vm;
                });
            }
            else
            {
                _sysNo = 0;
                DataContext = new CategoryVM();
                cmbCategoryStatusList.SelectedIndex = 0;
            }
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CategoryVM;
            if (vm == null)
            {
                return;
            }
            ValidationManager.Validate(ChildLayoutRoot);
            //vm.CategoryID = vm.CategoryID.Trim();
            //if (String.IsNullOrEmpty(vm.CategoryID))
            //{
            //    Window.MessageBox.Show("分类编号不能为空.", MessageBoxType.Warning);
            //    return;
            //}
            _facade = new CategoryFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                _facade.CreateCategory(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.SysNo = args.Result.SysNo;
                    vm.CategoryID = Convert.ToString(args.Result.SysNo);
                    Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                });
            }
            else
            {
                //_facade.UpdateCategory(vm, (obj, args) =>
                //{
                //    if (args.FaultsHandle())
                //    {
                //        var errorMsg = args.Error.Faults[0].ErrorDescription;
                //        Window.Alert(errorMsg);
                //        return;
                //    }
                //    Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                //});
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }

        private void btnMaintainBrand_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #endregion
    }

}
