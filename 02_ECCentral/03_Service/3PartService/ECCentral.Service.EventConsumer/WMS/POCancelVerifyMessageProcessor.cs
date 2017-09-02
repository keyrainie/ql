using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.WMS;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;
using System.ServiceModel;

namespace ECCentral.Service.EventConsumer.WMS
{
    /// <summary>
    /// PO单取消审核 - 调用WMS提供的Hold Service
    /// </summary>  
    public class POCancelVerifyMessageProcessor : IConsumer<PurchaseOrderCancelVerifyMessage>
    {
        #region IConsumer<PurchaseOrderCancelVerifyMessage> Members

        public void HandleEvent(PurchaseOrderCancelVerifyMessage eventMessage)
        {
            eventMessage.CompanyCode = eventMessage.CompanyCode.Trim();           
            IPOService service = WCFAdapter<IPOService>.GetProxy();
            int result = service.MerchantHoldPORequest(eventMessage.PONumber, eventMessage.CompanyCode.Trim());
            if (result != 1)
            {
                var message = result == -1 ?
                    "Hold PO失败，请两分钟后重试，如多次重试后仍有问题，请与管理员联系。" :
                    "PO存在，但是已经开始处理，不能Hold";
                throw new ThirdPartBizException(message);
            }
        }
        #endregion


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
    
    [System.ServiceModel.ServiceContract]
    public interface IPOService
    {
        [System.ServiceModel.OperationContract]
        int MerchantHoldPORequest(string PONumber, string companyNumber);
    }
}
