using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductPriceCompareQueryDA))]
    public class ProductPriceCompareQueryDA : IProductPriceCompareQueryDA
    {
        public DataTable Query(ProductPriceCompareQueryFilter query, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = query.PageInfo.SortBy;
            pagingEntity.MaximumRows = query.PageInfo.PageSize;
            pagingEntity.StartRowIndex = query.PageInfo.PageIndex * query.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductPriceCompare_Query");
            var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "EP.SysNo DESC");

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "EP.ProductSysNo",
                DbType.String,
                "@ProductSysNo",
                QueryConditionOperatorType.Equal,
                query.ProductSysNo);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "IP.ProductID",
                DbType.String,
                "@ProductID",
                QueryConditionOperatorType.Equal,
                query.ProductID);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "EP.CreateTime",
                DbType.DateTime,
                "@CreateTimeBegin",
                QueryConditionOperatorType.MoreThanOrEqual,
                query.CreateTimeFrom);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "EP.CreateTime",
                DbType.DateTime,
                "@CreateTimeEnd",
                QueryConditionOperatorType.LessThanOrEqual,
                query.CreateTimeTo);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "EP.Status",
                DbType.Int32,
                "@Status",
                QueryConditionOperatorType.Equal,
                query.Status);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "CategoryInfo.Category3Sysno",
                DbType.String,
                "@Category3",
                QueryConditionOperatorType.Equal,
                query.C3SysNo);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "CategoryInfo.Category2Sysno",
                DbType.String,
                "@Category2",
                QueryConditionOperatorType.Equal,
                query.C2SysNo);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "CategoryInfo.Category1Sysno",
                DbType.String,
                "@Category1",
                QueryConditionOperatorType.Equal,
                query.C1SysNo);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SysUser.UserSysNo",
                DbType.String,
                "@PMUserSysNo",
                QueryConditionOperatorType.Equal,
                query.PMSysNo);
            //TODO:添加渠道查询条件
            cmd.CommandText = sqlBuilder.BuildQuerySql();


            EnumColumnList enumConfig = new EnumColumnList();
            enumConfig.Add("Status", typeof (ProductPriceCompareStatus));
            enumConfig.Add("DisplayLinkStatus", typeof(DisplayLinkStatus));
            var dt = cmd.ExecuteDataTable(enumConfig);
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }
    }
}
