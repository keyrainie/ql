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
    [VersionExport(typeof(IConsignSettleQueryDA))]
    public class ConsignSettleQueryDA : IConsignSettleQueryDA
    {
        #region IConsignSettleQueryDA Members

        public System.Data.DataTable QueryConsignSettlement(QueryFilter.PO.ConsignSettleQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorSettle");
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
                            new List<int> { 29, 30, 31 });
                    }
                    else if (queryFilter.IsAutoSettle == AutoSettleStatus.Hand.ToString())
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.PayPeriodType", DbType.Int32,
                        "@PayPeriodType", QueryConditionOperatorType.Equal, 12);
                    }

                    if (!(queryFilter.IsManagerPM??false))
                    {
                        string sqlStr = @"Select 
									ProductLineSysNo 
                FROM OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p " +
                "WHERE  PMUserSysNo=" + ServiceContext.Current.UserSysNo + " OR CHARINDEX(';'+CAST(" + ServiceContext.Current.UserSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0";
                        builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "Settle.ProductLineSysNo", QueryConditionOperatorType.In, sqlStr);
                    }
                    if (queryFilter.IsLease.HasValue)
                    {
                         builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.IsToLease", DbType.AnsiStringFixedLength,
                        "@IsToLease", QueryConditionOperatorType.Equal, (int)queryFilter.IsLease.Value);
                    }
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.PMSysno", DbType.AnsiStringFixedLength,
                        "@PMSysno", QueryConditionOperatorType.Equal, queryFilter.PMSysno);

                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.SettleID", DbType.String,
                        "@SettleID", QueryConditionOperatorType.Equal, queryFilter.SettleID);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.StockSysNo", DbType.Int32,
                        "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.VendorName", DbType.String,
                        "@VendorName", QueryConditionOperatorType.Like, queryFilter.VendorName);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.VendorSysNo", DbType.Int32,
                        "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.Status", DbType.Int32,
                        "@Status", QueryConditionOperatorType.Equal, (null == queryFilter.Status ? (int?)null : (int)queryFilter.Status.Value));
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.Memo", DbType.String,
                        "@Memo", QueryConditionOperatorType.Like, queryFilter.Memo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.ConsignRange", DbType.String,
                        "@ConsignRange", QueryConditionOperatorType.Equal, queryFilter.ConsignRange);
                }

                ////TODO:取有权限的PMList SysNo集合
                //builder.ConditionConstructor.AddInCondition(
                //    QueryConditionRelationType.AND, "Settle.PMSysno", DbType.Int32, queryFilter.AccessiblePMSysNo);
                
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Settle.CompanyCode", DbType.AnsiStringFixedLength,
                        "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList colList = new EnumColumnList
                {
                    { 21, typeof(SettleStatus) },
                    { 25, typeof(VendorConsignFlag) },
                    { 28, typeof(PaySettleCompany) }
                };
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                codeNameColList.Add("PayPeriodType", "Invoice", "VendorPayPeriod");
                //colList.Add("PayPeriodType", typeof(VendorPayPeriodType));
                dt = command.ExecuteDataTable(colList,codeNameColList);

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

        public DataTable GetConsignSettlmentProductList(QueryFilter.PO.ConsignSettlementProductsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                 SortField = filter.PageInfo.SortBy,
                StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize,
                MaximumRows = filter.PageInfo.PageSize
            };

            DataTable dt = new DataTable();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryConsignSettleItemsNew");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "Product.ProductID DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.Status", DbType.String,
                    "@ConsignStatus", QueryConditionOperatorType.Equal, 0);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CompanyCode", DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);


                if (filter != null)
                {
                    if (filter.CreateDateFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CreateTime", DbType.DateTime,
                            "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                    }
                    if (filter.CreateDateTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CreateTime", DbType.DateTime,
                      "@CreateTimTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo.Value.AddDays(1));
                    }
                    if (filter.ProductSysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.SysNo", DbType.Int32,
                            "@ProductSysNo", QueryConditionOperatorType.Equal, filter.ProductSysNo);
                    }
                    if (filter.StockSysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.StockSysNo", DbType.Int32,
                            "@StockSysNo", QueryConditionOperatorType.Equal, filter.StockSysNo);
                    }
                    if (filter.VendorSysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.VendorSysNo", DbType.Int32,
                            "@VendorSysNo", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                    }
                    /////////////CRL17950新加条件  prince 2011-1-11//////////////////////////////////////
                    if (filter.Category1SysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "catrgory.Category1Sysno", DbType.Int32,
                           "@Category1Sysno", QueryConditionOperatorType.Equal, filter.Category1SysNo);
                    }
                    if (filter.Category2SysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "catrgory.Category2Sysno", DbType.Int32,
                            "@Category2Sysno", QueryConditionOperatorType.Equal, filter.Category2SysNo);
                    }
                    if (filter.Category3SysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "catrgory.Category3Sysno", DbType.Int32,
                          "@Category3Sysno", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                    }
                    if (filter.BrandSysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.BrandSysNo", DbType.Int32,
                          "@BrandSysNo", QueryConditionOperatorType.Equal, filter.BrandSysNo);
                    }

                    if (filter.PMSysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.PMUserSysNo", DbType.Int32,
                          "@PMUserSysNo", QueryConditionOperatorType.Equal, filter.PMSysNo.Value);
                    }
                    else
                    {
                        //TODO: 获取PM List: 
                        builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "Product.PMUserSysNo", DbType.Int32, filter.PMList);
                    }
                    ////////////////////////////////////////////////////////////////////////////////////
                    //区分业务模式 2012.1127
                    if(filter.IsConsign.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.IsConsign", DbType.Int32,
                         "@IsConsign", QueryConditionOperatorType.Equal, filter.IsConsign.Value);
                    }

                    builder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " Consign.Status = 0 ");
                }
                 
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.CompanyCode", DbType.AnsiStringFixedLength,
                 "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList list = new EnumColumnList();
                list.Add("ConsignToAccLogInfo.ConsignToAccStatus", typeof(ConsignToAccountLogStatus));
                list.Add("SettleType", typeof(SettleType));
                dt = command.ExecuteDataTable(list);
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion

        /// <summary>
        /// 经销商品结算单查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QuerySettleAccountWithOrigin(QueryFilter.PO.SettleOrderCreateQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PageInfo.SortBy,
                StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize,
                MaximumRows = filter.PageInfo.PageSize
            };

            DataTable dt = new DataTable();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSettleAccountWithOrigin");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "InDate desc"))
            {
                var flag = 0;//默认情况查询所有数据
                if (filter.POPositive.HasValue && filter.POPositive.Value == true)
                {
                    flag= flag == 0 ? 1 : flag;
                    flag = flag * 3;
                }
                if (filter.PONegative.HasValue && filter.PONegative.Value == true)
                {
                    flag = flag == 0 ? 1 : flag;
                    flag = flag * 5;
                }
                if (filter.ChangePrice.HasValue && filter.ChangePrice.Value == true)
                {
                    flag = flag == 0 ? 1 : flag;
                    flag = flag * 7;
                }

                command.AddInputParameter("@flag", DbType.Int32, flag);

                if (filter.VendorSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.SysNo", DbType.Int32,
                                            "@VendorSysNo", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                }

                if (filter.CreateDateFrom.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.Date,
                                                               "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                }
                if (filter.CreateDateTo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.Date,
                                                               "@InDateTo", QueryConditionOperatorType.LessThanOrEqual, filter.CreateDateTo);
                }

                if (filter.OrderSysNoList != null && filter.OrderSysNoList.Count > 0)
                {
                    List<object> list = new List<object>();
                    filter.OrderSysNoList.ForEach(p =>
                    {
                        list.Add(p);
                    });
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderSysNo", DbType.Int32,
                                                              "@OrderSysNoList", QueryConditionOperatorType.In, list);
                }

                command.CommandText = builder.BuildQuerySql();


                //EnumColumnList list = new EnumColumnList();
                //list.Add("ConsignToAccLogInfo.ConsignToAccStatus", typeof(ConsignToAccountLogStatus));
                //list.Add("SettleType", typeof(SettleType));
                dt = command.ExecuteDataTable();
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }

           
        }
    }
}
