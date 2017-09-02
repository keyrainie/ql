using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.IBizInteract
{
    public interface IPOBizInteract
    {
        /// <summary>
        /// 获取PO单信息
        /// </summary>
        /// <param name="purchaseOrderSysNo">PO单编号</param>
        /// <returns>PO单信息</returns>
        PurchaseOrderInfo GetPurchaseOrderInfo(int purchaseOrderSysNo);

        /// <summary>
        /// 获取PO单信息
        /// </summary>
        /// <param name="purchaseOrderSysNo">PO单编号</param>
        /// <param name="batchNumber">批次号</param>
        /// <returns>PO单信息</returns>
        PurchaseOrderInfo GetPurchaseOrderInfo(int purchaseOrderSysNo, int batchNumber);

        /// <summary>
        /// 获取代销结算单信息
        /// </summary>
        /// <param name="consignSettlementSysNo">代销结算单编号</param>
        /// <returns>代销结算单信息</returns>
        ConsignSettlementInfo GetConsignSettlementInfo(int consignSettlementSysNo);

        /// <summary>
        /// 获取代收结算单信息
        /// </summary>
        /// <param name="gatherSettlementSysNo">代收结算单编号</param>
        /// <returns>代收结算单信息</returns>
        GatherSettlementInfo GetGatherSettlementInfo(int gatherSettlementSysNo);


        /// <summary>
        /// 获取代收代付结算单信息
        /// </summary>
        /// <param name="consignSettlementSysNo">代销结算单编号</param>
        /// <returns>代销结算单信息</returns>
        CollectionPaymentInfo GetCollectionPaymentInfo(int consignSettlementSysNo);

        /// <summary>
        /// 获取佣金信息
        /// </summary>
        /// <param name="commissionMasterSysNo">佣金信息编号</param>
        /// <returns>佣金信息</returns>
        CommissionMaster GetCommissionMaster(int commissionMasterSysNo);

        /// <summary>
        /// 创建PO单
        /// </summary>
        /// <param name="PurchaseOrderInfo">需要创建的PO单</param>
        /// <returns>PO单编号</returns>
        string CreatePurchaseOrder(PurchaseOrderInfo purchaseOrderInfo);

        /// <summary>
        /// 批量创建代销转库存记录
        /// </summary>
        /// <param name="ConsignToAcctLogInfo">需要创建代销转财务记录</param>
        /// <returns>代销转财务记录</returns>
        List<ConsignToAcctLogInfo> BatchCreateConsignToAcctLogs(List<ConsignToAcctLogInfo> consignToAcctLogInfos);

        /// <summary>
        /// 批量创建代销转库存记录(Inventory)
        /// </summary>
        /// <param name="ConsignToAcctLogInfo">需要创建代销转财务记录</param>
        /// <returns>代销转财务记录</returns>
        List<ConsignToAcctLogInfo> BatchCreateConsignToAcctLogsInventory(List<ConsignToAcctLogInfo> consignToAcctLogInfos);

        /// <summary>
        /// 获取供应商信息
        /// </summary>
        /// <param name="vendorSysNo">供应商系统编号</param>
        /// <returns></returns>
        VendorInfo GetVendorInfoSysNo(int vendorSysNo);

        /// <summary>
        /// 获取供应商财务信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        VendorFinanceInfo GetVendorFinanceInfoBySysNo(int vendorSysNo);

        /// <summary>
        /// 根据VendorType获取商家名称List
        /// </summary>
        /// <param name="vendorType">类型(IPP = 0,VendorPortal= 1)</param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<KeyValuePair<int, string>> GetVendorNameListByVendorType(VendorType vendorType, string companyCode);

        #region For Invoice

        /// <summary>
        /// 判断指定SysNo供应商是否已锁定
        /// </summary>
        /// <param name="vendorSysNo">供应商SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorByVendorSysNo(int vendorSysNo);

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorByPOSysNo(int poSysNo);

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorByVendorSettleSysNo(int vendorSettleSysNo);

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="collectionSettlementSysNo">代收结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorByCollectionSettlementSysNo(int collectionSettlementSysNo);

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorPMByPOSysNo(int poSysNo);

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        bool IsHolderVendorPMByVendorSettleSysNo(int vendorSettleSysNo);

        /// <summary>
        /// 加载PO单返点信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        List<EIMSInfo> LoadPurchaseOrderEIMSInfo(int poSysNo);

        /// <summary>
        /// 采购单中止入库
        /// </summary>
        /// <param name="poSysNo">采购单系统编号</param>
        void StopInStock(int poSysNo);

        int? GetPurchaseOrderReturnPointSysNo(int poSysNo);

        int? GetConsignSettlementReturnPointSysNo(int consignSettleSysNo);

        /// <summary>
        /// 根据供应商系统编号取得PO单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        List<int> GetPurchaseOrderSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList);

        /// <summary>
        /// 根据供应商系统编号取得代销结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        List<int> GetVendorSettleSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList);

        /// <summary>
        /// 根据供应商系统编号取得代收结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        List<int> GetGatherSettlementSysNoListByVendorSysNo(int vendorSysNo);

        #endregion For Invoice

        /// <summary>
        /// 获取指定商品的最后一次采购时间
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        DateTime? GetLastPoDate(int productSysNo);

        /// <summary>
        /// 备货中心- 采购
        /// </summary>
        /// <param name="itemsList"></param>
        /// <returns></returns>
        int CreateBasketItemsForPrepare(List<BasketItemsInfo> itemsList);

        /// <summary>
        /// 获取供应商基本信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        VendorBasicInfo GetVendorBasicInfoBySysNo(int vendorSysNo);

        /// <summary>
        /// 获取供应商代理信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        List<VendorAgentInfo> GetVendorAgentInfo(VendorInfo vendorInfo);
    }
}
