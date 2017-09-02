using System;
using System.Windows;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductCreate : PageBase
    {
        public List<ProductCountryVM> OrginList;
        public ProductCreate()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            DataContext = new ProductCreateSingleVM();
            (new ProductCreateFacade()).GetProductCountryList((obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                ucProductCreateSingle.cmbOrgin.ItemsSource = arg.Result;
                ucProductCreateSingle.cmbOrgin.SelectedIndex = 0;
            });
            ucProductCreateSingle.cmbConsignFlag.SelectedIndex = 0;
            ucProductCreateSingle.cmbProductType.SelectedIndex = 0;
            base.OnPageLoad(sender, e);
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            ProductCreateSingleVM vm = DataContext as ProductCreateSingleVM;
            if (vm == null)
            {
                return;
            }
            ValidationManager.Validate(this);
            if (vm.ValidationErrors.Count != 0) return;

            if (vm.BrandSysNo == null || vm.BrandSysNo <= 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请至少选择一个品牌！", MessageBoxType.Warning);
                return;
            }
            if (vm.C3SysNo == null || vm.C3SysNo <= 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请至少选择一个三级类别！", MessageBoxType.Warning);
                return;
            }

            vm.BrandSysNo = Convert.ToInt32(ucProductCreateSingle.ucBrandPicker.SelectedBrandSysNo);
            vm.BrandName = ucProductCreateSingle.ucBrandPicker.SelectedBrandName;

            vm.OrginCode = ucProductCreateSingle.cmbOrgin.SelectedValue.ToString();

            new ProductCreateFacade().CreateProduct(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                //Window.MessageBox(Alert(ResProductCreate.Info_SaveSuccessfully);
                Window.MessageBox.Show("创建商品成功！商品编号为 " + args.Result.ProductID, MessageBoxType.Success);
            });
        }
    }
}
