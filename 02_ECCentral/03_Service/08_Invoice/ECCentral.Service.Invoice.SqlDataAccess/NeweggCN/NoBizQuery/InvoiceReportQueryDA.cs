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
    [VersionExport(typeof(IInvoiceReportQueryDA))]
    public class InvoiceReportQueryDA : IInvoiceReportQueryDA
    {
        #region IInvoiceReportQueryDA Members

        /// <summary>
        /// 发票明细表查询
        /// </summary>
        public DataTable InvoiceDetailReportQuery(InvoiceDetailReportQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (filter.PagingInfo != null)
            {
                pagingInfo.MaximumRows = filter.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
                pagingInfo.SortField = filter.PagingInfo.SortBy;
            }

            string sqlNameInConfig;
            if (filter.OrderType == "SO")
            {
                sqlNameInConfig = "QueryInvoiceDetailReport4SO";
            }
            else if (filter.OrderType == "RO")
            {
                sqlNameInConfig = "QueryInvoiceDetailReport4RO";
            }
            else if (filter.OrderType == "SHIFT")
            {
                sqlNameInConfig = "QueryInvoiceDetailReport4Shift";
            }
            else
            {
                sqlNameInConfig = "QueryInvoiceDetailReport4All";
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig(sqlNameInConfig);
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "a.OrderID DESC"))
            {
                AddParameter4InvoiceDetailReportQuery(filter, sqlBuilder);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                string suffix = GetStockSuffix(filter.StockSysNo.Value);
                dataCommand.CommandText = dataCommand.CommandText
                    .Replace("Invoice_detail", "Invoice_detail_" + suffix)
                    .Replace("Invoice_Result", "Invoice_Result_" + suffix)
                    .Replace("Invoice_TrackingNumber", "Invoice_TrackingNumber_" + suffix) + " ORDER BY OrderID ";

                result = dataCommand.ExecuteDataTable("InvoiceType", "Invoice", "InvoiceDetailReportInvoiceType");
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return result;
        }

        /// <summary>
        /// 礼品卡发票明细表查询
        /// </summary>
        public DataTable GiftInvoiceDetailReportQuery(GiftInvoiceDetaiReportQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (filter.PagingInfo != null)
            {
                pagingInfo.MaximumRows = filter.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
                pagingInfo.SortField = filter.PagingInfo.SortBy;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryGiftInvoiceDetailReport");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                    dataCommand.CommandText, dataCommand, pagingInfo, "a.SOID DESC"))
            {
                AddParameter4GiftInvoiceDetailReportQuery(filter, sqlBuilder);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                var suffix = GetStockSuffix(filter.StockSysNo.Value);
                dataCommand.CommandText = dataCommand.CommandText
                    .Replace("Invoice_detail", "Invoice_detail_" + suffix)
                    .Replace("Invoice_Result", "Invoice_Result_" + suffix)
                    .Replace("Invoice_TrackingNumber", "Invoice_TrackingNumber_" + suffix) + " ORDER BY OrderID ";

                result = dataCommand.ExecuteDataTable("InvoiceType", "Invoice", "InvoiceDetailReportInvoiceType");
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return result;
        }

        /// <summary>
        /// 移仓单明细表查询
        /// </summary>
        public DataTable ProductShiftDetailReportQuery(ProductShiftDetailReportQueryFilter filter, out int totalCount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 发票打印查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable InvoicePrintAllQuery(InvoicePrintAllQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            MapSortField4InvoicePrintAllQuery(filter);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoicePrintAll");

            dataCommand.AddInputParameter("@NEG", DbType.AnsiStringFixedLength, InvoiceType.SELF);
            dataCommand.AddInputParameter("@MET", DbType.AnsiStringFixedLength, InvoiceType.MET);

            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (filter.PagingInfo != null)
            {
                pagingInfo.MaximumRows = filter.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
                pagingInfo.SortField = filter.PagingInfo.SortBy;
            }
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "so.SysNo DESC"))
            {
                AddParameter4InvoicePrintAllQuery(filter, sqlBuilder);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("SOStatus", typeof(ECCentral.BizEntity.SO.SOStatus));

                CodeNamePairColumnList codeNamePairColunms = new CodeNamePairColumnList();
                codeNamePairColunms.Add("InvoiceType", "Invoice", "SOIsVAT");

                result = dataCommand.ExecuteDataTable(enumColumns, codeNamePairColunms);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return result;
        }

        /// <summary>
        /// 自印刷发票系统查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable InvoiceSelfPrintQuery(InvoiceSelfPrintQueryFilter filter, out int totalCount)
        {
            DataTable result = null;

            MapSelfPrintSortFileds(filter);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("InvoiceReport.GetSelfPrintInvoiceList");

            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (filter.PagingInfo != null)
            {
                pagingInfo.MaximumRows = filter.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
                pagingInfo.SortField = filter.PagingInfo.SortBy;
            }

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "sm.SysNo DESC"))
            {
                AddSelfPrintParameter(filter, sqlBuilder);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                result = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }

            return result;
        }

        /// <summary>
        /// 自印刷发票系统查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<CodeNamePair> InvoiceSelfStockQuery()
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("InvoiceReport.GetSelfPrintStocks");
            object stocks = dataCommand.ExecuteScalar();

            if (stocks != null)
            {
                List<CodeNamePair> result = new List<CodeNamePair>();
                CustomDataCommand Command = DataCommandManager.CreateCustomDataCommandFromConfig("InvoiceReport.GetSelfPrintStocksName");
                Dictionary<string, string> dir = new Dictionary<string, string>();
                using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(Command.CommandText, Command, null, "s.SysNo ASC"))
                {
                    List<string> stockSysNo = new List<string>();
                    foreach (string item in stocks.ToString().Split(','))
                    {
                        stockSysNo.Add(item.Trim());
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "s.SysNo", DbType.String, stockSysNo);
                    Command.CommandText = sqlBuilder.BuildQuerySql();
                    result = Command.ExecuteEntityList<CodeNamePair>();
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        #endregion IInvoiceReportQueryDA Members

        private Hashtable _stockSuffixMap;

        [Caching("local", ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        private string GetStockSuffix(int stockSysNo)
        {
            string cfg = AppSettingManager.GetSetting("Invoice", "InvoiceReportStockSuffixMap");
            if (_stockSuffixMap == null)
            {
                _stockSuffixMap = new Hashtable();
                if (!string.IsNullOrEmpty(cfg))
                {
                    string[] groups = cfg.Split(';');
                    foreach (var g in groups)
                    {
                        var pair = g.Split('-');
                        _stockSuffixMap.Add(pair[0], pair[1]);
                    }
                }
            }
            return (string)_stockSuffixMap[stockSysNo.ToString()];
        }

        /// <summary>
        /// 按公司编码取得“发票打印”支持的分仓
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [Caching("local", new string[] { "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        private string GetInvoicePrintStocks(string companyCode)
        {
            string cfg = AppSettingManager.GetSetting("Invoice", "InvoicePrintStocks-" + companyCode);
            if (!string.IsNullOrEmpty(cfg))
            {
                return cfg;
            }
            return String.Empty;
        }

        private void AddParameter4InvoiceDetailReportQuery(InvoiceDetailReportQueryFilter filter, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                , "OutTime"
                , DbType.DateTime
                , "@OutDateFrom"
                , QueryConditionOperatorType.MoreThanOrEqual
                , filter.OutDateFrom);

            if (filter.OutDateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "OutTime"
                    , DbType.DateTime
                    , "@OutDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , filter.OutDateTo.Value.AddDays(1).AddMinutes(-1));
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                , "InvoiceDate"
                , DbType.DateTime
                , "@InvoiceDateFrom"
                , QueryConditionOperatorType.MoreThanOrEqual
                , filter.InvoiceDateFrom);

            if (filter.InvoiceDateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "InvoiceDate"
                    , DbType.DateTime
                    , "@InvoiceDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , filter.InvoiceDateTo.Value.AddDays(1).AddMinutes(-1));
            }

            if (!string.IsNullOrEmpty(filter.OrderID))
            {
                List<string> orderIDList = new List<string>();
                foreach (string item in filter.OrderID.Split('.'))
                {
                    orderIDList.Add(item.Trim());
                }

                sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "OrderID", DbType.String, orderIDList);
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                , "InvoiceNumber"
                , DbType.String
                , "@InvoiceNumber"
                , QueryConditionOperatorType.Like
                , filter.InvoiceNumber);

            var customerNameString = filter.CustomerName;
            int intTry;
            decimal decimalTry;
            if (!string.IsNullOrEmpty(customerNameString))
            {
                if (int.TryParse(customerNameString, out intTry)
                    || decimal.TryParse(customerNameString, out decimalTry))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                        , "CustomerName"
                        , DbType.String
                        , "@CustomerName"
                        , QueryConditionOperatorType.Equal
                        , filter.CustomerName);
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                        , "CustomerName"
                        , DbType.String
                        , "@CustomerName"
                        , QueryConditionOperatorType.Like
                        , filter.CustomerName);
                }
            }
        }

        private void AddParameter4GiftInvoiceDetailReportQuery(GiftInvoiceDetaiReportQueryFilter filter, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
              , "a.OutTime"
              , DbType.DateTime
              , "@OutDateFrom"
              , QueryConditionOperatorType.MoreThanOrEqual
              , filter.OutDateFrom);

            if (filter.OutDateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "a.OutTime"
                    , DbType.DateTime
                    , "@OutDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , filter.OutDateTo.Value.AddDays(1).AddMinutes(-1));
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                , "sl.InvoiceCreateDate"
                , DbType.DateTime
                , "@InvoiceCreateTimeFrom"
                , QueryConditionOperatorType.MoreThanOrEqual
                , filter.InvoiceDateFrom);

            if (filter.InvoiceDateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "sl.InvoiceCreateDate"
                    , DbType.DateTime
                    , "@InvoiceCreateTimeTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , filter.InvoiceDateTo.Value.AddDays(1).AddMinutes(-1));
            }

            if (!string.IsNullOrEmpty(filter.OrderID))
            {
                List<string> orderIDList = new List<string>();
                foreach (string item in filter.OrderID.Split('.'))
                {
                    orderIDList.Add(item.Trim());
                }

                sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "a.SOID", DbType.String, orderIDList);
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                , "sl.InvoiceNo"
                , DbType.String
                , "@InvoiceNumber"
                , QueryConditionOperatorType.Like
                , filter.InvoiceNumber);

            int intTry;
            decimal decimalTry;
            if (!string.IsNullOrEmpty(filter.CustomerName))
            {
                if (int.TryParse(filter.CustomerName, out intTry)
                    || decimal.TryParse(filter.CustomerName, out decimalTry))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                        , "ct.CustomerName"
                        , DbType.String
                        , "@CustomerName"
                        , QueryConditionOperatorType.Equal
                        , filter.CustomerName);
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                        , "ct.CustomerName"
                        , DbType.String
                        , "@CustomerName"
                        , QueryConditionOperatorType.LeftLike
                        , filter.CustomerName);
                }
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                , "scs.SOType"
                , DbType.Int32
                , "@SOType"
                , QueryConditionOperatorType.Equal
                , filter.SOType);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                   , "a.CompanyCode"
                   , DbType.AnsiStringFixedLength
                   , "@CompanyCode"
                   , QueryConditionOperatorType.Equal
                   , filter.CompanyCode);
        }

        private void AddParameter4InvoicePrintAllQuery(InvoicePrintAllQueryFilter filter, DynamicQuerySqlBuilder sqlBuilder)
        {
            //SOSysNo
            var sosysNoString = filter.SOSysNo;
            if (!string.IsNullOrEmpty(sosysNoString))
            {
                List<int> sosysNOList = new List<int>();
                foreach (string item in sosysNoString.Split('.'))
                {
                    sosysNOList.Add(int.Parse(item.Trim()));
                }
                sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "so.SysNo", DbType.Int32, sosysNOList);
            }

            //SOStatusCode(已出库)
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                , "so.Status"
                , DbType.Int32
                , "@SOStatusCode"
                , QueryConditionOperatorType.Equal
                , ECCentral.BizEntity.SO.SOStatus.OutStock);

            //IsVAT是否增票
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
               , "so.IsVAT"
               , DbType.Int32
               , "@IsVAT"
               , QueryConditionOperatorType.Equal
               , filter.IsVAT);

            //CreateDateFrom
            var createdateFrom = filter.CreateDateFrom;
            if (createdateFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "so.OrderDate"
                    , DbType.DateTime
                    , "@CreateDateFrom"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , createdateFrom.Value);
            }
            //CreateDateTo
            var createdateTo = filter.CreateDateTo;
            if (createdateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "so.OrderDate"
                    , DbType.DateTime
                    , "@CreateDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , createdateTo.Value.AddDays(1).AddSeconds(-1));
            }

            //OutDateFrom
            var outdateFrom = filter.OutDateFrom;
            if (outdateFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                   , "so.OutTime"
                   , DbType.DateTime
                   , "@OutDateFrom"
                   , QueryConditionOperatorType.MoreThanOrEqual
                   , outdateFrom.Value);
            }
            //OutDateTo
            var outdateTo = filter.OutDateTo;
            if (outdateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "so.OutTime"
                    , DbType.DateTime
                    , "@OutDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , outdateTo.Value.AddDays(1).AddSeconds(-1));
            }

            //AuditDateFrom
            var auditDateFrom = filter.AuditDateFrom;
            if (auditDateFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "so.AuditTime"
                    , DbType.DateTime
                    , "@AuditDateFrom"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , auditDateFrom.Value);
            }
            //AuditDateTo
            var auditdateTo = filter.AuditDateTo;
            if (auditdateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "so.AuditTime"
                    , DbType.DateTime
                    , "@AuditDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , auditdateTo.Value.AddDays(1).AddSeconds(-1));
            }

            //StockSysNo
            var stockSysNOString = filter.StockSysNo;
            if (string.IsNullOrEmpty(stockSysNOString))
            {
                //1:全部查询:只查询“发票打印”支持的分仓,从配置文件中取值，即StockSysNO="51,59,90,99"；
                string invoicePrintstocks = this.GetInvoicePrintStocks(filter.CompanyCode);
                List<string> stockNOList = new List<string>();
                foreach (string item in invoicePrintstocks.Split(','))
                {
                    stockNOList.Add(item.Trim());
                }
                sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND
                    , "sb.WareHouseNumber"
                    , DbType.AnsiStringFixedLength
                    , stockNOList);
            }
            else
            {
                //非全部查询
                if (stockSysNOString.IndexOf(',') > 0)
                {
                    //2:上海仓:实际查询为“嘉定仓”+“宝山仓”,即StockSysNO="51,59"
                    List<string> stockNOList = new List<string>();
                    foreach (string item in stockSysNOString.Split(','))
                    {
                        stockNOList.Add(item.Trim());
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND
                        , "sb.WareHouseNumber"
                        , DbType.AnsiStringFixedLength
                        , stockNOList);
                }
                else
                {
                    //3: 其他仓:即StockSysNO="90" 或者 StockSysNo="99"
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                        , "sb.WareHouseNumber"
                        , DbType.AnsiStringFixedLength
                        , "@StockSysNo"
                        , QueryConditionOperatorType.Equal
                        , stockSysNOString);
                }
            }

            //MerchantSysNo
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
               , "scs.MerchantSysNo"
               , DbType.Int32
               , "@MerchantSysNo"
               , QueryConditionOperatorType.Equal
               , filter.VendorSysNo);

            //InvoiceType
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                  , "scs.InvoiceType"
                  , DbType.AnsiStringFixedLength
                  , "@InvoiceType"
                  , QueryConditionOperatorType.Equal
                  , filter.InvoiceType);

            //StockType
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                  , "scs.StockType"
                  , DbType.AnsiStringFixedLength
                  , "@StockType"
                  , QueryConditionOperatorType.Equal
                  , filter.StockType);

            //ShippingType
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                  , "scs.ShippingType"
                  , DbType.AnsiStringFixedLength
                  , "@ShippingType"
                  , QueryConditionOperatorType.Equal
                  , filter.ShippingType);

            //CompanyCode
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                   , "sb.CompanyCode"
                   , DbType.AnsiStringFixedLength
                   , "@CompanyCode"
                   , QueryConditionOperatorType.Equal
                   , filter.CompanyCode);
        }

        private void MapSortField4InvoicePrintAllQuery(InvoicePrintAllQueryFilter filter)
        {
            if (filter.PagingInfo != null && !string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0, filter.PagingInfo.SortBy.Length - index);
                var sortField = filter.PagingInfo.SortBy;
                switch (sort)
                {
                    case "SOSysNo":
                        filter.PagingInfo.SortBy = sortField.Replace("SOSysNo", "so.SysNo");
                        break;
                    case "WareHouseNumber":
                        filter.PagingInfo.SortBy = sortField.Replace("WareHouseNumber", "sb.WareHouseNumber");
                        break;
                    case "IsVAT":
                        filter.PagingInfo.SortBy = sortField.Replace("IsVAT", "so.IsVAT");
                        break;
                    case "SOStatus":
                        filter.PagingInfo.SortBy = sortField.Replace("SOStatus", "so.Status");
                        break;
                    case "SOTotalAmt":
                        filter.PagingInfo.SortBy = sortField.Replace("SOTotalAmt", "sb.SOTotalAmt");
                        break;
                    case "SOAmt":
                        filter.PagingInfo.SortBy = sortField.Replace("SOAmt", "sb.SOAmt");
                        break;
                    case "OtherAmt":
                        filter.PagingInfo.SortBy = sortField.Replace("OtherAmt", "sb.ShippingCharge + sb.PayPrice + sb.PremiumAmt");
                        break;
                    case "OrderDate":
                        filter.PagingInfo.SortBy = sortField.Replace("OrderDate", "so.OrderDate");
                        break;
                    case "OutTime":
                        filter.PagingInfo.SortBy = sortField.Replace("OutTime", "so.OutTime");
                        break;
                    case "AuditTime":
                        filter.PagingInfo.SortBy = sortField.Replace("AuditTime", "so.AuditTime");
                        break;
                }
            }
        }

        private static void MapSelfPrintSortFileds(InvoiceSelfPrintQueryFilter filter)
        {
            if (filter.PagingInfo != null && !String.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0, filter.PagingInfo.SortBy.Length - index);
                var sortField = filter.PagingInfo.SortBy;
                switch (sort)
                {
                    case "OrderID":
                        filter.PagingInfo.SortBy = sortField.Replace("OrderID", "sm.SysNo");
                        break;
                    case "OutTime":
                        filter.PagingInfo.SortBy = sortField.Replace("OutTime", "sm.OutTime");
                        break;
                    case "InvoiceNumber":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceNumber", "im.InvoiceNumber");
                        break;
                    case "InvoiceAmount":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceAmount", "im.Amount");
                        break;
                    case "InvoiceType":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceType", "im.InvoiceType");
                        break;
                    case "InvoiceCode":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceCode", "im.InvoiceCode");
                        break;
                    case "InvoiceDate":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceDate", "im.PrintDate");
                        break;
                    case "IsNegativeInvoice":
                        filter.PagingInfo.SortBy = sortField.Replace("IsNegativeInvoice", "im.IsNegativeInvoice");
                        break;
                    case "IsRePrint":
                        filter.PagingInfo.SortBy = sortField.Replace("IsRePrint", "im.IsRePrint");
                        break;
                    default: break;
                }
            }
        }

        private void AddSelfPrintParameter(InvoiceSelfPrintQueryFilter filter, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                       , "sm.Status"
                       , DbType.String
                       , "@Status"
                       , QueryConditionOperatorType.Equal
                       , "4");

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                       , "sm.IsVAT"
                       , DbType.Int32
                       , "@IsVAT"
                       , QueryConditionOperatorType.Equal
                       , 0);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                   , "sm.CompanyCode"
                   , DbType.String
                   , "@CompanyCode"
                   , QueryConditionOperatorType.Equal
                   , filter.CompanyCode);

            var warehouseNumber = filter.StockSysNo;
            if (warehouseNumber.HasValue & filter.StockSysNo!=0)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                       , "subso.WareHouseNumber"
                       , DbType.String
                       , "@WarehouseNumber"
                       , QueryConditionOperatorType.Equal
                       , filter.StockSysNo);
            }
            else
            {
                CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("InvoiceReport.GetSelfPrintStocks");
                string stocks = dataCommand.ExecuteScalar().ToString();
                if (!String.IsNullOrEmpty(stocks))
                {
                    List<string> stockSysNo = new List<string>();
                    foreach (string item in stocks.Split(','))
                    {
                        stockSysNo.Add(item.Trim());
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "subso.WareHouseNumber", DbType.String, stockSysNo);
                }
            }

            var outdateFrom = filter.OutDateFrom;
            if (outdateFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "sm.OutTime"
                    , DbType.DateTime
                    , "@OutDateFrom"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , filter.OutDateFrom);
            }
            var outdateTo = filter.OutDateTo;
            if (outdateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "sm.OutTime"
                    , DbType.DateTime
                    , "@OutDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , filter.OutDateTo);
            }

            var invoicedateFrom = filter.InvoiceDateFrom;
            if (invoicedateFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "im.PrintDate"
                    , DbType.DateTime
                    , "@InvoiceDateFrom"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , filter.InvoiceDateFrom);
            }

            var invoicedateTo = filter.InvoiceDateTo;
            if (invoicedateTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "im.PrintDate"
                    , DbType.DateTime
                    , "@InvoiceDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , invoicedateTo.Value.AddDays(1).AddMinutes(-1));
            }

            var orderIDString = filter.OrderID;
            if (!string.IsNullOrEmpty(orderIDString))
            {
                List<string> orderIDList = new List<string>();
                foreach (string item in orderIDString.Split('.'))
                {
                    orderIDList.Add(item.Trim());
                }

                sqlBuilder.ConditionConstructor.AddInCondition<string>(QueryConditionRelationType.AND, "sm.SysNo", DbType.String, orderIDList);
            }

            var invoiceNumber = filter.InvoiceNumber;
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "im.InvoiceNumber"
                    , DbType.String
                    , "@InvoiceNumber"
                    , QueryConditionOperatorType.Like
                    , filter.InvoiceNumber);
            }
        }
    }
}