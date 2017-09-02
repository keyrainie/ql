using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.AppService
{
    [Export(typeof(IPOBizInteract))]
    [VersionExport(typeof(POBizInteractAppService))]
    public class POBizInteractAppService : IPOBizInteract
    {
        #region IPOBizInteract Members

        /// <summary>
        /// 获取PO单信息
        /// </summary>
        /// <param name="purchaseOrderSysNo">PO单编号</param>
        /// <returns>PO单信息</returns>
        public BizEntity.PO.PurchaseOrderInfo GetPurchaseOrderInfo(int purchaseOrderSysNo)
        {
            return ObjectFactory<PurchaseOrderProcessor>.Instance.LoadPO(purchaseOrderSysNo);
        }

        /// <summary>
        /// 获取PO单信息
        /// </summary>
        /// <param name="purchaseOrderSysNo">PO单编号</param>
        /// <param name="batchNumber">批次号</param>
        /// <returns>PO单信息</returns>
        public PurchaseOrderInfo GetPurchaseOrderInfo(int purchaseOrderSysNo, int batchNumber)
        {
            return ObjectFactory<PurchaseOrderProcessor>.Instance.LoadPO(purchaseOrderSysNo);
        }

        /// <summary>
        /// 获取代销结算单信息
        /// </summary>
        /// <param name="consignSettlementSysNo">代销结算单编号</param>
        /// <returns>代销结算单信息</returns>
        public BizEntity.PO.ConsignSettlementInfo GetConsignSettlementInfo(int consignSettlementSysNo)
        {
            return ObjectFactory<ConsignSettlementProcessor>.Instance.LoadConsignSettlementInfo(consignSettlementSysNo);
        }

        /// <summary>
        /// 获取代收结算单信息
        /// </summary>
        /// <param name="gatherSettlementSysNo">代收结算单编号</param>
        /// <returns>代收结算单信息</returns>
        public GatherSettlementInfo GetGatherSettlementInfo(int gatherSettlementSysNo)
        {
            GatherSettlementInfo info = new GatherSettlementInfo()
            {
                SysNo = gatherSettlementSysNo
            };
            return ObjectFactory<GatherSettlementProcessor>.Instance.LoadGetherSettlement(info);
        }

        /// <summary>
        /// 获取代销结算单信息
        /// </summary>
        /// <param name="consignSettlementSysNo">代销结算单编号</param>
        /// <returns>代销结算单信息</returns>
        public BizEntity.PO.CollectionPaymentInfo GetCollectionPaymentInfo(int consignSettlementSysNo)
        {
            return ObjectFactory<CollectionPaymentProcessor>.Instance.Load(consignSettlementSysNo);
        }


        /// <summary>
        /// 获取佣金信息
        /// </summary>
        /// <param name="commissionMasterSysNo">佣金信息编号</param>
        /// <returns>佣金信息</returns>
        public CommissionMaster GetCommissionMaster(int commissionMasterSysNo)
        {
            return ObjectFactory<CommissionProcessor>.Instance.LoadCommissionInfo(commissionMasterSysNo);
        }

        /// <summary>
        /// 创建采购单
        /// </summary>
        /// <param name="purchaseOrderInfo"></param>
        /// <returns></returns>
        public string CreatePurchaseOrder(BizEntity.PO.PurchaseOrderInfo purchaseOrderInfo)
        {
            PurchaseOrderInfo info = ObjectFactory<PurchaseOrderProcessor>.Instance.CreatePO(purchaseOrderInfo);
            return info.SysNo.HasValue ? info.SysNo.Value.ToString() : null;
        }

        /// <summary>
        /// 批量创建代销转库存记录
        /// </summary>
        /// <param name="ConsignToAcctLogInfo">需要创建代销转财务记录</param>
        /// <returns>代销转财务记录</returns>
        public List<ConsignToAcctLogInfo> BatchCreateConsignToAcctLogs(List<ConsignToAcctLogInfo> consignToAcctLogInfos)
        {
            List<ConsignToAcctLogInfo> returnList = new List<ConsignToAcctLogInfo>();
            foreach (var item in consignToAcctLogInfos)
            {
                returnList.Add(ObjectFactory<ConsignSettlementProcessor>.Instance.CreateConsignToAcccountLog(item));
            }
            return returnList;
        }

        /// <summary>
        /// 批量创建代销转库存记录(Inventory)
        /// </summary>
        /// <param name="ConsignToAcctLogInfo">需要创建代销转财务记录</param>
        /// <returns>代销转财务记录</returns>
        public List<ConsignToAcctLogInfo> BatchCreateConsignToAcctLogsInventory(List<ConsignToAcctLogInfo> consignToAcctLogInfos)
        {
            List<ConsignToAcctLogInfo> returnList = new List<ConsignToAcctLogInfo>();
            foreach (var item in consignToAcctLogInfos)
            {
                returnList.Add(ObjectFactory<ConsignSettlementProcessor>.Instance.CreatePOConsignToAccLogForInventory(item));
            }
            return returnList;
        }
        

        /// <summary>
        /// 获取供应商财务信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public BizEntity.PO.VendorInfo GetVendorInfoSysNo(int vendorSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.LoadVendorInfo(vendorSysNo);
        }

        /// <summary>
        /// 获取供应商财务信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public VendorFinanceInfo GetVendorFinanceInfoBySysNo(int vendorSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.LoadVendorFinanceInfo(vendorSysNo);
        }

        #endregion IPOBizInteract Members

        #region For Invoice

        /// <summary>
        /// 判断指定SysNo供应商是否已锁定
        /// </summary>
        /// <param name="vendorSysNo">供应商SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorByVendorSysNo(int vendorSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.IsHolderVendorByVendorSysNo(vendorSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorByPOSysNo(int poSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.IsHolderVendorByPOSysNo(poSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorByVendorSettleSysNo(int vendorSettleSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.IsHolderVendorByVendorSettleSysNo(vendorSettleSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="collectionSettlementSysNo">代收结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorByCollectionSettlementSysNo(int collectionSettlementSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.IsHolderVendorByCollectionSettlementSysNo(collectionSettlementSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorPMByPOSysNo(int poSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.IsHolderVendorPMByPOSysNo(poSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorPMByVendorSettleSysNo(int vendorSettleSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.IsHolderVendorPMByVendorSettleSysNo(vendorSettleSysNo);
        }

        public List<KeyValuePair<int, string>> GetVendorNameListByVendorType(VendorType vendorType, string companyCode)
        {
            return ObjectFactory<VendorProcessor>.Instance.GetVendorNameListByVendorType(vendorType, companyCode);
        }

        public List<EIMSInfo> LoadPurchaseOrderEIMSInfo(int poSysNo)
        {
            return ObjectFactory<PurchaseOrderProcessor>.Instance.LoadPOEIMSInfo(poSysNo);
        }

        public void StopInStock(int poSysNo)
        {
            PurchaseOrderInfo info = new PurchaseOrderInfo()
            {
                SysNo = poSysNo
            };
            ObjectFactory<PurchaseOrderProcessor>.Instance.StopInStockPO(info);
        }

        public int? GetPurchaseOrderReturnPointSysNo(int poSysNo)
        {
            return ObjectFactory<PurchaseOrderProcessor>.Instance.GetPurchaseOrderReturnPointSysNo(poSysNo);
        }

        public int? GetConsignSettlementReturnPointSysNo(int consignSettleSysNo)
        {
            return ObjectFactory<ConsignSettlementProcessor>.Instance.GetConsignSettlementReturnPointSysNo(consignSettleSysNo);
        }

        public List<int> GetPurchaseOrderSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            return ObjectFactory<PurchaseOrderProcessor>.Instance.GetPurchaseOrderSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);
        }

        public List<int> GetVendorSettleSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            return ObjectFactory<ConsignSettlementProcessor>.Instance.GetVendorSettleSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);
        }

        public List<int> GetGatherSettlementSysNoListByVendorSysNo(int vendorSysNo)
        {
            return ObjectFactory<GatherSettlementProcessor>.Instance.GetGatherSettlementSysNoListByVendorSysNo(vendorSysNo);
        }

        #endregion For Invoice

        /// <summary>
        /// 获取指定商品的最后一次采购时间
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public DateTime? GetLastPoDate(int productSysNo)
        {
            return ObjectFactory<PurchaseOrderProcessor>.Instance.GetLastPoDate(productSysNo);
        }

        public int CreateBasketItemsForPrepare(List<BasketItemsInfo> itemsList)
        {
            return ObjectFactory<PurchaseOrderBasketProcessor>.Instance.CreateBasketItemsForPrepare(itemsList);
        }

        /// <summary>
        ///  获取供应商基本信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public VendorBasicInfo GetVendorBasicInfoBySysNo(int vendorSysNo)
        {
            return ObjectFactory<VendorProcessor>.Instance.LoadVendorBasicInfo(vendorSysNo);
        }

        #region For ExternalSYS
        /// <summary>
        /// 获取供应商代理信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public List<VendorAgentInfo> GetVendorAgentInfo(VendorInfo vendorInfo)
        {
            return ObjectFactory<VendorProcessor>.Instance.GetVendorAgentInfo(vendorInfo);
        }
        #endregion
    }
}
