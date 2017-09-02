using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IAmbassadorNewsQueryDA))]
    public class AmbassadorNewsQueryDA : IAmbassadorNewsQueryDA
    {
        #region IAmbassadorNewsQueryDA Members

        public System.Data.DataSet Query(QueryFilter.MKT.AmbassadorNewsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetAmbassadorNewsList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.NewsType", DbType.String, "@NewsType",
                    QueryConditionOperatorType.Equal, 12);

                cmd.AddInputParameter("@SysNo", DbType.Int32, filter.SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Title", DbType.String, "@Title",
                    QueryConditionOperatorType.LeftLike, filter.Title);


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CreateDate", DbType.DateTime, "@CreateDateTo",
                     QueryConditionOperatorType.LessThanOrEqual,
                     filter.InDateFromTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CreateDate", DbType.DateTime, "@CreateDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.InDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ReferenceSysNo", DbType.String, "@ReferenceSysNo",
                    QueryConditionOperatorType.Equal, filter.ReferenceSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CompanyCode", DbType.String, "@CompanyCode",
                    QueryConditionOperatorType.Equal, filter.CompanyCode);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();

                EnumColumnList enumConfigNews = new EnumColumnList();
                enumConfigNews.Add("Status", typeof(AmbassadorNewsStatus));

                cmd.ConvertEnumColumn(ds.Tables[0], enumConfigNews);

                if (Int32.TryParse(cmd.GetParameterValue("@TotalCount").ToString(), out totalCount))
                { }
                else
                {
                    totalCount = 0;
                }
                return ds;
            }
        }

        #endregion
    }
}
