using System;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductQueryDA))]
    public partial class ProductQueryDA : IProductQueryDA
    {

        private PagingInfoEntity ToPagingInfo(PagingInfo pagingInfo)
        {
            if (pagingInfo == null)
            {
                pagingInfo = new PagingInfo();
                pagingInfo.PageIndex = 0;
                pagingInfo.PageSize = 10;
            }

            return new PagingInfoEntity()
            {
                SortField = pagingInfo.SortBy,
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryProduct(ProductQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProduct");

            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "Product.SysNo Desc"))
            {
                #region AddParameter

                //商品ID
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "Product.ProductID",
                 DbType.String,
                 "@ProductID",
                 QueryConditionOperatorType.LeftLike,
                 StringUtility.IsEmpty(filter.ProductID) ? filter.ProductID : filter.ProductID.Trim()
                 );

                //商品系统编号
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "Product.SysNo",
                 DbType.Int32,
                 "@ProductSysNo",
                 QueryConditionOperatorType.Equal,
                 filter.ProductSysNo
                 );

                //商品名称
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "Product.ProductTitle",
                 DbType.String,
                 "@ProductName",
                 QueryConditionOperatorType.Like,
                 filter.ProductName
                 );

                if (filter.VendorSysNo.HasValue)
                {
                    // 供应商系统编号
                    //                    string template = string.Format(@"SELECT DISTINCT PO_ITM.ProductSysNo
                    //                                        FROM [IPP3].[dbo].[PO_Item] PO_ITM WITH(NOLOCK)
                    //                                        INNER JOIN [IPP3].[dbo].[PO_Master] PO_MST WITH(NOLOCK)
                    //                                        ON PO_MST.SysNo = PO_ITM.POSysNo WHERE PO_MST.VendorSysNo={0}", filter.VendorSysNo.Value);
                    //                    sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "Product.SysNo",
                    //                        QueryConditionOperatorType.In, template);

                    sb.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "Product.MerchantSysNo",
                         DbType.Int32,
                         "@MerchantSysNo",
                         QueryConditionOperatorType.Equal,
                         filter.VendorSysNo
                         );
                }

                //PM操作人员
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "Product.PMUserSysNo",
                 DbType.Int32,
                 "@PMUserSysNo",
                 QueryConditionOperatorType.Equal,
                 filter.PMUserSysNo
                 );

                //商品类型
                if (filter.ProductType.HasValue)
                {
                    //需要转变映射关系
                    sb.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "Product.ProductType",
                     DbType.Int32,
                     "@ProductType",
                     QueryConditionOperatorType.Equal,
                     (int)filter.ProductType.Value
                     );
                }

                //商品状态
                if (filter.ProductStatus.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "Product.Status",
                     DbType.Int32,
                     "@Status",
                     QueryConditionOperatorType.Equal,
                     (int)filter.ProductStatus.Value
                     );
                }

                //商品状态不为作废状态
                if (filter.IsNotAbandon.HasValue && filter.IsNotAbandon.Value)
                {
                    sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "Product.Status",
                    DbType.Int32,
                    "@IsNotAbandon",
                    QueryConditionOperatorType.NotEqual,
                    ProductStatus.Abandon
                    );
                }

                //品牌
                sb.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "Product.BrandSysNo",
                     DbType.Int32,
                     "@BrandSysNo",
                     QueryConditionOperatorType.Equal,
                     filter.BrandSysNo
                     );

                //创建时间
                sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "Product.[CreateTime]",
                DbType.DateTime,
                "@CreateDateFrom",
                QueryConditionOperatorType.MoreThanOrEqual,
                filter.CreateDateFrom
                );

                sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "Product.[CreateTime]",
                DbType.DateTime,
                "@CreateDateTo",
                QueryConditionOperatorType.LessThanOrEqual,
                filter.CreateDateTo
                );

                //第三级分类
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "Product.C3SysNo",
                 DbType.Int32,
                 "@C3SysNo",
                 QueryConditionOperatorType.Equal,
                 filter.C3SysNo
                 );

                sb.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "C3.C2SysNo",
               DbType.Int32,
               "@C2SysNo",
               QueryConditionOperatorType.Equal,
               filter.C2SysNo
               );

                sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "C2.C1SysNo",
                DbType.Int32,
                "@C1SysNo",
                QueryConditionOperatorType.Equal,
                filter.C1SysNo
                );

                //仓库查询后续提供
                //sb.ConditionConstructor.AddCondition(
                // QueryConditionRelationType.AND,
                // "Inventory.WareHouseSysNumber",
                // DbType.Int32,
                // "@StockSysNo",
                // QueryConditionOperatorType.Equal,
                // filter.StockSysNo
                // );

                //频道？

                //是否代销
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "Product.IsConsign",
                 DbType.String,
                 "@IsConsign",
                 QueryConditionOperatorType.Equal,
                 filter.IsConsign
                 );

                //库存数量
                if (filter.OnlineQty.HasValue)
                {
                    QueryConditionOperatorType operatorType = QueryConditionOperatorType.Equal;
                    switch (filter.OnlineCondition.ToLower())
                    {
                        case "greater":
                            operatorType = QueryConditionOperatorType.MoreThan;
                            break;
                        case "less":
                            operatorType = QueryConditionOperatorType.LessThan;
                            break;
                        default:
                            operatorType = QueryConditionOperatorType.Equal;
                            break;
                    }

                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "Inventory.OnlineQty",
                        DbType.Int32,
                        "@OnlineQty",
                        operatorType,
                        filter.OnlineQty
                    );
                }

                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "EX.CompanyProduct",
                    DbType.Int32,
                    "@CompanyProduct",
                    QueryConditionOperatorType.Equal,
                    filter.AZCustomer
                );

                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "Product.MerchantSysNo",
                    DbType.Int32,
                    "@MerchantSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.MerchantSysNo
                );

                //公司编码
                sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "Product.CompanyCode",
                 DbType.String,
                 "@CompanyCode",
                 QueryConditionOperatorType.Equal,
                 filter.CompanyCode
                 );

                if (filter.ProductIds != null && filter.ProductIds.Count > 0)
                {
                    sb.ConditionConstructor.AddInCondition<string>(
                     QueryConditionRelationType.AND,
                     "Product.ProductID",
                     DbType.String,
                     filter.ProductIds
                     );
                }

                cmd.CommandText = sb.BuildQuerySql();

                #endregion
                EnumColumnList columnConfig = new EnumColumnList();
                columnConfig.Add("Status", typeof(ProductStatus));
                columnConfig.Add("ProductType", typeof(ProductType));
                columnConfig.Add("InventoryType", typeof(ProductInventoryType));
                var dt = cmd.ExecuteDataTable(columnConfig);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }


    }
}
