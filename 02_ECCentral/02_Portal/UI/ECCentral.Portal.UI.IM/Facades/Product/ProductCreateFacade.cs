using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM.Product;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductCreateFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductCreateFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductCreateFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 创建单个商品(创建不基于组的商品)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateProduct(ProductCreateSingleVM data, EventHandler<RestClientEventArgs<ProductInfo>> callback)
        {
            string relativeUrl = "/IMService/Product/CreateProduct";
            restClient.Create(relativeUrl, CovertVMtoEntity(data), callback);
        }

        private ProductInfo CovertVMtoEntity(ProductCreateSingleVM data)
        {
            var product = new ProductInfo();
            product.PromotionTitle = new LanguageContent(String.Empty);
            product.ProductConsignFlag = (VendorConsignFlag)data.ConsignFlag;
            product.ProductStatus = ProductStatus.InActive_UnShow;
            product.ProductPOInfo = new ProductPOInfo() { MinPackNumber = 1 };
            product.ProductPayType = ProductPayType.MoneyOnly;
            product.Merchant = new Merchant { SysNo = 1 };//泰隆优选商品
            product.OperateUser = new UserInfo
                                      {
                                          SysNo = CPApplication.Current.LoginUser.UserSysNo,
                                          UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                                      };
            product.CompanyCode = CPApplication.Current.CompanyCode;
            product.LanguageCode = ConstValue.BizLanguageCode;
            product.ProductPriceInfo = new ProductPriceInfo
                                       {
                                           BasicPrice = 999999,
                                           CashRebate = 0,
                                           CurrentPrice = 999999,
                                           DiscountAmount = 0,
                                           MaxCountPerDay = 999999,
                                           MinCountPerOrder = 1,
                                           PayType = ProductPayType.MoneyOnly,
                                           Point = 0,
                                           ProductRankPrice = new List<ProductRankPriceInfo>(),
                                           ProductWholeSalePriceInfo = new List<ProductWholeSalePriceInfo>(),
                                           UnitCost = 999999,
                                           UnitCostWithoutTax = 999999,
                                           VirtualPrice = Math.Round(Convert.ToDecimal(data.VirtualPrice), 2)
                                       };
            product.ProductWarrantyInfo = new ProductWarrantyInfo
                                          {
                                              HostWarrantyDay = 0,
                                              IsNoExtendWarranty = true,
                                              OfferVATInvoice = OfferVATInvoice.Yes,
                                              PartWarrantyDay = 0,
                                              ServiceInfo = String.Empty,
                                              ServicePhone = String.Empty,
                                              Warranty = new LanguageContent(String.Empty),
                                              WarrantyShow = WarrantyShow.No
                                          };

            product.ProductBasicInfo = new ProductBasicInfo
                                {
                                    Attention = new LanguageContent(String.Empty),
                                    IsAccessoryShow = ProductIsAccessoryShow.No,
                                    IsTakePicture = data.IsTakePictures ? ProductIsTakePicture.Yes : ProductIsTakePicture.No,
                                    IsVirtualPic = ProductIsVirtualPic.No,
                                    Keywords = new LanguageContent(data.KeyWord),
                                    LongDescription = new LanguageContent(String.Empty),
                                    Note = String.Empty,
                                    PackageList = new LanguageContent(String.Empty),
                                    Performance = String.Empty,
                                    PhotoDescription = new LanguageContent(String.Empty),
                                    ProductBrandInfo = new BrandInfo { SysNo = data.BrandSysNo },
                                    ProductBriefAddition = new LanguageContent(String.Empty),
                                    ProductBriefTitle = new LanguageContent(data.ProductTitle),
                                    ProductCategoryInfo = new CategoryInfo { SysNo = data.C3SysNo },
                                    ProductDimensionInfo =
                                        new ProductDimensionInfo { Height = 0, Length = 0, Weight = 0, Width = 0 },
                                    ProductInfoFinishStatus = ProductInfoFinishStatus.No,
                                    ProductLink = String.Empty,
                                    ProductModel = new LanguageContent(data.ProductModel),
                                    ProductProperties = new List<ProductProperty>(),
                                    ProductTitle = new LanguageContent(data.ProductTitle),
                                    ShortDescription = new LanguageContent(String.Empty),
                                    UPCCode = data.UPCCode,
                                    BMCode = data.BMCode,
                                    TradeType = TradeType.Internal//泰隆银行默认为国内商品
                                };
            ProductType productType;
            switch (data.ProductType)
            {
                case "1":
                    product.ProductBasicInfo.ProductType = ProductType.OpenBox;
                    break;
                case "2":
                    product.ProductBasicInfo.ProductType = ProductType.Bad;
                    break;
                case "3":
                    product.ProductBasicInfo.ProductType = ProductType.Virtual;
                    break;
                default:
                    product.ProductBasicInfo.ProductType = ProductType.Normal;
                    break;
            }
            //product.ProductBasicInfo.ProductType = Enum.TryParse(data.ProductType, out productType) ? productType : ProductType.Normal;
            product.OrginCode = data.OrginCode;
            return product;
        }


        /// <summary>
        /// 商品价格信息
        /// </summary>
        public ProductPriceInfo ProductPriceInfo { get; set; }

        public void GetProductCountryList(EventHandler<RestClientEventArgs<List<ProductCountry>>> callback)
        {
            restClient.Query("/IMService/Product/GetProductCountryList", callback);
        }
    }
}
