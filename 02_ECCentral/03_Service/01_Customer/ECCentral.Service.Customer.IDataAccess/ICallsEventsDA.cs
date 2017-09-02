using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface ICallsEventsDA
    {
        /// <summary>
        /// 添加来电事件
        /// </summary>
        /// <param name="entity"></param>
        CallsEvents CreateEvents(CallsEvents entity);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        void UpdateEvents(CallsEvents entity);
        /// <summary>
        /// 添加跟进日志
        /// </summary>
        /// <param name="entity"></param>
        void CreateFollowUpLog(CallsEventsFollowUpLog entity);
   

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        CallsEvents Load(int SysNo);

        /// <summary>
        /// 根据外部引用信息找到来电事件 
        /// </summary>
        /// <param name="ReferenceType"></param>
        /// <param name="ReferenceSysNo"></param>
        /// <returns></returns>
        CallsEvents Load(CallingReferenceType ReferenceType, int ReferenceSysNo);

        void UpdateReference(CallsEvents entity);

        void CloseComplain(CallsEvents entity);

        List<CallsEventsFollowUpLog> GetLogsByEventsSysNo(int callEventsSysNo);
 


    }
}
