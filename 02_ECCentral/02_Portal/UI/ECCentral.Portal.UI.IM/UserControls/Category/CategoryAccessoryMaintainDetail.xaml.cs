using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryAccessoryMaintainDetail : UserControl
    {
        public IDialog Dialog { get; set; }

        public int? SysNo { get; set; }

        #region 属性
        private CategoryAccessoriesFacade _facade;
        #endregion

        #region 初始化加载
        public CategoryAccessoryMaintainDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BindcmbAccessoriesName();
            dpCategory.LoadCategoryCompleted += BindSource;
        }

        private void BindSource(object sender, EventArgs e)
        {
            BindPage();
        }
        #endregion

        #region 查询绑定
        private void BindPage()
        {
            if (SysNo != null)
            {
                _facade = new CategoryAccessoriesFacade();
                _facade.GetCategoryAccessoryBySysNo(SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得分类配件信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<CategoryAccessory, CategoryAccessoriesVM>();
                    vm.CategoryInfo = args.Result.CategoryInfo.Convert<CategoryInfo, CategoryVM>
                        ((v, t) =>
                        {
                            t.CategoryName = v.CategoryName.Content;
                        });
                    vm.Accessory = args.Result.Accessory.Convert<AccessoryInfo, AccessoryVM>
                       ((v, t) =>
                       {
                           t.AccessoryName = v.AccessoryName.Content;
                       });
                    DataContext = vm;
                });
            }
            else
            {
                var brand = new CategoryAccessoriesVM { Accessory = new AccessoryVM(), CategoryInfo = new CategoryVM() };
                DataContext = brand;
                dplistStatus.SelectedIndex = 0;
            }
        }

        private void BindcmbAccessoriesName()
        {
            CategoryAccessoriesQueryFacade facade = new CategoryAccessoriesQueryFacade(CPApplication.Current.CurrentPage);
            facade.GetAllAccessories((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                IList<string> source = args.Result.Select(item => item.AccessoryName.Content).ToList();
                this.cmbAccessoriesName.ItemsSource = source;
            });
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件


        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CategoryAccessoriesVM;
            if (vm == null)
            {
                return;
            }
            if (vm.CategoryInfo == null || vm.CategoryInfo.SysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择三级分类.");
                return;
            }
            if (vm.Accessory == null || String.IsNullOrEmpty(vm.Accessory.AccessoryName))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请填写配件.");
                return;
            }
            if (vm.Priority == null || vm.Priority.Value <= 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请填写正确的配件顺序.");
                return;
            }
            _facade = new CategoryAccessoriesFacade();
            vm.SysNo = SysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                _facade.CreateCategoryAccessory(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.SysNo = args.Result.SysNo;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {
                _facade.UpdateCategoryAccessory(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                    CloseDialog(DialogResultType.OK);
                });
            }
        }

        #endregion

        #endregion

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }


    }
}
