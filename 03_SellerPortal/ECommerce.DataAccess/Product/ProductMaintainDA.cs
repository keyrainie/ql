using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using ECommerce.Entity.Common;

namespace ECommerce.DataAccess.Product
{
    public class ProductMaintainDA
    {
        #region 选择分类
        //查询该商家的商品分类
        public static List<CategoryInfo> GetCategory1List(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategory1List");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteEntityList<CategoryInfo>();
        }
        public static List<CategoryInfo> GetCategory2List(int sellerSysNo, int C1SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategory2List");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            cmd.SetParameterValue("@C1SysNo", C1SysNo);
            return cmd.ExecuteEntityList<CategoryInfo>();
        }
        public static List<CategoryInfo> GetCategory3List(int sellerSysNo, int C2SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategory3List");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            cmd.SetParameterValue("@C2SysNo", C2SysNo);
            return cmd.ExecuteEntityList<CategoryInfo>();
        }

        //查询所有商品分类
        public static List<CategoryInfo> GetAllCategory1List()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllCategory1List");
            return cmd.ExecuteEntityList<CategoryInfo>();
        }
        public static List<CategoryInfo> GetAllCategory2List(int C1SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllCategory2List");
            cmd.SetParameterValue("@C1SysNo", C1SysNo);
            return cmd.ExecuteEntityList<CategoryInfo>();
        }
        public static List<CategoryInfo> GetAllCategory3List(int C2SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllCategory3List");
            cmd.SetParameterValue("@C2SysNo", C2SysNo);
            return cmd.ExecuteEntityList<CategoryInfo>();
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
            DataCommand cmd = DataCommandManager.GetDataCommand("GetParentFrontProductCategory");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteEntityList<FrontProductCategoryInfo>();
        }

        /// <summary>
        /// 获取商家前台根级类别
        /// </summary>
        /// <param name="parentCategoryCode">父类别编码</param>
        /// <returns></returns>
        public static List<FrontProductCategoryInfo> GetFrontProductCategoryByParentCode(string parentCategoryCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFrontProductCategoryByParentCode");
            cmd.SetParameterValue("@ParentCategoryCode", parentCategoryCode);
            return cmd.ExecuteEntityList<FrontProductCategoryInfo>();
        }

        /// <summary>
        /// 获取指定商家编号和商家前台类别编号的前台类别
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="sysNo">类别编号</param>
        /// <returns></returns>
        public static FrontProductCategoryInfo GetFrontProductCategoryBySysNo(int sellerSysNo, int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFrontProductCategoryBySysNo");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteEntity<FrontProductCategoryInfo>();
        }

        /// <summary>
        /// 获取指定类别编码的商家前台类别
        /// </summary>
        /// <param name="categoryCode">类别编码</param>
        /// <returns></returns>
        public static FrontProductCategoryInfo GetFrontProductCategoryByCode(string categoryCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFrontProductCategoryByCode");
            cmd.SetParameterValue("@CategoryCode", categoryCode);
            return cmd.ExecuteEntity<FrontProductCategoryInfo>();
        }

        /// <summary>
        /// 创建商品基础信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ProductMaintainBasicInfo CreateProductBasicInfo(ProductMaintainBasicInfo entity)
        {
            entity.MerchantSysNo = entity.SellerSysNo.Value;
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductBasicInfo");
            cmd.SetParameterValue<ProductMaintainBasicInfo>(entity);
            cmd.ExecuteNonQuery();
            entity.ProductGroupSysNo = (int)cmd.GetParameterValue("@ProductGroupSysNo");
            entity.ProductSysNo = (int)cmd.GetParameterValue("@ProductSysNo");
            return entity;
        }

        /// <summary>
        /// 获取指定C3的分组属性，2个
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        public static List<CategoryPropertyInfo> GetSplitPropertyByCategorySysNo(int categorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSplitPropertyByCategorySysNo");
            cmd.SetParameterValue("@CategorySysNo", categorySysNo);
            return cmd.ExecuteEntityList<CategoryPropertyInfo>();
        }

