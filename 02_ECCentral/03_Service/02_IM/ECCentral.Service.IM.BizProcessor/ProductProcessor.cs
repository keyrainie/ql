using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.IM.BizProcessor.IMAOP;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ValidStatus = ECCentral.BizEntity.IM.ValidStatus;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductProcessor))]
    public class ProductProcessor
    {
        private readonly IProductDA _productDA = ObjectFactory<IProductDA>.Instance;
        private readonly IProductResourceDA _productResourceDA = ObjectFactory<IProductResourceDA>.Instance;
        private readonly IProductCommonInfoDA _productCommonInfoDA = ObjectFactory<IProductCommonInfoDA>.Instance;
        private readonly IProductGroupDA _productGroupDA = ObjectFactory<IProductGroupDA>.Instance;
        private readonly ICategoryPropertyDA _categoryPropertyDA = ObjectFactory<ICategoryPropertyDA>.Instance;
        private readonly ICommonSKUNumberDA _commonSkuNumberDA = ObjectFactory<ICommonSKUNumberDA>.Instance;
        private readonly ICategorySettingDA _categorySettingDA = ObjectFactory<ICategorySettingDA>.Instance;
        private readonly IMKTBizInteract _iMKTBizInteract = ObjectFactory<IMKTBizInteract>.Instance;
        private readonly IPOBizInteract _poBizInteract = ObjectFactory<IPOBizInteract>.Instance;
        private readonly ICategoryDA _categoryDA = ObjectFactory<ICategoryDA>.Instance;
        private readonly IPropertyDA _propertyDA = ObjectFactory<IPropertyDA>.Instance;
        private readonly IBrandDA _brandDA = ObjectFactory<IBrandDA>.Instance;
        private readonly IProductEntryInfoDA _entryInfoDA = ObjectFactory<IProductEntryInfoDA>.Instance;

        #region 获取商品信息

        /// <summary>
        /// 获取商品的组系统编号
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns></returns>
        public int GetProductGroupSysNo(int productSysNo)
        {
            return _productDA.GetProductGroupSysNo(productSysNo);
        }

        /// <summary>
        /// 根据商品编号获取商品完整信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual ProductInfo GetProductInfo(int productSysNo)
        {
            return _productDA.GetProductInfoBySysNo(productSysNo);
        }

        /// <summary>
        /// 获取商品完整信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public virtual ProductInfo GetProductInfoByID(string productID)
        {
            return _productDA.GetProductInfoByID(productID);
        }

        public virtual ProductInfo GetSimpleProductInfo(int productSysNo)
        {
            return _productDA.GetSimpleProductInfoBySysNo(productSysNo);
        }

        public virtual List<ProductInfo> GetSimpleProductListByID(string productID)
        {
            return _productDA.GetSimpleProductListByID(productID);
        }
        #endregion

        #region 创建商品
        [ProductInfoChange]
        public Dictionary<int, string> ProductCreate(ProductGroup productGroup)
        {
            var errorDict = new Dictionary<int, string>();

            if (productGroup.SysNo.HasValue)
            {
                var group = ObjectFactory<ProductGroupProcessor>.Instance.GetProductGroupInfoBySysNo(productGroup.SysNo.Value);

                if (group == null || !group.SysNo.HasValue)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductGroupInfoNotExists"));
                }
                group.OperateUser = productGroup.OperateUser;
                group.CompanyCode = productGroup.CompanyCode;
                group.LanguageCode = productGroup.LanguageCode;
                productGroup.ProductList.ForEach(p => group.ProductList.Add(p));
                productGroup.ProductGroupName = group.ProductGroupName;
                productGroup = group;
            }

            productGroup.ProductList.Where(p => p.SysNo == 0).ForEach(product =>
            {
                var checkResult = ProductInfoCheck(product);
                if (checkResult.Length > 0)
                {
                    if (!errorDict.ContainsKey(product.GetHashCode()))
                    {
                        errorDict.Add(product.GetHashCode(), checkResult.ToString());
                    }
                }
            });

            var currectProductInfoCheckedProductList = productGroup.ProductList.Where(p => !errorDict.ContainsKey(p.GetHashCode())).ToList();

            if (productGroup.ProductGroupSettings.Any() && currectProductInfoCheckedProductList.Count > 1)
            {
                productGroup.ProductList.ForEach(productInfo =>
                {
                    var groupSettingCheckResult = new StringBuilder();
                    if (productGroup.SysNo.HasValue && productGroup.ProductList.Count > 1)
                    {
                        if (productGroup.ProductGroupSettings.Any(setting => !productInfo.ProductBasicInfo.ProductProperties.Any(
                            property =>
                            property.Property.PropertyInfo.SysNo == setting.ProductGroupProperty.SysNo &&
                            property.Property.SysNo != 0)))
                        {
                            groupSettingCheckResult.Append((ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                                                   IMConst.IMProductMRProductInfoGroupPropertyEmpty)));
                        }

                        var productGroupExcept =
                            productGroup.ProductList.Where(p => p != productInfo).ToList();

                        var groupPropertySysNoList = productGroup.ProductGroupSettings.Select(setting => setting.ProductGroupProperty.SysNo).ToList();

                        var productGroupPropertyValue = productInfo.ProductBasicInfo.ProductProperties.Where(
                            property => groupPropertySysNoList.Contains(property.Property.PropertyInfo.SysNo)).Select(
                                property => property.Property.SysNo).Join("|");

                        if (productGroupExcept.Select(p => p.ProductBasicInfo.ProductProperties.Where(
                            property => groupPropertySysNoList.Contains(property.Property.PropertyInfo.SysNo)).Select(
                                property => property.Property.SysNo).Join("|")).Any(d => d == productGroupPropertyValue))
                        {
                            groupSettingCheckResult.Append((ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                                                   IMConst.IMProductMRProductInfoGroupPropertyDuplicate)));
                        }
                    }
                    if (groupSettingCheckResult.Length > 0)
                    {
                        if (!errorDict.ContainsKey(productInfo.GetHashCode()) && productInfo.SysNo == 0)
                        {
                            errorDict.Add(productInfo.GetHashCode(), groupSettingCheckResult.ToString());
                        }
                    }
                });
            }

            var currectCheckedForCreateProductList = productGroup.ProductList.Where(p => !errorDict.ContainsKey(p.GetHashCode()) && p.SysNo == 0).ToList();

            if (currectCheckedForCreateProductList.Any())
            {
                if (!productGroup.SysNo.HasValue)
                {
                    _productGroupDA.CreateProductGroupInfo(productGroup);
                    _productGroupDA.CreateGroupPropertySetting(productGroup);
                }

                productGroup.ProductList = currectCheckedForCreateProductList;

                //传入商品信息带有ProductID则 保持原ID。
                if (productGroup.ProductList.Any(item => string.IsNullOrWhiteSpace(item.ProductID)))
                {
                    BuildCommonSkuNumber(productGroup);
                }


                productGroup.ProductList.ForEach(product =>
                {
                    try
                    {
                        using (var tran = new TransactionScope())
                        {
                            if (String.IsNullOrEmpty(product.ProductBasicInfo.Keywords.Content))
                            {
                                product.ProductBasicInfo.Keywords = new LanguageContent(CommonUtility.WordSegment(product.ProductName).Join(" "));
                            }

                            _productCommonInfoDA.InsertProductCommonInfo(productGroup.SysNo, product);

                            _productDA.InsertProductInfo(product);

                            //插入产地信息
                            _productDA.InsertProductEntryInfo(product.SysNo, product.OrginCode);

                            product.ProductBasicInfo.Performance = BuildProductPerformance(product.SysNo, product.ProductBasicInfo.ProductProperties);

                            _productCommonInfoDA.UpdateProductCommonInfoPerformance(product, product.OperateUser);
                            #region 更新商品描述
                            SetProductDescription(product);
                            _productCommonInfoDA.UpdateProductCommonInfoDesc(product, product.OperateUser);
                            #endregion

                            #region 初始化商品的退换货和保修
                            ProductRMAPolicyInfo rmaPolicyInfo = ObjectFactory<RmaPolicyProcessor>.Instance.MakeRMAPolicyEntity((int)product.ProductBasicInfo.ProductCategoryInfo.SysNo, (int)product.ProductBasicInfo.ProductBrandInfo.SysNo);
                            rmaPolicyInfo.CompanyCode = product.CompanyCode;
                            rmaPolicyInfo.LanguageCode = product.LanguageCode;
                            rmaPolicyInfo.ProductSysNo = product.SysNo;
                            rmaPolicyInfo.User = product.OperateUser;
                            ObjectFactory<ProductRMAPolicyProcessor>.Instance.CreateProductRMAPolicy(rmaPolicyInfo);
                            #endregion

                            if (!ExternalDomainBroker.InitProductInventoryInfo(product.SysNo))
                            {
                                throw new BizException(ResouceManager.GetMessageString("IM.Product",
                                                                                       "InitProductInventoryError"));
                            }
                            tran.Complete();
                            //Todo:图片处理
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!errorDict.ContainsKey(product.GetHashCode()))
                        {
                            errorDict.Add(product.GetHashCode(), ex.Message);
                        }
                    }
                });
            }
            return errorDict;
        }

        public string ProductClone(string productID, ProductCloneType cloneType, string companyCode
            , string languageCode, UserInfo operationUser)
        {
            var product = ProductCloneCheck(productID, cloneType);
            product.OperateUser = operationUser;
            product.CompanyCode = companyCode;
            product.LanguageCode = languageCode;
            product.ProductBasicInfo.ProductProperties.ForEach(p =>
            {
                p.CompanyCode = companyCode;
                p.LanguageCode = languageCode;
            });
            product.ProductStatus = ProductStatus.InActive_UnShow;
            if (cloneType == ProductCloneType.OpenBox)
            {
                product.ProductBasicInfo.ProductType = ProductType.OpenBox;
                product.ProductBasicInfo.ProductTitle =
                    new LanguageContent(product.ProductBasicInfo.ProductTitle.Content + "(二手品)");
            }
            product.ProductBasicInfo.ProductProperties =
                product.ProductBasicInfo.ProductProperties.Where(p => p.Property.SysNo.HasValue).ToList();
            product.ProductBasicInfo.ProductResources.ForEach(r =>
            {
                r.Resource.OperateUser = operationUser;
                r.CompanyCode = companyCode;
                r.LanguageCode = languageCode;
                r.OperateUser = operationUser;
            });

            var productGroup = new ProductGroup
            {
                ProductGroupName = product.ProductBasicInfo.ProductTitle,
                ProductGroupModel = product.ProductBasicInfo.ProductModel,
                ProductGroupSettings = new List<ProductGroupSettings>(),
                ProductList = new List<ProductInfo> { product },
                OperateUser = operationUser,
                CompanyCode = companyCode,
                LanguageCode = languageCode
            };

            BuildCloneCommonSkuNumber(product, cloneType);

            using (var tran = new TransactionScope())
            {
                _productGroupDA.CreateProductGroupInfo(productGroup);
                _productGroupDA.CreateGroupPropertySetting(productGroup);
                _productCommonInfoDA.InsertProductCommonInfo(productGroup.SysNo, product);
                _productDA.InsertProductInfo(product);
                product.ProductBasicInfo.Performance = BuildProductPerformance(product.SysNo, product.ProductBasicInfo.ProductProperties);
                _productCommonInfoDA.UpdateProductCommonInfoPerformance(product, operationUser);
                #region 更新商品描述
                SetProductDescription(product);
                _productCommonInfoDA.UpdateProductCommonInfoDesc(product, product.OperateUser);
                #endregion

                #region 初始化商品的退换货和保修
                ProductRMAPolicyInfo rmaPolicyInfo = ObjectFactory<RmaPolicyProcessor>.Instance.MakeRMAPolicyEntity((int)product.ProductBasicInfo.ProductCategoryInfo.SysNo, (int)product.ProductBasicInfo.ProductBrandInfo.SysNo);
                rmaPolicyInfo.CompanyCode = product.CompanyCode;
                rmaPolicyInfo.LanguageCode = product.LanguageCode;
                rmaPolicyInfo.ProductSysNo = product.SysNo;
                rmaPolicyInfo.User = product.OperateUser;
                ObjectFactory<ProductRMAPolicyProcessor>.Instance.CreateProductRMAPolicy(rmaPolicyInfo);
                #endregion


                _productDA.UpdateSourceProductID(productID, product.SysNo);

                if (product.ProductCommonInfoSysNo.HasValue)
                {
                    product.ProductBasicInfo.ProductResources
                        .ForEach(r => _productResourceDA.InsertProductResource(r, product.ProductCommonInfoSysNo.Value));
                }

                if (!ExternalDomainBroker.InitProductInventoryInfo(product.SysNo))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product",
                                                                           "InitProductInventoryError"));
                }
                tran.Complete();

            }
            return product.ProductID;
        }

        private string ProductInfoCheck(ProductInfo productInfo)
        {
            var result = new StringBuilder();
            if (!productInfo.Merchant.SysNo.HasValue)
            {
                result.Append(ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult1"));
            }

            if (!productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
            {
                result.Append(ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult2"));
            }

            if (!productInfo.ProductBasicInfo.ProductBrandInfo.SysNo.HasValue)
            {
                result.Append(ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult3"));
            }
            else
            {
                productInfo.ProductBasicInfo.ProductBrandInfo =
                    ObjectFactory<BrandProcessor>.Instance.GetBrandInfoBySysNo(
                        productInfo.ProductBasicInfo.ProductBrandInfo.SysNo.Value);
            }

            if (productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
            {
                CategorySetting categorySetting =
                            _categorySettingDA.GetCategorySettingBySysNo(
                                productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);

                productInfo.ProductBasicInfo.ProductDimensionInfo.LargeFlag =
                    categorySetting.CategoryBasicInfo.IsLargeInfo == IsLarge.Yes
                    ? Large.Yes
                    : Large.No;
            }

            if (productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue
                && productInfo.ProductBasicInfo.ProductBrandInfo.SysNo.HasValue)
            {
                var pm = ObjectFactory<ProductLineProcessor>.Instance.GetPMByC3(
                    productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value,
                    productInfo.ProductBasicInfo.ProductBrandInfo.SysNo.Value);
                if (pm != null)
                {
                    productInfo.ProductBasicInfo.ProductManager = pm;
                }
                else
                {
                    result.Append(ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult4"));
                }
            }

            if (String.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductTitle.Content))
            {
                result.Append(ResouceManager.GetMessageString("IM.Product", "ProductTitleEmpty"));
            }
            if (!String.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductTitle.Content))
            {
                if (productInfo.ProductBasicInfo.ProductTitle.Content.Length > 90)
                {
                    result.Append(ResouceManager.GetMessageString("IM.Product", "ProductTitleOutLength"));
                }
                if (StringUtility.CheckHtml(productInfo.ProductBasicInfo.ProductTitle.Content))
                {
                    result.Append(ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult5"));
                }
                if (StringUtility.CheckInputType(productInfo.ProductBasicInfo.ProductTitle.Content))
                {
                    result.Append(ResouceManager.GetMessageString("IM.Product", "ProductTitleQJChar"));
                }
            }

            //if (String.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductModel.Content))
            //{
            //    result.Append(ResouceManager.GetMessageString("IM.Product", "ProductModelEmpty"));
            //}
            //if (!String.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductModel.Content))
            //{
            //    if (StringUtility.CheckInputType(productInfo.ProductBasicInfo.ProductModel.Content))
            //    {
            //        result.Append(ResouceManager.GetMessageString("IM.Product", "ProductModelQJChar"));
            //    }
            //}
            if (!String.IsNullOrEmpty(productInfo.ProductBasicInfo.UPCCode))
            {
                if (StringUtility.CheckInputType(productInfo.ProductBasicInfo.UPCCode))
                {
                    result.Append(ResouceManager.GetMessageString("IM.Product", "ProductUPCCodeQJChar"));
                }
            }
            if (productInfo.ProductPriceInfo.VirtualPrice < 0)
            {
                result.Append(ResouceManager.GetMessageString("IM.Product", "ProductVirtualPriceXYZero"));
            }

            if (!String.IsNullOrEmpty(productInfo.ProductBasicInfo.Keywords.Content) && StringUtility.CheckInputType(productInfo.ProductBasicInfo.Keywords.Content))
            {
                result.Append(ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult6"));
            }

            if (productInfo.ProductBasicInfo.ProductDimensionInfo != null && (productInfo.ProductBasicInfo.ProductDimensionInfo.Length < 0
                || productInfo.ProductBasicInfo.ProductDimensionInfo.Width < 0
                || productInfo.ProductBasicInfo.ProductDimensionInfo.Height < 0))
            {
                result.Append(ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult7"));
            }
            if (_productDA.GetProductCountExcept(productInfo) > 0)
            {
                result.Append(ResouceManager.GetMessageString("IM.Product", "ExistsSameProductInfo"));
            }
            return result.ToString();
        }

        private ProductInfo ProductCloneCheck(string productID, ProductCloneType cloneType)
        {
            var product = _productDA.GetProductInfoByID(productID);
            if (product == null)
            {
                throw new BizException(productID + ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult8"));
            }

            if (product.ProductBasicInfo.ProductType != ProductType.Normal)
            {
                throw new BizException(productID + ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult9"));
            }

            if (!String.IsNullOrEmpty(product.SourceProductID))
            {
                throw new BizException(productID + ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult10"));
            }

            if (_productDA.GetProductInfoByID(productID + GetSixthLetter(cloneType)) != null)
            {
                throw new BizException(productID + ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult11") + EnumHelper.GetDescription(cloneType) + ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult12"));
            }

            if (product.Merchant.SysNo != 1)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductInfoCheckResult13"));
            }

            return product;
        }

        #region 生成ID

        private void BuildCommonSkuNumber_Old(ProductGroup productGroup)
        {
            var productInfo = productGroup.ProductList.First();

            var firstLetter = GetFirstLetter(productInfo);

            var secondLetter = GetSecondLetter(productInfo);

            var thirdLetter = GetThirdLetter(productInfo);

            var forthLetter = GetForthLetter(productInfo, productGroup);

            productGroup.ProductList.ForEach(product =>
            {
                var fifthLetter = GetFifthLetter(product, productGroup);
                if (fifthLetter == "01")
                {
                    fifthLetter = String.Empty;
                }

                var sixthLetter = GetSixthLetter(product);

                var seventhLetter = GetSeventhLetter(productInfo, firstLetter);

                product.ProductBasicInfo.CommonSkuNumber = firstLetter + secondLetter + "-" + thirdLetter + "-" +
                                                           forthLetter;
                if (!String.IsNullOrEmpty(fifthLetter))
                {
                    product.ProductBasicInfo.CommonSkuNumber += "-" + fifthLetter;
                }

                if (!String.IsNullOrEmpty(sixthLetter))
                {
                    product.ProductBasicInfo.CommonSkuNumber += sixthLetter;
                }

                product.ProductID = product.ProductBasicInfo.CommonSkuNumber;

                if (!String.IsNullOrEmpty(seventhLetter))
                {
                    product.ProductID += "_" + seventhLetter;
                }
            });
        }
        private void BuildCommonSkuNumber(ProductGroup productGroup)
        {
            var productInfo = productGroup.ProductList.First();
            var brandInfo = _brandDA.GetBrandInfoBySysNo(productInfo.ProductBasicInfo.ProductBrandInfo.SysNo.Value);
            var categoryInfo = _categoryDA.GetCategoryBySysNo(productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);

            string code = string.Format("{0}_{1}_{2}_", productInfo.OrginCode.Trim(), categoryInfo.C3Code.Trim(), brandInfo.BrandCode.Trim());
            string maxProductID = _productDA.GetProductSameIDMaxProductID(code);
            int maxSerialNumber = string.IsNullOrWhiteSpace(maxProductID) ? 0 : int.Parse(maxProductID.Replace(code, ""));

            productGroup.ProductList.ForEach(product =>
            {
                maxSerialNumber++;
                switch (maxSerialNumber.ToString().Length)
                {
                    case 1:
                        product.ProductID = string.Format("{0}000{1}", code, maxSerialNumber);
                        break;
                    case 2:
                        product.ProductID = string.Format("{0}00{1}", code, maxSerialNumber);
                        break;
                    case 3:
                        product.ProductID = string.Format("{0}0{1}", code, maxSerialNumber);
                        break;
                    case 4:
                        product.ProductID = string.Format("{0}{1}", code, maxSerialNumber);
                        break;
                }
                product.ProductBasicInfo.CommonSkuNumber = product.ProductID;
            });
        }

        private void BuildCloneCommonSkuNumber(ProductInfo productInfo, ProductCloneType cloneType)
        {
            var firstLetter = GetFirstLetter(productInfo);

            var sixthLetter = GetSixthLetter(cloneType);

            var seventhLetter = GetSeventhLetter(productInfo, firstLetter);

            //获取克隆次数
            var cloneCount = GetCloneCount(productInfo.ProductID, sixthLetter).ToString("00");

            if (int.Parse(cloneCount) >= 100)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "BuildCloneCommonSkuNumberResult"));
            }

            productInfo.ProductBasicInfo.CommonSkuNumber = productInfo.ProductBasicInfo.CommonSkuNumber + sixthLetter + cloneCount;

            productInfo.ProductID = productInfo.ProductBasicInfo.CommonSkuNumber;

            if (!String.IsNullOrEmpty(seventhLetter))
            {
                productInfo.ProductID += "_" + seventhLetter;
            }
        }

        private int GetCloneCount(string productID, string sixthLetter)
        {
            return _productDA.GetCloneCount(productID, sixthLetter);
        }

        private string GetFirstLetter(ProductInfo productInfo)
        {
            var firstLetter = "A";

            if (productInfo.Merchant.SysNo.HasValue)
            {
                var vendor = _poBizInteract.GetVendorInfoSysNo(productInfo.Merchant.SysNo.Value);
                if (vendor.SysNo.HasValue && vendor.VendorBasicInfo.VendorType == VendorType.VendorPortal)
                {
                    firstLetter = "S";
                }
            }
            else
            {
                productInfo.Merchant.SysNo = 1;
            }
            return firstLetter;
        }

        private string GetSecondLetter(ProductInfo productInfo)
        {
            string secondLetter;

            var categorySeries = _commonSkuNumberDA.GetCategorySeries(productInfo.ProductBasicInfo.ProductCategoryInfo);

            if (categorySeries == null || !categorySeries.SysNo.HasValue)
            {
                var start = 0;
                while (true)
                {
                    var availableCategorySeriesList = _commonSkuNumberDA.GetAvailableCategorySeries(start);
                    if (availableCategorySeriesList.Count == 0)
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetSecondLetterResult"));
                    }

                    foreach (var series in availableCategorySeriesList)
                    {
                        if (ContrainForbiddenChar(series.Number))
                        {
                            if (series.SysNo.HasValue)
                            {
                                start = series.SysNo.Value;
                            }
                        }
                        else
                        {
                            series.CategorySysNo = productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo;
                            _commonSkuNumberDA.UpdateCategorySeries(series);
                            secondLetter = series.Number;
                            return secondLetter;
                        }
                    }
                }
            }
            secondLetter = categorySeries.Number;
            return secondLetter;
        }

        private string GetThirdLetter(ProductInfo productInfo)
        {
            string thirdLetter;

            var brandSeries = _commonSkuNumberDA.GetBrandSeries(productInfo.ProductBasicInfo.ProductBrandInfo);

            if (brandSeries == null || !brandSeries.SysNo.HasValue)
            {
                var start = 0;

                while (true)
                {
                    var availableBrandSeriesList = _commonSkuNumberDA.GetAvailableBrandSeries(start);

                    if (availableBrandSeriesList.Count == 0)
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetThirdLetterResult"));
                    }

                    foreach (var series in availableBrandSeriesList)
                    {
                        if (ContrainForbiddenChar(series.Number))
                        {
                            if (series.SysNo.HasValue)
                            {
                                start = series.SysNo.Value;
                            }
                        }
                        else
                        {
                            series.BrandSysNo = productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo;
                            _commonSkuNumberDA.UpdateBrandSeries(series);
                            thirdLetter = series.Number;
                            return thirdLetter;
                        }
                    }
                }
            }
            thirdLetter = brandSeries.Number;
            return thirdLetter;
        }

        private string GetForthLetter(ProductInfo productInfo, ProductGroup productGroup)
        {
            int newSeriesNo;
            if (productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo == null || productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetForthLetterResult1"));
            }
            var category = _categoryDA.GetCategory3BySysNo(productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);
            if (category == null || category.ParentSysNumber == null || category.ParentSysNumber <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetForthLetterResult2"));
            }
            var modelSeriesList = _commonSkuNumberDA.GetModelSeriesList(category);

            if (!modelSeriesList.Any())
            {
                newSeriesNo = 1;
            }
            else
            {
                newSeriesNo = modelSeriesList.First().SeriesNo + 1;
            }

            while (true)
            {
                string modeSegment = DecimalConvertor.Decimal10To36(newSeriesNo);

                if (modeSegment.Length > 3)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetForthLetterResult3"));
                }
                modeSegment = modeSegment.PadLeft(3, '0');

                if (ContrainForbiddenChar(modeSegment))
                {
                    newSeriesNo++;
                    continue;
                }
                _commonSkuNumberDA.InsertModelSeries(newSeriesNo, productInfo.ProductBasicInfo.ProductCategoryInfo,
                                                     productInfo.ProductBasicInfo.ProductBrandInfo,
                                                     productGroup.ProductGroupModel.Content, productInfo.CompanyCode,
                                                     productInfo.LanguageCode);
                return modeSegment;
            }
        }

        private string GetFifthLetter(ProductInfo productInfo, ProductGroup productGroup)
        {
            if (productGroup.SysNo.HasValue)
            {
                IList<PropertySeries> propertySeriesList = _commonSkuNumberDA.GetPropertySeriesList(productGroup.SysNo.Value);
                int newSeriesNo;
                if (!propertySeriesList.Any())
                {
                    newSeriesNo = 1;
                }
                else
                {
                    newSeriesNo = propertySeriesList.Max(p => p.SeriesNo) + 1;
                }

                while (true)
                {
                    string propertySegment = newSeriesNo.ToString();

                    if (propertySegment.Length > 2)
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetFifthLetterResult1"));
                    }

                    propertySegment = propertySegment.PadLeft(2, '0');

                    if (ContrainForbiddenChar(propertySegment))
                    {
                        newSeriesNo++;
                        continue;
                    }
                    _commonSkuNumberDA.InsertPropertySeries(newSeriesNo, productGroup.SysNo.Value,
                                                            productInfo.ProductBasicInfo.ProductModel.Content,
                                                            productInfo.CompanyCode, productInfo.LanguageCode);

                    return propertySegment;
                }
            }
            throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetFifthLetterResult2"));
        }

        private string GetSixthLetter(ProductInfo productInfo)
        {
            string sixthLetter = String.Empty;

            switch (productInfo.ProductBasicInfo.ProductType)
            {
                case ProductType.OpenBox:
                    {
                        sixthLetter = "R";
                        break;
                    }
                case ProductType.Bad:
                    {
                        sixthLetter = "B";
                        break;
                    }
            }

            return sixthLetter;
        }

        private string GetSixthLetter(ProductCloneType productCloneType)
        {
            string sixthLetter = String.Empty;

            switch (productCloneType)
            {
                case ProductCloneType.OpenBox:
                    {
                        sixthLetter = "R";
                        break;
                    }
                case ProductCloneType.Gifts:
                    {
                        sixthLetter = "Z";
                        break;
                    }
                case ProductCloneType.Auction:
                    {
                        sixthLetter = "P";
                        break;
                    }
                case ProductCloneType.Bad:
                    {
                        sixthLetter = "B";
                        break;
                    }
            }

            return sixthLetter;
        }

        private string GetSeventhLetter(ProductInfo productInfo, string firstLetter)
        {
            string seventhLetter = String.Empty;
            if (firstLetter == "S")
            {
                if (productInfo.Merchant.SysNo.HasValue)
                {
                    var vendor = _poBizInteract.GetVendorInfoSysNo(productInfo.Merchant.SysNo.Value);
                    seventhLetter = vendor.VendorBasicInfo.SellerID;
                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetSeventhLetterResult"));
                }
            }
            return seventhLetter;
        }

        private static bool ContrainForbiddenChar(string input)
        {
            var forbiddenCharList = new List<String> { "I", "B", "O" };
            return forbiddenCharList.Any(input.Contains);
        }

        #endregion

        #endregion

        #region 商品上下架

        /// <summary>
        /// 商品上架
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [ProductInfoChange]
        public virtual int ProductOnSale(int productSysNo)
        {
            ProductInfo productInfo = _productDA.GetProductInfoBySysNo(productSysNo);

            if (ProductOnSaleCheck(productInfo))
            {
                productInfo.ProductStatus = ProductStatus.Active;
            }

            _productDA.UpdateProductOnlineTime(productSysNo);

            return _productDA.UpdateProductStatus(productSysNo, productInfo.ProductStatus);
        }

        /// <summary>
        /// 商品展示
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [ProductInfoChange]
        public virtual int ProductOnShow(int productSysNo)
        {
            ProductInfo productInfo = _productDA.GetProductInfoBySysNo(productSysNo);

            if (ProductOnShowCheck(productInfo))
            {
                productInfo.ProductStatus = ProductStatus.InActive_Show;
            }

            return _productDA.UpdateProductStatus(productSysNo, productInfo.ProductStatus);
        }

        /// <summary>
        /// 商品不展示
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [ProductInfoChange]
        public virtual int ProductUnShow(int productSysNo)
        {
            ProductInfo productInfo = _productDA.GetProductInfoBySysNo(productSysNo);

            productInfo.ProductStatus = ProductStatus.InActive_UnShow;

            return _productDA.UpdateProductStatus(productSysNo, productInfo.ProductStatus);
        }

        /// <summary>
        /// 商品作废
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [ProductInfoChange]
        public virtual int ProductInvalid(int productSysNo)
        {
            ProductInfo productInfo = _productDA.GetProductInfoBySysNo(productSysNo);

            productInfo.ProductStatus = ProductStatus.Abandon;

            return _productDA.UpdateProductStatus(productSysNo, productInfo.ProductStatus);
        }

        /// <summary>
        /// 商品上架销售验证
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        private bool ProductOnSaleCheck(ProductInfo productInfo)
        {
            return ProductOnShowCheck(productInfo);
        }

        /// <summary>
        /// 商品展示/下架验证
        /// 0.商品资料必须已完善
        /// 1.商品类别有效
        /// 2.商品品牌有效
        /// 3.商品名称不能为空
        /// 4.商品型号不能为空
        /// 5.商品短描述不能为空
        /// 6.商品长描述不能为空
        /// 7.商品有属性信息
        /// 8.商品有图片
        /// 9.商品市场价必须大于0
        /// 10.商品当前价不能为999999
        /// 11.商品重量必须大于0
        /// 12.商品不能是赠品
        /// 13.商品不能是附件
        /// 14.商品没有默认图片
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        private bool ProductOnShowCheck(ProductInfo productInfo)
        {
            var result = new StringBuilder();

            if (productInfo.ProductBasicInfo.ProductInfoFinishStatus == ProductInfoFinishStatus.No)
            {
                result.AppendLine(ResouceManager.GetMessageString("IM.Product", "ProductOnShowCheckResult1"));
            }
            if (String.IsNullOrEmpty(productInfo.ProductBasicInfo.DefaultImage))
            {
                result.AppendLine(ResouceManager.GetMessageString("IM.Product", "ProductOnShowCheckResult2"));
            }
            if (productInfo.ProductBasicInfo.ProductCategoryInfo == null || productInfo.ProductBasicInfo.ProductCategoryInfo.Status != CategoryStatus.Active)
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMCategoryMessageResourcesKey,
                                                              IMConst.IMCategoryMRCategoryInvalid));
            }

            if (productInfo.ProductBasicInfo.ProductBrandInfo == null || productInfo.ProductBasicInfo.ProductBrandInfo.Status != ValidStatus.Active)
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMBrandMessageResourcesKey,
                                                              IMConst.IMBrandMRCategoryInvalid));
            }

            if (string.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductTitle.Content))
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                              IMConst.IMProductMRProductTitleEmpty));
            }

            //if (string.IsNullOrEmpty(productInfo.ProductBasicInfo.ProductModel.Content))
            //{
            //    result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
            //                                                  IMConst.IMProductMRProductModelEmpty));
            //}

            if (string.IsNullOrEmpty(productInfo.ProductBasicInfo.ShortDescription.Content))
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                              IMConst.IMProductMRShortDescriptionEmpty));
            }

            if (string.IsNullOrEmpty(productInfo.ProductBasicInfo.LongDescription.Content))
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                              IMConst.IMProductMRLongDescriptionEmpty));
            }

            //if (productInfo.ProductBasicInfo.ProductProperties == null || productInfo.ProductBasicInfo.ProductProperties.Count(p => p.Property.SysNo.HasValue || !String.IsNullOrEmpty(p.PersonalizedValue.Content)) == 0)
            //{
            //    result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
            //                                                  IMConst.IMProductMRProductPropertiesEmpty));
            //}

            if (productInfo.ProductBasicInfo.ProductResources == null || !productInfo.ProductBasicInfo.ProductResources.Any(resource => resource.Resource.Type == ResourcesType.Image && resource.Resource.Status == ProductResourceStatus.Active))
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                              IMConst.IMProductMRProductNoImages));
            }

            if (productInfo.ProductPriceInfo.BasicPrice < IMConst.ProductPriceZero)
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                              IMConst.IMProductMRProductBasicPriceInvalid));
            }

            if (productInfo.ProductPriceInfo.CurrentPrice == IMConst.ProductDefaultPrice)
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                              IMConst.IMProductMRProductCurrentPriceInvalid));
            }

            if (productInfo.ProductBasicInfo.ProductDimensionInfo == null || productInfo.ProductBasicInfo.ProductDimensionInfo.Weight <= IMConst.ProductWeightZero)
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                              IMConst.IMProductMRProductWeightInvalid));
            }

            if (_iMKTBizInteract.ProductIsGift(productInfo.SysNo))
            {
                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                                  IMConst.IMProductMRProductIsGift));
            }

            //取消备案验证功能
            //#region  非虚拟团购 商品上架需要检测是否有税则号和备案号

            //if (productInfo.ProductConsignFlag != VendorConsignFlag.GroupBuying)
            //{
            //    List<int> productSysNoList = new List<int>();
            //    productSysNoList.Add(productInfo.SysNo);
            //    ProductEntryInfo entryinfo = new ProductEntryInfo();
            //    List<ProductEntryInfo> productEntryList = _entryInfoDA.GetProductEntryInfoList(productSysNoList);
            //    if (productEntryList != null && productEntryList.Count > 0)
            //    {
            //        entryinfo = productEntryList.First();
            //    }
            //    if (entryinfo != null)
            //    {
            //        if (entryinfo.ProductTradeType != TradeType.Internal)
            //        {
            //            //税则信息检查
            //            if (string.IsNullOrEmpty(entryinfo.TariffCode))
            //            {
            //                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
            //                                                                                IMConst.IMProductTariffCode));
            //            }
            //            if (!entryinfo.TariffRate.HasValue)
            //            {
            //                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
            //                                                                                                  IMConst.IMProductTariffRate));
            //            }
            //            //备案号检查
            //            if (string.IsNullOrEmpty(entryinfo.EntryCode))
            //            {
            //                result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
            //                                                                                IMConst.IMProductEntryCode));
            //            }


            //            //如果是自贸区（保税进口）则需要检查是否填入了货号，物资序号，申报单位，申报数量，毛重，净重

            //            if (entryinfo.BizType == EntryBizType.BondedImport)
            //            {
            //                if (string.IsNullOrEmpty(entryinfo.Product_SKUNO))
            //                {
            //                    result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey, IMConst.IMProductSKUNO));
            //                }
            //                if (string.IsNullOrEmpty(entryinfo.Supplies_Serial_No))
            //                {
            //                    result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey, IMConst.IMSuppliesSerialNo));
            //                }
            //                if (string.IsNullOrEmpty(entryinfo.ApplyUnit))
            //                {
            //                    result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey, IMConst.IMProductApplyUnit));
            //                }
            //                if (!entryinfo.ApplyQty.HasValue)
            //                {
            //                    result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey, IMConst.IMProductApplyQty));
            //                }
            //                if (!entryinfo.GrossWeight.HasValue)
            //                {
            //                    result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey, IMConst.IMProductGrossWeight));
            //                }
            //                if (!entryinfo.SuttleWeight.HasValue)
            //                {
            //                    result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey, IMConst.IMProductSuttleWeight));
            //                }

            //            }
            //        }
            //    }
            //    else
            //    {
            //        result.AppendLine(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
            //                                                                                      IMConst.IMProductEntryInfo));
            //    }
            //}

            //#endregion

            //取消备案验证功能
            //if (productInfo.ProductBasicInfo.TradeType == TradeType.DirectMail || productInfo.ProductBasicInfo.TradeType == TradeType.FTA)
            //{
            //    ProductEntryInfo entryInfo = ObjectFactory<ProductEntryInfoProcessor>.Instance.LoadProductEntryInfo(productInfo.SysNo);
            //    if (entryInfo.EntryStatus != ProductEntryStatus.EntrySuccess)
            //    {
            //        result.AppendLine(ResouceManager.GetMessageString("IM.Product", "ProductOnShowCheckResult_EntryStatus"));
            //    }
            //}


            //if (_mktBizInteract.ProductIsAttachment(productInfo.SysNo))
            //{
            //    throw new BizException(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey, IMConst.IMProductMRProductIsAttachment));
            //}

            if (productInfo.Merchant.VendorInvoiceType == VendorInvoiceType.GUD && string.IsNullOrWhiteSpace(productInfo.Merchant.ShoppingGuideURL))
            {
                result.AppendLine("导购类型的商品上架必须填写导购URL!");
            }

            if (result.ToString().Length > 0)
            {
                throw new BizException(new StringBuilder().AppendLine(productInfo.ProductID).ToString() + result);
            }
            return true;
        }

        #endregion

        #region 更新商品信息
        [ProductInfoChange]
        public virtual void UpdateProductBasicInfo(ProductInfo productInfo, ref ProductManagerInfo pmInfo)
        {
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);

            #region 排重处理

            //if (_productDA.GetProductCountExcept(productInfo) >= 1)
            //{
            //    throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult1"));
            //}

            #endregion

            #region PM处理

            if (productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue
    && productInfo.ProductBasicInfo.ProductBrandInfo.SysNo.HasValue)
            {
                var pm = ObjectFactory<ProductLineProcessor>.Instance.GetPMByC3(
                             productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value,
                             productInfo.ProductBasicInfo.ProductBrandInfo.SysNo.Value);
                if (pm != null)
                {
                    productInfo.ProductBasicInfo.ProductManager = pm;
                    pmInfo = pm;
                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult2"));
                }
            }

            #endregion

            #region 类别、品牌处理

            var group = _productGroupDA.GetProductGroup(productInfo.SysNo);

            if (product.ProductBasicInfo.ProductBrandInfo.SysNo != productInfo.ProductBasicInfo.ProductBrandInfo.SysNo)
            {
                UpdateGroupProductBrandInfo(group, productInfo);
            }

            if (product.ProductBasicInfo.ProductCategoryInfo.SysNo != productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo)
            {
                if (productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue && product.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
                {
                    if (_categoryPropertyDA.GetCategoryPropertyByCategorySysNo(
                        product.ProductBasicInfo.ProductCategoryInfo.SysNo.Value).Any(sourceCategoryProperty => !_categoryPropertyDA.GetCategoryPropertyByCategorySysNo(
                        productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.Value).Any(p => p.Property.SysNo == sourceCategoryProperty.Property.SysNo)))
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult3"));
                    }
                    UpdateGroupProductCategoryInfo(group, productInfo);
                }
            }

            #endregion

            #region 代销属性处理

            if (product.ProductConsignFlag != productInfo.ProductConsignFlag)
            {
                var productInventoryInfo = ExternalDomainBroker.GetProductTotalInventoryInfo(productInfo.SysNo);

                if (productInventoryInfo != null)
                {
                    if (productInventoryInfo.ShiftQty != 0)
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult4"));
                    }

                    if (productInfo.ProductConsignFlag == VendorConsignFlag.Consign && (productInventoryInfo.AccountQty != 0 || productInventoryInfo.PurchaseQty != 0))
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult5"));
                    }

                    if (productInfo.ProductConsignFlag == VendorConsignFlag.Sell && (productInventoryInfo.ConsignQty != 0 || productInventoryInfo.PurchaseQty != 0))
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult6"));
                    }
                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult7"));
                }
            }

            #endregion

            #region 时效性促销语处理

            var promotionTitle = String.Empty;

            var timelyPromotionTitle =
                    productInfo.ProductTimelyPromotionTitle.FirstOrDefault(p => p.PromotionTitleType == "CountDown");

            var normalPromotionTitle =
                    productInfo.ProductTimelyPromotionTitle.FirstOrDefault(p => p.PromotionTitleType == "Normal");

            if (timelyPromotionTitle != null)
            {
                if (!timelyPromotionTitle.BeginDate.HasValue || !timelyPromotionTitle.EndDate.HasValue)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult8"));
                }

                if (timelyPromotionTitle.BeginDate.Value > timelyPromotionTitle.EndDate)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductBasicInfoResult9"));
                }

                if (DateTime.Now >= timelyPromotionTitle.BeginDate && DateTime.Now <= timelyPromotionTitle.EndDate)
                {
                    promotionTitle = timelyPromotionTitle.PromotionTitle.Content;
                }
            }
            else
            {
                if (normalPromotionTitle != null)
                {
                    promotionTitle = normalPromotionTitle.PromotionTitle.Content;
                }
            }

            #endregion

            if ((product.ProductBasicInfo.ProductTitle.Content != productInfo.ProductBasicInfo.ProductTitle.Content) && String.IsNullOrEmpty(product.ProductBasicInfo.Keywords.Content))
            {
                product.ProductBasicInfo.Keywords = new LanguageContent(CommonUtility.WordSegment(productInfo.ProductName).Join(" "));
            }
            else
            {
                product.ProductBasicInfo.Keywords = productInfo.ProductBasicInfo.Keywords;
            }
            product.ProductBasicInfo.ProductTitle = productInfo.ProductBasicInfo.ProductTitle;
            product.ProductTimelyPromotionTitle = productInfo.ProductTimelyPromotionTitle;
            product.PromotionTitle = new LanguageContent(promotionTitle);
            product.ProductBasicInfo.ProductBriefTitle = productInfo.ProductBasicInfo.ProductBriefTitle;
            product.ProductBasicInfo.ProductBriefAddition = productInfo.ProductBasicInfo.ProductBriefAddition;
            product.ProductBasicInfo.ProductBrandInfo.SysNo = productInfo.ProductBasicInfo.ProductBrandInfo.SysNo;
            product.ProductBasicInfo.ProductCategoryInfo.SysNo = productInfo.ProductBasicInfo.ProductCategoryInfo.SysNo;
            product.ProductBasicInfo.ProductModel = productInfo.ProductBasicInfo.ProductModel;
            product.ProductBasicInfo.ProductType = productInfo.ProductBasicInfo.ProductType;
            product.ProductBasicInfo.ProductManager.SysNo = productInfo.ProductBasicInfo.ProductManager.UserInfo.SysNo;
            product.ProductConsignFlag = productInfo.ProductConsignFlag;
            product.ProductBasicInfo.ShortDescription = productInfo.ProductBasicInfo.ShortDescription;
            product.ProductBasicInfo.PackageList = productInfo.ProductBasicInfo.PackageList;
            product.ProductBasicInfo.ProductLink = productInfo.ProductBasicInfo.ProductLink;
            product.ProductBasicInfo.Attention = productInfo.ProductBasicInfo.Attention;
            product.ProductBasicInfo.IsTakePicture = productInfo.ProductBasicInfo.IsTakePicture;
            product.ProductBasicInfo.Note = productInfo.ProductBasicInfo.Note;
            product.ProductBasicInfo.ProductInfoFinishStatus = productInfo.ProductBasicInfo.ProductInfoFinishStatus;
            product.ProductBasicInfo.ShoppingGuideURL = productInfo.ProductBasicInfo.ShoppingGuideURL;
            product.ProductBasicInfo.TradeType = productInfo.ProductBasicInfo.TradeType;
            product.ProductBasicInfo.StoreType = productInfo.ProductBasicInfo.StoreType;
            product.ProductBasicInfo.SafeQty = productInfo.ProductBasicInfo.SafeQty;

            //add by kelvin 20140520 增加税则信息
            product.ProductBasicInfo.TaxNo = productInfo.ProductBasicInfo.TaxNo;
            product.ProductBasicInfo.TariffPrice = productInfo.ProductBasicInfo.TariffPrice;
            product.ProductBasicInfo.EntryRecord = productInfo.ProductBasicInfo.EntryRecord;

            //Add By Johhny 维护BMCode和UPCCode
            product.ProductBasicInfo.BMCode = productInfo.ProductBasicInfo.BMCode;
            product.ProductBasicInfo.UPCCode = productInfo.ProductBasicInfo.UPCCode;

            product.ProductWarrantyInfo = productInfo.ProductWarrantyInfo;
            product.JDProductID = productInfo.JDProductID;
            product.AMProductID = productInfo.AMProductID;
            product.OperateUser = productInfo.OperateUser;
            product.CompanyCode = productInfo.CompanyCode;
            product.LanguageCode = productInfo.LanguageCode;

            _productCommonInfoDA.UpdateProductCommonInfoBasicInfo(product, productInfo.OperateUser);
            _productCommonInfoDA.UpdateProductCommonInfoNote(product, productInfo.OperateUser);
            _productDA.UpdateProductBasicInfo(product);
            _productDA.UpdateProductInfoFinishStatus(product);
            _productDA.UpdateProductIsNoExtendWarranty(product);
            _productDA.DeleteProductTimelyPromotionTitle(product.SysNo);
            _productDA.InsertProductTimelyPromotionTitle(product.SysNo, product);
        }

        public virtual void BatchUpdateProductBasicInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            var list = _productGroupDA.GetProductSysNoListByGroupProductSysNo(productInfo.SysNo);
            if (list != null && list.Count != 0)
            {
                batchUpdateProductSysNoList.ForEach(productSysNo =>
                {
                    if (list.Any(p => p == productSysNo))
                    {
                        productInfo.SysNo = productSysNo;
                        _productDA.UpdateProductBasicInfoWhenBatchUpdate(productInfo);
                    }
                });
            }
        }

        public virtual void UpdateGroupProductBrandInfo(ProductGroup group, ProductInfo productInfo)
        {
            if (group != null && group.SysNo != 0 && group.ProductList != null && group.ProductList.Count >= 1)
            {
                _productDA.UpdateGroupProductBrandInfo(group, productInfo.ProductBasicInfo.ProductBrandInfo, productInfo.OperateUser);
                group.ProductList.ForEach(p => _productCommonInfoDA.UpdateProductCommonInfoPMInfo(productInfo, productInfo.OperateUser));
            }
        }

        public virtual void UpdateGroupProductCategoryInfo(ProductGroup group, ProductInfo productInfo)
        {
            if (group != null && group.SysNo != 0 && group.ProductList != null && group.ProductList.Count >= 1)
            {
                _productDA.UpdateGroupProductCategoryInfo(group, productInfo.ProductBasicInfo.ProductCategoryInfo, productInfo.OperateUser);
                group.ProductList.ForEach(p => _productCommonInfoDA.UpdateProductCommonInfoPMInfo(productInfo, productInfo.OperateUser));
            }
        }

        [ProductInfoChange]
        public virtual void UpdateProductDescriptionInfo(ProductInfo productInfo)
        {
            ProductDescriptionPreCheck(productInfo);
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            product.ProductBasicInfo.LongDescription = productInfo.ProductBasicInfo.LongDescription;
            product.ProductBasicInfo.PhotoDescription = productInfo.ProductBasicInfo.PhotoDescription;
            _productCommonInfoDA.UpdateProductCommonInfoDescription(product, productInfo.OperateUser);
        }

        public virtual void BatchUpdateProductDescriptionInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            var list = _productGroupDA.GetProductSysNoListByGroupProductSysNo(productInfo.SysNo);
            if (list != null && list.Count != 0)
            {
                batchUpdateProductSysNoList.ForEach(productSysNo =>
                {
                    if (list.Any(p => p == productSysNo))
                    {
                        productInfo.SysNo = productSysNo;
                        UpdateProductDescriptionInfo(productInfo);
                    }
                });
            }
        }

        private void ProductDescriptionPreCheck(ProductInfo productInfo)
        {
            if (new Regex(@"<script[\s\S]+?</script *>", RegexOptions.IgnoreCase).IsMatch(productInfo.ProductBasicInfo.LongDescription.Content))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductDescriptionPreCheckResult"));
            }
        }

        public virtual void UpdateProductAccessoryInfo(ProductInfo productInfo)
        {
            if (productInfo.ProductBasicInfo.ProductAccessories.Any(a => a.Priority > 255))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductAccessoryInfoResult"));
            }
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            _productDA.DeleteProductAccessory(productInfo.SysNo);
            foreach (var productAccessory in productInfo.ProductBasicInfo.ProductAccessories)
            {
                productAccessory.OperationUser = productInfo.OperateUser;
                _productDA.InsertProductAccessory(productInfo.SysNo, productAccessory);
            }
            product.ProductBasicInfo.IsAccessoryShow = productInfo.ProductBasicInfo.IsAccessoryShow;
            _productCommonInfoDA.UpdateProductCommonInfoIsAccessoryShow(product, productInfo.OperateUser);
        }

        public virtual void BatchUpdateProductAccessoryInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            var list = _productGroupDA.GetProductSysNoListByGroupProductSysNo(productInfo.SysNo);
            if (list != null && list.Count != 0)
            {
                batchUpdateProductSysNoList.ForEach(productSysNo =>
                {
                    if (list.Any(p => p == productSysNo))
                    {
                        productInfo.SysNo = productSysNo;
                        UpdateProductAccessoryInfo(productInfo);
                    }
                });
            }
        }

        [ProductInfoChange]
        public virtual void UpdateProductImageInfo(ProductInfo productInfo)
        {
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            if (product.ProductCommonInfoSysNo.HasValue)
            {
                _productCommonInfoDA.DeleteProductCommonInfoResourceImage(product.ProductCommonInfoSysNo.Value);
                foreach (var productImage in productInfo.ProductBasicInfo.ProductResources)
                {
                    _productCommonInfoDA.InsertProductCommonInfoResource(product.ProductCommonInfoSysNo.Value, productImage);
                }
            }
            product.ProductBasicInfo.IsVirtualPic = productInfo.ProductBasicInfo.IsVirtualPic;
            _productCommonInfoDA.UpdateProductCommonInfoIsVirtualPic(product, productInfo.OperateUser);
        }

        [ProductInfoChange]
        public virtual string UpdateProductPropertyInfo(ProductInfo productInfo)
        {
            string resultPerformance = String.Empty;
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            ProductPropertiesPreCheck(productInfo);
            var productGroup = _productGroupDA.GetProductGroup(productInfo.SysNo);
            if (productGroup.SysNo.HasValue && productGroup.ProductList.Count > 1)
            {
                if (productGroup.ProductGroupSettings.Any(setting => !productInfo.ProductBasicInfo.ProductProperties.Any(
                    property =>
                    property.Property.PropertyInfo.SysNo == setting.ProductGroupProperty.SysNo &&
                    property.Property.SysNo != 0)))
                {
                    throw new BizException(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                                           IMConst.IMProductMRProductInfoGroupPropertyEmpty));
                }

                var productGroupExcept =
                    productGroup.ProductList.Where(p => p.ProductCommonInfoSysNo != product.ProductCommonInfoSysNo).ToList();

                var groupPropertySysNoList = productGroup.ProductGroupSettings.Select(setting => setting.ProductGroupProperty.SysNo).ToList();

                var productGroupPropertyValue = productInfo.ProductBasicInfo.ProductProperties.Where(
                    property => groupPropertySysNoList.Contains(property.Property.PropertyInfo.SysNo)).Select(
                        property => property.Property.SysNo).Join("|");

                if (productGroupExcept.Select(p => p.ProductBasicInfo.ProductProperties.Where(
                    property => groupPropertySysNoList.Contains(property.Property.PropertyInfo.SysNo)).Select(
                        property => property.Property.SysNo).Join("|")).Any(d => d == productGroupPropertyValue))
                {
                    throw new BizException(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                                           IMConst.IMProductMRProductInfoGroupPropertyDuplicate));
                }
            }
            if (product.ProductCommonInfoSysNo.HasValue)
            {
                _productCommonInfoDA.DeleteProductCommonInfoProperty(product.ProductCommonInfoSysNo.Value);
                productInfo.ProductBasicInfo.ProductProperties.ForEach(productProperty =>
                {
                    productProperty.CompanyCode = productInfo.CompanyCode;
                    productProperty.LanguageCode = productInfo.LanguageCode;
                    productProperty.OperationUser = productInfo.OperateUser;
                    if (product.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
                    {
                        productProperty.SysNo = _productCommonInfoDA.InsertProductCommonInfoProperty(product.ProductCommonInfoSysNo.Value,
                                                                               product.ProductBasicInfo.
                                                                                   ProductCategoryInfo.SysNo,
                                                                               productProperty);
                    }
                });
                resultPerformance = BuildProductPerformance(productInfo.SysNo, productInfo.ProductBasicInfo.ProductProperties);
                product.ProductBasicInfo.Performance = resultPerformance;
                _productCommonInfoDA.UpdateProductCommonInfoPerformance(product, productInfo.OperateUser);
                #region 更新商品描述
                SetProductDescription(product);
                _productCommonInfoDA.UpdateProductCommonInfoDesc(product, productInfo.OperateUser);
                #endregion
            }

            return resultPerformance;
        }

        public virtual void BatchUpdateProductPropertyInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            ProductPropertiesPreCheck(productInfo);
            var productGroup = _productGroupDA.GetProductGroup(productInfo.SysNo);
            if (productGroup != null && productGroup.ProductList != null && productGroup.ProductList.Count != 0)
            {
                batchUpdateProductSysNoList.ForEach(productSysNo =>
                {
                    if (productGroup.ProductList.Any(p => p.SysNo == productSysNo))
                    {
                        var product = productGroup.ProductList.First(p => p.SysNo == productSysNo);

                        product.ProductBasicInfo.ProductProperties = BuildProductProperties(product, productInfo, productGroup);

                        if (product.ProductCommonInfoSysNo.HasValue)
                        {
                            _productCommonInfoDA.DeleteProductCommonInfoProperty(product.ProductCommonInfoSysNo.Value);
                            product.ProductBasicInfo.ProductProperties
                                .ForEach(productProperty =>
                                {
                                    productProperty.CompanyCode = productInfo.CompanyCode;
                                    productProperty.LanguageCode = productInfo.LanguageCode;
                                    productProperty.OperationUser = productInfo.OperateUser;
                                    productProperty.SysNo = _productCommonInfoDA.InsertProductCommonInfoProperty(product.ProductCommonInfoSysNo.Value, product.ProductBasicInfo.ProductCategoryInfo.SysNo, productProperty);
                                });
                            var resultPerformance = BuildProductPerformance(product.SysNo, product.ProductBasicInfo.ProductProperties);
                            product.ProductBasicInfo.Performance = resultPerformance.ToString();
                            _productCommonInfoDA.UpdateProductCommonInfoPerformance(product, productInfo.OperateUser);
                            #region 更新商品描述
                            SetProductDescription(product);
                            _productCommonInfoDA.UpdateProductCommonInfoDesc(product, productInfo.OperateUser);
                            #endregion

                        }
                    }
                });
            }
        }

        private void ProductPropertiesPreCheck(ProductInfo productInfo)
        {
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            product.ProductBasicInfo.ProductProperties
                .Where(property => property.Required == ProductPropertyRequired.Yes)
                .ForEach(property => productInfo.ProductBasicInfo.ProductProperties
                                         .Where(p => p.Property.PropertyInfo.SysNo == property.Property.PropertyInfo.SysNo)
                                         .ForEach(p =>
                                         {
                                             if (p.Property.SysNo == 0)
                                             {
                                                 throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductPropertiesPreCheckResult"));
                                             }
                                         }));
        }

        private List<ProductProperty> BuildProductProperties(ProductInfo targetPropertiesProduct, ProductInfo sourcePropertiesProduct, ProductGroup productGroup)
        {
            var descProductProperties = new List<ProductProperty>();

            targetPropertiesProduct.ProductBasicInfo.ProductProperties.ForEach(property =>
            {
                if (productGroup.ProductGroupSettings.Any(setting => setting.ProductGroupProperty.SysNo == property.Property.PropertyInfo.SysNo))
                {
                    descProductProperties.Add(property);
                }
            });

            sourcePropertiesProduct.ProductBasicInfo.ProductProperties.ForEach(property =>
            {
                if (productGroup.ProductGroupSettings.All(setting => setting.ProductGroupProperty.SysNo != property.Property.PropertyInfo.SysNo))
                {
                    property.OperationUser = new UserInfo();
                    property.OperationUser = ExternalDomainBroker.GetUserInfo(ServiceContext.Current.UserSysNo);
                    descProductProperties.Add(property);
                }
            });

            return descProductProperties;
        }

        public string BuildProductPerformance(int productSysNo, IList<ProductProperty> productProperties)
        {
            var resultPerformance = new StringBuilder();
            string head = String.Empty;
            if (productProperties.Any(p => !String.IsNullOrEmpty(p.PersonalizedValue.Content) || p.Property.SysNo != 0))
            {
                head = "<LongDescription Name='" + productSysNo + @"'>";
                resultPerformance.Append(head);
            }

            var tempGroupName = String.Empty;
            bool flag = false;

            foreach (var productProperty in productProperties.Where(p => !String.IsNullOrEmpty(p.PersonalizedValue.Content) || p.Property.SysNo != 0))
            {
                if (productProperty.PropertyGroup != null
                    && productProperty.PropertyGroup.SysNo >= 0)
                {
                    productProperty.PropertyGroup =
                        _propertyDA.GetPropertyGroupBySysNo(productProperty.PropertyGroup.SysNo);
                }
                if (productProperty.PropertyGroup != null && productProperty.PropertyGroup.PropertyGroupName != null
                    && (tempGroupName != productProperty.PropertyGroup.PropertyGroupName.Content
                    || String.IsNullOrEmpty(productProperty.PropertyGroup.PropertyGroupName.Content)))
                {
                    if (resultPerformance.ToString() != head && flag)
                    {
                        resultPerformance.Append("</Group>");
                    }
                    resultPerformance.Append("<Group GroupName='" + productProperty.PropertyGroup.PropertyGroupName.Content + "'>");
                    flag = true;
                    tempGroupName = productProperty.PropertyGroup.PropertyGroupName.Content;
                }
                if (productProperty.Property.SysNo > 0)
                {
                    productProperty.Property =
                        _propertyDA.GetPropertyValueByPropertyValueSysNo(productProperty.Property.SysNo.Value);
                }
                if (productProperty.Property.PropertyInfo.SysNo > 0)
                {
                    productProperty.Property.PropertyInfo =
                        _propertyDA.GetPropertyBySysNo(productProperty.Property.PropertyInfo.SysNo.Value);
                }

                if (!String.IsNullOrEmpty(productProperty.PersonalizedValue.Content)
                    && productProperty.Property.PropertyInfo.PropertyName != null
                    && productProperty.Property.ValueDescription != null)
                {
                    resultPerformance.Append("<Property Key='" +
                                             productProperty.Property.PropertyInfo.PropertyName.Content +
                                             "' Value='" + (string.IsNullOrWhiteSpace(productProperty.PersonalizedValue.Content) ? "" : productProperty.PersonalizedValue.Content.Replace("'", "‘")) + "'/>");
                }
                else if (productProperty.Property.PropertyInfo.PropertyName != null
                          && productProperty.Property.ValueDescription != null)
                {
                    resultPerformance.Append("<Property Key='" +
                                             productProperty.Property.PropertyInfo.PropertyName.Content +
                                             "' Value='" + (string.IsNullOrWhiteSpace(productProperty.Property.ValueDescription.Content) ? "" : productProperty.Property.ValueDescription.Content.Replace("'", "‘")) + "'/>");
                }
            }
            if (flag)
            {
                resultPerformance.Append("</Group>");
            }
            resultPerformance.Append("</LongDescription>");
            return resultPerformance.ToString();
        }

        public virtual void UpdateProductAutoPriceInfo(ProductInfo productInfo)
        {
            if (productInfo.AutoAdjustPrice.IsAutoAdjustPrice == IsAutoAdjustPrice.No)
            {
                if (!productInfo.AutoAdjustPrice.NotAutoPricingBeginDate.HasValue
                    || !productInfo.AutoAdjustPrice.NotAutoPricingEndDate.HasValue)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductAutoPriceInfoResult1"));
                }
                if (productInfo.AutoAdjustPrice.NotAutoPricingEndDate.Value
                    < productInfo.AutoAdjustPrice.NotAutoPricingBeginDate.Value)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductAutoPriceInfoResult2"));
                }
            }
            _productDA.UpdateProductAutoPriceInfo(productInfo);
        }

        [ProductInfoChange]
        public virtual void UpdateProductWarrantyInfo(ProductInfo productInfo)
        {
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            product.ProductWarrantyInfo = productInfo.ProductWarrantyInfo;
            product.ProductBasicInfo.Note = productInfo.ProductBasicInfo.Note;
            _productCommonInfoDA.UpdateProductCommonInfoNote(product, productInfo.OperateUser);
            _productCommonInfoDA.UpdateProductCommonInfoWarrantyInfo(product, productInfo.OperateUser);
        }

        public virtual void BatchUpdateProductWarrantyInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            var list = _productGroupDA.GetProductSysNoListByGroupProductSysNo(productInfo.SysNo);
            if (list != null && list.Count != 0)
            {
                batchUpdateProductSysNoList.ForEach(productSysNo =>
                {
                    if (list.Any(p => p == productSysNo))
                    {
                        productInfo.SysNo = productSysNo;
                        UpdateProductWarrantyInfo(productInfo);
                    }
                });
            }
        }

        [ProductInfoChange]
        public virtual void UpdateProductDimensionInfo(ProductInfo productInfo)
        {
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            product.ProductBasicInfo.ProductDimensionInfo = productInfo.ProductBasicInfo.ProductDimensionInfo;
            _productCommonInfoDA.UpdateProductCommonInfoDimensionInfo(product, productInfo.OperateUser);
        }

        public virtual void BatchUpdateProductDimensionInfo(ProductInfo productInfo, List<int> batchUpdateProductSysNoList)
        {
            var list = _productGroupDA.GetProductSysNoListByGroupProductSysNo(productInfo.SysNo);
            if (list != null && list.Count != 0)
            {
                batchUpdateProductSysNoList.ForEach(productSysNo =>
                {
                    if (list.Any(p => p == productSysNo))
                    {
                        productInfo.SysNo = productSysNo;
                        UpdateProductDimensionInfo(productInfo);
                    }
                });
            }
        }

        public virtual void UpdateProductSalesAreaInfo(ProductInfo productInfo)
        {

            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            _productDA.DeleteProductSalesArea(productInfo.SysNo);
            foreach (var productSalesInfo in productInfo.ProductSalesAreaInfoList)
            {
                productSalesInfo.OperationUser = productInfo.OperateUser;
                _productDA.InsertProductSalesArea(product, productSalesInfo);
            }
        }

        public virtual void BatchUpdateProductSalesAreaInfo(List<ProductInfo> productInfoList)
        {
            /*商品的销售区域设置在前端有两个地方1.商品查询2.批量设置销售区域.
           一：商品查询设置是先将该商品的销售区域load出来，设置之后，保存的步骤为：
           1.删除该商品的所有销售区域，2.批量插入该商品的销售区域（包括Load出来的--判断重复再前端）
           二：批量设置销售区域是不将商品的销售区域load出来，由于共用该方法，
           删除之后，只能将新设置的销售区域插入，原来设置的销售区域被删除。
            解决办法：
            step1.根据商品的SysNo得到该商品的所有销售区域。
            step2.将设置的销售区域和原有的进行合并（取并集）
            step3.删除该商品原有的销售区域
            step4.将合并后的销售区域insert
            */
            var errorlist = new List<string>();
            foreach (var productInfo in productInfoList)
            {
                var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
                if (product != null)
                {
                    //取并集
                    var newList = new List<ProductSalesAreaInfo>();
                    var source = (productInfo.ProductSalesAreaInfoList
                        .Where(e => !((product.ProductSalesAreaInfoList
                                         .Where(
                                             k =>
                                             k.Stock.SysNo == e.Stock.SysNo &&
                                             k.Province.ProvinceSysNo == e.Province.ProvinceSysNo)).Any()))).ToList();
                    newList.AddRange(product.ProductSalesAreaInfoList);
                    newList.AddRange(source);

                    productInfo.ProductSalesAreaInfoList = newList;
                    _productDA.DeleteProductSalesArea(productInfo.SysNo);

                    productInfo.ProductSalesAreaInfoList.ForEach(productSalesInfo =>
                    {
                        productSalesInfo.OperationUser = productInfo.OperateUser;
                        _productDA.InsertProductSalesArea(product, productSalesInfo);
                    });
                }
                else
                {
                    errorlist.Add(productInfo.ProductID);
                }
            }
            if (errorlist.Any())
            {
                var errorStr = new StringBuilder();
                errorlist.ForEach(item => errorStr.AppendLine(item));
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "BatchUpdateProductSalesAreaInfoResult"), productInfoList.Count(), (productInfoList.Count - errorlist.Count), errorlist.Count, errorStr));
            }


        }

        public virtual void UpdateProductPurchaseInfo(ProductInfo productInfo)
        {
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            product.ProductPOInfo = productInfo.ProductPOInfo;
            product.OperateUser = productInfo.OperateUser;
            _productDA.UpdateProductPurchaseInfo(product);
        }

        //public virtual void UpdateProductThirdPartyInventory(ProductInfo productInfo)
        //{
        //    var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
        //    product.ProductMappingList = productInfo.ProductMappingList;
        //    product.OperateUser = productInfo.OperateUser;
        //    product.CompanyCode = productInfo.CompanyCode;
        //    product.LanguageCode = productInfo.LanguageCode;
        //    if (product.ProductMappingList.Any())
        //    {
        //        _productDA.UpdateProductThirdPartyInventory(product);
        //    }
        //}

        public virtual void UpdateProductPromotionType(ProductInfo productInfo)
        {
            _productDA.UpdateProductPromotionType(productInfo);
        }

        #endregion

        #region 商品属性复制
        [ProductInfoChange]
        public virtual void ProductCopyProperty(int? sourceProductSysNo, int? targetProductSysNo, bool canOverrite, string companyCode, string languageCode, UserInfo operationUser)
        {
            if (!sourceProductSysNo.HasValue || !targetProductSysNo.HasValue)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductIsNull"));
            }

            if (sourceProductSysNo.Value == targetProductSysNo.Value)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductCopyPropertyResult1"));
            }

            ProductInfo sourceProduct = _productDA.GetProductInfoBySysNo(sourceProductSysNo.Value);

            ProductInfo targetProduct = _productDA.GetProductInfoBySysNo(targetProductSysNo.Value);

            if (sourceProduct.ProductBasicInfo.ProductCategoryInfo.SysNo != targetProduct.ProductBasicInfo.ProductCategoryInfo.SysNo)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductCopyPropertyResult2"));
            }

            var targetProductGroup = _productGroupDA.GetProductGroup(targetProductSysNo.Value);

            targetProduct.ProductBasicInfo.ProductProperties.ForEach(property =>
            {
                if (targetProductGroup.ProductGroupSettings.All(setting => setting.ProductGroupProperty.SysNo != property.Property.PropertyInfo.SysNo))
                {
                    if ((property.Property.SysNo.HasValue || !String.IsNullOrEmpty(property.PersonalizedValue.Content)) && !canOverrite)
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductCopyPropertyResult3"));
                    }
                }
            });

            var productGroup = _productGroupDA.GetProductGroup(sourceProductSysNo.Value);

            targetProduct.ProductBasicInfo.ProductProperties = BuildProductProperties(targetProduct, sourceProduct,
                                                                                      productGroup);

            if (targetProduct.ProductCommonInfoSysNo.HasValue)
            {
                _productCommonInfoDA.DeleteProductCommonInfoProperty(targetProduct.ProductCommonInfoSysNo.Value);
                targetProduct.ProductBasicInfo.ProductProperties
                    .Where(productProperty => productProperty.Property.SysNo.HasValue || !String.IsNullOrEmpty(productProperty.PersonalizedValue.Content))
                    .ForEach(productProperty =>
                    {
                        productProperty.CompanyCode = companyCode;
                        productProperty.LanguageCode = languageCode;
                        productProperty.OperationUser = operationUser;
                        productProperty.SysNo = _productCommonInfoDA.InsertProductCommonInfoProperty(targetProduct.ProductCommonInfoSysNo.Value, targetProduct.ProductBasicInfo.ProductCategoryInfo.SysNo, productProperty);
                    });
                var resultPerformance = BuildProductPerformance(targetProduct.SysNo, targetProduct.ProductBasicInfo.ProductProperties);
                targetProduct.ProductBasicInfo.Performance = resultPerformance;
                _productCommonInfoDA.UpdateProductCommonInfoPerformance(targetProduct, operationUser);
                #region 更新商品描述
                SetProductDescription(targetProduct);
                _productCommonInfoDA.UpdateProductCommonInfoDesc(targetProduct, operationUser);
                #endregion
            }
        }

        #endregion

        #region 商品查询合并分仓库存信息
        /// <summary>
        /// 获取包含分仓库存的数据
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual void GetInventoryInfoByStock(DataTable product)
        {
            if (product == null || product.Rows.Count == 0) return;
            #region 仓库编号硬编码，应该直接获取仓库列表
            //var warehouseList = ExternalDomainBroker.GetWarehouseList("8601");
            //var stockList = new Dictionary<string, int>();
            //warehouseList.ForEach(v =>
            //{
            //    if (v.SysNo.HasValue)
            //    {
            //        stockList.Add(v.WarehouseID, v.SysNo.Value);
            //    }
            //});
            var stockList = new Dictionary<string, int>
            { 
                                                            { "Shanghai", 51 }, 
                                                            { "Begjin", 52 }, 
                                                            { "Guangzhou", 53 } 
                                                           };
            #endregion
            var displayField = new[] { "AccountQty", "AvailableQty",
                                                  "OrderQty" ,"VirtualQty",
                                                  "PurchaseQty","OnlineQty",
                                                   "W1","M1"};

            var columns = (from k in stockList.Keys
                           from c in displayField
                           select k + c).ToList();
            AddTableColumns(product, columns);

            var productSysNoList = (from e in product.AsEnumerable()
                                    select e.Field<int>("SysNo")).ToList();
            var prarm = (from k in productSysNoList
                         from c in stockList.Keys
                         select new { ProductSysNo = k, Stock = c }).ToList();
            prarm.ForEach(v =>
            {
                var entity = ExternalDomainBroker.GetProductInventoryInfoByStock(v.ProductSysNo, stockList[v.Stock]);
                var otherentity = ExternalDomainBroker.GetProductSalesTrendInfoByStock(v.ProductSysNo, stockList[v.Stock]);
                var oneProduct = (from e in product.AsEnumerable()
                                  where e.Field<int>("SysNo") == v.ProductSysNo
                                  select e).FirstOrDefault();

                displayField.ForEach(k =>
                {
                    int value = 0;
                    if (entity != null)
                    {
                        object tempvalue = null;
                        var pro = entity.GetType().GetProperty(k);
                        if (pro != null)
                        {
                            tempvalue = Invoker.PropertyGet(entity, k);
                        }
                        else if (otherentity != null)
                        {
                            var otherPro = otherentity.GetType().GetProperty(k);
                            if (otherPro != null)
                            {
                                tempvalue = Invoker.PropertyGet(otherentity, k);
                            }
                        }
                        value = Convert.ToInt32(tempvalue);
                    }
                    var columnName = v.Stock + k;
                    if (oneProduct != null)
                        oneProduct.SetField(columnName, value);
                });

            });

        }

        /// <summary>
        /// 合并仓库数据
        /// </summary>
        /// <param name="product"></param>
        /// <param name="columns"></param>
        private void AddTableColumns(DataTable product, List<string> columns)
        {
            if (columns == null || columns.Count == 0) return;
            foreach (var item in columns)
            {
                if (!product.Columns.Contains(item)) product.Columns.Add(item).DefaultValue = 0;
            }
        }

        /// <summary>
        /// 清理重复数据
        /// </summary>
        /// <param name="product"></param>
        /// <param name="totalCount"></param>
        public void ClearProduct(DataTable product, ref int totalCount)
        {
            if (product == null || product.Rows.Count == 0) return;
            var productGroup = (from e in product.AsEnumerable()
                                group e by e.Field<int>("SysNo")
                                    into g
                                    select new { ProductSysNo = g.Key, Count = g.Count() }
                                        into c
                                        where c.Count > 1
                                        select c.ProductSysNo).ToList();
            if (productGroup.Count > 0)
            {
                var rowCount = 0;
                productGroup.ForEach(v =>
                {
                    var products = (from e in product.AsEnumerable()
                                    where e.Field<int>("SysNo") == v
                                    select e).ToList();
                    for (var i = 1; i < products.Count; i++)
                    {
                        products[i].Delete();
                        rowCount++;
                    }
                    product.AcceptChanges();
                });
                totalCount = totalCount - rowCount;

            }
        }
        #endregion

        #region 商品查询添加其他信息
        public virtual void AddOtherData(DataTable product)
        {
            if (product == null || product.Rows.Count == 0) return;
            AddotherColumns(product);
            var rows = (from e in product.AsEnumerable() select e).ToList();
            MarginUtility.PointDivisor = ExternalDomainBroker.GetPointToMoneyRatio();
            if (rows.Count == 0) return;
            rows.ForEach(v =>
            {
                decimal value = ObjectFactory<ProductPriceProcessor>.Instance.GetMargin(v.Field<decimal>("CurrentPrice"), v.Field<int?>("Point") ?? 0, v.Field<decimal?>("UnitCost") ??
                                                            0);
                v.SetField("MarginString", value.ToString("P"));
                var txtvalue = v.Field<string>("POMemo") ?? "";
                v.SetField("POMemoShort", txtvalue.Length > 10 ? (Convert.ToString(txtvalue).Substring(0, 10) + ".....") : txtvalue);
                txtvalue = v.Field<string>("SupplyMemo") ?? "";
                v.SetField("SupplyMemoDisplay", txtvalue.Length > 10 ? Convert.ToString(txtvalue).Substring(0, 10) + "....." : txtvalue);
                v.SetField("BrandNameDisplay", String.IsNullOrWhiteSpace(v.Field<string>("BrandNameCH")) ?
                                           v.Field<string>("BrandNameCH") : v.Field<string>("BrandNameEN"));
                var vendorType = v.Field<int>("VendorType");
                string mallName = AppSettingManager.GetSetting("IM", "IM_MallName");
                v.SetField("MerchantNameDisplay", vendorType == 0 ? mallName : v.Field<string>("MerchantName"));
            });
        }

        private void AddotherColumns(DataTable product)
        {
            product.Columns.Add("MarginString").DefaultValue = "0.00";
            product.Columns.Add("POMemoShort").DefaultValue = "";
            product.Columns.Add("SupplyMemoDisplay").DefaultValue = "";
            product.Columns.Add("BrandNameDisplay").DefaultValue = "";
            product.Columns.Add("MerchantNameDisplay").DefaultValue = "";
        }
        #endregion

        /// <summary>
        /// 设置商品的描述
        /// </summary>
        /// <param name="product"></param>
        private void SetProductDescription(ProductInfo product)
        {
            List<CategoryTemplateInfo> CategoryTemplateList = ObjectFactory<ICategoryTemplateQueryDA>.Instance.GetCategoryTemplateListByC3SysNo(product.ProductBasicInfo.ProductCategoryInfo.SysNo);
            if (CategoryTemplateList == null || CategoryTemplateList.Count == 0)
            {
                return;
            }
            List<CategoryTemplatePropertyInfo> CategoryTemplatePropertyList = ObjectFactory<ICategoryTemplateQueryDA>.Instance.GetCategoryPropertyListByC3SysNo(product.ProductBasicInfo.ProductCategoryInfo.SysNo);
            string[] arr = (from p in CategoryTemplateList where p.TemplateType == CategoryTemplateType.TemplateProductDescription select p).First().Templates.Split(',');
            StringBuilder result = new StringBuilder();
            List<int?> tempdata = new List<int?>();
            foreach (var s in arr)
            {
                foreach (var productProperty in product.ProductBasicInfo.ProductProperties)
                {
                    if (productProperty.Property.PropertyInfo.SysNo.ToString() == s && !tempdata.Contains(productProperty.Property.PropertyInfo.SysNo))
                    {
                        tempdata.Add(productProperty.Property.PropertyInfo.SysNo);
                        if (!String.IsNullOrEmpty(productProperty.PersonalizedValue.Content)
                    && productProperty.Property.PropertyInfo.PropertyName != null
                    && productProperty.Property.ValueDescription != null)
                        {
                            result.Append("<li>" +
                                                     productProperty.Property.PropertyInfo.PropertyName.Content +
                                                     " " + productProperty.PersonalizedValue.Content + "</li>");
                        }
                        else if (productProperty.Property.PropertyInfo.PropertyName != null
                                  && productProperty.Property.ValueDescription != null)
                        {
                            result.Append("<li>" +
                                                     productProperty.Property.PropertyInfo.PropertyName.Content +
                                                     " " + productProperty.Property.ValueDescription.Content + "</li>");
                        }
                    }

                }
            }
            product.ProductBasicInfo.ShortDescription.Content = result.ToString();
        }

        public List<KeyValuePair<int, decimal>> GetProductTax(List<int> pList)
        {
            List<KeyValuePair<int, decimal>> list = new List<KeyValuePair<int, decimal>>();
            DataTable products = null;
            if (pList != null && pList.Count > 0)
            {
                products = _productDA.GetProductTariffInfoProductSysNos(pList);
            }
            else
            {
                return list;
            }
            foreach (DataRow product in products.Rows)
            {
                decimal tax = 0.0M;
                decimal? CurrentPrice = null;
                if (product["CurrentPrice"] != null)
                {
                    decimal tempCurrentPrice;
                    if (decimal.TryParse(product["CurrentPrice"].ToString(), out tempCurrentPrice))
                    {
                        CurrentPrice = tempCurrentPrice;
                    }
                }
                if ((!CurrentPrice.HasValue))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductPriceInfoNotExists"));
                }

                decimal? TariffRate = null;
                if (product["TariffRate"] != null)
                {
                    decimal tempTariffRate;
                    if (decimal.TryParse(product["TariffRate"].ToString(), out tempTariffRate))
                    {
                        TariffRate = tempTariffRate;
                    }
                }
                if ((!TariffRate.HasValue))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductTariffInfoNotExists"));
                }

                tax = CurrentPrice.Value * TariffRate.Value;

                int productSysNo = 0;
                if (product["SysNo"] != null)
                {
                    int.TryParse(product["SysNo"].ToString(), out productSysNo);
                }

                KeyValuePair<int, decimal> item = new KeyValuePair<int, decimal>(productSysNo, tax);
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 获取商品产地列表
        /// </summary>
        /// <returns></returns>
        public List<ProductCountry> GetProductCountryList()
        {
            return _productDA.GetProductCountryList();
        }

        /// <summary>
        /// 创建商品多语言信息
        /// </summary>
        /// <param name="productGroup"></param>
        /// <returns></returns>
        public void ProductLangCreate(ProductGroup productGroup)
        {
            #region 商品组
            var productGroupLang = new MultiLanguageBizEntity()
            {
                SysNo = productGroup.SysNo.Value,
                BizEntityType = "ProductGroup",
                MappingTable = "IPPLang.dbo.Lang_ProductGroup",
                LanguageCode = productGroup.LanguageCode
            };

            productGroupLang.PropertyItemList = new List<PropertyItem>();
            productGroupLang.PropertyItemList.Add(new PropertyItem()
            {
                Field = "ProductGroupName",
                Value = productGroup.ProductGroupName.Content
            });
            ExternalDomainBroker.SetMultiLanguageBizEntity(productGroupLang);
            #endregion

            productGroup.ProductList.ForEach(product =>
            {
                #region 商品
                var productLang = new MultiLanguageBizEntity()
                {
                    SysNo = product.SysNo,
                    BizEntityType = "Product",
                    MappingTable = "IPPLang.dbo.Lang_Product",
                    LanguageCode = productGroup.LanguageCode
                };

                productLang.PropertyItemList = new List<PropertyItem>();
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "ProductName",
                    Value = product.ProductName
                });
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "ProductTitle",
                    Value = product.ProductBasicInfo.ProductTitle.ToString()
                });
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "BriefName",
                    Value = product.ProductBasicInfo.ProductBriefName
                });
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "Keywords",
                    Value = product.ProductBasicInfo.Keywords.ToString()
                });
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "ProductMode",
                    Value = product.ProductBasicInfo.ProductModel.ToString()
                });
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "PackageList",
                    Value = product.ProductBasicInfo.PackageList.ToString()
                });
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "Note",
                    Value = product.ProductBasicInfo.Note
                });
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "Attention",
                    Value = product.ProductBasicInfo.Attention.ToString()
                });
                productLang.PropertyItemList.Add(new PropertyItem()
                {
                    Field = "Warranty",
                    Value = product.ProductWarrantyInfo.Warranty.ToString()
                });

                ExternalDomainBroker.SetMultiLanguageBizEntity(productLang);
                #endregion

                #region 商品属性
                product.ProductBasicInfo.ProductProperties.ForEach(productProperty =>
                {
                    var productPropertyLang = new MultiLanguageBizEntity()
                    {
                        SysNo = productProperty.SysNo.Value,
                        BizEntityType = "ProductHasProperty",
                        MappingTable = "IPPLang.dbo.Lang_ProductHasProperty",
                        LanguageCode = productGroup.LanguageCode
                    };
                    productPropertyLang.PropertyItemList = new List<PropertyItem>();
                    productPropertyLang.PropertyItemList.Add(new PropertyItem()
                    {
                        Field = "InputValue",
                        Value = productProperty.PersonalizedValue.Content
                    });
                    ExternalDomainBroker.SetMultiLanguageBizEntity(productPropertyLang);
                });
                #endregion
            });
        }

        /// <summary>
        /// 更新商品采购价格
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="virtualPrice">采购价格</param>
        public void UpdateProductVirtualPrice(string productSysNo, string virtualPrice)
        {
            _productDA.UpdateProductVirtualPrice(productSysNo, virtualPrice);
        }
    }
}
