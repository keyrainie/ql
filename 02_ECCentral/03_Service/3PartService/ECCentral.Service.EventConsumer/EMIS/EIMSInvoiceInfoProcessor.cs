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
    public class EIMSInvoiceInfoProcessor : IConsumer<EIMSInvoiceInfoMessage>
    {
        public void HandleEvent(EIMSInvoiceInfoMessage eventMessage)
        {
            if (eventMessage.IsPage)
            {
                //分页查询返点信息
                InvoiceCriteria msg = new InvoiceCriteria();
                msg.PM = eventMessage.PMSysNo.ToString();
                msg.VendorNumber = eventMessage.VendorSysNo.ToString();
                msg.CompanyCode = eventMessage.CompanyCode;
                msg.ShowLineNumber = eventMessage.PageSize;
                msg.CurrentPage = eventMessage.PageIndex + 1;
                msg.ReceiveType = eventMessage.ReceiveType;

                IEIMSInterfaceService service = WCFAdapter<IEIMSInterfaceService>.GetProxy();
                EIMSInvoiceResult result = service.GetEIMSInvoiceListForIPP(msg);

                eventMessage.ResultList = new List<ReturnPointMsg>();
                if (result.EIMSInvoiceList.Count > 0)
                {
                    result.EIMSInvoiceList.ForEach(a =>
                       eventMessage.ResultList.Add(new ReturnPointMsg()
                        {
                            SysNo = a.InvoiceNumber,
                            ReturnPointName = a.InvoiceName,
                            ReturnPoint = a.OriginalAmount,
                            RemnantReturnPoint = a.CurrentAmount
                        })
                        );
                }
                eventMessage.TotalCount = result.Records;
            }
            else
            {
                //根据sysno查询返点信息
                IEIMSInterfaceService service = WCFAdapter<IEIMSInterfaceService>.GetProxy();
                EIMSInvoiceInfo result = service.GetEIMSInvoiceInfo(eventMessage.InvoiceNumber, eventMessage.CompanyCode);
                eventMessage.Result = new ReturnPointMsg()
                {
                    SysNo = result.InvoiceNumber,
                    ReturnPointName = result.InvoiceName,
                    ReturnPoint = result.OriginalAmount,
                    RemnantReturnPoint = result.CurrentAmount
                };
            }
        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
