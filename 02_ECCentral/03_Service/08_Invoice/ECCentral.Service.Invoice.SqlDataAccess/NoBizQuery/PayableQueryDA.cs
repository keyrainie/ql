using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPayableQueryDA))]
    public class PayableQueryDA : IPayableQueryDA
    {
        #region IPayableQueryDA Members

        public virtual DataTable QueryPayable(PayableQueryFilter filter, out int totalCount, out DataTable dtStatistical)
        {
            switch (filter.OrderType)
            {
                case null:
                    return GetAllPayList(filter, out  totalCount, out dtStatistical);
                case PayableOrderType.PO:
               // case PayableOrderType.POTempConsign:
                case PayableOrderType.POAdjust:
                    return GetPOPayList(filter, out  totalCount, out dtStatistical);
                case PayableOrderType.VendorSettleOrder:
                    return GetVendorSettleOrderPayList(filter, out  totalCount, out dtStatistical);
                //case PayableOrderType.SubAccount:
                //case PayableOrderType.ReturnPointCashAdjust:
                //case PayableOrderType.SubInvoice:
                //    return GetReturnPointCashSettlePayList(filter, out  totalCount, out dtStatistical);
                case PayableOrderType.RMAPOR:
                    return GetRMAPORPayList(filter, out  totalCount, out dtStatistical);
                case PayableOrderType.CollectionSettlement:
                    return GetCollectionSettlementPayList(filter, out  totalCount, out dtStatistical);
                case PayableOrderType.Commission:
                    return GetCommissionPayList(filter, out  totalCount, out dtStatistical);
                case PayableOrderType.CollectionPayment:
                    return GetCollectionPayment(filter, out  totalCount, out dtStatistical);
                case PayableOrderType.CostChange:
                    return GetCCPayList(filter, out  totalCount, out dtStatistical);
                default:
                    return GetCommonOrderPayList(filter, out  totalCount, out dtStatistical);
            }
        }

        public List<CodeNamePair> GetAllVendorPayTerms(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllVendorPayTerms");
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<CodeNamePair>();
        }

        #endregion IPayableQueryDA Members
        /// <summary>
        /// 财务通用查询，需要在OverseaInvoiceReceiptManagement.dbo.V_PayBizAdapter视图中添加适配业务表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <param name="dtStatistical"></param>
        /// <returns></returns>
        private DataTable GetCommonOrderPayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);
            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommonOrderPayList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region Condition

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                   DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "Biz.SysNo", DbType.Int32, orderIDList);
                }

                //仅负单据
                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderAmt", DbType.Decimal, "@OrderAmt", QueryConditionOperatorType.LessThan, 0);
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                //付款结算公司
                //if (query.PaySettleCompany.HasValue)
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                //     DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, query.PaySettleCompany.Value);
                //}

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sysUser.UserSysNo",
                   DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, query.CreatePMSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                 DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "balancesettle.StockSysNo",
                // DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, query.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);
                ////Finance Settle Order
                //if (query.OrderType.Value == PayableOrderType.FinanceSettleOrder)
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "balancesettle.Status",
                //        DbType.Int32, "@BalanceSettleStatus", QueryConditionOperatorType.Equal, query.FinanceSettleOrderStatus);
                //}
                //else//Balance Order
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "balancesettle.Status",
                //     DbType.Int32, "@BalanceSettleStatus", QueryConditionOperatorType.Equal, query.BalanceOrderStatus);
                //}

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                  DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom);
                if (query.POETPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }

                if (query.PayStyle.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(
                        "EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle));
                }
                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion Condition

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
                enumList.Add("OrderStatus", typeof(SettleStatus));
                

                DataTable dt = dataCommand.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetCommissionPayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);
            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommissionPayList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "commission.SysNo", DbType.Int32, orderIDList);
                }

                //仅负单据
                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderAmt", DbType.Decimal, "@OrderAmt", QueryConditionOperatorType.LessThan, 0);
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                //CRL19155:ETP查询条件
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                  DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom);
                if (query.POETPDateTo.HasValue)
                {
                    //query.Condition.POETPDateTo = query.Condition.POETPDateTo.Value.AddHours(10);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }

                //付款结算公司
                if (query.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                     DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, query.PaySettleCompany.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@SettleTimeTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                  DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "commission.Status",
                 DbType.Int32, "@VendorSettleStatus", QueryConditionOperatorType.Equal, query.VendorSettleStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.PayStyle.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(
                        "EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle));
                }
                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
                enumList.Add("OrderStatus", typeof(VendorCommissionMasterStatus));
                

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetCollectionSettlementPayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);
            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCollectionSettlementPayList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region Condition

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "collectionSettlement.SysNo", DbType.Int32, orderIDList);
                }

                //仅负单据
                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderAmt", DbType.Decimal, "@OrderAmt", QueryConditionOperatorType.LessThan, 0);
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                  DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom);
                if (query.POETPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }

                //付款结算公司
                if (query.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                     DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, query.PaySettleCompany.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@SettleTimeTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                  DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "collectionSettlement.StockSysNo",
                 DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, query.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "collectionSettlement.Status",
                 DbType.Int32, "@VendorSettleStatus", QueryConditionOperatorType.Equal, query.VendorSettleStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);
                if (query.PayStyle.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(
                        "EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle));
                }

                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
                enumList.Add("OrderStatus", typeof(PurchaseOrderStatus));
                

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetRMAPORPayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);
            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetRMAPORPayList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region condition

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "RMAPOR.SysNo", DbType.Int32, orderIDList);
                }

                //仅负单据
                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderAmt", DbType.Decimal, "@OrderAmt", QueryConditionOperatorType.LessThan, 0);
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@PMDAuditTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@PMDAuditTimeTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sysUser.UserSysNo",
                   DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, query.CreatePMSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                  DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                  DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom);
                if (query.POETPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }

                //付款结算公司
                if (query.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                     DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, query.PaySettleCompany.Value);
                }

                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
                enumList.Add("OrderStatus", typeof(SettleStatus));

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetReturnPointCashSettlePayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);
            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetReturnPointCashSettlePayList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region Condition

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                     DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "pay.OrderSysNo", DbType.Int32, orderIDList);
                }

                //仅负单据
                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderAmt", DbType.Decimal, "@OrderAmt", QueryConditionOperatorType.LessThan, 0);
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                //付款结算公司
                if (query.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                     DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, query.PaySettleCompany.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@ApproveTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@ApproveTimeTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                  DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                  DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                  DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom);
                if (query.POETPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }

                if (query.PayStyle.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(
                        "EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle));
                }
                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetVendorSettleOrderPayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);
            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetVendorSettleOrderPayList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region condition

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "vendorsettle.SysNo", DbType.Int32, orderIDList);
                }

                //仅负单据
                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderAmt", DbType.Decimal, "@OrderAmt", QueryConditionOperatorType.LessThan, 0);
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                  DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom);
                if (query.POETPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }
                //付款结算公司
                if (query.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                     DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, query.PaySettleCompany.Value);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@SettleTimeTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sysUser.UserSysNo",
                   DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, query.CreatePMSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                  DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendorSettle.StockSysNo",
                 DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, query.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendorSettle.Status",
                 DbType.Int32, "@VendorSettleStatus", QueryConditionOperatorType.Equal, query.VendorSettleStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.PayStyle.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(
                        "EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle));
                }

                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
                enumList.Add("OrderStatus", typeof(SettleStatus));

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                dt.Columns.Add("IsOldConsignSettle", typeof(Boolean));

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        PayableOrderType OrderType;
                        if (!Enum.TryParse(dr["OrderType"].ToString(), out OrderType))
                        {
                          //  OrderType = PayableOrderType.BalanceOrder;
                        }
                        if (OrderType == PayableOrderType.VendorSettleOrder)
                        {
                            dr["IsOldConsignSettle"] = IsOldConsignSettle(Convert.ToInt32(dr["OrderSysNo"]));
                        }
                    }
                }

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetPOPayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);

            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOPayList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region Condition

                //if (query.OrderType.Value == PayableOrderType.POTempConsign)
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.IsConsign",
                //      DbType.Int32, "@IsConsign", QueryConditionOperatorType.Equal, 2);
                //}
                //else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.IsConsign",
                        DbType.Int32, "@IsConsign", QueryConditionOperatorType.NotEqual, 1);
                    //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.IsConsign",
                    //   DbType.Int32, "@UnConsign", QueryConditionOperatorType.Equal, 0);
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "po.IsConsign",
                        DbType.Int32, new List<int>() { 0, 4 });
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "pay.OrderType", DbType.Int32,
                        new List<int>() { (int)PayableOrderType.PO, (int)PayableOrderType.POAdjust });
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                //仅负单据
                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderAmt", DbType.Decimal, "@OrderAmt", QueryConditionOperatorType.LessThan, 0);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "po.SysNo", DbType.Int32, orderIDList);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                    DbType.DateTime, "@InTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@InTimeTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sysUser.UserSysNo",
                   DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, query.CreatePMSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sysPOBelongPMUser.UserSysNo",
                 DbType.Int32, "@POBelongPMID", QueryConditionOperatorType.Equal, query.POBelongPMSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                   DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.Status",
                  DbType.Int32, "@POStatus", QueryConditionOperatorType.Equal, query.POStatus);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                  DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom);
                if (query.POETPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.POType",
                 DbType.Int32, "@POType", QueryConditionOperatorType.Equal, query.POType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.PayTypeSysNo",
                 DbType.Int32, "@PayTypeSysNo", QueryConditionOperatorType.Equal, query.PayPeriodType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.PayStockSysNo",
                 DbType.Int32, "@PayStockSysNo", QueryConditionOperatorType.Equal, query.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

               // if (query.OrderType != PayableOrderType.POTempConsign)//非POTempConfign
                {
                    //if (query.POType.HasValue && (int)query.POType.Value == (int)PayableOrderType.BalanceOrder)
                    //{
                    //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    //        DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, PayableOrderType.BalanceOrder);
                    //}
                    //else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                            DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);
                    }
                }
                if (query.PayStyle.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(
                        "EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle));
                }
                if (query.IsNotInStock.HasValue && query.IsNotInStock.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "pay.InstockAmt IS NULL");
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InDate",
                    DbType.DateTime, "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.InDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InDate",
                   DbType.DateTime, "@InDateTo", QueryConditionOperatorType.LessThan, query.InDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.InTime",
                    DbType.DateTime, "@InStockDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.InStockDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.InTime",
                   DbType.DateTime, "@InStockDateTo", QueryConditionOperatorType.LessThan, query.InStockDateTo);


                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
                enumList.Add("OrderStatus", typeof(PurchaseOrderStatus));

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetCollectionPayment(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);
            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCollectionPaymentList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "paymentsettle.SysNo", DbType.Int32, orderIDList);
                }

                //仅负单据
                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderAmt", DbType.Decimal, "@OrderAmt", QueryConditionOperatorType.LessThan, 0);
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                //付款结算公司
                if (query.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                     DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, query.PaySettleCompany.Value);
                }

                //CRL19155:ETP查询条件
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                  DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom);
                if (query.POETPDateTo.HasValue)
                {
                    //query.Condition.POETPDateTo = query.Condition.POETPDateTo.Value.AddHours(10);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@SettleTimeTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                  DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "paymentsettle.Status",
                 DbType.Int32, "@VendorSettleStatus", QueryConditionOperatorType.Equal, query.VendorSettleStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.PayStyle.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(
                        "EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle));
                }
                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
                enumList.Add("OrderStatus", typeof(SettleStatus));

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetCCPayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            MapSortField(query);

            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCCPayList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "pay.SysNo desc"))
            {
                #region Condition
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                 DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)PayableOrderType.CostChange);

                if (query.POEGPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POEGPDateFrom.Value);
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.EGP",
                     DbType.DateTime, "@POEGPDateTo", QueryConditionOperatorType.LessThanOrEqual, query.POEGPDateTo.Value);
                }

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "cc.SysNo", DbType.Int32, orderIDList);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                    DbType.DateTime, "@InTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, query.OrderDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderDate",
                   DbType.DateTime, "@InTimeTo", QueryConditionOperatorType.LessThan, query.OrderDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CurrencySysNo",
                   DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, query.CurrencySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sysUser.UserSysNo",
                   DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, query.CreatePMSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.PayStatus",
                   DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                   DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, query.InvoiceStatus);
                if (query.POETPDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                      DbType.DateTime, "@POETPFrom", QueryConditionOperatorType.MoreThanOrEqual, query.POETPDateFrom.Value);
                }
                if (query.POETPDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.ETP",
                     DbType.DateTime, "@POETPTo", QueryConditionOperatorType.LessThanOrEqual, query.POETPDateTo.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.PayStyle.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(
                        "EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle));
                }

                if (query.POBelongPMSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cc.PMSysNo",
                     DbType.Int32, "@POBelongPMSysNo", QueryConditionOperatorType.Equal, query.POBelongPMSysNo.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InDate",
                    DbType.DateTime, "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.InDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InDate",
                   DbType.DateTime, "@InDateTo", QueryConditionOperatorType.LessThan, query.InDateTo);

                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
                enumList.Add("OrderStatus", typeof(CostChangeStatus));
                

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private DataTable GetAllPayList(PayableQueryFilter query, out int totalCount, out DataTable dtStatistical)
        {
            PagingInfoEntity pagingEntity = CreatePagingInfo(query.PagingInfo);
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAllPayList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand,
                pagingEntity, "PaySysNo desc"))
            {
                #region ConditionBuilder

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                string sb1 = null;
                sb1 += "WHERE pay.OrderType IN(0,4,5)";
                string sb2 = null;
                sb2 += "WHERE pay.OrderType = 1";
                string sb3 = null;
                sb3 += "WHERE pay.OrderType IN(2,3)";
                string sb4 = null;
                sb4 += "WHERE pay.OrderType IN(6,7,8)";
                string sb5 = null;
                sb5 += "WHERE pay.OrderType = 9";
                string sb6 = null;
                sb6 += "WHERE pay.OrderType = 10";
                string sb7 = null;
                sb7 += "WHERE pay.OrderType = 11";
                string sb8 = null;
                sb8 += "WHERE pay.OrderType = 12";
                string sb9 = null;
                sb9 += "WHERE pay.OrderType = 16";

                sb1 += " AND pay.CompanyCode = @CompanyCode";
                sb2 += " AND pay.CompanyCode = @CompanyCode";
                sb3 += " AND pay.CompanyCode = @CompanyCode";
                sb4 += " AND pay.CompanyCode = @CompanyCode";
                sb5 += " AND pay.CompanyCode = @CompanyCode";
                sb6 += " AND pay.CompanyCode = @CompanyCode";
                sb7 += " AND pay.CompanyCode = @CompanyCode";
                sb8 += " AND pay.CompanyCode = @CompanyCode";
                sb9 += " AND pay.CompanyCode = @CompanyCode";
                dataCommand.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, query.CompanyCode);

                if (query.POETPDateFrom.HasValue)
                {
                    sb1 += " AND pay.ETP >= @POETPDateFrom";
                    sb2 += " AND pay.ETP >= @POETPDateFrom";
                    sb3 += " AND pay.ETP >= @POETPDateFrom";
                    sb4 += " AND pay.ETP >= @POETPDateFrom";
                    sb5 += " AND pay.ETP >= @POETPDateFrom";
                    sb6 += " AND pay.ETP >= @POETPDateFrom";
                    sb7 += " AND pay.ETP >= @POETPDateFrom";
                    sb8 += " AND pay.ETP >= @POETPDateFrom";
                    sb9 += " AND pay.ETP >= @POETPDateFrom";
                    dataCommand.AddInputParameter("@POETPDateFrom", DbType.DateTime, query.POETPDateFrom.Value);
                }
                if (query.POETPDateTo.HasValue)
                {
                    sb1 += " AND pay.ETP <= @POETPDateTo";
                    sb2 += " AND pay.ETP <= @POETPDateTo";
                    sb3 += " AND pay.ETP <= @POETPDateTo";
                    sb4 += " AND pay.ETP <= @POETPDateTo";
                    sb5 += " AND pay.ETP <= @POETPDateTo";
                    sb6 += " AND pay.ETP <= @POETPDateTo";
                    sb7 += " AND pay.ETP <= @POETPDateTo";
                    sb8 += " AND pay.ETP <= @POETPDateTo";
                    sb9 += " AND pay.ETP <= @POETPDateTo";
                    dataCommand.AddInputParameter("@POETPDateTo", DbType.DateTime, query.POETPDateTo.Value);
                }

                if (query.POEGPDateFrom.HasValue)
                {
                    sb1 += " AND pay.EGP >= @POEGPDateFrom";
                    sb2 += " AND pay.EGP >= @POEGPDateFrom";
                    sb3 += " AND pay.EGP >= @POEGPDateFrom";
                    sb4 += " AND pay.EGP >= @POEGPDateFrom";
                    sb5 += " AND pay.EGP >= @POEGPDateFrom";
                    sb6 += " AND pay.EGP >= @POEGPDateFrom";
                    sb7 += " AND pay.EGP >= @POEGPDateFrom";
                    sb8 += " AND pay.EGP >= @POEGPDateFrom";
                    sb9 += " AND pay.EGP >= @POEGPDateFrom";

                    dataCommand.AddInputParameter("@POEGPDateFrom", DbType.DateTime, query.POEGPDateFrom.Value);
                }

                if (query.IsOnlyNegativeOrder.HasValue && query.IsOnlyNegativeOrder.Value)
                {
                    sb1 += " AND pay.OrderAmt < 0";
                    sb2 += " AND pay.OrderAmt < 0";
                    sb3 += " AND pay.OrderAmt < 0";
                    sb4 += " AND pay.OrderAmt < 0";
                    sb5 += " AND pay.OrderAmt < 0";
                    sb6 += " AND pay.OrderAmt < 0";
                    sb7 += " AND pay.OrderAmt < 0";
                    sb8 += " AND pay.OrderAmt < 0";
                    sb9 += " AND pay.OrderAmt < 0";
                }

                if (query.POEGPDateTo.HasValue)
                {
                    sb1 += " AND pay.EGP <= @POEGPDateTo";
                    sb2 += " AND pay.EGP <= @POEGPDateTo";
                    sb3 += " AND pay.EGP <= @POEGPDateTo";
                    sb4 += " AND pay.EGP <= @POEGPDateTo";
                    sb5 += " AND pay.EGP <= @POEGPDateTo";
                    sb6 += " AND pay.EGP <= @POEGPDateTo";
                    sb7 += " AND pay.EGP <= @POEGPDateTo";
                    sb8 += " AND pay.EGP <= @POEGPDateTo";
                    sb9 += " AND pay.EGP <= @POEGPDateTo";

                    dataCommand.AddInputParameter("@POEGPDateTo", DbType.DateTime, query.POEGPDateTo.Value);
                }
                if (query.OrderDateFrom.HasValue)
                {
                    sb1 += " AND pay.OrderDate >= @OrderDateFrom";
                    sb2 += " AND pay.OrderDate >= @OrderDateFrom";
                    sb3 += " AND pay.OrderDate >= @OrderDateFrom";
                    sb4 += " AND pay.OrderDate >= @OrderDateFrom";
                    sb5 += " AND pay.OrderDate>= @OrderDateFrom";
                    sb6 += " AND pay.OrderDate>= @OrderDateFrom";
                    sb7 += " AND pay.OrderDate>= @OrderDateFrom";
                    sb8 += " AND pay.OrderDate>= @OrderDateFrom";
                    sb9 += " AND pay.OrderDate>= @OrderDateFrom";

                    dataCommand.AddInputParameter("@OrderDateFrom", DbType.DateTime, query.OrderDateFrom.Value);
                }
                if (query.OrderDateTo.HasValue)
                {
                    sb1 += " AND pay.OrderDate < @OrderDateTo";
                    sb2 += " AND pay.OrderDate < @OrderDateTo";
                    sb3 += " AND pay.OrderDate < @OrderDateTo";
                    sb4 += " AND pay.OrderDate < @OrderDateTo";
                    sb5 += " AND pay.OrderDate < @OrderDateTo";
                    sb6 += " AND pay.OrderDate < @OrderDateTo";
                    sb7 += " AND pay.OrderDate < @OrderDateTo";
                    sb8 += " AND pay.OrderDate < @OrderDateTo";
                    sb9 += " AND pay.OrderDate < @OrderDateTo";

                    dataCommand.AddInputParameter("@OrderDateTo", DbType.DateTime, query.OrderDateTo);
                }

                if (query.InDateFrom.HasValue)
                {
                    sb1 += " AND pay.InDate >= @InDateFrom";
                    sb2 += " AND pay.InDate >= @InDateFrom";
                    sb3 += " AND pay.InDate >= @InDateFrom";
                    sb4 += " AND pay.InDate >= @InDateFrom";
                    sb5 += " AND pay.InDate>= @InDateFrom";
                    sb6 += " AND pay.InDate>= @InDateFrom";
                    sb7 += " AND pay.InDate>= @InDateFrom";
                    sb8 += " AND pay.InDate>= @InDateFrom";
                    sb9 += " AND pay.InDate>= @InDateFrom";

                    dataCommand.AddInputParameter("@InDateFrom", DbType.DateTime, query.InDateFrom.Value);
                }

                if (query.InDateTo.HasValue)
                {
                    sb1 += " AND pay.InDate <= @InDateTo";
                    sb2 += " AND pay.InDate <= @InDateTo";
                    sb3 += " AND pay.InDate <= @InDateTo";
                    sb4 += " AND pay.InDate <= @InDateTo";
                    sb5 += " AND pay.InDate <= @InDateTo";
                    sb6 += " AND pay.InDate <= @InDateTo";
                    sb7 += " AND pay.InDate <= @InDateTo";
                    sb8 += " AND pay.InDate <= @InDateTo";
                    sb9 += " AND pay.InDate <= @InDateTo";

                    dataCommand.AddInputParameter("@InDateTo", DbType.DateTime, query.InDateTo);
                }

                if (query.VendorSysNo.HasValue)
                {
                    sb1 += " AND vendor.SysNo = @VendorSysNo";
                    sb2 += " AND vendor.SysNo = @VendorSysNo";
                    sb3 += " AND vendor.SysNo = @VendorSysNo";
                    sb4 += " AND vendor.SysNo = @VendorSysNo";
                    sb5 += " AND vendor.SysNo = @VendorSysNo";
                    sb6 += " AND vendor.SysNo = @VendorSysNo";
                    sb7 += " AND vendor.SysNo = @VendorSysNo";
                    sb8 += " AND vendor.SysNo = @VendorSysNo";
                    sb9 += " AND vendor.SysNo = @VendorSysNo";

                    dataCommand.AddInputParameter("@VendorSysNo", DbType.Int32, query.VendorSysNo.Value);
                }
                if (query.CreatePMSysNo.HasValue)
                {
                    sb1 += " AND sysUser.UserSysNo = @UserSysNo";
                    sb2 += " AND 1=2";
                    sb3 += " AND 1=2";
                    sb4 += " AND 1=2";
                    sb5 += " AND 1=2";
                    sb6 += " AND 1=2";
                    sb7 += " AND 1=2";
                    sb8 += " AND 1=2";
                    sb9 += " AND sysUser.UserSysNo = @UserSysNo";

                    dataCommand.AddInputParameter("@UserSysNo", DbType.Int32, query.CreatePMSysNo.Value);
                }

                if (query.POBelongPMSysNo.HasValue)
                {
                    sb1 += " AND sysPOBelongPMUser.UserSysNo = @POBelongPMSysNo";
                    sb2 += " AND 1=2";
                    sb3 += " AND 1=2";
                    sb4 += " AND 1=2";
                    sb5 += " AND 1=2";
                    sb6 += " AND 1=2";
                    sb7 += " AND 1=2";
                    sb8 += " AND 1=2";
                    sb9 += " AND cc.PMSysNo =@POBelongPMSysNo ";

                    dataCommand.AddInputParameter("@POBelongPMSysNo", DbType.Int32, query.POBelongPMSysNo.Value);
                }
                if (query.SysNo != null)
                {
                    sb1 += " AND pay.SysNo = @SysNo";
                    sb2 += " AND pay.SysNo = @SysNo";
                    sb3 += " AND pay.SysNo = @SysNo";
                    sb4 += " AND pay.SysNo = @SysNo";
                    sb5 += " AND pay.SysNo = @SysNo";
                    sb6 += " AND pay.SysNo = @SysNo";
                    sb7 += " AND pay.SysNo = @SysNo";
                    sb8 += " AND pay.SysNo = @SysNo";
                    sb9 += " AND pay.SysNo = @SysNo";

                    dataCommand.AddInputParameter("@SysNo", DbType.Int32, query.SysNo.Value);
                }

                if (query.PayStatus.HasValue)
                {
                    sb1 += " AND pay.PayStatus = @PayStatus";
                    sb2 += " AND pay.PayStatus = @PayStatus";
                    sb3 += " AND pay.PayStatus = @PayStatus";
                    sb4 += " AND pay.PayStatus = @PayStatus";
                    sb5 += " AND pay.PayStatus = @PayStatus";
                    sb6 += " AND pay.PayStatus = @PayStatus";
                    sb7 += " AND pay.PayStatus = @PayStatus";
                    sb8 += " AND pay.PayStatus = @PayStatus";
                    sb9 += " AND pay.PayStatus = @PayStatus";

                    dataCommand.AddInputParameter("@PayStatus", DbType.Int32, query.PayStatus.Value);
                }
                if (query.InvoiceStatus.HasValue)
                {
                    sb1 += " AND pay.InvoiceStatus = @InvoiceStatus";
                    sb2 += " AND pay.InvoiceStatus = @InvoiceStatus";
                    sb3 += " AND pay.InvoiceStatus = @InvoiceStatus";
                    sb4 += " AND pay.InvoiceStatus = @InvoiceStatus";
                    sb5 += " AND pay.InvoiceStatus = @InvoiceStatus";
                    sb6 += " AND pay.InvoiceStatus = @InvoiceStatus";
                    sb7 += " AND pay.InvoiceStatus = @InvoiceStatus";
                    sb8 += " AND pay.InvoiceStatus = @InvoiceStatus";
                    sb9 += " AND pay.InvoiceStatus = @InvoiceStatus";

                    dataCommand.AddInputParameter("@InvoiceStatus", DbType.Int32, query.InvoiceStatus.Value);
                }
                if (query.CurrencySysNo.HasValue)
                {
                    sb1 += " AND pay.CurrencySysNo = @CurrencySysNo";
                    sb2 += " AND pay.CurrencySysNo = @CurrencySysNo";
                    sb3 += " AND pay.CurrencySysNo = @CurrencySysNo";
                    sb4 += " AND pay.CurrencySysNo = @CurrencySysNo";
                    sb5 += " AND pay.CurrencySysNo = @CurrencySysNo";
                    sb6 += " AND pay.CurrencySysNo = @CurrencySysNo";
                    sb7 += " AND pay.CurrencySysNo = @CurrencySysNo";
                    sb8 += " AND pay.CurrencySysNo = @CurrencySysNo";
                    sb9 += " AND pay.CurrencySysNo = @CurrencySysNo";

                    dataCommand.AddInputParameter("@CurrencySysNo", DbType.Int32, query.CurrencySysNo.Value);
                }
                if (query.StockSysNo.HasValue)
                {
                    sb1 += " AND po.PayStockSysNo = @StockSysNo";
                    sb2 += " AND vendorSettle.StockSysNo = @StockSysNo";
                    sb3 += " AND balancesettle.StockSysNo = @StockSysNo";
                    sb6 += " AND collectionSettlement.StockSysNo = @StockSysNo";
                    sb8 += " AND paymentsettle.StockSysNo = @StockSysNo";
                    dataCommand.AddInputParameter("@StockSysNo", DbType.Int32, query.StockSysNo.Value);
                }
                //付款结算公司
                if (query.PaySettleCompany.HasValue)
                {
                    sb1 += " AND vendor.PaySettleCompany = @PaySettleCompany";
                    sb2 += " AND vendor.PaySettleCompany = @PaySettleCompany";
                    sb3 += " AND vendor.PaySettleCompany = @PaySettleCompany";
                    sb4 += " AND vendor.PaySettleCompany = @PaySettleCompany";
                    sb5 += " AND vendor.PaySettleCompany = @PaySettleCompany";
                    sb6 += " AND vendor.PaySettleCompany = @PaySettleCompany";
                    sb7 += " AND vendor.PaySettleCompany = @PaySettleCompany";
                    sb8 += " AND vendor.PaySettleCompany = @PaySettleCompany";
                    sb9 += " AND vendor.PaySettleCompany = @PaySettleCompany";

                    dataCommand.AddInputParameter("@PaySettleCompany", DbType.Int32, query.PaySettleCompany.Value);
                }

                if (!String.IsNullOrEmpty(query.OrderID))
                {
                    List<Int32> list = new List<int>();
                    string[] orderIDArray = query.OrderID.Trim(new char[] { '.' }).Split('.');
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        list.Add(Int32.Parse(orderIDArray[i]));
                    }
                    if (list != null && list.Count > 0)
                    {
                        int i = 0;
                        sb1 += " AND po.SysNo IN(";
                        sb2 += " AND vendorSettle.SysNo IN(";
                        sb3 += " AND balancesettle.SysNo IN(";
                        sb4 += " AND pay.OrderSysNo IN(";
                        sb5 += " AND RMAPOR.SysNo IN(";
                        sb6 += " AND collectionSettlement.SysNo IN(";
                        sb7 += " AND commission.SysNo IN(";
                        sb8 += " AND paymentsettle.SysNo IN(";
                        sb8 += " AND cc.SysNo IN(";

                        foreach (var sysNo in list)
                        {
                            string paramSysNo = "@SysNo" + i++;
                            sb1 += paramSysNo + ",";
                            sb2 += paramSysNo + ",";
                            sb3 += paramSysNo + ",";
                            sb4 += paramSysNo + ",";
                            sb5 += paramSysNo + ",";
                            sb6 += paramSysNo + ",";
                            sb7 += paramSysNo + ",";
                            sb8 += paramSysNo + ",";
                            sb9 += paramSysNo + ",";

                            dataCommand.AddInputParameter(paramSysNo, DbType.Int32, sysNo);
                        }
                        sb1 = sb1.Substring(0, sb1.Length - 1);
                        sb1 += " ) ";
                        sb2 = sb2.Substring(0, sb2.Length - 1);
                        sb2 += " ) ";
                        sb3 = sb3.Substring(0, sb3.Length - 1);
                        sb3 += " ) ";
                        sb4 = sb4.Substring(0, sb4.Length - 1);
                        sb4 += " ) ";
                        sb5 = sb5.Substring(0, sb5.Length - 1);
                        sb5 += " ) ";

                        sb6 = sb6.Substring(0, sb6.Length - 1);
                        sb6 += " ) ";
                        sb7 = sb7.Substring(0, sb7.Length - 1);
                        sb7 += " ) ";
                        sb8 = sb8.Substring(0, sb8.Length - 1);
                        sb8 += " ) ";
                        sb9 = sb9.Substring(0, sb9.Length - 1);
                        sb9 += " ) ";
                    }
                }

                if (query.PayStyle.HasValue)
                {
                    string portionSQL = string.Format(
                        " AND EXISTS (SELECT TOP 1 1 FROM ipp3.dbo.Finance_Pay_Item WITH(NOLOCK) WHERE PaySysNo = pay.SysNo AND Status<>-1 AND PayStyle={0})", (int)query.PayStyle);
                    sb1 += portionSQL;
                    sb2 += portionSQL;
                    sb3 += portionSQL;
                    sb4 += portionSQL;
                    sb5 += portionSQL;
                    sb6 += portionSQL;
                    sb7 += portionSQL;
                    sb8 += portionSQL;
                    sb9 += portionSQL; 
                }

                if (query.IsNotInStock.HasValue && query.IsNotInStock.Value)
                {
                    sb1 += " AND pay.InstockAmt IS NULL";
                    sb2 += " AND pay.InstockAmt IS NULL";
                    sb3 += " AND pay.InstockAmt IS NULL";
                    sb4 += " AND pay.InstockAmt IS NULL";
                    sb5 += " AND pay.InstockAmt IS NULL";
                    sb6 += " AND pay.InstockAmt IS NULL";
                    sb7 += " AND pay.InstockAmt IS NULL";
                    sb8 += " AND pay.InstockAmt IS NULL";
                    sb9 += " AND pay.InstockAmt IS NULL";
                }

                dataCommand.AddOutParameter("@APSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@PUSum", DbType.Double, 50);
                dataCommand.AddOutParameter("@SapSum", DbType.Double, 50);

                dataCommand.CommandText = dataCommand.CommandText
                    .Replace("#StrWhere1#", sb1)
                    .Replace("#StrWhere2#", sb2)
                    .Replace("#StrWhere3#", sb3)
                    .Replace("#StrWhere4#", sb4)
                    .Replace("#StrWhere5#", sb5)
                    .Replace("#StrWhere6#", sb6)
                    .Replace("#StrWhere7#", sb7)
                    .Replace("#StrWhere8#", sb8)
                    .Replace("#StrWhere9#", sb9); 

                #endregion

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OrderType", typeof(PayableOrderType));
                enumList.Add("PayStyle", typeof(PayItemStyle));
                enumList.Add("PayStatus", typeof(PayableStatus));
                enumList.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                dt.Columns.Add("IsOldConsignSettle", typeof(Boolean));
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        PayableOrderType OrderType;
                        if (!Enum.TryParse(dr["OrderType"].ToString(), out OrderType))
                        {
                           // OrderType = PayableOrderType.BalanceOrder;
                        }
                        if (OrderType == PayableOrderType.VendorSettleOrder)
                        {
                            dr["IsOldConsignSettle"] = IsOldConsignSettle(Convert.ToInt32(dr["OrderSysNo"]));
                        }
                    }
                }

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                decimal APSum = Convert.ToDecimal(dataCommand.GetParameterValue("APSum"));
                decimal PUSum = Convert.ToDecimal(dataCommand.GetParameterValue("PUSum"));
                decimal SapSum = Convert.ToDecimal(dataCommand.GetParameterValue("SapSum"));

                dtStatistical = new DataTable();
                dtStatistical.Columns.Add("APSum", typeof(decimal));
                dtStatistical.Columns.Add("PUSum", typeof(decimal));
                dtStatistical.Columns.Add("SapSum", typeof(decimal));
                dtStatistical.Rows.Add(APSum, PUSum, SapSum);

                return dt;
            }
        }

        private bool IsOldConsignSettle(int settleSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetPOConsignSettleItem");
            dataCommand.SetParameterValue("@SettleSysNo", settleSysNo);
            object obj = dataCommand.ExecuteScalar();
            return obj != null;
        }

        private void MapSortField(PayableQueryFilter query)
        {
            if (query.PagingInfo != null && !String.IsNullOrEmpty(query.PagingInfo.SortBy)
                && query != null && query.OrderType != null)
            {
                var index = 0;
                index = query.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = query.PagingInfo.SortBy.Substring(0, query.PagingInfo.SortBy.Length - index);
                var sortField = query.PagingInfo.SortBy;
                switch (sort)
                {
                    case "OrderType":
                        query.PagingInfo.SortBy = sortField.Replace("OrderType", "pay.OrderType");
                        break;
                    case "PayStatus":
                        query.PagingInfo.SortBy = sortField.Replace("PayStatus", "pay.PayStatus");
                        break;
                    case "InvoiceStatus":
                        query.PagingInfo.SortBy = sortField.Replace("InvoiceStatus", "pay.InvoiceStatus");
                        break;
                    case "CurrencySysNo":
                        query.PagingInfo.SortBy = sortField.Replace("CurrencySysNo", "pay.CurrencySysNo");
                        break;
                    case "UserName":
                        //if (query.OrderType == PayableOrderType.ReturnPointCashAdjust
                        //    || query.OrderType == PayableOrderType.SubAccount)
                        //{
                        //    query.PagingInfo.SortBy = sortField.Replace("UserName", "EIMS.CreateUser");
                        //}
                        //else
                        {
                            query.PagingInfo.SortBy = sortField.Replace("UserName", "sysUser.DisplayName");
                        }
                        break;
                    case "CreateDate":
                        if (query.OrderType != null)
                        {
                            switch (query.OrderType.Value)
                            {
                                case PayableOrderType.PO:
                               // case PayableOrderType.POTempConsign:
                                case PayableOrderType.POAdjust:
                                    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "po.InTime");
                                    break;
                                case PayableOrderType.VendorSettleOrder:
                                    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "vendorsettle.SettleTime");
                                    break;
                                //case PayableOrderType.SubAccount:
                                //case PayableOrderType.ReturnPointCashAdjust:
                                //case PayableOrderType.SubInvoice:
                                //    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "EIMS.CreateDate");
                                //    break;
                                case PayableOrderType.RMAPOR:
                                    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "RMAPOR.PMDAuditTime");
                                    break;
                                case PayableOrderType.CollectionSettlement:
                                    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "pay.OrderDate");
                                    break;
                                case PayableOrderType.Commission:
                                    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "pay.OrderDate");
                                    break;
                                case PayableOrderType.CollectionPayment:
                                    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "paymentsettle.SettleTime");
                                    break;
                                case PayableOrderType.CostChange:
                                    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "pay.OrderDate");
                                    break;
                                default:
                                    query.PagingInfo.SortBy = sortField.Replace("CreateDate", "balancesettle.CreateTime");
                                    break;
                            }
                        }
                        break;
                    case "PayableAmt":
                        query.PagingInfo.SortBy = sortField.Replace("PayableAmt", "pay.OrderAmt");
                        break;
                    case "AlreadyPayAmt":
                        query.PagingInfo.SortBy = sortField.Replace("AlreadyPayAmt", "pay.AlreadyPayAmt");
                        break;
                    case "VendorSysNo":
                        query.PagingInfo.SortBy = sortField.Replace("VendorSysNo", "vendor.SysNo");
                        break;
                    case "VendorName":
                        query.PagingInfo.SortBy = sortField.Replace("VendorName", "vendor.VendorName");
                        break;
                }
            }
        }

        private PagingInfoEntity CreatePagingInfo(ECCentral.QueryFilter.Common.PagingInfo pageInfo)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (pageInfo != null)
            {
                pagingInfo.MaximumRows = pageInfo.PageSize;
                pagingInfo.StartRowIndex = pageInfo.PageIndex * pageInfo.PageSize;
                pagingInfo.SortField = pageInfo.SortBy;
            }
            return pagingInfo;
        }
    }
}