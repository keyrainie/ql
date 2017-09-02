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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryExtendWarrantyBrandDetail : UserControl
    {

        public IDialog Dialog { get; set; }

        public int? SysNo { get; set; }


        #region 属性
        private CategoryExtendWarrantyFacade _facade;
        private int _sysNo;
        #endregion

        #region 初始化加载

        public CategoryExtendWarrantyBrandDetail()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
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
                _facade = new CategoryExtendWarrantyFacade();
                _facade.GetCategoryExtendWarrantyDisuseBrandBySysNo(SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得分类延保信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<CategoryExtendWarrantyDisuseBrand, CategoryExtendWarrantyDisuseBrandVM>();

                    if (args.Result.CategoryInfo.SysNo.HasValue && args.Result.CategoryInfo.SysNo > 0)
                    {
                        vm.CategoryInfo = args.Result.CategoryInfo.Convert<CategoryInfo, CategoryVM>
                            ((v, t) =>
                            {
                                t.Category3SysNo = v.SysNo > 0 ? v.SysNo : 0;
                                t.CategoryName = v.CategoryName.Content;
                            });
                    }
                    else
                    {
                        vm.CategoryInfo = new CategoryVM();
                        vm.CategoryInfo.Category3SysNo = 0;
                    }

                    vm.Brand = args.Result.Brand.Convert<BrandInfo, BrandVM>
                        ((v, t) =>
                        {
                            t.BrandNameLocal = v.BrandNameLocal.Content;
                        });

                    _sysNo = SysNo.Value;

                    this.ucManufacturerPicker.IsEnabled = false;
                    this.dpCategory.IsEnabled = false;

                    DataContext = vm;
                });
            }
            else
            {
                _sysNo = 0;
                var item = new CategoryExtendWarrantyDisuseBrandVM { CategoryInfo = new CategoryVM(), Brand = new BrandVM() };
                item.Status = CategoryExtendWarrantyDisuseBrandStatus.DeActive;
                DataContext = item;

            }
        }


        #endregion

        #region 页面内按钮处理事件

        #region 界面事件


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CategoryExtendWarrantyDisuseBrandVM;
            if (vm == null)
            {
                return;
            }

            if (!ValidationManager.Validate(this))
            {
                return;
            }

            //if (vm.CategoryInfo == null || vm.CategoryInfo.SysNo == null)
            //{
            //    CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请选择三级分类.", MessageBoxType.Warning);
            //    return;
            //}

            if (vm.Brand == null || vm.Brand.SysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请选择品牌.", MessageBoxType.Warning);
                return;
            }

            _facade = new CategoryExtendWarrantyFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                _facade.CreateCategoryExtendWarrantyDisuseBrand(vm, (obj, args) =>
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
                _facade.UpdateCategoryExtendWarrantyDisuseBrand(vm, (obj, args) =>
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        #endregion
    }
}
