using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IInvoiceDA
    {
        /// <summary>
        /// 创建一张发票
        /// </summary>
        /// <param name="invoice">发票信息，包含主信息和Transaction信息</param>
        /// <returns></returns>
        InvoiceInfo Create(InvoiceInfo entity);

        /// <summary>
        /// 加载发票
        /// </summary>
        /// <param name="invoiceSysNo"></param>
        /// <returns></returns>
        InvoiceInfo LoadBySysNo(int invoiceSysNo);

        /// <summary>
        /// 为RMA退款单编辑页面三费合计查询费用信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="stockID">仓库系统编号</param>
        /// <param name="totalAmt">三费合计总金额</param>
        /// <param name="premiumAmt">保价费</param>
        /// <param name="shippingCharge">运费</param>
        /// <param name="payPrice">手续费</param>
        void GetShipFee(int soSysNo, int stockSysNo, out decimal totalAmt, out decimal premiumAmt, out decimal shippingCharge, out decimal payPrice);

        /// <summary>
        /// 根据订单编号取得发票主信息列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>满足条件的发票主信息列表</returns>
        List<InvoiceMasterInfo> GetMasterInfoList(int soSysNo);


        void UpdateSOInvoice(int soSysNo, string invoiceNo,string warehouseNo,string companyCode);

        void InsertTransactionCheckBill(TransactionCheckBill entity);

        int SOOutStockInvoiceSync(int soSysNo, int stockSysNo, string invoiceNo, string companyCode);
    }
}