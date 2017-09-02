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
    [VersionExport(typeof(ILendRequestQueryDA))]
    public class LendRequestQueryDA : ILendRequestQueryDA
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
                        tsort[0] = "a.LendID";
                        break;
                    case "REQUESTSTATUS":
                        tsort[0] = "a.Status";
                        break;
                    case "LENDTOTALCOST":
                        tsort[0] = "SUM(IsNULL(li.LendCostWhenCreate,0)*IsNULL(li.LendQty,0))";
                        break;
                    case "LENDTOTALAMOUNT":
                        tsort[0] = "SUM(IsNULL(i.CurrentPrice,0)*IsNULL(li.LendQty,0))";
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
                    case "LENDUSERNAME":
                        tsort[0] = "ul.ACLogicUserName";
                        break;
                    case "CREATEUSERNAME":
                        tsort[0] = "uc.ACLogicUserName";
                        break;
                    case "AUDITUSERNAME":
                        tsort[0] = "uA.ACLogicUserName";
                        break;
                    case "OUTSTOCKUSERNAME":
                        tsort[0] = "uo.ACLogicUserName";
                        break;
                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        #region ILendRequestQueryDA Members

        /// <summary>
        /// 查询借货单
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryLendRequest(LendRequestQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = SortFieldMapping(queryCriteria.PagingInfo.SortBy);
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryLendRequest");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                if (queryCriteria.SysNo.HasValue && queryCriteria.SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.SysNo", DbType.Int32, "@RequestSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.SysNo);
                }

                if (!string.IsNullOrEmpty(queryCriteria.RequestID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.LendID", DbType.String, "@RequestID",
                            QueryConditionOperatorType.Like, queryCriteria.RequestID);
                }
 
                if (queryCriteria.StockSysNo.HasValue && queryCriteria.StockSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.StockSysNo", DbType.Int32, "@StockSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.StockSysNo);
                }


                if (queryCriteria.LendUserSysNo.HasValue && queryCriteria.LendUserSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.LendUserSysNo", DbType.String, "@LendUserSysNo",
                            QueryConditionOperatorType.LeftLike, queryCriteria.LendUserSysNo);
                }

                if (queryCriteria.RequestStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.Status", DbType.Int32, "@RequestStatus",
                            QueryConditionOperatorType.Equal, queryCriteria.RequestStatus);
                }

                if (queryCriteria.CreateDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.CreateTime", DbType.DateTime, "@CreateTimeFrom",
                            QueryConditionOperatorType.MoreThanOrEqual, queryCriteria.CreateDateFrom);
                }

                if (queryCriteria.CreateDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.CreateTime", DbType.DateTime, "@CreateTimeTo",
                            QueryConditionOperatorType.LessThanOrEqual, queryCriteria.CreateDateTo);
                }

                if (!string.IsNullOrEmpty(queryCriteria.CompanyCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                            QueryConditionOperatorType.Equal, queryCriteria.CompanyCode);
                }

                if ((queryCriteria.ProductSysNo.HasValue && queryCriteria.ProductSysNo > 0))
                {
                    //添加子查询，并添加参数
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "",
                        QueryConditionOperatorType.Exist,
                        @"  SELECT TOP 1 li.SysNo 
                            FROM IPP3.dbo.St_Lend_Item li WITH(NOLOCK) 
                            WHERE a.SysNo=li.LendSysNo and li.ProductSysNo =@ProductSysNo");

                    cmd.AddInputParameter("@ProductSysNo", DbType.Int32, queryCriteria.ProductSysNo);
                }

                if (queryCriteria.PMUserSysNo.HasValue && queryCriteria.PMUserSysNo > 0)
                {
                    string subSQLString_ProductSysNoList =
                     @" SELECT ProductLineSysNo 
                      FROM OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p 
                      WHERE  PMUserSysNo="+queryCriteria.PMUserSysNo+" OR CHARINDEX(';'+CAST("+queryCriteria.PMUserSysNo+" AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0";

                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                    "a.ProductLineSysno", QueryConditionOperatorType.In, subSQLString_ProductSysNoList);                
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

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("RequestStatus", typeof(LendRequestStatus));

                var resultData = cmd.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }
        }

        /// <summary>
        /// 按PM统计
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable ExportAllByPM(LendRequestQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryLendItemforExport");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " userInfo.DisplayName ASC"))
            {

                if (queryCriteria.SysNo.HasValue && queryCriteria.SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "lend.SysNo", DbType.Int32, "@RequestSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.SysNo);
                }

                if (!string.IsNullOrEmpty(queryCriteria.RequestID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "lend.LendID", DbType.String, "@RequestID",
                            QueryConditionOperatorType.Like, queryCriteria.RequestID);
                }

                if (queryCriteria.LendUserSysNo.HasValue && queryCriteria.LendUserSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "lendUser.IPPUserSysNo", DbType.String, "@LendUserSysNo",
                            QueryConditionOperatorType.LeftLike, queryCriteria.LendUserSysNo);
                }

                if (queryCriteria.CreateDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "lend.CreateTime", DbType.DateTime, "@CreateTimeFrom",
                            QueryConditionOperatorType.MoreThanOrEqual, queryCriteria.CreateDateFrom);
                }

                if (queryCriteria.CreateDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "lend.CreateTime", DbType.DateTime, "@CreateTimeTo",
                            QueryConditionOperatorType.LessThanOrEqual, queryCriteria.CreateDateTo);
                }

                if (queryCriteria.RequestStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "lend.Status", DbType.Int32, "@RequestStatus",
                            QueryConditionOperatorType.Equal, queryCriteria.RequestStatus);
                }

                if (queryCriteria.StockSysNo.HasValue && queryCriteria.StockSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "stock.SysNo", DbType.Int32, "@StockSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.StockSysNo);
                }

                if (queryCriteria.ProductSysNo.HasValue && queryCriteria.ProductSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                           "lendItem.productsysno", DbType.Int32, "@ProductSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.ProductSysNo);
                }

                if (queryCriteria.PMUserSysNo.HasValue && queryCriteria.PMUserSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                          "itemBasic.PMUserSysNo", DbType.Int32, "@PMUserSysNo",
                          QueryConditionOperatorType.Equal, queryCriteria.ProductSysNo);
                }

                if (!string.IsNullOrEmpty(queryCriteria.CompanyCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "lendItem.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                         QueryConditionOperatorType.Equal, queryCriteria.CompanyCode);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable resultData = cmd.ExecuteDataTable();
                totalCount = 10000;
                return resultData;
            }
        }

        /// <summary>
        /// 根据状态获取对应状态下的成本
        /// </summary>
        /// <param name="queryCriteria">查询条件</param>
        /// <returns></returns>
        public DataTable QueryLendCostbyStatus(LendRequestQueryFilter queryCriteria)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryLendCostbyStatus");
            #region 加载条件
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd, "a.SysNo DESC"))
            {
                if (queryCriteria.SysNo.HasValue && queryCriteria.SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "st.SysNo", DbType.Int32, "@RequestSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.SysNo);
                }

                if (!string.IsNullOrEmpty(queryCriteria.RequestID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "st.LendID", DbType.String, "@RequestID",
                            QueryConditionOperatorType.Like, queryCriteria.RequestID);
                }

                if (queryCriteria.StockSysNo.HasValue && queryCriteria.StockSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "s.StockSysNo", DbType.Int32, "@StockSysNo",
                            QueryConditionOperatorType.Equal, queryCriteria.StockSysNo);
                }


                if (queryCriteria.LendUserSysNo.HasValue && queryCriteria.LendUserSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "l.IPPUserSysNo", DbType.String, "@LendUserSysNo",
                            QueryConditionOperatorType.LeftLike, queryCriteria.LendUserSysNo);
                }

                if (queryCriteria.RequestStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "st.Status", DbType.Int32, "@RequestStatus",
                            QueryConditionOperatorType.Equal, queryCriteria.RequestStatus);
                }

                if (queryCriteria.CreateDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "st.CreateTime", DbType.DateTime, "@CreateTimeFrom",
                            QueryConditionOperatorType.MoreThanOrEqual, queryCriteria.CreateDateFrom);
                }

                if (queryCriteria.CreateDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "st.CreateTime", DbType.DateTime, "@CreateTimeTo",
                            QueryConditionOperatorType.LessThanOrEqual, queryCriteria.CreateDateTo);
                }

                if (!string.IsNullOrEmpty(queryCriteria.CompanyCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "st.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                            QueryConditionOperatorType.Equal, queryCriteria.CompanyCode);
                }

                if ((queryCriteria.ProductSysNo.HasValue && queryCriteria.ProductSysNo > 0)
                    && !(queryCriteria.PMUserSysNo.HasValue && queryCriteria.PMUserSysNo > 0))
                {
                    //添加子查询，并添加参数
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "",
                        QueryConditionOperatorType.Exist,
                        @"  SELECT TOP 1 li.SysNo 
                            FROM IPP3.dbo.St_Lend_Item li WITH(NOLOCK) 
                            WHERE st.SysNo=li.LendSysNo and li.ProductSysNo =@ProductSysNo");

                    cmd.AddInputParameter("@ProductSysNo", DbType.Int32, queryCriteria.ProductSysNo);
                }

                if (!(queryCriteria.ProductSysNo.HasValue && queryCriteria.ProductSysNo > 0)
                    && (queryCriteria.PMUserSysNo.HasValue && queryCriteria.PMUserSysNo > 0))
                {
                    //添加子查询，并添加数
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "", QueryConditionOperatorType.Exist,
                        @"  SELECT TOP 1 li.SysNo
                            FROM IPP3.dbo.st_Lend_Item li WITH(NOLOCK) 
                                INNER JOIN OverseaContentManagement.dbo.V_CM_ItemBasicInfo i WITH(NOLOCK) 
                                ON li.ProductSysNo=i.SysNo 
                            WHERE li.LendSysNo=st.sysno and i.PMUserSysNo=@PMUserSysNo");

                    cmd.AddInputParameter("@PMUserSysNo", DbType.Int32, queryCriteria.PMUserSysNo);
                }

                if ((queryCriteria.ProductSysNo.HasValue && queryCriteria.ProductSysNo > 0)
                    && (queryCriteria.PMUserSysNo.HasValue && queryCriteria.PMUserSysNo > 0))
                {
                    //添加子查询，并添加参数
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "", QueryConditionOperatorType.Exist,
                        @"  SELECT TOP 1 li.SysNo 
                            FROM IPP3.dbo.St_Lend_Item li WITH(NOLOCK) 
                                INNER JOIN OverseaContentManagement.dbo.V_CM_ItemBasicInfo i WITH(NOLOCK)  
                                ON li.ProductSysNo=i.SysNo 
                            WHERE li.LendSysNo=st.SysNo AND i.PMUserSysNo=@PMUserSysNo AND li.ProductSysNo =@ProductSysNo");

                    cmd.AddInputParameter("@PMUserSysNo", DbType.Int32, queryCriteria.PMUserSysNo);
                    cmd.AddInputParameter("@ProductSysNo", DbType.Int32, queryCriteria.ProductSysNo);
                }
          
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var resultData = cmd.ExecuteDataTable();
                return resultData;
            }
            #endregion
        }
        #endregion
    }
}
