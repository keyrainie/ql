using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(RMATrackingAppService))]
    public class RMATrackingAppService
    {
        /// <summary>
        /// 派发RMA跟进单
        /// </summary>
        /// <param name="msg"></param>
        public virtual void DispatchRMATracking(List<int> sysNoList, int handlerSysNo)
        {
            ObjectFactory<RMATrackingProcessor>.Instance.DispatchRMATracking(sysNoList, handlerSysNo);
        }
        /// <summary>
        /// 取消派发RMA跟进单
        /// </summary>
        /// <param name="msg"></param>
        public virtual void CancelDispatchRMATracking(List<int> sysNoList)
        {
            ObjectFactory<RMATrackingProcessor>.Instance.CancelDispatchRMATracking(sysNoList);
        }
        /// <summary>
        /// 关闭RMA跟进单
        /// </summary>
        /// <param name="msg"></param>
        public virtual void CloseRMATracking(InternalMemoInfo msg)
        {
            ObjectFactory<RMATrackingProcessor>.Instance.CloseRMATracking(msg);
        }
        /// <summary>
        /// 创建RMA跟进日志
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual InternalMemoInfo CreateRMATracking(InternalMemoInfo msg)
        {
            return ObjectFactory<RMATrackingProcessor>.Instance.CreateRMATracking(msg);
        }

        public virtual string GetReasonCodePath(int reasonCodeSysNo, string companyCode)
        {
            return ExternalDomainBroker.GetReasonCodePath(reasonCodeSysNo, companyCode);
        }

    }
}
