using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductGroupProcessor))]
    public class ProductGroupProcessor
    {
        private readonly IProductGroupDA _productGroupDA = ObjectFactory<IProductGroupDA>.Instance;

        private readonly IProductCommonInfoDA _productCommonInfoDA = ObjectFactory<IProductCommonInfoDA>.Instance;

        private readonly IProductResourceDA _productResourceDA = ObjectFactory<IProductResourceDA>.Instance;

        public ProductGroup GetProductGroup(int productSysNo)
        {
            return _productGroupDA.GetProductGroup(productSysNo);
        }

        /// <summary>
        /// 获取商品组信息
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        /// <returns></returns>
        public virtual ProductGroup GetProductGroupInfoBySysNo(int productGroupSysNo)
        {
            ProductGroup entity = _productGroupDA.GetProductGroupInfoBySysNo(productGroupSysNo);
            return entity;
        }

        /// <summary>
        /// 创建商品组
        /// </summary>
        /// <param name="productGroup"></param>
        /// <returns></returns>
        public virtual ProductGroup CreateProductGroupInfo(ProductGroup productGroup)
        {
            CheckGroupProductInfo(productGroup);

            _productGroupDA.CreateProductGroupInfo(productGroup);

            if (productGroup.SysNo.HasValue)
            {
                productGroup.ProductList.ForEach(
                    product => _productCommonInfoDA.UpdateProductCommonInfoGroupSysNo(productGroup.SysNo.Value, product, product.OperateUser ?? productGroup.OperateUser));

                _productGroupDA.CreateGroupPropertySetting(productGroup);
            }

            return productGroup;
        }

        /// <summary>
        /// 更新商品组
        /// </summary>
        /// <param name="productGroup"></param>
        /// <returns></returns>
        public virtual ProductGroup UpdateProductGroupInfo(ProductGroup productGroup)
        {
            CheckGroupProductInfo(productGroup);

            _productGroupDA.UpdateProductGroupInfo(productGroup);

            if (productGroup.SysNo.HasValue)
            {
                ProductGroup allProduct = _productGroupDA.GetProductGroupInfoBySysNo(productGroup.SysNo.Value);
                allProduct.ProductList.ForEach(p =>
                {
                    if (productGroup.ProductList.All(product => product.ProductCommonInfoSysNo != p.ProductCommonInfoSysNo))
                    {
                        var newProductGroup = new ProductGroup
                        {
                            ProductGroupName = p.ProductBasicInfo.ProductTitle,
                            ProductGroupModel = p.ProductBasicInfo.ProductModel,
                            ProductGroupSettings = new List<ProductGroupSettings>(),
                            ProductList = new List<ProductInfo> { p },
                            OperateUser = productGroup.OperateUser,
                            CompanyCode = productGroup.CompanyCode,
                            LanguageCode = productGroup.LanguageCode
                        };
                        _productGroupDA.CreateProductGroupInfo(newProductGroup);
                        _productGroupDA.CreateGroupPropertySetting(newProductGroup);
                        if (newProductGroup.SysNo.HasValue)
                        {
                            _productCommonInfoDA.UpdateProductCommonInfoGroupSysNo(newProductGroup.SysNo.Value, p,
                                                                                   productGroup.OperateUser);
                        }
                    }
                });

                //_productGroupDA.RemoveProductCommonInfoGroupSysNo(productGroup.SysNo.Value);

                productGroup.ProductList.ForEach(product =>
                {
                    _productCommonInfoDA.UpdateProductCommonInfoGroupSysNo(productGroup.SysNo.Value, product, productGroup.OperateUser);
                    if (product.ProductCommonInfoSysNo.HasValue)
                    {
                        var productResources =
                            _productResourceDA.GetNeweggProductResourceListByProductCommonInfoSysNo(
                                product.ProductCommonInfoSysNo.Value);
                        productResources.ForEach(r =>
                        {
                            if (r.Resource.ResourceSysNo.HasValue)
                            {
                                _productResourceDA.UpdateResource(r.Resource.ResourceSysNo.Value, productGroup.SysNo.Value, productGroup.OperateUser);
                            }
                        });
                    }
                });

                _productGroupDA.DeleteGroupPropertySetting(productGroup.SysNo.Value);
                _productGroupDA.CreateGroupPropertySetting(productGroup);
            }
            return productGroup;
        }

        /// <summary>
        /// 检查是否符合创建或者修改组的条件
        /// </summary>
        /// <param name="productGroup"></param>
        private void CheckGroupProductInfo(ProductGroup productGroup)
        {
            if (productGroup.ProductList == null || productGroup.ProductList.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductGroupNOTExistProduct"));
            }

            if (productGroup.ProductGroupSettings == null || productGroup.ProductGroupSettings.Count == 0 || productGroup.ProductGroupSettings.Count > 2)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductGroupCheckGroupProductInfoResult1"));
            }

            if (productGroup.ProductGroupSettings.GroupBy(setting => setting.ProductGroupProperty.SysNo).Count() != productGroup.ProductGroupSettings.Count)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductGroupCheckGroupProductInfoResult2"));
            }

            if (productGroup.ProductGroupSettings.Any(productGroupSetting => productGroupSetting.ProductGroupProperty.SysNo != 0 && String.IsNullOrEmpty(productGroupSetting.PropertyBriefName.Content)))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductGroupCheckGroupProductInfoResult3"));
            }

            if (!productGroup.SysNo.HasValue)
            {
                productGroup.SysNo = 0;
            }

            //if (_productGroupDA.GetProductGroupInfoByGroupName(productGroup.SysNo.Value, productGroup.ProductGroupName.Content) != null)
            //{
            //    throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductGroupNameISExist"));
            //}

            var productList = new List<ProductInfo>();

            productGroup.ProductList.ForEach(productInfo => productList.Add(
                ObjectFactory<ProductProcessor>.Instance.GetProductInfo(
                    productInfo.SysNo)));

            productGroup.ProductList = productList;

            if (productGroup.ProductList.GroupBy(product => product.ProductBasicInfo.ProductCategoryInfo.SysNo).Count() != 1)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductGroupCheckGroupProductInfoResult4"));
            }

            if (productGroup.ProductList.GroupBy(product => product.ProductBasicInfo.ProductBrandInfo.SysNo).Count() != 1)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductGroupCheckGroupProductInfoResult5"));
            }

            productGroup.ProductList.ForEach(product =>
            {
                var productGroupExcept = productGroup.ProductList.Where(p => p.ProductCommonInfoSysNo != product.ProductCommonInfoSysNo).ToList();

                var groupPropertySysNoList = productGroup.ProductGroupSettings.Select(setting => setting.ProductGroupProperty.SysNo).ToList();

                if (product.ProductBasicInfo.ProductProperties
                    .Where(property => property.Property.PropertyInfo.SysNo.HasValue
                        && groupPropertySysNoList.Contains(property.Property.PropertyInfo.SysNo.Value)).Any(property => !property.Property.SysNo.HasValue))
                {
                    throw new BizException(String.Format(ResouceManager.GetMessageString("IM.Product", "ProductGroupCheckGroupProductInfoResult6"), product.ProductID));
                }

                var productGroupPropertyValue = product.ProductBasicInfo.ProductProperties.Where(
                property => groupPropertySysNoList.Contains(property.Property.PropertyInfo.SysNo)).Select(
                    property => property.Property.SysNo).Join("|");

                if (productGroupExcept.Select(p => p.ProductBasicInfo.ProductProperties.Where(
                property => groupPropertySysNoList.Contains(property.Property.PropertyInfo.SysNo)).Select(
                    property => property.Property.SysNo).Join("|")).Any(d => d == productGroupPropertyValue))
                {
                    throw new BizException(ResouceManager.GetMessageString(IMConst.IMProductMessageResourcesKey,
                                                                        IMConst.IMProductMRProductInfoGroupPropertyDuplicate));
                }
            });
        }


        /// <summary>
        /// 获取商品所在商品组的其它商品ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public virtual List<string> GetProductGroupIDSFromProductID(string productID)
        {
            return _productGroupDA.GetProductGroupIDSFromProductID(productID);
        }
    }
}
