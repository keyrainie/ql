using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IHelpCenterQueryDA))]
    public class HelpCenterQueryDA : IHelpCenterQueryDA
    {
        public DataTable Query(HelpCenterQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("HelpCenter_GetHelpTopic");
            var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "B.SysNo DESC");
            sqlBuilder.ConditionConstructor.AddCondition(
              QueryConditionRelationType.AND,
              "B.Priority",
              DbType.Int32,
              "@Priority",
              QueryConditionOperatorType.Equal,
              filter.Priority);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "B.Status",
           DbType.AnsiStringFixedLength,
           "@Status",
           QueryConditionOperatorType.Equal,
           filter.Status);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "B.Type",
           DbType.AnsiStringFixedLength,
           "@Type",
           QueryConditionOperatorType.Equal,
           filter.Type);

            sqlBuilder.ConditionConstructor.AddCondition(
             QueryConditionRelationType.AND,
             "B.CategorySysNo",
             DbType.Int32,
             "@CategorySysNo",
             QueryConditionOperatorType.Equal,
             filter.CategorySysNo);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "B.CompanyCode",
           DbType.AnsiStringFixedLength,
           "@CompanyCode",
           QueryConditionOperatorType.Equal,
         filter.CompanyCode);
            //TODO:添加ChannelID参数
            cmd.CommandText = sqlBuilder.BuildQuerySql();
            EnumColumnList enumConfig = new EnumColumnList();
            enumConfig.Add("Status", typeof(ADStatus));
            enumConfig.Add("Type", typeof(FeatureType));
            var dt = cmd.ExecuteDataTable(enumConfig);
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }

        public DataTable QueryCategory(HelpCenterCategoryQueryFilter filter)
        {
            DataCommand command = DataCommandManager.GetDataCommand("HelpCenter_GetCategory");
            command.SetParameterValue("@Status", filter.Status);
            command.SetParameterValue("@CompanyCode", filter.CompanyCode);
            return command.ExecuteDataTable();
        }
    }
}
