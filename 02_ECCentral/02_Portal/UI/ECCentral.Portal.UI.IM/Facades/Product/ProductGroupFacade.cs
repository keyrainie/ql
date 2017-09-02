using System;
using System.Linq;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM.Product;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Models.Product.ProductGroup;
using ECCentral.Service.IM.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductGroupFacade
    {

        #region 构造函数和字段

        private readonly RestClient _restClient;

        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductGroupFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductGroupFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }

        private const string QueryProductGroupInfoRelativeUrl = "/IMService/ProductGroup/QueryProductGroupInfo";
        private const string GetProductGroupInfoBySysNoRelativeUrl = "/IMService/ProductGroup/GetProductGroupInfoBySysNo";
        private const string CreateProductGroupInfoRelativeUrl = "/IMService/ProductGroup/CreateProductGroupInfo";
        private const string UpdateProductGroupInfoRelativeUrl = "/IMService/ProductGroup/UpdateProductGroupInfo";
        private const string GetCategorySettingRelativeUrl = "/IMService/CategoryKPI/GetCategorySettingBySysNo";
        private const string GetProductListRelativeUrl = "/IMService/Product/GetProductInfoListBySysNoList";
        private const string GetPropertyValueInfoByPropertySysNoListRelativeUrl = "/IMService/Property/GetPropertyValueInfoByPropertySysNoList";
        private const string CreateSimilarProductRelativeUrl = "/IMService/Product/CreateSimilarProduct";

        #endregion

        #region ServiceFacade

        public void GetProductGroupInfoBySysNo(int productGroupSysNo, EventHandler<RestClientEventArgs<ProductGroup>> callback)
        {
            _restClient.Query(GetProductGroupInfoBySysNoRelativeUrl, productGroupSysNo, callback);
        }

        public void CreateProductGroupInfo(ProductGroupMaintainVM data, EventHandler<RestClientEventArgs<ProductGroup>> callback)
        {
            _restClient.Create(CreateProductGroupInfoRelativeUrl, CovertProductGroupMaintainVMToProductGroupEntity(data), callback);
        }

        public void UpdateProductGroupInfo(ProductGroupMaintainVM data, EventHandler<RestClientEventArgs<ProductGroup>> callback)
        {
            _restClient.Update(UpdateProductGroupInfoRelativeUrl, CovertProductGroupMaintainVMToProductGroupEntity(data), callback);
        }

        private ProductGroup CovertProductGroupMaintainVMToProductGroupEntity(ProductGroupMaintainVM data)
        {
            var productGroup = new ProductGroup
            {
                ProductList = new List<ProductInfo>(),
                ProductGroupSettings = new List<ProductGroupSettings>(),
                SysNo = data.ProductGroupSysNo,
                ProductGroupName = new LanguageContent(data.BasicInfoVM.ProductGroupName),
                ProductGroupModel = new LanguageContent(data.BasicInfoVM.ProductGroupModel),
                OperateUser = new UserInfo
                                  {
                                      SysNo = CPApplication.Current.LoginUser.UserSysNo,
                                      UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                                  },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };

            data.ProductListVM.ProductGroupProductVMList.ForEach(product => productGroup.ProductList.Add(new ProductInfo
            {
                SysNo = product.ProductSysNo,
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            }));

            data.PropertyVM.ProductGroupSettings.Where(setting => setting.ProductGroupProperty.SysNo != 0).ForEach(setting =>
            {
                var productGroupSetting = new ProductGroupSettings
                {
                    ProductGroupProperty = new PropertyInfo
                    {
                        SysNo = setting.ProductGroupProperty.SysNo,
                    },
                    PropertyBriefName = new LanguageContent(setting.PropertyBriefName),
                    ImageShow = setting.ImageShow,
                    Polymeric = setting.Polymeric
                };
                productGroup.ProductGroupSettings.Add(productGroupSetting);
            });
            return productGroup;
        }

        public void CreateSimilarProduct(ProductGroupMaintainSimilarProductVM data, EventHandler<RestClientEventArgs<CreateSimilerProductRsp>> callback)
        {
            _restClient.Create(CreateSimilarProductRelativeUrl, CovertProductGroupMaintainSimilarProductVMToProductGroupEntity(data), callback);
        }

        private ProductGroup CovertProductGroupMaintainSimilarProductVMToProductGroupEntity(ProductGroupMaintainSimilarProductVM data)
        {
            var productGroup = new ProductGroup
            {
                SysNo = data.MainPageVM.ProductGroupSysNo,
                ProductList = new List<ProductInfo>(),
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };

            data.SelectedProduct.Where(p => p.IsChecked).ForEach(p =>
            {
                var product = new ProductInfo();
                product.PromotionTitle = new LanguageContent(String.Empty);
                product.ProductConsignFlag = VendorConsignFlag.Sell;
                product.ProductStatus = ProductStatus.InActive_UnShow;
                product.ProductPOInfo = new ProductPOInfo();
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
                    MaxCountPerDay = 1,
                    MinCountPerOrder = 1,
                    PayType = ProductPayType.MoneyOnly,
                    Point = 0,
                    ProductRankPrice = new List<ProductRankPriceInfo>(),
                    ProductWholeSalePriceInfo = new List<ProductWholeSalePriceInfo>(),
                    UnitCost = 999999,
                    UnitCostWithoutTax = 999999,
                    VirtualPrice = Math.Round(Convert.ToDecimal(p.VirtualPrice), 2)
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
                    IsTakePicture = ProductIsTakePicture.No,
                    IsVirtualPic = ProductIsVirtualPic.No,
                    Keywords = new LanguageContent(String.Empty),
                    LongDescription = new LanguageContent(String.Empty),
                    Note = String.Empty,
                    PackageList = new LanguageContent(String.Empty),
                    Performance = String.Empty,
                    PhotoDescription = new LanguageContent(String.Empty),
                    ProductBrandInfo = new BrandInfo { SysNo = data.MainPageVM.BasicInfoVM.ProductGroupBrand.SysNo },
                    ProductBriefAddition = new LanguageContent(String.Empty),
                    ProductBriefTitle = new LanguageContent(p.ProductTitle),
                    ProductCategoryInfo = new CategoryInfo { SysNo = data.MainPageVM.BasicInfoVM.ProductGroupCategory.SysNo },
                    ProductDimensionInfo = new ProductDimensionInfo { Height = 0, Length = 0, Weight = 0, Width = 0 },
                    ProductInfoFinishStatus = ProductInfoFinishStatus.No,
                    ProductLink = String.Empty,
                    ProductModel = new LanguageContent(p.ProductModel),
                    ProductProperties = ConvertPropertyValueListVMToProductPropertyList(p.PropertyValueList),
                    ProductTitle = new LanguageContent(p.ProductTitle),
                    ShortDescription = new LanguageContent(String.Empty),
                    UPCCode = p.UPCCode,
                    BMCode = p.BMCode,
                    ProductType = ProductType.Normal,
                    TradeType = TradeType.Internal//泰隆银行默认为国内商品
                };
                product.OrginCode = p.CountryCode;
                productGroup.ProductList.Add(product);
            });
            return productGroup;
        }

        private List<ProductProperty> ConvertPropertyValueListVMToProductPropertyList(List<PropertyValueVM> data)
        {
            var result = new List<ProductProperty>();
            data.ForEach(property => result.Add(new ProductProperty
            {
                Property = new PropertyValueInfo
                {
                    PropertyInfo = new PropertyInfo
                    {
                        SysNo = property.PropertySysNo,
                        PropertyName = new LanguageContent(property.PropertyName)
                    },
                    SysNo = property.SysNo,
                    ValueDescription = new LanguageContent(property.ValueDescription)
                },
                PersonalizedValue = new LanguageContent(String.Empty),
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            }));
            return result;
        }

        public void GetCategorySetting(int categorySysNo, EventHandler<RestClientEventArgs<CategorySetting>> callback)
        {
            _restClient.Query(GetCategorySettingRelativeUrl, categorySysNo, callback);
        }

        public void GetProductList(List<int> productSysNoList, EventHandler<RestClientEventArgs<List<ProductInfo>>> callback)
        {
            _restClient.Query(GetProductListRelativeUrl, productSysNoList, callback);
        }

        public void GetPropertyValueInfoByPropertySysNoList(List<int> propertySysNoList, EventHandler<RestClientEventArgs<Dictionary<int, List<PropertyValueInfo>>>> callback)
        {
            _restClient.Query(GetPropertyValueInfoByPropertySysNoListRelativeUrl, propertySysNoList, callback);
        }

        public void QueryProductGroupInfo(ProductGroupQueryVM model, int pageSize, int pageIndex, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductGroupQueryFilter filter = model.ConvertVM<ProductGroupQueryVM, ProductGroupQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                SortBy = sortField
            };

            _restClient.QueryDynamicData(QueryProductGroupInfoRelativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        #endregion

        #region Convert Entity To VM

        public ProductGroupMaintainBasicInfoVM ConvertProductGroupEntityToProductGroupMaintainBasicInfoVM(ProductGroup data)
        {

                
                var vm = new ProductGroupMaintainBasicInfoVM
                {
                    ProductGroupName = data.ProductGroupName == null ? "" : data.ProductGroupName.Content
                };
                    var product = data.ProductList.First();
                    if (product != null)
                    {
                        vm.ProductGroupCategory = new CategoryVM
                        {
                            SysNo = product.ProductBasicInfo.ProductCategoryInfo.SysNo,
                            CategoryName = product.ProductBasicInfo.ProductCategoryInfo.CategoryName.Content
                        };
                        vm.ProductGroupBrand = new BrandVM
                        {
                            SysNo = product.ProductBasicInfo.ProductBrandInfo.SysNo,
                            BrandNameLocal = product.ProductBasicInfo.ProductBrandInfo.BrandNameLocal.Content
                        };
                    }
               
                vm.ProductGroupModel = data.ProductGroupModel.Content;
                return vm;
            
           
         
        }

        public ProductGroupMaintainProductListVM ConvertProductGroupEntityToProductGroupMaintainProductListVM(ProductGroup data)
        {
            var vm = new ProductGroupMaintainProductListVM
            {
                ProductGroupProductVMList = new List<ProductGroupProductVM>()
            };
            data.ProductList.ForEach(productInfo =>
            {
                var product = new ProductGroupProductVM
                {
                    ProductSysNo = productInfo.SysNo,
                    ProductID = productInfo.ProductID,
                    ProductTitle = productInfo.ProductBasicInfo.ProductTitle.Content,
                    ProductModel = productInfo.ProductBasicInfo.ProductModel.Content,
                    ProductBrand = new BrandVM
                    {
                        SysNo = productInfo.ProductBasicInfo.ProductBrandInfo.SysNo,
                        BrandNameLocal = productInfo.ProductBasicInfo.ProductBrandInfo.BrandNameLocal.Content
                    },
                    ProductCategory = new CategoryVM
                    {
                        SysNo = productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo,
                        CategoryName = productInfo.ProductBasicInfo.ProductCategoryInfo.CategoryName.Content
                    },
                    ProductStatus = productInfo.ProductStatus,
                    ProductCurrentPrice = productInfo.ProductPriceInfo.CurrentPrice.Round(2)
                };
                vm.ProductGroupProductVMList.Add(product);
            });
            return vm;
        }

        public ProductGroupProductVM ConvertProductInfoEntityToProductGroupProductVM(ProductInfo data)
        {
            var product = new ProductGroupProductVM
            {
                ProductSysNo = data.SysNo,
                ProductID = data.ProductID,
                ProductTitle = data.ProductBasicInfo.ProductTitle.Content,
                ProductModel = data.ProductBasicInfo.ProductModel.Content,
                ProductBrand = new BrandVM
                {
                    SysNo = data.ProductBasicInfo.ProductBrandInfo.SysNo,
                    BrandNameLocal = data.ProductBasicInfo.ProductBrandInfo.BrandNameLocal.Content
                },
                ProductCategory = new CategoryVM
                {
                    SysNo = data.ProductBasicInfo.ProductCategoryInfo.SysNo,
                    CategoryName = data.ProductBasicInfo.ProductCategoryInfo.CategoryName.Content
                },
                ProductStatus = data.ProductStatus,
                ProductCurrentPrice = data.ProductPriceInfo.CurrentPrice.Round(2)
            };
            return product;
        }

        public ProductGroupMaintainPropertySettingVM ConvertProductGroupEntityToProductGroupMaintainPropertySettingVM(ProductGroup data)
        {
            var vm = new ProductGroupMaintainPropertySettingVM { ProductGroupSettings = new List<ProductGroupSettingVM>() };

            ProductGroupMaintainPropertySettingVM localVM = vm;

            data.ProductGroupSettings.ForEach(settingInfo =>
            {
                var setting = new ProductGroupSettingVM
                {
                    ProductGroupProperty = new PropertyVM()
                };
                if (settingInfo.ProductGroupProperty.SysNo.HasValue)
                {
                    setting.ProductGroupProperty.SysNo = settingInfo.ProductGroupProperty.SysNo.Value;
                    setting.ProductGroupProperty.PropertyName = settingInfo.ProductGroupProperty.PropertyName.Content;
                    setting.PropertyBriefName = settingInfo.PropertyBriefName.Content;
                    setting.ImageShow = settingInfo.ImageShow;
                    setting.Polymeric = settingInfo.Polymeric;
                    localVM.ProductGroupSettings.Add(setting);
                }
            });

            if (vm.ProductGroupSettings.Count == 0)
            {
                vm = new ProductGroupMaintainPropertySettingVM();
            }

            if (vm.ProductGroupSettings.Count == 1)
            {
                vm.ProductGroupSettings.Add(new ProductGroupSettingVM());
            }

            return vm;
        }

        public List<PropertyVM> ConvertCategoryPropertyListToPropertyVMList(IEnumerable<CategoryProperty> categoryPropertyList)
        {
            var propertyVMList = new List<PropertyVM>();

            categoryPropertyList.ForEach(categoryProperty =>
            {
                var propertyVM = new PropertyVM();
                if (categoryProperty.Property.SysNo.HasValue)
                {
                    propertyVM.SysNo = categoryProperty.Property.SysNo.Value;
                    propertyVM.PropertyName = categoryProperty.Property.PropertyName.Content;
                }
                propertyVMList.Add(propertyVM);
            });
            return propertyVMList;
        }

        #endregion
    }
}
