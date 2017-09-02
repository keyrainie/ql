using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ITariffInfoQueryDA))]
    public class TariffQueryDA : ITariffInfoQueryDA
    {
        /// <summary>
        /// 查询税率规则
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryTariffInfo(TariffInfoQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryTariffInfo");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo desc"))
            {

                //系统编号
                if (queryCriteria.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "SysNo",
                        DbType.Int32,
                        "@SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.SysNo.Value);
                }

                if (queryCriteria.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                          QueryConditionRelationType.AND,
                                          "Status",
                                          DbType.Int32,
                                          "@Status",
                                          QueryConditionOperatorType.Equal,
                                          queryCriteria.Status.Value);
                }

                if (!string.IsNullOrEmpty(queryCriteria.ItemCategoryName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                         QueryConditionRelationType.AND,
                                         "ItemCategoryName",
                                         DbType.String,
                                         "@ItemCategoryName",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.ItemCategoryName);
                }

                if (!string.IsNullOrEmpty(queryCriteria.TariffCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                         QueryConditionRelationType.AND,
                                         "TariffCode",
                                         DbType.String,
                                         "@TariffCode",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.TariffCode);
                }

                if (!string.IsNullOrEmpty(queryCriteria.TariffRate))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                         QueryConditionRelationType.AND,
                                         "TariffRate",
                                         DbType.String,
                                         "@TariffRate",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.TariffRate);
                }

                if (queryCriteria.ParentSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                         QueryConditionRelationType.AND,
                                         "ParentSysNo",
                                         DbType.Int32,
                                         "@ParentSysNo",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.ParentSysNo.Value);
                }

                if (queryCriteria.SearchCode.HasValue)
                {
                    if (string.IsNullOrEmpty(queryCriteria.TariffCode) && string.IsNullOrEmpty(queryCriteria.ItemCategoryName)
                        && !queryCriteria.ParentSysNo.HasValue)
                    {
                        cmd.CommandText = sqlBuilder.BuildQuerySql().Replace("#RepalceWhere#", "where TariffPrice>0");
                    }
                    else
                    {
                        cmd.CommandText = sqlBuilder.BuildQuerySql().Replace("#RepalceWhere#", "and TariffPrice>0");
                    }
                }
                else
                {
                    cmd.CommandText = sqlBuilder.BuildQuerySql().Replace("#RepalceWhere#", "");
                }




                EnumColumnList enumConfig = new EnumColumnList();
                enumConfig.Add("Status", typeof(TariffStatus));
                DataTable dt = cmd.ExecuteDataTable(enumConfig);
                totalCount = int.Parse(cmd.GetParameterValue("TotalCount").ToString());

                return dt;
            }
        }
    }
}
