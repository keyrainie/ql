using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Common;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IHomePageSectionQueryDA))]
    public class HomePageSectionQueryDA : IHomePageSectionQueryDA
    {
        public DataTable Query(HomePageSectionQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Domain_GetDomainList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                      QueryConditionRelationType.AND,
                      "A.CompanyCode",
                      DbType.AnsiStringFixedLength,
                      "@CompanyCode",
                      QueryConditionOperatorType.Equal,
                    filter.CompanyCode);
                //TODO:添加ChannelID参数

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumConfig = new EnumColumnList();
                enumConfig.Add("Status", typeof(ADStatus));
                var dt = cmd.ExecuteDataTable(enumConfig);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }

        }

        public List<CodeNamePair> GetDomainCodeNames(string companyCode, string channelID)
        {
            var cmd = DataCommandManager.GetDataCommand("Domain_GetDomainCodeNames");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@ChannelID", channelID);

            return cmd.ExecuteEntityList<CodeNamePair>();
        }
    }
}
