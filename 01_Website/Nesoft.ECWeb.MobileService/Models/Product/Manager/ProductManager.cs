using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Product;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.UI;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Product.Models;
using Nesoft.ECWeb.Entity.Promotion;
using Nesoft.ECWeb.Entity;
using Nesoft.Utility;
using Nesoft.ECWeb.Facade.GroupBuying;
using Nesoft.ECWeb.Entity.Promotion.GroupBuying;
using Nesoft.ECWeb.MobileService.Models.App;
using Nesoft.ECWeb.MobileService.AppCode;
using Nesoft.ECWeb.MobileService.Models.MemberService;
using Nesoft.ECWeb.Facade.Store;
using Nesoft.ECWeb.Entity.Store;
using Nesoft.ECWeb.Facade.Member;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class ProductManager
    {
        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="id">系统编号</param>
        /// <param name="isGroupBuy">0--标识id为商品系统编号，1--标识id为团购活动系统编号</param>
        /// <returns></returns>
        public ProductDetailModel GetProductDetails(int id, int isGroupBuy)
        {
            int productSysNo = 0;
            GroupBuyingInfo groupBuyInfo = null;
            if (isGroupBuy == 1)
            {
                groupBuyInfo = GroupBuyingFacade.GetGroupBuyingInfoBySysNo(id);
                if (groupBuyInfo == null)
                {
                    //提示团购活动找不到
                    throw new BusinessException("团购活动找不到啦，请选购其它商品，谢谢。");
                }
                productSysNo = groupBuyInfo.ProductSysNo;
            }
            else
            {
                productSysNo = id;
            }


            //商品基本信息
            ProductBasicInfo basicInfo = ProductFacade.GetProductBasicInfoBySysNo(productSysNo);

            //商品销售信息
            ProductSalesInfo salesInfo = ProductFacade.GetProductSalesInfoBySysNo(productSysNo);

            if (basicInfo == null || salesInfo == null)
            {
                //提示商品找不到
                throw new BusinessException("商品找不到啦，请选购其它商品，谢谢。");
            }
            //如果是不展示或下架
            if (basicInfo.ProductStatus == ProductStatus.NotShow || basicInfo.ProductStatus == ProductStatus.Abandon)
            {
                //提示商品状态已下架或已作废
                throw new BusinessException("商品已下架或已作废，请选购其它商品，谢谢。");
            }
            //商品组信息
            List<ProductPropertyView> propertyView = ProductFacade.GetProductPropetyView(productSysNo, basicInfo.ProductCommonInfoSysNo);

            //商品附件
            List<ProductItemInfo> attachmentList = ProductFacade.GetProductAttachmentList(productSysNo);

            //商品配件
            List<ProductAccessories> accessoriesList = ProductFacade.GetProductAccessoriesList(productSysNo);
            
            //商家信息
            StoreBasicInfo storeinfo = StoreFacade.QueryStoreBasicInfo(basicInfo.VendorSysno);

            //商品促销信息
            ProductPromotionInfo promotionInfo = Nesoft.ECWeb.Facade.Product.ProductFacade.GetProductPromotionInfo(productSysNo);

            //商品组图片信息
            List<ProductImage> productImages = ProductFacade.GetProductImages(basicInfo.ProductCommonInfoSysNo);

            //商品内容(商品详情,规格参数,售后服务,购买须知等)
            List<ProductContent> contentList = ProductFacade.GetProductContentList(basicInfo);

            ProductDetailModel result = new ProductDetailModel();
            //基本信息
            result.BasicInfo = TransformBasicInfo(basicInfo);
            //商品销售(价格,库存等)信息
            result.SalesInfo = TransformSalesInfo(basicInfo, salesInfo);

            //商品图片列表
            result.ImageList = TransformImageList(productImages);
            //商品描述信息
            result.DescInfo = TransformDescInfo(contentList);
            //if (result.DescInfo != null && basicInfo != null)
            //    result.DescInfo.Performance = basicInfo.Performance;


            //分组属性
            result.GroupPropertyInfo = TransformGroupProperty(propertyView);
            //附件信息
            result.AttachmentInfo = TransformAttachmentInfo(attachmentList);
            //配件信息
            result.AccessoryList = TransformAccessoryInfo(accessoriesList);

            //商家信息
            result.StoreBasicInfo = Transformstoreinfo(storeinfo);


            //限时抢购,赠品,套餐等促销信息
            result.PromoInfo = TransformPromoInfo(promotionInfo);

            //如果是团购商品进一步加载团购详情
            if (promotionInfo != null && promotionInfo.GroupBuySysNo > 0)
            {
                if (groupBuyInfo == null)
                {
                    groupBuyInfo = GroupBuyingFacade.GetGroupBuyingInfoBySysNo(promotionInfo.GroupBuySysNo);
                }
                if (groupBuyInfo == null)
                {
                    result.PromoInfo.GroupBuyingSysNo = 0;
                }
                else
                {
                    //团购图片特殊处理,用活动设置的图片
                    result.BasicInfo.DefaultImageUrl = groupBuyInfo.GroupBuyingPicUrl;
                    result.ImageList.Clear();
                    ProductImageModel groupBuyImage = new ProductImageModel();
                    groupBuyImage.ImageUrlBig = groupBuyInfo.GroupBuyingPicUrl;
                    groupBuyImage.ImageUrlHuge = groupBuyInfo.GroupBuyingPicUrl;
                    result.ImageList.Add(groupBuyImage);
                    //海外团购商品，算税
                    if (groupBuyInfo.GroupBuyingTypeSysNo == 0)
                    {
                        result.SalesInfo.TariffPrice = salesInfo.CurrentPrice * salesInfo.TariffRate;

                        if (result.SalesInfo.TariffPrice <= ConstValue.TariffFreeLimit)
                        {
                            result.SalesInfo.FreeEntryTax = true;
                            result.SalesInfo.TotalPrice = salesInfo.CurrentPrice;
                        }
                        else
                        {
                            result.SalesInfo.FreeEntryTax = false;
                            result.SalesInfo.TotalPrice = salesInfo.CurrentPrice + result.SalesInfo.TariffPrice;
                        }
                        decimal snapShotTariffPrice = groupBuyInfo.SnapShotCurrentPrice * salesInfo.TariffRate;
                        if (snapShotTariffPrice <= ConstValue.TariffFreeLimit)
                        {
                            snapShotTariffPrice = 0;
                        }
                        result.SalesInfo.BasicPrice = groupBuyInfo.SnapShotCurrentPrice + groupBuyInfo.SnapShotCashRebate + snapShotTariffPrice;
                    }
                    else
                    {
                        result.SalesInfo.BasicPrice = groupBuyInfo.SnapShotCurrentPrice + groupBuyInfo.SnapShotCashRebate;
                        result.SalesInfo.TotalPrice = salesInfo.CurrentPrice;
                        result.SalesInfo.TariffPrice = 0;
                        result.SalesInfo.FreeEntryTax = false;
                    }
                    groupBuyInfo.MarketPrice = result.SalesInfo.BasicPrice;

                    result.GroupBuyInfo = MapGroupBuyInfo(groupBuyInfo);
                }
            }
            if (promotionInfo != null && promotionInfo.Countdown != null)
            {
                //限时抢购重算市场价
                decimal snapShotTariffPrice = promotionInfo.Countdown.SnapShotCurrentPrice * salesInfo.TariffRate;
                if (snapShotTariffPrice <= ConstValue.TariffFreeLimit)
                {
                    snapShotTariffPrice = 0;
                }
                result.SalesInfo.BasicPrice = promotionInfo.Countdown.SnapShotCurrentPrice + promotionInfo.Countdown.SnapShotCashRebate + snapShotTariffPrice;
            }

            result.ActionInfo = MapActionInfo(result);

            return result;
        }

        

        private UIActionInfo MapActionInfo(ProductDetailModel model)
        {
            //仅显示状态
            if (model.BasicInfo.ProductStatus == (int)ProductStatus.OnlyShow)
            {
                return UIActionInfo.BuildActionOver();
            }

            bool isCountDownOver = model.PromoInfo.CountDownSysNo > 0 && model.PromoInfo.CountDownLeftSecond <= 0;
            bool isGroupBuyOver = model.PromoInfo.GroupBuyingSysNo > 0 && model.GroupBuyInfo.LeftSeconds <= 0;
            //限时抢购或团购结束
            if (isCountDownOver
                || isGroupBuyOver)
            {
                return UIActionInfo.BuildActionDone();
            }
            bool isCountDown = model.PromoInfo.CountDownSysNo > 0 && model.PromoInfo.CountDownLeftSecond > 0;
            bool isGroupBuy = model.PromoInfo.GroupBuyingSysNo > 0 && model.GroupBuyInfo.LeftSeconds > 0;
            //库存没了
            if (model.SalesInfo.OnlineQty <= 0)
            {
                if (isCountDown)
                {
                    return UIActionInfo.BuildActionOver();
                }
                else if (isGroupBuy)
                {
                    return UIActionInfo.BuildActionOver();
                }
                else
                {
                    return UIActionInfo.BuildActionNotify();
                }
            }
            else
            {
                if (isCountDown)
                {
                    return UIActionInfo.BuildActionCountDown();
                }
                else if (isGroupBuy)
                {
                    return UIActionInfo.BuildActionGroupBuy();
                }
                else
                {
                    return UIActionInfo.BuildActionCart();
                }
            }
        }

        private GroupBuyDetailModel MapGroupBuyInfo(GroupBuyingInfo item)
        {
            GroupBuyDetailModel itemModel = new GroupBuyDetailModel();
            if (item == null)
            {
                return itemModel;
            }
            itemModel.GroupBuyingDesc = item.GroupBuyingDesc;
            itemModel.GroupBuyingDescLong = item.GroupBuyingDescLong;
            itemModel.GroupBuyingRules = item.GroupBuyingRules;
            itemModel.ProductPhotoDesc = item.ProductPhotoDesc;
            itemModel.GroupBuyingPicUrl = item.GroupBuyingPicUrl;
            itemModel.GroupBuyingSysNo = item.SysNo;
            itemModel.GroupBuyingTitle = item.GroupBuyingTitle;
            itemModel.CurrentSellCount = item.CurrentSellCount;

            decimal realTariffPrice = item.CurrentPrice * item.TaxRate;
            if (realTariffPrice <= ConstValue.TariffFreeLimit)
            {
                realTariffPrice = 0;
            }
            decimal finalPrice = item.CurrentPrice + realTariffPrice;

            //计算折扣
            if (item.MarketPrice == 0)
            {
                itemModel.DiscountStr = "";
            }
            else
            {
                itemModel.DiscountStr = (finalPrice / item.MarketPrice * 10).ToString("F2") + "折";
            }
            //计算省好多
            itemModel.SaveMoney = item.MarketPrice - finalPrice;
            //计算剩余时间
            if (item.EndDate <= DateTime.Now)
            {
                itemModel.LeftSeconds = 0;
            }
            else
            {
                itemModel.LeftSeconds = (long)(item.EndDate - DateTime.Now).TotalSeconds;
            }

            //计算销售状态
            if (itemModel.LeftSeconds <= 0)
            {
                itemModel.SellStatusStr = "团购结束";
            }
            else if (item.OnlineQty <= 0)
            {
                itemModel.SellStatusStr = "已售罄";
            }
            else
            {
                itemModel.SellStatusStr = "团购进行中";
            }

            return itemModel;
        }

        private ProductContentModel TransformDescInfo(List<ProductContent> contentList)
        {
            contentList = contentList ?? new List<ProductContent>();
            ProductContentModel result = new ProductContentModel();
            result.Detail = GetContent(contentList, ProductContentType.Detail);
            string xmPerformance = GetContent(contentList, ProductContentType.Performance);
            result.Performance = ProductFacade.BuildProductPerformanceToHtml(xmPerformance);
            //购买须知移除html tag
            string attention = GetContent(contentList, ProductContentType.Attention);
            result.Attention = StringUtility.RemoveHtmlTag(attention);
            result.Warranty = GetContent(contentList, ProductContentType.Warranty);

            //包装html内容
            MobileAppConfig config = AppSettings.GetCachedConfig();
            if (config != null)
            {
                result.Detail = (config.ProductDescTemplate ?? "").Replace("${content}", result.Detail);
                result.Performance = (config.ProductSpecTemplate ?? "").Replace("${content}", result.Performance);
            }

            return result;
        }

        private string GetContent(List<ProductContent> contentList, ProductContentType contentType)
        {

            var detail = contentList.SingleOrDefault(item => item.ContentType == contentType);
            if (detail != null)
            {
                return detail.Content ?? "";
            }

            return "";
        }

        private List<ProductImageModel> TransformImageList(List<ProductImage> imageList)
        {
            imageList = imageList ?? new List<ProductImage>();
            ImageSize imageSizeBig = ImageUrlHelper.GetImageSize(ImageType.Big);
            ImageSize imageSizeHuge = ImageUrlHelper.GetImageSize(ImageType.Huge);
            List<ProductImageModel> result = new List<ProductImageModel>();
            foreach (var image in imageList)
            {
                ProductImageModel model = new ProductImageModel();
                model.ImageUrlBig = ProductFacade.BuildProductImage(imageSizeBig, image.ResourceUrl);
                model.ImageUrlHuge = ProductFacade.BuildProductImage(imageSizeHuge, image.ResourceUrl);

                result.Add(model);
            }

            return result;
        }

        private BasicInfoModel TransformBasicInfo(ProductBasicInfo basicInfo)
        {
            BasicInfoModel result = new BasicInfoModel();
            result.ID = basicInfo.ID;
            result.Code = basicInfo.Code;
            result.C3SysNo = basicInfo.CategoryID;
            result.ProductTitle = basicInfo.ProductTitle;
            result.PromotionTitle = basicInfo.PromotionTitle;
            result.ProductMode = basicInfo.ProductMode;
            result.ProducePlace = basicInfo.ProductEntryInfo.OriginCountryName;
            result.ProductGroupSysNo = basicInfo.ProductGroupSysNo;
            result.ProductStatus = (int)basicInfo.ProductStatus;
            result.ProductType = (int)basicInfo.ProductType;

            //构造商品图片
            ImageSize imageSizeMiddle = ImageUrlHelper.GetImageSize(ImageType.Middle);
            result.DefaultImageUrl = ProductFacade.BuildProductImage(imageSizeMiddle, basicInfo.DefaultImage);

            return result;
        }

        private SalesInfoModel TransformSalesInfo(ProductBasicInfo basicInfo, ProductSalesInfo salesInfo)
        {
            SalesInfoModel priceModel = new SalesInfoModel();
            //预计到货时间(单位：天)
            int shippingDays = int.Parse(Nesoft.ECWeb.Facade.CommonFacade.GetSysConfigByKey("仓库处理时间"));
            shippingDays += int.Parse(Nesoft.ECWeb.Facade.CommonFacade.GetSysConfigByKey("快递配送时间"));
            shippingDays += basicInfo.ProductEntryInfo.LeadTimeDays;
            priceModel.ETA = shippingDays;

            priceModel.BasicPrice = salesInfo.MarketPrice;
            priceModel.CurrentPrice = salesInfo.CurrentPrice;
            priceModel.CashRebate = salesInfo.CashRebate;
            priceModel.TariffPrice = salesInfo.EntryTax ?? 0;
            priceModel.TotalPrice = salesInfo.TotalPrice;
            priceModel.MinCountPerOrder = salesInfo.MinCountPerOrder;
            priceModel.MaxCountPerOrder = salesInfo.MaxCountPerOrder;
            priceModel.PresentPoint = salesInfo.Point;
            //是否免关税
            priceModel.FreeEntryTax = salesInfo.FreeEntryTax;
            //是否免运费
            priceModel.FreeShipping = false;
            priceModel.OnlineQty = salesInfo.OnlineQty;

            //计算库存状态
            string inventoryStatus = "";
            if (basicInfo.ProductStatus == ProductStatus.OnlyShow
                || salesInfo.OnlineQty <= 0)
            {
                inventoryStatus = "已售罄";
            }
            else if (salesInfo.OnlineQty <= ConstValue.ProductWarnInventory)
            {
                inventoryStatus = "即将售完";
            }
            else
            {
                inventoryStatus = "有货";
            }
            priceModel.InventoryStatus = inventoryStatus;

            //计算是否已加入收藏夹
            bool productIsWished = false;
            LoginUser currUser = UserMgr.ReadUserInfo();
            if (currUser == null || currUser.UserSysNo <= 0)
            {
                productIsWished = false;
            }
            else
            {
                productIsWished = ProductFacade.IsProductWished(basicInfo.ID, currUser.UserSysNo);
            }
            priceModel.IsWished = productIsWished;

            return priceModel;
        }

        private GroupPropertyModel TransformGroupProperty(List<ProductPropertyView> propertyView)
        {
            propertyView = propertyView ?? new List<ProductPropertyView>();
            GroupPropertyModel result = new GroupPropertyModel();
            var g1 = propertyView.FirstOrDefault();
            if (g1 != null && g1.ProductList.Count > 0 && g1.Current != null)
            {

                result.Property1List = TransformGroupPropertyItemList(g1.Type, g1.ProductList);
                string propertyName;
                string propertyValue;
                if (g1.Type == 1)
                {
                    propertyName = g1.Current.ParentPropertyName;
                    propertyValue = g1.Current.ParentValue;
                }
                else
                {
                    propertyName = g1.Current.PropertyName;
                    propertyValue = g1.Current.Value;
                }
                result.PropertyDescription1 = propertyName;
                result.ValueDescription1 = propertyValue;
            }

            if (propertyView.Count > 1)
            {
                var g2 = propertyView[1];
                result.Property2List = TransformGroupPropertyItemList(g2.Type, g2.ProductList);
                string propertyName;
                string propertyValue;
                if (g2.Type == 1)
                {
                    propertyName = g2.Current.ParentPropertyName;
                    propertyValue = g2.Current.ParentValue;
                }
                else
                {
                    propertyName = g2.Current.PropertyName;
                    propertyValue = g2.Current.Value;
                }
                result.PropertyDescription2 = propertyName;
                result.ValueDescription2 = propertyValue;
            }

            return result;
        }



        private List<GroupPropertyItemModel> TransformGroupPropertyItemList(int type, List<ProductPropertyInfo> productList)
        {
            var result = new List<GroupPropertyItemModel>();
            foreach (var p in productList)
            {
                GroupPropertyItemModel itemModel = new GroupPropertyItemModel();
                itemModel.ProductSysNo = p.ProductSysNo;
                itemModel.ProductID = p.ProductId;
                if (type == 1)
                {
                    itemModel.PropertyValue = p.ParentValue;
                }
                else
                {
                    itemModel.PropertyValue = p.Value;
                }

                result.Add(itemModel);
            }

            return result;
        }

        private PromoInfoModel TransformPromoInfo(ProductPromotionInfo promotionInfo)
        {
            PromoInfoModel result = new PromoInfoModel();
            result.CountDownSysNo = promotionInfo.CountdownSysNo;
            if (promotionInfo.Countdown != null && promotionInfo.Countdown.EndTime.HasValue)
            {
                //计算限时抢购剩余时间
                if (promotionInfo.Countdown.EndTime <= DateTime.Now)
                {
                    result.CountDownLeftSecond = 0;
                }
                else
                {
                    var ts = promotionInfo.Countdown.EndTime.Value - DateTime.Now;
                    result.CountDownLeftSecond = (int)ts.TotalSeconds;
                }
            }
            result.GroupBuyingSysNo = promotionInfo.GroupBuySysNo;
            //读取单品买赠和厂商赠品列表
            result.GiftInfo = new List<GiftItemModel>();
            if (promotionInfo.SaleGiftList == null)
            {
                promotionInfo.SaleGiftList = new List<SaleGiftInfo>();
            }
            var giftPromoList = promotionInfo.SaleGiftList.Where(item => item.SaleGiftType == SaleGiftType.Single || item.SaleGiftType == SaleGiftType.Vendor);
            ImageSize imageSizeGift = ImageUrlHelper.GetImageSize(ImageType.Tiny);
            foreach (var promo in giftPromoList)
            {
                if (promo.GiftItemList == null)
                {
                    promo.GiftItemList = new List<GiftItem>();
                }
                foreach (var giftItem in promo.GiftItemList)
                {
                    GiftItemModel model = new GiftItemModel();
                    model.ID = giftItem.ProductSysNo;
                    model.ProductName = giftItem.ProductName;
                    model.ImageUrl = ProductFacade.BuildProductImage(imageSizeGift, giftItem.DefaultImage);
                    model.UnitQuantity = giftItem.UnitQuantity;

                    result.GiftInfo.Add(model);
                }
            }
            //套餐活动信息
            result.ComboInfo = new List<ComboInfoModel>();
            var comboList = promotionInfo.ComboList ?? new List<ComboInfo>();
            ImageSize imageSizeCombo = ImageUrlHelper.GetImageSize(ImageType.Middle);
            foreach (var combo in comboList)
            {
                decimal originalTotal = 0;
                decimal discount = 0;
                List<ComboItemModel> comboItemList = new List<ComboItemModel>();
                foreach (var item in combo.Items)
                {
                    ComboItemModel itemModel = new ComboItemModel();
                    itemModel.ID = item.ProductSysNo;
                    itemModel.Code = item.ProductID;
                    itemModel.ProductTitle = item.ProductName;
                    itemModel.Quantity = item.Quantity;
                    itemModel.ImageUrl = ProductFacade.BuildProductImage(imageSizeCombo, item.DefaultImage);
                    itemModel.CurrentPrice = item.CurrentPrice;
                    var tariffPrice = item.TariffRate * item.CurrentPrice;
                    itemModel.TariffPrice = tariffPrice;
                    itemModel.Discount = item.Discount;
                    if (tariffPrice <= ConstValue.TariffFreeLimit)
                    {
                        originalTotal += item.CurrentPrice * item.Quantity;
                    }
                    else
                    {
                        originalTotal += (item.CurrentPrice + tariffPrice) * item.Quantity;
                    }

                    discount += item.Discount * item.Quantity;

                    comboItemList.Add(itemModel);
                }

                ComboInfoModel model = new ComboInfoModel();
                if (combo.SysNo.HasValue)
                {
                    model.SysNo = combo.SysNo.Value;
                }
                model.SaleRuleName = combo.SaleRuleName;
                model.OriginalPrice = originalTotal;
                model.DiscountPrice = discount;
                model.TotalPrice = originalTotal + (discount > 0 ? -discount : discount);
                model.Items = comboItemList;

                result.ComboInfo.Add(model);
            }

            return result;
        }

        private StoreBasicInfoModel Transformstoreinfo(StoreBasicInfo storeinfo)
        {
            StoreBasicInfoModel result = new StoreBasicInfoModel();
            result.Address = storeinfo.Address;
            result.BrandAuthorize = storeinfo.BrandAuthorize;
            result.ContactName = storeinfo.ContactName;
            result.CooperationMode = storeinfo.CooperationMode;
            result.CurrentECChannel = storeinfo.CurrentECChannel;
            result.ECExpValue = storeinfo.ECExpValue;
            result.EditDate = storeinfo.EditDate;
            result.EditUserName = storeinfo.EditUserName;
            result.EditUserSysNo = storeinfo.EditUserSysNo;
            result.Email = storeinfo.Email;
            result.ExportExpValue = storeinfo.ExportExpValue;
            result.HaveECExp = storeinfo.HaveECExp;
            result.HaveExportExp = storeinfo.HaveExportExp;
            result.InDate = storeinfo.InDate;
            result.InUserName = storeinfo.InUserName;
            result.InUserSysNo = storeinfo.InUserSysNo;
            result.MainBrand = storeinfo.MainBrand;
            result.MainProductCategory = storeinfo.MainProductCategory;
            result.Mobile = storeinfo.Mobile;
            result.Name = storeinfo.Name;
            result.Phone = storeinfo.Phone;
            result.QQ = storeinfo.QQ;
            result.Remark = storeinfo.Remark;
            result.SellerSysNo = storeinfo.SellerSysNo;
            result.Site = storeinfo.Site;
            result.Status = storeinfo.Status;
            result.StoreName = storeinfo.StoreName;
            result.SysNo = storeinfo.SysNo;
            result.ValidDate = storeinfo.ValidDate;

            //构造商品图片
            ImageSize imageSizeMiddle = ImageUrlHelper.GetImageSize(ImageType.Middle);
            result.LogoURL = ProductFacade.BuildProductImage(imageSizeMiddle, storeinfo.LogoURL);


            //是否被收藏
            #region 是否被收藏
            LoginUser CurrUser = UserMgr.ReadUserInfo();
            bool StoreIsWished = false;
            if (CurrUser == null || CurrUser.UserSysNo < 0)
            {
                StoreIsWished = false;
            }
            else
            {
                StoreIsWished = CustomerFacade.IsMyFavoriteSeller(CurrUser.UserSysNo, storeinfo.SellerSysNo.Value);
            }
            #endregion
            result.StoreIsWished = StoreIsWished;
            return result;
        }

        private List<AttachmentInfo> TransformAttachmentInfo(List<ProductItemInfo> attachmentList)
        {
            attachmentList = attachmentList ?? new List<ProductItemInfo>();
            ImageSize imageSizeGift = ImageUrlHelper.GetImageSize(ImageType.Tiny);
            List<AttachmentInfo> result = new List<AttachmentInfo>();
            foreach (var item in attachmentList)
            {
                AttachmentInfo model = new AttachmentInfo();
                model.ID = item.ID;
                model.ProductName = item.ProductTitle;
                model.UnitQuantity = item.DisplayQuantity;
                model.ImageUrl = ProductFacade.BuildProductImage(imageSizeGift, item.DefaultImage);

                result.Add(model);
            }

            return result;
        }
        private List<AttachmentInfo> TransformAccessoryInfo(List<ProductAccessories> accessoriesList)
        {
            accessoriesList = accessoriesList ?? new List<ProductAccessories>();
            List<AttachmentInfo> result = new List<AttachmentInfo>();
            foreach (var item in accessoriesList)
            {
                AttachmentInfo model = new AttachmentInfo();
                model.ID = item.ProductSysNo;
                model.ProductName = item.AccessoriesName;
                model.UnitQuantity = item.Quantity;
                model.ImageUrl = "";

                result.Add(model);
            }

            return result;
        }

        public static ProductReviewListViewModel GetProductReviewList(int productSysNo, int productGroupSysNo, List<ReviewScoreType> searchType, int pageIndex, int pageSize)
        {
            Product_ReviewQueryInfo queryInfo = new Product_ReviewQueryInfo() { ProductSysNo = productSysNo, ProductGroupSysNo = productGroupSysNo, SearchType = searchType, PagingInfo = new PageInfo() { PageSize = pageSize, PageIndex = pageIndex } };


            var originReviewResult = ReviewFacade.GetProductReviewListByProductGroupSysNoForProduct(queryInfo);
            var result = EntityConverter<Product_ReviewList, ProductReviewListViewModel>.Convert(originReviewResult, (s, t) =>
            {
                t.ProductReviewDetailList.PageIndex = s.ProductReviewDetailList.PageNumber - 1;
                t.ProductReviewDetailList.PageSize = s.ProductReviewDetailList.PageSize;
                t.ProductReviewDetailList.TotalRecords = s.ProductReviewDetailList.TotalRecords;
                t.ProductReviewDetailList.CurrentPageData = EntityConverter<List<Product_ReviewDetail>, List<ProductReviewDetailItemViewModel>>.Convert(s.ProductReviewDetailList.CurrentPageData);
                t.TotalCount1 = s.TotalCount1 + s.TotalCount2;
                t.TotalCount3 = s.TotalCount3 + s.TotalCount4;


                if (t.ProductReviewDetailList != null && t.ProductReviewDetailList.CurrentPageData != null && t.ProductReviewDetailList.CurrentPageData.Count > 0)
                {
                    for (int i = 0; i < t.ProductReviewDetailList.CurrentPageData.Count; i++)
                    {
                        t.ProductReviewDetailList.CurrentPageData[i].NickName = s.ProductReviewDetailList.CurrentPageData[i].CustomerInfo.NickName;
                    }
                }

                t.ProductReviewDetailList.PageIndex = pageIndex - 1;
                t.ProductReviewDetailList.PageSize = pageSize;
                t.ProductReviewDetailList.TotalRecords = searchType.Contains(ReviewScoreType.ScoreType11) ? t.TotalCount1 : (searchType.Contains(ReviewScoreType.ScoreType13) ? t.TotalCount3 : (searchType.Contains(ReviewScoreType.ScoreType15) ? t.TotalCount5 : t.TotalCount1));
                t.ProductReviewDetailList.TotalPages = t.ProductReviewDetailList.TotalRecords <= 0 ? 0 : (t.ProductReviewDetailList.TotalRecords % t.ProductReviewDetailList.PageSize == 0 ? t.ProductReviewDetailList.TotalRecords / t.ProductReviewDetailList.PageSize : t.ProductReviewDetailList.TotalRecords / t.ProductReviewDetailList.PageSize + 1);
                if (null != t.ProductReviewScore && t.ProductReviewScore.ProductSysNo == 0)
                {
                    t.ProductReviewScore.ProductSysNo = productSysNo;
                }
            });
            return result;
        }

        public static bool CreateProductConsultInfo(AddProductConsultInfoViewModel request)
        {
            if (request.ProductSysNo <= 0)
            {
                throw new BusinessException("商品编号不能为空!");
            }
            if (string.IsNullOrEmpty(request.Content))
            {
                throw new BusinessException("咨询内容不能为空!");
            }
            if (string.IsNullOrEmpty(request.Type))
            {
                throw new BusinessException("咨询类型不能为空!");
            }
            return ConsultationFacade.CreateProductConsult(EntityConverter<AddProductConsultInfoViewModel, ConsultationInfo>.Convert(request));
        }
    }
}
