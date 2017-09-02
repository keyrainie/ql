using System;
using System.Collections;
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
    [VersionExport(typeof(IInvoiceQueryDA))]
    public class InvoiceQueryDA : IInvoiceQueryDA
    {
        private const int TimeOut = 180;
        private const string TempTable = @"DECLARE @temporder TABLE(
 OrderSysNo int
);
INSERT @temporder
SELECT OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WHERE MasterSoSysNo IN ({0})
UNION ALL
SELECT MasterSoSysNo  As OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WHERE OrderSysNo IN ({0})
UNION ALL
SELECT OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WHERE OrderSysNo IN ({0})
UNION ALL
SELECT OrderSysNo FROM [IPP3].[dbo].[Finance_SOIncome] WHERE MasterSoSysNo in
 (SELECT MasterSoSysNo As OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WHERE OrderSysNo IN ({0}));";

        private const string ExTempTable = @"DECLARE @temporder TABLE(
 OrderSysNo int
);
INSERT @temporder
SELECT OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WHERE OrderSysNo IN ({0});";

        private const string ExTempTableForSOOnly = @"DECLARE @temporder TABLE(
 OrderSysNo int
);
INSERT @temporder
SELECT SONumber
FROM OverseaInvoiceReceiptManagement.dbo.Invoice_Master  WHERE SONumber IN ({0});";

        private const string TempWhere = @"select OrderSysNo from  @temporder";

        #region IInvoiceQueryDA Members

        /// <summary>
        /// 销售-分公司查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataSet Qeury(InvoiceQueryFilter filter, out int totalCount)
        {
            DataSet result = null;
            totalCount = 0;
            bool getInvoiceAll = false;

            if (!filter.OrderType.HasValue)
            {
                getInvoiceAll = true;
            }
            if ((filter.IsByCustomer ?? false) && !string.IsNullOrEmpty(filter.OrderID))
            {
                getInvoiceAll = true;
                filter.OrderID = "";
            }

            if (getInvoiceAll)
            {
                return GetInvoiceAll(filter, out totalCount);
            }

            switch (filter.OrderType)
            {
                case SOIncomeOrderType.SO:
                    result = GetInvoiceSO(filter, out totalCount);
                    break;

                case SOIncomeOrderType.RO:
                    result = GetInvoiceRO(filter, out totalCount);
                    break;

                case SOIncomeOrderType.RO_Balance:
                    result = GetInvoiceROBalance(filter, out totalCount);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 对账单查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable ReconciliationQuery(ReconciliationQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            totalCount = 0;
            bool getInvoiceAll = false;
            string customerSysNoList = string.Empty;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceReconciliation");
            PagingInfoEntity pagingInfo = CreateReconciliationPagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "T1.OrderSysNo",
                   DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, filter.OrderSysNo);
                }

                if (!string.IsNullOrEmpty(filter.SerialNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "T2.SerialNo",
                   DbType.AnsiStringFixedLength, "@SerialNo", QueryConditionOperatorType.Equal, filter.SerialNo);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "T2.SerialNo",
                   DbType.AnsiStringFixedLength, "@SerialNoR", QueryConditionOperatorType.Equal, "R" + filter.SerialNo);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "T2.SerialNo",
                   DbType.AnsiStringFixedLength, "@SerialNoP", QueryConditionOperatorType.Equal, "P" + filter.SerialNo);

                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "T1.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "T1.IncomeTime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "T1.IncomeTime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                }
                if (filter.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "T1.ConfirmTime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.ConfirmDateFrom.Value);
                }
                if (filter.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "T1.ConfirmTime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, filter.ConfirmDateTo);
                }

                //目前对账只对OrderType=1（so单）
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "T1.OrderType",
                   DbType.AnsiStringFixedLength, "@OrderType", QueryConditionOperatorType.Equal, 1);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;
                result = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            
            return result;
        }

        #endregion IInvoiceQueryDA Members

        #region GetInvoiceSO

        private DataSet GetInvoiceSO(InvoiceQueryFilter filter, out int totalCount)
        {
            string tempTab1 = string.Empty;
            string customerSysNoList = string.Empty;
            DataTable result = null;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceSO");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.CustomerSysNo))
                {
                    customerSysNoList = filter.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.CustomerSysNo in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.OrderID);
                    if (filter.IsRelated)
                    {
                        tempTab1 = string.Format(TempTable, orderIDList.ToListString());
                    }
                    else
                    {
                        tempTab1 = string.Format(ExTempTableForSOOnly, orderIDList.ToListString());
                    }
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("M.SONumber in ({0})", TempWhere));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "M.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, filter.IncomeType);
                }

                //选中仅销售单时需要排除作废和测试客户的收款单
                if (filter.IsSalesOrder)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format("((so.[Status] >=0 AND M.CustomerSysNo NOT IN ({0})) or (so.[Status] is null AND M.CustomerSysNo NOT IN ({0}))) ", GetTestCustomerString())
                    );
                }

                //礼品卡框勾选时只查礼品卡订单的收款单记录
                if (filter.IsGiftCard)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " sc.SOType NOT IN(4,5) ");
                }

                if (filter.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                }
                if (filter.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.ConfirmDateFrom.Value);
                }
                if (filter.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, filter.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(filter.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(filter.IncomeConfirmer));
                }

                if (filter.SOOutDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "M.InvoiceDate",
                   DbType.DateTime, "@SOOutDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.SOOutDateFrom.Value);
                }

                if (filter.SOOutDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "M.InvoiceDate",
                   DbType.DateTime, "@SOOutDateTo", QueryConditionOperatorType.LessThanOrEqual, filter.SOOutDateTo.Value);
                }

                if (filter.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, filter.IncomeStatus);
                }

                if (!string.IsNullOrEmpty(filter.PayTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payTypeSysNo",
                 DbType.Int32, "@payTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(filter.PayTypeSysNo));
                }

                if (!string.IsNullOrEmpty(filter.ShipTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.ShipTypeSysNo",
                    DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(filter.ShipTypeSysNo));
                }

                if (!string.IsNullOrEmpty(filter.StockID))
                {
                    List<string> stockIDList = new List<string>();
                    stockIDList.AddRange(filter.StockID.TrimEnd(',').Split(','));
                    sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "M.warehousenumber",
                  DbType.Int32, stockIDList);
                }

                if (filter.IsException)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " SO.SOStatus = 4 and stock.StockName is null ");
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;
                dataCommand.CommandText = dataCommand.CommandText.Replace("#tempTab1#", tempTab1);

                result = ExecuteDataTable(dataCommand, out totalCount);
            }
            DataTable soAmount = GetSOAmount(filter);
            return GetResult(result, soAmount);
        }

        private DataTable GetSOAmount(InvoiceQueryFilter filter)
        {
            string tempTab1 = string.Empty;
            string customerSysNoList = string.Empty;
            DataTable result = null;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceSOAmount");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SO.SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.CustomerSysNo))
                {
                    customerSysNoList = filter.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.CustomerSysNo in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.OrderID);
                    if (filter.IsRelated)
                    {
                        tempTab1 = string.Format(TempTable, orderIDList.ToListString());
                    }
                    else
                    {
                        tempTab1 = string.Format(ExTempTableForSOOnly, orderIDList.ToListString());
                    }
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("M.SONumber in ({0})", TempWhere));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "M.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, filter.IncomeType);
                }

                //销售单据选项选中时将只显示属于销售单的收款单，但排除属于测试用户和作废的收款单
                if (filter.IsSalesOrder)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(" ((so.[Status] >=0 AND M.CustomerSysNo NOT IN ({0})) or (so.[Status] is null AND M.CustomerSysNo NOT IN ({0}))) ", GetTestCustomerString()));
                }

                //礼品卡框勾选时只查礼品卡订单的收款单记录
                if (filter.IsGiftCard)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " sc.SOType NOT IN(4,5) ");
                }

                if (filter.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                }
                if (filter.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.ConfirmDateFrom.Value);
                }
                if (filter.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, filter.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(filter.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(filter.IncomeConfirmer));
                }

                if (filter.SOOutDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "M.InvoiceDate",
                   DbType.DateTime, "@SOOutDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.SOOutDateFrom.Value);
                }

                if (filter.SOOutDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "M.InvoiceDate",
                   DbType.DateTime, "@SOOutDateTo", QueryConditionOperatorType.LessThanOrEqual, filter.SOOutDateTo.Value);
                }

                if (filter.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, filter.IncomeStatus);
                }

                if (!string.IsNullOrEmpty(filter.PayTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payTypeSysNo",
                 DbType.Int32, "@payTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(filter.PayTypeSysNo));
                }

                if (!string.IsNullOrEmpty(filter.ShipTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.ShipTypeSysNo",
                    DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(filter.ShipTypeSysNo));
                }

                if (!string.IsNullOrEmpty(filter.StockID))
                {
                    List<string> stockIDList = new List<string>();
                    stockIDList.AddRange(filter.StockID.TrimEnd(',').Split(','));
                    sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "M.warehousenumber",
                  DbType.Int32, stockIDList);
                }

                if (filter.IsException)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " SO.SOStatus = 4 and stock.StockName is null ");
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;
                dataCommand.CommandText = dataCommand.CommandText.Replace("#tempTab1#", tempTab1);
                result = dataCommand.ExecuteDataTable();
            }
            return result;
        }

        #endregion GetInvoiceSO

        #region GetInvoiceRO

        private DataSet GetInvoiceRO(InvoiceQueryFilter filter, out int totalCount)
        {
            string customerSysNoList = string.Empty;
            DataTable result = null;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceRO");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.CustomerSysNo))
                {
                    customerSysNoList = filter.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.CustomerSysNo in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.OrderID);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.fsOrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, filter.IncomeType);
                }

                //选中仅销售单时需要排除作废和测试客户的收款单
                if (filter.IsSalesOrder)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format(" so.[Status] >=0 AND so.CustomerSysNo NOT IN ({0}) ", GetTestCustomerString())
                    );
                }

                //礼品卡框勾选时只查礼品卡订单的收款单记录
                if (filter.IsGiftCard)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " sc.SOType NOT IN(4,5) ");
                }

                if (filter.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                }
                if (filter.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.ConfirmDateFrom.Value);
                }
                if (filter.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, filter.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(filter.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(filter.IncomeConfirmer));
                }

                if (filter.RORefundDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.RefundTime",
                   DbType.DateTime, "@RORefundDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.RORefundDateFrom);
                }

                if (filter.RORefundDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.RefundTime",
                   DbType.DateTime, "@RORefundDateTo", QueryConditionOperatorType.LessThan, filter.RORefundDateTo);
                }

                if (filter.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, filter.IncomeStatus);
                }

                if (!filter.OrderType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType.Value);
                }

                if (filter.IsCash)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CashFlag",
               DbType.Int32, "@CashFlag", QueryConditionOperatorType.Equal, 0);
                }

                if (!string.IsNullOrEmpty(filter.StockID))
                {
                    List<string> stockIDList = new List<string>();
                    stockIDList.AddRange(filter.StockID.TrimEnd(',').Split(','));
                    sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "rr.OwnByWarehouse",
                  DbType.Int32, stockIDList);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;

                result = ExecuteDataTable(dataCommand, out totalCount);
            }

            DataTable roAmount = GetROAmount(filter);
            return GetResult(result, roAmount);
        }

        private DataTable GetROAmount(InvoiceQueryFilter filter)
        {
            string customerSysNoList = string.Empty;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceROAmount");
            DataTable result = null;
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SO.SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.CustomerSysNo))
                {
                    customerSysNoList = filter.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.CustomerSysNo in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.OrderID);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.fsOrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, filter.IncomeType);
                }

                //选中仅销售单时需要排除作废和测试客户的收款单
                if (filter.IsSalesOrder)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format(" so.[Status] >=0 AND so.CustomerSysNo NOT IN ({0}) ", GetTestCustomerString())
                    );
                }
                //礼品卡框勾选时只查礼品卡订单的收款单记录
                if (filter.IsGiftCard)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " sc.SOType NOT IN(4,5) ");
                }

                if (filter.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                }
                if (filter.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.ConfirmDateFrom.Value);
                }
                if (filter.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, filter.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(filter.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(filter.IncomeConfirmer));
                }

                if (filter.RORefundDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.RefundTime",
                   DbType.DateTime, "@RORefundDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.RORefundDateFrom);
                }

                if (filter.RORefundDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.RefundTime",
                   DbType.DateTime, "@RORefundDateTo", QueryConditionOperatorType.LessThan, filter.RORefundDateTo);
                }

                if (filter.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, filter.IncomeStatus);
                }

                if (filter.OrderType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType.Value);
                }

                if (filter.IsCash)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CashFlag",
               DbType.Int32, "@CashFlag", QueryConditionOperatorType.Equal, 0);
                }

                if (!string.IsNullOrEmpty(filter.StockID))
                {
                    List<string> stockIDList = new List<string>();
                    stockIDList.AddRange(filter.StockID.TrimEnd(',').Split(','));
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "SO.fsOrderSysNo",
                        QueryConditionOperatorType.In, string.Format(@"SELECT rmi.RefundSysNo from
         	         [IPP3].[dbo].[RMA_Refund_Item] rmi WITH(NOLOCK)
                    INNER JOIN [IPP3].[dbo].[RMA_Register] rr WITH(NOLOCK)
                    ON rr.sysno=rmi.registersysno
         	        WHERE rr.OwnByWarehouse in ({0})", stockIDList.ToListString()));
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;
                result = dataCommand.ExecuteDataTable();
            }
            return result;
        }

        #endregion GetInvoiceRO

        #region GetInvoiceROBalance

        private DataSet GetInvoiceROBalance(InvoiceQueryFilter filter, out int totalCount)
        {
            string customerSysNoList = string.Empty;
            DataTable result = null;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceROBalance");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.CustomerSysNo))
                {
                    customerSysNoList = filter.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.CustomerSysNo in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.OrderID);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.fsOrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, filter.IncomeType);
                }

                //选中仅销售单时需要排除作废和测试客户的收款单
                if (filter.IsSalesOrder)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format(" so.[Status] >=0 AND so.CustomerSysNo NOT IN ({0}) ", GetTestCustomerString())
                    );
                }
                //礼品卡框勾选时只查礼品卡订单的收款单记录
                if (filter.IsGiftCard)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " sc.SOType NOT IN(4,5) ");
                }

                if (filter.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                }
                if (filter.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.ConfirmDateFrom.Value);
                }
                if (filter.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, filter.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(filter.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(filter.IncomeConfirmer));
                }

                if (filter.RORefundDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.RefundTime",
                   DbType.DateTime, "@RORefundDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.RORefundDateFrom.Value);
                }

                if (filter.RORefundDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.RefundTime",
                   DbType.DateTime, "@RORefundDateTo", QueryConditionOperatorType.LessThan, filter.RORefundDateTo);
                }

                if (filter.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, filter.IncomeStatus);
                }

                if (filter.OrderType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType.Value);
                }

                if (filter.IsCash)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CashFlag",
               DbType.Int32, "@CashFlag", QueryConditionOperatorType.Equal, 0);
                }

                if (!string.IsNullOrEmpty(filter.StockID))
                {
                    List<string> stockIDList = new List<string>();
                    stockIDList.AddRange(filter.StockID.TrimEnd(',').Split(','));
                    sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "rr.OwnByWarehouse",
                  DbType.Int32, stockIDList);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;

                result = ExecuteDataTable(dataCommand, out totalCount);
            }

            DataTable roAmount = GetROBalanceAmount(filter);
            return GetResult(result, roAmount);
        }

        private DataTable GetROBalanceAmount(InvoiceQueryFilter filter)
        {
            string customerSysNoList = string.Empty;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceROBalanceAmount");
            DataTable result = null;
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SO.SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.CustomerSysNo))
                {
                    customerSysNoList = filter.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.OrderID);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.fsOrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, filter.IncomeType);
                }

                //选中仅销售单时需要排除作废和测试客户的收款单
                if (filter.IsSalesOrder)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format(" so.[Status] >=0 AND so.CustomerSysNo NOT IN ({0}) ", GetTestCustomerString())
                    );
                }
                //礼品卡框勾选时只查礼品卡订单的收款单记录
                if (filter.IsGiftCard)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " sc.SOType NOT IN(4,5) ");
                }

                if (filter.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                }
                if (filter.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.ConfirmDateFrom.Value);
                }
                if (filter.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, filter.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(filter.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(filter.IncomeConfirmer));
                }

                if (filter.RORefundDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.RefundTime",
                   DbType.DateTime, "@RORefundDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.RORefundDateFrom.Value);
                }

                if (filter.RORefundDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.RefundTime",
                   DbType.DateTime, "@RORefundDateTo", QueryConditionOperatorType.LessThan, filter.RORefundDateTo);
                }

                if (filter.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, filter.IncomeStatus);
                }

                if (filter.OrderType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType.Value);
                }

                if (filter.IsCash)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CashFlag",
               DbType.Int32, "@CashFlag", QueryConditionOperatorType.Equal, 0);
                }

                if (!string.IsNullOrEmpty(filter.StockID))
                {
                    List<string> stockIDList = new List<string>();
                    stockIDList.AddRange(filter.StockID.TrimEnd(',').Split(','));
                    sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "rr.OwnByWarehouse",
                  DbType.Int32, stockIDList);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;
                result = dataCommand.ExecuteDataTable();
            }

            return result;
        }

        #endregion GetInvoiceROBalance

        #region GetInvoiceAll

        private DataSet GetInvoiceAll(InvoiceQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            string tempTab1 = string.Empty;
            string tempTab2 = string.Empty;
            string customerSysNoList = string.Empty;
            StringBuilder sbSO = new StringBuilder();
            StringBuilder sbRO = new StringBuilder();
            StringBuilder sbROBalance = new StringBuilder();

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceAll");

            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "SysNo DESC"))
            {
                sbSO.Append("WHERE 1=1 ");
                sbRO.Append("WHERE 1=1 ");
                sbROBalance.Append("WHERE 1=1 ");
                if (!string.IsNullOrEmpty(filter.CustomerSysNo))
                {
                    customerSysNoList = filter.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });

                    sbSO.Append(string.Format("AND M.customersysno in ({0})", customerSysNoList));
                    sbRO.Append(string.Format("AND SO.customersysno in ({0})", customerSysNoList));
                    sbROBalance.Append(string.Format("AND SO.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.OrderID);
                    if (filter.IsRelated)
                    {
                        tempTab1 = string.Format(TempTable, orderIDList.ToListString());
                    }
                    else
                    {
                        tempTab1 = string.Format(ExTempTableForSOOnly, orderIDList.ToListString());
                    }
                    tempTab2 = string.Format(@" and M.SONumber in ({0})", TempWhere);
                    sbRO.Append(string.Format("AND SO.fsOrderSysNo in ({0}) ", orderIDList.ToListString()));
                    sbROBalance.Append(string.Format("AND SO.fsOrderSysNo in ({0}) ", orderIDList.ToListString()));
                }

                sbSO.Append("AND M.CompanyCode=@CompanyCode ");
                sbRO.Append("AND SO.CompanyCode=@CompanyCode ");
                sbROBalance.Append("AND SO.CompanyCode=@CompanyCode ");
                dataCommand.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, filter.CompanyCode);

                if (filter.IncomeType.HasValue)
                {
                    sbSO.Append("AND SO.incomestyle=@IncomeType ");
                    sbRO.Append("AND SO.incomestyle=@IncomeType ");
                    sbROBalance.Append("AND SO.incomestyle=@IncomeType ");
                    dataCommand.AddInputParameter("@IncomeType", DbType.Int32, filter.IncomeType);
                }

                //销售单据选项选中时将只显示属于销售单的收款单，但排除属于测试用户和作废的收款单
                if (filter.IsSalesOrder)
                {
                    sbSO.AppendFormat(" AND ((so.[Status] >=0 AND M.CustomerSysNo NOT IN ({0})) or (so.[Status] is null AND M.CustomerSysNo NOT IN ({0}))) ", GetTestCustomerString());
                    //sbRO.AppendFormat(" AND so.[Status] >=0  AND so.CustomerSysNo NOT IN ({0}) ", GetTestCustomerString());
                    //sbROBalance.AppendFormat(" AND so.[Status] >=0  AND so.CustomerSysNo NOT IN ({0}) ", GetTestCustomerString());
                }

                //礼品卡框勾选时只查礼品卡订单的收款单记录
                if (filter.IsGiftCard)
                {
                    sbSO.Append(" AND sc.SOType NOT IN(4,5) ");
                    sbRO.Append(" AND sc.SOType NOT IN(4,5) ");
                    sbROBalance.Append(" AND sc.SOType NOT IN(4,5) ");
                }

                if (filter.CreateDateFrom != null)
                {
                    sbSO.Append("AND SO.IncomeTime>=@CreateDateFrom ");
                    sbRO.Append("AND SO.IncomeTime>=@CreateDateFrom ");
                    sbROBalance.Append("AND SO.IncomeTime>=@CreateDateFrom ");
                    dataCommand.AddInputParameter("@CreateDateFrom", DbType.DateTime, filter.CreateDateFrom.Value);
                }

                if (filter.CreateDateTo != null)
                {
                    sbSO.Append("AND SO.IncomeTime<@CreateDateTo ");
                    sbRO.Append("AND SO.IncomeTime<@CreateDateTo ");
                    sbROBalance.Append("AND SO.IncomeTime<@CreateDateTo ");
                    dataCommand.AddInputParameter("@CreateDateTo", DbType.DateTime, filter.CreateDateTo);
                }

                if (filter.ConfirmDateFrom != null)
                {
                    sbSO.Append("AND SO.confirmtime>=@ConfirmDateFrom ");
                    sbRO.Append("AND SO.confirmtime>=@ConfirmDateFrom ");
                    sbROBalance.Append("AND SO.confirmtime>=@ConfirmDateFrom ");
                    dataCommand.AddInputParameter("@ConfirmDateFrom", DbType.DateTime, filter.ConfirmDateFrom);
                }

                if (filter.ConfirmDateTo != null)
                {
                    sbSO.Append("AND SO.confirmtime<@ConfirmDateTo ");
                    sbRO.Append("AND SO.confirmtime<@ConfirmDateTo ");
                    sbROBalance.Append("AND SO.confirmtime<@ConfirmDateTo ");
                    dataCommand.AddInputParameter("@ConfirmDateTo", DbType.DateTime, filter.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(filter.IncomeConfirmer))
                {
                    sbSO.Append("AND SO.ConfirmuserSysNo=@IncomeConfirmer ");
                    sbRO.Append("AND SO.ConfirmuserSysNo=@IncomeConfirmer ");
                    sbROBalance.Append("AND SO.ConfirmuserSysNo=@IncomeConfirmer ");
                    dataCommand.AddInputParameter("@IncomeConfirmer", DbType.Int32, Int32.Parse(filter.IncomeConfirmer));
                }

                if (filter.IncomeStatus.HasValue)
                {
                    sbSO.Append("AND SO.Status=@IncomeStatus ");
                    sbRO.Append("AND SO.Status=@IncomeStatus ");
                    sbROBalance.Append("AND SO.Status=@IncomeStatus ");
                    dataCommand.AddInputParameter("@IncomeStatus", DbType.Int32, filter.IncomeStatus);
                }

                if (!string.IsNullOrEmpty(filter.StockID))
                {
                    sbSO.Append(string.Format("AND M.warehousenumber in ({0})", filter.StockID.TrimEnd(',')));
                    sbRO.Append(string.Format("AND rr.OwnByWarehouse in ({0})", filter.StockID.TrimEnd(',')));
                    sbROBalance.Append(string.Format("AND rr.OwnByWarehouse in ({0})", filter.StockID.TrimEnd(',')));
                }

                dataCommand.AddOutParameter("@PageTotalCount", DbType.String, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;
                dataCommand.CommandText = dataCommand.CommandText.Replace("#StrWhere1#", sbSO.ToString());
                dataCommand.CommandText = dataCommand.CommandText.Replace("#StrWhere2#", sbRO.ToString());
                dataCommand.CommandText = dataCommand.CommandText.Replace("#StrWhere3#", sbROBalance.ToString());
                dataCommand.CommandText = dataCommand.CommandText.Replace("#tempTab1#", tempTab1);
                dataCommand.CommandText = dataCommand.CommandText.Replace("#tempTab2#", tempTab2);

                result = ExecuteDataTable(dataCommand, out totalCount);
            }
            DataTable allAmount = GetAllAmount(filter);         
            return GetResult(result, allAmount);
        }

        private DataTable GetAllAmount(InvoiceQueryFilter filter)
        {
            string tempTab1 = string.Empty;
            string customerSysNoList = string.Empty;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceAllAmount");
            DataTable result = null;
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SO.SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.CustomerSysNo))
                {
                    customerSysNoList = filter.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.Customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.OrderID);
                    if (filter.IsRelated)
                    {
                        tempTab1 = string.Format(TempTable, orderIDList.ToListString());
                    }
                    else
                    {
                        tempTab1 = string.Format(ExTempTable, orderIDList.ToListString());
                    }
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SO.ordersysno in ({0})", TempWhere));
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, filter.IncomeType);
                }

                //选中仅销售单时需要排除作废和测试客户的收款单
                if (filter.IsSalesOrder)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format(" (so.[Status] >=0 or so.[Status] is null) AND so.CustomerSysNo NOT IN ({0}) ", GetTestCustomerString())
                    );
                }

                //礼品卡框勾选时只查礼品卡订单的收款单记录
                if (filter.IsGiftCard)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " sc.SOType NOT IN(4,5) ");
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.IncomeTime",
                   DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);

                if (filter.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.IncomeTime",
                       DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.ConfirmDateFrom);

                if (filter.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.confirmtime",
                       DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, filter.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(filter.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.ConfirmuserSysNo",
                      DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(filter.IncomeConfirmer));
                }

                if (filter.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.Status",
                      DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, filter.IncomeStatus);
                }
                if (!string.IsNullOrEmpty(filter.StockID))
                {
                    List<string> stockIDList = new List<string>();
                    stockIDList.AddRange(filter.StockID.TrimEnd(',').Split(','));
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "SO.fsOrderSysNo",
                       QueryConditionOperatorType.In, string.Format(@"SELECT rmi.RefundSysNo from
         	         [IPP3].[dbo].[RMA_Refund_Item] rmi WITH(NOLOCK)
                    INNER JOIN [IPP3].[dbo].[RMA_Register] rr WITH(NOLOCK)
                    ON rr.sysno=rmi.registersysno
         	        WHERE rr.OwnByWarehouse in ({0})", stockIDList.ToListString()));
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandTimeout = TimeOut;
                dataCommand.CommandText = dataCommand.CommandText.Replace("#tempTab1#", tempTab1);
                result = dataCommand.ExecuteDataTable();
            }

            filter.OrderType = SOIncomeOrderType.SO;
            DataTable soAmount = GetSOAmount(filter);

            if ((result != null && result.Rows.Count > 0) && (soAmount != null && soAmount.Rows.Count > 0))
            {
                result.Rows[0]["TotalAmt"] = Convert.ToDecimal(result.Rows[0]["TotalAmt"]) + Convert.ToDecimal(soAmount.Rows[0]["TotalAmt"]);
                result.Rows[0]["TotalAmtForSOOnly"] = Convert.ToDecimal(soAmount.Rows[0]["TotalAmtForSOOnly"]);
                result.Rows[0]["GiftCardPayAmt"] = Convert.ToDecimal(soAmount.Rows[0]["GiftCardPayAmt"]);
                result.Rows[0]["UnionAmt"] = Convert.ToDecimal(result.Rows[0]["UnionAmt"]) + Convert.ToDecimal(soAmount.Rows[0]["UnionAmt"]);
                result.Rows[0]["TSOTotalAmt"] = Convert.ToDecimal(result.Rows[0]["TSOTotalAmt"]) + Convert.ToDecimal(soAmount.Rows[0]["TSOTotalAmt"]);
                result.Rows[0]["PrepayAmt"] = Convert.ToDecimal(soAmount.Rows[0]["PrepayAmt"]);
                result.Rows[0]["IncomeAmt"] = Convert.ToDecimal(soAmount.Rows[0]["IncomeAmt"]);
            }
            return result;
        }

        #endregion GetInvoiceAll

        private DataTable ExecuteDataTable(CustomDataCommand dataCommand, out int totalCount)
        {
            EnumColumnList enumColumns = new EnumColumnList();
            enumColumns.Add("IncomeStyle", typeof(SOIncomeOrderStyle));
            enumColumns.Add("OrderType", typeof(SOIncomeOrderType));
            enumColumns.Add("IncomeStatus", typeof(SOIncomeStatus));
            enumColumns.Add("SapImportedStatus", typeof(SapImportedStatus));
            var result = dataCommand.ExecuteDataTable(enumColumns);
            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            return result;
        }

        /// <summary>
        /// 组装返回给客户端的结果
        /// </summary>
        /// <param name="resultDT"></param>
        /// <param name="invoiceAmt"></param>
        /// <returns></returns>
        private DataSet GetResult(DataTable resultDT, DataTable invoiceAmtDT)
        {
            DataSet result = new DataSet();
            if (resultDT != null)
            {
                resultDT.TableName = "ResultTable";
                result.Tables.Add(resultDT);
            }
            if (invoiceAmtDT != null)
            {
                invoiceAmtDT.TableName = "InvoiceAmtTable";
                result.Tables.Add(invoiceAmtDT);
            }
            return result;
        }

        /// <summary>
        /// 构造PagingInfo对象
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private PagingInfoEntity CreatePagingInfo(InvoiceQueryFilter query)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (query.PagingInfo != null)
            {
                pagingInfo.MaximumRows = query.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;
                pagingInfo.SortField = query.PagingInfo.SortBy;
            }
            return pagingInfo;
        }

        private PagingInfoEntity CreateReconciliationPagingInfo(ReconciliationQueryFilter query)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (query.PagingInfo != null)
            {
                pagingInfo.MaximumRows = query.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;
                pagingInfo.SortField = query.PagingInfo.SortBy;
            }
            return pagingInfo;
        }
        /// <summary>
        /// 取得测试用户的系统编号
        /// </summary>
        /// <returns></returns>
        private string GetTestCustomerString()
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetTestCustomerSysNos");

            string result = dataCommand.CommandText;
            if (string.IsNullOrEmpty(result))
            {
                result = "0";
            }
            return result.Trim();
        }

        private List<int> GetOrderIDListInt(string orderid)
        {
            if (!string.IsNullOrEmpty(orderid))
            {
                orderid = orderid.ToUpper();
                orderid = orderid.Replace(" ", "");
                orderid = orderid.Replace(".", ",");
                List<int> orderIDList = new List<int>();
                string[] temp = orderid.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    int index = temp[i].IndexOf("R3");
                    if (index != -1)
                    {
                        temp[i] = temp[i].Substring(2).TrimStart('0');
                    }
                }
                int[] result;
                result = Array.ConvertAll(temp, str =>
                {
                    int p = 0;
                    int.TryParse(str, out p);
                    return p;
                });
                orderIDList.AddRange(result);
                return orderIDList;
            }
            else
            {
                return new List<int>();
            }
        }
    }

    /// <summary>
    /// 存储金额信息
    /// </summary>
    public class InvoiceAmtEntity
    {
        /// <summary>
        /// 发票金额（不包含代收金额）
        /// </summary>
        public decimal TotalAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal IncomeAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 预收金额
        /// </summary>
        public decimal PrepayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 礼品卡支付金额
        /// </summary>
        public decimal GiftCardPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 代收金额
        /// </summary>
        public decimal UnionAmt
        {
            get;
            set;
        }

        /// <summary>
        /// SO单总金额
        /// </summary>
        public decimal TotalAmtForSOOnly
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? TSOTotalAmt
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 将InvoiceAmtEnitity转换成DataTable
    /// </summary>
    public static class InvoiceAmtEntityExtension
    {
        public static DataTable ToDataTable(this InvoiceAmtEntity entity)
        {
            DataTable table = new DataTable("InvoiceAmtTable");
            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("TotalAmt",typeof(decimal)),
                new DataColumn("IncomeAmt",typeof(decimal)),
                new DataColumn("PrepayAmt",typeof(decimal)),
                new DataColumn("GiftCardPayAmt",typeof(decimal)),
                new DataColumn("UnionAmt",typeof(decimal)),
                new DataColumn("TotalAmtForSOOnly",typeof(decimal)),
                new DataColumn("TSOTotalAmt",typeof(decimal))
            });

            if (entity != null)
            {
                DataRow row = table.NewRow();
                row["TotalAmt"] = entity.TotalAmt;
                row["IncomeAmt"] = entity.IncomeAmt;
                row["PrepayAmt"] = entity.PrepayAmt;
                row["GiftCardPayAmt"] = entity.GiftCardPayAmt;
                row["UnionAmt"] = entity.UnionAmt;
                row["TotalAmtForSOOnly"] = entity.TotalAmtForSOOnly;
                row["TSOTotalAmt"] = entity.TSOTotalAmt;

                table.Rows.Add(row);
            }
            return table;
        }
    }
}