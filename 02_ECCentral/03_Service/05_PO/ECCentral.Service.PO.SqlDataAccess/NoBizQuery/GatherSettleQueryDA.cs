using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IGatherSettleQueryDA))]
    public class GatherSettleQueryDA : IGatherSettleQueryDA
    {
        #region IGatherSettleQueryDA Members

        public System.Data.DataTable Query(QueryFilter.PO.GatherSettleQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            string wherhoseSql = @" AND EXISTS (  SELECT 1 FROM 
                                  (
          	                         SELECT DISTINCT 
			                                so.WareHouseNumber
					                         ,ite.SettlementSysNo
					
			                          FROM OverseaPOASNManagement.dbo.CollectionSettlement_Item ite WITH(NOLOCK)
			                          INNER JOIN IPP3.dbo.RMA_Refund  re WITH(NOLOCK)
			                          ON re.SysNo = ite.ReferenceSysNo AND ite.ReferenceType = 'RMA'
			                          INNER JOIN OverseaOrderManagement.dbo.V_OM_Sub_SO_Master so WITH(nolock)
			                          ON so.SOSysNo = re.SOSysNo
		   
		                           UNION ALL 
		   
		                           SELECT DISTINCT
		                                  so.WareHouseNumber
		                                 ,ite.SettlementSysNo
		                           FROM OverseaPOASNManagement.dbo.CollectionSettlement_Item ite WITH(NOLOCK)
		                           INNER JOIN   OverseaOrderManagement.dbo.V_OM_Sub_SO_Master so WITH(nolock)
		                           ON so.SOSysNo = ite.ReferenceSysNo AND ite.ReferenceType = 'SO'
                                  )wh
                                  WHERE wh.WareHouseNumber = @WareHouseNumber AND Settle.SysNo = wh.SettlementSysNo";


            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorSettleGather");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "Settle.[SysNo]  DESC"))
            {
                if (pagingInfo != null)
                {


                    if (queryFilter.CreateDateFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.Indate", DbType.DateTime,
                       "@CreateDateFrom", QueryConditionOperatorType.MoreThan, queryFilter.CreateDateFrom.Value);
                    }
                    if (queryFilter.CreateDateTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.Indate", DbType.DateTime,
                        "@CreateDateTo", QueryConditionOperatorType.LessThan, queryFilter.CreateDateTo.Value.AddDays(1));
                    }

                    ////////////////////audit date
                    if (queryFilter.AuditDateFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.AuditDate", DbType.DateTime,
                       "@AuditDataFrom", QueryConditionOperatorType.MoreThan, queryFilter.AuditDateFrom.Value);
                    }
                    if (queryFilter.AuditDateTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.AuditDate", DbType.DateTime,
                        "@AuditDataTo", QueryConditionOperatorType.LessThan, queryFilter.AuditDateTo.Value.AddDays(1));
                    }
                    ////////////////
                    if (queryFilter.SettledDateFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.SettleDate", DbType.DateTime,
                        "@SettledDateFrom", QueryConditionOperatorType.MoreThan, queryFilter.SettledDateFrom.Value);
                    }
                    if (queryFilter.SettledDateTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.SettleDate", DbType.DateTime,
                        "@SettledDateTo", QueryConditionOperatorType.LessThan, queryFilter.SettledDateTo.Value.AddDays(1));
                    }
                    if (!string.IsNullOrEmpty(queryFilter.CreateUser))
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.InUser", DbType.String,
                        "@CreateUser", QueryConditionOperatorType.LeftLike, queryFilter.CreateUser);
                    }
                    if (!string.IsNullOrEmpty(queryFilter.SettleID))
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.SysNo", DbType.Int32,
                       "@SysNo", QueryConditionOperatorType.Equal, int.Parse(queryFilter.SettleID));
                    }
                    if (queryFilter.VendorSysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.MerchantSysNo", DbType.Int32,
                       "@MerchantSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                    }
                    if (queryFilter.Status != null && queryFilter.Status.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.Status", DbType.String,
                      "@Status", QueryConditionOperatorType.Equal, queryFilter.Status.Value.ToString());
                    }
                    if (!string.IsNullOrEmpty(queryFilter.Memo))
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.Memo", DbType.String,
                            "@Memo", QueryConditionOperatorType.LeftLike, queryFilter.Memo);
                    }
                    if (queryFilter.PaySettleCompany.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.PaySettleCompany", DbType.Int32,
                            "@PaySettleCompany", QueryConditionOperatorType.Equal, queryFilter.PaySettleCompany.Value);
                    }
                    if (!string.IsNullOrEmpty(queryFilter.IsAutoSettle))
                    {
                        if (queryFilter.IsAutoSettle == "Auto")
                        {
                            builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.InUser", DbType.String,
                            "@InUser", QueryConditionOperatorType.Equal, @"System\bitkoo\sys[8601]");
                        }
                        else
                        {
                            builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.InUser", DbType.String,
                            "@InUser", QueryConditionOperatorType.NotEqual, @"System\bitkoo\sys[8601]");
                        }
                    }
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                }
                command.CommandText = builder.BuildQuerySql();
                if (queryFilter.StockSysNo.HasValue)
                {
                    wherhoseSql = wherhoseSql.Replace("@WareHouseNumber", queryFilter.StockSysNo.Value.ToString());
                    command.CommandText = command.CommandText.Replace("##WareHouseNumber##", wherhoseSql);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace("##WareHouseNumber##", "");
                }

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(GatherSettleStatus));
                enumList.Add("PaySettleCompany", typeof(PaySettleCompany));

                dt = command.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["CreateUser"] = (dt.Rows[i]["CreateTime"] == null || dt.Rows[i]["CreateTime"] == DBNull.Value) ? string.Empty : string.Format("{0}[{1}]", dt.Rows[i]["CreateUser"].ToString(), dt.Rows[i]["CreateTime"].ToString());
                        dt.Rows[i]["AuditUser"] = (dt.Rows[i]["AuditTime"] == null || dt.Rows[i]["AuditTime"] == DBNull.Value) ? string.Empty : string.Format("{0}[{1}]", dt.Rows[i]["AuditUser"].ToString(), dt.Rows[i]["AuditTime"].ToString());
                        dt.Rows[i]["SettleUser"] = (dt.Rows[i]["SettleTime"] == null || dt.Rows[i]["SettleTime"] == DBNull.Value) ? string.Empty : string.Format("{0}[{1}]", dt.Rows[i]["SettleUser"].ToString(), dt.Rows[i]["SettleTime"].ToString());
                    }
                }
            }
            return dt;
        }

        public System.Data.DataTable QuerySettleList(QueryFilter.PO.SettleQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = -1;
                return null;
            }
            CustomDataCommand dataCommand = null;

            dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySettleList");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "SysNo desc"))
            {

                if (queryFilter.SettleSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.SettleSysNo);
                }

                if (queryFilter.VendorSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VendorSysNo",
                    DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo.Value);
                }

                if (queryFilter.Status != null && queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, (int)queryFilter.Status);
                }

                if (queryFilter.CreateTime.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CreateTimeDay",
                    DbType.String, "@CreateTime", QueryConditionOperatorType.Equal, queryFilter.CreateTime.Value.ToString("yyyy-MM-dd"));
                }
                if (queryFilter.AuditTime.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "AuditTimeDay",
                        DbType.String, "@AuditTime", QueryConditionOperatorType.Equal, queryFilter.AuditTime.Value.ToString("yyyy-MM-dd"));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", System.Data.DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);


                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                dataCommand.SetParameterValue("@StartNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                dataCommand.SetParameterValue("@EndNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);

                DataTable dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion
    }
}
