using ECCentral.Portal.UI.IM.Facades.RmaPolicy;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class DefaultRMAPolicyEditMaintain : UserControl
    {
        #region Property
        public Int32? SysNo { get; set; }
        public IDialog Dialog { get; set; }
        public Int32 EditSysNos { get; set; }
        public RmaPolicySettingQueryVM Data { get; set; }
        public DefaultRMAPolicyFacade Facade { get; set; }
        #endregion

        #region Method
        public DefaultRMAPolicyEditMaintain()
        {
            InitializeComponent();
            this.ucCategoryPicker.LoadCategoryCompleted += ucCategoryPicker_LoadCategoryCompleted;
        }
        private void ucCategoryPicker_LoadCategoryCompleted(object sender, EventArgs e)
        {
            if (Data == null)
            {
                Data = new RmaPolicySettingQueryVM();
                this.ucBrandPicker.IsEnabled = true;
                this.ucCategoryPicker.IsEnabled = true;
                this.ucRmaPolicyComboxList.IsEdit = true;
            }
            else
            {
                this.ucBrandPicker.SelectedBrandName = Data.BrandName;
                this.ucBrandPicker.SelectedBrandSysNo = Data.BrandSysNo.ToString();
                this.ucCategoryPicker.Category1SysNo = Data.C1SysNo;
                this.ucCategoryPicker.Category2SysNo = Data.C2SysNo;
                this.ucCategoryPicker.Category3SysNo = Data.C3SysNo;
                this.ucBrandPicker.IsEnabled = false;
                this.ucCategoryPicker.IsEnabled = false;
                this.ucRmaPolicyComboxList.IsEdit = true;
                this.ucRmaPolicyComboxList.BingSelectValue(Data.RMAPolicySysNo);
            }
            DataContext = Data;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Facade = new DefaultRMAPolicyFacade();
            var vm = DataContext as RmaPolicySettingQueryVM;
            vm.RMAPolicySysNo = ucRmaPolicyComboxList.SelectValue;
            if (ValidatePage(vm))
            {
                if (Data.SysNo == null)
                {
                    Facade.DefaultRMAPolicyInfoAdd(vm, (obj, arg) =>
                    {
                        if (arg.FaultsHandle()) return;
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("保存成功!", MessageBoxType.Success);
                    });
                }
                else
                {
                    Facade.UpdateDefaultRMAPolicy(vm, (obj, arg) => {
                        if (arg.FaultsHandle()) return;
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("保存成功!", MessageBoxType.Success);
                    });
                }
                CloseDialog(DialogResultType.OK);
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
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

        #region provate method
        protected Boolean ValidatePage(RmaPolicySettingQueryVM pageVm)
        {
            if (Data.SysNo == null)
            {
                //类别
                if (pageVm.C3SysNo == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("必须选择三级类别");
                    return false;
                }
            }
            if (ucRmaPolicyComboxList.SelectValue == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择退换货政策");
                return false;
            }
            return true;
        }
        #endregion
    }
}
