using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IFinancialReportDA))]
    public class FinancialReportDA : IFinancialReportDA
    {
        public DataSet IncomeCostReportQuery(IncomeCostReportQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter.PagingInfo);
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("FinancialReport.IncomeCostReportQuery");
            DataSet result = null;
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "OrderDate DESC"))
            {
                #region Set dynamic codition for where
                if (filter.SODateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "OrderDate",
                        DbType.DateTime,
                        "@OrderDateFrom_query",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.SODateFrom.Value);
                }

                if (filter.SODateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "OrderDate",
                        DbType.DateTime,
                        "@SODateTo_query",
                        QueryConditionOperatorType.LessThan,
                        filter.SODateTo.Value);
                }

                if (filter.PayTypeSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "PayTypeSysNo",
                        DbType.Int32,
                        "@PayTypeSysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.PayTypeSysNo.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.ChannelID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "ChannelID",
                        DbType.AnsiStringFixedLength,
                        "@ChannelID_query",
                        QueryConditionOperatorType.Equal,
                        filter.ChannelID);
                }

                if (filter.SOSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "SOSysNo",
                        DbType.Int32,
                        "@SOSysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.SOSysNo.Value);
                }

                if (filter.SOStatusList != null && filter.SOStatusList.Count > 0)
                {
                    sqlBuilder.ConditionConstructor.AddInCondition<int>(
                        QueryConditionRelationType.AND,
                        "SOStatus",
                        System.Data.DbType.Int32,
                        filter.SOStatusList);
                }
                if (filter.VendorSysNoList != null && filter.VendorSysNoList.Count > 0)
                {
                    sqlBuilder.ConditionConstructor.AddInCondition(
                        QueryConditionRelationType.AND,
                        "VendorSysNo",
                        DbType.Int32,
                        filter.VendorSysNoList);
                }

                #endregion

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                result = dataCommand.ExecuteDataSet();
                dataCommand.ConvertEnumColumn(result.Tables[0], 18, typeof(SOStatus));
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return result;
        }


        /// <summary>
        /// Table[0]: Result,
        /// Table[1]: Statistics
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataSet SalesStatisticsReportQuery(SalesStatisticsReportQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter.PagingInfo);
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("FinancialReport.SalesStatisticsReportQuery");
            DataSet result = null;
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "B.ProductID DESC, A.PayTypeSysNo"))
            {
                #region Set dynamic codition for where

                if (filter.SOStatusList != null && filter.SOStatusList.Count > 0)
                {
                    sqlBuilder.ConditionConstructor.AddInCondition<int>(
                        QueryConditionRelationType.AND,
                        "SOStatus",
                        System.Data.DbType.Int32,
                        filter.SOStatusList);
                }

                if (filter.SODateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.OrderDate",
                        DbType.DateTime,
                        "@OrderDateFrom_query",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.SODateFrom.Value);
                }

                if (filter.SODateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.OrderDate",
                        DbType.DateTime,
                        "@SODateTo_query",
                        QueryConditionOperatorType.LessThan,
                        filter.SODateTo.Value);
                }


                if (filter.C1SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.C1SysNo",
                        DbType.Int32,
                        "@C1SysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.C1SysNo);
                }

                if (filter.C2SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.C2SysNo",
                        DbType.Int32,
                        "@C2SysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.C2SysNo);
                }

                if (filter.C3SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.C3SysNo",
                        DbType.Int32,
                        "@C3SysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.C3SysNo);
                }

                if (!string.IsNullOrWhiteSpace(filter.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.ProductID",
                        DbType.StringFixedLength,
                        "@ProductID_query",
                        QueryConditionOperatorType.Equal,
                        filter.ProductID);
                }
                if (!string.IsNullOrWhiteSpace(filter.BrandName))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format(" (brand.BrandName_Ch = N'{0}' OR brand.BrandName_En = N'{0}') ", filter.BrandName.Replace("'", "''")));
                }

                sqlBuilder.ConditionConstructor.AddInCondition(
                       QueryConditionRelationType.AND,
                       "rpt.VendorSysNo",
                       DbType.Int32,
                       filter.VendorSysNoList);

                sqlBuilder.ConditionConstructor.AddInCondition(
                       QueryConditionRelationType.AND,
                       "rpt.WarehouseNumber",
                       DbType.StringFixedLength,
                       filter.WarehouseNumberList);

                #endregion

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                result = dataCommand.ExecuteDataSet();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return result;
        }


        /// <summary>
        /// 构造PagingInfo对象
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        private PagingInfoEntity CreatePagingInfo(ECCentral.QueryFilter.Common.PagingInfo pageInfo)
        {
            var pagingInfo = new PagingInfoEntity();
            if (pageInfo != null)
            {
                pagingInfo.MaximumRows = pageInfo.PageSize;
                pagingInfo.StartRowIndex = pageInfo.PageIndex * pageInfo.PageSize;
                pagingInfo.SortField = pageInfo.SortBy;
            }
            return pagingInfo;
        }

        public DataSet CouponUseedReportQuery(CouponUsedReportFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter.PagingInfo);
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("FinancialReport.CouponUsedReportQuery");
            DataSet result = null;
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "OrderDate DESC"))
            {
                #region Set dynamic codition for where
                if (filter.SODateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "OrderDate",
                        DbType.DateTime,
                        "@OrderDateFrom_query",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.SODateFrom.Value);
                }

                if (filter.SODateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "OrderDate",
                        DbType.DateTime,
                        "@SODateTo_query",
                        QueryConditionOperatorType.LessThan,
                        filter.SODateTo.Value);
                }

                if (filter.PayTypeSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "PayTypeSysNo",
                        DbType.Int32,
                        "@PayTypeSysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.PayTypeSysNo.Value);
                }

                if (filter.SoSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "SOSysNo",
                        DbType.Int32,
                        "@SOSysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.SoSysNo.Value);
                }
                if (filter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "Status",
                        DbType.Int32,
                        "@Status",
                        QueryConditionOperatorType.Equal,
                        filter.Status.Value);
                }
                if (filter.SOPayStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "SOPayStatus",
                        DbType.Int32,
                        "@SOPayStatus",
                        QueryConditionOperatorType.Equal,
                        filter.SOPayStatus.Value);
                }
                if (filter.MerchantSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "VendorSysNo",
                        DbType.Int32,
                        "@MerchantSysNo",
                        QueryConditionOperatorType.Equal,
                        filter.MerchantSysNo.Value);
                }
                if (filter.CouponSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CouponSysNo",
                        DbType.Int32,
                        "@CouponSysNo",
                        QueryConditionOperatorType.Equal,
                        filter.CouponSysNo.Value);
                }
                #endregion
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("Status", typeof(SOStatus));
                enumColList.Add("NetPayStatus", typeof(ECCentral.BizEntity.Invoice.NetPayStatus));
                enumColList.Add("SOIncomeStatus", typeof(ECCentral.BizEntity.Invoice.SOIncomeStatus));
                enumColList.Add("SOPayStatus", typeof(ECCentral.BizEntity.Invoice.SOIncomeStatus));
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                result = dataCommand.ExecuteDataSet();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                if (result.Tables[0] != null && result.Tables[0].Rows.Count > 0)
                {
                    result.Tables[0].Columns.Add("SOIncomeStatusText", typeof(string));
                    foreach (DataRow dr in result.Tables[0].Rows)
                    {
                        //int ippStatus = dr["Status"] != null && dr["Status"] != DBNull.Value ? (int)dr["Status"] : int.MinValue;
                        //if (ippStatus != int.MinValue)
                        //{
                        //    int isAutoRMA = dr["HaveAutoRMA"] != null && dr["HaveAutoRMA"] != DBNull.Value ? (int)dr["HaveAutoRMA"] : 0;

                        //    int isCombine = dr["IsCombine"] != null && dr["IsCombine"] != DBNull.Value ? (int)dr["IsCombine"] : 0;
                        //    int isMergeComplete = dr["IsMergeComplete"] != null && dr["IsMergeComplete"] != DBNull.Value ? (int)dr["IsMergeComplete"] : 0;
                        //    SOStatus status = Mapping_SOStatus_IPPToThis(ippStatus, isAutoRMA != 0, isCombine == 1, isMergeComplete == 1);
                        //    dr["Status"] = (int)status;
                        //}
                        string soIncomeStatusText = string.Empty;    
                        // Set column value for SOIncomeStatusText
                        SOIncomeStatus? soIncomeStatus = dr.IsNull("SOPayStatus") ? new SOIncomeStatus?() : (SOIncomeStatus)dr["SOPayStatus"];
                        if ((int)dr["SOPayStatus"] == -1)
                        {
                            soIncomeStatusText = ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__NotPay;
                        }
                        //NetPayStatus? netPayStatus = dr.IsNull("SOPayStatus") ? new NetPayStatus?() : (NetPayStatus)dr["SOPayStatus"];
                        else if ((int)dr["SOPayStatus"] == -999)
                        {
                            soIncomeStatusText = ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__Paied;
                        }
                        else
                        {
                            soIncomeStatusText = EnumHelper.GetDescription(soIncomeStatus.Value);
                        }
                       
                        dr["SOIncomeStatusText"] = soIncomeStatusText;
                    }
                }

                dataCommand.ConvertEnumColumn(result.Tables[0], enumColList);
            }
            return result;
        }

        /// <summary>
        ///  IPP的订单状态Mapping到现在的订单状态
        /// </summary>
        /// <param name="ippStatus"></param>
        /// <returns></returns>
        internal static SOStatus Mapping_SOStatus_IPPToThis(int ippStatus, bool isAutoRMA, bool isCombine, bool isMergeComplete)
        {
            SOStatus status;
            switch (ippStatus)
            {
                case -6://已拆分
                    status = SOStatus.Split;
                    break;
                case -5://BackOrder
                    status = SOStatus.Origin;
                    break;
                case -4://
                    status = SOStatus.SystemCancel;
                    break;
                case -1://
                    status = SOStatus.Abandon;
                    break;
                case 0://待审核
                    status = SOStatus.Origin;
                    break;
                case 1://待出库
                    status = SOStatus.WaitingOutStock;
                    break;
                case 2://待支付
                    status = SOStatus.Origin;
                    break;
                case 3://待主管审核
                    status = SOStatus.WaitingManagerAudit;
                    break;
                case 4://已出库
                    //if (isAutoRMA)
                    //{
                    //    status = SOStatus.Reject;
                    //}
                    //else if (isCombine && !isMergeComplete)
                    //{
                    //    status = SOStatus.Shipping;
                    //}
                    //else
                    //{
                    status = SOStatus.OutStock;
                    //}
                    break;
                case 5://已完成
                    status = SOStatus.Complete;
                    break;
                case 41://已报关
                    status = SOStatus.Reported;
                    break;
                case 45://已通关发往顾客
                    status = SOStatus.CustomsPass;
                    break;
                case 6://申报失败订单作废
                    status = SOStatus.Reject;
                    break;
                case 65://通关失败订单作废
                    status = SOStatus.CustomsReject;
                    break;
                case 7://物流作废
                    status = SOStatus.ShippingReject;
                    break;
                default://已作废
                    status = SOStatus.Abandon;
                    break;
            }
            return status;

        }
    }
}