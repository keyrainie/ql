using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ITrackingInfoQueryDA))]
    public class TrackingInfoQueryDA : ITrackingInfoQueryDA
    {
        #region ITrackingInfoQueryDA Members

        public DataSet QueryTrackingInfo(TrackingInfoQueryFilter filter, out int totalCount)
        {
            DataSet result = null;
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter.PagingInfo);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryTrackingInfo");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "Temp.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderSysNo))
                {
                    var orderSysNoList = filter.OrderSysNo.Split('.').ToList()
                    .ConvertAll(x => Convert.ToInt32(x.Trim()));

                    builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "Temp.OrderSysNo", DbType.Int32, orderSysNoList);
                }

                var statusList = new List<string>();
                //业务跟进
                if (filter.HasStatusFollow)
                {
                    statusList.Add("A");
                }
                //提交报损
                if (filter.HasStatusSubmit)
                {
                    statusList.Add("S");
                }
                //核销完毕
                if (filter.HasStatusConfirm)
                {
                    statusList.Add("C");
                }

                //部分选择时再添加该条件
                if (statusList.Count > 0 && statusList.Count < 3)
                {
                    builder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "Temp.Status", DbType.AnsiStringFixedLength, statusList);
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.IncomeStyle", DbType.Int32, "@IncomeStyle", QueryConditionOperatorType.Equal, filter.IncomeStyle);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.PayTypeSysNo", DbType.Int32, "@PayTypeSysNo", QueryConditionOperatorType.Equal, filter.PayTypeSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.OutDate", DbType.DateTime, "@OutFromDate", QueryConditionOperatorType.MoreThanOrEqual, filter.OutFromDate);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.OutDate", DbType.DateTime, "@OutToDate", QueryConditionOperatorType.LessThanOrEqual, filter.OutToDate);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.ShipTypeSysNo", DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, filter.ShipTypeSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.LossType", DbType.Int32, "@LossType", QueryConditionOperatorType.Equal, filter.LossType);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.ResponsibleUserName", DbType.String, "@ResponsibleUserName", QueryConditionOperatorType.Equal, filter.ResponsibleUserName);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.InType", DbType.Int32, "@InType", QueryConditionOperatorType.Equal, filter.InType);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Temp.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.AddOutParameter("@OrderAmt", DbType.Double, 12);
                cmd.AddOutParameter("@PrepayAmt", DbType.Double, 12);
                cmd.AddOutParameter("@GiftCardPayAmt", DbType.Double, 12);
                cmd.AddOutParameter("@IncomeAmt", DbType.Double, 12);
                cmd.AddOutParameter("@UnpayedAmt", DbType.Double, 12);

                cmd.CommandText = builder.BuildQuerySql();

                result = ExecuteDataCommandForTrackingInfo(cmd, out totalCount);
            }
            return result;
        }

        public DataTable QueryResponsibleUser(ResponsibleUserQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter.PagingInfo);
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryResponsibleUser");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "sr.SysNo desc"))
            {
                if (filter.IsSpecialMode == false)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.IsPayWhenRecv", DbType.Int32, "@IncomeStyle", QueryConditionOperatorType.Equal, filter.IncomeStyle);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.PayTypeSysNo", DbType.Int32, "@PayTypeSysNo", QueryConditionOperatorType.Equal, filter.PayTypeSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.ShipTypeSysNo", DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, filter.ShipTypeSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.ResponsibleUserName", DbType.String, "@ResponsibleUserName", QueryConditionOperatorType.LeftLike, filter.ResponsibleUserName);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.EmailAddress", DbType.String, "@EmailAddress", QueryConditionOperatorType.Equal, filter.EmailAddress);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.SourceType", DbType.Int32, "@SrouceType", QueryConditionOperatorType.Equal, filter.SourceType);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                }
                else
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.IsPayWhenRecv", DbType.Int32, "@IsPayWhenRecv", QueryConditionOperatorType.Equal, filter.IncomeStyle);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                    builder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND, "sr.PayTypeSysNo", QueryConditionOperatorType.IsNull);
                    builder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND, "sr.ShipTypeSysNo", QueryConditionOperatorType.IsNull);
                    builder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND, "sr.CustomerSysNo", QueryConditionOperatorType.IsNull);
                }

                dataCommand.CommandText = builder.BuildQuerySql();

                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("IncomeStyle", typeof(ResponseUserOrderStyle));
                enumColumns.Add("Status", typeof(ResponseUserStatus));
                enumColumns.Add("SourceType", typeof(ResponsibleSource));

                result = dataCommand.ExecuteDataTable(enumColumns);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return result;
        }

        public DataTable QueryOrder(OrderQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryOrder");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, null, "OrderSysNo desc"))
            {
                var intList = filter.OrderSysNo.Split('.').ToList()
                    .ConvertAll(x => Convert.ToInt32(x.Trim()));

                builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "OrderSysNo", DbType.String, intList);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                cmd.CommandText = builder.BuildQuerySql();
            }

            var result = cmd.ExecuteDataTable();
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

            return result;
        }

        /// <summary>
        /// 加载逾期收款单责任人列表
        /// </summary>
        /// <returns></returns>
        public List<CodeNamePair> GetResponsibleUsers(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryResponsibleUserNames");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<ResponsibleUserInfo>().ConvertAll<CodeNamePair>(user =>
            {
                string name = user.ResponsibleUserName;
                if (user.Status == ResponseUserStatus.Dective)
                {
                    //如是责任人无效，则需要在显示名称后加（无效）
                    name = string.Format("{0} ({1})", user.ResponsibleUserName, ResponseUserStatus.Dective.ToDisplayText());
                }
                return new CodeNamePair()
                {
                    Code = user.ResponsibleUserName,
                    Name = name
                };
            });
        }

        #endregion ITrackingInfoQueryDA Members

        private DataSet ExecuteDataCommandForTrackingInfo(CustomDataCommand dataCommand, out int totalCount)
        {
            DataSet ds = new DataSet();

            //注册CodeNamePair类型
            CodeNamePairColumnList codeNamePairColumnList = new CodeNamePairColumnList();
            codeNamePairColumnList.Add("LossType", "Invoice", "TrackingInfoLoseType", "LossTypeID");

            EnumColumnList enumColumnList = new EnumColumnList();
            enumColumnList.Add("Status", typeof(TrackingInfoStatus));
            enumColumnList.Add("OrderType", typeof(SOIncomeOrderType));
            enumColumnList.Add("Source", typeof(NetPaySource));
            enumColumnList.Add("OrderStatus", typeof(SOIncomeStatus));
            enumColumnList.Add("IncomeStyle", typeof(SOIncomeOrderStyle));
            enumColumnList.Add("InType", typeof(TrackingInfoStyle));

            DataTable resultDT = dataCommand.ExecuteDataTable(enumColumnList, codeNamePairColumnList);
            resultDT.TableName = "DataResult";
            ds.Tables.Add(resultDT);

            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            #region 构造统计信息

            DataTable statisticDT = new DataTable("StatisticResult");
            statisticDT.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("OrderAmt",typeof(decimal)),         /*  订单金额   */
                new DataColumn("PrepayAmt",typeof(decimal)),        /*  预收金额   */
                new DataColumn("GiftCardPayAmt",typeof(decimal)),   /*  礼品卡金额 */
                new DataColumn("IncomeAmt",typeof(decimal)),        /*  实收金额   */
                new DataColumn("UnpayedAmt",typeof(decimal))        /*  未收金额   */
            });

            DataRow row = statisticDT.NewRow();
            statisticDT.Rows.Add(row);
            row["OrderAmt"] = dataCommand.GetParameterValue("@OrderAmt");
            row["PrepayAmt"] = dataCommand.GetParameterValue("@PrepayAmt");
            row["GiftCardPayAmt"] = dataCommand.GetParameterValue("@GiftCardPayAmt");
            row["IncomeAmt"] = dataCommand.GetParameterValue("@IncomeAmt");
            row["UnpayedAmt"] = dataCommand.GetParameterValue("@UnpayedAmt");

            ds.Tables.Add(statisticDT);

            #endregion 构造统计信息

            return ds;
        }

        private PagingInfoEntity CreatePagingInfo(ECCentral.QueryFilter.Common.PagingInfo pagingInfo)
        {
            PagingInfoEntity pagingInfoEntity = new PagingInfoEntity();
            if (pagingInfoEntity != null)
            {
                pagingInfoEntity.MaximumRows = pagingInfo.PageSize;
                pagingInfoEntity.StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize;
                pagingInfoEntity.SortField = pagingInfo.SortBy;
            }
            return pagingInfoEntity;
        }
    }
}