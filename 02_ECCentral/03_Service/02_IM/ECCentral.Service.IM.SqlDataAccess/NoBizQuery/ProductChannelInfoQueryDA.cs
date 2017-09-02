//************************************************************************
// 用户名				泰隆优选
// 系统名				类别延保管理
// 子系统名		        类别延保管理NoBizQuery查询接口实现
// 作成者				Kevin
// 改版日				2012.6.5
// 改版内容				新建
//************************************************************************

using System;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductChannelInfoQueryDA))]
    class ProductChannelInfoQueryDA : IProductChannelInfoQueryDA
    {
        public DataTable QueryProductChannelInfo(ProductChannelInfoQueryFilter queryCriteria, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductChannelInfo");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "CP.SysNo DESC"))
            {
                if (queryCriteria.ChannelSysNo>0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CM.Sysno",
                        DbType.Int32, "@ChannelSysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ChannelSysNo);
                }

                if (queryCriteria.C1SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C1.Sysno",
                        DbType.Int32, "@C1SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C1SysNo.Value);
                }

                if (queryCriteria.C2SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C2.Sysno",
                        DbType.Int32, "@C2SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C2SysNo.Value);
                }

                if (queryCriteria.C3SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C3.Sysno",
                        DbType.Int32, "@C3SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C3SysNo.Value);
                }

                if (queryCriteria.ProductSysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.SysNo",
                        DbType.Int32, "@ProductSysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ProductSysNo.Value);
                }

                if (!string.IsNullOrEmpty(queryCriteria.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.ProductID",
                        DbType.String, "@ProductID",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ProductID);
                }

                if (!string.IsNullOrEmpty(queryCriteria.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "CP.Status",
                            DbType.String, "@Status",
                            QueryConditionOperatorType.Equal,
                            queryCriteria.Status);
                }

                //渠道商品ID
                if (!string.IsNullOrEmpty(queryCriteria.ChannelProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CP.SynProductID",
                        DbType.String, "@ChannelProductID",
                        QueryConditionOperatorType.Like,
                        queryCriteria.ChannelProductID);
                }
                //taobaoSKU
                if (!string.IsNullOrEmpty(queryCriteria.TaobaoSKU))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CP.TaoBaoSku",
                        DbType.String, "@TaobaoSKU",
                        QueryConditionOperatorType.Like,
                        queryCriteria.TaobaoSKU);
                }


                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(ProductChannelInfoStatus) }
                                                  , { "IsAppointInventory", typeof(BooleanEnum) }
                                                  , { "IsUsePromotionPrice", typeof(BooleanEnum) }};

                DataTable dt = dataCommand.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }

        }

        public DataTable QueryProductChannelPeriodPrice(ProductChannelPeriodPriceQueryFilter queryCriteria, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductChannelPeriodPrice");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "SysNo DESC"))
            {
                if (queryCriteria.ChannelProductSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "ChannelProductInfoSysNo",
                        DbType.Int32, "@ChannelProductSysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ChannelProductSysNo);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Status",
                    DbType.String, "@Status",
                    QueryConditionOperatorType.NotEqual,
                    ProductChannelPeriodPriceStatus.Abandon);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = dataCommand.ExecuteDataTable("Status", typeof(ProductChannelPeriodPriceStatus));

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }

        }
    }
}
