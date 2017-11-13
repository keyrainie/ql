using System;
using System.Collections.Generic;
using System.Data;
using ECommerce.Entity.Common;
using ECommerce.Entity.SO;
using ECommerce.Enums;
using ECommerce.Utility.DataAccess;
using ECommerce.Utility;
using ECommerce.Entity.Inventory;
using System.Linq;
using ECommerce.Entity.Invoice;

namespace ECommerce.DataAccess.SO
{
    public static class SODA
    {
        public static QueryResult<SOInfo> SOQuery(SOQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SOQuery");
            using (
                var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter,
                    string.IsNullOrEmpty(queryFilter.SortFields) ? "s.SysNo ASC" : queryFilter.SortFields))
            {
                if (queryFilter.SOSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format("(so.SysNo = {0} or sc.SoSplitMaster = {0})", queryFilter.SOSysNo));
                }

                if (queryFilter.OrderDateBegin.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.OrderDate",
                        DbType.DateTime, "@OrderDateBegin",
                        QueryConditionOperatorType.MoreThanOrEqual, queryFilter.OrderDateBegin);
                }

                if (queryFilter.OrderDateEnd.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.OrderDate",
                        DbType.DateTime, "@OrderDateEnd",
                        QueryConditionOperatorType.LessThan, queryFilter.OrderDateEnd.Value.Date.AddDays(1));
                }

                if (!string.IsNullOrWhiteSpace(queryFilter.CustomerID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cust.CustomerID",
                        DbType.String, "@CustomerID",
                        QueryConditionOperatorType.Equal, queryFilter.CustomerID);
                }

                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format(@"(so.SysNo in (SELECT si.sosysno
			                            from ipp3..so_item si(nolock)
			                            WHERE  si.ProductSysNo = {0}))", queryFilter.ProductSysNo));
                }

                if (!string.IsNullOrWhiteSpace(queryFilter.ReceivePhone))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        "(so.ReceivePhone = @ReceivePhone OR so.ReceiveCellPhone = @ReceivePhone)");
                    command.AddInputParameter("@ReceivePhone", DbType.String, queryFilter.ReceivePhone);
                }

                if (queryFilter.SOStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.Status",
                        DbType.Int32, "@SOStatus",
                        QueryConditionOperatorType.Equal, (int)queryFilter.SOStatus.Value);
                }

                if (queryFilter.PayTypeSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.PayTypeSysNo",
                                  DbType.Int32, "@PayTypeSysNo",
                                  QueryConditionOperatorType.Equal, queryFilter.PayTypeSysNo);
                }

                if (queryFilter.ShipTypeSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.ShipTypeSysNo",
                                  DbType.Int32, "@ShipTypeSysNo",
                                  QueryConditionOperatorType.Equal, queryFilter.ShipTypeSysNo);
                }

                if (queryFilter.ConsolidatedPaymentStatus.HasValue)
                {
                    //已支付未审核
                    if (queryFilter.ConsolidatedPaymentStatus == ConsolidatedPaymentStatus.PaymentWaitApprove)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                            "(netpay.Status = 0)");
                    //未支付
                    else if (queryFilter.ConsolidatedPaymentStatus == ConsolidatedPaymentStatus.PaymentNone)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                            "(netpay.SysNo IS NULL OR netpay.Status = -1)");
                    // 匹配SOIncome状态
                    else
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                            "fs.Status = " + (int)queryFilter.ConsolidatedPaymentStatus.Value);
                }

                if (queryFilter.MerchantSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sc.MerchantSysNo",
                        DbType.Int32, "@MerchantSysNo",
                        QueryConditionOperatorType.Equal, queryFilter.MerchantSysNo);
                }

                if (string.IsNullOrWhiteSpace(queryFilter.SortFields))
                {
                    queryFilter.SortFields = "so.[OrderDate] DESC";
                }


                command.CommandText = sqlBuilder.BuildQuerySql();
                List<SOInfo> resultList = command.ExecuteEntityList<SOInfo>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<SOInfo>
                {
                    PageInfo = new PageInfo
                    {
                        PageIndex = queryFilter.PageIndex,
                        PageSize = queryFilter.PageSize,
                        TotalCount = totalCount,
                        SortBy = queryFilter.SortFields
                    },
                    ResultList = resultList
                };
            }
        }

        public static SOInfo GetSOBySysNo(int soSysNo, bool isFromWriteServer = false)
        {
            #region execute data command
            DataTable soTable = null, itemTable = null;
            DataRow currentSORow = null;
            SOInfo currentSOInfo;

            var command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOBySysNo");
            if (isFromWriteServer)
                command.DatabaseAliasName = "Read";
            command.SetParameterValue("@SOSysNo", soSysNo);
            var dataSet = command.ExecuteDataSet();

            if (dataSet.Tables.Count >= 2)
            {
                soTable = dataSet.Tables[0];
                itemTable = dataSet.Tables[1];
            }

            if (soTable != null && soTable.Rows.Count > 0)
            {
                currentSORow = soTable.Rows[0];
            }

            if (currentSORow == null)
                return null;
            #endregion

            // Build SO info.
            currentSOInfo = DataMapper.GetEntity<SOInfo>(currentSORow);

            // Fill item list of SO
            currentSOInfo.SOItemList = DataMapper.GetEntityList<SOItemInfo, List<SOItemInfo>>(itemTable.Rows);

            // Fill area info of SO
            var areaQueryResult = CommonDA.QueryArea(new AreaInfoQueryFilter()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                DistrictSysNo = currentSOInfo.ReceiveAreaSysNo
            });
            if (areaQueryResult.ResultList != null && areaQueryResult.ResultList.Count > 0)
            {
                currentSOInfo.ReceiveArea = areaQueryResult.ResultList[0];
            }

            return currentSOInfo;
        }

        public static void CreatMoreSOSysNoAndTrackingNumber(string SoTrackingTableXml, string UserID, int SellerSysNo)
        {
            var command = DataCommandManager.GetDataCommand("SO_BatchInsertSOTrackingNumber");
            command.SetParameterValue("@SoTracking", SoTrackingTableXml);
            command.SetParameterValue("@CreateUserID", UserID);
            command.SetParameterValue("@MerchantSysNo", SellerSysNo);
            command.ExecuteNonQuery();
        }
        public static void UpdateSOStatus(int soSysNo, SOStatus status, DateTime? auditTime = null, DateTime? outTime = null, int? updateUserSysNo = null)
        {
            var command = DataCommandManager.GetDataCommand("UpdateSOStatus");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@UpdateUserSysNo", updateUserSysNo);
            command.SetParameterValue("@AuditTime", auditTime);
            command.SetParameterValue("@OutTime", outTime);
            command.SetParameterValue("@Status", status);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 回滚分仓库存：财务库存(+)、可用库存(+)、已订库存(-)
        /// </summary>
        /// <param name="soSysNo"></param>
        public static void RollbackStockInventoryForVoidSO(int soSysNo)
        {
            var command = DataCommandManager.GetDataCommand("RollbackStockInventoryForVoidSO");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 回滚总库存：财务库存(+)、可用库存(+)、已订库存(-)
        /// </summary>
        /// <param name="soSysNo"></param>
        public static void RollbackTotalInventoryForVoidSO(int soSysNo)
        {
            var command = DataCommandManager.GetDataCommand("RollbackTotalInventoryForVoidSO");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 取消使用延保
        /// </summary>
        /// <param name="soSysNo"></param>
        public static void CancelSOExtendWarranty(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_CancelSOExtendWarranty");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        public static void SOUpdate(SOUpdateInfo soUpdateInfo)
        {
            using (var tx = TransactionManager.Create())
            {
                SOUpdateMaster(soUpdateInfo);
                SOUpdateItems(soUpdateInfo);
                tx.Complete();
            }
        }

        public static void SOCheckShippingUpdateShippingFee(SOUpdateInfo soUpdateInfo)
        {
            var command = DataCommandManager.GetDataCommand("SOCheckShippingUpdateShippingFee");
            command.SetParameterValue("@ShippingFee", soUpdateInfo.ShipPrice);
            command.SetParameterValue("@SOSysNo", soUpdateInfo.SOSysNo);
            command.ExecuteNonQuery();
        }

        private static void SOUpdateItems(SOUpdateInfo soUpdateInfo)
        {
            foreach (var item in soUpdateInfo.Items)
            {
                var command = DataCommandManager.GetDataCommand("UpdateSOItem");
                command.SetParameterValue("@ItemSysNo", item.SysNo);
                command.SetParameterValue("@ItemPrice", item.Price);
                command.SetParameterValue("@ItemOriginalPrice", item.OriginalPrice);
                command.SetParameterValue("@ItemTariffAmt", item.TariffAmt);
                command.ExecuteNonQuery();
            }
        }

        private static void SOUpdateMaster(SOUpdateInfo soUpdateInfo)
        {
            var command = DataCommandManager.GetDataCommand("UpdateSOMaster");
            command.SetParameterValue("@SOSysNo", soUpdateInfo.SOSysNo);
            command.SetParameterValue("@ReceiveContact", soUpdateInfo.ReceiveContact);
            command.SetParameterValue("@ReceiveAddress", soUpdateInfo.ReceiveAddress);
            command.SetParameterValue("@ReceiveZip", soUpdateInfo.ReceiveZip);
            command.SetParameterValue("@ReceivePhone", soUpdateInfo.ReceivePhone);
            command.SetParameterValue("@ReceiveCellPhone", soUpdateInfo.ReceiveCellPhone);
            command.SetParameterValue("@ShipPrice", soUpdateInfo.ShipPrice);
            command.SetParameterValue("@TariffAmt", soUpdateInfo.TariffAmt);
            command.SetParameterValue("@SOAmt", soUpdateInfo.SOAmt);
            command.SetParameterValue("@CashPay", soUpdateInfo.CashPay);
            command.ExecuteNonQuery();
        }

        public static QueryResult AOQuery(AOQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("AOQuery");
            using (
                var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter,
                    string.IsNullOrEmpty(queryFilter.SortFields) ? "bank.SysNo ASC" : queryFilter.SortFields))
            {
                if (queryFilter.SOSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format("(so.SysNo = {0} or sc.SoSplitMaster = {0})", queryFilter.SOSysNo));
                }

                if (queryFilter.OrderDateBegin.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.OrderDate",
                        DbType.DateTime, "@OrderDateBegin",
                        QueryConditionOperatorType.MoreThanOrEqual, queryFilter.OrderDateBegin);
                }

                if (queryFilter.OrderDateEnd.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.OrderDate",
                        DbType.DateTime, "@OrderDateEnd",
                        QueryConditionOperatorType.LessThan, queryFilter.OrderDateEnd.Value.Date.AddDays(1));
                }

                if (!string.IsNullOrWhiteSpace(queryFilter.CustomerID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cust.CustomerID",
                        DbType.String, "@CustomerID",
                        QueryConditionOperatorType.Equal, queryFilter.CustomerID);
                }

                if (queryFilter.PayTypeSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.PayTypeSysNo",
                                  DbType.Int32, "@PayTypeSysNo",
                                  QueryConditionOperatorType.Equal, queryFilter.PayTypeSysNo);
                }

                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "bank.Status",
                        DbType.Int32, "@SOStatus",
                        QueryConditionOperatorType.Equal, (int)queryFilter.Status.Value);
                }


                if (queryFilter.RefundPayType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RefundPayType",
                        DbType.Int32, "@RefundPayType",
                        QueryConditionOperatorType.Equal, (int)queryFilter.RefundPayType.Value);
                }

                if (queryFilter.MerchantSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sc.MerchantSysNo",
                        DbType.Int32, "@MerchantSysNo",
                        QueryConditionOperatorType.Equal, queryFilter.MerchantSysNo);
                }

                if (string.IsNullOrWhiteSpace(queryFilter.SortFields))
                {
                    queryFilter.SortFields = "bank.SysNo ASC";
                }

                command.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("Status", typeof(RefundStatus));
                enumColumns.Add("RefundPayType", typeof(RefundPayType));
                DataTable result = command.ExecuteDataTable(enumColumns);
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult(result, queryFilter, totalCount);
            }
        }

        /// <summary>
        /// 订单出库
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool UpdateSOStatusToOutStock(SOStatusChangeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_StockOut");
            command.SetParameterValue<SOStatusChangeInfo>(info);
            return command.ExecuteNonQuery() > 0;
        }

        public static void WriteLog(SOInfo soInfo, BizLogType OperationType, string operationName)
        {
            SOLogInfo logInfo = new SOLogInfo
            {
                OptType = OperationType,
                OperationName = operationName,
                OptIP = string.Empty,//TODO:ServiceContext.Current.ClientIP,
                OptTime = DateTime.Now,
                SOSysNo = soInfo.SOSysNo,
                UserSysNo = 0,//ServiceContext.Current.UserSysNo,
                CompanyCode = soInfo.CompanyCode
            };
            SOLogNote notInfo = new SOLogNote
            {
                ActionName = logInfo.OperationName,
                SOSysNo = soInfo.SOSysNo,
                PayType = soInfo.Payment.PayTypeID,
                RecvSysNo = soInfo.ReceiveAreaSysNo,
                RecvAddress = soInfo.ReceiveAddress,
                CustomerSysNo = soInfo.CustomerSysNo,
                ShipType = soInfo.ShipType.ShipTypeSysNo,
                SOItems = (from item in soInfo.SOItemList
                           select new SOLogItemEntity
                           {
                               ProductSysNo = 1,
                               Qty = 1,
                               Price = 0
                           }).ToList()
            };
            logInfo.Note = SerializationUtility.XmlSerialize(notInfo);

            logInfo.OptIP = string.Empty;//TODO: ServiceContext.Current.ClientIP;
            logInfo.OptTime = DateTime.Now;
            logInfo.UserSysNo = 1;// ServiceContext.Current.UserSysNo;
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_SOLog");
            command.SetParameterValue<SOLogInfo>(logInfo, true, false);
            command.ExecuteNonQuery();
        }

        public static List<SOLogInfo> GetOrderLogBySOSysNo(int sosysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SO_GetOrderLogBySOSysNo");
            dataCommand.SetParameterValue("@SOSysNo", sosysno);
            return dataCommand.ExecuteEntityList<SOLogInfo>();
        }
        /// <summary>
        ///撮合交易
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public static List<SOLogMatchedTrading> GetOrderMatchedTradingLogBySOSysNo(int sosysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SO_GetOrderMatchedTradingLogBySOSysNo");
            dataCommand.SetParameterValue("@SOSysNo", sosysno);
            return dataCommand.ExecuteEntityList<SOLogMatchedTrading>();
        }

        /// <summary>
        /// 获取某个订单的推荐商品信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public static List<CommendatoryProductsInfo> GetCommendatoryProducts(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_CommendatoryProducts");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<CommendatoryProductsInfo>();
        }

        public static void UpdateSOIsCombine(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_UpdateSOIsCombine");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        public static void InsertSOTrackingNumber(SOTrackingInfo trackingInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_InsertSOTrackingNumber");
            command.SetParameterValue(trackingInfo);
            command.ExecuteNonQuery();
        }

        public static void CreateInvoiceMaster(InvoiceMasterInfo invoiceMaster, out int invoiceSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SO_InsertInvoiceMaster");
            dataCommand.SetParameterValue<InvoiceMasterInfo>(invoiceMaster);
            invoiceSysNo = Convert.ToInt32(dataCommand.ExecuteScalar());
        }

        public static void CreateInvoiceTransactions(List<InvoiceTransactionInfo> invoiceTransactions, int invoiceSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SO_InsertInvoiceTransaction");
            if (invoiceTransactions != null && invoiceTransactions.Count > 0)
            {
                invoiceTransactions.ForEach(entity =>
                {
                    entity.MasterSysNo = invoiceSysNo;
                    dataCommand.SetParameterValue<InvoiceTransactionInfo>(entity);
                    dataCommand.ExecuteNonQuery();
                });
            }
        }

        /// <summary>
        /// 根据订单编号取得订单拆分的价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public static List<SOPriceMasterInfo> GetSOPriceBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Get_SOPriceBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<SOPriceMasterInfo>();
        }

        public static List<SOPriceItemInfo> GetSOItemPriceBySOSysNo(int soSysNo, int? masterSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Get_SOItemPriceBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@MasterSysNo", masterSysNo);
            return command.ExecuteEntityList<SOPriceItemInfo>();
        }

        public static void UpdateSOPayStatus(int soSysNo, int toStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOPayStatus");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@ToStatus", toStatus);
            command.ExecuteNonQuery();
        }
    }
}