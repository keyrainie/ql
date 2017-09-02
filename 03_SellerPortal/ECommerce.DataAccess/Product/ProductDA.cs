using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Product
{
    public class ProductDA
    {
        /// <summary>
        /// 分页查询商品列表Demo
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ProductInfo> QueryProductList(ProductListQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "p.SysNo ASC" : queryFilter.SortFields))
            {
                //Set SQL WHERE Condition:
                #region Build Condition:

                if (!string.IsNullOrEmpty(queryFilter.SysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.SysNo);
                }
                if (!string.IsNullOrEmpty(queryFilter.StartDateString))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.InDate", DbType.DateTime, "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual, Convert.ToDateTime(queryFilter.StartDateString));
                }
                if (!string.IsNullOrEmpty(queryFilter.EndDateString))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.InDate", DbType.DateTime, "@InDateTo", QueryConditionOperatorType.LessThan, Convert.ToDateTime(queryFilter.EndDateString).AddDays(1));
                }
                if (!string.IsNullOrEmpty(queryFilter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.Status", DbType.StringFixedLength, "@Status", QueryConditionOperatorType.Equal, queryFilter.Status);
                }

                #endregion

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<ProductInfo> resultList = command.ExecuteEntityList<ProductInfo>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<ProductInfo>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }


        }

        /// <summary>
        /// 分页查询商品
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ProductCommonInfo> QueryCommonProduct(ProducCommonQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCommonProduct");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "P.[SysNo] ASC" : queryFilter.SortFields))
            {
                if (!string.IsNullOrEmpty(queryFilter.VendorSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.[SysNo]", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                }
                if (!string.IsNullOrEmpty(queryFilter.BrandSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[BrandSysNo]", DbType.Int32, "@BrandSysNo", QueryConditionOperatorType.Equal, queryFilter.BrandSysNo);
                }
                if (!string.IsNullOrEmpty(queryFilter.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductID]", DbType.String, "@ProductID", QueryConditionOperatorType.Like, queryFilter.ProductID);
                }
                if (!string.IsNullOrEmpty(queryFilter.ProductTitle))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductTitle]", DbType.String, "@ProductTitle", QueryConditionOperatorType.Like, queryFilter.ProductTitle);
                }

                if (!string.IsNullOrEmpty(queryFilter.FrontCategorySysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PE.[FrontCategorySysNo]", DbType.Int32
, "@FrontCategorySysNo", QueryConditionOperatorType.Like, Convert.ToInt32(queryFilter.FrontCategorySysNo));
                }

                if (!string.IsNullOrEmpty(queryFilter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[Status]", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, queryFilter.Status);
                }
                if (queryFilter.CreateTimeBegin != null && queryFilter.CreateTimeBegin != new DateTime())
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[CreateTime]", DbType.DateTime, "@CreateTimeBegin", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateTimeBegin.ToString("yyyy-MM-dd"));
                }
                if (queryFilter.CreateTimeEnd != null && queryFilter.CreateTimeEnd != new DateTime())
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[CreateTime]", DbType.DateTime, "@CreateTimeEnd", QueryConditionOperatorType.LessThan, queryFilter.CreateTimeEnd.AddDays(1).ToString("yyyy-MM-dd"));
                }
                if (!string.IsNullOrEmpty(queryFilter.GroupName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PG.[ProductName]", DbType.String, "@GroupName", QueryConditionOperatorType.Like, queryFilter.GroupName);
                }
                if (!string.IsNullOrEmpty(queryFilter.ProductTradeType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PE.[ProductTradeType]", DbType.String, "@ProductTradeType", QueryConditionOperatorType.Equal, queryFilter.ProductTradeType);
                }

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<ProductCommonInfo> resultList = command.ExecuteEntityList<ProductCommonInfo>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<ProductCommonInfo>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }

        /// <summary>
        /// 根据供应商编号查询品牌
        /// </summary>
        /// <returns></returns>
        public static List<BrandInfo> GetBrandByVendorSysNo(int vendorSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandByVendorSysNo");
            cmd.SetParameterValue("@VendorSysNo", vendorSysNo);
            return cmd.ExecuteEntityList<BrandInfo>();
        }

        /// <summary>
        /// 根据品牌编号查询品牌
        /// </summary>
        /// <param name="brandSysNo">品牌编号</param>
        /// <returns></returns>
        public static BrandInfo GetBrandInfoBySysNo(int brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandInfoBySysNo");
            cmd.SetParameterValue("@SysNo", brandSysNo);
            var sourceEntity = cmd.ExecuteEntity<BrandInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 分页查询商品
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ProductQueryInfo> QueryProduct(ProductQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProduct");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "P.[SysNo] DESC" : queryFilter.SortFields))
            {
                if (!string.IsNullOrEmpty(queryFilter.VendorSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.MerchantSysNo", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                }
                if (queryFilter.ProductSysNo.HasValue && queryFilter.ProductSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "p.[SysNo]"
                        , DbType.Int32
                        , "@ProductSysNo"
                        , QueryConditionOperatorType.Equal
                        , queryFilter.ProductSysNo
                        );
                }
                if (!string.IsNullOrEmpty(queryFilter.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.[ProductID]", DbType.String, "@ProductID", QueryConditionOperatorType.Like, queryFilter.ProductID);
                }
                if (!string.IsNullOrEmpty(queryFilter.ProductTitle))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductTitle]", DbType.String, "@ProductTitle", QueryConditionOperatorType.Like, queryFilter.ProductTitle);
                }
                if (!string.IsNullOrEmpty(queryFilter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[Status]", DbType.Int32, "@Status", queryFilter.StatusCondition == 0 ? QueryConditionOperatorType.Equal : QueryConditionOperatorType.NotEqual, queryFilter.Status);
                }
                if (!string.IsNullOrEmpty(queryFilter.CategorySysNo))
                {
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "FPC.[SysNo]", QueryConditionOperatorType.In, queryFilter.CategorySysNo);
                }
                if (queryFilter.CreateTimeBegin != null && queryFilter.CreateTimeBegin != new DateTime())
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[CreateTime]", DbType.DateTime, "@CreateTimeBegin", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateTimeBegin.ToString("yyyy-MM-dd"));
                }
                if (queryFilter.CreateTimeEnd != null && queryFilter.CreateTimeEnd != new DateTime())
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[CreateTime]", DbType.DateTime, "@CreateTimeEnd", QueryConditionOperatorType.LessThan, queryFilter.CreateTimeEnd.AddDays(1).ToString("yyyy-MM-dd"));
                }
                if (!string.IsNullOrEmpty(queryFilter.ProductTradeType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PE.[ProductTradeType]", DbType.String, "@ProductTradeType", QueryConditionOperatorType.Equal, queryFilter.ProductTradeType);
                }
                if (!string.IsNullOrEmpty(queryFilter.UPCCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                        , "PC.[UPCCode]"
                        , DbType.String
                        , "@UPCCode"
                        , QueryConditionOperatorType.Like, queryFilter.UPCCode
                        );
                }
                string sqlstr = sqlBuilder.BuildQuerySql();
                command.CommandText = sqlstr;

                List<ProductQueryInfo> resultList = command.ExecuteEntityList<ProductQueryInfo>();

                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<ProductQueryInfo>()
                {
                    PageInfo = new PageInfo()
                    {
                        PageIndex = queryFilter.PageIndex,
                        PageSize = queryFilter.PageSize,
                        TotalCount = totalCount,
                        SortBy = queryFilter.SortFields
                    },
                    ResultList = resultList
                };
            }
        }

        /// <summary>
        /// 查询商家前台类别
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public static List<FrontProductCategoryInfo> GetFrontProductCategory(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFrontProductCategory");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteEntityList<FrontProductCategoryInfo>();
        }

        /// <summary>
        /// 修改前台类别
        /// </summary>
        /// <param name="info"></param>
        public static FrontProductCategoryInfo UpdateFrontProductCategory(FrontProductCategoryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateFrontProductCategory");
            cmd.SetParameterValue("@CategoryCode", info.CategoryCode);
            cmd.SetParameterValue("@CategoryName", info.CategoryName);
            cmd.SetParameterValue("@UIModeType", info.UIModeType);
            cmd.SetParameterValue("@FPCLinkUrlMode", info.FPCLinkUrlMode);
            cmd.SetParameterValue("@FPCLinkUrl", info.FPCLinkUrl);
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@EditUserSysNo", info.EditUserSysNo);
            cmd.SetParameterValue("@EditUserName", info.EditUserName);
            cmd.SetParameterValue("@SellerSysNo", info.SellerSysNo);
            cmd.ExecuteNonQuery();

            info.SysNo = int.Parse(cmd.GetParameterValue("@SysNo").ToString());
            info.Status = (CommonStatus)int.Parse(cmd.GetParameterValue("@StatusOutput").ToString());

            return info;
        }

        /// <summary>
        /// 添加前台类别
        /// </summary>
        /// <param name="info"></param>
        public static FrontProductCategoryInfo InsertFrontProductCategory(FrontProductCategoryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertFrontProductCategory");
            cmd.SetParameterValue("@CategoryName", info.CategoryName);
            cmd.SetParameterValue("@ParentCategoryCode", info.ParentCategoryCode);
            cmd.SetParameterValue("@IsLeaf", info.IsLeaf);
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@FPCLinkUrlMode", info.FPCLinkUrlMode);
            cmd.SetParameterValue("@FPCLinkUrl", info.FPCLinkUrl);
            cmd.SetParameterValue("@UIModeType", info.UIModeType);
            cmd.SetParameterValue("@SellerSysNo", info.SellerSysNo);
            cmd.SetParameterValue("@InUserSysNo", info.InUserSysNo);
            cmd.SetParameterValue("@InUserName", info.InUserName);

            int count = cmd.ExecuteNonQuery();
            if (count > 0)
            {
                info.CategoryCode = cmd.GetParameterValue("@CategoryCode").ToString();
                info.SysNo = int.Parse(cmd.GetParameterValue("@SysNo").ToString());
            }

            return info;
        }

        #region [商品库存调整单 相关]
        /// <summary>
        /// 查询商品库存调整单列表
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        public static QueryResult<ProductStockAdjustViewInfo> QueryProductStockAdjustmentList(ProductStockAdjustListQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductStockAdjustmentList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "ps.SysNo DESC" : queryFilter.SortFields))
            {
                //Set SQL WHERE Condition:
                #region Build Condition:

                if (queryFilter.VendorSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ps.VendorSysNo", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo.Value);
                }

                var sysno = 0;
                if (!string.IsNullOrEmpty(queryFilter.SysNo) && int.TryParse(queryFilter.SysNo, out sysno))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ps.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, sysno);
                }
                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ps.Status", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, (int)queryFilter.Status.Value);
                }

                if (queryFilter.StockSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ps.StockSysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo.Value);
                }
                int productSysNo = 0;
                if (!string.IsNullOrEmpty(queryFilter.ProductSysNo) && int.TryParse(queryFilter.ProductSysNo, out productSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(" Exists(SELECT TOP 1 1 FROM ECStore.dbo.Product_StockAdjustDetail (NOLOCK) WHERE ProductSysNo = {0} AND AdjustSysNo = ps.SysNo)", productSysNo));
                }

                if (!string.IsNullOrEmpty(queryFilter.CreateDateFrom))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ps.InDate", DbType.DateTime, "@DateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, Convert.ToDateTime(queryFilter.CreateDateFrom));
                }
                if (!string.IsNullOrEmpty(queryFilter.CreateDateTo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ps.InDate", DbType.DateTime, "@DateTimeTo", QueryConditionOperatorType.LessThan, Convert.ToDateTime(queryFilter.CreateDateTo).AddDays(1));
                }
                if (!string.IsNullOrEmpty(queryFilter.AuditDateFrom))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ps.AuditDate", DbType.DateTime, "@AuditTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, Convert.ToDateTime(queryFilter.AuditDateFrom));
                }
                if (!string.IsNullOrEmpty(queryFilter.AuditDateTo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ps.AuditDate", DbType.DateTime, "@AuditTimeTo", QueryConditionOperatorType.LessThan, Convert.ToDateTime(queryFilter.AuditDateTo).AddDays(1));
                }
                #endregion

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<ProductStockAdjustViewInfo> resultList = command.ExecuteEntityList<ProductStockAdjustViewInfo>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<ProductStockAdjustViewInfo>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }

        public static ProductStockAdjustInfo GetProductStockAdjustmentInfo(int adjustSysNo)
        {
            ProductStockAdjustInfo adjustInfo = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductStockAdjustmentInfo");
            cmd.SetParameterValue("@SysNo", adjustSysNo);
            var sourceEntity = cmd.ExecuteDataSet();
            if (null != sourceEntity && sourceEntity.Tables.Count > 0)
            {
                if (sourceEntity.Tables.Count >= 1)
                {
                    DataTable mainInfoDt = sourceEntity.Tables[0];
                    if (mainInfoDt.Rows.Count > 0)
                    {
                        adjustInfo = new ProductStockAdjustInfo();
                        adjustInfo = DataMapper.GetEntity<ProductStockAdjustInfo>(mainInfoDt.Rows[0]);
                    }
                }
                if (sourceEntity.Tables.Count >= 2)
                {
                    DataTable itemsInfoDt = sourceEntity.Tables[1];
                    if (itemsInfoDt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in itemsInfoDt.Rows)
                        {
                            adjustInfo.AdjustItemList.Add(DataMapper.GetEntity<ProductStockAdjustItemInfo>(dr));
                        }
                    }
                }
            }
            return adjustInfo;
        }


        public static int UpdateProductStockAdjustmentStatus(ProductStockAdjustStatus newStatus, int adjustSysNo, int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductStockAdjustmentStatus");
            cmd.SetParameterValue("@SysNo", adjustSysNo);
            cmd.SetParameterValue("@Status", (int)newStatus);
            cmd.SetParameterValue("@EditUserSysNo", userSysNo);
            return cmd.ExecuteNonQuery();
        }


        public static int UpdateProductStockAdjustmentAuditDate(int adjustSysNo, int auditUserSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductStockAdjustmentAuditDate");
            cmd.SetParameterValue("@AuditUserSysNo", auditUserSysNo);
            cmd.SetParameterValue("@SysNo", adjustSysNo);
            return cmd.ExecuteNonQuery();
        }

        public static int SaveProductStockAdjustmentInfo(ProductStockAdjustInfo adjustInfo)
        {
            if (adjustInfo.SysNo.HasValue)
            {
                //编辑:
                DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductStockAdjustmentInfo");
                cmd.SetParameterValue("@SysNo", adjustInfo.SysNo.Value);
                cmd.SetParameterValue("@StockSysNo", adjustInfo.StockSysNo.Value);
                cmd.SetParameterValue("@CurrencyCode", adjustInfo.CurrencyCode.Value);
                cmd.SetParameterValue("@Memo", adjustInfo.Memo);
                cmd.SetParameterValue("@EditUserSysNo", adjustInfo.EditUserSysNo.Value);
                cmd.ExecuteNonQuery();


                DataCommand deleteCmd = DataCommandManager.GetDataCommand("DeleteAllProductStockAdjustmentItemInfo");
                deleteCmd.SetParameterValue("@AdjustSysNo", adjustInfo.SysNo.Value);
                deleteCmd.ExecuteNonQuery();

                foreach (var item in adjustInfo.AdjustItemList)
                {
                    DataCommand itemCmd = DataCommandManager.GetDataCommand("InsertProductStockAdjustmentItemInfo");
                    itemCmd.SetParameterValue("@AdjustSysNo", adjustInfo.SysNo.Value);
                    itemCmd.SetParameterValue("@ProductSysNo", item.ProductSysNo.Value);
                    itemCmd.SetParameterValue("@AdjustQty", item.AdjustQty.Value);
                    itemCmd.SetParameterValue("@InUserSysNo", item.InUserSysNo.Value);
                    itemCmd.ExecuteNonQuery();
                }
            }
            else
            {
                //新建:
                DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductStockAdjustmentInfo");
                cmd.SetParameterValue("@Status", (int)adjustInfo.Status);
                cmd.SetParameterValue("@CurrencyCode", adjustInfo.CurrencyCode.Value);
                cmd.SetParameterValue("@StockSysNo", adjustInfo.StockSysNo.Value);
                cmd.SetParameterValue("@Memo", adjustInfo.Memo);
                cmd.SetParameterValue("@VendorSysNo", adjustInfo.VendorSysNo.Value);
                cmd.SetParameterValue("@InUserSysNo", adjustInfo.InUserSysNo.Value);
                cmd.ExecuteNonQuery();
                adjustInfo.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));

                foreach (var item in adjustInfo.AdjustItemList)
                {
                    item.InUserSysNo = adjustInfo.InUserSysNo;
                    DataCommand itemCmd = DataCommandManager.GetDataCommand("InsertProductStockAdjustmentItemInfo");
                    itemCmd.SetParameterValue("@AdjustSysNo", adjustInfo.SysNo.Value);
                    itemCmd.SetParameterValue("@ProductSysNo", item.ProductSysNo.Value);
                    itemCmd.SetParameterValue("@AdjustQty", item.AdjustQty.Value);
                    itemCmd.SetParameterValue("@InUserSysNo", item.InUserSysNo.Value);
                    itemCmd.ExecuteNonQuery();
                }

            }
            return adjustInfo.SysNo.Value;
        }
        #endregion

        /// <summary>
        /// 添加类别Root
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static FrontProductCategoryInfo InsertFrontProductCategoryRoot(FrontProductCategoryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertFrontProductCategoryRoot");
            cmd.SetParameterValue("@CategoryName", info.CategoryName);
            cmd.SetParameterValue("@IsLeaf", info.IsLeaf);
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@FPCLinkUrlMode", info.FPCLinkUrlMode);
            cmd.SetParameterValue("@FPCLinkUrl", info.FPCLinkUrl);
            cmd.SetParameterValue("@UIModeType", info.UIModeType);
            cmd.SetParameterValue("@SellerSysNo", info.SellerSysNo);
            cmd.SetParameterValue("@InUserSysNo", info.InUserSysNo);
            cmd.SetParameterValue("@InUserName", info.InUserName);

            int count = cmd.ExecuteNonQuery();
            if (count > 0)
            {
                info.CategoryCode = cmd.GetParameterValue("@CategoryCode").ToString();
                info.SysNo = int.Parse(cmd.GetParameterValue("@SysNo").ToString());
            }

            return info;
        }


        #region [WMS API接口调用相关方法:]
        public static int SaveProductInspectionInfo(ProductInspectionInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaveProductCustomsMergeInfo");
            cmd.SetParameterValue<ProductInspectionInfo>(info);
            return cmd.ExecuteNonQuery();
        }

        public static bool ExistsProductInspectionInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ExistsProductCustomsMergeInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteScalar() == null ? false : true;
        }
        #endregion
    }
}
