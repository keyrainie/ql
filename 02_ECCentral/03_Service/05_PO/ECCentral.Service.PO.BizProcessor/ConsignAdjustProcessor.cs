using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.BizProcessor
{
    [VersionExport(typeof(ConsignAdjustProcessor))]
    public  class ConsignAdjustProcessor
    {

        public ConsignAdjustInfo MaintainStatus(ConsignAdjustInfo info)
        {
            var old= ObjectFactory<IConsignAdjustDA>.Instance.LoadInfo(info.SysNo.Value);
            if (info.Status == ConsignAdjustStatus.Abandon && old.Status != ConsignAdjustStatus.WaitAudit)
            {
                throw new BizException("记录状态不为待审核，作废失败");
            }
            if (info.Status == ConsignAdjustStatus.Audited && old.Status != ConsignAdjustStatus.WaitAudit)
            {
                 throw new BizException("记录状态不为待审核，审核失败");
            }
            if (info.Status == ConsignAdjustStatus.Audited)
            {
                ExternalDomainBroker.CreatePayItem(new PayItemInfo()
                {
                    OrderSysNo = old.SysNo.Value,
                    PayAmt = old.TotalAmt,
                    OrderType = PayableOrderType.ConsignAdjust,
                    PayStyle = PayItemStyle.Normal,
                    CompanyCode= "8601"
                });
            }           
            return ObjectFactory<IConsignAdjustDA>.Instance.UpdateStatus(info);
        }

        public ConsignAdjustInfo Create(ConsignAdjustInfo info)
        {
            info.Status = ConsignAdjustStatus.WaitAudit;
            var settleInfo = ObjectFactory<ConsignSettlementProcessor>.Instance.LoadConsignSettlementInfo(info.SettleSysNo.Value);

            if (null==settleInfo)
            {
                throw new BizException("无效的结算单");
            }

            if (settleInfo.VendorInfo.SysNo!=info.VenderSysNo)
            {
                throw new BizException("选择供应商与结算单供应商不一致");
            }
            if (settleInfo.PMInfo.SysNo!= info.PMSysNo)
            {
                throw new BizException("选择产品经理与结算单产品经理不一致");
            }
            if (settleInfo.Status!=SettleStatus.SettlePassed)
            {
                  throw new BizException("代销单不是已结算的单据");
            }
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                info = ObjectFactory<IConsignAdjustDA>.Instance.Create(info);
                if (info.SysNo.HasValue)
                {
                    info.ItemList.ForEach((item) =>
                    {
                        item.ConsignAdjustSysNo = info.SysNo.Value;
                        ObjectFactory<IConsignAdjustDA>.Instance.CreateConsignAdjustItem(item);
                    });
                }
                scope.Complete();
                return info;
            }
        }
    }
}
