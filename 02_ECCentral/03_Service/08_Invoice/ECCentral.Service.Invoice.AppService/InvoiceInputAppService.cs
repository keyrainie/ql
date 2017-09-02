using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(InvoiceInputAppService))]
    public class InvoiceInputAppService
    {
        private InvoiceInputProcessor processor = ObjectFactory<InvoiceInputProcessor>.Instance;

        #region BatchAction
        /// <summary>
        /// 退回供应商
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="vpCancelReason"></param>
        /// <returns></returns>
        public string BatchVPCancel(List<int> sysNoList, string vpCancelReason)
        {
            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.VPCancel(sysNo, vpCancelReason);

            });
            return result.PromptMessage;
        }
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public string BatchSubmitAPInvoice(List<int> sysNoList)
        {
            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.Submit(sysNo);

            });
            return result.PromptMessage;
        }


        /// <summary>
        /// 撤销审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public string BatchCancelAPInvoice(List<int> sysNoList)
        {
            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.CancelAudit(sysNo);

            });
            return result.PromptMessage;
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public string BatchAuditAPInvoice(List<int> sysNoList)
        {
            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.Audit(sysNo);

            });
            return result.PromptMessage;
        }
        /// <summary>
        /// 拒绝审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public string BatchRefuseAPInvoice(List<int> sysNoList)
        {
            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.RefuseAudit(sysNo);

            });
            return result.PromptMessage;
        }

        private List<BatchActionItem<int>> GetRequestItem(List<int> sysNoList)
        {
            return sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();
        }
        #endregion

        #region Load

        /// <summary>
        /// 根据ApInvoiceMaster 系统编号加载APInvoice全部信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo LoadAPInvoiceWithItemsBySysNo(int sysNo)
        {
            return processor.LoadAPInvoiceWithItemsBySysNo(sysNo);
        }
        /// <summary>
        /// 根据APInvoice_Master的系统编号获取其POItems列表
        /// </summary>
        /// <param name="docNo"></param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> GetPOItemsByDocNo(int docNo)
        {
            return processor.GetPOItemsByDocNo(docNo);
        }
        /// <summary>
        /// 根据APInvoice_Master的系统编号获取其InvoiceItems列表
        /// </summary>
        /// <param name="docNo"></param>
        /// <returns></returns>
        public virtual List<APInvoiceInvoiceItemInfo> GetInvoiceItemsByDocNo(int docNo)
        {
            return processor.GetInvoiceItemsByDocNo(docNo);
        }

        /// <summary>
        /// 加载供应商未录入的POItems
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> LoadNotInputPOItems(APInvoiceItemInputEntity request)
        {
            return processor.LoadNotInputPOItems(request);
        }
        #endregion

        #region Action--Create&Update
        /// <summary>
        /// 创建APInvoice
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo CreateAPInvoice(APInvoiceInfo data)
        {
            return processor.CreateAPInvoice(data);
        }
        /// <summary>
        /// 更新APInvoice
        /// </summary>
        /// <param name="data"></param>
        public virtual APInvoiceInfo UpdateAPInvoice(APInvoiceInfo data)
        {
            return processor.UpdateAPInvoice(data);
        }
        /// <summary>
        /// 提交审核时保存（创建或更新）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo SubmitWithSaveAPInvoice(APInvoiceInfo data)
        {
            return processor.SubmitWithSaveAPInvoice(data);
        }

        #endregion

        #region Input
        /// <summary>
        /// 录入InvoiceItem
        /// </summary>
        /// <param name="inputEntity"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public virtual List<APInvoiceInvoiceItemInfo> InputInvoiceItem(APInvoiceItemInputEntity inputEntity, ref string errorMsg)
        {
            return processor.InputInvoiceItem(inputEntity, ref errorMsg);
        }
        /// <summary>
        /// 录入PoItem
        /// </summary>
        /// <param name="checkInfo"></param>
        /// <param name="vendorName"></param>
        /// <param name="errorMsg"></param>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> InputPoItem(APInvoiceItemInputEntity inputEntity, ref string vendorName, ref string errorMsg, out int vendorSysNo)
        {
            return processor.InputPoItem(inputEntity, ref vendorName, ref errorMsg, out vendorSysNo);
        }
        #endregion
    }
}
