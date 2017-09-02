using System;
using System.Windows;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
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
    public partial class AccessoryMaintain : PageBase
    {
        #region 属性
        private AccessoryFacade _facade;
        private int _sysNo;
        #endregion

        #region 初始化加载

        public AccessoryMaintain()
        {
            InitializeComponent();
           
        }



        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            var accessorySysNo = Request.Param;
            BindPage(accessorySysNo);
        }

        #endregion

        #region 查询绑定
        private void BindPage(string accessorySysNo)
        {
            if (!String.IsNullOrEmpty(accessorySysNo))
            {
                _facade = new AccessoryFacade();
                if (!int.TryParse(accessorySysNo, out _sysNo))
                {
                    Window.MessageBox.Show("配件编号无效.", MessageBoxType.Warning);
                    return;
                }

                _facade.GetAccessoryBySysNo(int.Parse(accessorySysNo), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        Window.MessageBox.Show("没有获得配件信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<AccessoryInfo, AccessoryVM>();

                    DataContext = vm;
                });
            }
            else
            {
                _sysNo = 0;
                DataContext = new AccessoryVM();
            }
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as AccessoryVM;
            if (vm == null)
            {
                return;
            }
            ValidationManager.Validate(ChildLayoutRoot);
            if (vm.AccessoryName != null)
            {
                vm.AccessoryName = vm.AccessoryName.Trim();
            }
            if (String.IsNullOrEmpty(vm.AccessoryName))
            {
                Window.MessageBox.Show("配件名称不能为空.", MessageBoxType.Warning);
                return;
            }
            _facade = new AccessoryFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                _facade.CreateAccessory(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.SysNo = args.Result.SysNo;
                    vm.AccessoryID = Convert.ToString(vm.SysNo);
                    Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                });
            }
            else
            {
                _facade.UpdateAccessory(vm, (obj, args) =>
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


        #endregion

        #endregion
    }
}
