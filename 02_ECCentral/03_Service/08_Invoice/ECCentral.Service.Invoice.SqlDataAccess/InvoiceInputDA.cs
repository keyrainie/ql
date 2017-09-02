using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice;
using System.Data;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IInvoiceInputDA))]
    public class InvoiceInputDA : IInvoiceInputDA
    {

        #region Load
        /// <summary>
        /// 加载供应商未录入的PO单据
        /// </summary>
        /// <param name="request">查询条件</param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> LoadNotInputPOItems(APInvoiceItemInputEntity request)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("LoadNotInputPOItems");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "A.OrderSysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND
                 , "A.PoBaselineDate"
                 , DbType.DateTime
                 , "@PODateFrom"
                 , QueryConditionOperatorType.MoreThanOrEqual
                 , request.PODateFrom);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
            }
            dataCommand.AddInputParameter("@VendorSysNo", DbType.Int32, request.VendorSysNo);
            dataCommand.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, request.CompanyCode);
            return dataCommand.ExecuteEntityList<APInvoicePOItemInfo>();
        }

        /// <summary>
        /// 根据APInvoiceMaster系统编号获取POItems列表
        /// </summary>
        /// <param name="docNo"></param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> GetPOItemsByDocNo(int docNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetPOItemsByDocNo");
            dataCommand.SetParameterValue("@DocNo", docNo);
            var result = dataCommand.ExecuteEntityList<APInvoicePOItemInfo>();
            return result;
        }
        /// <summary>
        /// 根据APInvoiceMaster系统编号获取InvoiceItems列表
        /// </summary>
        /// <param name="docNo"></param>
        /// <returns></returns>
        public virtual List<APInvoiceInvoiceItemInfo> GetInvoiceItemsByDocNo(int docNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetInvoiceItemsByDocNo");
            dataCommand.SetParameterValue("@DocNo", docNo);
            var result = dataCommand.ExecuteEntityList<APInvoiceInvoiceItemInfo>();
            return result;
        }
        /// <summary>
        /// 根据APInvoiceMaster系统编号获取APInvoice主信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo GetAPInvoiceMasterBySysNo(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetAPInvoiceMasterBySysNo");
            dataCommand.SetParameterValue("@DocNo", sysNo);
            var result = dataCommand.ExecuteEntity<APInvoiceInfo>();
            return result;
        }

        #endregion

        #region Input
        /// <summary>
        /// 获取无效的发票号码
        /// </summary>
        /// <param name="invoiceList"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<string> GetInvalidInvoiceNo(List<string> invoiceList, APInvoiceItemInputEntity entity)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvalidInvoiceNo");
            List<string> result = new List<string>();

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "API.InvoiceNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "APM.VendorSysNo"
                    , DbType.Int32
                    , "@VendorSysNo"
                    , QueryConditionOperatorType.Equal
                    , entity.VendorSysNo);

                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "API.[Status]"
                    , DbType.String
                    , "@ItemStatus"
                    , QueryConditionOperatorType.Equal
                    , APInvoiceItemStatus.Active);

                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.OR);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "API.[Status]"
                    , DbType.String
                    , "@ItemNStatus"
                    , QueryConditionOperatorType.Equal
                    , APInvoiceItemStatus.Deactive);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "APM.[Status]"
                    , DbType.Int32
                    , "@MasterStatus"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , APInvoiceMasterStatus.Origin);

                sqlBuilder.ConditionConstructor.EndGroupCondition();

                sqlBuilder.ConditionConstructor.EndGroupCondition();

                sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND
                    , "API.InvoiceNo"
                    , DbType.String
                    , invoiceList);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "API.CompanyCode",
                    DbType.AnsiStringFixedLength,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    entity.CompanyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var dataSet = dataCommand.ExecuteDataSet();

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dataSet.Tables[0].Rows)
                    {
                        result.Add(dr["InvoiceNo"].ToString());
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 根据输入的PO单据号获取应付款中的PO单列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<POItemCheckEntity> GetPOCheckList(List<int> condition, APInvoiceItemInputEntity entity)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("POInputCheck");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "POM.VendorSysNo"))
            {

                sqlBuilder.ConditionConstructor.AddInCondition(
                    QueryConditionRelationType.AND,
                    "pay.OrderSysNo",
                    DbType.Int32,
                    condition);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "pay.OrderType",
                    DbType.Int32,
                    "@OrderType",
                    QueryConditionOperatorType.Equal,
                    entity.OrderType);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "pay.CompanyCode",
                    DbType.AnsiStringFixedLength,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    entity.CompanyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                return dataCommand.ExecuteEntityList<POItemCheckEntity>();
            }

        }
        /// <summary>
        /// 根据输入的PONO获取对应的POItems信息
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="payableOrderType"></param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> GetPOInputItemsHand(List<int> condition, PayableOrderType payableOrderType)
        {
            List<int> orderStatus = new List<int>();
            List<int> orderType = new List<int>();
            string customCondition = string.Empty;

            if (payableOrderType == PayableOrderType.PO)
            {
                orderStatus.Add(4);
                orderStatus.Add(6);
                orderStatus.Add(7);
                orderStatus.Add(8);
                orderStatus.Add(9);
                orderType.Add(0);
                orderType.Add(5);
                customCondition = @" NOT Exists
				  (
						SELECT PoNo
						FROM   OverseaInvoiceReceiptManagement.dbo.APInvoice_PO_Item ai WITH(NOLOCK)
						WHERE  Status = 'A'
						and pay.BatchNumber=ai.BatchNumber AND pay.OrderSysNo=ai.PoNo AND pay.OrderType=ai.OrderType

						UNION ALL
						
						SELECT ai.PoNo
						FROM   OverseaInvoiceReceiptManagement.dbo.APInvoice_PO_Item ai WITH(NOLOCK)     
						INNER JOIN   OverseaInvoiceReceiptManagement.dbo.APInvoice_Master am WITH(NOLOCK)
						ON   am.DocNo = ai.DocNo and pay.BatchNumber=ai.BatchNumber AND pay.OrderSysNo=ai.PoNo AND pay.OrderType=ai.OrderType
						WHERE  am.Status >= 0						
				  )
                ";
            }
            else if (payableOrderType == PayableOrderType.RMAPOR)
            {
                orderStatus.Add(3);
                orderStatus.Add(5);
                orderType.Add(9);
                customCondition = string.Empty;
            }
            //else if (payableOrderType == PayableOrderType.SubInvoice)
            //{
            //    orderStatus.Add(3);
            //    orderStatus.Add(2);
            //    orderStatus.Add(1);
            //    orderType.Add(7);
            //    customCondition = string.Empty;
            //}
            //else
            {
                orderStatus.Add(3);
                orderType.Add(1);
                customCondition = string.Empty;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOInputItemsHand");
            List<APInvoicePOItemInfo> result = new List<APInvoicePOItemInfo>();
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                 dataCommand.CommandText, dataCommand, null, "POM.VendorSysNo"))
            {
                sqlBuilder.ConditionConstructor.AddInCondition(
                    QueryConditionRelationType.AND,
                    "pay.OrderStatus",
                    DbType.Int32,
                    orderStatus);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "pay.InvoiceStatus",
                    DbType.Int32,
                    "@InvoiceStatus",
                    QueryConditionOperatorType.Equal,
                    0);

                sqlBuilder.ConditionConstructor.AddInCondition(
                    QueryConditionRelationType.AND,
                    "pay.ordertype",
                    DbType.Int32,
                    orderType);

                sqlBuilder.ConditionConstructor.AddInCondition(
                    QueryConditionRelationType.AND,
                    "pay.OrderSysNo",
                    DbType.Int32,
                    condition);

                //sqlBuilder.ConditionConstructor.AddCondition(
                //    QueryConditionRelationType.AND,
                //    "pay.CompanyCode",
                //    DbType.AnsiStringFixedLength,
                //    "@CompanyCode",
                //    QueryConditionOperatorType.Equal,
                //    CPContext.Current.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "pay.PayStatus",
                    DbType.Int32,
                    "@PayStatus",
                    QueryConditionOperatorType.NotEqual,
                    -1);
                if (customCondition != string.Empty)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, customCondition);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                result = dataCommand.ExecuteEntityList<APInvoicePOItemInfo>();
            }
            return result;
        }

        #endregion

        #region Create

        /// <summary>
        /// 新建APInvoice Master信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int InsertAPInvoiceMaster(APInvoiceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertMaster");
            //command.SetParameterValue("@DocDate", entity.DocDate);
            command.SetParameterValue("@WareHouseNo", entity.StockSysNo);
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            command.SetParameterValue("@VendorName", entity.VendorName);
            command.SetParameterValue("@VendorTaxRate", entity.VendorTaxRate);
            command.SetParameterValue("@Memo", entity.Memo);
            command.SetParameterValue("@DiffMemo", entity.DiffMemo);
            command.SetParameterValue("@PoNetAmt", entity.PoNetAmt);
            command.SetParameterValue("@PoNetTaxAmt", entity.PoNetTaxAmt);
            command.SetParameterValue("@InvoiceAmt", entity.InvoiceAmt ?? 0);
            command.SetParameterValue("@InvoiceTaxAmt", entity.InvoiceTaxAmt ?? 0);
            command.SetParameterValue("@DiffTaxAmt", entity.DiffTaxAmt);
            command.SetParameterValue("@DiffTaxTreatmentType", entity.DiffTaxTreatmentType);
            command.SetParameterValueAsCurrentUserAcct("@InUser");
            //command.SetParameterValue("@InDate", entity.InDate);
            command.SetParameterValue("@ConfirmUser", entity.ConfirmUser);
            command.SetParameterValue("@ConfirmDate", entity.ConfirmDate);
            command.SetParameterValue("@Status", entity.Status);
            //command.SetParameterValue("LanguageCode", entity.LanguageCode);
            //command.SetParameterValue("CurrencyCode", entity.CurrencyCode);
            command.SetParameterValue("CompanyCode", entity.CompanyCode);
            //command.SetParameterValue("StoreCompanyCode", entity.StoreCompanyCode);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// 新建POitems
        /// </summary>
        /// <param name="entitylist"></param>
        /// <param name="docNo"></param>
        public virtual void InsertPOItem(List<APInvoicePOItemInfo> entitylist, int docNo)
        {
            if (entitylist != null && entitylist.Count > 0)
            {
                StringBuilder sqlBuilder = new StringBuilder();
                string sqlHeader = @"INSERT INTO [OverseaInvoiceReceiptManagement].[dbo].[APInvoice_PO_Item]
				                   ([DocNo]
				                   ,[PoNo]
				                   ,[PoWarehouseNo]
				                   ,[PoCurrency]
				                   ,[PoAmt]
				                   ,[EIMSNo]
				                   ,[EIMSAmt]
				                   ,[EIMSNetAmt]
				                   ,[EIMSNetTaxAmt]
				                   ,[PoNetAmt]
				                   ,[PaymentAmt]
				                   ,[PoBaselineDate]
				                   ,[PoPaymentTerm]
				                   ,[Status]
                                   ,[InUser]
                                   ,[InDate]
                                   ,[OrderType]
                                   ,[BatchNumber]
                                   ,[PayableTaxAmt])";

                sqlBuilder.Append(sqlHeader);
                int countFlag = 0;
                foreach (APInvoicePOItemInfo entity in entitylist)
                {
                    sqlBuilder.Append("\r\nSELECT ");
                    sqlBuilder.Append(docNo.ToString());
                    AppendNumeric(sqlBuilder, entity.PoNo);
                    AppendNumeric(sqlBuilder, entity.PoStockSysNo ?? 0);
                    AppendNumeric(sqlBuilder, 1);
                    AppendNumeric(sqlBuilder, entity.PoAmt ?? 0);
                    AppendText(sqlBuilder, entity.EIMSNo);
                    AppendText(sqlBuilder, entity.EIMSAmt);
                    if (entity.EIMSNetAmt == null)
                    {
                        entity.EIMSNetAmt = 0;
                    }
                    AppendText(sqlBuilder, entity.EIMSNetAmt);
                    if (entity.EIMSNetTaxAmt == null)
                    {
                        entity.EIMSNetTaxAmt = 0;
                    }
                    AppendText(sqlBuilder, entity.EIMSNetTaxAmt);
                    AppendNumeric(sqlBuilder, entity.PoNetAmt);
                    AppendText(sqlBuilder, entity.PaymentAmt ?? 0);
                    AppendText(sqlBuilder, entity.PoBaselineDate);
                    AppendNumeric(sqlBuilder, entity.PoPaymentTerm);
                    AppendText(sqlBuilder, "D");
                    AppendText(sqlBuilder, "");
                    AppendText(sqlBuilder, DateTime.Now);

                    AppendNumeric(sqlBuilder, (int)entity.OrderType);
                    AppendText(sqlBuilder, entity.BatchNumber);
                    AppendNumeric(sqlBuilder, entity.PayableTaxAmt);
                    if (++countFlag != entitylist.Count)
                    {
                        sqlBuilder.Append(" UNION ALL");
                    }
                }

                CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("InsertAPPOItem");
                command.CommandType = CommandType.Text;
                command.CommandText = sqlBuilder.ToString();
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 新建InvoiceItems
        /// </summary>
        /// <param name="entitylist"></param>
        /// <param name="docNo"></param>
        public virtual void InsertInvoItem(List<APInvoiceInvoiceItemInfo> entitylist, int docNo)
        {
            if (entitylist != null && entitylist.Count > 0)
            {
                StringBuilder sqlBuilder = new StringBuilder();
                string sqlHeader = @"INSERT INTO [OverseaInvoiceReceiptManagement].[dbo].[APInvoice_Invo_Item]
					               ([DocNo]
					               ,[InvoiceNo]
					               ,[InvoiceDate]
					               ,[InvoiceCurrency]
					               ,[InvoiceAmt]
					               ,[InvoiceNetAmt]
					               ,[InvoiceTaxAmt]
					               ,[Status]
                                   ,[InUser]
                                   ,[InDate]
					              )";

                sqlBuilder.Append(sqlHeader);
                int countFlag = 0;
                foreach (APInvoiceInvoiceItemInfo entity in entitylist)
                {
                    sqlBuilder.Append("\r\nSELECT ");
                    sqlBuilder.Append(docNo.ToString());
                    AppendText(sqlBuilder, entity.InvoiceNo);
                    AppendText(sqlBuilder, entity.InvoiceDate);
                    AppendNumeric(sqlBuilder, entity.InvoiceCurrency);
                    AppendNumeric(sqlBuilder, entity.InvoiceAmt);
                    AppendNumeric(sqlBuilder, entity.InvoiceNetAmt);
                    AppendNumeric(sqlBuilder, entity.InvoiceTaxAmt);
                    AppendText(sqlBuilder, "D");
                    AppendText(sqlBuilder, "");
                    AppendText(sqlBuilder, DateTime.Now);

                    if (++countFlag != entitylist.Count)
                    {
                        sqlBuilder.Append(" UNION ALL");
                    }
                }

                CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("InsertAPInvoItem");
                command.CommandType = CommandType.Text;
                command.CommandText = sqlBuilder.ToString();
                command.ExecuteNonQuery();

            }
        }

        private static void AppendText(StringBuilder builder, object text)
        {
            if (text != null)
            {
                builder.Append("\r\n\t ,'");
                builder.Append(text.ToString());
                builder.Append("'");
            }
            else
            {
                builder.Append("\r\n\t, NULL");
            }
        }

        private static void AppendNumeric(StringBuilder builder, object numeric)
        {
            if (numeric != null)
            {
                builder.Append("\r\n\t, ");
                builder.Append(numeric.ToString());
            }
            else
            {
                builder.Append("\r\n\t, NULL");
            }
        }

        #endregion

        #region 更新APInvoice全部信息
        /// <summary>
        /// 更新POItems
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="docNo"></param>
        public virtual void UpdatePOItem(APInvoicePOItemInfo entity, int docNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateAPInvoicePOItem");
            command.SetParameterValue("@DocNo", docNo);
            command.SetParameterValue("@PoNo", entity.PoNo);
            command.SetParameterValue("@PoWarehouseNo", entity.PoStockSysNo ?? 0);
            command.SetParameterValue("@PoCurrency", 1);
            command.SetParameterValue("@PoAmt", entity.PoAmt ?? 0);
            command.SetParameterValue("@EIMSNo", entity.EIMSNo);
            if (entity.EIMSAmt == null)
            {
                entity.EIMSAmt = 0;
            }
            command.SetParameterValue("@EIMSAmt", entity.EIMSAmt);
            if (entity.EIMSNetAmt == null)
            {
                entity.EIMSNetAmt = 0;
            }
            command.SetParameterValue("@EIMSNetAmt", entity.EIMSNetAmt);
            command.SetParameterValue("@EIMSNetTaxAmt", entity.EIMSNetTaxAmt);
            command.SetParameterValue("@PoNetAmt", entity.PoNetAmt);
            command.SetParameterValue("@PaymentAmt", entity.PaymentAmt ?? 0);
            command.SetParameterValue("@PoBaselineDate", entity.PoBaselineDate);
            command.SetParameterValue("@PoPaymentTerm", entity.PoPaymentTerm);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");
            command.SetParameterValue("@OrderType", (int)entity.OrderType);
            command.SetParameterValue("@BatchNumber", entity.BatchNumber);
            command.SetParameterValue("@PayableTaxAmt", entity.PayableTaxAmt);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除PoItems
        /// </summary>
        /// <param name="poNoList"></param>
        public virtual void DeletePOItems(List<POItemKeyEntity> poNoList)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("DeleteAPPOItem");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText, command, null, "PoNo"))
            {
                for (int i = 0; i < poNoList.Count; i++)
                {
                    sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.OR);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "PoNo",
                        DbType.Int32,
                        "@PoNo" + i.ToString(),
                        QueryConditionOperatorType.Equal,
                        poNoList[i].PONo);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "OrderType",
                        DbType.Int32,
                        "@OrderType" + i.ToString(),
                        QueryConditionOperatorType.Equal,
                        (int)poNoList[i].OrderType);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "BatchNumber",
                        DbType.Int32,
                        "@BatchNumber" + i.ToString(),
                        QueryConditionOperatorType.Equal,
                        poNoList[i].BatchNumber);

                    sqlBuilder.ConditionConstructor.EndGroupCondition();
                }

                command.CommandText = sqlBuilder.BuildQuerySql();

                command.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// 更新InvoiceItem
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="docNo"></param>
        public virtual void UpdateInvoItem(APInvoiceInvoiceItemInfo entity, int docNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateAPInvoItem");
            command.SetParameterValue("@DocNo", docNo);
            command.SetParameterValue("@InoviceNo", entity.InvoiceNo);
            command.SetParameterValue("@InvoiceDate", entity.InvoiceDate);
            command.SetParameterValue("@InvoiceCurrency", entity.InvoiceCurrency);
            command.SetParameterValue("@InvoiceAmt", entity.InvoiceAmt);
            command.SetParameterValue("@InvoiceNetAmt", entity.InvoiceNetAmt);
            command.SetParameterValue("@InvoiceTaxAmt", entity.InvoiceTaxAmt);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");

            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 删除InvoiceItems
        /// </summary>
        /// <param name="invoNoList"></param>
        public virtual void DeleteInvoItems(List<string> invoNoList)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("DeleteAPInvoItem");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText, command, null, "InvoiceNo"))
            {
                sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND
                    , "InvoiceNo"
                    , DbType.String
                    , invoNoList);

                command.CommandText = sqlBuilder.BuildQuerySql();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新APInvoice Master信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateAPInvoiceMaster(APInvoiceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateAPInvoiceMaster");
            command.SetParameterValue("@WareHouseNo", entity.StockSysNo);
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            command.SetParameterValue("@VendorName", entity.VendorName);
            command.SetParameterValue("@VendorTaxRate", entity.VendorTaxRate);
            command.SetParameterValue("@Memo", entity.Memo);
            if (entity.DiffMemo == null)
            {
                entity.DiffMemo = "";
            }
            command.SetParameterValue("@DiffMemo", entity.DiffMemo);
            command.SetParameterValue("@PoNetAmt", entity.PoNetAmt);
            command.SetParameterValue("@PoNetTaxAmt", entity.PoNetTaxAmt);
            command.SetParameterValue("@InvoiceAmt", entity.InvoiceAmt);
            command.SetParameterValue("@InvoiceTaxAmt", entity.InvoiceTaxAmt);
            command.SetParameterValue("@DiffTaxAmt", entity.DiffTaxAmt);
            command.SetParameterValue("@DiffTaxTreatmentType", entity.DiffTaxTreatmentType);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");
            command.SetParameterValue("@DocNo", entity.SysNo);

            command.ExecuteNonQuery();
        }

        #endregion

        #region UpdateStatus
        /// <summary>
        /// 更新APInvoice状态
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateAPInvoiceStatus(APInvoiceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateAPInvoiceStatus");
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@DocNo", entity.SysNo);
            command.SetParameterValueAsCurrentUserSysNo("@EditUser");
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新APInvoice状态及处理人
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateAPInvoiceStatusWithConfirmUser(APInvoiceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateAPInvoiceStatusWithConfirmUser");
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@DocNo", entity.SysNo);
            command.SetParameterValueAsCurrentUserSysNo("@EditUser");
            command.SetParameterValueAsCurrentUserAcct("@ConfirmUser");
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新POitem状态
        /// </summary>
        /// <param name="docNo"></param>
        public virtual void UpdatePOItemStatus(int docNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOItemStatus");
            command.SetParameterValue("@DocNo", docNo);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");

            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新InvoiceItem状态
        /// </summary>
        /// <param name="docNo"></param>
        public virtual void UpdateInvoItemStatus(int docNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInvoItemStatus");
            command.SetParameterValue("@DocNo", docNo);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");

            command.ExecuteNonQuery();
        }

        #endregion

        /// <summary>
        /// 检查POItem状态是否为有效
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool CheckPOItemAudit(APInvoicePOItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckPOItemAudit");
            command.SetParameterValue("@OrderType", entity.OrderType);
            command.SetParameterValue("@PONo", entity.PoNo);
            command.SetParameterValue("@BatchNumber", entity.BatchNumber);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        /// <summary>
        /// 获取FinancePaySysNo
        /// </summary>
        /// <param name="poNo"></param>
        /// <param name="orderType"></param>
        /// <param name="batchNumber"></param>
        /// <returns></returns>
        public virtual int GetFinancePaySysNo(int poNo, PayableOrderType orderType, int? batchNumber)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetFinancePaySysNo");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText, command, null, "Sysno"
                ))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "OrderSysNo",
                    DbType.Int32,
                    "@PoNo",
                    QueryConditionOperatorType.Equal, poNo);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "OrderType",
                    DbType.Int32,
                    "@OrderType",
                    QueryConditionOperatorType.Equal,
                    orderType);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "BatchNumber",
                    DbType.Int32,
                    "@BatchNumber",
                    QueryConditionOperatorType.Equal,
                    batchNumber);

                command.CommandText = sqlBuilder.BuildQuerySql();
            }

            return Convert.ToInt32(command.ExecuteScalar());
        }

    }
}
