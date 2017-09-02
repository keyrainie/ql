using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IPayableDA))]
    public class PayableDA : IPayableDA
    {
        #region IPayableDA Members

        public PayableInfo Create(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertPayable");
            command.SetParameterValue(entity);

            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());
            return entity;
        }

        /// <summary>
        /// 更新付款单发票信息
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateInvoiceInfo(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableInvoive");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@InvoiceStatus", entity.InvoiceStatus);
            command.SetParameterValue("@InvoiceFactStatus", entity.InvoiceFactStatus);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValueAsCurrentUserSysNo("@UpdateInvoiceUserSysNo");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新应付款审核信息
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateAuditInfo(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableAuditInfo");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@AuditStatus", entity.AuditStatus);
            //command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");

            //付款金额为零，自动支付时审核人是系统用户.
            if (entity.AuditUserSysNo.HasValue)
            {
                command.SetParameterValue("@AuditUserSysNo", entity.AuditUserSysNo);
            }
            else
            {
                command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            }
            command.ExecuteNonQuery();
        }

        public PayableInfo LoadBySysNo(int sysNo)
        {
            var list = GetListByCriteria(new PayableInfo()
            {
                SysNo = sysNo
            });
            if (list != null && list.Count > 0)
            {
                var pay = list[0];

                var query = new PayItemInfo()
                {
                    PaySysNo = pay.SysNo
                };
                pay.PayItemList = ObjectFactory<IPayItemDA>.Instance.GetListByCriteria(query);

                return pay;
            }
            return null;
        }

        public List<PayableInfo> GetListByCriteria(PayableInfo query)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPayableList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderSysNo",
                  DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, query.OrderSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType",
                 DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PayStatus",
                 DbType.Int32, "@PayStatus", QueryConditionOperatorType.Equal, query.PayStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo",
                 DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "BatchNumber",
                 DbType.Int32, "@BatchNumber", QueryConditionOperatorType.Equal, query.BatchNumber);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var result = dataCommand.ExecuteEntityList<PayableInfo>();
                return result;
            }
        }

        /// <summary>
        /// 更新应付款状态和已付金额
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateStatusAndAlreadyPayAmt(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableStatusAndAlreadyPayAmt");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@AlreadyPayAmt", entity.AlreadyPayAmt);
            command.SetParameterValue("@PayStatus", entity.PayStatus);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新应付款状态和单据金额
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateStatusAndOrderAmt(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableStatusAndOrderAmt");
            command.SetParameterValue(entity);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新应付款状态
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateStatus(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@PayStatus", entity.PayStatus);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新预计付款时间
        /// </summary>
        /// <param name="payableInfo"></param>
        public void UpdateETP(PayableInfo payableInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableETP");
            command.SetParameterValue("@SysNo", payableInfo.SysNo);
            command.SetParameterValue("@ETP", payableInfo.EstimatedTimeOfPay);

            command.ExecuteNonQuery();
        }

        public void UpdatePayableInvoiceStatus(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableInvoiceStatus");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@InvoiceStatus", entity.InvoiceStatus);
            command.SetParameterValue("@InvoiceFactStatus", entity.InvoiceFactStatus);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@UpdateInvoiceUserSysNo", entity.UpdateInvoiceUserSysNo);
            command.ExecuteNonQuery();
        }

        public void UpdatePayableInvoiceStatusWithEtp(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableInvoiceStatusWithETP");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@InvoiceStatus", entity.InvoiceStatus);
            command.SetParameterValue("@InvoiceFactStatus", entity.InvoiceFactStatus);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@UpdateInvoiceUserSysNo", entity.UpdateInvoiceUserSysNo);
            if (entity.EstimatedTimeOfPay.HasValue)
            {
                command.SetParameterValue("@ETP", entity.EstimatedTimeOfPay);
            }
            else
            {
                command.SetParameterValue("@ETP", DBNull.Value);
            }
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据PaySysNo查询单据状态
        /// </summary>
        /// <param name="sysNo">PaySysNo</param>
        /// <returns></returns>
        public string QueryInvoiceStatusByPaySysNo(int sysNo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_GetInvoiceStatusByPaySysNo");
            cmd.SetParameterValue("@PaySysNo", sysNo);
            return cmd.ExecuteScalar<string>();
        }


        public void UpdatePayableEGPAndETP(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayableEGPAndETP");

            command.SetParameterValue("@SysNo", entity.SysNo);
            //command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@EGP", entity.EGP);
            command.SetParameterValue("@ETP", entity.ETP);
            command.ExecuteNonQuery();

            //return entity;
        }

        #endregion IPayableDA Members

        #region For PO Domain

        public bool IsAbandonGatherPayItem(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPayGatherItemStatusByOrderNo");
            int? status;
            command.SetParameterValue("@SysNo", sysNo);
            status = command.ExecuteScalar<Nullable<int>>();

            if (status.HasValue)
            {
                if (status.Value == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsAbandonPayItem(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPayItemStatusByOrderNoNew");
            int? status;

            command.SetParameterValue("@SysNo", sysNo);

            status = command.ExecuteScalar<Nullable<int>>();

            if (status.HasValue)
            {
                if (status.Value == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public decimal GetVendorPayBalanceByVendorSysNo(int vendorSysNo)
        {
            decimal result = 0m;
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorPayBalanceByVendorSysNo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            DataTable dt = command.ExecuteDataTable();
            if (null != dt && 0 < dt.Rows.Count)
            {
                result = dt.Rows[0]["TotalPayBalance"] == null || dt.Rows[0]["TotalPayBalance"].ToString() == "" ? 0m : Convert.ToDecimal(dt.Rows[0]["TotalPayBalance"].ToString());
            }
            return result;
        }

        public PayItemStatus? GetPOPrePayItemStatus(int poSysNo)
        {
            PayItemStatus status;
            var cmd = DataCommandManager.GetDataCommand("GetPOPrepay");

            cmd.SetParameterValue("@PoSysNO", poSysNo);

            DataTable dt = cmd.ExecuteDataTable();
            if (null != dt && dt.Rows.Count > 0)
            {
                if (!Enum.TryParse<PayItemStatus>(dt.Rows[0]["Status"].ToString(), out status))
                {
                    return null;
                }
                return status;
            }
            else
            {
                return null;
            }
        }

        public List<PayableInfo> GetUnpaidListByVendorSysNo(int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetUnpaidPayableByVendorSysNo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);

            return command.ExecuteEntityList<PayableInfo>();
        }

        public PayableInfo GetFirstPay(PayableOrderType orderType, int orderSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetFirstPay");
            command.SetParameterValue("@OrderType", orderType);
            command.SetParameterValue("@OrderSysNo", orderSysNo);

            return command.ExecuteEntity<PayableInfo>();
        }

        public void UpdateFirstPay(PayableInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateFirstPay");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@PayStatus", entity.PayStatus);
            command.SetParameterValue("@OrderAmt", entity.OrderAmt);
            command.SetParameterValue("@OrderStatus", entity.OrderStatus);
            command.SetParameterValue("@InStockAmt", entity.InStockAmt);
            command.SetParameterValue("@EIMSAmt", entity.EIMSAmt);
            command.SetParameterValue("@RawOrderAmt", entity.RawOrderAmt);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 取得未支付的应付款（包括PO、代销结算单、代收结算单）
        /// </summary>
        /// <returns></returns>
        public List<PayableInfo> GetUnPayOrPartlyPayList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetUnPayPayableList");
            return command.ExecuteEntityList<PayableInfo>();
        }

        #endregion For PO Domain
    }
}