using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.IM.BizProcessor.IMAOP;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductAppService))]
    public partial class ProductAppService
    {

        private readonly ProductProcessor _productProcessor = ObjectFactory<ProductProcessor>.Instance;

        #region QueryProduct

        public virtual ProductInfo GetProductInfo(int productSysNo)
        {
            return _productProcessor.GetProductInfo(productSysNo);
        }

        public List<ProductInfo> GetProductInfoListByProductSysNoList(List<int> productSysNoList)
        {
            return productSysNoList.Select(productSysNo => ObjectFactory<ProductProcessor>.Instance.GetProductInfo(productSysNo)).Where(productInfo => productInfo != null).ToList();
        }

        public virtual ProductInfo GetProductInfoByID(string productID)
        {
            if (!String.IsNullOrEmpty(productID))
            {
                return _productProcessor.GetProductInfoByID(productID);
            }
            throw new ArgumentException("GetProductInfoByID Args Empty");
        }

        #endregion

        #region OnSaleProduct

        public virtual int ProductOnSale(int productSysNo)
        {
            return _productProcessor.ProductOnSale(productSysNo);
        }

        public virtual int ProductOnShow(int productSysNo)
        {
            return _productProcessor.ProductOnShow(productSysNo);
        }

        public virtual int ProductUnShow(int productSysNo)
        {
            return _productProcessor.ProductUnShow(productSysNo);
        }

        public virtual int ProductInvalid(int productSysNo)
        {
            return _productProcessor.ProductInvalid(productSysNo);
        }

        #endregion

        #region UpdateProduct

        public virtual void UpdateProductBasicInfo(ProductInfo productInfo, ref ProductManagerInfo productManager)
        {
            #region PreCheck

            if (!productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
            {
                throw new BizException("请指定该商品三级类别");
            }

            if (productInfo.ProductBasicInfo.ProductManager.SysNo == 0)
            {
                throw new BizException("请指定该商品PM");
            }

            if (StringUtility.CheckHtml(productInfo.ProductBasicInfo.ProductTitle.Content) || StringUtility.CheckInputType(productInfo.ProductBasicInfo.ProductTitle.Content))
            {
                throw new BizException("商品标题不能含有HTML标签/不能含有全角字符");
            }

            if (StringUtility.CheckHtml(productInfo.ProductBasicInfo.ProductBriefTitle.Content) || StringUtility.CheckInputType(productInfo.ProductBasicInfo.ProductBriefTitle.Content))
            {
                throw new BizException("商品简名不能含有HTML标签/不能含有全角字符");
            }

            if (StringUtility.CheckHtml(productInfo.ProductBasicInfo.ProductBriefAddition.Content) || StringUtility.CheckInputType(productInfo.ProductBasicInfo.ProductBriefAddition.Content))
            {
                throw new BizException("商品简名附加不能含有HTML标签/不能含有全角字符");
            }

            if (String.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductBriefTitle.Content) && String.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductBriefAddition.Content))
            {
                throw new BizException("商品简名和商品简名附加不能同时为空");
            }

            #endregion

            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductBasicInfo(productInfo, ref productManager);
                tran.Complete();
            }
        }

        public virtual void BatchUpdateProductBasicInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.BatchUpdateProductBasicInfo(productInfo, batchUpdateProductSysNoList);
                tran.Complete();
            }
        }

        public virtual void UpdateProductDescriptionInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductDescriptionInfo(productInfo);
                tran.Complete();
            }
        }

        public virtual void BatchUpdateProductDescriptionInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.BatchUpdateProductDescriptionInfo(productInfo, batchUpdateProductSysNoList);
                tran.Complete();
            }
        }

        public virtual void UpdateProductAccessoryInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductAccessoryInfo(productInfo);
                tran.Complete();
            }
        }

        public virtual void BatchUpdateProductAccessoryInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.BatchUpdateProductAccessoryInfo(productInfo, batchUpdateProductSysNoList);
                tran.Complete();
            }
        }

        public virtual void UpdateProductImageInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductImageInfo(productInfo);
                tran.Complete();
            }
        }
        [ProductInfoChange]
        public virtual string UpdateProductPropertyInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                var result = _productProcessor.UpdateProductPropertyInfo(productInfo);
                tran.Complete();
                return result;
            }
        }

        public virtual void BatchUpdateProductPropertyInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.BatchUpdateProductPropertyInfo(productInfo, batchUpdateProductSysNoList);
                tran.Complete();
            }
        }

        public virtual void UpdateProductAutoPriceInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductAutoPriceInfo(productInfo);
                tran.Complete();
            }
        }

        public virtual void UpdateProductWarrantyInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductWarrantyInfo(productInfo);
                tran.Complete();
            }
        }

        public virtual void BatchUpdateProductWarrantyInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.BatchUpdateProductWarrantyInfo(productInfo, batchUpdateProductSysNoList);
                tran.Complete();
            }
        }

        public virtual void UpdateProductDimensionInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductDimensionInfo(productInfo);
                tran.Complete();
            }
        }

        public virtual void BatchUpdateProductDimensionInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.BatchUpdateProductDimensionInfo(productInfo, batchUpdateProductSysNoList);
                tran.Complete();
            }
        }

        public virtual void UpdateProductSalesAreaInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductSalesAreaInfo(productInfo);
                tran.Complete();
            }
        }
        /// <summary>
        /// 批量设置商品销售区域信息
        /// </summary>
        /// <param name="listProductInfo"></param>
        public virtual void UpdateProductSalesAreaInfoByList(List<ProductInfo> listProductInfo)
        {

            _productProcessor.BatchUpdateProductSalesAreaInfo(listProductInfo);
        }

        public virtual void UpdateProductPurchaseInfo(ProductInfo productInfo)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.UpdateProductPurchaseInfo(productInfo);
                tran.Complete();
            }
        }

        //public virtual void UpdateProductThirdPartyInventory(ProductInfo productInfo)
        //{
        //    using (var tran = new TransactionScope())
        //    {
        //        _productProcessor.UpdateProductThirdPartyInventory(productInfo);
        //        tran.Complete();
        //    }
        //}

        #endregion

        #region CreateProduct

        public Dictionary<int, string> ProductCreate(ProductGroup productGroup)
        {
            return _productProcessor.ProductCreate(productGroup);
        }

        public string ProductClone(string productID, ProductCloneType cloneType, string companyCode, string languageCode, UserInfo operationUser)
        {
            return _productProcessor.ProductClone(productID, cloneType, companyCode, languageCode, operationUser);
        }

        #endregion

        public virtual void ProductCopyProperty(int? sourceProductSysNo, int? targetProductSysNo, bool canOverrite, string companyCode, string languageCode, UserInfo operationUser)
        {
            using (var tran = new TransactionScope())
            {
                _productProcessor.ProductCopyProperty(sourceProductSysNo, targetProductSysNo, canOverrite, companyCode, languageCode, operationUser);
                tran.Complete();
            }
        }

        /// <summary>
        /// 获取包含分仓库存的数据
        /// </summary>
        /// <param name="product"></param>
        public virtual void GetInventoryInfoByStock(DataTable product)
        {
            _productProcessor.GetInventoryInfoByStock(product);
        }

        /// <summary>
        /// 商品查询添加其他信息
        /// </summary>
        /// <param name="product"></param>
        public virtual void AddOtherData(DataTable product)
        {
            _productProcessor.AddOtherData(product);
        }


        /// <summary>
        /// 清理数据
        /// </summary>
        /// <param name="product"></param>
        /// <param name="totalCount"></param>
        public virtual void ClearProduct(DataTable product, ref int totalCount)
        {
            _productProcessor.ClearProduct(product, ref totalCount);
        }

        /// <summary>
        /// 获取商品产地列表
        /// </summary>
        /// <returns></returns>
        public virtual List<ProductCountry> GetProductCountryList()
        {
            return _productProcessor.GetProductCountryList();
        }

        /// <summary>
        /// 更新商品采购价格
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="virtualPrice">采购价格</param>
        public void UpdateProductVirtualPrice(string productSysNo, string virtualPrice)
        {
            _productProcessor.UpdateProductVirtualPrice(productSysNo, virtualPrice);
        }

        public void ProductBatchEntry(List<int> productSysNoList, string Note, ProductEntryStatus entryStatus, ProductEntryStatusEx entryStatusEx)
        {
            if (productSysNoList == null || productSysNoList.Count < 1)
            {
                return;
            }
            else
            {
                List<BatchActionItem<int>> actionItemList = new List<BatchActionItem<int>>();
                foreach (int productsysno in productSysNoList)
                {
                    BatchActionItem<int> item = new BatchActionItem<int>();
                    item.Data = productsysno;
                    item.ID = productsysno.ToString();
                    actionItemList.Add(item);
                }

                var resutl = BatchActionManager.DoBatchAction<int, BizException>(actionItemList, (productsysno) =>
                {
                    ProductEntryInfo entryInfo = new ProductEntryInfo();
                    entryInfo.ProductSysNo = productsysno;
                    entryInfo.AuditNote = Note;
                    switch (entryStatus)
                    {
                        case ProductEntryStatus.AuditFail:
                            {
                                ObjectFactory<ProductEntryInfoProcessor>.NewInstance().AuditFail(entryInfo);
                                break;
                            }
                        case ProductEntryStatus.AuditSucess:
                            {
                                ObjectFactory<ProductEntryInfoProcessor>.NewInstance().AuditSucess(entryInfo);
                                break;
                            }
                        default:
                            {
                                switch (entryStatusEx)
                                {
                                    case ProductEntryStatusEx.Inspection:
                                        {
                                            ObjectFactory<ProductEntryInfoProcessor>.NewInstance().ToInspection(entryInfo);
                                            break;
                                        }
                                    case ProductEntryStatusEx.InspectionFail:
                                        {
                                            ObjectFactory<ProductEntryInfoProcessor>.NewInstance().InspectionFail(entryInfo);
                                            break;
                                        }
                                    case ProductEntryStatusEx.InspectionSucess:
                                        {
                                            ObjectFactory<ProductEntryInfoProcessor>.NewInstance().InspectionSucess(entryInfo);
                                            break;
                                        }
                                    case ProductEntryStatusEx.Customs:
                                        {
                                            ObjectFactory<ProductEntryInfoProcessor>.NewInstance().ToCustoms(entryInfo);
                                            break;
                                        }
                                    case ProductEntryStatusEx.CustomsFail:
                                        {
                                            ObjectFactory<ProductEntryInfoProcessor>.NewInstance().CustomsFail(entryInfo);
                                            break;
                                        }
                                    case ProductEntryStatusEx.CustomsSuccess:
                                        {
                                            ObjectFactory<ProductEntryInfoProcessor>.NewInstance().CustomsSuccess(entryInfo);
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                });
                if(string.IsNullOrWhiteSpace(resutl.PromptMessage))
                {
                    return;
                }
                else
                {
                    throw new BizException(resutl.PromptMessage);
                }

            }
        }
    }
}
