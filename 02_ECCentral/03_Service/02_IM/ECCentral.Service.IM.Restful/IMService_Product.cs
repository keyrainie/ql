using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.IM.Restful.RequestMsg;
using ECCentral.Service.IM.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity.IM.Product;
using ECCentral.Service.IM.IDataAccess;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region QueryProduct

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/QueryProduct", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProduct(ProductQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductQueryDA>.Instance.QueryProduct(request, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/GetProductInfo", Method = "POST")]
        public ProductInfo GetProductInfo(int productSysNo)
        {
            return ObjectFactory<ProductAppService>.Instance.GetProductInfo(productSysNo);
        }

        /// <summary>
        /// 查询商品列表
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/GetProductInfoListBySysNoList", Method = "POST")]
        public List<ProductInfo> GetProductInfoListBySysNoList(List<int> productSysNoList)
        {
            return ObjectFactory<ProductAppService>.Instance.GetProductInfoListByProductSysNoList(productSysNoList);
        }

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/GetProductInfoByID", Method = "POST")]
        public ProductInfo GetProductInfoByID(string productID)
        {
            ProductInfo result = ObjectFactory<ProductAppService>.Instance.GetProductInfoByID(productID);
            return result;
        }

        #endregion

        #region OnSaleProduct

        /// <summary>
        /// 商品上架
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/ProductOnSale", Method = "PUT")]
        public int ProductOnSale(int productSysNo)
        {
            return ObjectFactory<ProductAppService>.Instance.ProductOnSale(productSysNo);
        }

        /// <summary>
        /// 商品批量上架
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/ProductBatchOnSale", Method = "PUT")]
        public Dictionary<int, String> ProductBatchOnSale(List<int> productSysNoList)
        {
            var result = new Dictionary<int, String>();

            foreach (var sysNo in productSysNoList)
            {
                try
                {
                    ObjectFactory<ProductAppService>.Instance.ProductOnSale(sysNo);
                }
                catch (BizException exception)
                {
                    if (!result.ContainsKey(sysNo))
                    {
                        result.Add(sysNo, exception.Message);
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  商品显示
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/ProductOnShow", Method = "PUT")]
        public int ProductOnShow(int productSysNo)
        {
            return ObjectFactory<ProductAppService>.Instance.ProductOnShow(productSysNo);
        }

        /// <summary>
        /// 商品不显示
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/ProductUnShow", Method = "PUT")]
        public int ProductUnShow(int productSysNo)
        {
            return ObjectFactory<ProductAppService>.Instance.ProductUnShow(productSysNo);
        }

        /// <summary>
        /// 商品作废
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/ProductInvalid", Method = "PUT")]
        public int ProductInvalid(int productSysNo)
        {
            return ObjectFactory<ProductAppService>.Instance.ProductInvalid(productSysNo);
        }

        /// <summary>
        /// 商品批量审核
        /// </summary>
        /// <param name="req"></param>
        [WebInvoke(UriTemplate = "/Product/ProductBatchAudit", Method = "PUT")]
        public void ProductBatchAudit(ProductBatchAuditReq req)
        {
            ObjectFactory<IProductDA>.Instance.BatchAuditProduct(req.ProductSysNo, req.Status);
        }

        #endregion

        #region UpdateProduct

        /// <summary>
        /// 更新商品基本信息
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/UpdateProductBasicInfo", Method = "PUT")]
        public UpdateProductBasicInfoRsp UpdateProductBasicInfo(ProductInfo productInfo)
        {
            ProductManagerInfo productManager = null;
            ObjectFactory<ProductAppService>.Instance.UpdateProductBasicInfo(productInfo, ref productManager);
            return new UpdateProductBasicInfoRsp { ProductManager = productManager };
        }

        //批量更新商品基本信息
        [WebInvoke(UriTemplate = "/Product/BatchUpdateProductBasicInfo", Method = "PUT")]
        public void BatchUpdateProductBasicInfo(ProductBatchUpdateRequestMsg requestMsg)
        {
            ObjectFactory<ProductAppService>.Instance.BatchUpdateProductBasicInfo(requestMsg.ProductInfo, requestMsg.BatchUpdateProductSysNoList);
        }


        /// <summary>
        /// 更新商品详细描述信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductDescriptionInfo", Method = "PUT")]
        public void UpdateProductDescriptionInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductDescriptionInfo(productInfo);
        }

        /// <summary>
        /// 批量更新商品详细描述信息
        /// </summary>
        /// <param name="requestMsg"></param>
        [WebInvoke(UriTemplate = "/Product/BatchUpdateProductDescriptionInfo", Method = "PUT")]
        public void BatchUpdateProductDescriptionInfo(ProductBatchUpdateRequestMsg requestMsg)
        {
            ObjectFactory<ProductAppService>.Instance.BatchUpdateProductDescriptionInfo(requestMsg.ProductInfo, requestMsg.BatchUpdateProductSysNoList);
        }

        /// <summary>
        /// 更新商品配件信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductAccessoryInfo", Method = "PUT")]
        public void UpdateProductAccessoryInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductAccessoryInfo(productInfo);
        }

        /// <summary>
        /// 批量更新商品配件信息
        /// </summary>
        /// <param name="requestMsg"></param>
        [WebInvoke(UriTemplate = "/Product/BatchUpdateProductAccessoryInfo", Method = "PUT")]
        public void BatchUpdateProductAccessoryInfo(ProductBatchUpdateRequestMsg requestMsg)
        {
            ObjectFactory<ProductAppService>.Instance.BatchUpdateProductAccessoryInfo(requestMsg.ProductInfo, requestMsg.BatchUpdateProductSysNoList);
        }

        /// <summary>
        /// 更新商品图片信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductImageInfo", Method = "PUT")]
        public void UpdateProductImageInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductImageInfo(productInfo);
        }

        /// <summary>
        /// 更新商品属性信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductPropertyInfo", Method = "PUT")]
        public string UpdateProductPropertyInfo(ProductInfo productInfo)
        {
            return ObjectFactory<ProductAppService>.Instance.UpdateProductPropertyInfo(productInfo);
        }

        /// <summary>
        /// 批量更新商品属性信息
        /// </summary>
        /// <param name="requestMsg"></param>
        [WebInvoke(UriTemplate = "/Product/BatchUpdateProductPropertyInfo", Method = "PUT")]
        public void BatchUpdateProductPropertyInfo(ProductBatchUpdateRequestMsg requestMsg)
        {
            ObjectFactory<ProductAppService>.Instance.BatchUpdateProductPropertyInfo(requestMsg.ProductInfo, requestMsg.BatchUpdateProductSysNoList);
        }

        /// <summary>
        /// 更新商品自动调价信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductAutoPriceInfo", Method = "PUT")]
        public void UpdateProductAutoPriceInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductAutoPriceInfo(productInfo);
        }

        /// <summary>
        /// 更新商品质保信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductWarrantyInfo", Method = "PUT")]
        public void UpdateProductWarrantyInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductWarrantyInfo(productInfo);
        }

        /// <summary>
        /// 批量更新商品质保信息
        /// </summary>
        /// <param name="requestMsg"></param>
        [WebInvoke(UriTemplate = "/Product/BatchUpdateProductWarrantyInfo", Method = "PUT")]
        public void BatchUpdateProductWarrantyInfo(ProductBatchUpdateRequestMsg requestMsg)
        {
            ObjectFactory<ProductAppService>.Instance.BatchUpdateProductWarrantyInfo(requestMsg.ProductInfo, requestMsg.BatchUpdateProductSysNoList);
        }

        /// <summary>
        /// 更新商品重量信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductDimensionInfo", Method = "PUT")]
        public void UpdateProductDimensionInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductDimensionInfo(productInfo);
        }

        /// <summary>
        /// 批量更新商品重量信息
        /// </summary>
        /// <param name="requestMsg"></param>
        [WebInvoke(UriTemplate = "/Product/BatchUpdateProductDimensionInfo", Method = "PUT")]
        public void BatchUpdateProductDimensionInfo(ProductBatchUpdateRequestMsg requestMsg)
        {
            ObjectFactory<ProductAppService>.Instance.BatchUpdateProductDimensionInfo(requestMsg.ProductInfo, requestMsg.BatchUpdateProductSysNoList);
        }

        /// <summary>
        /// 更新商品销售区域信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductSalesAreaInfo", Method = "PUT")]
        public void UpdateProductSalesAreaInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductSalesAreaInfo(productInfo);
        }
        /// <summary>
        /// 批量设置商品销售区域信息
        /// </summary>
        /// <param name="listProductInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductSalesAreaInfoByList", Method = "PUT")]
        public void UpdateProductSalesAreaInfoByList(List<ProductInfo> listProductInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductSalesAreaInfoByList(listProductInfo);
        }
        /// <summary>
        /// 更新商品采购相关信息
        /// </summary>
        /// <param name="productInfo"></param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductPurchaseInfo", Method = "PUT")]
        public void UpdateProductPurchaseInfo(ProductInfo productInfo)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductPurchaseInfo(productInfo);
        }

        ///// <summary>
        ///// 更新商品库存同步合作信息
        ///// </summary>
        ///// <param name="productInfo"></param>
        //[WebInvoke(UriTemplate = "/Product/UpdateProductThirdPartyInventory", Method = "PUT")]
        //public void UpdateProductThirdPartyInventory(ProductInfo productInfo)
        //{
        //    ObjectFactory<ProductAppService>.Instance.UpdateProductThirdPartyInventory(productInfo);
        //}

        /// <summary>
        /// 商品属性复制
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/Product/ProductCopyProperty", Method = "PUT")]
        public void ProductCopyProperty(ProductCopyPropertyRequestMsg request)
        {
            ObjectFactory<ProductAppService>.Instance.ProductCopyProperty(request.SourceProductSysNo, request.TargetProductSysNo, request.CanOverrite, request.CompanyCode, request.LanguageCode, request.OperationUser);
        }

        #endregion

        #region CreateProduct

        /// <summary>
        /// 创建单个商品
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/CreateProduct", Method = "POST")]
        public ProductInfo CreateProduct(ProductInfo productInfo)
        {
            var productGroup = new ProductGroup
                                   {
                                       ProductGroupName = productInfo.ProductBasicInfo.ProductTitle,
                                       ProductGroupModel = productInfo.ProductBasicInfo.ProductModel,
                                       ProductGroupSettings = new List<ProductGroupSettings>(),
                                       ProductList = new List<ProductInfo> { productInfo },
                                       OperateUser = productInfo.OperateUser,
                                       CompanyCode = productInfo.CompanyCode,
                                       LanguageCode = productInfo.LanguageCode,
                                   };
            var dict = ObjectFactory<ProductAppService>.Instance.ProductCreate(productGroup);
            if (dict.Count > 0)
            {
                String exceptionMsg;
                if (dict.TryGetValue(productInfo.GetHashCode(), out exceptionMsg))
                {
                    throw new BizException(exceptionMsg);
                }
            }
            return productInfo;
        }

        /// <summary>
        /// 创建相似商品
        /// </summary>
        /// <param name="productGroup"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/CreateSimilarProduct", Method = "POST")]
        public CreateSimilerProductRsp CreateSimilarProduct(ProductGroup productGroup)
        {
            var dict = ObjectFactory<ProductAppService>.Instance.ProductCreate(productGroup);

            var successProductList = new List<ProductInfo>();

            productGroup.ProductList.Where(p => !dict.ContainsKey(p.GetHashCode())).ForEach(p =>
            {
                var successProduct = ObjectFactory<ProductAppService>.Instance.GetProductInfo(p.SysNo);
                successProductList.Add(successProduct);
            });

            var errorProductList = new List<ErrorProduct>();

            productGroup.ProductList.Where(p => dict.ContainsKey(p.GetHashCode())).ForEach(p =>
            {
                var errorProduct = new ErrorProduct
                                       {
                                           ProductTitle = p.ProductBasicInfo.ProductTitle.Content,
                                           ErrorMsg = dict[p.GetHashCode()]
                                       };
                errorProductList.Add(errorProduct);
            });

            return new CreateSimilerProductRsp
                       {
                           SuccessProductList = successProductList,
                           ErrorProductList = errorProductList
                       };
        }

        /// <summary>
        /// 商品克隆
        /// </summary>
        /// <param name="productCloneRequest"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/ProductClone", Method = "POST")]
        public ProductCloneRsp ProductClone(ProductCloneRequestMsg productCloneRequest)
        {
            var productCloneRsp = new ProductCloneRsp
                                      {
                                          ErrorProductList = new List<CloneErrorProduct>(),
                                          SuccessProductList = new List<CloneSuccessProduct>()
                                      };

            productCloneRequest.ProductIDList.ForEach(p =>
            {
                try
                {
                    var targetProductID = ObjectFactory<ProductAppService>
                        .Instance.ProductClone(p, productCloneRequest.CloneType
                        , productCloneRequest.CompanyCode
                        , productCloneRequest.LanguageCode
                        , productCloneRequest.OperateUser);
                    productCloneRsp.SuccessProductList.Add(new CloneSuccessProduct
                                                               {
                                                                   SourceProductID = p,
                                                                   TargetProductID = targetProductID
                                                               });
                }
                catch (BizException exception)
                {
                    productCloneRsp.ErrorProductList.Add(new CloneErrorProduct
                                                             {
                                                                 ProductID = p,
                                                                 ErrorMsg = exception.Message
                                                             });
                }
            });

            return productCloneRsp;
        }

        #endregion

        #region 退换货相关
        [WebInvoke(UriTemplate = "/Product/GetProductRMAPolicyByProductSysNo", Method = "POST")]
        public ProductRMAPolicyInfo GetProductRMAPolicyByProductSysNo(int? productSysNo)
        {
            return ObjectFactory<ProductRmaPolicyAppService>.Instance.GetProductRMAPolicyByProductSysNo(productSysNo);
        }
        [WebInvoke(UriTemplate = "/Product/CreateProductRMAPolicy", Method = "POST")]
        public void CreateProductRMAPolicy(ProductRMAPolicyInfo info)
        {
            ObjectFactory<ProductRmaPolicyAppService>.Instance.CreateProductRMAPolicy(info);
        }

        [WebInvoke(UriTemplate = "/Product/UpdateProductRMAPolicy", Method = "PUT")]
        public void UpdateProductRMAPolicy(ProductRMAPolicyInfo info)
        {
            ObjectFactory<ProductRmaPolicyAppService>.Instance.UpdateProductRMAPolicy(info);
        }

        #endregion

        /// <summary>
        /// 获取商品产地列表
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Product/GetProductCountryList")]
        public List<ProductCountry> GetProductCountryList()
        {
            return ObjectFactory<ProductAppService>.Instance.GetProductCountryList();
        }

        /// <summary>
        /// 更新商品采购价格
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="virtualPrice">采购价格</param>
        [WebInvoke(UriTemplate = "/Product/UpdateProductVirtualPrice", Method = "PUT")]
        public void UpdateProductVirtualPrice(ProductVirtualPriceReq req)
        {
            ObjectFactory<ProductAppService>.Instance.UpdateProductVirtualPrice(req.ProductSysNo, req.VirtualPrice);
        }


        /// <summary>
        /// 商品批量备案操作
        /// </summary>
        /// <param name="req"></param>
        [WebInvoke(UriTemplate = "/Product/ProductBatchEntry", Method = "PUT")]
        public void ProductBatchEntry(ProductBatchEntryReq req)
        {
            ObjectFactory<ProductAppService>.Instance.ProductBatchEntry(req.ProductSysNoList, req.Note, req.EntryStatus, req.EntryStatusEx);
        }
    }



}

