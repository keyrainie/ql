using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.IM.Product;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Service.IM.Restful.RequestMsg;
using ECCentral.Service.IM.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Text;


namespace ECCentral.Portal.UI.IM.Facades.Product
{
    public class ProductFacade
    {
        #region 构造函数和字段

        private readonly RestClient _restClient;
        private const string GetProductRelativeUrl = "/IMService/Product/GetProductInfo";
        private const string GetProductByIDRelativeUrl = "/IMService/Product/GetProductInfoByID";
        private const string GetProductGroupRelativeUrl = "/IMService/Product/GetProductGroup";
        private const string ProductOnSaleRelativeUrl = "/IMService/Product/ProductOnSale";
        private const string ProductBatchOnSaleRelativeUrl = "/IMService/Product/ProductBatchOnSale";
        private const string ProductOnShowRelativeUrl = "/IMService/Product/ProductOnShow";
        private const string ProductUnShowRelativeUrl = "/IMService/Product/ProductUnShow";
        private const string ProductInvalidRelativeUrl = "/IMService/Product/ProductInvalid";
        private const string GetPropertyValueListRelativeUrl = "/IMService/Property/GetPropertyValueInfoByPropertySysNoList";
        private const string UpdateProductBasicInfoRelativeUrl = "/IMService/Product/UpdateProductBasicInfo";
        private const string BatchUpdateProductBasicInfoRelativeUrl = "/IMService/Product/BatchUpdateProductBasicInfo";
        private const string UpdateProductDescriptionInfoRelativeUrl = "/IMService/Product/UpdateProductDescriptionInfo";
        private const string BatchUpdateProductDescriptionInfoRelativeUrl = "/IMService/Product/BatchUpdateProductDescriptionInfo";
        private const string UpdateProductAccessoryInfoRelativeUrl = "/IMService/Product/UpdateProductAccessoryInfo";
        private const string BatchUpdateProductAccessoryInfoRelativeUrl = "/IMService/Product/BatchUpdateProductAccessoryInfo";
        private const string UpdateProductImageInfoRelativeUrl = "/IMService/Product/UpdateProductImageInfo";
        private const string UpdateProductPriceInfoRelativeUrl = "/IMService/Product/UpdateProductPriceInfo";
        private const string UpdateProductAutoPriceInfoRelativeUrl = "/IMService/Product/UpdateProductAutoPriceInfo";
        private const string AuditRequestProductPriceRelativeUrl = "/IMService/Product/AuditRequestProductPrice";
        private const string CancelAuditProductPriceRequestRelativeUrl = "/IMService/Product/CancelAuditProductPriceRequest";
        private const string UpdateProductPropertyInfoRelativeUrl = "/IMService/Product/UpdateProductPropertyInfo";
        private const string BatchUpdateProductPropertyInfoRelativeUrl = "/IMService/Product/BatchUpdateProductPropertyInfo";
        private const string UpdateProductWarrantyInfoRelativeUrl = "/IMService/Product/UpdateProductWarrantyInfo";
        private const string BatchUpdateProductWarrantyInfoRelativeUrl = "/IMService/Product/BatchUpdateProductWarrantyInfo";
        private const string UpdateProductDimensionInfoRelativeUrl = "/IMService/Product/UpdateProductDimensionInfo";
        private const string BatchUpdateProductDimensionInfoRelativeUrl = "/IMService/Product/BatchUpdateProductDimensionInfo";
        private const string UpdateProductSalesAreaInfoRelativeUrl = "/IMService/Product/UpdateProductSalesAreaInfo";
        private const string UpdateProductPurchaseInfoRelativeUrl = "/IMService/Product/UpdateProductPurchaseInfo";
        //private const string UpdateProductThirdPartyInventoryRelativeUrl = "/IMService/Product/UpdateProductThirdPartyInventory";
        private const string ProductCloneRelativeUrl = "/IMService/Product/ProductClone";
        private const string ProductRmaPolicyRelativeUrl = "/IMService/Product/GetProductRMAPolicyByProductSysNo";
        private const string UpdateProductRMAPolicyRelativeUrl = "/IMService/Product/UpdateProductRMAPolicy";
        private const string ProductBatchEntryRelativeUrl = "/IMService/Product/ProductBatchEntry";
        private const string ProductBatchAuditRelativeUrl = "/IMService/Product/ProductBatchAudit";

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }

        #endregion

        #region 函数

        #region ServiceFacade

        public void GetProductInfo(int productSysNo, EventHandler<RestClientEventArgs<ProductInfo>> callback)
        {
            _restClient.Query(GetProductRelativeUrl, productSysNo, callback);
        }

        public void GetProductInfo(string productID, EventHandler<RestClientEventArgs<ProductInfo>> callback)
        {
            _restClient.Query(GetProductByIDRelativeUrl, productID, callback);
        }

        public void GetProductGroup(int productSysNo, EventHandler<RestClientEventArgs<ProductGroup>> callback)
        {
            _restClient.Query(GetProductGroupRelativeUrl, productSysNo, callback);
        }

        public void ProductOnSale(int productSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            _restClient.Update(ProductOnSaleRelativeUrl, productSysNo, callback);
        }

        public void ProductBatchOnSale(List<int> productSysNoList, EventHandler<RestClientEventArgs<Dictionary<int, String>>> callback)
        {
            _restClient.Update(ProductBatchOnSaleRelativeUrl, productSysNoList, callback);
        }

        public void ProductOnShow(int productSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            _restClient.Update(ProductOnShowRelativeUrl, productSysNo, callback);
        }

        public void ProductUnShow(int productSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            _restClient.Update(ProductUnShowRelativeUrl, productSysNo, callback);
        }

        public void ProductInvalid(int productSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            _restClient.Update(ProductInvalidRelativeUrl, productSysNo, callback);
        }

        public void GetPropertyValueList(List<int> propertySysNoList, EventHandler<RestClientEventArgs<Dictionary<int, List<PropertyValueInfo>>>> callback)
        {
            _restClient.Query(GetPropertyValueListRelativeUrl, propertySysNoList, callback);
        }

        #region 更新商品基本信息TAB

        public void UpdateProductBasicInfo(ProductVM vm, EventHandler<RestClientEventArgs<UpdateProductBasicInfoRsp>> callback)
        {
            var productInfo = BuildProductBasicInfo(vm);
            _restClient.Update(UpdateProductBasicInfoRelativeUrl, productInfo, callback);
        }

