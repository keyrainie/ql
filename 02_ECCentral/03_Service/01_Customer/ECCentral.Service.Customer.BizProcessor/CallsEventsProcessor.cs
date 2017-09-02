using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;
using System.Transactions;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(CallsEventsProcessor))]
    public class CallsEventsProcessor
    {
        /// <summary>
        /// 创建顾客来电
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void Create(CallsEvents entity)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ObjectFactory<ICallsEventsDA>.Instance.CreateEvents(entity);
                //跟进日志
                foreach (var item in entity.LogList)
                {
                    item.CallsEventsSysNo = entity.SysNo;
                    ObjectFactory<ICallsEventsDA>.Instance.CreateFollowUpLog(item);
                }
                //根据状态做相应的更新
                entity = ObjectFactory<CallsEventsProcessor>.Instance.Load(entity.SysNo.Value);
                if (entity.Status == CallsEventsStatus.Replied)
                {
                    entity.LastEditDate = null;
                    entity.CloseDate = null;
                    ObjectFactory<ICallsEventsDA>.Instance.UpdateEvents(entity);
                }
                else if (entity.Status == CallsEventsStatus.Abandoned)
                {
                    entity.LastEditDate = DateTime.Now;
                    entity.CloseDate = null;
                }
                else
                    this.CloseCallsEvents(entity);
                scope.Complete();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateReference(CallsEvents entity)
        {
            ObjectFactory<ICallsEventsDA>.Instance.UpdateReference(entity);
        }

        public virtual void CloseCallsEvents(CallsEvents entity)
        {
            int usedHours = 0;
            List<DateTime> holidays = ExternalDomainBroker.GetHoliday(AppSettingManager.GetSetting("Customer", "CSHoliday"), entity.CompanyCode);
            int holidayHours = holidays.Where(p => p > entity.CreateDate.Value && p < DateTime.Now).Count() * 24;
            usedHours = (DateTime.Now - entity.CreateDate.Value).Hours - holidayHours;
            if (usedHours < 0)
                usedHours = 0;
            entity.Status = CallsEventsStatus.Handled;
            entity.CloseDate = DateTime.Now;
            entity.LastEditDate = DateTime.Now;
            entity.UsedHours = usedHours;
            ObjectFactory<ICallsEventsDA>.Instance.UpdateEvents(entity);
        }

        /// <summary>
        /// 加载顾客
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public virtual CallsEvents Load(int SysNo)
        {
            return ObjectFactory<ICallsEventsDA>.Instance.Load(SysNo); ;
        }

        /// <summary>
        /// 转换RMA
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void TransRMA(int callingSysNO, InternalMemoInfo request)
        {
            CallsEvents entity = ObjectFactory<CallsEventsProcessor>.Instance.Load(callingSysNO);
            entity.ReferenceType = CallingReferenceType.RMA;
            using (TransactionScope scope = new TransactionScope())
            {
                entity.ReferenceSysNo = ExternalDomainBroker.CreateRMATracking(request).SysNo;
                ObjectFactory<CallsEventsProcessor>.Instance.UpdateReference(entity);
                ObjectFactory<CallsEventsProcessor>.Instance.CloseCallsEvents(entity);
                scope.Complete();
            }
        }

        /// <summary>
        /// 转换投诉
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void TransComplaint(int callingSysNO, SOComplaintCotentInfo complaintInfo)
        {
            CallsEvents entity = ObjectFactory<CallsEventsProcessor>.Instance.Load(callingSysNO);
            entity.ReferenceType = CallingReferenceType.Complain;
            using (TransactionScope scope = new TransactionScope())
            {
                complaintInfo.CompanyCode = entity.CompanyCode;
                entity.ReferenceSysNo = ExternalDomainBroker.AddComplain(complaintInfo).SysNo;
                ObjectFactory<CallsEventsProcessor>.Instance.UpdateReference(entity);
                ObjectFactory<CallsEventsProcessor>.Instance.CloseCallsEvents(entity);
                scope.Complete();
            }

        }

        /// <summary>
        /// 关闭投诉
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void CloseComplain(CallsEvents entity)
        {
            ObjectFactory<ICallsEventsDA>.Instance.CloseComplain(entity);
        }

        public virtual List<CallsEventsFollowUpLog> GetLogsByEventsSysNo(int sysno)
        {
            return ObjectFactory<ICallsEventsDA>.Instance.GetLogsByEventsSysNo(sysno);

        }


        public virtual void CreateCallsEventsFollowUpLog(CallsEventsFollowUpLog request)
        {
            //保存来电事件和跟进日志的状态一致
            CallsEvents entity = ObjectFactory<CallsEventsProcessor>.Instance.Load(request.CallsEventsSysNo.Value);
            using (TransactionScope scope = new TransactionScope())
            {
                if (request.Status == CallsEventsStatus.Replied)
                {
                    entity.LastEditDate = DateTime.Now;
                    entity.CloseDate = null;
                    entity.Status = CallsEventsStatus.Replied;
                    ObjectFactory<ICallsEventsDA>.Instance.UpdateEvents(entity);
                }
                else if (request.Status == CallsEventsStatus.Abandoned)
                {
                    entity.LastEditDate = DateTime.Now;
                    entity.CloseDate = null;
                    entity.Status = CallsEventsStatus.Abandoned;
                    ObjectFactory<ICallsEventsDA>.Instance.UpdateEvents(entity);
                }
                else
                {
                    entity.Status = CallsEventsStatus.Handled;
                    this.CloseCallsEvents(entity);
                }
                ObjectFactory<ICallsEventsDA>.Instance.CreateFollowUpLog(request);
                scope.Complete();
            }
        }

        public virtual void CloseCallsEvents(CallingReferenceType ReferenceType, int ReferenceSysNo, string note)
        {
            CallsEvents entity = ObjectFactory<ICallsEventsDA>.Instance.Load(ReferenceType, ReferenceSysNo);
            this.CloseCallsEvents(entity);
        }
    }
}
