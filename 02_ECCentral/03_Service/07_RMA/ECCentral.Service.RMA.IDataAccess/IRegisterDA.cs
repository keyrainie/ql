using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;
using System.Data;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IRegisterDA
    {
        RMARegisterInfo LoadBySysNo(int sysNo);

        RMARegisterInfo LoadForEditBySysNo(int sysNo);

        List<RMARegisterInfo> LoadBySOSysNo(int soSysNo);

        List<RMARegisterInfo> QueryByRequestSysNo(int requestSysNo);

        List<RMARegisterInfo> GetRegistersBySysNoList(List<int> sysNoList);

        int CreateSysNo();

        bool Create(RMARegisterInfo register);

        bool InsertRequestItem(int requestSysNo, int registerSysNo);

        bool UpdateForRMAAuto(RMARegisterInfo register);

        bool UpdateBasicInfo(RMARegisterInfo register);

        bool UpdateCheckInfo(RMARegisterInfo register);

        bool UpdateResponseInfo(RMARegisterInfo register);

        bool UpdateRefundStatus(int sysNo, RMARefundStatus? refundStatus);

        bool UpdateReturnStatus(int sysNo, RMAReturnStatus? returnStatus);

        bool UpdateRevertStatus(RMARegisterInfo register);

        bool UpdateOutboundStatus(int sysNo, RMAOutBoundStatus? outboundStatus);

        bool Close(RMARegisterInfo register);

        bool CloseForVendorRefund(RMARegisterInfo register);

        bool ReOpen(RMARegisterInfo register);

        bool SetAbandon(int registerSysNo);

        void PurelyUpdate(RMARegisterInfo register);

        void UpdateInventory(int wareshouseSysNo, int productSysNo, bool isRecv, string companyCode);

        int? GetRegisterQty(int productSysNo, int soItemType, int soSysNo);

        bool CanWaitingRevert(int sysNo);

        bool CanWaitingRefund(int sysNo);

        bool CanCancelWaitingRefund(int sysNo);

        bool CanWaitingReturn(int sysNo);

        bool CanCancelWaitingReturn(int sysNo);

        bool CanCancelWaitingRevert(int sysNo);

        bool CanWaitingOutbound(int sysNo);

        bool CanCancelWaitingOutbound(int sysNo);

        bool CanReOpen(int sysNo);

        void UpdateMemo(RegisterBasicInfo registerEntity);

        /// <summary>
        /// RMA退款单退款成功后更新单件的RefundStatus
        /// </summary>
        /// <param name="entity"></param>
        void UpdateRegisterAfterRefund(RMARegisterInfo entity);

        RMAInventory GetRMAInventoryBy(int wareshouseSysNo, int productSysNo);

        void UpdateInventory(RMAInventory inventory);

        #region For PO Domain
        /// <summary>
        /// 根据单件号获取接收仓库:
        /// </summary>
        /// <param name="registerSysNo"></param>
        /// <returns></returns>
        string[] GetReceiveWarehouseByRegisterSysNo(int registerSysNo);
        #endregion

        DataRow LoadRegisterMemoBySysNo(int registerSysNo);

        bool SyncERP(int sysNo);
    }
}
