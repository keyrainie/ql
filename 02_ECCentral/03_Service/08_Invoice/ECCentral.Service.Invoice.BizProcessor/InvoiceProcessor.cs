using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(InvoiceProcessor))]
    public class InvoiceProcessor
    {
        /// <summary>
        /// 根据订单编号取得发票主信息列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>满足条件的发票主信息列表</returns>
        public virtual List<InvoiceMasterInfo> GetMasterInfoList(int soSysNo)
        {
            return ObjectFactory<IInvoiceDA>.Instance.GetMasterInfoList(soSysNo);
        }

        /// <summary>
        /// 为RMA退款单编辑(三费合计)查询费用信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="stockSysNo">仓库系统编号</param>
        /// <param name="totalAmt">三费合计总金额</param>
        /// <param name="premiumAmt">保价费</param>
        /// <param name="shippingCharge">运费</param>
        /// <param name="payPrice">手续费</param>
        public virtual void GetShipFee(int soSysNo, string stockSysNo, out decimal totalAmt, out decimal premiumAmt, out decimal shippingCharge, out decimal payPrice)
        {
            ObjectFactory<IInvoiceDA>.Instance.GetShipFee
                (
                soSysNo
                , int.Parse(stockSysNo)
                , out totalAmt
                , out premiumAmt
                , out shippingCharge
                , out payPrice
                );
        }

        /// <summary>
        /// 创建一张发票
        /// </summary>
        /// <param name="invoice">发票信息，包含主信息和Transaction信息</param>
        /// <returns></returns>
        public virtual InvoiceInfo Create(InvoiceInfo invoice)
        {
            try
            {
                return ObjectFactory<IInvoiceDA>.Instance.Create(invoice);
            }
            catch (System.Exception ex)
            {
                //发送内部邮件通知异常信息
                KeyValueVariables replaceVariables = new KeyValueVariables();
                replaceVariables.Add("SO", invoice.MasterInfo.SONumber);
                replaceVariables.Add("Stock", invoice.MasterInfo.StockSysNo);
                EmailHelper.SendEmailByTemplate("", "Invoice_CreateError_Notify", replaceVariables, true);

                throw ex;
            }
        }

        //更新发表号
        public virtual void UpdateSOInvoice(int soSysNo,string invoiceNo,string warehouseNo,string companyCode)
        {

            ObjectFactory<IInvoiceDA>.Instance.UpdateSOInvoice(soSysNo, invoiceNo, warehouseNo,companyCode);

            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                string.Format("用户\"{0}\"更新了编号为\"{1}\"分仓发票号码",ServiceContext.Current.UserSysNo, soSysNo)
                    , BizLogType.Finance_SOIncome_UpdateInvoiceNo
                    ,soSysNo
                    , companyCode);

        }


        #region Private Helper Methods

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.Invoice, msgKeyName), args);
        }

        #endregion Private Helper Methods

        /// <summary>
        /// 同步对账单
        /// </summary>
        /// <param name="billType">
        /// 交易类型
        /// 1 交易对账
        /// 2 实扣税费对账
        /// 3 保证金对账
        /// 4 外币账户对账
        /// </param>
        /// <param name="date">日期，格式：yyyyMMdd</param>
        /// <returns></returns>
        public virtual bool SyncTradeBill(string billType, string date)
        {
            //return (new EasiPayUtils()).SyncTradeBill(billType, date);
            return (new IPSPayUtils()).SyncTradeBill(date);
        }


        public virtual int SOOutStockInvoiceSync(int soSysNo, int stockSysNo, string invoiceNo, string companyCode)
        {
           return ObjectFactory<IInvoiceDA>.Instance.SOOutStockInvoiceSync(soSysNo, stockSysNo, invoiceNo, companyCode);
        }
    }
}