        /// <summary>
        /// 创建商品组分组属性
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <param name="propertyInfo">分组属性</param>
        /// <returns></returns>
        public static bool CreateProductGroupInfoPropertySetting(int productGroupSysNo, ProductPropertyInfo propertyInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductGroupInfoPropertySetting");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            cmd.SetParameterValue("@PropertySysNo", propertyInfo.PropertySysNo);
            cmd.SetParameterValue("@PropertyName", propertyInfo.PropertyName);
            cmd.SetParameterValue("@InUserName", propertyInfo.InUserName);
            cmd.SetParameterValue("@CompanyCode", propertyInfo.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", propertyInfo.LanguageCode);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 获取指定C3的一般属性
        /// </summary>
        /// <param name="categorySysNo">C3</param>
        /// <returns></returns>
        public static List<CategoryPropertyInfo> GetNormalPropertyByCategorySysNo(int categorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNormalPropertyByCategorySysNo");
            cmd.SetParameterValue("@CategorySysNo", categorySysNo);
            return cmd.ExecuteEntityList<CategoryPropertyInfo>();
        }

        /// <summary>
        /// 获取指定C3的一般属性的值
        /// </summary>
        /// <param name="categorySysNo">C3</param>
        /// <returns></returns>
        public static List<CategoryPropertyValueInfo> GetNormalPropertyValueByCategorySysNo(int categorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNormalPropertyValueByCategorySysNo");
            cmd.SetParameterValue("@CategorySysNo", categorySysNo);
            return cmd.ExecuteEntityList<CategoryPropertyValueInfo>();
        }

        /// <summary>
        /// 获取商品基础信息
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static ProductMaintainBasicInfo GetProductBasicInfoByProductGroupSysNo(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductBasicInfoByProductGroupSysNo");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            return cmd.ExecuteEntity<ProductMaintainBasicInfo>();
        }
        
        /// <summary>
        /// 获取同系商品ID前缀最大的商品ID
        /// </summary>
        /// <param name="code">商品ID前缀，如：xx_xxx_xxx</param>
        /// <returns></returns>
        public static string GetProductSameIDMaxProductID(string code)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductSameIDMaxProductID");
            cmd.SetParameterValue("@Code", code);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        /// <summary>
        /// 获取商品产地列表
        /// </summary>
        /// <returns></returns>
        public static List<ProductCountry> GetProductCountryList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductCountryList");
            return cmd.ExecuteEntityList<ProductCountry>();
        }

        /// <summary>
        /// 获取商品组是否设置同组商品
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static bool GetIsGroupProduct(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetIsGroupProduct");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0].ToString().Equals("1"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 更新商品基础信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool UpdateProductBasicInfoByProductGroupSysNo(ProductMaintainBasicInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductBasicInfoByProductGroupSysNo");
            cmd.SetParameterValue<ProductMaintainBasicInfo>(entity);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 新建商品属性
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="productID">商品ID</param>
        /// <param name="property">属性</param>
        /// <returns></returns>
        public static bool CreateProductProperties(int productSysNo, string productID, ProductPropertyInfo property)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductProperties");
            cmd.SetParameterValue("@CommonSKUNumber", productID);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@PropertyGroupSysNo", property.PropertyGroupSysNo);
            cmd.SetParameterValue("@PropertySysNo", property.PropertySysNo);
            cmd.SetParameterValue("@PropertyValueSysNo", property.PropertyValueSysNo);
            cmd.SetParameterValue("@PropertyName", property.PropertyName);
            cmd.SetParameterValue("@PropertyGroupName", property.PropertyGroupName);
            cmd.SetParameterValue("@ValueDescription", property.ValueDescription);
            cmd.SetParameterValue("@InUserSysNo", property.InUserSysNo);
            cmd.SetParameterValue("@InUserName", property.InUserName);
            cmd.SetParameterValue("@CompanyCode", property.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", property.LanguageCode);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 更新产品一般属性
        /// </summary>
        /// <param name="productID">产品编码</param>
        /// <param name="performance">一般属性</param>
        /// <returns></returns>
        public static bool UpdateProductPerformanceByProductID(string productID, string performance)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductPerformanceByProductID");
            cmd.SetParameterValue("@ProductID", productID);
            cmd.SetParameterValue("@Performance", performance);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 根据商品组编号删除商品一般属性
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static bool DeleteProductNormalPropertyByProductGroupSysNo(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductNormalPropertyByProductGroupSysNo");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 根据商品组编号获取构建商品
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static string[] GetBuildProductIDDataByProductGroupSysNo(int productGroupSysNo)
        {
            string[] result = new string[3];
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBuildProductIDDataByProductGroupSysNo");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                result[0] = dt.Rows[0]["BrandSysno"].ToString();
                result[1] = dt.Rows[0]["Origin"].ToString();
                result[2] = dt.Rows[0]["C3SysNo"].ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取类别文本
        /// </summary>
        /// <param name="c3SysNo">C3</param>
        /// <returns></returns>
        public static string GetCategoryTextByCategorySysNo(int c3SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategoryTextByCategorySysNo");
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return "";
        }

        /// <summary>
        /// 获取商品ID前缀
        /// </summary>
        /// <param name="c3SysNo">C3</param>
        /// <param name="brandSysNo">品牌编号</param>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="countryCode">国家编码</param>
        /// <returns></returns>
        public static string GetProductIDPreCode(int c3SysNo, int brandSysNo, int sellerSysNo, string countryCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductIDPreCode");
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            cmd.SetParameterValue("@CountryCode", countryCode);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return "";
        }

        #endregion

        #region 同组商品维护

        /// <summary>
        /// 获取商品组的分组属性
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static List<CategoryPropertyInfo> GetSplitGroupPropertyByProductGroupSysNo(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSplitGroupPropertyByProductGroupSysNo");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            return cmd.ExecuteEntityList<CategoryPropertyInfo>();
        }

        /// <summary>
        /// 获取商品组的分组属性的值
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static List<CategoryPropertyValueInfo> GetSplitGroupPropertyValueByProductGroupSysNo(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSplitGroupPropertyValueByProductGroupSysNo");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            return cmd.ExecuteEntityList<CategoryPropertyValueInfo>();
        }

        /// <summary>
        /// 获取商品组分组属性商品列表
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <param name="propertySysNo">属性编号</param>
        /// <param name="valueSysNo">属性值编号</param>
        /// <returns></returns>
        public static List<int> GetProductGroupSplitPropertyProductList(int productGroupSysNo, int propertySysNo, int valueSysNo)
        {
            List<int> result = new List<int>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupSplitPropertyProductList");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            cmd.SetParameterValue("@PropertySysNo", propertySysNo);
            cmd.SetParameterValue("@ValueSysNo", valueSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(int.Parse(row[0].ToString()));
                }
            }
            return result;
        }

        /// <summary>
        /// 创建同组商品
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="productID"></param>
        /// <param name="splitName">分组名称</param>
        /// <param name="manufacturerSysNo">生产商编号</param>
        /// <returns></returns>
        public static int CreateGroupProduct(ProductMaintainGroupProductInfo entity, string productID, string splitName, int manufacturerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateGroupProduct");
            cmd.SetParameterValue("@ProductGroupSysNo", entity.ProductGroupSysNo);
            cmd.SetParameterValue("@ProductID", productID);
            cmd.SetParameterValue("@SellerSysNo", entity.SellerSysNo);
            cmd.SetParameterValue("@InUserSysNo", entity.InUserSysNo);
            cmd.SetParameterValue("@SplitName", splitName);
            cmd.SetParameterValue("@ManufacturerSysNo", manufacturerSysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@ProductSysNo");
        }

        /// <summary>
        /// 根据商品组编号获取商品列表
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static List<ProductMaintainInfo> GetProductListByProductGroupSysNo(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductListByProductGroupSysNo");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            return cmd.ExecuteEntityList<ProductMaintainInfo>();
        }

        /// <summary>
        /// 获取单个商品维护信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductMaintainInfo GetSingleProductMaintainInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSingleProductMaintainInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductMaintainInfo>();
        }

        /// <summary>
        /// 更新单个商品信息
        /// </summary>
        /// <param name="entity">商品信息</param>
        /// <returns></returns>
        public static bool UpdateSingleProductMaintainInfo(ProductMaintainInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSingleProductMaintainInfo");
            cmd.SetParameterValue<ProductMaintainInfo>(entity);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 获取商家财务类型
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <returns></returns>
        public static string GetSellerInvoiceType(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetVendorInvoiceType");
            cmd.SetParameterValue("@VendorSysNo", sellerSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        #endregion

        #region 图片信息

        /// <summary>
        /// 保存商品图片信息
        /// </summary>
        /// <param name="imageInfo">商品图片信息</param>
        /// <returns></returns>
        public static bool SaveProductImageInfo(ProductImageInfo imageInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaveProductImageInfo");
            cmd.SetParameterValue("@ProductSysNo", imageInfo.ProductSysNo);
            cmd.SetParameterValue("@ResourceUrl", imageInfo.ResourceUrl);
            cmd.SetParameterValue("@InUserName", imageInfo.InUserName);
            cmd.SetParameterValue("@CompanyCode", imageInfo.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", imageInfo.LanguageCode);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 根据商品组编号查询商品图片列表
        /// </summary>
        /// <param name="productGroupSysNo">商品组编号</param>
        /// <returns></returns>
        public static List<ProductImageInfo> GetProductImageListByProductGroupSysNo(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductImageListByProductGroupSysNo");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            return cmd.ExecuteEntityList<ProductImageInfo>();
        }

        /// <summary>
        /// 更新商品图片优先级
        /// </summary>
        /// <param name="imageInfo">商品图片信息</param>
        /// <returns></returns>
        public static bool UpdateProductImagePriority(ProductImageInfo imageInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductImagePriority");
            cmd.SetParameterValue("@SysNo", imageInfo.SysNo);
            cmd.SetParameterValue("@Priority", imageInfo.Priority);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 删除指定商品图片
        /// </summary>
        /// <param name="sysNo">商品图片编号</param>
        /// <returns></returns>
        public static bool DeleteProductImageBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductImageBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 设置商品默认图片
        /// </summary>
        /// <param name="sysNo">商品图片编号</param>
        /// <returns></returns>
        public static bool SetProductDefaultImage(ProductImageInfo imageInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetProductDefaultImage");
            cmd.SetParameterValue("@SysNo", imageInfo.SysNo);
            cmd.SetParameterValue("@ProductSysNo", imageInfo.ProductSysNo);
            cmd.ExecuteNonQuery();
            return true;
        }

        #endregion

        #region 价格信息

        /// <summary>
        /// 根据商品编号获取商品
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductMaintainInfo GetProductByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductMaintainInfo>();
        }

        /// <summary>
        /// 根据商品编号获取商品价格信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductPriceInfo GetProductPriceInfoByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductPriceInfoByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductPriceInfo>();
        }

        /// <summary>
        /// 保存商品价格信息
        /// </summary>
        /// <param name="priceInfo">价格信息</param>
        /// <returns></returns>
        public static bool SaveProductPriceInfo(ProductPriceInfo priceInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaveProductPriceInfo");
            cmd.SetParameterValue<ProductPriceInfo>(priceInfo);
            cmd.ExecuteNonQuery();
            return true;
        }

        #endregion

        #region 备案信息

        /// <summary>
        /// 根据商品编号获取商品备案信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductEntryInfo GetProductEntryInfoByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductEntryInfoByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductEntryInfo>();
        }

        /// <summary>
        /// 保存商品备案信息
        /// </summary>
        /// <param name="entryInfo">备案信息</param>
        /// <returns></returns>
        public static bool SaveProductEntryInfo(ProductEntryInfo entryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaveProductEntryInfo");
            cmd.SetParameterValue<ProductEntryInfo>(entryInfo);
            cmd.ExecuteNonQuery();
            return true;
        }

        #endregion

        #region 备案信息管理

        /// <summary>
        /// 查询备案信息
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="dataCount">总记录数</param>
        /// <returns></returns>
        public static List<ProductEntryInfo> ProductEntryInfoQuery(ProductEntryInfoQueryFilter filter, out int dataCount)
        {
            List<ProductEntryInfo> result = new List<ProductEntryInfo>();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductEntryInfo");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, filter, !string.IsNullOrWhiteSpace(filter.SortFields) ? filter.SortFields : "P.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.MerchantSysNo", DbType.Int32,
                    "@SellerSysNo", QueryConditionOperatorType.Equal,
                    filter.SellerSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.SysNo", DbType.Int32,
                    "@ProductSysNo", QueryConditionOperatorType.Equal,
                    filter.ProductSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.ProductTitle", DbType.String,
                    "@ProductTitle", QueryConditionOperatorType.Like,
                    filter.ProductTitle);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Product_SKUNO", DbType.String,
                    "@ProductSKUNO", QueryConditionOperatorType.Like,
                    filter.ProductSKUNO);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Supplies_Serial_No", DbType.String,
                    "@SuppliesSerialNo", QueryConditionOperatorType.Like,
                    filter.SuppliesSerialNo);

                if (!string.IsNullOrEmpty(filter.ManufactureDateBegin))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "ManufactureDate", DbType.String,
                        "@ManufactureDateBegin", QueryConditionOperatorType.MoreThanOrEqual,
                        filter.ManufactureDateBegin);
                }
                if (!string.IsNullOrEmpty(filter.ManufactureDateEnd))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "ManufactureDate", DbType.String,
                        "@ManufactureDateEnd", QueryConditionOperatorType.LessThan,
                        DateTime.Parse(filter.ManufactureDateEnd).AddDays(1).ToShortDateString());
                }

