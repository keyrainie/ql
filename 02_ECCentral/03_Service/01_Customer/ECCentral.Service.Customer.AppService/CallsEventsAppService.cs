using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CallsEventsAppService))]
    public class CallsEventsAppService
    {
        public virtual void Create(CallsEvents entity)
        {
            ObjectFactory<CallsEventsProcessor>.Instance.Create(entity);
        }

        public virtual void CallingToComplaint(BizEntity.SO.SOComplaintCotentInfo request)
        {
            //用SOComplaintCotentInfo 做DTO,借用字段 sysno
            ObjectFactory<CallsEventsProcessor>.Instance.TransComplaint(request.SysNo.Value, request);
        }

        public virtual void CallingToRMA(BizEntity.RMA.InternalMemoInfo request)
        {
            //用InternalMemoInfo 做DTO,借用字段 sysno
            ObjectFactory<CallsEventsProcessor>.Instance.TransRMA(request.SysNo.Value, request);
        }

        public virtual CallsEvents Load(int SysNo)
        {
            return ObjectFactory<CallsEventsProcessor>.Instance.Load(SysNo);
        }
        public virtual string GetReasonCodePath(int reasonCodeSysNo, string companyCode)
        {
            return ExternalDomainBroker.GetReasonCodePath(reasonCodeSysNo, companyCode);
        }

        public virtual void CreateCallsEventsFollowUpLog(CallsEventsFollowUpLog request)
        {
            ObjectFactory<CallsEventsProcessor>.Instance.CreateCallsEventsFollowUpLog(request);
        }
    }
}
