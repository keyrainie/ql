using System.Windows.Controls;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPriceInfo : UserControl, ISave
    {
        public ProductMaintainPriceInfoVM VM
        {
            get { return DataContext as ProductMaintainPriceInfoVM; }
        }

        public ProductMaintainPriceInfo()
        {
            InitializeComponent();
        }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (!ValidationManager.Validate(ucBasicPriceInfo))
            {
                return;
            }

            if (!VM.HasItemPriceMaintainPermission)
            {
                mainPage.Context.Window.Alert("没有编辑该商品价格的权限，无法保存", MessageType.Error);
                return;
            }

            if (VM.ProductPriceRequestStatus.HasValue)
            {
                mainPage.Context.Window.Alert("已提交价格调整申请，无法保存", MessageType.Error);
                return;
            }

            if (mainPage != null && mainPage is ProductMaintain)
            {
                new ProductFacade().UpdateProductPriceInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品价格信息更新成功", MessageBoxType.Success);
                    });
            }
        }
    }
}
