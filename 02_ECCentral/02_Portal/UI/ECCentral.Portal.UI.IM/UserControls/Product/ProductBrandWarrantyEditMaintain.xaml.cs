using ECCentral.Portal.UI.IM.Facades;
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
    public partial class ProductBrandWarrantyEditMaintain : UserControl
    {

        #region Property
        public Int32? SysNo { get; set; }
        public IDialog Dialog { get; set; }
        public List<Int32> EditSysNos { get; set; }
        public ProductBrandWarrantyQueryVM Data { get; set; }
        #endregion

        #region Method
        public ProductBrandWarrantyEditMaintain()
        {
            InitializeComponent();
            //this.Loaded += ProductBrandWarrantyEditMaintain_Loaded;
            this.ucCategoryPicker.LoadCategoryCompleted += ucCategoryPicker_LoadCategoryCompleted;
        }
        public void ucCategoryPicker_LoadCategoryCompleted(object sender, EventArgs e)
        {
            if (Data == null)
            {
                Data = new ProductBrandWarrantyQueryVM();
                Data.WarrantyDay = "0";
                this.ucCategoryPicker.IsEnabled = true;
            }
            else
            {
                this.ucBrandPicker.IsEnabled = false;
                this.ucCategoryPicker.IsEnabled = false;
                this.ucBrandPicker.SelectedBrandName = Data.BrandName;
            }
            DataContext = Data;
        }
        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            ProductBrandWarrantyFacade facade = new ProductBrandWarrantyFacade();
            var vm = DataContext as ProductBrandWarrantyQueryVM;
            if (vm == null || !ValidationManager.Validate(this)) return;
            if (ValidatePage(vm))
            {
                //新增
                if (EditSysNos == null)
                {
                    facade.BrandWarrantyInfoByAddOrUpdate(vm, (obj, arg) =>
                    {
                        if (arg.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("保存成功!", MessageBoxType.Success);
                    });
                }
                else
                {
                    //批量更新
                    facade.UpdateBrandWarrantyInfoBySysNo(EditSysNos, vm,(obj, arg) =>
                    {
                        if (arg.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("保存成功!", MessageBoxType.Success);
                    });
                }
                CloseDialog(DialogResultType.OK);
            }
        }
        //private void BtnPreview_Click(object sender, RoutedEventArgs e)
        //{

        //}
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
        protected Boolean ValidatePage(ProductBrandWarrantyQueryVM pageVm)
        {
            if (EditSysNos == null)
            {
                //品牌
                if (pageVm.BrandSysNo == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("必须选择品牌");
                    return false;
                }
                //类别
                if (pageVm.C1SysNo == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("必须选择类别");
                    return false;
                }
            }
            //详细描述
            if (string.IsNullOrEmpty(pageVm.WarrantyDesc))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请输入详细描述");
                return false;
            }
            return true;
        }
        #endregion
    }
}
