using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IRefundDA
    {                
        List<int> GetWaitingSOForRefund();

        List<RefundItemInfo> GetItemsByRegisterSysNoList(List<int> sysNoList);

        int GetRefundSysNoByRegisterSysNo(int registerSysNo);

        int GetRefundCountBySOSysNoAndProductSysNo(int soSysNo, int ProductSysNo);
       
        int CreateSysNo();

        RefundInfo InsertMaster(RefundInfo entity);

        RefundItemInfo InsertItem(RefundItemInfo entity);

        string GetWarehouseNo(int refundSysNo);

        RefundInfo GetMasterBySysNo(int sysNo);

        void UpdateMaster(RefundInfo entity);

        void UpdateRefundPayTypeAndReason(int sysNo, int refundPayType, int refundReason);

        List<RefundItemInfo> GetItemsByRefundSysNo(int refundSysNo,string companyCode);

        /// <summary>
        /// 获取退款单的Item列表信息，并返回了商品的ID和名称
        /// </summary>
        /// <param name="refundSysNo"></param>
        /// <returns></returns>
        List<RefundItemInfo> GetItemsWithProductInfoByRefundSysNo(int refundSysNo);

        void UpdateMasterForCalc(RefundInfo entity);

        void UpdateItemForCalc(RefundItemInfo entity);

        void BatchUpdateRegisterRefundStatus(int refundSysNo, RMARefundStatus? refundStatus);

        List<RefundAmountForCheck> GetSOAmountForRefund(int refundSysNo,string companyCode);

        List<RefundAmountForCheck> GetROAmountForRefund(int refundSysNo,string companyCode);

        List<RegisterForRefund> GetRegistersForRefund(int refundSysNo);

        RefundItemInfo GetRefundItemCost(int sysNo);

        void UpdateItemForRefund(RefundItemInfo entity);

        List<RefundInfo> GetMasterByOrderSysNo(int orderSysNo);

        List<CodeNamePair> GetRefundReasons();

        /// <summary>
        /// 根据RegisterSysNo列表返回查询到的记录数
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        List<RMARegisterInfo> GetRegistersForCreate(List<int> sysNoList);

        /// <summary>
        /// 现金，礼品卡RO统计
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="rMARefundStatus"></param>
        /// <returns></returns>
        HistoryRefundAmount GetRefundCashAmtBySOSysNo(int soSysNo, RMARefundStatus rMARefundStatus);
        /// <summary>
        /// 现金，礼品卡ROBlance统计
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="rMARefundStatus"></param>
        /// <returns></returns>
        HistoryRefundAmount GetRO_BalanceCashAmtByOrgSOSysNo(int soSysNo, RMARefundStatus rMARefundStatus);

        HistoryRefundAmount GetRO_BalanceShipPriceAmtByOrgSOSysNo(int soSysNo, RefundBalanceStatus refundBalanceStatus, int productSysNo, string stockID);

        HistoryRefundAmount GetRO_BalanceShipPriceAmtByOrgSOSysNoAndOtherStockID(int soSysNo, RefundBalanceStatus refundBalanceStatus, int productSysNo, string stockID);

        HistoryRefundAmount GetRefundShipPriceBySOSysNoAndStockID(int soSysNo, RMARefundStatus rMARefundStatus, string stockID);

        HistoryRefundAmount GetRefundShipPriceBySOSysNoAndOtherStockID(int soSysNo, RMARefundStatus rMARefundStatus, string stockID);

        int GetAutoRMARefundCountBySOSysNo(int soSysNo);

        string GetMSMQAddressByStockID(string stockID);
    }
}
