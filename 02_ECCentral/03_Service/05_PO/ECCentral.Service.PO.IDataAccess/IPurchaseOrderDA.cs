using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IPurchaseOrderDA
    {
        /// <summary>
        /// 增加新的PO SysNo ,写入PO_Sequence表:
        /// </summary>
        /// <returns></returns>
        int CreatePOSequenceSysNo();

        /// <summary>
        /// 加载采购单主信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        PurchaseOrderInfo LoadPOMaster(int poSysNo);

        /// <summary>
        /// 加载采购单商品信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        List<PurchaseOrderItemInfo> LoadPOItems(int poSysNo);

        /// <summary>
        /// 加载单个PO ITEM信息
        /// </summary>
        /// <param name="itemSysNo"></param>
        /// <returns></returns>
        PurchaseOrderItemInfo LoadPOItem(int itemSysNo);

        /// <summary>
        /// 记载带有商品信息的采购单信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        List<PurchaseOrderDetailInfo> LoadPODetialInfo(int poSysNo);

        /// <summary>
        /// 加载采购单商品扩展信息
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        PurchaseOrderItemInfo LoadExtendPOItem(int itemSysNo);

        /// <summary>
        /// 加载采购单EIMS信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        List<EIMSInfo> LoadPOEIMSInfo(int poSysNo);

        /// <summary>
        /// 加载采购单EIMS信息(打印)
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        List<EIMSInfo> LoadPOEIMSInfoForPrint(int poSysNo);

        /// <summary>
        /// 加载PO单预计到货时间 - 审核信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        PurchaseOrderETATimeInfo LoadPOETATimeInfo(int poSysNo);

        /// <summary>
        /// 更新采购单主信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        PurchaseOrderInfo UpdatePOMaster(PurchaseOrderInfo entity);

        /// <summary>
        /// 更新PO单的ETA信息
        /// </summary>
        /// <param name="etaInfo"></param>
        /// <returns></returns>
        int UpdatePOMasterETAInfo(PurchaseOrderETATimeInfo etaInfo);

        /// <summary>
        /// 更新PO单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int UpdatePOStatus(PurchaseOrderInfo entity);

        /// <summary>
        /// 更新PO单 - ETA信息
        /// </summary>
        /// <param name="etaInfo"></param>
        /// <returns></returns>
        PurchaseOrderETATimeInfo UpdatePOETAInfo(PurchaseOrderETATimeInfo etaInfo);

        /// <summary>
        /// 更新采购单库存信息(终止入库)
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        PurchaseOrderInfo UpdatePOMasterStock(PurchaseOrderInfo entity);

        /// <summary>
        /// 更新PO单检查结果
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        int UpdateCheckResult(PurchaseOrderInfo poInfo);

        /// <summary>
        /// 创建PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        PurchaseOrderInfo CreatePO(PurchaseOrderInfo poInfo);

        /// <summary>
        /// 创建PO单Item
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        PurchaseOrderItemInfo CreatePOItem(PurchaseOrderItemInfo itemInfo);

        /// <summary>
        /// 创建PO单EIMS信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        EIMSInfo CreatePOEIMSInfo(EIMSInfo entity);

        /// <summary>
        /// 创建PO单的ETA信息
        /// </summary>
        /// <param name="etaInfo"></param>
        /// <returns></returns>
        PurchaseOrderETATimeInfo CreatePOETAInfo(PurchaseOrderETATimeInfo etaInfo);

        /// <summary>
        /// PO单等待入库操作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int WaitingInStockPO(PurchaseOrderInfo entity);

        /// <summary>
        /// 删除PO单EIMS信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        int DeletePOEIMSInfo(int poSysNo);

        /// <summary>
        /// 删除PO items信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        int DeletePOItems(int poSysNo);

        /// <summary>
        /// 删除单个PO Item 信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        int DeletePOItem(int sysNo);

        /// <summary>
        /// 记载PO SSB LOG
        /// </summary>
        /// <returns></returns>
        List<PurchaseOrderSSBLogInfo> LoadPOSSBLog(int poSysNo, PurchaseOrderSSBMsgType msgType);

        /// <summary>
        /// 创建采购单 SSB LOG
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        PurchaseOrderSSBLogInfo CreatePOSSBLog(PurchaseOrderSSBLogInfo logInfo, string companyCode);

        /// <summary>
        /// 更新PO单自动收件人邮件地址
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="autoSendMailAddress"></param>
        /// <returns></returns>
        int UpdatePOAutoSendMailAddress(int poSysNo, string autoSendMailAddress);

        /// <summary>
        /// 更新PO单的MailAddress
        /// </summary>
        /// <param name="POSysNo"></param>
        /// <param name="MailAddress"></param>
        /// <returns></returns>
        int UpdateMailAddressByPOSysNo(int POSysNo, string MailAddress);

        /// <summary>
        /// 获取PO单的MailAddress:
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        string GetMailAddressByPOSysNo(int poSysNo);

        /// <summary>
        /// 更新PO单商品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        PurchaseOrderItemInfo UpdatePOItem(PurchaseOrderItemInfo entity);

        /// <summary>
        /// 更新PO单商品状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        PurchaseOrderInfo UpdatePOItemsStatus(PurchaseOrderInfo entity);

        /// <summary>
        /// 更新PO单 TP STATUS
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int UpdatePOTPStatus(PurchaseOrderInfo entity);

        /// <summary>
        /// 获取PO单 PM联系方式
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        string GetPhoneNumberByPOSysNo(int poSysNo);

        /// <summary>
        /// 发送SSB消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        int CallSSBMessageSP(string message);

        /// <summary>
        /// 获取当天PO单的TotalAmt
        /// </summary>
        /// <param name="POSysNo"></param>
        /// <param name="PmSysNo"></param>
        /// <returns></returns>
        decimal GetPOTotalAmtToday(int POSysNo, int PmSysNo);

        /// <summary>
        /// 获取PO单的合作金额及期限等信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        DataTable GetPOTotalAmt(int poSysNo);

        /// <summary>
        /// 获取PM待入库采购金额
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        decimal GetAuditPOTotalAmt(int pmSysNo);

        /// <summary>
        /// 通过供应商和PM查询EIMS信息:
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        int QueryEIMSInvoiceInfoByPMAndVendor(int vendorSysNo, string pmSysNo);

        /// <summary>
        /// CRL21195 通过供应商查询EIMS信息:
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        int QueryEIMSInvoiceInfoByVendor(int vendorSysNo);

        /// <summary>
        /// 获取PO 日志信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        PurchaseOrderLogInfo LoadPOLogInfo(int poSysNo);

        /// <summary>
        /// 取消作废PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        PurchaseOrderInfo CanecelAbandonPO(PurchaseOrderInfo poInfo);

        /// <summary>
        /// 作废PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        PurchaseOrderInfo AbandonPO(PurchaseOrderInfo poInfo);

        /// <summary>
        /// PM与供应商确认 操作
        /// </summary>
        /// <param name="poInfo"></param>
        int ConfirmWithVendor(PurchaseOrderInfo poInfo);

        /// <summary>
        /// 更新PO单入库备注
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        PurchaseOrderInfo UpdateInStockMemo(PurchaseOrderInfo poInfo);

        /// <summary>
        /// 获取供应商代理产品
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        List<ProductInfo> QueryVendorProductByVendorSysNo(int vendorSysNo);

        /// <summary>
        /// 加载采购单的收货信息
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        List<PurchaseOrderReceivedInfo> LoadPurchaseOrderReceivedInfo(PurchaseOrderInfo poInfo);

        /// <summary>
        /// 查询EIMS合同信息
        /// </summary>
        /// <param name="ruleNumber"></param>
        /// <returns></returns>
        PurchaseOrderEIMSRuleInfo GetEIMSRuleInfoBySysNo(int ruleNumber);

        /// <summary>
        /// 查询EIMS合同信息
        /// </summary>
        /// <param name="ruleNumber"></param>
        /// <returns></returns>
        PurchaseOrderEIMSRuleInfo GetEIMSRuteInfoByAssignedCode(string ruleNumber);

        List<EIMSInfo> GetEIMSInvoiceInfoByPOSysNo(int poSysNo);

        EIMSInfo GetAvilableEimsAmtInfo(int sysNo);

        /// <summary>
        ///  确认 VendorPortal 提交的PO单据信息
        /// </summary>
        /// <returns></returns>
        PurchaseOrderInfo ConfirmVendorPortalPurchaseOrder(PurchaseOrderInfo entity);

        /// <summary>
        /// 退回VP PO单据信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        bool RetreatVendorPortalPurchaseOrder(int sysNo);

        /// <summary>
        /// 更新采购单TotalAmt
        /// </summary>
        /// <param name="poSysNo"></param>
        void UpdatePOMasterTotalAmt(int poSysNo);

        void UpdateAllAcquireReturnInfo(int poSysNo, int? AcquireReturnPointType, decimal? AcquireReturnPoint);

        /// <summary>
        /// 根据商品信息，创建PO Item:
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        PurchaseOrderItemInfo AddPurchaseOrderItemByProduct(PurchaseOrderItemProductInfo productInfo);

        /// <summary>
        /// 获取PO Item 扩展信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        PurchaseOrderItemInfo GetExtendPurchaseOrderItemInfo(int productSysNo);

        /// <summary>
        /// /判断是否是虚库商品
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        bool IsVirtualStockPurchaseOrderProduct(int productSysNo);

        /// <summary>
        /// 获取采购单商品中的赠品信息
        /// </summary>
        /// <param name="productSysNos"></param>
        /// <returns></returns>
        List<PurchaseOrderItemProductInfo> QueryPurchaseOrderGiftList(List<int> productSysNos);

        /// <summary>
        ///  获取采购单商品中的附件信息
        /// </summary>
        /// <param name="productSysNos"></param>
        /// <returns></returns>
        List<PurchaseOrderItemProductInfo> QueryPurchaseOrderAccessoriesList(List<int> productSysNos);

        /// <summary>
        /// 获取采购单商品的ExceptStatus
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        void GetPurchaseOrderItemExceptStatus(PurchaseOrderItemInfo info);

        /// <summary>
        /// 更新po单ITEM退货批次信息
        /// </summary>
        /// <param name="info"></param>
        void UpdatePurchaseOrderBatchInfo(PurchaseOrderItemInfo info);

        /// <summary>
        /// 判断PO负采购时分仓库存是否大于采购数量
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        bool CheckReturnPurchaseOrderValid(int stockSysNo, int productSysNo, int quantity, string companyCode);

        /// <summary>
        /// 检查该供应商已付款30天但缺少发票，或者已付款10但还没收到货的情况
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        DataSet GetPOHistoryAbsentInvoiceOrWaitingInStock(int vendorSysNo, int status, string companyCode);

        /// <summary>
        /// 是否为批量商品
        /// </summary>
        /// <param name="poItem"></param>
        /// <returns></returns>
        bool IsBatchProduct(PurchaseOrderItemInfo poItem);

        /// <summary>
        /// 获取商品附件格式字符串
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        string GetItemAccessoriesStringByPurchaseOrder(List<int?> productSysNoList, string companyCode);

        /// <summary>
        /// 获取Item的京东价
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        decimal? JDPriceByProductSysNo(int productSysNo);

        decimal? GetLastPriceBySysNo(int productSysNo);

        int? GetUnActivatyCount(int productSysNo);

        /// <summary>
        /// 获取最后一次采购时间
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        DateTime? GetLastPoDate(int productSysNo);

        int? GetPurchaseOrderReturnPointSysNo(int poSysNo);

        /// <summary>
        /// 获取PO单打印流水号
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        string GetWareHouseReceiptSerialNumber(int poSysNo);

        /// <summary>
        /// 更新PO单打印流水号
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="receiptSerialNumber"></param>
        int UpdateGetWareHouseReceiptSerialNumber(int poSysNo, string receiptSerialNumber);

        /// <summary>
        /// 根据供应商系统编号取得采购单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        List<int> GetPurchaseOrderSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList);

        List<PurchaseOrderItemInfo> LoadPOItemAddInfo(List<int> productSysNos);

        /// <summary>
        /// 获取PM销售信息
        /// </summary>
        /// <param name="pmUserSysNo"></param>
        /// <param name="saleRatePerMonth"></param>
        /// <param name="saleTargetPerMonth"></param>
        /// <param name="maxAmtPerDay"></param>
        /// <param name="tlSaleRatePerMonth"></param>
        void LoadPMSaleInfo(int pmUserSysNo, out decimal saleRatePerMonth, out decimal saleTargetPerMonth, out decimal maxAmtPerOrder, out decimal maxAmtPerDay, out decimal tlSaleRatePerMonth, out decimal pmdMaxAmtPerOrder, out decimal pmdMaxAmtPerDay, string companyCode);

        /// <summary>
        /// PM当前财务库存金额
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        decimal GetPMInventoryAmt(int pmSysNo, string companyCode);

        /// <summary>
        /// （Check)获取二级类别库存金额
        /// </summary>
        /// <param name="C2SysNo"></param>
        /// <returns></returns>
        DataTable GetC2TotalAmt(string C2SysNo);

        /// <summary>
        /// 获取采购单item最后一次采购价格:
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        decimal? GetPurchaseOrderItemLastPrice(int productSysNo);

        /// <summary>
        ///  获取采购单item的京东价:
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        decimal? GetPurchaseOrderItemJingDongPrice(int productSysNo);

        /// <summary>
        /// 更新采购单入库金额
        /// </summary>
        /// <param name="poSysNo"></param>
        void UpdatePurchaseOrderInstockAmt(int poSysNo);

        /// <summary>
        /// 获取采购单商品W1
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        PurchaseOrderItemInfo GetPurchaseOrderItemSalesTrend(int productSysNo);

        /// <summary>
        /// JOB设置PO单系统关闭
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        int SetPurchaseOrderStatusClose(int poSysNo, string closeUser);

        /// <summary>
        /// 获取需要自动关闭的PO单 （JOB）
        /// </summary>
        /// <returns></returns>
        List<PurchaseOrderInfo> GetNeedToClosePurchaseOrderList();

        /// <summary>
        /// 获取ETA需要关闭的PO单
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<PurchaseOrderInfo> GetPurchaseOrderForJobETA(string companyCode);

        /// <summary>
        /// 获取PO单商品的三级类别基准毛利率
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        DataTable GetPurchaseOrderItemMinInterestRate(string productSysNos);

        /// <summary>
        /// CRL 18058  根据采购单号 获取商品列表
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        List<PurchaseOrderItemInfo> SearchPOItemsList(int poSysNo);


        int AbandonPOForJOB(int poSysNo);
        int AbandonETAForJOB(int poSysNo);
        int UpdateExtendPOInfoForJob(int poSysNo, int productSysNo);

        /// <summary>
        /// 获取po单中 商品的SysNo; 用于PO单特殊的产品线验证 即：更新单据时取老数据的商品sysno 加上新的sysNo 判断产品线是否一致的问题
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        List<int> GetProductSysNoByPOSysNo(int poSysNo);
        /// <summary>
        /// 根据商品编号和成本获取成本库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="cost"></param>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        int GetCostQuantity(int productSysNo, decimal cost, int stockSysNo);        
    }
}
