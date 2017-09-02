using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    /// <summary>
    /// 发票操作SQL持久化层
    /// </summary>
    [VersionExport(typeof(IInvoiceDA))]
    public class InvoiceDA : IInvoiceDA
    {
        #region IInvoiceDA Members

        /// <summary>
        /// 创建发票
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public InvoiceInfo Create(InvoiceInfo entity)
        {
            var invoiceMaster = entity.MasterInfo;
            var invoiceTransaction = entity.InvoiceTransactionInfoList;
            var invoiceSysNo = 0;

            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

            using (TransactionScope trans = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                InnerCreateInvoiceMaster(invoiceMaster, out invoiceSysNo);
                InnerCreateInvoiceTransactions(invoiceTransaction, invoiceSysNo);

                trans.Complete();
            }

            return LoadBySysNo(invoiceSysNo);
        }

        #region Inner Private Create Methods

        private void InnerCreateInvoiceMaster(InvoiceMasterInfo invoiceMaster, out int invoiceSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InsertInvoiceMaster");
            dataCommand.SetParameterValue<InvoiceMasterInfo>(invoiceMaster);
            invoiceSysNo = Convert.ToInt32(dataCommand.ExecuteScalar());
        }

        private void InnerCreateInvoiceTransactions(List<InvoiceTransactionInfo> invoiceTransactions, int invoiceSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InsertInvoiceTransaction");
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

        #endregion Inner Private Create Methods

        /// <summary>
        /// 加载发票信息
        /// </summary>
        /// <param name="invoiceSysNo"></param>
        /// <returns></returns>
        public InvoiceInfo LoadBySysNo(int invoiceSysNo)
        {
            InvoiceMasterInfo masterInfo = null;
            List<InvoiceTransactionInfo> transactionList = null;

            //加载Master信息
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetInvoiceMasterBySysNo");
            dataCommand.SetParameterValue("@InvoiceNumber", invoiceSysNo);
            masterInfo = dataCommand.ExecuteEntity<InvoiceMasterInfo>();

            //加载Transaction列表
            dataCommand = DataCommandManager.GetDataCommand("GetInvoiceTransactionListByMasterSysNo");
            dataCommand.SetParameterValue("@MasterInvoiceNumber", masterInfo.SysNo);
            transactionList = dataCommand.ExecuteEntityList<InvoiceTransactionInfo>();

            InvoiceInfo invoice = new InvoiceInfo()
            {
                SysNo = masterInfo.SysNo,
                MasterInfo = masterInfo,
                InvoiceTransactionInfoList = transactionList,
                CompanyCode = masterInfo.CompanyCode
            };

            return invoice;
        }

        /// <summary>
        /// 为RMA退款单编辑三费合计查询费用信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="stockID">仓库系统编号</param>
        /// <param name="totalAmt">三费合计总金额</param>
        /// <param name="premiumAmt">保价费</param>
        /// <param name="shippingCharge">运费</param>
        /// <param name="payPrice">手续费</param>
        public void GetShipFee(int soSysNo, int stockSysNo, out decimal totalAmt, out decimal premiumAmt, out decimal shippingCharge, out decimal payPrice)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetShipFee");
            dataCommand.SetParameterValue("@SONumber", soSysNo);
            dataCommand.SetParameterValue("@StockSysNo", stockSysNo);

            dataCommand.AddOutParameter("@TotalAmount", DbType.String, 50);
            dataCommand.AddOutParameter("@PremiumAmount", DbType.String, 50);
            dataCommand.AddOutParameter("@ShippingCharge", DbType.String, 50);
            dataCommand.AddOutParameter("@PayPrice", DbType.String, 50);

            dataCommand.ExecuteNonQuery();

            totalAmt = Convert.ToDecimal(dataCommand.GetParameterValue("@TotalAmount") is DBNull ? 0M : dataCommand.GetParameterValue("@TotalAmount").ToDecimal());
            premiumAmt = Convert.ToDecimal(dataCommand.GetParameterValue("@PremiumAmount") is DBNull ? 0M : dataCommand.GetParameterValue("@PremiumAmount").ToDecimal());
            shippingCharge = Convert.ToDecimal(dataCommand.GetParameterValue("@ShippingCharge") is DBNull ? 0M : dataCommand.GetParameterValue("@ShippingCharge").ToDecimal());
            payPrice = Convert.ToDecimal(dataCommand.GetParameterValue("@PayPrice") is DBNull ? 0M : dataCommand.GetParameterValue("@PayPrice").ToDecimal());
        }

        /// <summary>
        /// 根据订单编号和分仓编号取得发票主信息列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="stockSysNo">分仓系统编号</param>
        /// <returns>满足条件的发票主信息列表</returns>
        public List<InvoiceMasterInfo> GetMasterInfoList(int soSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetInvoiceMasterListBySOSysNo");
            dataCommand.SetParameterValue("@SOSysNo", soSysNo);

            return dataCommand.ExecuteEntityList<InvoiceMasterInfo>();
        }

        public void UpdateSOInvoice(int soSysNo, string invoiceNo, string warehouseNumber, string companyCode)
        {

            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOInvoice");
            command.SetParameterValue("@SONumber", soSysNo);
            command.SetParameterValue("@InvoiceNo", invoiceNo);
            command.SetParameterValue("@WarehouseNumber", warehouseNumber);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.ExecuteNonQuery();
        }

        public void InsertTransactionCheckBill(TransactionCheckBill entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Invoice.InsertTransactionCheckBill");
            dataCommand.SetParameterValue<TransactionCheckBill>(entity);
            dataCommand.ExecuteNonQuery();
        }

        #endregion IInvoiceDA Members

        public int SOOutStockInvoiceSync(int soSysNo, int stockSysNo, string invoiceNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SOOutStockInvoiceSync");
            cmd.SetParameterValue("@SONumber", soSysNo);
            cmd.SetParameterValue("@WarehouseNumber", stockSysNo.ToString());
            cmd.SetParameterValue("@InvoiceNo", invoiceNo);
            cmd.SetParameterValue("@CreateDate", DateTime.Now);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteNonQuery();
        }
    }
}
