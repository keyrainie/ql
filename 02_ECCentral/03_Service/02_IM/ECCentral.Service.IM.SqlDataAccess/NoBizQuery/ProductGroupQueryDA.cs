using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductGroupQueryDA))]
    public class ProductGroupQueryDA : IProductGroupQueryDA
    {

        /// <summary>
        /// 商品组查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryProductGroupInfo(ProductGroupQueryFilter queryCriteria, out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductGroupInfo");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "pgi.SysNo DESC"))
            {
                if (!String.IsNullOrEmpty(queryCriteria.ProductGroupName))
                {
                    dataCommand.AddInputParameter("@ProductGroupName", DbType.String, queryCriteria.ProductGroupName);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "pgi.ProductName",
                        DbType.String, "@ProductGroupName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.ProductGroupName);
                }
                if (queryCriteria.C3SysNo.HasValue)
                {
                    dataCommand.AddInputParameter("@C3SysNo", DbType.Int32, queryCriteria.C3SysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "pgi.C3SysNo",
                        DbType.Int32, "@C3SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C3SysNo);
                }
                if (queryCriteria.C2SysNo.HasValue)
                {
                    dataCommand.AddInputParameter("@C2SysNo", DbType.Int32, queryCriteria.C2SysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.C2SysNo",
                        DbType.Int32, "@C2SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C2SysNo);
                }
                if (queryCriteria.C1SysNo.HasValue)
                {
                    dataCommand.AddInputParameter("@C1SysNo", DbType.Int32, queryCriteria.C1SysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "D.C1SysNo",
                        DbType.Int32, "@C1SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C1SysNo);
                }
                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist, 
                    " Select TOP 1 P.SysNo from IPP3.dbo.Product P WITH(NOLOCK) " +
                    "inner join OverseaContentManagement.dbo.ProductCommonInfo c WITH(NOLOCK) " +
                    "on c.SysNo=P.ProductCommonInfoSysno where c.ProductGroupSysno=pgi.SysNO");
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
