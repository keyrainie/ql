using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(IRMABizInteract))]
    public class BizInteractAppService : IRMABizInteract
    {
        #region RMA跟进日志
        public virtual InternalMemoInfo CreateRMATracking(InternalMemoInfo entity)
        {
            return ObjectFactory<RMATrackingProcessor>.Instance.CreateRMATracking(entity);
        }

        public virtual void CloseRMATracking(InternalMemoInfo entity)
        {
            ObjectFactory<RMATrackingProcessor>.Instance.CloseRMATracking(entity);
        }

        #endregion

        public virtual bool IsRMARequestExists(int soSysNo)
        {
            return ObjectFactory<RequestProcessor>.Instance.IsRMARequestExists(soSysNo);
        }

        public virtual int GetAutoRMARefundCountBySOSysNo(int soSysNo)
        {
            return ObjectFactory<RefundProcessor>.Instance.GetAutoRMARefundCountBySOSysNo(soSysNo);
        }

        public List<BizEntity.RMA.RefundItemInfo> GetRefundItems(int refundSysNo, DateTime? createFromDate, DateTime? createToDate, BizEntity.Common.WebChannel webChannel)
        {
            throw new NotImplementedException();
        }

        public virtual RefundBalanceInfo GetRefundBalanceBySysNo(int RefundBalanceSysNo)
        {
            return ObjectFactory<RefundBalanceProcessor>.Instance.GetRefundBalanceBySysNo(RefundBalanceSysNo);
        }

        public virtual RefundInfo GetRefundBySysNo(int RefundSysNo)
        {
            return ObjectFactory<RefundProcessor>.Instance.LoadBySysNo(RefundSysNo);
        }

        #region For PO Domain.供应商退款单

        public List<RMARegisterInfo> GetRMARegisterList(List<int> registerNoList)
        {
            return ObjectFactory<RegisterProcessor>.Instance.LoadBySysNoList(registerNoList);
        }

        public string[] GetReceiveWarehouseByRegisterSysNo(int registerNo)
        {
            return ObjectFactory<RegisterProcessor>.Instance.GetReceiveWarehouseByRegisterSysNo(registerNo);
        }

        public virtual List<int> GetOutBoundSysNoListByRegisterSysNo(string registerSysNoList)
        {
            return ObjectFactory<OutBoundProcessor>.Instance.GetOutBoundSysNoListByRegisterSysNoList(registerSysNoList);
        }

        public bool UpdateOutBound(string outBoundSysNoList)
        {
            return ObjectFactory<OutBoundProcessor>.Instance.UpdateOutBounds(outBoundSysNoList);
        }

        public void BatchCloseRegisterForVendorRefund(List<int> registerSysNoList)
        {
            ObjectFactory<VendorRefundProcessor>.Instance.BatchCloseRegisterForVendorRefund(registerSysNoList);
        }

        public void BatchDeductOnVendorQty(List<BizEntity.PO.VendorRefundInfo> list)
        {
            foreach(var item in list)
            {
                //扣减数量为1个，Victor.W.Ye说之前调用的时候是写死的
                ObjectFactory<VendorRefundProcessor>.Instance.DeductOnVendorQty(item.RegisterSysNo, item.WarehouseSysNo, item.ProductSysNo, 1);
            }
        }

        public void UpdateRefundPayTypeAndReason(int sysNo, int refundPayType, int refundReason)
        {
            ObjectFactory<RefundProcessor>.Instance.UpdateRefundPayTypeAndReason(sysNo, refundPayType, refundReason);
        }

        #endregion
    }
}
