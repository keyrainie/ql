using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.BizEntity.Invoice;
using System.Data;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IInvoiceInputDA
    {
        #region Load
        /// <summary>
        /// 根据APInvoiceMaster系统编号获取APInvoice主信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        APInvoiceInfo GetAPInvoiceMasterBySysNo(int sysNo);

        /// <summary>
        /// 根据APInvoiceMaster系统编号获取POItems列表
        /// </summary>
        /// <param name="docNo"></param>
        /// <returns></returns>
        List<APInvoicePOItemInfo> GetPOItemsByDocNo(int docNo);

        /// <summary>
        /// 根据APInvoiceMaster系统编号获取InvoiceItems列表
        /// </summary>
        /// <param name="docNo"></param>
        /// <returns></returns>
        List<APInvoiceInvoiceItemInfo> GetInvoiceItemsByDocNo(int docNo);
        /// <summary>
        /// 加载未录入的POitems
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<APInvoicePOItemInfo> LoadNotInputPOItems(APInvoiceItemInputEntity request);
        #endregion

        #region Input
        /// <summary>
        /// 获取无效的发票号码
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        List<string> GetInvalidInvoiceNo(List<string> condition, APInvoiceItemInputEntity entity);

        List<POItemCheckEntity> GetPOCheckList(List<int> condition, APInvoiceItemInputEntity entity);

        List<APInvoicePOItemInfo> GetPOInputItemsHand(List<int> POCondition, PayableOrderType payableOrderType);

        #endregion


        #region Create

        /// <summary>
        /// 创建APInvoiceMaster
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int InsertAPInvoiceMaster(APInvoiceInfo entity);
        /// <summary>
        /// 创建POItem
        /// </summary>
        /// <param name="entitylist"></param>
        /// <param name="docNo"></param>
        void InsertPOItem(List<APInvoicePOItemInfo> entitylist, int docNo);
        /// <summary>
        /// 创建InvoiceItem
        /// </summary>
        /// <param name="entitylist"></param>
        /// <param name="docNo"></param>
        void InsertInvoItem(List<APInvoiceInvoiceItemInfo> entitylist, int docNo);
        #endregion

        #region 更新APinvoice
        /// <summary>
        /// 更新Poitem
        /// </summary>
        /// <param name="poItem"></param>
        /// <param name="docNo"></param>
        void UpdatePOItem(APInvoicePOItemInfo poItem, int docNo);
        /// <summary>
        /// 删除POitems
        /// </summary>
        /// <param name="poNoList"></param>
        void DeletePOItems(List<POItemKeyEntity> poNoList);
        /// <summary>
        /// 更新InvoiceItem
        /// </summary>
        /// <param name="InvoiceItem"></param>
        /// <param name="docNo"></param>
        void UpdateInvoItem(APInvoiceInvoiceItemInfo InvoiceItem, int docNo);
        /// <summary>
        /// 删除Invoices
        /// </summary>
        /// <param name="invoNoList"></param>
        void DeleteInvoItems(List<string> invoNoList);
        /// <summary>
        /// gengxApInvoice Master
        /// </summary>
        /// <param name="entity"></param>
        void UpdateAPInvoiceMaster(APInvoiceInfo entity);

        #endregion

        #region UpdateStatus
        /// <summary>
        /// 更新APInvoiceMaster状态
        /// </summary>
        /// <param name="entity"></param>
        void UpdateAPInvoiceStatus(APInvoiceInfo entity);
        /// <summary>
        /// 更新APInvoiceMaster状态及处理人
        /// </summary>
        /// <param name="entity"></param>
        void UpdateAPInvoiceStatusWithConfirmUser(APInvoiceInfo entity);

        void UpdatePOItemStatus(int docNo);

        void UpdateInvoItemStatus(int docNo);
        /// <summary>
        ///   检查POItem状态是否已通过审核
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckPOItemAudit(APInvoicePOItemInfo item);
        /// <summary>
        /// 获取FinancePaySysNo
        /// </summary>
        /// <param name="poNo"></param>
        /// <param name="orderType"></param>
        /// <param name="batchNumber"></param>
        /// <returns></returns>
        int GetFinancePaySysNo(int poNo, PayableOrderType orderType, int? batchNumber);

        #endregion

    }
}