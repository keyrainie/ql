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
    [VersionExport(typeof(IPayItemQueryDA))]
    public class PayItemQueryDA : IPayItemQueryDA
    {
        #region IPayItemQueryDA Members

        public DataSet Query(PayItemQueryFilter filter, out int totalCount)
        {
            if (filter.IsFilterPOETP)
            {
                return GetPOETPPayItemList(filter, out totalCount);
            }

            switch (filter.OrderType)
            {
                case null:
                    return GetAllPayItemList(filter, out totalCount);
                case PayableOrderType.PO:
                //case PayableOrderType.POTempConsign:
                case PayableOrderType.POAdjust:
                    return GetPOPayItemList(filter, out totalCount);
                case PayableOrderType.VendorSettleOrder:
                case PayableOrderType.LeaseSettle:
                    return GetVendorSettleOrderPayItemList(filter, out totalCount);
                //case PayableOrderType.SubAccount:
                //case PayableOrderType.ReturnPointCashAdjust:
                //case PayableOrderType.SubInvoice:
                //    return GetReturnPointCashSettlePayItemList(filter, out totalCount);
                case PayableOrderType.RMAPOR:
                    return GetRMAPORPayItemList(filter, out totalCount);
                case PayableOrderType.CollectionSettlement:
                    return GetCollectionSettleOrderPayItemList(filter, out totalCount);
                case PayableOrderType.Commission:
                    return GetCommissionMasterOrderPayItemList(filter, out totalCount);
                case PayableOrderType.CollectionPayment:
                    return GetCollenctionPaymentPayItemList(filter, out totalCount);
                case PayableOrderType.CostChange:
                    return GetCCPayItemList(filter, out totalCount);
                default:
                    return GetCommonPayItemList(filter, out totalCount);
            }
        }    

        #endregion IPayItemQueryDA Members

        /// <summary>
        /// 取得到期的未付款PO明细
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private DataSet GetPOETPPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOETPPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "PayItemSysNo desc"))
            {
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                StringBuilder publicQueryCondition = new StringBuilder();

                publicQueryCondition.Append(" AND payitem.CompanyCode = @CompanyCode");
                dataCommand.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, filter.CompanyCode);

                if (filter.Status != null)
                {
                    publicQueryCondition.Append(" AND payitem.Status = @Status");
                    dataCommand.AddInputParameter("@Status", DbType.Int32, filter.Status.Value);
                }
                if (filter.PayStyle != null)
                {
                    publicQueryCondition.Append(" AND payitem.PayStyle = @PayStyle");
                    dataCommand.AddInputParameter("@PayStyle", DbType.Int32, filter.PayStyle.Value);
                }
                if (filter.VendorSysNo != null)
                {
                    publicQueryCondition.Append(" AND vendor.SysNo = @VendorSysNo");
                    dataCommand.AddInputParameter("@VendorSysNo", DbType.Int32, filter.VendorSysNo.Value);
                }
                if (filter.UserID != null)
                {
                    publicQueryCondition.Append(" AND payItem.CreateUserSysNo = @UserSysNo");
                    dataCommand.AddInputParameter("@UserSysNo", DbType.Int32, filter.UserID.Value);
                }
                if (filter.InvoiceStatus != null)
                {
                    publicQueryCondition.Append(" AND pay.InvoiceStatus = @InvoiceStatus");
                    dataCommand.AddInputParameter("@InvoiceStatus", DbType.Int32, filter.InvoiceStatus.Value);
                }
                if (filter.EstimatePayDateFrom != null)
                {
                    publicQueryCondition.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    dataCommand.AddInputParameter("@EstimatePayDateFrom", DbType.DateTime, filter.EstimatePayDateFrom.Value);
                }
                if (filter.EstimatePayDateTo != null)
                {
                    publicQueryCondition.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    dataCommand.AddInputParameter("@EstimatePayDateTo", DbType.DateTime, filter.EstimatePayDateTo);
                }
                if (filter.PayDateFrom != null)
                {
                    publicQueryCondition.Append(" AND payitem.PayTime >= @PayDateFrom");
                    dataCommand.AddInputParameter("@PayDateFrom", DbType.DateTime, filter.PayDateFrom.Value);
                }
                if (filter.PayDateTo != null)
                {
                    publicQueryCondition.Append(" AND payitem.PayTime < @PayDateTo");
                    dataCommand.AddInputParameter("@PayDateTo", DbType.DateTime, filter.PayDateTo);
                }
                if (filter.InvoiceEditDateFrom != null)
                {
                    publicQueryCondition.Append(" AND pay.InvoiceUpdateTime >= @InvoiceEditDateFrom");
                    dataCommand.AddInputParameter("@InvoiceEditDateFrom", DbType.DateTime, filter.InvoiceEditDateFrom.Value);
                }
                if (filter.InvoiceEditDateTo != null)
                {
                    publicQueryCondition.Append(" AND pay.InvoiceUpdateTime < @InvoiceEditDateTo");
                    dataCommand.AddInputParameter("@InvoiceEditDateTo", DbType.DateTime, filter.InvoiceEditDateTo);
                }
                if (!string.IsNullOrEmpty(filter.Note))
                {
                    publicQueryCondition.Append(" AND payitem.Note LIKE @Note");
                    dataCommand.AddInputParameter("@Note", DbType.String, "%" + filter.Note.Trim() + "%");
                }
                if (!string.IsNullOrEmpty(filter.ReferenceID))
                {
                    publicQueryCondition.Append(" AND payitem.ReferenceID LIKE @ReferenceID");
                    dataCommand.AddInputParameter("@ReferenceID", DbType.String, "%" + filter.ReferenceID.Trim() + "%");
                }
                if (filter.IsFilterAbandonItem)
                {
                    publicQueryCondition.Append(" AND payitem.Status <> @AbandonStatus");
                    dataCommand.AddInputParameter("@AbandonStatus", DbType.Int32, -1);
                }
                if (filter.PaySettleCompany.HasValue)
                {
                    publicQueryCondition.Append(" AND vendor.PaySettleCompany = @PaySettleCompany");
                    dataCommand.AddInputParameter("@PaySettleCompany", DbType.Int32, filter.PaySettleCompany.Value);
                }

                StringBuilder sb1 = new StringBuilder();
                sb1.Append(" WHERE po.IsConsign <> 1 and po.POType<>1 AND pay.OrderType in(0,5,7) and po.Status <> 0");
                StringBuilder sb2 = new StringBuilder();
                sb2.Append(" WHERE pay.OrderType = 1 AND vendorsettle.status<>0 AND vendorsettle.settletime >= dateadd(day,-50,getdate())");
                StringBuilder sb3 = new StringBuilder();
                sb3.Append(" WHERE po.IsConsign <> 1 and ((po.POType=1 and pay.OrderType = 0) or ( pay.OrderType in(5,7))) and po.Status <> 0 and po.intime >=dateadd(day,-50,getdate())");
                StringBuilder sb4 = new StringBuilder();
                sb4.Append(" WHERE pay.OrderType = 8 and invoice.CreateDate >= dateadd(day,-50,getdate())");
                StringBuilder sb5 = new StringBuilder();
                sb5.Append(" WHERE pay.OrderType = 9 AND RMAPOR.PMDAuditTime >= dateadd(day,-50,getdate())");
                if (filter.OrderType != null)
                {
                    sb1.Append(" AND po.IsConsign = 0  AND pay.OrderType = @OrderType ");
                    sb5.Append(" AND 1 <> 1");
                    dataCommand.AddInputParameter("@OrderType", DbType.Int32, filter.OrderType.Value);
                }
                if (filter.CreateDateFrom != null)
                {
                    sb1.Append(" AND po.InTime >= @CreateDateFrom");
                    sb2.Append(" AND vendorSettle.SettleTime >= @CreateDateFrom");
                    sb3.Append(" AND po.InTime >= @CreateDateFrom");
                    sb4.Append(" AND invoice.CreateDate >= @CreateDateFrom");
                    sb5.Append(" AND RMAPOR.PMDAuditTime >= @CreateDateFrom");
                    dataCommand.AddInputParameter("@CreateDateFrom", DbType.DateTime, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sb1.Append(" AND po.InTime < @CreateDateTo");
                    sb2.Append(" AND vendorSettle.SettleTime < @CreateDateTo");
                    sb3.Append(" AND po.InTime < @CreateDateTo");
                    sb4.Append(" AND invoice.CreateDate < @CreateDateTo");
                    sb5.Append(" AND RMAPOR.PMDAuditTime < @CreateDateTo");
                    dataCommand.AddInputParameter("@CreateDateTo", DbType.DateTime, filter.CreateDateTo);
                }
                if (filter.StockSysNo != null)
                {
                    sb1.Append(" AND po.PayStockSysNo = @StockSysNo");
                    sb2.Append(" AND vendorSettle.StockSysNo = @StockSysNo");
                    sb3.Append(" AND po.PayStockSysNo = @StockSysNo");
                    dataCommand.AddInputParameter("@StockSysNo", DbType.Int32, filter.StockSysNo);
                    if (filter.StockSysNo != 51)
                    {
                        sb5.Append(" AND 1 <> 1");
                    }
                }
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<Int32> list = new List<int>();
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        list.Add(Int32.Parse(orderIDArray[i]));
                    }
                    if (list != null && list.Count > 0)
                    {
                        int i = 0;
                        StringBuilder str1 = new StringBuilder();
                        StringBuilder str2 = new StringBuilder();
                        StringBuilder str3 = new StringBuilder();
                        StringBuilder str4 = new StringBuilder();
                        StringBuilder str5 = new StringBuilder();
                        str1.Append(" AND po.SysNo IN(");
                        str2.Append(" AND vendorSettle.SysNo IN(");
                        str3.Append(" AND po.SysNo IN(");
                        str4.Append(" AND invoice.invoiceNumber IN(");
                        str5.Append(" AND RMAPOR.SysNo IN(");
                        foreach (var sysNo in list)
                        {
                            string paramSysNo = "@SysNo" + i++;
                            str1.Append(paramSysNo + ",");
                            str2.Append(paramSysNo + ",");
                            str3.Append(paramSysNo + ",");
                            str4.Append(paramSysNo + ",");
                            str5.Append(paramSysNo + ",");
                            dataCommand.AddInputParameter(paramSysNo, DbType.Int32, sysNo);
                        }
                        sb1.Append(str1.ToString().Substring(0, str1.Length - 1) + " ) ");
                        sb2.Append(str2.ToString().Substring(0, str2.Length - 1) + " ) ");
                        sb3.Append(str3.ToString().Substring(0, str3.Length - 1) + " ) ");
                        sb4.Append(str4.ToString().Substring(0, str4.Length - 1) + " ) ");
                        sb5.Append(str5.ToString().Substring(0, str5.Length - 1) + " ) ");
                    }
                }

                if (filter.IsFilterPOETP)
                {
                    sb1.Append(" AND po.Status <> @POAbandonStatus");
                    sb2.Append(" AND vendorSettle.Status <> @POAbandonStatus");
                    dataCommand.AddInputParameter("@POAbandonStatus", DbType.Int32, ECCentral.BizEntity.PO.PurchaseOrderStatus.Abandoned);
                    if (filter.ETPFrom != null)
                    {
                        sb1.Append(" AND pay.ETP >= @ETPFrom");
                        dataCommand.AddInputParameter("@ETPFrom", DbType.DateTime, filter.ETPFrom);
                    }
                    if (filter.ETPTo != null)
                    {
                        sb1.Append(" AND pay.ETP < @ETPTo");
                        dataCommand.AddInputParameter("@ETPTo", DbType.DateTime, filter.ETPTo);
                    }
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Decimal, 50);

                dataCommand.CommandText = dataCommand.CommandText
                   .Replace("#StrWhere1#", sb1.ToString() + publicQueryCondition)
                   .Replace("#StrWhere2#", sb2.ToString() + publicQueryCondition)
                   .Replace("#StrWhere3#", sb3.ToString() + publicQueryCondition)
                   .Replace("#StrWhere4#", sb4.ToString() + publicQueryCondition)
                   .Replace("#StrWhere5#", sb5.ToString() + publicQueryCondition);

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetAllPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAllPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "PayItemSysNo desc"))
            {
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                StringBuilder publicQueryCondition = new StringBuilder();

                publicQueryCondition.Append(" AND payitem.CompanyCode = @CompanyCode");
                dataCommand.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, filter.CompanyCode);

                if (filter.Status != null)
                {
                    publicQueryCondition.Append(" AND payitem.Status = @Status");
                    dataCommand.AddInputParameter("@Status", DbType.Int32, filter.Status.Value);
                }
                if (filter.PayStyle != null)
                {
                    publicQueryCondition.Append(" AND payitem.PayStyle = @PayStyle");
                    dataCommand.AddInputParameter("@PayStyle", DbType.Int32, filter.PayStyle.Value);
                }
                if (filter.VendorSysNo != null)
                {
                    publicQueryCondition.Append(" AND vendor.SysNo = @VendorSysNo");
                    dataCommand.AddInputParameter("@VendorSysNo", DbType.Int32, filter.VendorSysNo.Value);
                }

                if (filter.InvoiceStatus != null)
                {
                    publicQueryCondition.Append(" AND pay.InvoiceStatus = @InvoiceStatus");
                    dataCommand.AddInputParameter("@InvoiceStatus", DbType.Int32, filter.InvoiceStatus.Value);
                }

                if (filter.PayDateFrom != null)
                {
                    publicQueryCondition.Append(" AND payitem.PayTime >= @PayDateFrom");
                    dataCommand.AddInputParameter("@PayDateFrom", DbType.DateTime, filter.PayDateFrom.Value);
                }
                if (filter.PayDateTo != null)
                {
                    publicQueryCondition.Append(" AND payitem.PayTime < @PayDateTo");
                    dataCommand.AddInputParameter("@PayDateTo", DbType.DateTime, filter.PayDateTo);
                }
                if (filter.InvoiceEditDateFrom != null)
                {
                    publicQueryCondition.Append(" AND pay.InvoiceUpdateTime >= @InvoiceEditDateFrom");
                    dataCommand.AddInputParameter("@InvoiceEditDateFrom", DbType.DateTime, filter.InvoiceEditDateFrom.Value);
                }
                if (filter.InvoiceEditDateTo != null)
                {
                    publicQueryCondition.Append(" AND pay.InvoiceUpdateTime < @InvoiceEditDateTo");
                    dataCommand.AddInputParameter("@InvoiceEditDateTo", DbType.DateTime, filter.InvoiceEditDateTo);
                }
                if (!string.IsNullOrEmpty(filter.Note))
                {
                    publicQueryCondition.Append(" AND payitem.Note LIKE @Note");
                    dataCommand.AddInputParameter("@Note", DbType.String, "%" + filter.Note.Trim() + "%");
                }
                if (!string.IsNullOrEmpty(filter.ReferenceID))
                {
                    publicQueryCondition.Append(" AND payitem.ReferenceID LIKE @ReferenceID");
                    dataCommand.AddInputParameter("@ReferenceID", DbType.String, "%" + filter.ReferenceID.Trim() + "%");
                }
                if (filter.IsFilterAbandonItem)
                {
                    publicQueryCondition.Append(" AND payitem.Status <> @AbandonStatus");
                    dataCommand.AddInputParameter("@AbandonStatus", DbType.Int32, -1);
                }
                //付款结算公司
                if (filter.PaySettleCompany.HasValue)
                {
                    publicQueryCondition.Append(" AND vendor.PaySettleCompany = @PaySettleCompany");
                    dataCommand.AddInputParameter("@PaySettleCompany", DbType.Int32, filter.PaySettleCompany.Value);
                }

                StringBuilder sb1 = new StringBuilder();
                sb1.Append(" WHERE pay.OrderType IN(0,4,5) AND po.IsConsign <> 1");
                StringBuilder sb2 = new StringBuilder();
                sb2.Append(" WHERE pay.OrderType = 1");
                StringBuilder sb3 = new StringBuilder();
                sb3.Append(" WHERE pay.OrderType IN(2,3)");
                StringBuilder sb4 = new StringBuilder();
                sb4.Append(" WHERE pay.OrderType IN(6,7,8)");
                StringBuilder sb5 = new StringBuilder();
                sb5.Append(" WHERE pay.OrderType = 9");
                //代收结算单
                StringBuilder sb6 = new StringBuilder();
                sb6.Append(" WHERE pay.OrderType = 10");
                //佣金账扣单
                StringBuilder sb7 = new StringBuilder();
                sb7.Append(" WHERE pay.OrderType = 11");
                //代收代付结算单
                StringBuilder sb8 = new StringBuilder();
                sb8.Append(" WHERE pay.OrderType = 12");
                //成本变价单
                StringBuilder sb9 = new StringBuilder();
                sb9.Append(" WHERE pay.OrderType = 16");

                if (filter.NotInStock)
                {
                    sb1.Append("  AND pay.InstockAmt IS NULL");
                    sb2.Append("  AND pay.InstockAmt IS NULL");
                    sb3.Append("  AND pay.InstockAmt IS NULL");
                    sb4.Append("  AND pay.InstockAmt IS NULL");
                    sb5.Append("  AND pay.InstockAmt IS NULL");
                    sb6.Append("  AND pay.InstockAmt IS NULL");
                    sb7.Append("  AND pay.InstockAmt IS NULL");
                    sb8.Append("  AND pay.InstockAmt IS NULL");
                    sb9.Append("  AND pay.InstockAmt IS NULL");
                }

                if (filter.EstimatePayDateFrom != null)
                {
                    sb1.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    sb2.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    sb3.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    sb4.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    sb5.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    sb6.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    sb7.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    sb8.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    sb9.Append(" AND payitem.EstimatePayTime >= @EstimatePayDateFrom");
                    dataCommand.AddInputParameter("@EstimatePayDateFrom", DbType.DateTime, filter.EstimatePayDateFrom.Value);
                }
                if (filter.EstimatePayDateTo != null)
                {
                    sb1.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    sb2.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    sb3.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    sb4.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    sb5.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    sb6.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    sb7.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    sb8.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    sb9.Append(" AND payitem.EstimatePayTime < @EstimatePayDateTo");
                    dataCommand.AddInputParameter("@EstimatePayDateTo", DbType.DateTime, filter.EstimatePayDateTo);
                }
                if (filter.CreateDateFrom != null)
                {
                    sb1.Append(" AND payitem.CreateTime >= @CreateDateFrom");
                    sb2.Append(" AND vendorSettle.SettleTime >= @CreateDateFrom");
                    sb3.Append(" AND balancesettle.CreateTime >= @CreateDateFrom");
                    sb4.Append(" AND invoice.CreateDate >= @CreateDateFrom");
                    sb5.Append(" AND RMAPOR.PMDAuditTime >= @CreateDateFrom");
                    sb6.Append(" AND collectionSettle.InDate >= @CreateDateFrom");
                    sb7.Append(" AND commissionMaster.InDate >= @CreateDateFrom");
                    sb8.Append(" AND paymentsettle.CreateTime >= @CreateDateFrom");
                    sb9.Append(" AND paymentsettle.CreateTime >= @CreateDateFrom");
                    dataCommand.AddInputParameter("@CreateDateFrom", DbType.DateTime, filter.CreateDateFrom.Value);
                }
                if (filter.CreateDateTo != null)
                {
                    sb1.Append(" AND payitem.CreateTime < @CreateDateTo");
                    sb2.Append(" AND vendorSettle.SettleTime < @CreateDateTo");
                    sb3.Append(" AND balancesettle.CreateTime < @CreateDateTo");
                    sb4.Append(" AND invoice.CreateDate < @CreateDateTo");
                    sb5.Append(" AND RMAPOR.PMDAuditTime < @CreateDateTo");
                    sb6.Append(" AND collectionSettle.InDate < @CreateDateTo");
                    sb7.Append(" AND commissionMaster.InDate < @CreateDateTo");
                    sb8.Append(" AND paymentsettle.CreateTime < @CreateDateTo");
                    sb9.Append(" AND paymentsettle.CreateTime < @CreateDateTo");
                    dataCommand.AddInputParameter("@CreateDateTo", DbType.DateTime, filter.CreateDateTo);
                }
                if (filter.StockSysNo != null)
                {
                    sb1.Append(" AND po.PayStockSysNo = @StockSysNo");
                    sb2.Append(" AND vendorSettle.StockSysNo = @StockSysNo");
                    sb3.Append(" AND balancesettle.StockSysNo = @StockSysNo");
                    sb6.Append(" AND collectionSettle.StockSysNo = @StockSysNo");
                    sb8.Append(" AND paymentsettle.StockSysNo = @StockSysNo");
                    dataCommand.AddInputParameter("@StockSysNo", DbType.Int32, filter.StockSysNo);
                }

                if (filter.UserID != null)
                {
                    sb1.Append(" AND po.pmsysno = @UserID");
                    sb2.Append(" AND vendorsettle.pmsysno = @UserID");
                    sb8.Append(" AND paymentsettle.PMSysNo = @UserID");
                    sb9.Append(" AND cc.PMSysNo = @UserID");

                    dataCommand.AddInputParameter("@UserID", DbType.Int32, filter.UserID);
                }

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    List<Int32> list = new List<int>();
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        list.Add(Int32.Parse(orderIDArray[i]));
                    }
                    if (list != null && list.Count > 0)
                    {
                        int i = 0;
                        StringBuilder str1 = new StringBuilder();
                        StringBuilder str2 = new StringBuilder();
                        StringBuilder str3 = new StringBuilder();
                        StringBuilder str4 = new StringBuilder();
                        StringBuilder str5 = new StringBuilder();
                        StringBuilder str6 = new StringBuilder();
                        StringBuilder str7 = new StringBuilder();
                        StringBuilder str8 = new StringBuilder();
                        StringBuilder str9 = new StringBuilder();
                        str1.Append(" AND po.SysNo IN(");
                        str2.Append(" AND vendorSettle.SysNo IN(");
                        str3.Append(" AND balancesettle.SysNo IN(");
                        str4.Append(" AND invoice.invoiceNumber IN(");
                        str5.Append(" AND RMAPOR.SysNo IN(");
                        str6.Append(" AND collectionSettle.SysNo IN(");
                        str7.Append(" AND commissionMaster.SysNo IN(");
                        str8.Append(" AND paymentsettle.SysNo IN(");
                        str9.Append(" AND cc.SysNo IN(");
                        foreach (var sysNo in list)
                        {
                            string paramSysNo = "@SysNo" + i++;
                            str1.Append(paramSysNo + ",");
                            str2.Append(paramSysNo + ",");
                            str3.Append(paramSysNo + ",");
                            str4.Append(paramSysNo + ",");
                            str5.Append(paramSysNo + ",");
                            str6.Append(paramSysNo + ",");
                            str7.Append(paramSysNo + ",");
                            str8.Append(paramSysNo + ",");
                            str9.Append(paramSysNo + ",");
                            dataCommand.AddInputParameter(paramSysNo, DbType.Int32, sysNo);
                        }
                        sb1.Append(str1.ToString().Substring(0, str1.Length - 1) + " ) ");
                        sb2.Append(str2.ToString().Substring(0, str2.Length - 1) + " ) ");
                        sb3.Append(str3.ToString().Substring(0, str3.Length - 1) + " ) ");
                        sb4.Append(str4.ToString().Substring(0, str4.Length - 1) + " ) ");
                        sb5.Append(str5.ToString().Substring(0, str5.Length - 1) + " ) ");
                        sb6.Append(str6.ToString().Substring(0, str6.Length - 1) + " ) ");
                        sb7.Append(str7.ToString().Substring(0, str7.Length - 1) + " ) ");
                        sb8.Append(str8.ToString().Substring(0, str8.Length - 1) + " ) ");
                        sb9.Append(str8.ToString().Substring(0, str8.Length - 1) + " ) ");
                    }
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);

                dataCommand.CommandText = dataCommand.CommandText
                   .Replace("#StrWhere1#", sb1.ToString() + publicQueryCondition)
                   .Replace("#StrWhere2#", sb2.ToString() + publicQueryCondition)
                   .Replace("#StrWhere3#", sb3.ToString() + publicQueryCondition)
                   .Replace("#StrWhere4#", sb4.ToString() + publicQueryCondition)
                   .Replace("#StrWhere5#", sb5.ToString() + publicQueryCondition)
                   .Replace("#StrWhere6#", sb6.ToString() + publicQueryCondition)
                   .Replace("#StrWhere7#", sb7.ToString() + publicQueryCondition)
                   .Replace("#StrWhere8#", sb8.ToString() + publicQueryCondition)
                   .Replace("#StrWhere9#", sb9.ToString() + publicQueryCondition);

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetCommonPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommonPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "biz.SysNo", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                    DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CreateUserSysNo",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.CreateTime",
                    DbType.DateTime, "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.CreateTime",
                     DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "biz.StockSysNo",
                    DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, filter.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                //if (filter.PaySettleCompany.HasValue)
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                //        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                //}

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetCommissionMasterOrderPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommissionMasterOrderPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "commissionMaster.SysNo", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                    DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CreateUserSysNo",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "commissionMaster.InDate",
                    DbType.DateTime, "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "commissionMaster.InDate",
                     DbType.DateTime, "@SettleTimeTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (filter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetCollenctionPaymentPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCollectionPaymentPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "paymentsettle.SysNo", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                    DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CreateUserSysNo",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "paymentsettle.CreateTime",
                    DbType.DateTime, "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "paymentsettle.CreateTime",
                     DbType.DateTime, "@SettleTimeTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "paymentsettle.StockSysNo",
                    DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, filter.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (filter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetCollectionSettleOrderPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCollectionSettleOrderPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "collectionSettle.SysNo", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                    DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CreateUserSysNo",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "collectionSettle.SettleDate",
                    DbType.DateTime, "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "collectionSettle.SettleDate",
                     DbType.DateTime, "@SettleTimeTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "collectionSettle.StockSysNo",
                    DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, filter.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (filter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetRMAPORPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetRMAPORPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "RMAPOR.SysNo", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                   DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CreateUserSysNo",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMAPOR.PMDAuditTime",
                    DbType.DateTime, "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMAPOR.PMDAuditTime",
                     DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                     DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (filter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetReturnPointCashSettlePayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetReturnPointCashSettlePayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "invoice.invoiceNumber", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                    DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CreateUserSysNo",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "invoice.CreateDate",
                    DbType.DateTime, "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "invoice.CreateDate",
                     DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                     DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (filter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetVendorSettleOrderPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetVendorSettleOrderPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "vendorsettle.SysNo", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                    DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CreateUserSysNo",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendorsettle.SettleTime",
                    DbType.DateTime, "@SettleTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendorsettle.SettleTime",
                     DbType.DateTime, "@SettleTimeTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendorSettle.StockSysNo",
                    DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, filter.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (filter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetPOPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "po.SysNo", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.IsConsign",
                   DbType.Int32, "@IsConsign", QueryConditionOperatorType.NotEqual, 1);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.SysNo",
                    DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.pmsysno",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.CreateTime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.CreateTime",
                     DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.PayStockSysNo",
                    DbType.Int32, "@PayStockSysNo", QueryConditionOperatorType.Equal, filter.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (filter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                }
                if (filter.NotInStock)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "pay.InstockAmt IS NULL");
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private DataSet GetCCPayItemList(PayItemQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);

            DataSet result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCCPayItemList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "payitem.SysNo desc"))
            {
                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    string[] orderIDArray = filter.OrderID.Trim(new char[] { '.' }).Split('.');
                    List<int> orderIDList = new List<int>();
                    for (var i = 0; i < orderIDArray.Length; i++)
                    {
                        orderIDList.Add(int.Parse(orderIDArray[i]));
                    }
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "cc.SysNo", DbType.Int32, orderIDList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cc.VendorSysNo",
                    DbType.Int32, "@VendorID", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cc.pmsysno",
                    DbType.Int32, "@UserID", QueryConditionOperatorType.Equal, filter.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayStyle",
                    DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, filter.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.ReferenceID",
                    DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.CreateTime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.CreateTime",
                     DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.EstimatePayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.EstimatePayTime",
                    DbType.DateTime, "@EstimatePayDateTo", QueryConditionOperatorType.LessThan, filter.EstimatePayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.PayDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.PayTime",
                    DbType.DateTime, "@PayDateTo", QueryConditionOperatorType.LessThan, filter.PayDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InvoiceEditDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceUpdateTime",
                    DbType.DateTime, "@InvoiceEditDateTo", QueryConditionOperatorType.LessThan, filter.InvoiceEditDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, filter.Note);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pay.InvoiceStatus",
                    DbType.Int32, "@InvoiceStatus", QueryConditionOperatorType.Equal, filter.InvoiceStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payitem.Status",
                    DbType.Int32, "@AbandonStatus", QueryConditionOperatorType.NotEqual, filter.IsFilterAbandonItem ? -1 : default(int?));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "payItem.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (filter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.PaySettleCompany",
                        DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, filter.PaySettleCompany.Value);
                }
                if (filter.NotInStock)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "pay.InstockAmt IS NULL");
                }

                dataCommand.AddOutParameter("@AllPayAmt", DbType.Double, 50);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataCommand(filter, dataCommand, out totalCount);
            }
            return result;
        }

        private EnumColumnList RegisterEnumColumn(PayItemQueryFilter filter)
        {
            //注册Enum类型
            EnumColumnList enumColumns = new EnumColumnList();
            enumColumns.Add("OrderType", typeof(PayableOrderType));
            enumColumns.Add("PayStyle", typeof(PayItemStyle));
            enumColumns.Add("Status", typeof(PayItemStatus));
            enumColumns.Add("InvoiceStatus", typeof(PayableInvoiceStatus));
            enumColumns.Add("InvoiceFactStatus", typeof(PayableInvoiceFactStatus));
            if (!filter.IsFilterPOETP)
            {
                enumColumns.Add("SapImportedStatus", typeof(SapImportedStatus));
            }
            return enumColumns;
        }

        private DataSet ExecuteDataCommand(PayItemQueryFilter filter, CustomDataCommand dataCommand, out int totalCount)
        {
            DataSet result = new DataSet();

            //注册Enum类型
            EnumColumnList enumColumns = RegisterEnumColumn(filter);
            //查询结果
            DataTable resultDT = dataCommand.ExecuteDataTable(enumColumns);
            resultDT.TableName = "DataResult";
            result.Tables.Add(resultDT);

            //统计信息
            DataTable statisticTable = result.Tables.Add("StatisticResult");
            statisticTable.Columns.Add("AllPayAmt", typeof(decimal));
            DataRow row = statisticTable.NewRow();
            row["AllPayAmt"] = Convert.ToDecimal(dataCommand.GetParameterValue("AllPayAmt") is DBNull ? 0M : dataCommand.GetParameterValue("AllPayAmt"));
            statisticTable.Rows.Add(row);

            //当OrderType=VendorSettleOrder时需要计算IsOldConsignSettle
            if (!resultDT.Columns.Contains("IsOldConsignSettle"))
            {
                var col = new DataColumn("IsOldConsignSettle", typeof(Boolean));
                col.DefaultValue = false;
                resultDT.Columns.Add(col);
            }

            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            return result;
        }

        /// <summary>
        /// 排序字段映射
        /// </summary>
        /// <param name="filter"></param>
        private void MapSortField(PayItemQueryFilter filter)
        {
            if (filter != null && filter.PagingInfo != null
                && !string.IsNullOrEmpty(filter.PagingInfo.SortBy) && filter.OrderType != null)
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0, filter.PagingInfo.SortBy.Length - index);
                var sortField = filter.PagingInfo.SortBy;

                switch (sort)
                {
                    case "PayItemSysNo":
                        filter.PagingInfo.SortBy = sortField.Replace("PayItemSysNo", "payitem.SysNo");
                        break;
                    case "VendorSysNo":
                        filter.PagingInfo.SortBy = sortField.Replace("VendorSysNo", "vendor.SysNo");
                        break;
                    case "VendorName":
                        filter.PagingInfo.SortBy = sortField.Replace("VendorName", "vendor.VendorName");
                        break;
                    case "PayAmt":
                        filter.PagingInfo.SortBy = sortField.Replace("PayAmt", "payitem.PayAmt");
                        break;
                    case "CurrencySysNo":
                        filter.PagingInfo.SortBy = sortField.Replace("CurrencySysNo", "pay.CurrencySysNo");
                        break;
                    case "PayStyle":
                        filter.PagingInfo.SortBy = sortField.Replace("PayStyle", "payitem.PayStyle");
                        break;
                    case "EstimatePayTime":
                        filter.PagingInfo.SortBy = sortField.Replace("EstimatePayTime", "payitem.EstimatePayTime");
                        break;
                    case "PayTime":
                        filter.PagingInfo.SortBy = sortField.Replace("PayTime", "payitem.PayTime");
                        break;
                    case "InvoiceStatus":
                        filter.PagingInfo.SortBy = sortField.Replace("InvoiceStatus", "pay.InvoiceStatus");
                        break;
                    case "Status":
                        filter.PagingInfo.SortBy = sortField.Replace("Status", "payitem.Status");
                        break;
                    case "UpdateInvoiceUserName":
                        filter.PagingInfo.SortBy = sortField.Replace("UpdateInvoiceUserName", "sysUser.DisplayName");
                        break;
                    case "SAPPostDate":
                        filter.PagingInfo.SortBy = sortField.Replace("SAPPostDate", "payitem.SAPPostDate");
                        break;
                    case "PaySettleCompany":
                        filter.PagingInfo.SortBy = sortField.Replace("PaySettleCompany", "vendor.PaySettleCompany");
                        break; 
                }
            }
        }

        /// <summary>
        /// 构造PagingInfo对象
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private PagingInfoEntity CreatePagingInfo(PayItemQueryFilter query)
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

        #region IPayItemQueryDA Members

        public DataTable SimpleQuery(int paySysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPayItemList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
              dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PaySysNo", DbType.Int32, "@PaySysNo",
                    QueryConditionOperatorType.Equal, paySysNo);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("PayStyle", typeof(PayItemStyle));
                enumColumns.Add("Status", typeof(PayItemStatus));
                enumColumns.Add("OrderType", typeof(PayableOrderType));

                return dataCommand.ExecuteDataTable(enumColumns);
            }
        }

        #endregion IPayItemQueryDA Members
    }
}