using System;
using System.Data;
using System.Collections.Generic;

using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IShiftRequestQueryDA))]
    public class ShiftRequestQueryDA : IShiftRequestQueryDA
    {
        private string ShiftRequestSortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "REQUESTID":
                        tsort[0] = "a.ShiftID";
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
                    case "INSTOCKDATE":
                        tsort[0] = "a.InTime";
                        break;
                    case "SOURCESTOCKSYSNO":
                        tsort[0] = "a.StockSysNoA";
                        break;
                    case "TARGETSTOCKSYSNO":
                        tsort[0] = "a.StockSysNoB";
                        break;
                    case "TOTALAMOUNT":
                        tsort[0] = "a.TotalAmt";
                        break;
                    case "TRACKINGNUMBER":
                        tsort[0] = "a.TrackingNumber";
                        break;
                    case "SHIFTSHIPPINGTYPE":
                        tsort[0] = "a.shipViaTerm";
                        break;
                    case "TOTALWEIGHT":
                        tsort[0] = "TotalWeight";
                        break;
                    case "SPECIALSHIFTTYPE":
                        tsort[0] = "a.SpecialShiftType";
                        break;
                    case "SPECIALSHIFTSETDATE":
                        tsort[0] = "a.SpecialShiftSetTime";
                        break;
                    case "SOSYSNO":
                        tsort[0] = "AutoST.SOSysNo";
                        break;
                    case "SOSTATUS":
                        tsort[0] = "AutoST.SOstatus";
                        break;
                    case "SOOUTSTOCKDATE":
                        tsort[0] = "SOOutStockDate";
                        break;

                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        private string ShiftConfigSortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "SYSNO":
                        tsort[0] = " a.SysNo";
                        break;
                    case "OUTSTOCKNAME":
                        tsort[0] = "d.StockName";
                        break;
                    case "INSTOCKNAME":
                        tsort[0] = "e.StockName";
                        break;
                    case "SPLINTERVAL":
                        tsort[0] = "a.SPLTimeInterval";
                        break;
                    case "SHIPINTERVAL":
                        tsort[0] = "a.ShipTimeInterval";
                        break;
                    case "CREATETIME":
                        tsort[0] = "a.CreateTime";
                        break;
                    case "UPDATETIME":
                        tsort[0] = "a.UpdateTime";
                        break;
                    case "SHIFTTYPE":
                        tsort[0] = "a.ShiftType";
                        break;               

                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        private PagingInfoEntity PageInfoToEntity(PagingInfo info)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = info.SortBy;
            pagingEntity.MaximumRows = info.PageSize;
            pagingEntity.StartRowIndex = info.PageIndex * info.PageSize;
            return pagingEntity;
        }
        /// <summary>
        /// 查询移仓单
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryShiftRequest(ShiftRequestQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = PageInfoToEntity(queryCriteria.PagingInfo);
            pagingEntity.SortField = ShiftRequestSortFieldMapping(pagingEntity.SortField);

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryShiftRequest");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                //Build Query Condition
                BuildeCondition(queryCriteria, pagingEntity, cmd, sqlBuilder);

                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("RequestStatus", typeof(ShiftRequestStatus));
                enumColumnList.Add("SOStatus", typeof(ECCentral.BizEntity.SO.SOStatus));
                enumColumnList.Add("SpecialShiftType", typeof(SpecialShiftRequestType));

                var resultData = cmd.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }
        }

        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        public DataSet QueryCountData(ShiftRequestQueryFilter queryCriteria)
        {
            PagingInfoEntity pagingEntity = PageInfoToEntity(queryCriteria.PagingInfo);
            pagingEntity.SortField = ShiftRequestSortFieldMapping(pagingEntity.SortField);

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCountData");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                BuildeCondition(queryCriteria, pagingEntity, cmd, sqlBuilder);
                return cmd.ExecuteDataSet();
            }
        }

        private static void BuildeCondition(ShiftRequestQueryFilter queryCriteria, PagingInfoEntity pagingEntity, CustomDataCommand cmd, DynamicQuerySqlBuilder sqlBuilder)
        {
            if (!string.IsNullOrEmpty(queryCriteria.RequestID))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ShiftID", DbType.String, "@RequestID",
                    QueryConditionOperatorType.Like, queryCriteria.RequestID);
            }

            if (queryCriteria.ShiftRequestSysNoList != null && queryCriteria.ShiftRequestSysNoList.Count > 0)
            {
                sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND,
                    "a.SysNo", DbType.Int32, queryCriteria.ShiftRequestSysNoList);

            }

            if (!string.IsNullOrEmpty(queryCriteria.CompanyCode))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                        QueryConditionOperatorType.Equal, queryCriteria.CompanyCode);
            }

            if (queryCriteria.CreateDateFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CreateTime", DbType.DateTime, "@CreateDateFrom",
                    QueryConditionOperatorType.MoreThan, queryCriteria.CreateDateFrom);
            }

            if (queryCriteria.CreateDateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CreateTime", DbType.DateTime, "@CreateDateTo",
                    QueryConditionOperatorType.LessThan, queryCriteria.CreateDateTo.Value.AddDays(1));
            }

            if (queryCriteria.CreateUserSysNo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CreateUserSysNo", DbType.Int32, "@CreateUserSysNo",
                    QueryConditionOperatorType.Equal, queryCriteria.CreateUserSysNo.Value);
            }

            if (queryCriteria.InStockDateFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.InTime", DbType.DateTime, "@InStockDateFrom",
                    QueryConditionOperatorType.MoreThan, queryCriteria.InStockDateFrom);
            }

            if (queryCriteria.InStockDateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.InTime", DbType.DateTime, "@InStockDateTo",
                    QueryConditionOperatorType.LessThan, queryCriteria.InStockDateTo.Value.AddDays(1));
            }
            //是否为特殊移仓单
            if (queryCriteria.IsSpecialShift.HasValue && queryCriteria.IsSpecialShift.Value)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.SpecialShiftType", DbType.Int32, "@SpecialShiftType",
                    QueryConditionOperatorType.MoreThan, 0);
            }

            if (queryCriteria.SpecialShiftRequestStatus.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.SpecialShiftType", DbType.Int32, "@SpecialShiftRequestStatus",
                    QueryConditionOperatorType.Equal, queryCriteria.SpecialShiftRequestStatus);
            }

            if (queryCriteria.ProductSysNo.HasValue && queryCriteria.ProductSysNo > 0)
            {
                //添加子查询，并添加参数
                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "",
                    QueryConditionOperatorType.Exist,
                    @"  SELECT TOP 1 SysNo 
                            FROM St_Shift_Item si WITH(NOLOCK)
                            WHERE a.SysNo = si.ShiftSysNo AND si.ProductSysNo = @ProductSysNo");

                cmd.AddInputParameter("@ProductSysNo", DbType.Int32, queryCriteria.ProductSysNo);
            }

            if (queryCriteria.OutStockDateFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.OutTime", DbType.DateTime, "@OutStockDateFrom",
                    QueryConditionOperatorType.MoreThan, queryCriteria.OutStockDateFrom);
            }

            if (queryCriteria.OutStockDateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.OutTime", DbType.DateTime, "@OutStockDateTo",
                    QueryConditionOperatorType.LessThan, queryCriteria.OutStockDateTo.Value.AddDays(1));
            }

            if (queryCriteria.ConsignFlag.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.IsConsign", DbType.Int32, "@ConsignFlag",
                    QueryConditionOperatorType.Equal, queryCriteria.ConsignFlag);
            }

            if (queryCriteria.SOStatus.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "AutoST.SOstatus", DbType.Int32, "@SOstatus",
                    QueryConditionOperatorType.Equal, queryCriteria.SOStatus);
            }

            if (queryCriteria.RequestStatus.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Status", DbType.Int32, "@RequestStatus",
                    QueryConditionOperatorType.Equal, queryCriteria.RequestStatus);
            }


            //当前数据存放的是配送方式描述，而不是枚举值
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "a.ShipViaTerm", DbType.String, "@ShipViaTerm",
                QueryConditionOperatorType.Equal, queryCriteria.ShiftShippingType);


            if (queryCriteria.ShiftRquestType.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ShiftType", DbType.Int32, "@ShiftType",
                    QueryConditionOperatorType.Equal, queryCriteria.ShiftRquestType);

            }

            if (queryCriteria.SourceStockSysNo.HasValue && queryCriteria.SourceStockSysNo > 0)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.StockSysNoA", DbType.Int32, "@SourceStockSysNo",
                    QueryConditionOperatorType.Equal, queryCriteria.SourceStockSysNo);
            }

            if (queryCriteria.TargetStockSysNo.HasValue && queryCriteria.TargetStockSysNo > 0)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.StockSysNoB", DbType.Int32, "@TargetStockSysNo",
                    QueryConditionOperatorType.Equal, queryCriteria.TargetStockSysNo);

            }

            if (queryCriteria.CreateUserSysNo.HasValue && queryCriteria.CreateUserSysNo > 0)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CreateUserSysNo", DbType.Int32, "@CreateUserSysNo",
                    QueryConditionOperatorType.Equal, queryCriteria.CreateUserSysNo);
            }
            if (queryCriteria.IsVirtualTransfer.HasValue)
            {
                switch (queryCriteria.IsVirtualTransfer.Value)
                {
                    case VirtualTransferType.Yes:
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "a.Status", QueryConditionOperatorType.NotIn, "1,2");
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "a.IsScanned", QueryConditionOperatorType.In, "0");
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "a.CreateUserSysNo", QueryConditionOperatorType.In, "493");
                        break;
                    case VirtualTransferType.No:
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "a.Status", QueryConditionOperatorType.NotIn, "1,2");
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "a.IsScanned", QueryConditionOperatorType.NotIn, "0");
                        break;
                    case VirtualTransferType.Empty:
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "a.Status", QueryConditionOperatorType.In, "1,2");
                        break;
                    default:
                        break;
                }
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
            string tmpSql = string.Empty;
            string soCondition = null;
            bool isSOCondition = false;
            if (queryCriteria.SOStatus.HasValue)
            {
                soCondition = String.Format(" AND so.[Status]={0} ", (int)queryCriteria.SOStatus.Value);
                isSOCondition = true;
            }
            if (queryCriteria.SOSysNoList != null && queryCriteria.SOSysNoList.Count > 0)
            {
                soCondition = String.Format(" AND so.SysNo IN({0}) ", queryCriteria.SOSysNoList.Join(","));
                isSOCondition = true;
            }
            if (isSOCondition)
            {

                tmpSql = String.Format(@"INNER JOIN ( 
                                    SELECT 
                                        DISTINCT sost.ShiftSysNo ShiftSysNo 
                                       ,so.SysNo SOSysNo 
                                       ,so.OutTime SOOutStockDate
                                       ,so.Status SOStatus  
                                    FROM IPP3.dbo.SO_AutoShift sost WITH (NOLOCK) 
                                        INNER JOIN IPP3.dbo.V_SO_Master so WITH (NOLOCK) 
                                            ON so.SysNo = sost.SOSysNo 
                                                AND sost.Status= 1 {0}) AutoST ON AutoST.ShiftSysNo = a.SysNo", soCondition);
            }
            else if (pagingEntity.SortField != null
                && (pagingEntity.SortField.ToUpper().IndexOf("AUTOST") > -1
                    || pagingEntity.SortField.ToUpper().IndexOf("SOOUTSTOCKDATE") > -1))
            {
                tmpSql = @"INNER JOIN ( 
                                    SELECT 
                                        DISTINCT sost.ShiftSysNo ShiftSysNo 
                                       ,so.SysNo SOSysNo 
                                       ,so.OutTime SOOutStockDate
                                       ,so.Status SOStatus  
                                    FROM IPP3.dbo.SO_AutoShift sost WITH (NOLOCK) 
                                        INNER JOIN IPP3.dbo.V_SO_Master so WITH (NOLOCK) 
                                            ON so.SysNo = sost.SOSysNo 
                                                AND sost.Status= 1 ) AutoST 
                               ON AutoST.ShiftSysNo = a.SysNo";
            }

            cmd.CommandText = cmd.CommandText.Replace("#JoinType_Count#", tmpSql);

            cmd.CommandText = cmd.CommandText.Replace("#SortByWeightCondition#", (pagingEntity.SortField != null && pagingEntity.SortField.ToUpper().IndexOf("TOTALWEIGHT") > -1) ? @"
                                LEFT JOIN ( 
                                    SELECT 
                                        si.ShiftSysNo 
                                       ,SUM(si.ShiftQty * product.weight) AS TotalWeight 
                                    FROM IPP3.dbo.St_Shift_Item si WITH (NOLOCK) 
                                    LEFT JOIN OverseaContentManagement.dbo.V_CM_ItemBasicInfo product WITH (NOLOCK) 
                                        ON si.ProductSysNo=product.SysNo 
                                    GROUP BY si.ShiftSysNo 
                                 ) AS b 
                                 ON a.SysNo = b.ShiftSysNo 
                            " : string.Empty);

            cmd.CommandText = cmd.CommandText.Replace("#JoinType#", (queryCriteria.SOStatus.HasValue
                                                                        ? "INNER JOIN"
                                                                        : "LEFT JOIN"));
        }

        public DataTable QueryStockShiftConfig(StockShiftConfigFilter filter, out int totalCount)
        {
            totalCount = 0;
            if (filter == null)
            {
                return null;
            }
            PagingInfoEntity pagingEntity = PageInfoToEntity(filter.PagingInfo);
            pagingEntity.SortField = ShiftConfigSortFieldMapping(pagingEntity.SortField);
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_Query_StockShiftConfig");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingEntity, "a.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.Int32, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockASysno",
                    DbType.Int32, "@WarehouseASysNumber", QueryConditionOperatorType.Equal, filter.OutStockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockBSysno",
                    DbType.Int32, "@WarehouseBSysNumber", QueryConditionOperatorType.Equal, filter.InStockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ShiftType",
                    DbType.Int32, "@TransferType", QueryConditionOperatorType.Equal, filter.ShiftType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.SPLTimeInterval",
                    DbType.Int32, "@SPLInterval", QueryConditionOperatorType.Equal, filter.SPLInterval);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ShipTimeInterval",
                    DbType.Int32, "@ShipInterval", QueryConditionOperatorType.Equal, filter.ShipInterval);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
           
                DataTable dt = dataCommand.ExecuteDataTable();
                if (null != dt && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["ShiftTypeString"] = CodeNamePairManager.GetName("Inventory", "StockShiftConfigShippingType", dr["ShiftType"].ToString());
                    }
                }

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QueryShiftRequestCreateUserList(string companyCode, out int totalCount)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Inventory_QueryShiftRequestCreateUserList");
            dataCommand.SetParameterValue("@CompanyCode", companyCode);            
            DataTable dt = dataCommand.ExecuteDataTable();
            totalCount = dt == null ? 0 : dt.Rows.Count;
            return dt;
        }


        public DataTable GetRMAShift(int shiftSysNo)
        { 
            var dataCommand = DataCommandManager.GetDataCommand("GetRMAShift");
            dataCommand.SetParameterValue("@SysNo", shiftSysNo);
            dataCommand.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            return dataCommand.ExecuteDataTable();
        }
    }
}
