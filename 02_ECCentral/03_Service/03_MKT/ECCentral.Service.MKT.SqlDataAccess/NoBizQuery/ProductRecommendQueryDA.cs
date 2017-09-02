using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductRecommendQueryDA))]
    public class ProductRecommendQueryDA : IProductRecommendQueryDA
    {
        public DataTable Query(ProductRecommendQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductRecommend_Query");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "result.[SysNo] DESC"))
            {
                //商品ID
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "IC.ProductID",
                    DbType.String,
                    "@ProductID",
                    QueryConditionOperatorType.Equal,
                    filter.ProductID);

                //商品系统编号
                sqlBuilder.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "IC.SysNo",
                 DbType.Int32,
                 "@SysNo",
                 QueryConditionOperatorType.Equal,
                 filter.ProductSysNo);
                if (filter.ProductStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "IC.Status",
                       DbType.Int32,
                       "@ProductStatus",
                       QueryConditionOperatorType.Equal,
                       filter.ProductStatus.Value);
                }

                //状态
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "A.Status",
                  DbType.AnsiStringFixedLength,
                  "@AStatus",
                  QueryConditionOperatorType.Equal,
                  filter.Status);

                #region PageType,PageID,PositionID相关

                //位置编号
                sqlBuilder.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "B.PositionID",
                         DbType.Int32,
                         "@PositionID",
                         QueryConditionOperatorType.Equal,
                         filter.PositionID);

                if (filter.PageType.HasValue)
                {
                    //页面类型
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "B.PageType",
                        DbType.Int32,
                        "@PageType",
                        QueryConditionOperatorType.Equal,
                        filter.PageType);

                    PageTypePresentationType pType = PageTypeUtil.ResolvePresentationType(ModuleType.ProductRecommend, filter.PageType.Value.ToString());
                    if (pType == PageTypePresentationType.Merchant && filter.PageID == 1)
                    {
                        //特殊商家
                        sqlBuilder.ConditionConstructor.AddCondition(
                      QueryConditionRelationType.AND,
                      "V.VendorType",
                      DbType.Int32,
                      "@VendorType",
                      QueryConditionOperatorType.Equal,
                      0);
                    }
                    else if (pType == PageTypePresentationType.Category1)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                                 QueryConditionRelationType.AND,
                                 "C3.[C1Sysno]",
                              DbType.Int32,
                              "@C1Sysno",
                              QueryConditionOperatorType.Equal,
                              filter.PageID);
                    }
                    else if (pType == PageTypePresentationType.Category2)
                    {

                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "C3.[C2Sysno]",
                            DbType.Int32,
                            "@C2Sysno",
                            QueryConditionOperatorType.Equal,
                            filter.PageID);
                    }
                    else if (pType == PageTypePresentationType.Category3)
                    {

                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                          "C3.[C3SysNo]",
                            DbType.Int32,
                          "@C3Sysno",
                            QueryConditionOperatorType.Equal,
                            filter.PageID);
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                          QueryConditionRelationType.AND,
                          "B.PageID",
                          DbType.Int32,
                          "@PageID",
                          QueryConditionOperatorType.Equal,
                          filter.PageID);
                    }
                }
                #endregion

                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "A.BeginDate",
                  DbType.DateTime,
                  "@BeginDateFrom",
                  QueryConditionOperatorType.MoreThanOrEqual,
                  filter.BeginDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "A.BeginDate",
                   DbType.DateTime,
                   "@BeginDateTo",
                   QueryConditionOperatorType.LessThan,
                   filter.BeginDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "A.EndDate",
                    DbType.DateTime,
                    "@EndDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.EndDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "A.EndDate",
                  DbType.DateTime,
                  "@EndDateTo",
                  QueryConditionOperatorType.LessThan,
                  filter.EndDateTo);


                sqlBuilder.ConditionConstructor.AddCondition(
              QueryConditionRelationType.AND,
              "B.Status",
              DbType.String,
              "@BStatus",
              QueryConditionOperatorType.Equal,
              "A");
                //TODO:添加渠道过滤条件

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();
                //转换DataTable的枚举列
                EnumColumnList enumConfig = new EnumColumnList();
                enumConfig.Add("Status", typeof(ADStatus));
                enumConfig.Add("ProductStatus", typeof(ProductStatus));
                cmd.ConvertEnumColumn(ds.Tables[0], enumConfig);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds.Tables[0];
            }
        }
    }
}
