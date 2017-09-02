using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICollectionPaymentQueryDA))]
    public class CollectionPaymentQueryDA : ICollectionPaymentQueryDA
    {
        #region IConsignSettleQueryDA Members

        public System.Data.DataTable QueryCollectionPayment(QueryFilter.PO.CollectionPaymentFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCollVendorSettle");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "Settle.[SysNo]  DESC"))
            {
                if (queryFilter != null)
                {

                    if (queryFilter.CreateDateFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.CreateTime", DbType.DateTime,
                            "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateDateFrom.Value);
                    }

                    if (queryFilter.CreateDateTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.CreateTime", DbType.DateTime,
                        "@CreateDateTo", QueryConditionOperatorType.LessThan, queryFilter.CreateDateTo.Value.AddDays(1));
                    }


                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "UserInfo.DisPlayName", DbType.String,
                        "@DisplayName", QueryConditionOperatorType.Like, queryFilter.CreateUser);

                    if (queryFilter.SettledDateFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.SettleTime", DbType.DateTime,
                            "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.SettledDateFrom.Value);
                    }
                    if (queryFilter.SettledDateTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.SettleTime", DbType.DateTime,
                        "@SettledDateTo", QueryConditionOperatorType.LessThan, queryFilter.SettledDateTo.Value.AddDays(1));
                    }
                    if (queryFilter.PaySettleCompany.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.PaySettleCompany", DbType.Int32,
                            "@PaySettleCompany", QueryConditionOperatorType.Equal, queryFilter.PaySettleCompany.Value);
                    }
                    //是否自动结算
                    if (queryFilter.IsAutoSettle == AutoSettleStatus.Auto.ToString())
                    {
                        builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "Vendor.PayPeriodType", DbType.Int32,
                            new List<int> { 72, 73, 74 });
                    }
                    else if (queryFilter.IsAutoSettle == AutoSettleStatus.Hand.ToString())
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.PayPeriodType", DbType.Int32,
                        "@PayPeriodType", QueryConditionOperatorType.Equal, 71);
                    }

                    /////////////crl17950 代销结算单增加Pm查询条件///////////////////////////
                    if (queryFilter.PMSysno.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.PMSysno", DbType.Int32,
                      "@PMSysno", QueryConditionOperatorType.Equal, queryFilter.PMSysno);
                    }
                    //////////////////////////////////////////////////////////////////////////

                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.SettleID", DbType.String,
                        "@SettleID", QueryConditionOperatorType.Equal, queryFilter.SettleID);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.StockSysNo", DbType.Int32,
                        "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                    //builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.VendorName", DbType.String,
                    //    "@VendorName", QueryConditionOperatorType.Like, query.Condition.VendorName);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.VendorSysNo", DbType.Int32,
                        "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.Status", DbType.Int32,
                        "@Status", QueryConditionOperatorType.Equal,  queryFilter.Status);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.Memo", DbType.String,
                        "@Memo", QueryConditionOperatorType.Like,  queryFilter.Memo);
                }

               

                //builder.ConditionConstructor.AddInCondition(
                //    QueryConditionRelationType.AND, "Settle.PMSysno", DbType.Int32, AuthorizedPMs());

               // ProviderHelper.SetCommonParams(builder, "Settle");


                command.CommandText = builder.BuildQuerySql();

                EnumColumnList colList = new EnumColumnList();
                colList.Add("PaySettleCompany", typeof(PaySettleCompany));
                //{
                //    { 19, typeof(SettleStatus) }
                    
                //};
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                codeNameColList.Add("PayPeriodType", "Invoice", "VendorPayPeriod");
                //colList.Add("PayPeriodType", typeof(VendorPayPeriodType));
                colList.Add("Status", typeof(POCollectionPaymentSettleStatus));
                colList.Add("IsConsign", typeof(PurchaseOrderConsignFlag));
                
                dt = command.ExecuteDataTable(colList,codeNameColList);

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["CreateUser"] = (dt.Rows[i]["CreateTime"] == null || dt.Rows[i]["CreateTime"] == DBNull.Value) ? string.Empty : string.Format("{0}[{1}]", dt.Rows[i]["CreateUser"].ToString(), dt.Rows[i]["CreateTime"].ToString());
                        dt.Rows[i]["AuditUser"] = (dt.Rows[i]["AuditTime"] == null || dt.Rows[i]["AuditTime"] == DBNull.Value) ? string.Empty : string.Format("{0}[{1}]", dt.Rows[i]["AuditUser"].ToString(), dt.Rows[i]["AuditTime"].ToString());
                        dt.Rows[i]["SettleUser"] = (dt.Rows[i]["SettleTime"] == null || dt.Rows[i]["SettleTime"] == DBNull.Value) ? string.Empty : string.Format("{0}[{1}]", dt.Rows[i]["SettleUser"].ToString(), dt.Rows[i]["SettleTime"].ToString());
                    }
                }
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            }
            return dt;

        }

        #endregion
    }
}

