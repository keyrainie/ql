using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.AppService;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.BizEntity.Inventory;
using System.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 查询采购单列表:
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/QueryPurchaseOrderList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPurchaseOrderList(PurchaseOrderQueryFilter request)
        {
            request.PMAuthorizedList = new List<int>();
            request.PMAuthorizedList = ObjectFactory<PurchaseOrderAppService>.Instance.GetAuthorizedPMList(request.PMQueryType.Value, request.CurrentUserName, request.CompanyCode).Select(x => x.SysNo.Value).ToList();
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IPurchaseOrderQueryDA>.Instance.QueryPurchaseOrderList(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        // 查询最后一次PO入库的item价格
        [WebInvoke(UriTemplate = "/PurchaseOrder/QueryPurchaseOrderLastPrice/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPurchaseOrderLastPrice(string sysNo)
        {
            QueryResult result = new QueryResult()
            {
                Data = ObjectFactory<IPurchaseOrderQueryDA>.Instance.QueryPurchaseOrderLastPrice(Convert.ToInt32(sysNo))
            };
            return result;
        }

        /// <summary>
        /// 按采购单状态统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/CountPurchaseOrderList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult CountPurchaseOrderList(PurchaseOrderQueryFilter request)
        {
            request.PMAuthorizedList = new List<int>();
            request.PMAuthorizedList = ObjectFactory<PurchaseOrderAppService>.Instance.GetAuthorizedPMList(request.PMQueryType.Value, request.CurrentUserName, request.CompanyCode).Select(x => x.SysNo.Value).ToList();
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IPurchaseOrderQueryDA>.Instance.CountPurchaseOrder(request)
            };
            return returnResult;
        }

        /// <summary>
        /// 查询采购单RMA List
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/QueryPurchaseOrderRMAList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPurchaseOrderRMAList(PurchaseOrderQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IPurchaseOrderQueryDA>.Instance.QueryRMAList(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 查询PO单退货批次信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/QueryPurchaseOrderBatchNumberList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPurchaseOrderBatchNumberList(PurchaseOrderBatchNumberQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int totalCount = 0;
            result.Data = ObjectFactory<IPurchaseOrderQueryDA>.Instance.QueryPurchaseOrderBatchNumberList(queryFilter, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 根据供应商编号，获取可用返点
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/QueryPurchaseOrderEIMSInvoiceInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPurchaseOrderEIMSInvoiceInfo(PurchaseOrderQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult()
            {
                Data = ObjectFactory<IPurchaseOrderQueryDA>.Instance.QuertPurchaseOrderEIMSInvoiceInfo(Convert.ToInt32(queryFilter.VendorSysNo)),
                TotalCount = 0
            };
            return result;
        }

        /// <summary>
        /// 查询PO单历史
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/QueryPurchaseOrderHistory", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPurchaseOrderHistory(PurchaseOrderQueryFilter filter)
        {
            int totalCount = 0;
            QueryResult result = new QueryResult()
            {
                Data = ObjectFactory<IPurchaseOrderQueryDA>.Instance.QueryPurchaseOrderHistory(filter, out totalCount)
            };
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 加载采购单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/LoadPurchaseOrderInfo/{sysNo}", Method = "GET")]
        public PurchaseOrderInfo LoadPurchaseOrderInfo(string sysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.LoadPurchaseOrderInfo(Convert.ToInt32(sysNo));
        }

        /// <summary>
        /// 加载单个PO ITEM信息
        /// </summary>
        /// <param name="itemSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/LoadPurchaseOrderItemInfo/{itemSysNo}", Method = "GET")]
        public PurchaseOrderItemInfo LoadPurchaseOrderItemInfo(string itemSysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.LoadPurchaseOrderItemInfo(Convert.ToInt32((itemSysNo)));
        }

        /// <summary>
        /// 创建PO单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/CreatePurchaseOrderInfo", Method = "PUT")]
        public PurchaseOrderInfo CreatePurchaseOrderInfo(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.CreatePurchaseOrderInfo(info);
        }

        /// <summary>
        /// 检查PO单信息
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/CheckPurchaseOrderInfo", Method = "PUT")]
        public PurchaseOrderInfo CheckPurchaseOrderInfo(PurchaseOrderInfo poInfo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.CheckPOInfo(poInfo);
        }

        /// <summary>
        /// 更新PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/UpdatePurchaseOrderInfo", Method = "PUT")]
        public PurchaseOrderInfo UpdatePurchaseOrderInfo(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.UpdatePurchaseOrderInfo(info);
        }

        /// <summary>
        /// 提交审核PO单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/SubmitAuditPO", Method = "PUT")]
        public PurchaseOrderInfo SubmitPurchaseOrder(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.SubmitAuditPurchaseOrder(info);
        }

        /// <summary>
        /// 确认PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/VerifyPO", Method = "PUT")]
        public PurchaseOrderInfo VerifyPurchaseOrder(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.VerifyPurchaseOrder(info);
        }

        /// <summary>
        /// 取消确认PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/CancelVerifyPO", Method = "PUT")]
        public PurchaseOrderInfo CancelVerifyPurchaseOrder(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.CancelVerifyPurchaseOrder(info);
        }

        /// <summary>
        /// 拒绝并退回PO单:
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/RefusePO", Method = "PUT")]
        public PurchaseOrderInfo RefusePurchaseOrder(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.RefusePurchaseOrder(info);
        }

        /// <summary>
        /// 作废PO单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/AbandonPO", Method = "PUT")]
        public PurchaseOrderInfo AbandonPurchaseOrder(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.AbandonPurchaseOrder(info);
        }

        /// <summary>
        /// 取消作废PO单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/CancelAbandonPO", Method = "PUT")]
        public PurchaseOrderInfo CancelAbandonPurchaseOrder(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.CancelAbandonPurchaseOrder(info);
        }

        /// <summary>
        /// PM与供应商确认
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/PMConfirmWithVendorPO", Method = "PUT")]
        public PurchaseOrderInfo PMConfirmWithVendor(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.PMConfirmWithVendor(info);
        }

        /// <summary>
        /// 更新入库备注和到付运费金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/UpdateInStockMemoPO", Method = "PUT")]
        public PurchaseOrderInfo UpdateInStockMemoPO(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.UpdateInfoStockMemo(info);
        }

        /// <summary>
        /// PO单中止入库
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/StopInStockPO", Method = "PUT")]
        public PurchaseOrderInfo StopInStockPO(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.StopInStockPO(info);
        }

        /// <summary>
        /// 查询EIMS合同信息
        /// </summary>
        /// <param name="ruleNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/GetEIMSRuleInfoBySysNo/{ruleNo}", Method = "GET")]
        public PurchaseOrderEIMSRuleInfo GetEIMSRuleInfoBySysNo(string ruleNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.QueryEIMSRuleInfoByNumber(Convert.ToInt32(ruleNo));
        }

        /// <summary>
        /// 查询EIMS合同信息
        /// </summary>
        /// <param name="ruleNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/GetEIMSRuleInfoByAssignedCode/{id}", Method = "GET")]
        public PurchaseOrderEIMSRuleInfo GetEIMSRuleInfoByAssignedCode(string id)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.QueryEIMSRuteInfoByAssignedCode(id);
        }


        /// <summary>
        /// 审核通过  -采购单预计到货时间
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/PassPurchaseOrderETAInfo", Method = "PUT")]
        public PurchaseOrderETATimeInfo PassPurchaseOrderETAInfo(PurchaseOrderETATimeInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.PassETAInfo(info);
        }

        /// <summary>
        /// 取消审核  -采购单预计到货时间
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/CancelPurchaseOrderETAInfo", Method = "PUT")]
        public PurchaseOrderETATimeInfo CancelPurchaseOrderETAInfo(PurchaseOrderETATimeInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.CancelETAInfo(info);
        }

        /// <summary>
        /// 提交审核  -采购单预计到货时间
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/SubmitPurchaseOrderETAInfo", Method = "PUT")]
        public PurchaseOrderETATimeInfo SubmitPurchaseOrderETAInfo(PurchaseOrderETATimeInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.SubmitETAInfo(info);
        }

        /// <summary>
        /// 确认 VendorPortal 提交的PO单据信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/ConfirmVendorPortalPO", Method = "PUT")]
        public PurchaseOrderInfo ConfirmVendorPortalPO(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.ConfirmVendorPortalPurchaseOrder(info);
        }

        /// <summary>
        /// 审核 VendorPortal 提交的PO单据信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/AuditVendorPortalPO", Method = "PUT")]
        public PurchaseOrderInfo AuditVendorPortalPO(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.AuditVendorPortalPurchaseOrder(info);
        }

        /// <summary>
        /// 补充创建PO单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/RenewCreatePurchaseOrder", Method = "PUT")]
        public PurchaseOrderInfo RenewCreatePurchaseOrder(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.RenewCreatePO(info);
        }

        /// <summary>
        /// 退回 VendorPortal 创建的PO单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/RetreatVendorPortalPurchaseOrder", Method = "PUT")]
        public void RetreatVendorPortalPurchaseOrder(PurchaseOrderRetreatReq request)
        {
            ObjectFactory<PurchaseOrderAppService>.Instance.RetreatVendorPortalPurchaseOrder(request.poSysNo.Value, request.retreatType);
        }

        /// <summary>
        /// 采购单添加新商品，获取Item信息
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/AddNewPurchaseOrderItem", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public PurchaseOrderItemInfo AddPurchaseOrderItem(PurchaseOrderItemProductInfo itemInfo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.AddPurchaseOrderItemFromProductInfo(itemInfo);
        }



        /// <summary>
        /// (批量)采购单添加新商品，获取Item信息
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/BatchAddPurchaseOrderItems", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<PurchaseOrderItemInfo> BatchAddPurchaseOrderItems(List<PurchaseOrderItemProductInfo> itemInfoList)
        {
            List<PurchaseOrderItemInfo> returnList = new List<PurchaseOrderItemInfo>();
            itemInfoList.ForEach(x =>
            {
                returnList.Add(ObjectFactory<PurchaseOrderAppService>.Instance.AddPurchaseOrderItemFromProductInfo(x));
            });
            return returnList;
        }

        /// <summary>
        /// 获取采购单赠品信息
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/GetPurchaseOrderGiftInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<PurchaseOrderItemProductInfo> GetPurchaseOrderGiftInfo(List<int> productSysNoList)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetPurchaseOrderGiftInfo(productSysNoList);
        }

        /// <summary>
        /// 获取采购单附件信息
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/GetPurchaseOrderAccessoriesInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<PurchaseOrderItemProductInfo> GetPurchaseOrderAccessoriesInfo(List<int> productSysNoList)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetPurchaseOrderAccessoriesInfo(productSysNoList);
        }

        /// <summary>
        /// 审核通过后发邮件  标识采购单已经发过邮件（HasSendMail=1）。记录至MailAddress
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/PurchaseOrder/UpdateMailAddressAndHasSentMail", Method = "PUT")]
        public void UpdateMailAddressAndHasSentMail(PurchaseOrderInfo info)
        {
            ObjectFactory<PurchaseOrderAppService>.Instance.UpdateMailAddressAndHasSentMail(info.SysNo.Value, info.PurchaseOrderBasicInfo.MailAddress, info.CompanyCode);
        }

        /// <summary>
        /// 更新PO单ITEM的批次信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/PurchaseOrder/UpdatePurchaseOrderBatchInfo", Method = "PUT")]
        public void UpdatePurchaseOrderBatchInfo(PurchaseOrderItemInfo info)
        {
            ObjectFactory<PurchaseOrderAppService>.Instance.UpdatePurchaeOrderBatchInfo(info);
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/GetPurchaseOrderWarehouseList", Method = "POST")]
        public List<WarehouseInfo> GetPurchaseOrderWarehouseList(string companyCode)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetPurchaseOrderWarehouseList(companyCode);
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/LoadPurchaseOrderEIMSInfo/{poSysNo}", Method = "GET")]
        public List<EIMSInfo> LoadPurchaseOrderEIMSInfo(string poSysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.LoadPurchaseOrderEIMSInfo(Convert.ToInt32(poSysNo));
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/UpdatePurchaseOrderInstockAmt", Method = "PUT")]
        public int UpdatePurchaseOrderInstockAmt(string poSysNo)
        {
            ObjectFactory<PurchaseOrderAppService>.Instance.UpdatePurchaseOrderInstockAmt(Convert.ToInt32(poSysNo));
            return 1;
        }
        [WebInvoke(UriTemplate = "/PurchaseOrder/PurchaseOrderStatusClose", Method = "PUT")]
        public int PurchaseOrderStatusClose(PurchaseOrderInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.SetPurchaseOrderStatusClose(info.SysNo.Value, info.PurchaseOrderBasicInfo.CreateUserName);
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/AdjustPurchaseOrderQtyInventory", Method = "PUT")]
        public virtual int AdjustPurchaseOrderQtyInventory(InventoryAdjustContractInfo info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.AdjustPurchaseOrderQtyInventory(info);
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/GetMailContentForAutoClosePOJob/{poSysNo}", Method = "GET")]
        public virtual string GetMailContentForAutoClosePOJob(string poSysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetMailContentForAutoClosePOJob(Convert.ToInt32(poSysNo));
        }

        /// <summary>
        /// 获取需要自动关闭的PO单（JOB）
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/GetNeedToClosePurchaseOrderList", Method = "POST")]
        public virtual List<PurchaseOrderInfo> GetNeedToClosePurchaseOrderList()
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetNeedToClosePurchaseOrderList();
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/GetNeedSendMailPOForAutoCloseJob/{poSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetNeedSendMailPOForAutoCloseJob(string poSysNo)
        {
            QueryResult result = new QueryResult()
            {
                Data = ObjectFactory<IPurchaseOrderQueryDA>.Instance.GetNeedSendMailPOForAutoCloseJob(Convert.ToInt32(poSysNo))
            };
            result.TotalCount = result.Data.Rows.Count;
            return result;
        }

        /// <summary>
        /// 发送PO单自动关闭SSB消息 （JOB）
        /// </summary>
        /// <param name="poSysNoAndUserSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/SendPuchaseOrderAutoCloseSSBMessage", Method = "PUT")]
        public int SendPuchaseOrderAutoCloseSSBMessage(string poSysNoAndUserSysNo)
        {
            int poSysNo = Convert.ToInt32(poSysNoAndUserSysNo.Split('|')[0].Trim());
            int userSysNo = Convert.ToInt32(poSysNoAndUserSysNo.Split('|')[1].Trim());
            string companyCode = poSysNoAndUserSysNo.Split('|')[2].Trim();
            return ObjectFactory<PurchaseOrderAppService>.Instance.SendPurchaseOrderCloseSSBMessage(poSysNo, userSysNo, companyCode);
        }

        /// <summary>
        /// 获取EMA可用的POList(JOB)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/GetPurchaseOrderListForETA", Method = "POST")]
        public List<PurchaseOrderInfo> GetPurchaseOrderListForETA(string companyCode)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetPurchaseOrderForETA(companyCode);
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/GetFinanceItemByPOSysNo", Method = "POST")]
        public PayItemInfo GetFinanceItemByPOSysNo(string poSysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetPayItemInfoByPOSysNo(Convert.ToInt32(poSysNo));
        }
        [WebInvoke(UriTemplate = "/PurchaseOrder/InsertFinancePayInfo", Method = "PUT")]
        public int InsertFinancePayInfo(PayItemInfo info)
        {
            ObjectFactory<PurchaseOrderAppService>.Instance.InsertPayItemInfo(info);
            return 1;
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/AbandonPOForJob", Method = "PUT")]
        public int AbandonPOForJob(string poSysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.AbandonPOForJob(Convert.ToInt32(poSysNo));
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/AbandonETAForJob", Method = "PUT")]
        public int AbandonETAForJob(string poSysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.AbandonETAForJob(Convert.ToInt32(poSysNo));
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/UpdateExtendPOInfoForJob", Method = "PUT")]
        public int UpdateExtendPOInfoForJob(string poItemSysNoAndProductSysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.UpdateExtendPOInfoForJob(Convert.ToInt32(poItemSysNoAndProductSysNo.Split('|')[0].Trim()), Convert.ToInt32(poItemSysNoAndProductSysNo.Split('|')[1].Trim()));
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/SendMailWhenAuditPurchaseOrder", Method = "PUT")]
        public int SendMailWhenAuditPurchaseOrder(PurchaseOrderSendMailReq request)
        {
            if (null != request)
            {
                return ObjectFactory<PurchaseOrderAppService>.Instance.SendMailWhenAuditPurchaseOrder(request.MailContent, request.PurchaseOrderSysNo.Value, request.MailAddress);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 根据传入的ProductSysno 检测与当前PM是否匹配
        /// </summary>
        [WebInvoke(UriTemplate = "/PurchaseOrder/CheckOperateRightForCurrentUser", Method = "PUT")]
        public bool CheckOperateRightForCurrentUser(ProductPMLine info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.CheckOperateRightForCurrentUser(info);
        }

        /// <summary>
        /// 根据商品sysNo获取每个商品的产品线和所属PM
        /// </summary>
        [WebInvoke(UriTemplate = "/PurchaseOrder/GetProductLineSysNoByProductList", Method = "POST")]
        public List<ProductPMLine> GetProductLineSysNoByProductList(int[] info)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetProductLineSysNoByProductList(info);
        }
        /// <summary>
        /// 根据PM，获取其全部产品线
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/GetProductLineInfoByPM", Method = "POST")]
        public List<ProductPMLine> GetProductLineInfoByPM(int pmSysNo)
        {
            return ObjectFactory<PurchaseOrderAppService>.Instance.GetProductLineInfoByPM(pmSysNo);
        }
    }
}
