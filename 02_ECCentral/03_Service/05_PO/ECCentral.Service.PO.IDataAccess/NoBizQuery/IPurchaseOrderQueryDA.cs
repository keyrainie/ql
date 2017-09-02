using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface IPurchaseOrderQueryDA
    {
        DataTable QueryPurchaseOrderList(PurchaseOrderQueryFilter queryFilter, out int totalCount);

        DataTable QueryRMAList(PurchaseOrderQueryFilter queryFilter, out int totalCount);

        DataTable CountPurchaseOrder(PurchaseOrderQueryFilter queryFilter);

        /// <summary>
        /// 查询商品的最后一次PO入库的价格
        /// </summary>
        DataTable QueryPurchaseOrderLastPrice(int itemSysNo);

        /// <summary>
        /// 查询PO单退货批次信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        DataTable QueryPurchaseOrderBatchNumberList(PurchaseOrderBatchNumberQueryFilter queryFilter, out int totalCount);

        /// <summary>
        /// 根据供应商编号和PM名称，获取可用返点
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="pmName"></param>
        /// <returns></returns>
        DataTable QuertPurchaseOrderEIMSInvoiceInfo(int vendorSysNo);

        /// <summary>
        /// 获取采购单配件信息List
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        DataTable GetPurchaseOrderAccessories(int poSysNo);

        /// <summary>
        /// 查询PO单历史
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryPurchaseOrderHistory(PurchaseOrderQueryFilter filter, out int totalCount);


        DataTable GetNeedSendMailPOForAutoCloseJob(int poSysNo);
    }
}
