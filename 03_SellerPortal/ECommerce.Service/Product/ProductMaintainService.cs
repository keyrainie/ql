using System;
using System.IO;
using System.Linq;
using System.Text;
using ECommerce.Utility;
using ECommerce.WebFramework;
using System.Xml.Serialization;
using System.Collections.Generic;
using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.DataAccess.Product;
using ECommerce.Enums;
using ECommerce.DataAccess.Promotion;

namespace ECommerce.Service.Product
{
    /// <summary>
    /// 商品维护
    /// </summary>
    public class ProductMaintainService
    {
        #region 选择分类
        //查询该商家的商品分类
        public static List<CategoryInfo> GetCategory1List(int sellerSysNo)
        {
            return ProductMaintainDA.GetCategory1List(sellerSysNo);
        }
        public static List<CategoryInfo> GetCategory2List(int sellerSysNo, int C1SysNo)
        {
            return ProductMaintainDA.GetCategory2List(sellerSysNo, C1SysNo);
        }
        public static List<CategoryInfo> GetCategory3List(int sellerSysNo, int C2SysNo)
        {
            return ProductMaintainDA.GetCategory3List(sellerSysNo, C2SysNo);
        }
        //查询所有商品分类
        public static List<CategoryInfo> GetAllCategory1List()
        {
            return ProductMaintainDA.GetAllCategory1List();
        }
        public static List<CategoryInfo> GetAllCategory2List(int C1SysNo)
        {
            return ProductMaintainDA.GetAllCategory2List(C1SysNo);
        }
        public static List<CategoryInfo> GetAllCategory3List(int C2SysNo)
        {
            return ProductMaintainDA.GetAllCategory3List(C2SysNo);
        }

        #endregion

        #region 基础信息维护

        /// <summary>
        /// 获取商家前台根级类别
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <returns></returns>
        public static List<FrontProductCategoryInfo> GetParentFrontProductCategory(int sellerSysNo)
        {
            return ProductMaintainDA.GetParentFrontProductCategory(sellerSysNo);
        }

        /// <summary>
        /// 获取商家前台根级类别
        /// </summary>
        /// <param name="parentCategoryCode">父类别编码</param>
        /// <returns></returns>
        public static List<FrontProductCategoryInfo> GetFrontProductCategoryByParentCode(string parentCategoryCode)
        {
            return ProductMaintainDA.GetFrontProductCategoryByParentCode(parentCategoryCode);
        }
        
        /// <summary>
        /// 获取指定C3的分组属性
        /// </summary>
        /// <param name="categorySysNo">C3</param>
        /// <returns></returns>
        public static List<CategoryPropertyInfo> GetSplitPropertyByCategorySysNo(int categorySysNo)
        {
            return ProductMaintainDA.GetSplitPropertyByCategorySysNo(categorySysNo);
        }

        /// <summary>
        /// 获取指定类别编码的商家前台类别
        /// </summary>
        /// <param name="categoryCode">类别编码</param>
        /// <returns></returns>
        public static FrontProductCategoryInfo GetFrontProductCategoryByCode(string categoryCode)
        {
            return ProductMaintainDA.GetFrontProductCategoryByCode(categoryCode);
        }
        
        /// <summary>
        /// 获取创建商品基础信息模板
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="categorySysNo">C3</param>
        public static ProductMaintainBasicInfo GetCreateProductBasicInfoTemplate(int sellerSysNo, int categorySysNo)
        {
            ProductMaintainBasicInfo template = new ProductMaintainBasicInfo();
            template.SellerSysNo = sellerSysNo;
            template.NormalProperties = ProductMaintainDA.GetNormalPropertyByCategorySysNo(categorySysNo);
            template.NormalPropertyValues = ProductMaintainDA.GetNormalPropertyValueByCategorySysNo(categorySysNo);
            template.CountryCodeList = ProductMaintainDA.GetProductCountryList();
            template.CategoryText = GetCategoryTextByCategorySysNo(categorySysNo);
            //添加默认产地和运营地。 2017.11.21 jk001
            template.OriginCode = "CN";
            template.ShoppingCountryCode = "CN";
            return template;
        }
        /// <summary>
        /// 获取编辑商品基础信息模板
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="productGroupSysNo">商品组编号</param>
        public static ProductMaintainBasicInfo GetMaintainProductBasicInfoTemplate(int sellerSysNo, int productGroupSysNo)
        {
            ProductMaintainBasicInfo template = new ProductMaintainBasicInfo();
            template = ProductMaintainDA.GetProductBasicInfoByProductGroupSysNo(productGroupSysNo);
            template.SellerSysNo = sellerSysNo;
            template.NormalProperties = ProductMaintainDA.GetNormalPropertyByCategorySysNo(template.C3SysNo);
            template.NormalPropertyValues = ProductMaintainDA.GetNormalPropertyValueByCategorySysNo(template.C3SysNo);
            template.CountryCodeList = ProductMaintainDA.GetProductCountryList();
            template.CategoryText = GetCategoryTextByCategorySysNo(template.C3SysNo);
            template.SelectNormalProperties = BuildProductPerformanceToList(template.Performance);
            return template;
        }
        /// <summary>
        /// 获取前台类别文本
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="categorySysNo">类别编号</param>
        /// <returns></returns>
        private static string GetFrontCategoryTextByCategorySysNo(int sellerSysNo, int categorySysNo)
        {
            FrontProductCategoryInfo category = ProductMaintainDA.GetFrontProductCategoryBySysNo(sellerSysNo, categorySysNo);
            string categoryText = category.CategoryName.Trim();
            if (category.IsLeaf == Enums.CommonYesOrNo.Yes && !string.IsNullOrWhiteSpace(category.ParentCategoryCode))
            {
                category = ProductMaintainDA.GetFrontProductCategoryByCode(category.ParentCategoryCode);
                categoryText = string.IsNullOrWhiteSpace(categoryText) ? category.CategoryName.Trim() : string.Format("{0} > {1}", category.CategoryName.Trim(), categoryText);
            }

            return categoryText;
        }
        /// <summary>
        /// 获取类别文本
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="categorySysNo">C3</param>
        /// <returns></returns>
        private static string GetCategoryTextByCategorySysNo(int categorySysNo)
        {
            return ProductMaintainDA.GetCategoryTextByCategorySysNo(categorySysNo);
        }