        public void BatchUpdateProductBasicInfo(ProductVM vm, EventHandler<RestClientEventArgs<UpdateProductBasicInfoRsp>> callback)
        {
            var productBatchUpdateRequestMsg = new ProductBatchUpdateRequestMsg
            {
                ProductInfo = BuildProductBasicInfo(vm),
                BatchUpdateProductSysNoList = vm.ProductMaintainCommonInfo.GroupProductList
                                                .Where(p => p.IsChecked).Select(p => p.ProductSysNo).ToList()
            };
            _restClient.Update(BatchUpdateProductBasicInfoRelativeUrl, productBatchUpdateRequestMsg, callback);
        }
        private ProductInfo BuildProductBasicInfo(ProductVM vm)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductBasicInfo = new ProductBasicInfo
                {
                    ProductTitle = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.ProductTitle)
                },
                PromotionTitle = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.PromotionTitle)
            };
            productInfo.ProductBasicInfo.ProductBriefTitle = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.ProductBriefTitle);
            productInfo.ProductBasicInfo.ProductBriefAddition = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.ProductBriefAddition);
            productInfo.ProductBasicInfo.Keywords = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.Keywords);
            productInfo.ProductBasicInfo.ShoppingGuideURL = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.ShoppingGuideURL;
            if (!string.IsNullOrEmpty(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.SafeQty))
            {
                productInfo.ProductBasicInfo.SafeQty = int.Parse(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.SafeQty);
            }
            productInfo.ProductBasicInfo.StoreType = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.StoreType;

            productInfo.ProductBasicInfo.TradeType = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TradeType;
            productInfo.ProductBasicInfo.ProductCategoryInfo = new CategoryInfo
            {
                SysNo = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.CategoryInfo.SysNo
            };
            productInfo.ProductBasicInfo.ProductBrandInfo = new BrandInfo
            {
                SysNo = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.BrandInfo.SysNo
            };
            productInfo.ProductBasicInfo.ProductCategoryInfo = new CategoryInfo
            {
                SysNo = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.CategoryInfo.SysNo
            };
            productInfo.ProductBasicInfo.ProductModel = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.ProductModel);
            productInfo.ProductBasicInfo.ProductType = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.ProductType;
            productInfo.ProductBasicInfo.Note = vm.Note;
            productInfo.ProductBasicInfo.ProductInfoFinishStatus = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoOther.ProductInfoFinishStatus;
            productInfo.ProductBasicInfo.ProductManager = new ProductManagerInfo
            {
                SysNo = Convert.ToInt32(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.ProductManagerInfo.SysNo)
            };
            productInfo.ProductWarrantyInfo = new ProductWarrantyInfo
            {
                IsNoExtendWarranty = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoOther.NoExtendWarranty
            };
            productInfo.ProductConsignFlag = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.ProductConsignFlag;
            productInfo.ProductBasicInfo.IsTakePicture = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.ProductIsTakePicture;
            productInfo.ProductTimelyPromotionTitle = new List<ProductTimelyPromotionTitle>();
            if (!String.IsNullOrEmpty(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.NormalPromotionTitle))
            {
                var normalPromotionTitle = new ProductTimelyPromotionTitle
                {
                    PromotionTitle = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.NormalPromotionTitle),
                    PromotionTitleType = "Normal",
                    Status = TimelyPromotionTitleStatus.Active
                };
                productInfo.ProductTimelyPromotionTitle.Add(normalPromotionTitle);
            }

            if (!String.IsNullOrEmpty(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TimelyPromotionTitle))
            {
                var timelyPromotionTitle = new ProductTimelyPromotionTitle
                {
                    PromotionTitle = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TimelyPromotionTitle),
                    PromotionTitleType = "CountDown",
                    BeginDate = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TimelyPromotionBeginDate,
                    EndDate = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TimelyPromotionEndDate,
                    Status = TimelyPromotionTitleStatus.Original
                };
                productInfo.ProductTimelyPromotionTitle.Add(timelyPromotionTitle);
            }

            productInfo.ProductBasicInfo.ShortDescription = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDescriptionInfo.ProductDescription);
            productInfo.ProductBasicInfo.PackageList = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDescriptionInfo.PackageList);
            productInfo.ProductBasicInfo.ProductLink = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDescriptionInfo.ProductLink;
            productInfo.ProductBasicInfo.Attention = new LanguageContent(vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDescriptionInfo.Attention);
            productInfo.ProductBasicInfo.TaxNo = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TaxNo;
            productInfo.ProductBasicInfo.TariffPrice = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.TariffPrice;
            productInfo.ProductBasicInfo.EntryRecord = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoDisplayInfo.EntryRecord;

            productInfo.ProductBasicInfo.UPCCode = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.UPCCode;
            productInfo.ProductBasicInfo.BMCode = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoSpecificationInfo.BMCode;

            productInfo.Merchant = new Merchant
            {
                SysNo = vm.ProductMaintainBasicInfo.ProductMaintainBasicInfoChannelInfo.SellerSysNo
            };
            productInfo.OperateUser = new UserInfo
            {
                SysNo = CPApplication.Current.LoginUser.UserSysNo,
                UserDisplayName = CPApplication.Current.LoginUser.DisplayName
            };
            productInfo.CompanyCode = CPApplication.Current.CompanyCode;
            productInfo.LanguageCode = ConstValue.BizLanguageCode;
            return productInfo;
        }

        public void UpdateProductVirtualPrice(string productSysNo, string virtualPrice, EventHandler<RestClientEventArgs<object>> callback)
        {
            ProductVirtualPriceReq req = new ProductVirtualPriceReq()
            {
                ProductSysNo = productSysNo,
                VirtualPrice = virtualPrice
            };

            _restClient.Update("/IMService/Product/UpdateProductVirtualPrice", req, callback);
        }

        #endregion

        #region 更新商品描述信息TAB

        public void UpdateProductDescriptionInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = BuildProductDescriptionInfo(vm);
            _restClient.Update(UpdateProductDescriptionInfoRelativeUrl, productInfo, callback);
        }

        public void BatchUpdateProductDescriptionInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productBatchUpdateRequestMsg = new ProductBatchUpdateRequestMsg
            {
                ProductInfo = BuildProductDescriptionInfo(vm),
                BatchUpdateProductSysNoList = vm.ProductMaintainCommonInfo.GroupProductList
                                                .Where(p => p.IsChecked).Select(p => p.ProductSysNo).ToList()
            };

            _restClient.Update(BatchUpdateProductDescriptionInfoRelativeUrl, productBatchUpdateRequestMsg, callback);
        }

        private ProductInfo BuildProductDescriptionInfo(ProductVM vm)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductBasicInfo = new ProductBasicInfo
                {
                    LongDescription = new LanguageContent(vm.ProductMaintainDescription.ProductLongDescription),
                    PhotoDescription = new LanguageContent(vm.ProductMaintainDescription.ProductPhotoDescription)
                },
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };
            return productInfo;
        }

        #endregion

        #region 更新商品配件信息TAB

        public void UpdateProductAccessoryInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = BuildProductAccessoryInfo(vm);
            _restClient.Update(UpdateProductAccessoryInfoRelativeUrl, productInfo, callback);
        }

        public void BatchUpdateProductAccessoryInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productBatchUpdateRequestMsg = new ProductBatchUpdateRequestMsg
            {
                ProductInfo = BuildProductAccessoryInfo(vm),
                BatchUpdateProductSysNoList = vm.ProductMaintainCommonInfo.GroupProductList
                                                .Where(p => p.IsChecked).Select(p => p.ProductSysNo).ToList()
            };

            _restClient.Update(BatchUpdateProductAccessoryInfoRelativeUrl, productBatchUpdateRequestMsg, callback);
        }

        private ProductInfo BuildProductAccessoryInfo(ProductVM vm)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductBasicInfo = new ProductBasicInfo
                {
                    ProductAccessories = new List<ProductAccessory>(),
                    IsAccessoryShow = vm.ProductMaintainAccessory.IsAccessoryShow
                },
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };

            vm.ProductMaintainAccessory.ProductAccessoryList.ForEach(productAccessory =>
            {
                var pa = new ProductAccessory
                {
                    AccessoryInfo = new AccessoryInfo
                    {
                        SysNo = productAccessory.Accessory.SysNo,
                        AccessoryID = productAccessory.Accessory.AccessoryID,
                        AccessoryName = new LanguageContent(productAccessory.Accessory.AccessoryName)
                    },
                    Status = productAccessory.IsShow,
                    Qty = Convert.ToInt32(productAccessory.Qty),
                    Description = new LanguageContent(productAccessory.Description),
                    Priority = Convert.ToInt32(productAccessory.Priority),
                    CompanyCode = CPApplication.Current.CompanyCode,
                    LanguageCode = ConstValue.BizLanguageCode
                };
                productInfo.ProductBasicInfo.ProductAccessories.Add(pa);
            });
            return productInfo;
        }

        #endregion

        #region 更新商品图片信息TAB

        public void UpdateProductImageInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = BuildProductImageInfo(vm);
            _restClient.Update(UpdateProductImageInfoRelativeUrl, productInfo, callback);
        }

        private ProductInfo BuildProductImageInfo(ProductVM vm)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductBasicInfo = new ProductBasicInfo
                {
                    ProductResources = new List<ProductResourceForNewegg>(),
                    IsVirtualPic = vm.ProductMaintainImage.IsVirtualPic
                },
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName =
                        CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };

            vm.ProductMaintainImage.ProductImageList.ForEach(productImage =>
            {
                var pr = new ProductResourceForNewegg
                {
                    Resource = new ResourceForNewegg
                    {
                        ResourceSysNo = productImage.ResourceSysNo,
                        Priority = productImage.Priority
                    },
                    IsShow = productImage.IsShow,
                    OperateUser = new UserInfo
                    {
                        SysNo = CPApplication.Current.LoginUser.UserSysNo,
                        UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                    },
                    CompanyCode = CPApplication.Current.CompanyCode,
                    LanguageCode = ConstValue.BizLanguageCode
                };
                productInfo.ProductBasicInfo.ProductResources.Add(pr);
            });
            return productInfo;
        }

        #endregion

        #region 更新商品价格信息TAB

        public void UpdateProductPriceInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductPriceRequest = BuildProductPriceRequestInfo(vm),
                ProductBasicInfo = new ProductBasicInfo
                {
                    Note = vm.Note
                },
                ProductPriceInfo = new ProductPriceInfo
                {
                    IsSyncShopPrice = vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.IsSyncShopPrice ? IsSyncShopPrice.YES : IsSyncShopPrice.NO
                },
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                }
            };
            _restClient.Update(UpdateProductPriceInfoRelativeUrl, productInfo, callback);
        }

        public void UpdateProductAutoPriceInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                AutoAdjustPrice = new AutoAdjustPrice
                {
                    IsAutoAdjustPrice = IsAutoAdjustPrice.No, //vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoAutoPrice.IsAutoAdjustPrice,
                    NotAutoPricingBeginDate = vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoAutoPrice.NotAutoAdjustPriceBeginDate,
                    NotAutoPricingEndDate = vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoAutoPrice.NotAutoAdjustPriceEndDate
                },
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                }
            };
            _restClient.Update(UpdateProductAutoPriceInfoRelativeUrl, productInfo, callback);
        }

        public void AuditProductPriceInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductPriceRequest = BuildProductPriceRequestInfo(vm)
            };
            _restClient.Update(AuditRequestProductPriceRelativeUrl, productInfo, callback);
        }

        public void CancelAuditProductPriceInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };

            _restClient.Update(CancelAuditProductPriceRequestRelativeUrl, productInfo, callback);
        }

        private ProductPriceRequestInfo BuildProductPriceRequestInfo(ProductVM vm)
        {
            var request = new ProductPriceRequestInfo
            {
                BasicPrice = Math.Round(Convert.ToDecimal(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.BasicPrice), 2),
                CurrentPrice = ((decimal?)new NullableConverter<Decimal>()
                .ConvertFromString(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.RequestCurrentPrice)).Round(2),
                CashRebate = ((decimal?)new NullableConverter<Decimal>()
                .ConvertFromString(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.RequestCashRebate)).Round(2),
                Point = ((int?)new NullableConverter<Int32>()
                .ConvertFromString(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.RequestPoint)),
                PayType = vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.PayType,
                MaxCountPerDay = Convert.ToInt32(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.MaxCountPerDay),
                MinCountPerOrder = Convert.ToInt32(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.MinCountPerOrder),
                MinCommission = Math.Round(Convert.ToDecimal(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.MinCommission), 2),
                VirtualPrice = Math.Round(Convert.ToDecimal(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.VirtualPrice), 2),
                ProductRankPrice = (vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoRankPrice.ProductRankPriceList
                .Select(rankPrice => new ProductRankPriceInfo
                {
                    Rank = rankPrice.CustomerRank,
                    RankPrice = ((decimal?)new NullableConverter<Decimal>().ConvertFromString(rankPrice.RequestRankPrice)).Round(2),
                    Status = rankPrice.Status
                })).ToList(),
                ProductWholeSalePriceInfo = (vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoVolumePrice.ProductVolumePriceList
                .Where(volumePrice => volumePrice.IsChecked)
                .Select(volumePrice => new ProductWholeSalePriceInfo
                {
                    Level = volumePrice.Level,
                    Qty = ((int?)new NullableConverter<Int32>().ConvertFromString(volumePrice.VolumePriceRequestQty)),
                    Price = ((decimal?)new NullableConverter<Decimal>().ConvertFromString(volumePrice.VolumePriceRequestPrice)).Round(2)
                })).ToList(),
                AlipayVipPrice = ((decimal?)new NullableConverter<Decimal>()
                .ConvertFromString(vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoVIPPrice.RequestAlipayVIPPrice)).Round(2),
                IsUseAlipayVipPrice = vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoVIPPrice.IsUseAlipayVipPrice,
                PMMemo = vm.ProductMaintainPriceInfo.ProductMaintainPriceInfoAuditMemo.PMMemo,
                CreateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                }
            };
            return request;
        }

        #endregion

        #region 更新商品属性信息TAB

        public void UpdateProductPropertyInfo(ProductVM vm, EventHandler<RestClientEventArgs<String>> callback)
        {
            var productInfo = BuildProductPropertyInfo(vm);
            _restClient.Update(UpdateProductPropertyInfoRelativeUrl, productInfo, callback);
        }

        public void BatchUpdateProductPropertyInfo(ProductVM vm, EventHandler<RestClientEventArgs<String>> callback)
        {
            var productBatchUpdateRequestMsg = new ProductBatchUpdateRequestMsg
            {
                ProductInfo = BuildProductPropertyInfo(vm),
                BatchUpdateProductSysNoList = vm.ProductMaintainCommonInfo.GroupProductList
                                                .Where(p => p.IsChecked).Select(p => p.ProductSysNo).ToList()
            };

            _restClient.Update(BatchUpdateProductPropertyInfoRelativeUrl, productBatchUpdateRequestMsg, callback);
        }

        private ProductInfo BuildProductPropertyInfo(ProductVM vm)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductBasicInfo = new ProductBasicInfo
                {
                    ProductProperties = new List<ProductProperty>()
                },
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };

            vm.ProductMaintainProperty.ProductPropertyValueList.ForEach(p =>
            {
                var productProperty = new ProductProperty
                {
                    Property = new PropertyValueInfo
                    {
                        PropertyInfo = new PropertyInfo
                        {
                            SysNo = p.Property.SysNo,
                            PropertyName = new LanguageContent(p.Property.PropertyName)
                        },
                        SysNo = p.ProductPropertyValue.SysNo,
                        ValueDescription = new LanguageContent((string.IsNullOrWhiteSpace(p.ProductPropertyValue.ValueDescription) ? "" : p.ProductPropertyValue.ValueDescription.Replace("'", "‘")))
                    },
                    PropertyGroup = new PropertyGroupInfo
                    {
                        SysNo = p.PropertyGroupInfo.SysNo,
                        PropertyGroupName = new LanguageContent(p.PropertyGroupInfo.PropertyGroupName)
                    },
                    PersonalizedValue = new LanguageContent(string.IsNullOrWhiteSpace(p.PersonalizedValue) ? "" : p.PersonalizedValue.Replace("'", "‘"))
                };
                productInfo.ProductBasicInfo.ProductProperties.Add(productProperty);
            });
            return productInfo;
        }

        #endregion

        #region 更新商品质保信息TAB

        public void UpdateProductWarrantyInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = BuildProductWarrantyInfo(vm);
            _restClient.Update(UpdateProductWarrantyInfoRelativeUrl, productInfo, callback);
        }

        public void BatchUpdateProductWarrantyInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productBatchUpdateRequestMsg = new ProductBatchUpdateRequestMsg
            {
                ProductInfo = BuildProductWarrantyInfo(vm),
                BatchUpdateProductSysNoList = vm.ProductMaintainCommonInfo.GroupProductList
                                                .Where(p => p.IsChecked).Select(p => p.ProductSysNo).ToList()
            };

            _restClient.Update(BatchUpdateProductWarrantyInfoRelativeUrl, productBatchUpdateRequestMsg, callback);
        }

        private ProductInfo BuildProductWarrantyInfo(ProductVM vm)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductWarrantyInfo = new ProductWarrantyInfo
                {
                    HostWarrantyDay = vm.ProductMaintainWarranty.HostWarrantyDay,
                    PartWarrantyDay = vm.ProductMaintainWarranty.PartWarrantyDay,
                    Warranty = new LanguageContent(vm.ProductMaintainWarranty.Warranty),
                    ServicePhone = vm.ProductMaintainWarranty.ServicePhone,
                    ServiceInfo = vm.ProductMaintainWarranty.ServiceInfo,
                    OfferVATInvoice = vm.ProductMaintainWarranty.OfferVATInvoice,
                    WarrantyShow = vm.ProductMaintainWarranty.WarrantyShow
                },
                ProductBasicInfo = new ProductBasicInfo
                {
                    Note = vm.Note
                },
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };
            return productInfo;
        }

        #endregion

        #region 更新商品重量信息TAB

        public void UpdateProductDimensionInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = BuildProductDimensionInfo(vm);
            _restClient.Update(UpdateProductDimensionInfoRelativeUrl, productInfo, callback);
        }

        public void BatchUpdateProductDimensionInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productBatchUpdateRequestMsg = new ProductBatchUpdateRequestMsg
            {
                ProductInfo = BuildProductDimensionInfo(vm),
                BatchUpdateProductSysNoList = vm.ProductMaintainCommonInfo.GroupProductList
                                                .Where(p => p.IsChecked).Select(p => p.ProductSysNo).ToList()
            };

            _restClient.Update(BatchUpdateProductDimensionInfoRelativeUrl, productBatchUpdateRequestMsg, callback);
        }

        private ProductInfo BuildProductDimensionInfo(ProductVM vm)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductBasicInfo = new ProductBasicInfo
                {
                    ProductDimensionInfo = new ProductDimensionInfo
                    {
                        Weight = Math.Round(Convert.ToDecimal(vm.ProductMaintainDimension.Weight), 2),
                        LargeFlag = vm.ProductMaintainDimension.LargeFlag,
                        Length = Math.Round(Convert.ToDecimal(vm.ProductMaintainDimension.Length), 2),
                        Width = Math.Round(Convert.ToDecimal(vm.ProductMaintainDimension.Width), 2),
                        Height = Math.Round(Convert.ToDecimal(vm.ProductMaintainDimension.Height), 2)
                    }
                },
                OperateUser = new UserInfo
                    {
                        SysNo = CPApplication.Current.LoginUser.UserSysNo,
                        UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                    },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };
            return productInfo;
        }

        #endregion

        #region 更新商品销售区域信息TAB

        public void UpdateProductSalesAreaInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = BuildProductSalesAreaInfo(vm);
            _restClient.Update(UpdateProductSalesAreaInfoRelativeUrl, productInfo, callback);
        }

        private ProductInfo BuildProductSalesAreaInfo(ProductVM vm)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductSalesAreaInfoList = new List<ProductSalesAreaInfo>(),
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };

            vm.ProductMaintainSalesArea.ProductMaintainSalesAreaSaveList.ForEach(productSalesAreaSave =>
            {
                var productSalesArea = new ProductSalesAreaInfo
                {
                    Stock = new StockInfo
                    {
                        SysNo = productSalesAreaSave.StockSysNo,
                        StockName = productSalesAreaSave.StockName
                    },
                    Province = new AreaInfo
                    {
                        ProvinceSysNo = productSalesAreaSave.Province.ProvinceSysNo,
                        ProvinceName = productSalesAreaSave.Province.ProvinceName,
                        CitySysNo = productSalesAreaSave.City.CitySysNo,
                        CityName = productSalesAreaSave.City.CityName
                    },
                    CompanyCode = CPApplication.Current.CompanyCode,
                    LanguageCode = ConstValue.BizLanguageCode
                };
                productInfo.ProductSalesAreaInfoList.Add(productSalesArea);
            });
            return productInfo;
        }

        #endregion

        #region 更新商品采购相关信息TAB

        public void UpdateProductPurchaseInfo(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var productInfo = new ProductInfo
            {
                SysNo = vm.ProductSysNo,
                ProductPOInfo = new ProductPOInfo
                {
                    MinPackNumber = Convert.ToInt32(vm.ProductMaintainPurchaseInfo.MinPackCount),
                    POMemo = vm.ProductMaintainPurchaseInfo.POMemo,
                    InventoryType = vm.ProductMaintainPurchaseInfo.InventoryType,
                    ERPProductID = vm.ProductMaintainPurchaseInfo.ERPProductID
                },
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                },
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode
            };
            _restClient.Update(UpdateProductPurchaseInfoRelativeUrl, productInfo, callback);
        }

        #endregion

        #region 更新商品库存同步合作信息TAB

        //public void UpdateProductThirdPartyInventory(ProductVM vm, EventHandler<RestClientEventArgs<object>> callback)
        //{
        //    var productInfo = BuildProductThirdPartyInventory(vm);
        //    _restClient.Update(UpdateProductThirdPartyInventoryRelativeUrl, productInfo, callback);
        //}

        //private ProductInfo BuildProductThirdPartyInventory(ProductVM vm)
        //{
        //    var productInfo = new ProductInfo
        //    {
        //        SysNo = vm.ProductSysNo,
        //        ProductMappingList = new List<ProductMapping>
        //        {
        //            new ProductMapping
        //            {
        //                ThirdPartner = vm.ProductMaintainThirdPartyInventory.ThirdPartner,
        //                SynProductID = vm.ProductMaintainThirdPartyInventory.SynProductID,
        //                StockRules = vm.ProductMaintainThirdPartyInventory.StockRules,
        //                LimitQty = Convert.ToInt32(vm.ProductMaintainThirdPartyInventory.LimitCount),
        //                Status = vm.ProductMaintainThirdPartyInventory.Status
        //            }
        //        },
        //        OperateUser = new UserInfo
        //        {
        //            SysNo = CPApplication.Current.LoginUser.UserSysNo,
        //            UserDisplayName = CPApplication.Current.LoginUser.DisplayName
        //        },
        //        CompanyCode = CPApplication.Current.CompanyCode,
        //        LanguageCode = ConstValue.BizLanguageCode
        //    };
        //    return productInfo;
        //}

        #endregion

        #region 商品克隆

        public void ProductClone(List<String> productIDList, ProductCloneType productCloneType, EventHandler<RestClientEventArgs<ProductCloneRsp>> callback)
        {
            var productCloneRequest = new ProductCloneRequestMsg
            {
                ProductIDList = productIDList,
                CloneType = productCloneType,
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = ConstValue.BizLanguageCode,
                OperateUser = new UserInfo
                {
                    SysNo = CPApplication.Current.LoginUser.UserSysNo,
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName
                }
            };
            _restClient.Create(ProductCloneRelativeUrl, productCloneRequest, callback);
        }

        #endregion

        #endregion

        #region ConvertEntityToVM

        #region BasicInfo

        public ProductMaintainBasicInfoDisplayInfoVM ConvertProductEntityToProductMaintainBasicInfoDisplayInfoVM(ProductInfo data)
        {
            var vm = new ProductMaintainBasicInfoDisplayInfoVM
            {
                ProductID = data.ProductID,
                ProductName = data.ProductName,
                ProductTitle = data.ProductBasicInfo.ProductTitle.Content,
                PromotionTitle = data.PromotionTitle.Content,
                ProductBriefName = data.ProductBasicInfo.ProductBriefName,
                ProductBriefTitle = data.ProductBasicInfo.ProductBriefTitle.Content,
                ProductBriefAddition = data.ProductBasicInfo.ProductBriefAddition.Content,
                Keywords = data.ProductBasicInfo.Keywords.Content,
                TaxNo = data.ProductBasicInfo.TaxNo,
                TariffPrice = data.ProductBasicInfo.TariffPrice,
                EntryRecord = data.ProductBasicInfo.EntryRecord,
                ShoppingGuideURL = data.ProductBasicInfo.ShoppingGuideURL,
                TradeType = data.ProductBasicInfo.TradeType,
                SafeQty = data.ProductBasicInfo.SafeQty.HasValue?data.ProductBasicInfo.SafeQty.ToString():string.Empty,
                StoreType = data.ProductBasicInfo.StoreType
            };
            if (data.ProductTimelyPromotionTitle
                .Any(promotionTitle => promotionTitle.PromotionTitleType.Trim() == "Normal"))
            {
                vm.NormalPromotionTitle = data.ProductTimelyPromotionTitle
                    .First(promotionTitle => promotionTitle.PromotionTitleType.Trim() == "Normal")
                    .PromotionTitle.Content;
            }

            if (data.ProductTimelyPromotionTitle
                             .Any(promotionTitle => promotionTitle.PromotionTitleType.Trim() == "CountDown"))
            {
                vm.TimelyPromotionTitle = data.ProductTimelyPromotionTitle
                    .First(promotionTitle => promotionTitle.PromotionTitleType.Trim() == "CountDown")
                    .PromotionTitle.Content;
                vm.TimelyPromotionBeginDate = data.ProductTimelyPromotionTitle
                    .First(promotionTitle => promotionTitle.PromotionTitleType.Trim() == "CountDown")
                    .BeginDate;
                vm.TimelyPromotionEndDate = data.ProductTimelyPromotionTitle
                    .First(promotionTitle => promotionTitle.PromotionTitleType.Trim() == "CountDown")
                    .EndDate;
            }
            return vm;
        }

        public ProductMaintainBasicInfoChannelInfoVM ConvertProductEntityToProductMaintainBasicInfoChannelInfoVM(ProductInfo data)
        {
            var vm = new ProductMaintainBasicInfoChannelInfoVM();
            if (data.Merchant.SysNo.HasValue)
            {
                vm.SellerSysNo = data.Merchant.SysNo.Value;
                vm.SellerName = data.Merchant.MerchantName;
            }
            vm.WebChannel = new UIWebChannel { ChannelID = "1", ChannelName = ResProductMaintain.DefaultChannelName, ChannelType = UIWebChannelType.Publicity };
            return vm;
            //throw new NotImplementedException();
        }

        public ProductMaintainBasicInfoStatusInfoVM ConvertProductEntityToProductMaintainBasicInfoStatusInfoVM(ProductInfo data)
        {
            var vm = new ProductMaintainBasicInfoStatusInfoVM
            {
                ProductStatus = data.ProductStatus
            };
            return vm;
        }

        public ProductMaintainBasicInfoSpecificationInfoVM ConvertProductEntityToProductMaintainBasicInfoSpecificationInfoVM(ProductInfo data)
        {
            var vm = new ProductMaintainBasicInfoSpecificationInfoVM
            {
                BrandInfo = new BrandVM
                {
                    SysNo = data.ProductBasicInfo.ProductBrandInfo.SysNo,
                    BrandNameLocal = data.ProductBasicInfo.ProductBrandInfo.BrandNameLocal.Content
                },
                CategoryInfo = new CategoryVM
                {
                    SysNo = data.ProductBasicInfo.ProductCategoryInfo.SysNo,
                    CategoryName = data.ProductBasicInfo.ProductCategoryInfo.CategoryName.Content
                },
                ManufacturerInfo = new ManufacturerVM
                {
                    SysNo = data.ProductBasicInfo.ProductBrandInfo.Manufacturer.SysNo,
                    ManufacturerNameLocal = data.ProductBasicInfo.ProductBrandInfo.Manufacturer.ManufacturerNameLocal.Content
                },
                ProductManagerInfo = new PMVM
                {
                    SysNo = data.ProductBasicInfo.ProductManager.SysNo,
                    PMUserName = data.ProductBasicInfo.ProductManager.UserInfo.UserDisplayName
                },
                ProductModel = data.ProductBasicInfo.ProductModel.Content,
                ProductType = data.ProductBasicInfo.ProductType,
                ProductConsignFlag = data.ProductConsignFlag,
                ProductIsTakePicture = data.ProductBasicInfo.IsTakePicture,
                UPCCode = data.ProductBasicInfo.UPCCode,
                BMCode = data.ProductBasicInfo.BMCode
            };
            return vm;
        }

        public ProductMaintainBasicInfoDescriptionInfoVM ConvertProductEntityToProductMaintainBasicInfoDescriptionInfoVM(ProductInfo productInfo)
        {
            var vm = new ProductMaintainBasicInfoDescriptionInfoVM
                         {
                             ProductDescription = productInfo.ProductBasicInfo.ShortDescription.Content,
                             Performance = productInfo.ProductBasicInfo.Performance,
                             PackageList = productInfo.ProductBasicInfo.PackageList.Content,
                             ProductLink = productInfo.ProductBasicInfo.ProductLink,
                             Attention = productInfo.ProductBasicInfo.Attention.Content
                         };
            return vm;
        }

        public ProductMaintainBasicInfoOtherVM ConvertProductEntityToProductMaintainBasicInfoOtherVM(ProductInfo productInfo)
        {
            var vm = new ProductMaintainBasicInfoOtherVM
            {
                NoExtendWarranty = productInfo.ProductWarrantyInfo.IsNoExtendWarranty,
                ProductInfoFinishStatus = productInfo.ProductBasicInfo.ProductInfoFinishStatus
            };
            return vm;
        }

        #endregion

        #region CommonInfo

        public ProductMaintainCommonInfoVM ConvertProductGroupEntityToProductMaintainCommonInfoVM(ProductGroup productGroup, ProductInfo product)
        {
            var vm = new ProductMaintainCommonInfoVM { ProductID = product.ProductID };
            if (productGroup.SysNo.HasValue)
            {
                vm.ProductGroupSysNo = productGroup.SysNo.Value;
                vm.ProductGroupName = productGroup.ProductGroupName.Content;
            }
            vm.ProductCategoryName = product.ProductBasicInfo.ProductCategoryInfo.CategoryName.Content;
            vm.ProductManufacturerName = product.ProductBasicInfo.ProductBrandInfo.Manufacturer.ManufacturerNameLocal.Content;
            vm.ProductBrandName = product.ProductBasicInfo.ProductBrandInfo.BrandNameLocal.Content;
            if (product.FirstOnSaleDate.HasValue)
            {
                vm.ProductFirstOnSaleDate = product.FirstOnSaleDate.Value;
            }
            if (product.LastEditDate.HasValue)
            {
                vm.ProductLastEditDate = product.LastEditDate.Value;
            }
            vm.GroupProductList = new List<ProductMaintainCommonInfoGroupProductVM>();
            productGroup.ProductList.ForEach(p => vm.GroupProductList.Add(ConvertProductEntityToProductMaintainCommonInfoGroupProductVM(p, product, productGroup)));
            return vm;
        }

        public ProductMaintainCommonInfoVM ConvertProductEntityToProductMaintainCommonInfoVM(ProductInfo product)
        {
            var vm = new ProductMaintainCommonInfoVM
            {
                ProductID = product.ProductID,
                ProductCategoryName = product.ProductBasicInfo.ProductCategoryInfo.CategoryName.Content,
                ProductManufacturerName = product.ProductBasicInfo.ProductBrandInfo.Manufacturer.ManufacturerNameLocal.Content,
                ProductBrandName = product.ProductBasicInfo.ProductBrandInfo.BrandNameLocal.Content
            };
            if (product.FirstOnSaleDate.HasValue)
            {
                vm.ProductFirstOnSaleDate = product.FirstOnSaleDate.Value;
            }
            if (product.LastEditDate.HasValue)
            {
                vm.ProductLastEditDate = product.LastEditDate.Value;
            }
            vm.GroupProductList = new List<ProductMaintainCommonInfoGroupProductVM>
            {
                new ProductMaintainCommonInfoGroupProductVM
                {
                    IsChecked = true,
                    PrductIDLinkColor = "Red",
                    ProductID = product.ProductID,
                    ProductStatus = product.ProductStatus,
                    ProductSysNo = product.SysNo
                }
            };
            return vm;
        }

        public ProductMaintainCommonInfoGroupProductVM ConvertProductEntityToProductMaintainCommonInfoGroupProductVM(ProductInfo productInfo, ProductInfo currentProductInfo, ProductGroup productGroup)
        {
            var vm = new ProductMaintainCommonInfoGroupProductVM
            {
                ProductID = productInfo.ProductID,
                ProductStatus = productInfo.ProductStatus,
                IsChecked = productInfo.ProductID == currentProductInfo.ProductID,
                PrductIDLinkColor = productInfo.ProductID == currentProductInfo.ProductID ? "Red" : "Black",
                ProductSysNo = productInfo.SysNo,
                ProductGroupProperties = productGroup.ProductGroupSettings
                .SelectMany(setting => productInfo.ProductBasicInfo.ProductProperties
                    .Where(property => property.Property.PropertyInfo.SysNo == setting.ProductGroupProperty.SysNo))
                    .Select(p => String.IsNullOrEmpty(ConvertLanguageContent(p.PersonalizedValue))
                        ? ConvertLanguageContent(p.Property.ValueDescription)
                        : ConvertLanguageContent(p.PersonalizedValue)).Join("  ")
            };
            return vm;
        }

        #endregion

        #region Description
        /// <summary>
        /// 为空的LanguageContent对象转换
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        private string ConvertLanguageContent(LanguageContent language)
        {
            if (language == null || language.Content == null)
            {
                return string.Empty;
            }
            else
            {
                return language.Content;
            }
        }

        public ProductMaintainDescriptionVM ConvertProductEntityToProductMaintainDescriptionVM(ProductInfo data)
        {
            var vm = new ProductMaintainDescriptionVM
            {
                ProductLongDescription = data.ProductBasicInfo.LongDescription.Content,
                ProductPhotoDescription = data.ProductBasicInfo.PhotoDescription.Content
            };
            return vm;
        }

        #endregion

        #region Dimension

        public ProductMaintainDimensionVM ConvertProductEntityToProductMaintainDimensionVM(ProductInfo data)
        {
            var vm = new ProductMaintainDimensionVM
            {
                Weight = Math.Round(data.ProductBasicInfo.ProductDimensionInfo.Weight, 2).ToString(),
                LargeFlag = data.ProductBasicInfo.ProductDimensionInfo.LargeFlag,
                Length = Math.Round(data.ProductBasicInfo.ProductDimensionInfo.Length, 2).ToString(),
                Width = Math.Round(data.ProductBasicInfo.ProductDimensionInfo.Width, 2).ToString(),
                Height = Math.Round(data.ProductBasicInfo.ProductDimensionInfo.Height, 2).ToString()
            };
            return vm;
        }

        #endregion

        #region Accessory

        public List<ProductMaintainAccessoryProductAccessoryVM> ConvertProductEntityToProductMaintainAccessoryProductAccessoryVMList(ProductInfo data)
        {
            return data.ProductBasicInfo.ProductAccessories.Select(productAccessory => new ProductMaintainAccessoryProductAccessoryVM
            {
                Accessory = new AccessoryVM
                {
                    AccessoryID = productAccessory.AccessoryInfo.AccessoryID,
                    AccessoryName = productAccessory.AccessoryInfo.AccessoryName.Content,
                    SysNo = productAccessory.AccessoryInfo.SysNo
                },
                SysNo = productAccessory.SysNo,
                Description = productAccessory.Description.Content,
                IsShow = productAccessory.Status,
                Priority = productAccessory.Priority.ToString(),
                Qty = productAccessory.Qty.ToString()
            }).ToList();
        }

        #endregion

        #region Image

        public ProductMaintainImageVM ConvertProductEntityToProductMaintainImageVM(ProductInfo data)
        {
            var vm = new ProductMaintainImageVM
                         {
                             ProductImageList = new List<ProductMaintainProductImageSingleVM>(),
                             IsVirtualPic = data.ProductBasicInfo.IsVirtualPic
                         };
            data.ProductBasicInfo.ProductResources
                .Where(resouce => resouce.Resource.Type == ResourcesType.Image)
                .ForEach(image => vm.ProductImageList.Add(ConvertProductResourcesToProductMaintainProductImageSingleVM(image)));
            return vm;
        }

        private ProductMaintainProductImageSingleVM ConvertProductResourcesToProductMaintainProductImageSingleVM(ProductResourceForNewegg data)
        {
            if (data.Resource.ResourceSysNo.HasValue)
            {
                //Ocean.20130514, Move ImgUrl to ControlMenuConfiguration
                string websiteProductImgUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebsiteProductImageUrl);
                var vm = new ProductMaintainProductImageSingleVM
                {
                    IsShow = data.IsShow,
                    ResourceName = data.Resource.ResourceURL,
                    ResourceUrl = websiteProductImgUrl + data.Resource.ResourceURL,
                    ResourceSysNo = data.Resource.ResourceSysNo.Value,
                    Priority = data.Resource.Priority
                };
                return vm;
            }
            return null;
        }

        #endregion

        #region Price

        public ProductMaintainPriceInfoVM ConvertProductEntityToProductMaintainPriceInfoVM(ProductInfo productInfo)
        {

            #region AutoPrice

            var autoPriceVM = new ProductMaintainPriceInfoAutoPriceVM
            {
                IsAutoAdjustPrice = IsAutoAdjustPrice.No, // productInfo.AutoAdjustPrice.IsAutoAdjustPrice,
                NotAutoAdjustPriceBeginDate = productInfo.AutoAdjustPrice.NotAutoPricingBeginDate,
                NotAutoAdjustPriceEndDate = productInfo.AutoAdjustPrice.NotAutoPricingEndDate,
                NotAutoAdjustPriceShow =
                    productInfo.AutoAdjustPrice.IsAutoAdjustPrice == IsAutoAdjustPrice.Yes
                        ? "Collapsed"
                        : "Visible"
            };

            #endregion

            #region BasicPrice

            var basicPriceVM = new ProductMaintainPriceInfoBasicPriceVM
            {
                BasicPrice = Math.Round(productInfo.ProductPriceInfo.BasicPrice, 2).ToString(),
                CurrentPrice = productInfo.ProductPriceInfo.CurrentPrice.Round(2),
                CashRebate = productInfo.ProductPriceInfo.CashRebate.Round(2),
                Point = productInfo.ProductPriceInfo.Point,
                PayType = productInfo.ProductPriceInfo.PayType,
                MinCommission = Math.Round(productInfo.ProductPriceInfo.MinCommission, 2).ToString(),
                MaxCountPerDay = productInfo.ProductPriceInfo.MaxCountPerDay.ToString(),
                UnitCost = Math.Round(productInfo.ProductPriceInfo.UnitCost, 2),
                UnitCostWithoutTax = Math.Round(productInfo.ProductPriceInfo.UnitCostWithoutTax, 2),
                DiscountAmount = Math.Round(productInfo.ProductPriceInfo.DiscountAmount, 2),
                VirtualPrice = Math.Round(productInfo.ProductPriceInfo.VirtualPrice, 2).ToString(),
                MinCountPerOrder = productInfo.ProductPriceInfo.MinCountPerOrder.ToString(),
                RequestCurrentPrice = productInfo.ProductPriceRequest.CurrentPrice.Round(2).ToString(),
                RequestCashRebate = productInfo.ProductPriceRequest.CashRebate.Round(2).ToString(),
                RequestPoint = productInfo.ProductPriceRequest.Point.ToString(),
                IsSyncShopPrice = productInfo.ProductPriceInfo.IsSyncShopPrice == IsSyncShopPrice.YES
            };

            #endregion

            #region RankPrice

            var rankPriceVM = new ProductMaintainPriceInfoRankPriceVM
            {
                ProductRankPriceList = new List<ProductRankPriceVM>()
            };

            productInfo.ProductPriceInfo.ProductRankPrice.ForEach(rankPrice =>
            {
                var rp = new ProductRankPriceVM
                {
                    CustomerRank = rankPrice.Rank,
                    RankPrice = rankPrice.RankPrice.Round(2),
                    Status = rankPrice.Status
                };
                if (productInfo.ProductPriceRequest.SysNo.HasValue)
                {
                    if (productInfo.ProductPriceRequest.ProductRankPrice.Any(r => r.Rank == rankPrice.Rank))
                    {
                        rp.RequestRankPrice =
                            productInfo.ProductPriceRequest.ProductRankPrice.First(r => r.Rank == rankPrice.Rank).
                                RankPrice.Round(2).ToString();
                    }
                }
                //泰隆优选前台没有8钻会员
                if (rankPrice.Rank != ECCentral.BizEntity.Customer.CustomerRank.Eight)
                {
                    rankPriceVM.ProductRankPriceList.Add(rp);
                }

            });

            #endregion

            #region VolumePrice

            var volumePriceVM = new ProductMaintainPriceInfoVolumePriceVM
            {
                ProductVolumePriceList = new List<ProductVolumePriceVM>()
            };

            productInfo.ProductPriceInfo.ProductWholeSalePriceInfo.ForEach(volumePrice =>
            {
                var vp = new ProductVolumePriceVM
                {
                    Level = volumePrice.Level,
                    Qty = volumePrice.Qty,
                    Price = volumePrice.Price.Round(2),
                    IsChecked = volumePrice.Qty.HasValue
                };
                if (productInfo.ProductPriceRequest.SysNo.HasValue)
                {
                    if (productInfo.ProductPriceRequest.ProductWholeSalePriceInfo.Any(w => w.Level == volumePrice.Level))
                    {
                        vp.VolumePriceRequestQty =
                            productInfo.ProductPriceRequest.ProductWholeSalePriceInfo.First(
                                w => w.Level == volumePrice.Level).Qty.ToString();
                        vp.VolumePriceRequestPrice =
                            productInfo.ProductPriceRequest.ProductWholeSalePriceInfo.First(
                                w => w.Level == volumePrice.Level).Price.Round(2).ToString();
                    }
                }
                volumePriceVM.ProductVolumePriceList.Add(vp);
            });

            #endregion

            #region VIPPrice

            var vipPriceVM = new ProductMaintainPriceInfoVIPPriceVM
            {
                IsUseAlipayVipPrice = productInfo.ProductPriceInfo.IsUseAlipayVipPrice,
                AlipayVIPPrice = productInfo.ProductPriceInfo.AlipayVipPrice,
                RequestAlipayVIPPrice = productInfo.ProductPriceRequest.AlipayVipPrice.Round(2).ToString()
            };

            #endregion

            #region AuditMemo

            var auditMemoVM = new ProductMaintainPriceInfoAuditMemoVM
            {
                PMMemo = productInfo.ProductPriceRequest.PMMemo,
                TLMemo = productInfo.LastProductPriceRequestInfo.TLMemo,
                PMDMemo = productInfo.LastProductPriceRequestInfo.PMDMemo
            };

            #endregion

            var vm = new ProductMaintainPriceInfoVM
            {
                ProductMaintainPriceInfoAutoPrice = autoPriceVM,
                ProductMaintainPriceInfoBasicPrice = basicPriceVM,
                ProductMaintainPriceInfoRankPrice = rankPriceVM,
                ProductMaintainPriceInfoVolumePrice = volumePriceVM,
                ProductMaintainPriceInfoVIPPrice = vipPriceVM,
                ProductMaintainPriceInfoAuditMemo = auditMemoVM,
                ProductPriceRequestStatus = productInfo.ProductPriceRequest.RequestStatus,
                ProductMerchant = productInfo.Merchant
            };
            return vm;
        }

        #endregion

        #region Warranty

        public ProductMaintainWarrantyVM ConvertProductEntityToProductMaintainWarrantyVM(ProductInfo data)
        {
            var vm = new ProductMaintainWarrantyVM
            {
                HostWarrantyDay = data.ProductWarrantyInfo.HostWarrantyDay,
                PartWarrantyDay = data.ProductWarrantyInfo.PartWarrantyDay,
                ServicePhone = data.ProductWarrantyInfo.ServicePhone,
                ServiceInfo = data.ProductWarrantyInfo.ServiceInfo,
                Warranty = data.ProductWarrantyInfo.Warranty.Content,
                WarrantyShow = data.ProductWarrantyInfo.WarrantyShow,
                OfferVATInvoice = data.ProductWarrantyInfo.OfferVATInvoice
            };
            return vm;
        }

        #endregion

        #region Property

        public ProductMaintainPropertyVM ConvertProductEntityToProductMaintainPropertyVM(ProductInfo data, ProductGroup productGroup)
        {
            var vm = new ProductMaintainPropertyVM
            {
                ProductPropertyValueList = new List<ProductMaintainPropertyPropertyValueVM>()
            };

            if (data == null || data.ProductBasicInfo == null || data.ProductBasicInfo.ProductProperties == null || data.ProductBasicInfo.ProductProperties.Count == 0
                || productGroup == null || productGroup.ProductGroupSettings == null
                || data.ProductBasicInfo.ProductProperties[0].Property == null || data.ProductBasicInfo.ProductProperties[0].PropertyGroup == null
                || data.ProductBasicInfo.ProductProperties[0].Property.PropertyInfo == null)
            {
                return vm;
            }
            data.ProductBasicInfo.ProductProperties.ForEach(productProperty =>
            {
                var productPropertyVM = new ProductMaintainPropertyPropertyValueVM
                {
                    SysNo = productProperty.SysNo,
                    PropertyGroupInfo = new PropertyGroupInfoVM
                    {
                        SysNo = productProperty.PropertyGroup.SysNo,
                        PropertyGroupName = productProperty.PropertyGroup.PropertyGroupName.Content
                    },
                    Property = new PropertyVM()
                };
                if (productProperty.Property.PropertyInfo.SysNo.HasValue)
                {
                    productPropertyVM.Property.SysNo = productProperty.Property.PropertyInfo.SysNo.Value;
                    productPropertyVM.Property.PropertyName = productProperty.Property.PropertyInfo.PropertyName.Content;
                }
                productPropertyVM.ProductPropertyValue = new PropertyValueVM();
                if (productProperty.Property.SysNo.HasValue)
                {
                    productPropertyVM.ProductPropertyValue.SysNo = productProperty.Property.SysNo.Value;
                    productPropertyVM.ProductPropertyValue.ValueDescription =
                        productProperty.Property.ValueDescription.Content;
                }
                productPropertyVM.PersonalizedValue = productProperty.PersonalizedValue.Content;

                if (productGroup.SysNo.HasValue)
                {
                    productPropertyVM.PropertyType = productGroup.ProductGroupSettings.Any(
                        setting => setting.ProductGroupProperty.SysNo == productProperty.Property.PropertyInfo.SysNo)
                        ? PropertyType.Grouping
                        : PropertyType.Other;
                }
                else
                {
                    productPropertyVM.PropertyType = PropertyType.Other;
                }
                productPropertyVM.RequiredColor = productProperty.Required == ProductPropertyRequired.Yes ? "Red" : "Black";
                if (productProperty.Required == ProductPropertyRequired.Yes || productGroup.ProductGroupSettings.Any(
                        setting => setting.ProductGroupProperty.SysNo == productProperty.Property.PropertyInfo.SysNo))
                {
                    productPropertyVM.CanPersonalizedValueInput = false;
                }
                else
                {
                    productPropertyVM.CanPersonalizedValueInput = true;
                }
                vm.ProductPropertyValueList.Add(productPropertyVM);
            });
            return vm;
        }

        public List<PropertyValueVM> ConvertPropertyValueInfoToPropertyValueVM(List<PropertyValueInfo> data)
        {
            var list = new List<PropertyValueVM>();
            data.ForEach(propertyValue =>
            {
                var vm = new PropertyValueVM();
                if (propertyValue.SysNo.HasValue)
                {
                    vm.SysNo = propertyValue.SysNo.Value;
                    vm.ValueDescription = propertyValue.ValueDescription.Content;
                }
                list.Add(vm);
            });
            return list;
        }

        #endregion

        #region SalesArea

        public List<ProvinceVM> ConvertAreaInfoEntityToProvinceVM(List<AreaInfo> data)
        {
            var list = new List<ProvinceVM>();
            data.ForEach(area =>
            {
                var vm = new ProvinceVM();
                if (area.SysNo.HasValue)
                {
                    vm.ProvinceSysNo = area.SysNo.Value;
                    vm.ProvinceName = area.ProvinceName;
                }
                list.Add(vm);
            });
            return list;
        }

        public List<CityVM> ConvertAreaInfoEntityToCityVM(List<AreaInfo> data)
        {
            var list = new List<CityVM>();
            data.ForEach(area =>
            {
                var vm = new CityVM();
                if (area.SysNo.HasValue)
                {
                    vm.CitySysNo = area.SysNo.Value;
                    vm.CityName = area.CityName;
                }
                list.Add(vm);
            });
            return list;
        }

        public List<ProductMaintainSalesAreaSelectVM> ConvertProductInfoEntityToProductMaintainSalesAreaSelectVM(ProductInfo productInfo)
        {
            return (productInfo.ProductSalesAreaInfoList
                .Where(productSalesAreaInfo => productSalesAreaInfo.Province.ProvinceSysNo.HasValue)
                .Select(productSalesAreaInfo => productSalesAreaInfo.Province.ProvinceSysNo.HasValue && productSalesAreaInfo.Province.CitySysNo.HasValue
                    ? new ProductMaintainSalesAreaSelectVM
                    {
                        StockSysNo = productSalesAreaInfo.Stock.SysNo,
                        StockName = productSalesAreaInfo.Stock.StockName,
                        Province = new ProvinceVM
                                       {
                                           ProvinceSysNo =
                                               productSalesAreaInfo.Province.ProvinceSysNo.
                                               Value,
                                           ProvinceName =
                                               productSalesAreaInfo.Province.ProvinceName
                                       },
                        City = new CityVM
                        {
                            CitySysNo =
                                productSalesAreaInfo.Province.CitySysNo.
                                Value,
                            CityName =
                                productSalesAreaInfo.Province.CityName
                        }
                    } : null)).ToList();
        }

        #endregion

        #region PurchaseInfo

        public ProductMaintainPurchaseInfoVM ConvertProductEntityToProductMaintainPurchaseInfoVM(ProductInfo data)
        {
            var vm = new ProductMaintainPurchaseInfoVM
            {
                MinPackCount = data.ProductPOInfo.MinPackNumber.ToString(),
                POMemo = data.ProductPOInfo.POMemo,
                InventoryType = data.ProductPOInfo.InventoryType,
                ERPProductID = data.ProductPOInfo.ERPProductID
            };
            return vm;
        }

        #endregion

        public ProductMaintainBatchManagementInfoVM ConvertProductEntityToProductMaintainBatchManagementInfoVM(ProductInfo data)
        {
            var re = EntityConverter<ProductBatchManagementInfo, ProductMaintainBatchManagementInfoVM>.Convert(data.ProductBatchManagementInfo);
            if (re == null)
            {
                return new ProductMaintainBatchManagementInfoVM();
            }
            return re;
        }

        #region ThirdPartyInventory

        public ProductMaintainThirdPartyInventoryVM ConvertProductEntityToProductMaintainThirdPartyInventoryVM(ProductInfo data)
        {
            ProductMapping productMapping;
            if (data.ProductMappingList.Any())
            {
                productMapping = data.ProductMappingList.First();
            }
            else
            {
                productMapping = new ProductMapping
                {
                    ThirdPartner = ThirdPartner.Ingrammicro,
                    SynProductID = String.Empty,
                    StockRules = StockRules.Limit,
                    LimitQty = 0,
                    Status = ProductMappingStatus.DeActive
                };
            }

            var vm = new ProductMaintainThirdPartyInventoryVM
            {
                ThirdPartner = productMapping.ThirdPartner,
                SynProductID = productMapping.SynProductID,
                StockRules = productMapping.StockRules,
                LimitCount = productMapping.LimitQty.ToString(),
                Status = productMapping.Status
            };
            return vm;
        }

        #endregion

        #endregion

        #endregion

        #region 退换货相关
        public void GetProductRMAPolicyByProductSysNo(int? productSysNo, EventHandler<RestClientEventArgs<ProductRMAPolicyInfo>> callback)
        {
            _restClient.Query<ProductRMAPolicyInfo>(ProductRmaPolicyRelativeUrl, productSysNo, callback);
        }

        public void UpdateProductRMAPolicy(ProductRmaPolicyVM vm, int? productSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductRMAPolicyInfo info = new ProductRMAPolicyInfo()
            {
                RMAPolicyMasterSysNo = vm.RmaPolicySysNo,
                IsBrandWarranty = vm.IsBrandWarranty ? "Y" : "N",
                WarrantyDay = string.IsNullOrEmpty(vm.WarrantyDay) ? 0 : Convert.ToInt32(vm.WarrantyDay),
                WarrantyDesc = vm.WarrantyDesc,
                User = new UserInfo()
                {
                    UserDisplayName = CPApplication.Current.LoginUser.DisplayName,
                    SysNo = CPApplication.Current.LoginUser.UserSysNo
                },
                ProductSysNo = productSysNo,
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = CPApplication.Current.LanguageCode
            };
            _restClient.Update(UpdateProductRMAPolicyRelativeUrl, info, callback);
        }
        #endregion

        public void ProductBatchEntry(List<int> productSysNos, string note, ProductEntryStatus entryStatus, ProductEntryStatusEx entryStatusEx, Action<string> callback)
        {
            ProductBatchEntryReq req = new ProductBatchEntryReq();
            req.ProductSysNoList = productSysNos;
            req.Note = note;
            req.EntryStatus = entryStatus;
            req.EntryStatusEx = entryStatusEx;
            _restClient.Update(ProductBatchEntryRelativeUrl, req, (obj, args) =>
            {
                if (args.Error != null)
                {
                    string error = GetError(args.Error);
                    callback(error);
                    return;
                }

                if (!args.FaultsHandle() && args.Result != null && callback != null)
                {
                    if (args.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }

        public string GetError(RestServiceError error)
        {
            StringBuilder build = new StringBuilder();
            foreach (Error item in error.Faults)
            {
                build.Append(string.Format("{0}", item.ErrorDescription));
            }
            return build.ToString();
        }


        public void BatchAuditProduct(List<int> productSysNos, ProductStatus status, Action<string> callback)
        {
            ProductBatchAuditReq req = new ProductBatchAuditReq();
            req.ProductSysNo = productSysNos;
            req.Status = status;
            _restClient.Update(ProductBatchAuditRelativeUrl, req, (obj, args) =>
            {
                if (args.Error != null)
                {
                    string error = GetError(args.Error);
                    callback(error);
                    return;
                }

                if (!args.FaultsHandle() && args.Result != null && callback != null)
                {
                    if (args.Result != null)
                    {
                        callback(string.Empty);
                    }
                }
            });
        }
    }

    public class ProductBatchAuditReq
    {
        public List<int> ProductSysNo { set; get; }
        public ProductStatus Status { set; get; }
    }

}

