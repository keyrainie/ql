using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.IBizInteract
{
    public interface IRMABizInteract
    {
        /// <summary>
        /// 创建RMA跟进信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        InternalMemoInfo CreateRMATracking(InternalMemoInfo entity);

        /// <summary>
        /// 关闭RMA跟进信息
        /// </summary>
        /// <param name="entity"></param>
        void CloseRMATracking(InternalMemoInfo entity);

        /// <summary>
        /// 判断RMA申请单是否存在
        /// </summary>
        /// <param name="requestSysNo">RMA申请单编号</param>
        /// <returns></returns>
        bool IsRMARequestExists(int soSysNo);

        /// <summary>
        /// 是否物流拒收
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        int GetAutoRMARefundCountBySOSysNo(int soSysNo);

        /// <summary>
        /// 根据单据编号、供应商、日期等查询Refund信息
        /// </summary>
        /// <param name="refundSysNo">退款单编号</param>
        /// <param name="createFromDate">创建开始时间</param>
        /// <param name="createToDate">创建结束时间</param>
        /// <param name="webChannel">渠道信息</param>
        /// <returns></returns>
        List<RefundItemInfo> GetRefundItems(int refundSysNo, DateTime? createFromDate, DateTime? createToDate, WebChannel webChannel);

        /// <summary>
        /// 根据退款调整单系统编号取得退款调整单信息
        /// </summary>
        /// <param name="sysNo">退款调整单系统编号</param>
        /// <returns></returns>
        RefundBalanceInfo GetRefundBalanceBySysNo(int roBalanceSysNo);


        /// <summary>
        /// 根据退款单系统编号取得退款单信息
        /// </summary>
        /// <param name="roSysNo">退款单系统编号</param>
        /// <returns></returns>
        RefundInfo GetRefundBySysNo(int roSysNo);

        /// <summary>
        /// 更新退款单的退款支付方式和退款原因（供Invoice Domain调用）
        /// </summary>
        /// <param name="sysNo">退款单编号</param>
        /// <param name="refundPayType">退款类型</param>
        /// <param name="refundReason">退款原因</param>
        void UpdateRefundPayTypeAndReason(int sysNo, int refundPayType, int refundReason);

        #region For PO Domain 供应商退款单

        /// <summary>
        /// 根据SysNo获取相关的RMA Register信息
        /// </summary>
        /// <param name="registerNoList"></param>
        /// <returns></returns>
        List<RMARegisterInfo> GetRMARegisterList(List<int> registerNoList);

        /// <summary>
        /// 根据单件号获取接收仓库:
        /// </summary>
        /// <param name="registerNo"></param>
        /// <returns></returns>
        string[] GetReceiveWarehouseByRegisterSysNo(int registerNo);

        /// <summary>
        /// 通过多个单件号获取相关的送修单SysNo
        /// </summary>
        /// <param name="registerSysNos"></param>
        /// <returns></returns>
        List<int> GetOutBoundSysNoListByRegisterSysNo(string registerSysNoList);

        /// <summary>
        /// 更新送修单
        /// </summary>
        /// <param name="outBoundSysNos"></param>
        bool UpdateOutBound(string outBoundSysNoList);

        /// <summary>
        /// 关闭单件(供应商退款)
        /// </summary>
        /// <param name="dic"></param>
        void BatchCloseRegisterForVendorRefund(List<int> registerSysNoList);

        /// <summary>
        /// 扣减RMA库存中OnVendorQty数量
        /// </summary>
        /// <param name="list"></param>
        void BatchDeductOnVendorQty(List<VendorRefundInfo> list);        

        #endregion
    }
}