                if (filter.EntryStatus == 0)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "ISNULL(EntryStatus, 0)", DbType.Int32,
                        "@EntryStatus1", QueryConditionOperatorType.Equal,
                        filter.EntryStatus);
                }
                else
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "EntryStatus", DbType.Int32,
                        "@EntryStatus", QueryConditionOperatorType.Equal,
                        filter.EntryStatus);
                }

                command.CommandText = builder.BuildQuerySql();
                result = command.ExecuteEntityList<ProductEntryInfo>();
                dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return result;
            }
        }

        /// <summary>
        /// 提交备案申请
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static bool SubmitProductEntryAudit(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SubmitProductEntryAudit");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.ExecuteNonQuery();
            return true;
        }

        #endregion

        #region 商品上下架，作废管理

        /// <summary>
        /// 根据商品编号获取商品上下架信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductOnSaleInfo GetProductOnSaleInfoByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductOnSaleInfoByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductOnSaleInfo>();
        }

        /// <summary>
        /// 判断商品在赠品活动中是否作为赠品存在，但是要排除已“作废”和“完成”两种状态的记录的赠品活动
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static bool ProductIsGift(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGiftProductIsGift");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            int count = cmd.ExecuteScalar<int>();
            return count > 0;
        }

        /// <summary>
        /// 更新商品状态
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public static int UpdateProductStatus(int productSysNo, ProductStatus status)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductStatus");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.SetParameterValue("@ProductStatus", status);
            int count = dc.ExecuteNonQuery();
            return count;
        }

        #endregion

        #region 商品销售区域管理

        /// <summary>
        /// 获取商品销售区域列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static QueryResult<ProductSalesAreaInfo> GetProductSalesAreaInfoBySysNo(ProductQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductSalesAreaInfoBySysNo");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "pra.SysNo ASC" : queryFilter.SortFields))
            {
                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pra.ProductSysNo", DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo.Value);
                }
                command.CommandText = sqlBuilder.BuildQuerySql();
                List<ProductSalesAreaInfo> resultList = command.ExecuteEntityList<ProductSalesAreaInfo>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<ProductSalesAreaInfo>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }
        
        /// <summary>
        /// 新增商品销售区域列表
        /// </summary>
        /// <param name="productInfo"></param>
        /// <param name="productSalesAreaInfo"></param>
        public static void InsertProductSalesArea(ProductQueryInfo productInfo, ProductSalesAreaInfo productSalesAreaInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertProductSalesArea");
            dc.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            dc.SetParameterValue("@ProductID", productInfo.ProductID);
            dc.SetParameterValue("@ProductName", productInfo.ProductTitle);
            dc.SetParameterValue("@StockSysNo", productSalesAreaInfo.Stock.SysNo);
            dc.SetParameterValue("@StockName", productSalesAreaInfo.Stock.StockName);
            dc.SetParameterValue("@ProvinceSysNo", productSalesAreaInfo.Province.ProvinceSysNo);
            dc.SetParameterValue("@ProvinceName", productSalesAreaInfo.Province.ProvinceName);
            dc.SetParameterValue("@CitySysNo", productSalesAreaInfo.Province.CitySysNo);
            dc.SetParameterValue("@CityName", productSalesAreaInfo.Province.CityName);
            dc.SetParameterValue("@InUser", productSalesAreaInfo.OperationUser.UserName);
            dc.SetParameterValue("@CompanyCode", productSalesAreaInfo.CompanyCode);
            dc.SetParameterValue("@LanguageCode", productSalesAreaInfo.LanguageCode);
            dc.ExecuteNonQuery();
        }
        
        /// <summary>
        /// 根据SysNo获取商品[ProductID]和[ProductTitle]
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductQueryInfo GetProductTitleByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductTitleInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductQueryInfo>();
        }
        
        /// <summary>
        /// 根据SysNo删除商品销售区域列表
        /// </summary>
        /// <param name="productSysNo"></param>
        public static void DeleteProductSalesArea(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteProductSalesArea");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.ExecuteNonQuery();
        }

        #endregion

        #region 商品批号管理

        /// <summary>
        /// 更新商品批号
        /// </summary>
        /// <param name="batchManagementInfo"></param>
        public static void UpdateIsBatch(ProductBatchManagementInfo batchManagementInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductBatchManagementInfo");

            //cmd.SetParameterValue(batchManagementInfo);
            cmd.SetParameterValue("@IsBatch", batchManagementInfo.IsBatch.Value ? "Y" : "N");
            cmd.SetParameterValue("@ProductSysNo", batchManagementInfo.ProductSysNo);
            cmd.SetParameterValue("@CollectType", batchManagementInfo.CollectType == CollectDateType.ExpiredDate ? "E" : "P");
            cmd.SetParameterValue("@IsCollectBatchNo", batchManagementInfo.IsCollectBatchNo.Value ? "Y" : "N");
            cmd.SetParameterValue("@MinReceiptDays", batchManagementInfo.MinReceiptDays);
            cmd.SetParameterValue("@MaxDeliveryDays", batchManagementInfo.MaxDeliveryDays);
            cmd.SetParameterValue("@GuaranteePeriodYear", batchManagementInfo.GuaranteePeriodYear);
            cmd.SetParameterValue("@GuaranteePeriodMonth", batchManagementInfo.GuaranteePeriodMonth);
            cmd.SetParameterValue("@GuaranteePeriodDay", batchManagementInfo.GuaranteePeriodDay);
            cmd.SetParameterValue("@EidtUser", batchManagementInfo.EidtUser);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据商品SysNo获取商品批号和历史备注
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductBatchManagementInfo GetProductBatchManagementInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductBatchManagementInfo");

            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            var result = cmd.ExecuteEntity<ProductBatchManagementInfo>();
            if (result != null)
            {
                result.Logs = GetProductBatchManagementLogByBatchManagementSysNo(result.SysNo.Value);
                foreach (var item in result.Logs)
                {
                    result.Notes += item.InDate.ToString() + "," + item.InUser + "," + item.Note;
                }
            }
            else
            {
                result = new ProductBatchManagementInfo();
                result.ProductSysNo = productSysNo;
                result.IsBatch = false;
                result.IsCollectBatchNo = false;
                result.CollectType = 0;
            }
            return result;
        }

        /// <summary>
        /// 新增历史备注
        /// </summary>
        /// <param name="entity"></param>
        public static void InsertProductBatchManagementLog(ProductBatchManagementInfoLog entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertProductBatchManagementLog");
            command.SetParameterValue("@BatchManagementSysNo", entity.BatchManagementSysNo);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@InUser", entity.InUser);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据商品批号编号获得历史备注
        /// </summary>
        /// <param name="batchManagementSysNo"></param>
        /// <returns></returns>
        public static List<ProductBatchManagementInfoLog> GetProductBatchManagementLogByBatchManagementSysNo(int batchManagementSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductBatchManagementLogByBatchManagementSysNo");
            cmd.SetParameterValue("@BatchManagementSysNo", batchManagementSysNo);
            return cmd.ExecuteEntityList<ProductBatchManagementInfoLog>();
        }

        #endregion
    }
}
