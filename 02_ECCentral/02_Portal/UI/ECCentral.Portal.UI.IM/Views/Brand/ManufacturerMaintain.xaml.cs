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
    public partial class ManufacturerMaintain : PageBase
    {
        #region 属性
        private ManufacturerFacade _facade;
        private int _sysNo;
        #endregion

        #region 初始化加载

        public ManufacturerMaintain()
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
                _facade = new ManufacturerFacade();
                if (!int.TryParse(brandSysNo, out _sysNo))
                {
                    Window.MessageBox.Show("生产商编号无效.", MessageBoxType.Warning);
                };
                _facade.GetManufacturerBySysNo(_sysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        Window.MessageBox.Show("没有获得生产商信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<ManufacturerInfo, ManufacturerVM>();
                    vm.SupportInfo = args.Result.SupportInfo.Convert<ManufacturerSupportInfo, ManufacturerSupportVM>();
                    DataContext = vm;
                });
            }
            else
            {
                _sysNo = 0;
                var brand = new ManufacturerVM { SupportInfo = new ManufacturerSupportVM() };
                DataContext = brand;
                cmbManufacturerStatusList.SelectedIndex = 0;
            }
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManufacturerVM;
            if (vm == null)
            {
                return;
            }
            //ValidationManager.Validate(ChildLayoutRoot);
            if (!ValidationManager.Validate(this))
            {
                return;
            }
           
            _facade = new ManufacturerFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {


                _facade.CreateManufacturer(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.SysNo = args.Result.SysNo;
                    vm.ManufacturerID = Convert.ToString(args.Result.SysNo);
                    Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                });
            }
            else
            {
                _facade.UpdateManufacturer(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                });
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

        #region 跳转
        #endregion
    }
}
