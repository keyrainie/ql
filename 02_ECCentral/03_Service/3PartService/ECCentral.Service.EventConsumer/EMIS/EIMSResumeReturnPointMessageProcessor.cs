using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.EIMS;
using ECCentral.Service.Utility;
using Newegg.EIMSCN.ServiceInterfaces.DataContracts;
using Newegg.EIMSCN.ServiceInterfaces.ServiceContracts;

namespace ECCentral.Service.EventConsumer.EMIS
{
    public class EIMSResumeReturnPointMessageProcessor : IConsumer<EIMSResumeReturnPointMessage>
    {
        #region IConsumer<EIMSResumeReturnPointMessage> Members

        public void HandleEvent(EIMSResumeReturnPointMessage eventMessage)
        {
            //是否撤销扣减积分
            bool isBack = eventMessage.IsComeBackPoint;

            EIMSMessage<IppAttachInfo> eims = new EIMSMessage<IppAttachInfo>();
            IppAttachInfo ippAttachInfo = new IppAttachInfo();
            ippAttachInfo.InvoiceNumber = eventMessage.PM_ReturnPointSysNo;
            ippAttachInfo.IppNumber = eventMessage.VendorSettleSysNo;

            ippAttachInfo.UseInvoiceAmount = isBack ? 0 - eventMessage.UsingReturnPoint : eventMessage.UsingReturnPoint;

            ippAttachInfo.UseTime = DateTime.Now;
            ippAttachInfo.PostTime = DateTime.Now;

            ippAttachInfo.IppStatus = isBack ? "2" : "3";

            ippAttachInfo.IppType = "002";
            ippAttachInfo.ExtendedInfo = eventMessage.SettleID;
            ippAttachInfo.AttacheUser = eventMessage.AuditUserSysNo.ToString();
            eims.Body = ippAttachInfo;
            EIMSMessageHeader eimsHeader = new EIMSMessageHeader();
            eimsHeader.CompanyCode = "8601";
            eimsHeader.UserID = eventMessage.AuditUserSysNo.ToString();
            eims.Header = eimsHeader;

            IEIMSInterfaceService service = WCFAdapter<IEIMSInterfaceService>.GetProxy();
            EIMSMessageResult result = service.PostEIMSReceivedAmountFromIPP(eims);

            eventMessage.Error = result.Error;
            eventMessage.IsSucceed = result.IsSucceed;
        }

        #endregion


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