        /// <summary>
        /// 新建商品基础信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ProductMaintainBasicInfo CreateProductBasicInfo(ProductMaintainBasicInfo entity)
        {
            if (string.IsNullOrEmpty(entity.ProductDescLong))
            {
                throw new BusinessException(LanguageHelper.GetText("请输入商品的详细描述！"));
            }
            if (entity.SelectNormalProperties != null && entity.SelectNormalProperties.Count > 0)
            {
                entity.Performance = BuildProductPerformance(0, entity.SelectNormalProperties);
            }
            else
            {
                entity.Performance = "";
            }

            using (ITransaction transaction = TransactionManager.Create())
            {
                //分组属性
                var splitGroupProperties = entity.SelectNormalProperties.FindAll(m => m.IsSplitGroupProperty.Equals(1));
                string splitName = "";
                ProductMaintainGroupProductInfo groupPropertyInfo = new ProductMaintainGroupProductInfo();
                if (splitGroupProperties.Count > 0)
                {
                    splitName = "({0})";
                    if (splitGroupProperties.Count >= 1)
                    {
                        groupPropertyInfo.ValueSysNo1 = splitGroupProperties[0].PropertyValueSysNo;
                        groupPropertyInfo.ValueName1 = splitGroupProperties[0].ValueDescription;
                    }
                    if (splitGroupProperties.Count >= 2)
                    {
                        groupPropertyInfo.ValueSysNo2 = splitGroupProperties[1].PropertyValueSysNo;
                        groupPropertyInfo.ValueName2 = splitGroupProperties[1].ValueDescription;
                    }
                    splitName = string.Format(splitName, BuildSplitName(groupPropertyInfo));
                }
                entity.ProductTitle = string.Format("{0}{1}", entity.ProductName, splitName);

                BuildProductID(entity);
                entity = ProductMaintainDA.CreateProductBasicInfo(entity);
                //更新一般属性
                string performance = BuildProductPerformance(entity.ProductSysNo.Value, entity.SelectNormalProperties);
                ProductMaintainDA.UpdateProductPerformanceByProductID(entity.ProductID, performance);
                //新建商品属性
                foreach (var item in entity.SelectNormalProperties)
                {
                    ProductMaintainDA.CreateProductProperties(entity.ProductSysNo.Value, entity.ProductID, item);
                }
                //创建商品分组属性
                foreach (var item in splitGroupProperties)
                {
                    ProductMaintainDA.CreateProductGroupInfoPropertySetting(entity.ProductGroupSysNo.Value, item);
                }

                transaction.Complete();
            }
            return entity;
        }

        /// <summary>
        /// 构造商品编码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static void BuildProductID(ProductMaintainBasicInfo entity)
        {
            entity.ProductID = BuildProductID(entity.SellerSysNo.Value, entity.BrandSysNo, entity.OriginCode, entity.C3SysNo);
        }
        private static string BuildProductID(int sellerSysNo, int productGroupSysNo)
        {
            string[] data = ProductMaintainDA.GetBuildProductIDDataByProductGroupSysNo(productGroupSysNo);
            return BuildProductID(sellerSysNo, int.Parse(data[0]), data[1], int.Parse(data[2]));
        }
        private static string BuildProductID(int sellerSysNo, int brandSysNo, string originCode, int c3SysNo)
        {
            string productID = "";
            string code = ProductMaintainDA.GetProductIDPreCode(c3SysNo, brandSysNo, sellerSysNo, originCode);
            string maxProductID = ProductMaintainDA.GetProductSameIDMaxProductID(code);
            int maxSerialNumber = string.IsNullOrWhiteSpace(maxProductID) ? 0 : int.Parse(maxProductID.Replace(code, ""));
            maxSerialNumber++;
            for (int i = maxSerialNumber.ToString().Length; i < 4; i++)
            {
                productID = string.Format("0{0}", productID);
            }
            productID = code + productID + maxSerialNumber.ToString();
            return productID;
        }

