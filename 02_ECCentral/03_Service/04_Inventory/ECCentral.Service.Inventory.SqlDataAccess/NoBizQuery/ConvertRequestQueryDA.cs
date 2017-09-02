using System;
using System.Data;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IConvertRequestQueryDA))]
    public class ConvertRequestQueryDA : IConvertRequestQueryDA
    {

        private string SortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "REQUESTID":
                        tsort[0] = "a.TransferID";
                        break;
                    case "REQUESTSTATUS":
                        tsort[0] = "a.Status";
                        break;
                    case "CREATEDATE":
                        tsort[0] = "a.CreateTime";
                        break;
                    case "AUDITDATE":
                        tsort[0] = "a.AuditTime";
                        break;
                    case "OUTSTOCKDATE":
                        tsort[0] = "a.OutTime";
                        break;
                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        /// <summary>
        /// 查询转换单
        /// </summary>
        /// <returns></returns>
        /// 
        public DataTable QueryConvertRequest(ConvertRequestQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = SortFieldMapping(queryCriteria.PagingInfo.SortBy);
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryConvertRequest");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                if (queryCriteria.SysNo.HasValue && queryCriteria.SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.SysNo", DbType.Int32, "@RequestSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.SysNo);
                }
                else
                {

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.TransferID", DbType.String, "@RequestID",
                            QueryConditionOperatorType.Like, queryCriteria.RequestID);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.Status", DbType.Int32, "@RequestStatus",
                            QueryConditionOperatorType.Equal, queryCriteria.RequestStatus);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.StockSysNo", DbType.Int32, "@StockSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.StockSysNo);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.CreateTime", DbType.DateTime, "@CreateDateFrom",
                            QueryConditionOperatorType.MoreThanOrEqual, queryCriteria.CreateDateFrom);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.CreateTime", DbType.DateTime, "@CreateDateTo",
                            QueryConditionOperatorType.LessThanOrEqual, queryCriteria.CreateDateTo);

                }

                //不是高级权限
                if (queryCriteria.PMQueryRightType != PMQueryType.AllValid)
                {
                    string subSQLString_ProductSysNoList =
                    @" SELECT ProductLineSysNo 
                    FROM OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p 
                    WHERE  PMUserSysNo=" + queryCriteria.UserSysNo + " OR CHARINDEX(';'+CAST(" + queryCriteria.UserSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0";

                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                    "a.ProductLineSysno", QueryConditionOperatorType.In, subSQLString_ProductSysNoList);
                }
                if (queryCriteria.ProductSysNo.HasValue)
                {
                    //添加子查询，并添加参数
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "",
                        QueryConditionOperatorType.Exist,
                        @"  SELECT TOP 1 ti.SysNo
                            FROM IPP3.dbo.St_Transfer_Item ti WITH(NOLOCK) 
                            WHERE a.SysNo = ti.TransferSysNo AND ti.ProductSysNo =@ProductSysNo");

                    cmd.AddInputParameter("@ProductSysNo", DbType.Int32, queryCriteria.ProductSysNo);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.CompanyCode", DbType.String, "@CompanyCode",
                        QueryConditionOperatorType.Equal, queryCriteria.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("RequestStatus", typeof(ConvertRequestStatus));

                var resultData = cmd.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }
        }
    }
}
