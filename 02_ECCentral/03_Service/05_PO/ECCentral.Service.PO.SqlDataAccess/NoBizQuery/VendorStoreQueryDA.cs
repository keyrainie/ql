using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IVendorStoreQueryDA))]
    public class VendorStoreQueryDA : IVendorStoreQueryDA
    {
        public DataTable QueryVendorStoreList(int vendorSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetVendorStoreInfoList");
            dataCommand.SetParameterValue("@VendorSysNo", vendorSysNo);
            return dataCommand.ExecuteDataTable();
        }


        public DataTable QueryCommissionRuleTemplateInfo(CommissionRuleTemplateQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = -1;
                return null;
            }
            CustomDataCommand dataCommand = dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCommissionRuleTemplateInfo");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "r.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "r.C3SysNo",
                           DbType.AnsiStringFixedLength, "@C3SysNo", QueryConditionOperatorType.Equal, queryFilter.C3SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "r.C2SysNo",
               DbType.AnsiStringFixedLength, "@C2SysNo", QueryConditionOperatorType.Equal, queryFilter.C2SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "r.C1SysNo",
                DbType.AnsiStringFixedLength, "@C1SysNo", QueryConditionOperatorType.Equal, queryFilter.C1SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "r.BrandSysNo",
               DbType.AnsiStringFixedLength, "@BrandSysNo", QueryConditionOperatorType.Equal, queryFilter.BrandSysNo);


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "r.Status",
                DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal, queryFilter.Status);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList columnEnums = new EnumColumnList();
                columnEnums.Add("Status", typeof(CommissionRuleStatus));
                DataTable dt = dataCommand.ExecuteDataTable(columnEnums);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QuerySecondDomain(SecondDomainQueryFilter filter, out int totalCount)
        {

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PageInfo.SortBy,
                StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize,
                MaximumRows = filter.PageInfo.PageSize
            };

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySecondDomain");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "v.[SysNo] DESC"))
            {
                builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "v.SysNo", DbType.Int32, filter.VendorSysNoList);

                command.CommandText = builder.BuildQuerySql();
                DataTable dt = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public bool CheckSecondDomainStatus(int SysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("CheckSecondDomainStatus");
            dataCommand.SetParameterValue("@SysNo", SysNo);
            return dataCommand.ExecuteScalar<int>() == 0;
        }

        public void ChangeSecondDomainStatus(int SysNo,int SecondDomainStatus)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ChangeSecondDomainStatus");
            dataCommand.SetParameterValue("@SysNo", SysNo);
            dataCommand.SetParameterValue("@SecondDomainStatus", SecondDomainStatus);
            dataCommand.ExecuteNonQuery();
        }
    }
}