        /// <summary>
        /// 将属性转换成商品规格
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="productProperties"></param>
        /// <returns></returns>
        public static string BuildProductPerformance(int productSysNo, List<ProductPropertyInfo> productProperties)
        {
            var resultPerformance = new StringBuilder();
            string head = String.Empty;
            if (productSysNo > 0)
            {
                head = "<LongDescription Name='" + productSysNo + @"'>";
                resultPerformance.Append(head);
            }
            else
            {
                resultPerformance.Append("<LongDescription>");
            }

            List<int> propertyGroupSysNoList = productProperties.GroupBy(p => new { p.PropertyGroupSysNo }).Select(g => g.First().PropertyGroupSysNo).ToList<int>();
            foreach (int propertyGroupSysNo in propertyGroupSysNoList)
            {
                var properties = productProperties.FindAll(m => m.PropertyGroupSysNo.Equals(propertyGroupSysNo));
                resultPerformance.Append("<Group GroupName='" + properties[0].PropertyGroupName + "'>");
                foreach (var productProperty in properties)
                {
                    resultPerformance.Append("<Property Key='" +
                                             productProperty.PropertyName +
                                             "' Value='" + productProperty.ValueDescription + "'/>");
                }
                resultPerformance.Append("</Group>");
            }

            resultPerformance.Append("</LongDescription>");
            return resultPerformance.ToString();
        }
        /// <summary>
        /// 将商品规格(Performance)转换成列表
        /// </summary>
        /// <param name="xmlPerformance"></param>
        /// <returns></returns>
        public static List<ProductPropertyInfo> BuildProductPerformanceToList(string xmlPerformance)
        {
            List<ProductPropertyInfo> productProperties = new List<ProductPropertyInfo>();

            if (string.IsNullOrWhiteSpace(xmlPerformance) ||
                xmlPerformance == "</LongDescription>")
                return productProperties;

            LongDescription performance;
            xmlPerformance = xmlPerformance.Replace("&", "&amp;");
            XmlSerializer mySerializer = new XmlSerializer(typeof(LongDescription));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlPerformance)))
            {
                using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                {
                    performance = (LongDescription)mySerializer.Deserialize(sr);
                }
            }

            if (performance != null && performance.Groups != null && performance.Groups.Count > 0)
            {
                foreach (Group entity in performance.Groups)
                {
                    if (entity != null && entity.Propertys != null && entity.Propertys.Count > 0)
                    {
                        foreach (var item in entity.Propertys)
                        {
                            ProductPropertyInfo property = new ProductPropertyInfo();
                            property.PropertyGroupName = entity.GroupName;
                            property.PropertyName = item.Key;
                            property.ValueDescription = item.Value;
                            productProperties.Add(property);
                        }
                    }
                }
            }

            return productProperties;
        }

        /// <summary>
        /// 获取商品组是否设置同组商品
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static bool GetIsGroupProduct(int productGroupSysNo)
        {
            return ProductMaintainDA.GetIsGroupProduct(productGroupSysNo);
        }

        /// <summary>
        /// 更新商品基础信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool UpdateProductBasicInfoByProductGroupSysNo(ProductMaintainBasicInfo entity)
        {
            if (string.IsNullOrEmpty(entity.ProductDescLong))
            {
                throw new BusinessException(LanguageHelper.GetText("请输入商品的详细描述！"));
            }
            if (entity.SelectNormalProperties != null && entity.SelectNormalProperties.Count > 0)
            {
                entity.Performance = BuildProductPerformance(0, entity.SelectNormalProperties);
            }
            else
            {
                entity.Performance = "";
            }

            using (ITransaction transaction = TransactionManager.Create())
            {
                ProductMaintainDA.UpdateProductBasicInfoByProductGroupSysNo(entity);
                ProductMaintainDA.DeleteProductNormalPropertyByProductGroupSysNo(entity.ProductGroupSysNo.Value);
                if (entity.SelectNormalProperties != null && entity.SelectNormalProperties.Count > 0)
                {
                    List<ProductMaintainInfo> productList = ProductMaintainDA.GetProductListByProductGroupSysNo(entity.ProductGroupSysNo.Value);
                    foreach (ProductMaintainInfo product in productList)
                    {
                        foreach (ProductPropertyInfo property in entity.SelectNormalProperties)
                        {
                            ProductMaintainDA.CreateProductProperties(product.ProductSysNo, product.ProductID, property);
                        }
                    }
                }

                transaction.Complete();
            }
            return true;
        }

        /// <summary>
        /// 获取商品产地列表
        /// </summary>
        /// <returns></returns>
        public static List<ProductCountry> GetProductCountryList()
        {
            return ProductMaintainDA.GetProductCountryList();
        }
        #endregion

        #region 同组商品维护

        /// <summary>
        /// 获取维护同组商品模板
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static ProductMaintainGroupProduct GetMaintainGroupProductTemplate(int productGroupSysNo)
        {
            ProductMaintainGroupProduct template = new ProductMaintainGroupProduct();
            template.SplitGroupProperties = ProductMaintainDA.GetSplitGroupPropertyByProductGroupSysNo(productGroupSysNo);
            template.SplitGroupPropertyValues = ProductMaintainDA.GetSplitGroupPropertyValueByProductGroupSysNo(productGroupSysNo);
            template.ProductList = ProductMaintainDA.GetProductListByProductGroupSysNo(productGroupSysNo);
            return template;
        }

        /// <summary>
        /// 根据商品组编号获取商品列表
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static List<ProductMaintainInfo> GetProductListByProductGroupSysNo(int productGroupSysNo)
        {
            return ProductMaintainDA.GetProductListByProductGroupSysNo(productGroupSysNo);
        }

        /// <summary>
        /// 创建同款商品
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CreateGroupProduct(List<ProductMaintainGroupProductInfo> list)
        {
            string businessMessage = "";
            foreach (ProductMaintainGroupProductInfo item in list)
            {
                if (CheckProductIsExists(item))
                {
                    businessMessage += string.Format(LanguageHelper.GetText("{0}已存在，"), BuildSplitName(item));
                }
            }
            if (!string.IsNullOrWhiteSpace(businessMessage))
                throw new BusinessException(businessMessage.TrimEnd(new char[] { '，' }));

            //创建
            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (var item in list)
                {
                    string productID = BuildProductID(item.SellerSysNo.Value, item.ProductGroupSysNo);
                    string splitName = BuildSplitName(item);
                    int currProductSysNo = ProductMaintainDA.CreateGroupProduct(item, productID, splitName, 0);
                    ProductPropertyInfo property = new ProductPropertyInfo();
                    if (item.ValueSysNo1.Equals(0) || item.ValueSysNo2.Equals(0))
                    {
                        #region 只有一个属性
                        property.PropertyGroupSysNo = item.ProductGroupSysNo;
                        property.PropertyGroupName = item.PropertyGroupName;
                        property.SellerSysNo = item.SellerSysNo;
                        property.CompanyCode = item.CompanyCode;
                        property.LanguageCode = item.LanguageCode;
                        property.InUserSysNo = item.InUserSysNo;
                        property.InUserName = item.InUserName;
                        if (item.ValueSysNo1.Equals(0))
                        {
                            property.PropertySysNo = item.PropertySysNo2;
                            property.PropertyName = item.PropertyName2;
                            property.PropertyValueSysNo = item.ValueSysNo2;
                            property.ValueDescription = item.ValueName2;
                        }
                        else
                        {
                            property.PropertySysNo = item.PropertySysNo1;
                            property.PropertyName = item.PropertyName1;
                            property.PropertyValueSysNo = item.ValueSysNo1;
                            property.ValueDescription = item.ValueName1;
                        }
                        ProductMaintainDA.CreateProductProperties(currProductSysNo, productID, property);
                        #endregion
                    }
                    else
                    {
                        #region 有2个属性
                        property.PropertyGroupSysNo = item.ProductGroupSysNo;
                        property.PropertyGroupName = item.PropertyGroupName;
                        property.SellerSysNo = item.SellerSysNo;
                        property.CompanyCode = item.CompanyCode;
                        property.LanguageCode = item.LanguageCode;
                        property.InUserSysNo = item.InUserSysNo;
                        property.InUserName = item.InUserName;
                        property.PropertySysNo = item.PropertySysNo1;
                        property.PropertyName = item.PropertyName1;
                        property.PropertyValueSysNo = item.ValueSysNo1;
                        property.ValueDescription = item.ValueName1;
                        ProductMaintainDA.CreateProductProperties(currProductSysNo, productID, property);
                        property.PropertyGroupSysNo = item.ProductGroupSysNo;
                        property.PropertyGroupName = item.PropertyGroupName;
                        property.SellerSysNo = item.SellerSysNo;
                        property.CompanyCode = item.CompanyCode;
                        property.LanguageCode = item.LanguageCode;
                        property.InUserSysNo = item.InUserSysNo;
                        property.InUserName = item.InUserName;
                        property.PropertySysNo = item.PropertySysNo2;
                        property.PropertyName = item.PropertyName2;
                        property.PropertyValueSysNo = item.ValueSysNo2;
                        property.ValueDescription = item.ValueName2;
                        ProductMaintainDA.CreateProductProperties(currProductSysNo, productID, property);
                        #endregion
                    }
                }

                transaction.Complete();
            }

            return true;
        }
        private static string BuildSplitName(ProductMaintainGroupProductInfo entity)
        {
            if (entity.ValueSysNo1.Equals(0) || entity.ValueSysNo2.Equals(0))
            {
                if (entity.ValueSysNo1.Equals(0))
                    return entity.ValueName2;
                else
                    return entity.ValueName1;
            }
            else
            {
                return string.Format("{0}_{1}", entity.ValueName1, entity.ValueName2);
            }
        }

        /// <summary>
        /// 判断商品是否已经存在
        /// </summary>
        /// <param name="product"></param>
        /// <returns>True=存在，False=不存在</returns>
        private static bool CheckProductIsExists(ProductMaintainGroupProductInfo product)
        {
            bool bResult = false;
            if (product.ValueSysNo1.Equals(0) || product.ValueSysNo2.Equals(0))
            {
                //只设置一个分组属性
                List<int> existsProductSysNo = new List<int>();
                if (product.ValueSysNo1.Equals(0))
                {
                    existsProductSysNo = ProductMaintainDA.GetProductGroupSplitPropertyProductList(product.ProductGroupSysNo, product.PropertySysNo2, product.ValueSysNo2);
                }
                else
                {
                    existsProductSysNo = ProductMaintainDA.GetProductGroupSplitPropertyProductList(product.ProductGroupSysNo, product.PropertySysNo1, product.ValueSysNo1);
                }
                if (existsProductSysNo != null && existsProductSysNo.Count > 0)
                    bResult = true;
            }
            else
            {
                //设置两个分组属性
                List<int> existsProductSysNo1 = new List<int>();
                List<int> existsProductSysNo2 = new List<int>();
                existsProductSysNo1 = ProductMaintainDA.GetProductGroupSplitPropertyProductList(product.ProductGroupSysNo, product.PropertySysNo1, product.ValueSysNo1);
                existsProductSysNo2 = ProductMaintainDA.GetProductGroupSplitPropertyProductList(product.ProductGroupSysNo, product.PropertySysNo2, product.ValueSysNo2);
                foreach (int productSysNo in existsProductSysNo1)
                {
                    if (existsProductSysNo2.Exists(m => m.Equals(productSysNo)))
                    {
                        bResult = true;
                        break;
                    }
                }
            }
            return bResult;
        }

        /// <summary>
        /// 获取单个商品维护信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="sellerSysNo">商家编号</param>
        /// <returns></returns>
        public static ProductMaintainInfo GetSingleProductMaintainInfo(int productSysNo, int sellerSysNo)
        {
            ProductMaintainInfo info = ProductMaintainDA.GetSingleProductMaintainInfo(productSysNo);
            if (info == null || string.IsNullOrWhiteSpace(info.MerchantInvoiceType))
            {
                if (info == null)
                    info = new ProductMaintainInfo();
                info.MerchantInvoiceType = ProductMaintainDA.GetSellerInvoiceType(sellerSysNo);
            }
            return info;
        }

        /// <summary>
        /// 更新单个商品信息
        /// </summary>
        /// <param name="entity">商品信息</param>
        /// <returns></returns>
        public static bool UpdateSingleProductMaintainInfo(ProductMaintainInfo entity)
        {
            bool bResult = false;
            using (ITransaction transaction = TransactionManager.Create())
            {
                bResult = ProductMaintainDA.UpdateSingleProductMaintainInfo(entity);

                transaction.Complete();
            }
            return bResult;
        }

        #endregion

        #region 图片信息维护

        /// <summary>
        /// 保存商品图片信息
        /// </summary>
        /// <param name="imageInfoList">商品图片信息列表</param>
        /// <returns></returns>
        public static bool SaveProductImageInfo(List<ProductImageInfo> imageInfoList)
        {
            bool bResult = false;

            if (imageInfoList == null || imageInfoList.Count == 0)
                throw new BusinessException(LanguageHelper.GetText("请选择商品上传图片！"));
            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (var imageInfo in imageInfoList)
                {
                    ProductMaintainDA.SaveProductImageInfo(imageInfo);
                }

                transaction.Complete();
            }

            return bResult;
        }

        /// <summary>
        /// 根据商品组编号查询商品图片列表
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static List<ProductImageInfo> GetProductImageListByProductGroupSysNo(int productGroupSysNo)
        {
            return ProductMaintainDA.GetProductImageListByProductGroupSysNo(productGroupSysNo);
        }

        /// <summary>
        /// 更新商品图片优先级
        /// </summary>
        /// <param name="imageInfo">商品图片信息</param>
        /// <returns></returns>
        public static bool UpdateProductImagePriority(ProductImageInfo imageInfo)
        {
            return ProductMaintainDA.UpdateProductImagePriority(imageInfo);
        }

        /// <summary>
        /// 删除指定商品图片
        /// </summary>
        /// <param name="sysNoList">商品图片编号列表</param>
        /// <returns></returns>
        public static bool DeleteProductImageBySysNo(List<int> sysNoList)
        {
            bool bResult = false;

            //[0]为商品组编号
            if (sysNoList == null || sysNoList.Count < 2)
                throw new BusinessException(LanguageHelper.GetText("请选择商品！"));
            var productImageList = ProductMaintainDA.GetProductImageListByProductGroupSysNo(sysNoList[0]);
            using (ITransaction transaction = TransactionManager.Create())
            {
                for (int i = 1; i < sysNoList.Count; i++)
                {
                    var productImage = productImageList.Find(m => m.SysNo.Equals(sysNoList[i]));
                    if(productImage != null && productImage.ResourceUrl.Equals(productImage.DefaultImage))
                        throw new BusinessException(string.Format(LanguageHelper.GetText("商品【{0}】为默认图片，不能删除！"), productImage.InDate.ToString()));

                }
                for (int j = 1; j < sysNoList.Count; j++)
                {
                    ProductMaintainDA.DeleteProductImageBySysNo(sysNoList[j]);
                }

                transaction.Complete();
            }

            return bResult;
        }

        /// <summary>
        /// 设置商品默认图片
        /// </summary>
        /// <param name="sysNo">商品图片编号</param>
        /// <returns></returns>
        public static bool SetProductDefaultImage(ProductImageInfo imageInfo)
        {
            return ProductMaintainDA.SetProductDefaultImage(imageInfo);
        }

        #endregion

        #region 价格信息维护

        /// <summary>
        /// 根据商品编号获取商品价格信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductPriceInfo GetProductPriceInfoByProductSysNo(int productSysNo)
        {
            ProductPriceInfo priceInfo = ProductMaintainDA.GetProductPriceInfoByProductSysNo(productSysNo);
            if (priceInfo == null)
            {
                priceInfo = new ProductPriceInfo();
                priceInfo.ProductSysNo = productSysNo;
                priceInfo.MinCountPerOrder = 1;
            }
            return priceInfo;
        }

        /// <summary>
        /// 保存商品价格信息
        /// </summary>
        /// <param name="priceInfoList">价格信息</param>
        /// <returns></returns>
        public static bool MaintainProductPriceInfo(List<ProductPriceInfo> priceInfoList)
        {
            bool result = false;
            
            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (ProductPriceInfo priceInfo in priceInfoList)
                {
                    priceInfo.VirtualPrice = priceInfo.UnitCost;
                    //var product = ProductMaintainDA.GetProductByProductSysNo(priceInfo.ProductSysNo);
                    //if (product.Status == Enums.ProductStatus.Active)
                    //{
                    //    throw new BusinessException(LanguageHelper.GetText("已上架的商品不能直接保存价格，请下架后再保存！"));
                    //}
                    priceInfo.Discount = priceInfo.BasicPrice == 0 ? 0 : decimal.Parse((priceInfo.CurrentPrice / priceInfo.BasicPrice).ToString("#.##"));
                    ProductMaintainDA.SaveProductPriceInfo(priceInfo);
                }

                result = true;

                transaction.Complete();
            }
            return result;
        }

        #endregion

        #region 备案信息维护

        /// <summary>
        /// 根据商品编号获取商品备案信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductEntryInfo GetProductEntryInfoByProductSysNo(int productSysNo)
        {
            ProductEntryInfo entryInfo = ProductMaintainDA.GetProductEntryInfoByProductSysNo(productSysNo);
            if (entryInfo == null)
            {
                entryInfo = new ProductEntryInfo();
                entryInfo.ProductSysNo = productSysNo;
                var product = ProductMaintainDA.GetProductByProductSysNo(productSysNo);
                if (product != null)
                    entryInfo.ProductTitle = product.ProductTitle;
            }
            return entryInfo;
        }

        /// <summary>
        /// 保存商品备案信息
        /// </summary>
        /// <param name="entryInfo">备案信息</param>
        /// <returns></returns>
        public static bool MaintainProductEntryInfo(ProductEntryInfo entryInfo)
        {
            ProductEntryInfo currEntryInfo = ProductMaintainDA.GetProductEntryInfoByProductSysNo(entryInfo.ProductSysNo);
            if (currEntryInfo != null
                && (currEntryInfo.EntryStatus == Enums.ProductEntryStatus.Entry
                || currEntryInfo.EntryStatus == Enums.ProductEntryStatus.EntrySuccess))
            {
                throw new BusinessException(LanguageHelper.GetText("只有待备案和备案失败的商品才能维护备案信息！"));
            }
            return ProductMaintainDA.SaveProductEntryInfo(entryInfo);
        }

        #endregion

        #region 备案信息管理

        /// <summary>
        /// 查询备案信息
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="dataCount">总记录数</param>
        /// <returns></returns>
        public static QueryResult<ProductEntryInfo> QueryProductEntryInfo(ProductEntryInfoQueryFilter filter)
        {
            int count = 0;
            return new QueryResult<ProductEntryInfo>(ProductMaintainDA.ProductEntryInfoQuery(filter, out count), filter, count);
        }

        /// <summary>
        /// 批量提交备案申请
        /// </summary>
        /// <param name="productSysNoList">商品编号</param>
        /// <returns></returns>
        public static bool BatchSubmitProductEntryAudit(List<int> productSysNoList)
        {
            if(productSysNoList == null || productSysNoList.Count == 0)
                throw new BusinessException(LanguageHelper.GetText("请选择商品！"));

            string businessMessage = "";
            foreach (int productSysNo in productSysNoList)
            {
                var entryInfo = ProductMaintainDA.GetProductEntryInfoByProductSysNo(productSysNo);
                if (entryInfo == null)
                {
                    businessMessage += string.Format(LanguageHelper.GetText("商品【{0}】还没填写备案信息，"), productSysNo);
                }
                else if (entryInfo.EntryStatus != Enums.ProductEntryStatus.Origin
                     && entryInfo.EntryStatus != Enums.ProductEntryStatus.AuditFail)
                {
                    businessMessage += string.Format(LanguageHelper.GetText("商品【{0}】不是初始态或审核失败，"), productSysNo);
                }
            }
            if (!string.IsNullOrWhiteSpace(businessMessage))
            {
                businessMessage = businessMessage.TrimEnd(new char[] { '，' });
                throw new BusinessException(businessMessage);
            }

            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (int productSysNo in productSysNoList)
                {
                    ProductMaintainDA.SubmitProductEntryAudit(productSysNo);
                }

                transaction.Complete();
            }

            return true;
        }

        #endregion

        #region 商品上下架上架不展示，作废管理

        /// <summary>
        /// 商品批量上架
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns></returns>
        public static bool ProductBatchOnline(List<int> productSysNoList)
        {
            if (productSysNoList == null || productSysNoList.Count == 0)
                throw new BusinessException(LanguageHelper.GetText("请选择商品！"));

            string checkMessage = "";
            foreach (int productSysNo in productSysNoList)
            {
                string currCheckMessage = CheckProductOnline(productSysNo);
                if (!string.IsNullOrWhiteSpace(currCheckMessage))
                {
                    checkMessage += currCheckMessage;
                }
                string CheckMessagePromoting = CheckProductPromoting(productSysNo);
                if (!string.IsNullOrWhiteSpace(CheckMessagePromoting))
                {
                    checkMessage += CheckMessagePromoting;
                }
            }

            if (!string.IsNullOrWhiteSpace(checkMessage))
            {
                throw new BusinessException(checkMessage);
            }
            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (int productSysNo in productSysNoList)
                {
                    ProductMaintainDA.UpdateProductStatus(productSysNo, Enums.ProductStatus.Active);
                }

                transaction.Complete();
            }

            return true;
        }

        /// <summary>
        /// 商品批量上架不展示
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns></returns>
        public static bool ProductBatchOnlineNotShow(List<int> productSysNoList)
        {
            if (productSysNoList == null || productSysNoList.Count == 0)
                throw new BusinessException(LanguageHelper.GetText("请选择商品！"));

            string checkMessage = "";
            foreach (int productSysNo in productSysNoList)
            {
                string currCheckMessage = CheckProductOnline(productSysNo);
                if (!string.IsNullOrWhiteSpace(currCheckMessage))
                    checkMessage += currCheckMessage;
            }

            if (!string.IsNullOrWhiteSpace(checkMessage))
            {
                throw new BusinessException(checkMessage);
            }
            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (int productSysNo in productSysNoList)
                {
                    ProductMaintainDA.UpdateProductStatus(productSysNo, Enums.ProductStatus.InActive_Show);
                }

                transaction.Complete();
            }

            return true;
        }

        /// <summary>
        /// 商品批量下架
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns></returns>
        public static bool ProductBatchOffline(List<int> productSysNoList)
        {
            if (productSysNoList == null || productSysNoList.Count == 0)
                throw new BusinessException(LanguageHelper.GetText("请选择商品！"));
            string checkMessage = "";
            foreach (int productSysNo in productSysNoList)
            {
                string currCheckMessage = CheckProductUnShow(productSysNo);
                if (!string.IsNullOrWhiteSpace(currCheckMessage))
                    checkMessage += currCheckMessage;
            }

            if (!string.IsNullOrWhiteSpace(checkMessage))
            {
                throw new BusinessException(checkMessage);
            }
            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (int productSysNo in productSysNoList)
                {
                    ProductMaintainDA.UpdateProductStatus(productSysNo, Enums.ProductStatus.InActive_UnShow);
                }

                transaction.Complete();
            }

            return true;
        }

        /// <summary>
        /// 商品批量作废
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns></returns>
        public static bool ProductBatchAbandon(List<int> productSysNoList)
        {
            if (productSysNoList == null || productSysNoList.Count == 0)
                throw new BusinessException(LanguageHelper.GetText("请选择商品！"));
            string checkMessage = "";
            foreach (int productSysNo in productSysNoList)
            {
                string currCheckMessage = CheckProductUnShow(productSysNo);
                if (!string.IsNullOrWhiteSpace(currCheckMessage))
                    checkMessage += currCheckMessage;
            }

            if (!string.IsNullOrWhiteSpace(checkMessage))
            {
                throw new BusinessException(checkMessage);
            }

            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (int productSysNo in productSysNoList)
                {
                    ProductMaintainDA.UpdateProductStatus(productSysNo, Enums.ProductStatus.Abandon);
                }

                transaction.Complete();
            }

            return true;
        }
        /// <summary>
        /// 检查商品是否能下架或作废
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        private static string CheckProductUnShow(int productSysNo)
        {
            string checkMessage = "";
            var info = ProductMaintainDA.GetProductOnSaleInfoByProductSysNo(productSysNo);
            if (info.Status == ProductStatus.InActive_Auditing)
            {
                checkMessage += LanguageHelper.GetText("商品还处于待审核状态，");
            }
            if (info.Status == ProductStatus.InActive_AuditenNO)
            {
                checkMessage += LanguageHelper.GetText("商品审核未通过，");
            }
            if (string.IsNullOrWhiteSpace(checkMessage))
                return "";

            checkMessage = checkMessage.TrimEnd(new char[] { '，' });
            checkMessage += "！\r\n";
            checkMessage = string.Format(LanguageHelper.GetText("商品编号为【{0}】的商品不能下架或作废：\n{1}"), productSysNo, checkMessage);
            return checkMessage;
        }

        /// <summary>
        /// 检查商品是否能上架
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        private static string CheckProductOnline(int productSysNo)
        {
            string checkMessage = "";
            var info = ProductMaintainDA.GetProductOnSaleInfoByProductSysNo(productSysNo);
            if (info == null)
                checkMessage += LanguageHelper.GetText("不存在，");
            if(string.IsNullOrWhiteSpace(info.DefaultImage))
                checkMessage += LanguageHelper.GetText("没有设置默认图片，");
            if (!info.BrandStatus.Equals("A"))
                checkMessage += LanguageHelper.GetText("品牌无效，");
            if (!info.CategoryStatus.Equals(0))
                checkMessage += LanguageHelper.GetText("类别无效，");
            if (string.IsNullOrWhiteSpace(info.ProductDesc))
                checkMessage += LanguageHelper.GetText("商品描述为空，");
            if (string.IsNullOrWhiteSpace(info.ProductDescLong))
                checkMessage += LanguageHelper.GetText("详细描述为空，");
            if (info.BasicPrice.HasValue && info.BasicPrice.Value < 0)
                checkMessage += LanguageHelper.GetText("市场价不能小于0，");
            if (!info.CurrentPrice.HasValue || info.CurrentPrice.Value <= 0)
                checkMessage += LanguageHelper.GetText("还未设置销售价，");
            if (!info.Weight.HasValue || info.Weight.Value <= 0)
                checkMessage += LanguageHelper.GetText("还未设置重量，");
            if (info.Status == ProductStatus.InActive_Auditing)
            {
                checkMessage += LanguageHelper.GetText("商品还处于待审核状态，");
            }
            if (info.Status == ProductStatus.InActive_AuditenNO)
            {
                checkMessage += LanguageHelper.GetText("商品审核未通过，");
            }
            if (ProductMaintainDA.GetSellerInvoiceType(info.MerchantSysNo) == "GUD")
            {
                var pInfo = ProductMaintainDA.GetSingleProductMaintainInfo(productSysNo);
                if(string.IsNullOrWhiteSpace(pInfo.ProductOutUrl))
                    checkMessage += LanguageHelper.GetText("还未设置导购链接；请在同款商品或者商品查询页编辑商品进行设置，");
            }

            //List<ProductPropertyInfo> properties = BuildProductPerformanceToList(info.Performance);
            //if(properties == null || properties.Count == 0)
            //    checkMessage += string.Format(LanguageHelper.GetText("商品【{0}】属性为空，"), productSysNo);

            //if(ProductMaintainDA.ProductIsGift(productSysNo))
            //    checkMessage += string.Format(LanguageHelper.GetText("商品【{0}】是加购商品，"), productSysNo);

            //备案信息
            //if (info.ProductTradeType == Enums.TradeType.DirectMail
            //    || info.ProductTradeType == Enums.TradeType.FTA)
            //{
            //    if (info.EntryStatus == Enums.ProductEntryStatus.EntrySuccess)
            //    {
            //        if (string.IsNullOrWhiteSpace(info.TariffCode))
            //            checkMessage += LanguageHelper.GetText("税则号为空，");
            //        if (string.IsNullOrWhiteSpace(info.EntryCode))
            //            checkMessage += LanguageHelper.GetText("备案号为空，");
            //        if (info.BizType == Enums.ProductBizType.Tax)
            //        {
            //            //自贸商品
            //            if (string.IsNullOrWhiteSpace(info.ProductSKUNO))
            //                checkMessage += LanguageHelper.GetText("货号为空，");
            //            if (string.IsNullOrWhiteSpace(info.SuppliesSerialNo))
            //                checkMessage += LanguageHelper.GetText("物资序号为空，");
            //            if (string.IsNullOrWhiteSpace(info.ApplyUnit))
            //                checkMessage += LanguageHelper.GetText("申报单位为空，");
            //            if (info.ApplyQty <= 0)
            //                checkMessage += LanguageHelper.GetText("申报数量必须大于0，");
            //            if (info.GrossWeight <= 0)
            //                checkMessage += LanguageHelper.GetText("净重必须大于0，");
            //            if (info.SuttleWeight <= 0)
            //                checkMessage += LanguageHelper.GetText("毛重必须大于0，");
            //        }
            //    }
            //    else
            //    {
            //        checkMessage += LanguageHelper.GetText("还未备案成功，");
            //    }
            //}

            if (string.IsNullOrWhiteSpace(checkMessage))
                return "";

            checkMessage = checkMessage.TrimEnd(new char[] { '，' });
            checkMessage += "！\r\n";
            checkMessage = string.Format(LanguageHelper.GetText("商品编号为【{0}】的商品不能上架或上架不展示：\n{1}"), productSysNo, checkMessage);

            return checkMessage;
        }

        /// <summary>
        /// 根据赠品编号判断此赠品活动是否在审核中，就绪中，运行中
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        private static string CheckProductPromoting(int productSysNo)
        {
            string checkMessage = "";
            var info = ProductMaintainDA.GetProductOnSaleInfoByProductSysNo(productSysNo);
            if (info == null)
            {
                checkMessage += LanguageHelper.GetText("不存在，");
            }
            else if (info.Status == ProductStatus.InActive_Show)
            {
                if (GiftPromotionDA.CheckGiftItemListByProductSysNo(info.ProductSysNo))
                {
                    checkMessage += LanguageHelper.GetText("正在参加赠品促销活动，");
                }
            }

            if (string.IsNullOrWhiteSpace(checkMessage))
                return "";

            checkMessage = checkMessage.TrimEnd(new char[] { '，' });
            checkMessage += "！\r\n";
            checkMessage = string.Format(LanguageHelper.GetText("商品编号为【{0}】的商品暂时不能上架，：\n{1}"), productSysNo, checkMessage);

            return checkMessage;
        }
        #endregion

        #region 商品销售区域
        /// <summary>
        /// 获取商品销售区域列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static QueryResult<ProductSalesAreaInfo> GetProductSalesAreaInfoBySysNo(ProductQueryFilter queryFilter)
        {
            return ProductMaintainDA.GetProductSalesAreaInfoBySysNo(queryFilter);
        }

        /// <summary>
        /// 新增商品销售区域列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static void InsertProductSalesArea(ProductQueryInfo productInfo, List<ProductSalesAreaInfo> productSalesAreaInfo)
        {
            ProductMaintainDA.DeleteProductSalesArea(productInfo.SysNo);
            foreach (var productSalesInfo in productSalesAreaInfo)
            {
                ProductMaintainDA.InsertProductSalesArea(productInfo, productSalesInfo);
            }
            
        }

        /// <summary>
        /// 根据SysNo获取商品[ProductID]和[ProductTitle]
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductQueryInfo GetProductTitleByProductSysNo(int productSysNo)
        {
            return ProductMaintainDA.GetProductTitleByProductSysNo(productSysNo);
        }

        #endregion

        #region 商品批号
        /// <summary>
        /// 更新商品批号
        /// </summary>
        /// <param name="batchManagementInfo"></param>
        /// <returns></returns>
        public static ProductBatchManagementInfo UpdateIsBatch(ProductBatchManagementInfo batchManagementInfo)
        {

            using (ITransaction transaction = TransactionManager.Create())
            {
                //更新商品批号
                ProductMaintainDA.UpdateIsBatch(batchManagementInfo);
                //根据商品SysNo获取商品批号
                var batch = ProductMaintainDA.GetProductBatchManagementInfo(batchManagementInfo.ProductSysNo.Value);

                if (!string.IsNullOrEmpty(batchManagementInfo.Note) && batchManagementInfo.IsBatch.Value)
                {
                    var log = new ProductBatchManagementInfoLog { Note = batchManagementInfo.Note, BatchManagementSysNo = batch.SysNo, InUser = batchManagementInfo.EidtUser };
                    //新增历史备注
                    ProductMaintainDA.InsertProductBatchManagementLog(log);
                }
                transaction.Complete();
            }
            //获取更新后的商品批号
            return ProductMaintainDA.GetProductBatchManagementInfo(batchManagementInfo.ProductSysNo.Value);
        }

        /// <summary>
        /// 根据商品SysNo获取商品批号和历史备注
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductBatchManagementInfo GetProductBatchManagementInfo(int productSysNo)
        {
            return ProductMaintainDA.GetProductBatchManagementInfo(productSysNo);
        }
        #endregion
    }
}
