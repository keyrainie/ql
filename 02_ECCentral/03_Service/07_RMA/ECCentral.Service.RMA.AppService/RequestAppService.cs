using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(RequestAppService))]
    public class RequestAppService
    {      
        public virtual RMARequestInfo Create(RMARequestInfo entity)
        {
            return ObjectFactory<RequestProcessor>.Instance.Create(entity);
        }

        public virtual void Update(RMARequestInfo entity)
        {
            ObjectFactory<RequestProcessor>.Instance.Update(entity);
        }

        public virtual RMARequestInfo LoadWithRegistersBySysNo(int sysNo, out CustomerInfo customer, 
            out SOBaseInfo soBaseInfo, 
            out DeliveryInfo deliveryInfo, 
            out string deliveryUserName,
            out string businessModel)
        {
            var request = ObjectFactory<RequestProcessor>.Instance.LoadWithRegistersBySysNo(sysNo);

            businessModel = ObjectFactory<RequestProcessor>.Instance.GetBusinessModel(request);

            //配送信息
            deliveryUserName = string.Empty;
            deliveryInfo = ExternalDomainBroker.GetDeliveryInfo(DeliveryType.RMARequest, sysNo, DeliveryStatus.OK);
            if (deliveryInfo != null && deliveryInfo.DeliveryUserSysNo.HasValue)
            {
                deliveryUserName = ExternalDomainBroker.GetUserInfoBySysNo(deliveryInfo.DeliveryUserSysNo.Value);            
            }
            if (request.CreateUserSysNo != null)
            {
                request.CreateUserName = ExternalDomainBroker.GetUserInfoBySysNo(request.CreateUserSysNo.Value);
            }
            if (request.ReceiveUserSysNo.HasValue)
            {
                request.ReceiveUserName = ExternalDomainBroker.GetUserInfoBySysNo(request.ReceiveUserSysNo.Value);
            }
            customer = ExternalDomainBroker.GetCustomerInfo(request.CustomerSysNo.Value);

            soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(request.SOSysNo.Value);            

            return request;
        }

        public virtual RMARequestInfo LoadByRegisterSysNo(int sysNo)
        {
            return ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(sysNo);
        }

        public virtual RMARequestInfo Receive(RMARequestInfo entity)
        {
            return ObjectFactory<RequestProcessor>.Instance.Receive(entity);
        }

        public virtual void CancelReceive(int sysNo)
        {
            ObjectFactory<RequestProcessor>.Instance.CancelReceive(sysNo);
        }

        public virtual void Close(RMARequestInfo request)
        {
            ObjectFactory<RequestProcessor>.Instance.Close(request.SysNo.Value);
        }

        public virtual void PrintLabels(int sysNo)
        {
            ObjectFactory<RequestProcessor>.Instance.PrintLables(sysNo);
        }

        public virtual void Abandon(int sysNo)
        {
            ObjectFactory<RequestProcessor>.Instance.Abandon(sysNo);
        }

        public virtual RMARequestInfo Adjust(int sysNo)
        {
            return ObjectFactory<RequestProcessor>.Instance.Adjust(sysNo);
        }

        public virtual RMARequestInfo Refused(int sysNo)
        {
            return ObjectFactory<RequestProcessor>.Instance.Refused(sysNo);
        }
    }
}
