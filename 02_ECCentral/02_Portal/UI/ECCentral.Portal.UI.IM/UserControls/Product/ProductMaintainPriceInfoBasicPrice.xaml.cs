using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPriceInfoBasicPrice : UserControl
    {
        public ProductMaintainPriceInfoBasicPriceVM VM
        {
            get { return DataContext as ProductMaintainPriceInfoBasicPriceVM; }
        }


        public ProductMaintainPriceInfoBasicPrice()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ProductMaintainPriceInfoBasicPrice_Loaded);
        }

        private void ProductMaintainPriceInfoBasicPrice_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(ProductMaintainPriceInfoBasicPrice_Loaded);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemVirtualPriceEdit))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            if (txtVirtualPrice.IsEnabled)
            {
                List<ValidationEntity> validList = new List<ValidationEntity>();
                validList.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, txtVirtualPrice.Text, "不能为空"));
                validList.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^[0-9]+\.?[0-9]{0,6}$", "请输入正确的金额"));
                if (!ValidationHelper.Validation(txtVirtualPrice, validList)) return;

                var virtualPrice = txtVirtualPrice.Text.ToString();
                var servicesFacade = new ProductFacade();
                servicesFacade.UpdateProductVirtualPrice(VM.MainPageVM.ProductSysNo.ToString(), virtualPrice, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    txtVirtualPrice.IsEnabled = false;
                    btnEdit.Content = ResProductMaintain.Button_Edit;
                });
            }
            else
            {
                txtVirtualPrice.IsEnabled = true;
                btnEdit.Content = ResProductMaintain.Button_Save;
            }
        }
    }
}
