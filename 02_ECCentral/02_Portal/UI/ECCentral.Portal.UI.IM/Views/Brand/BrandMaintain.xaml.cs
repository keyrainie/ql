using System;
using System.Windows;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class BrandMaintain
    {
        #region 属性
        private BrandFacade _facade;
        private int _sysNo;
        #endregion

        #region 初始化加载
        public BrandMaintain()
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
                _facade = new BrandFacade();
                if (!int.TryParse(brandSysNo,out _sysNo))
                {
                    Window.MessageBox.Show("无效品牌编号.", MessageBoxType.Warning);
                    return;
                }
                _facade.GetBrandBySysNo(_sysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        Window.MessageBox.Show("没有获得品牌信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<BrandInfo, BrandVM>();
                    vm.BrandSupportInfo = args.Result.BrandSupportInfo.Convert<BrandSupportInfo, BrandSupportVM>();
                    if (args.Result.Manufacturer != null)
                    {
                        vm.ManufacturerInfo = args.Result.Manufacturer.Convert<ManufacturerInfo, ManufacturerVM>();
                        vm.ManufacturerInfo.ManufacturerNameLocal = args.Result.Manufacturer.ManufacturerNameLocal.Content;
                    }
                    vm.BrandID = Convert.ToString(vm.SysNo);
                    DataContext = vm;
                    //ucManufacturerPicker.IsEnabled = false;
                });
            }
            else
            {
                _sysNo = 0;
                var brand = new BrandVM { ManufacturerInfo = new ManufacturerVM(), BrandSupportInfo = new BrandSupportVM() };
                DataContext = brand;
                cmbBrandStatusLis.SelectedIndex = 0;
                ucManufacturerPicker.IsEnabled = true;
            }
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件


        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as BrandVM;
            if (vm == null)
            {
                return;
            }

            if (!ValidationManager.Validate(this))
            {
                return;
            }
            /*
            if (vm.ManufacturerInfo == null || vm.ManufacturerInfo.SysNo == null)
            {
                Window.MessageBox.Show("生产商不空为能.", MessageBoxType.Warning);
                return;
            }*/
            _facade = new BrandFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                _facade.CreateBrand(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.SysNo = args.Result.SysNo;
                    vm.BrandID = Convert.ToString(vm.SysNo);
                    Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                });
            }
            else
            {
                _facade.UpdateBrand(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                });
            }
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }


        #endregion

        #endregion

        #region 跳转
        #endregion
    }
}
