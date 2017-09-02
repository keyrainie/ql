//************************************************************************
// 用户名				泰隆优选
// 系统名				商家商品需求管理
// 子系统名		        商家商品需求管理业务逻辑实现
// 作成者				Kevin
// 改版日				2012.6.7
// 改版内容				新建
//************************************************************************
using System.Linq;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using System.Transactions;
using System;
using ECCentral.BizEntity.IM.Product;


namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(SellerProductRequestProcessor))]
    public class SellerProductRequestProcessor
    {

        private readonly ISellerProductRequestDA _SellerProductRequestDA = ObjectFactory<ISellerProductRequestDA>.Instance;
        private readonly IProductCommonInfoDA _ProductCommonInfoDA = ObjectFactory<IProductCommonInfoDA>.Instance;
        private readonly IProductDA _ProductDA = ObjectFactory<IProductDA>.Instance;
        private readonly IProductGroupDA _productGroupDA = ObjectFactory<IProductGroupDA>.Instance;

        #region 商家商品需求管理业务方法
        /// <summary>
        /// 根据SysNo获取商家商品信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual SellerProductRequestInfo GetSellerProductRequestInfoBySysNo(int sysNo)
        {
            CheckSellerProductRequestInfoProcessor.CheckSellerProductRequestInfoSysNo(sysNo);
            var result = _SellerProductRequestDA.GetSellerProductRequestInfoBySysNo(sysNo);

            var pmManageInfo = ObjectFactory<ProductLineProcessor>.Instance.GetPMByC3(
                    result.CategoryInfo.SysNo.Value,
                    result.Brand.SysNo.Value);

            if (pmManageInfo != null)
            {
                result.PMUser = pmManageInfo.UserInfo;
            }

            result.ProductDescLong = RemoveHtmlStr(result.ProductDescLong);

            result.SellerProductRequestFileList = _SellerProductRequestDA.GetSenderProductRequestImageList(sysNo);
            result.SellerProductRequestPropertyList = _SellerProductRequestDA.GetProductRequestPropertyList(sysNo);

            return result;
        }

        /// <summary>
        /// 过滤html,js,css代码
        /// </summary>
        /// <param name="html">参数传入</param>
        /// <returns></returns>
        public string RemoveHtmlStr(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+?</script *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@"<a.*?>|</a>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@"(<[^>]*)\son[\s\S]*?=([^>]*>)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+?</iframe *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+?</frameset *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex6 = new System.Text.RegularExpressions.Regex(@"<!--[\s\S]*?-->", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex7 = new System.Text.RegularExpressions.Regex(@"<form.*?>|</form>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            html = regex1.Replace(html, ""); //过滤<script></script>标记
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性
            html = regex3.Replace(html, "$1$2"); //过滤其它控件的on...事件
            html = regex4.Replace(html, ""); //过滤iframe
            html = regex5.Replace(html, ""); //过滤frameset
            html = regex6.Replace(html, ""); //过滤注释
            html = regex7.Replace(html, ""); //过滤From

            return html;
        }

        /// <summary>
        /// 根据ProductIDo获取商家商品信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public virtual SellerProductRequestInfo GetSellerProductInfoByProductID(string productID)
        {
            //CheckSellerProductRequestInfoProcessor.CheckSellerProductRequestInfoSysNo(sysNo);
            if (string.IsNullOrEmpty(productID))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestProductID"));
            }

            ProductProcessor productBp = new ProductProcessor();
            ProductInfo productInfo = productBp.GetProductInfoByID(productID);

            if (productInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestProductID"));
            }

            var entity = new SellerProductRequestInfo();

            entity.ProductID = productInfo.ProductID;
            entity.BriefName = productInfo.ProductBasicInfo.ProductBriefName;
            entity.ProductName = productInfo.ProductBasicInfo.ProductTitle.Content;
            entity.ProductLink = productInfo.ProductBasicInfo.ProductLink;
            entity.PackageList = productInfo.ProductBasicInfo.PackageList.Content;
            entity.Keywords = productInfo.ProductBasicInfo.Keywords.Content;
            entity.UPCCode = productInfo.ProductBasicInfo.UPCCode;
            entity.BriefName = productInfo.ProductBasicInfo.ProductBriefName;
            entity.IsTakePictures = (SellerProductRequestTakePictures)productInfo.ProductBasicInfo.IsTakePicture;
            entity.Attention = productInfo.ProductBasicInfo.Attention.Content;
            entity.Note = productInfo.ProductBasicInfo.Note;
            entity.PromotionTitle = productInfo.PromotionTitle.Content;
            //entity.HostWarrantyDay = productInfo.ProductWarrantyInfo.HostWarrantyDay;
            //entity.PartWarrantyDay = productInfo.ProductWarrantyInfo.PartWarrantyDay;
            //entity.Warranty = productInfo.ProductWarrantyInfo.Warranty.Content;
            entity.ServicePhone = productInfo.ProductWarrantyInfo.ServicePhone;
            entity.ServiceInfo = productInfo.ProductWarrantyInfo.ServiceInfo;
            // entity.IsOfferInvoice = productInfo.ProductWarrantyInfo.OfferVATInvoice== OfferVATInvoice.Yes? SellerProductRequestOfferInvoice.Yes: SellerProductRequestOfferInvoice.No;
            entity.Height = productInfo.ProductBasicInfo.ProductDimensionInfo.Height;
            entity.Weight = productInfo.ProductBasicInfo.ProductDimensionInfo.Weight;
            entity.Length = productInfo.ProductBasicInfo.ProductDimensionInfo.Length;
            entity.Width = productInfo.ProductBasicInfo.ProductDimensionInfo.Width;
            entity.MinPackNumber = productInfo.ProductPOInfo.MinPackNumber;
            entity.ProductDescLong = productInfo.ProductBasicInfo.LongDescription.Content;

            entity.SellerProductRequestPropertyList = _SellerProductRequestDA.GetSellerProductPropertyListByProductID(productID);


            return entity;
        }

        /// <summary>
        /// 审核通过新品创建请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo ApproveProductRequest(SellerProductRequestInfo entity)
        {
            CheckSellerProductRequestInfoProcessor.CheckSellerProductRequestInfo(entity);
            using (TransactionScope scope = new TransactionScope())
            {
                if (entity.Type == SellerProductRequestType.ImageAndDescriptionUpdate)
                {
                    //如果是图片与描述更新，则审批通过后状态变为‘处理中’
                    entity.Status = SellerProductRequestStatus.Processing;
                    entity = _SellerProductRequestDA.UpdateSellerProductRequest(entity);

                }
                else
                {
                    entity.Status = SellerProductRequestStatus.Approved;
                    entity = _SellerProductRequestDA.SetSellerProductRequestStatus(entity);
                    _SellerProductRequestDA.SetSellerProductRequestOtherStatus(entity);
                }

                int result = _SellerProductRequestDA.CallExternalSP(entity);
                if (result == 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest",
                                                                           "SellerPortalCallExternalSPError"));
                }
                scope.Complete();
                return entity;
            }
        }

        /// <summary>
        /// 退回新品创建请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo DenyProductRequest(SellerProductRequestInfo entity)
        {

            CheckSellerProductRequestInfoProcessor.CheckSellerProductRequestInfo(entity);

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (entity.Status == SellerProductRequestStatus.Approved)
                    {
                        entity.Status = SellerProductRequestStatus.WaitApproval;
                    }
                    else
                    {
                        entity.Status = SellerProductRequestStatus.UnApproved;
                    }


                    entity = _SellerProductRequestDA.SetSellerProductRequestStatus(entity);
                    _SellerProductRequestDA.SetSellerProductRequestOtherStatus(entity);
                    int result = _SellerProductRequestDA.CallExternalSP(entity);

                    if (result == 0)
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerPortalCallExternalSPError"));
                    }

                }
                catch (Exception ex)
                {
                    throw new BizException(ex.Message);
                }

                scope.Complete();
            }

            return entity;

        }

        /// <summary>
        ///  更新商品请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo UpdateProductRequest(SellerProductRequestInfo entity)
        {
            CheckSellerProductRequestInfoProcessor.CheckSellerProductRequestInfo(entity);

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    ProductProcessor productBp = new ProductProcessor();

                    ProductInfo productInfo = productBp.GetProductInfoByID(entity.ProductID);

                    if (productInfo == null)
                    {
                        //商品不存在
                        throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestProductID"));
                    }

                    productInfo.CompanyCode = entity.CompanyCode;
                    productInfo.LanguageCode = entity.LanguageCode;

                    productInfo.ProductBasicInfo.ProductTitle.Content = entity.ProductName;
                    productInfo.ProductBasicInfo.ProductLink = entity.ProductLink;
                    productInfo.ProductBasicInfo.PackageList.Content = entity.PackageList;
                    productInfo.ProductBasicInfo.Keywords.Content = entity.Keywords;
                    productInfo.ProductBasicInfo.UPCCode = entity.UPCCode;
                    productInfo.ProductBasicInfo.ProductBriefTitle.Content = entity.BriefName;
                    productInfo.ProductBasicInfo.IsTakePicture = (ProductIsTakePicture)entity.IsTakePictures;
                    productInfo.ProductBasicInfo.Attention.Content = entity.Attention;
                    productInfo.ProductBasicInfo.Note = entity.Note;
                    productInfo.PromotionTitle.Content = entity.PromotionTitle;
                    //productInfo.ProductWarrantyInfo.HostWarrantyDay = entity.HostWarrantyDay;
                    //productInfo.ProductWarrantyInfo.PartWarrantyDay = entity.PartWarrantyDay;
                    //productInfo.ProductWarrantyInfo.Warranty.Content = entity.Warranty;
                    productInfo.ProductWarrantyInfo.ServicePhone = entity.ServicePhone;
                    productInfo.ProductWarrantyInfo.ServiceInfo = entity.ServiceInfo;
                    // productInfo.ProductWarrantyInfo.OfferVATInvoice = entity.IsOfferInvoice== SellerProductRequestOfferInvoice.Yes? OfferVATInvoice.Yes: OfferVATInvoice.No;
                    productInfo.ProductBasicInfo.ProductDimensionInfo.Height = entity.Height;
                    productInfo.ProductBasicInfo.ProductDimensionInfo.Weight = entity.Weight;
                    productInfo.ProductBasicInfo.ProductDimensionInfo.Length = entity.Length;
                    productInfo.ProductBasicInfo.ProductDimensionInfo.Width = entity.Width;
                    productInfo.ProductPOInfo.MinPackNumber = entity.MinPackNumber;

                    //更新ProductCommonInfo
                    _ProductCommonInfoDA.UpdateProductCommonInfoBasicInfo(productInfo, entity.EditUser);
                    _ProductCommonInfoDA.UpdateProductCommonInfoDimensionInfo(productInfo, entity.EditUser);
                    _ProductCommonInfoDA.UpdateProductCommonInfoWarrantyInfo(productInfo, entity.EditUser);
                    _ProductCommonInfoDA.UpdateProductCommonInfoNote(productInfo, entity.EditUser);
                    productInfo.OperateUser = entity.EditUser;
                    _ProductDA.UpdateProductPurchaseInfo(productInfo);
                    _ProductDA.UpdateProductBasicInfo(productInfo);

                    if (entity.SellerProductRequestPropertyList.Count > 0)
                    {
                        foreach (SellerProductRequestPropertyInfo requestProperty in entity.SellerProductRequestPropertyList)
                        {
                            if (productInfo.ProductBasicInfo.ProductProperties != null && productInfo.ProductBasicInfo.ProductProperties.Count > 0)
                            {
                                var property = productInfo.ProductBasicInfo.ProductProperties.Where(p => p.Property.PropertyInfo.SysNo == requestProperty.PropertySysno && p.PropertyGroup.SysNo == requestProperty.GroupSysno).ToList();
                                if (property.Count > 0)
                                {
                                    property.ForEach(p => productInfo.ProductBasicInfo.ProductProperties.Remove(p));
                                }
                            }

                            ProductProperty newProperty = new ProductProperty();
                            newProperty.Property = new PropertyValueInfo();
                            newProperty.PropertyGroup = new PropertyGroupInfo();
                            newProperty.Property.PropertyInfo = new PropertyInfo();
                            newProperty.PropertyGroup.SysNo = requestProperty.GroupSysno;
                            newProperty.PropertyGroup.PropertyGroupName = new LanguageContent(requestProperty.GroupDescription);
                            newProperty.Property.PropertyInfo.SysNo = requestProperty.PropertySysno;
                            newProperty.Property.PropertyInfo.PropertyName = new LanguageContent(requestProperty.PropertyDescription);
                            newProperty.Property.SysNo = requestProperty.ValueSysno;
                            newProperty.Property.ValueDescription = new LanguageContent(requestProperty.ValueDescription);
                            newProperty.PersonalizedValue = new LanguageContent(requestProperty.ManualInput);
                            productInfo.ProductBasicInfo.ProductProperties.Add(newProperty);

                            //更新请求属性信息
                            _SellerProductRequestDA.UpdateSellerProductRequestProperty(requestProperty, entity.EditUser.UserDisplayName);
                        }
                    }

                    foreach (var p in productInfo.ProductBasicInfo.ProductProperties)
                    {
                        p.CompanyCode = entity.CompanyCode;
                        p.LanguageCode = entity.LanguageCode;
                        p.Property.SysNo = p.Property.SysNo ?? 0;

                    }

                    string result = productBp.UpdateProductPropertyInfo(productInfo);

                    entity.Status = SellerProductRequestStatus.Finish;
                    entity = _SellerProductRequestDA.SetSellerProductRequestStatus(entity);

                }
                catch (Exception ex)
                {
                    throw new BizException(ex.Message);
                }

                scope.Complete();
            }

            return entity;
        }

        /// <summary>
        /// 创建商品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo CreateItemIDForNewProductRequest(SellerProductRequestInfo entity)
        {

            //判断请求类型是否正确
            if (entity.Type != SellerProductRequestType.NewCreated)
            {
                //请求信息类型不为新品创建！
                throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestTypeNotNewCreated"));
            }

            CheckSellerProductRequestInfoProcessor.CheckSellerProductRequestInfo(entity);

            //系统语言列表
            List<Language> languageList = ExternalDomainBroker.GetAllLanguageList();
            var defaultLanguage = languageList.FirstOrDefault(item => item.IsDefault == 1);

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var productGroup = BuildProductGroupInfo(entity);
                    productGroup.ProductList.ForEach(product => { product.ProductBasicInfo.LongDescription = new LanguageContent(); });
                    var dict = ObjectFactory<ProductProcessor>.Instance.ProductCreate(productGroup);

                    if (!productGroup.LanguageCode.Equals(defaultLanguage.LanguageCode))
                    {
                        ObjectFactory<ProductProcessor>.Instance.ProductLangCreate(productGroup);//写入商品多语言
                    }

                    if (dict.Count > 0)
                    {
                        String exceptionMsg;
                        if (dict.TryGetValue(productGroup.ProductList.First().GetHashCode(), out exceptionMsg))
                        {
                            throw new BizException(exceptionMsg);
                        }
                    }

                    int i = 0;
                    foreach (SellerProductRequestFileInfo image in entity.SellerProductRequestFileList)
                    {
                        if (i == 0)
                        {
                            image.ImageName = productGroup.ProductList.First().ProductBasicInfo.CommonSkuNumber + ".jpg";
                        }
                        else
                        {
                            image.ImageName = productGroup.ProductList.First().ProductBasicInfo.CommonSkuNumber + "_" + i.ToString().PadLeft(2, '0') + ".jpg";
                        }

                        //设置Commonsku每张图片写上文件名
                        _SellerProductRequestDA.UpdateProductRequestImageName(image);

                        i++;
                    }

                    entity.Status = SellerProductRequestStatus.Processing;
                    entity.CommonSKUNumber = productGroup.ProductList.First().ProductBasicInfo.CommonSkuNumber;
                    entity.ProductID = productGroup.ProductList.First().ProductID;

                    entity = _SellerProductRequestDA.UpdateSellerProductRequest(entity);

                    entity.Memo = entity.ProductID;
                    _SellerProductRequestDA.CallExternalSP(entity);

                }
                catch (Exception ex)
                {
                    throw new BizException(ex.Message);
                }

                scope.Complete();
            }


            return entity;
        }

        private ProductGroup BuildProductGroupInfo(SellerProductRequestInfo data)
        {
            var productGroup = new ProductGroup
            {
                SysNo = data.GroupSysno,
                ProductList = new List<ProductInfo>(),
                LanguageCode = data.LanguageCode
            };

            var product = new ProductInfo();
            product.CompanyCode = data.CompanyCode;
            product.LanguageCode = data.LanguageCode;
            product.PromotionTitle = new LanguageContent(data.PromotionTitle);
            product.ProductConsignFlag = data.IsConsign;
            product.ProductStatus = ProductStatus.InActive_UnShow;
            product.ProductPOInfo = new ProductPOInfo { MinPackNumber = data.MinPackNumber };
            product.ProductPayType = ProductPayType.All;
            product.Merchant = new Merchant { SysNo = data.SellerSysNo };
            product.OrginCode = data.OrginCode;

            product.ProductPriceInfo = new ProductPriceInfo
                                       {
                                           BasicPrice = data.BasicPrice,
                                           CashRebate = 0,
                                           CurrentPrice = data.CurrentPrice,
                                           DiscountAmount = 0,
                                           MaxCountPerDay = 1,
                                           MinCountPerOrder = 1,
                                           PayType = ProductPayType.All,
                                           Point = 0,
                                           ProductRankPrice = new List<ProductRankPriceInfo>(),
                                           ProductWholeSalePriceInfo = new List<ProductWholeSalePriceInfo>(),
                                           UnitCost = 999999,
                                           UnitCostWithoutTax = 999999,
                                           VirtualPrice = Math.Round(data.VirtualPrice, 2)
                                       };
            product.ProductWarrantyInfo = new ProductWarrantyInfo
                                          {
                                              HostWarrantyDay = data.HostWarrantyDay,
                                              IsNoExtendWarranty = true,
                                              OfferVATInvoice = OfferVATInvoice.No,//CRL 21623
                                              PartWarrantyDay = data.PartWarrantyDay,
                                              ServiceInfo = data.ServiceInfo,
                                              ServicePhone = data.ServicePhone,
                                              Warranty = new LanguageContent(data.Warranty),
                                              WarrantyShow = WarrantyShow.Yes
                                          };

            product.ProductBasicInfo = new ProductBasicInfo
                                {
                                    Attention = new LanguageContent(data.Attention),
                                    IsAccessoryShow = ProductIsAccessoryShow.No,
                                    IsTakePicture = ProductIsTakePicture.No,
                                    IsVirtualPic = ProductIsVirtualPic.No,
                                    Keywords = new LanguageContent(data.Keywords),
                                    LongDescription = new LanguageContent(data.ProductDescLong),
                                    Note = data.Note,
                                    PackageList = new LanguageContent(data.PackageList),
                                    Performance = String.Empty,
                                    PhotoDescription = new LanguageContent(String.Empty),
                                    ProductBrandInfo = data.Brand,
                                    ProductBriefTitle = new LanguageContent(data.BriefName),
                                    ProductBriefAddition = new LanguageContent(String.Empty),
                                    ProductCategoryInfo = data.CategoryInfo,
                                    ProductDimensionInfo = new ProductDimensionInfo
                                    {
                                        Height = data.Height,
                                        Length = data.Length,
                                        Weight = data.Weight,
                                        Width = data.Width
                                    },
                                    ProductInfoFinishStatus = ProductInfoFinishStatus.No,
                                    ProductLink = data.ProductLink,
                                    ProductModel = new LanguageContent(data.ProductModel),
                                    ProductTitle = new LanguageContent(data.ProductName),
                                    ShortDescription = new LanguageContent(String.Empty),
                                    UPCCode = data.UPCCode,
                                    ProductType = ProductType.Normal,
                                    ProductProperties = BuildProductPropertyList(data)
                                };

            product.OperateUser = data.EditUser;
            product.CompanyCode = data.CompanyCode;
            product.LanguageCode = data.LanguageCode;
            productGroup.ProductList.Add(product);
            return productGroup;
        }

        private List<ProductProperty> BuildProductPropertyList(SellerProductRequestInfo data)
        {
            var productProperties = new List<ProductProperty>();
            data.SellerProductRequestPropertyList.ForEach(sellerProductProperty =>
            {
                var property = new ProductProperty
                                   {
                                       PropertyGroup = new PropertyGroupInfo { SysNo = sellerProductProperty.GroupSysno },
                                       Property = new PropertyValueInfo
                                                      {
                                                          SysNo = sellerProductProperty.ValueSysno,
                                                          PropertyInfo =
                                                              new PropertyInfo { SysNo = sellerProductProperty.PropertySysno }
                                                      },
                                       PersonalizedValue = new LanguageContent(sellerProductProperty.ManualInput),
                                       CompanyCode = data.CompanyCode,
                                       LanguageCode = data.LanguageCode
                                   };
                productProperties.Add(property);
            });

            return productProperties;
        }


        #endregion

        #region 检查商家商品逻辑

        private static class CheckSellerProductRequestInfoProcessor
        {
            private static readonly ISellerProductRequestDA _SellerProductRequestDA = ObjectFactory<ISellerProductRequestDA>.Instance;

            /// <summary>
            /// 检查商家商品实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckSellerProductRequestInfo(SellerProductRequestInfo entity)
            {
                if (entity == null)
                {
                    //请求信息不存在
                    throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestSysNOIsNull"));
                }

                SellerProductRequestInfo oldEntity = _SellerProductRequestDA.GetSellerProductRequestInfoBySysNo(entity.SysNo.Value);

                if (oldEntity == null)
                {
                    //请求信息不存在
                    throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestSysNOIsNull"));
                }

                //判断商品状态是否正确
                if (oldEntity.Status != entity.Status)
                {
                    //请求信息状态已变更请刷新再试！
                    throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestStatusChange"));
                }
            }

            /// <summary>
            /// 检查商家商品编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckSellerProductRequestInfoSysNo(int? sysNo)
            {

                if (sysNo == null || sysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestSysNOIsNull"));
                }
            }

        }
        #endregion
    }
}
