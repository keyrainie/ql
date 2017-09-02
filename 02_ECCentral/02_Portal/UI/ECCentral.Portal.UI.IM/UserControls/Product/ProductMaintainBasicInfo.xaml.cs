using System;
using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainBasicInfo : UserControl, ISave, IBatchSave
    {

        public ProductMaintainBasicInfoVM VM
        {
            get
            {
                return DataContext as ProductMaintainBasicInfoVM;
            }
        }

        public ProductMaintainBasicInfo()
        {
            InitializeComponent();
        }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().UpdateProductBasicInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        mainPage.Context.Window.MessageBox.Show("商品基本信息更新成功", MessageBoxType.Success);

                        var page = mainPage as ProductMaintain;
                        if (page != null)
                        {
                            page.VM.ProductMaintainCommonInfo.ProductBrandName =
                                page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.BrandInfo.
                                    BrandNameLocal;
                            page.VM.ProductMaintainCommonInfo.ProductManufacturerName =
                                page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.
                                    ManufacturerInfo.ManufacturerNameLocal;
                            page.VM.ProductMaintainCommonInfo.ProductCategoryName =
                                page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.CategoryInfo.
                                    CategoryName;
                            page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.
                                ProductManagerInfo = new PMVM
                                                         {
                                                             SysNo = args.Result.ProductManager.UserInfo.SysNo,
                                                             PMUserName = args.Result.ProductManager.UserInfo.UserDisplayName
                                                         };
                            if (!String.IsNullOrEmpty(page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TimelyPromotionTitle)
                                && DateTime.Now >= page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TimelyPromotionBeginDate
                                && DateTime.Now <= page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TimelyPromotionEndDate)
                            {
                                page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.PromotionTitle =
                                    page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.
                                        TimelyPromotionTitle;
                            }
                            else
                            {
                                page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.PromotionTitle =
                                    page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.
                                        NormalPromotionTitle;
                            }
                        }
                    });
        }

        public void BatchSave()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().BatchUpdateProductBasicInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品基本信息批量更新成功", MessageBoxType.Success);
                    });
        }
    }
}
