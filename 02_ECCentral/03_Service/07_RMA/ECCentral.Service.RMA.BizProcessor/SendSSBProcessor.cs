using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.RMA.IDataAccess;
using System.Threading;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.EventMessage;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(SendSSBProcessor))]
    public class SendSSBProcessor
    {
        public void SendROMessage(int orderSysNo, string stockID, string companyCode)
        {
            InvoiceROMessage msg = new InvoiceROMessage();
            msg.Header = GenerateMessageHeader(companyCode);
            string msmqAddress = GetMSMQAddress(stockID);
            msg.Body = new InvoiceCollection(){ 
                new InvoiceItem()
                {
                    OrderSysNo = orderSysNo.ToString(),
                    StockID = stockID,
                    OrderType = InvoiceMsgOrderType.RO,
                    MSMQAddress = msmqAddress
                }
            };
            EventPublisher.Publish<InvoiceROMessage>(msg);
        }

        public void SendADJUSTMessage(int orderSysNo, string stockID, string companyCode)
        {
            InvoiceADJUSTMessage msg = new InvoiceADJUSTMessage();
            msg.Header = GenerateMessageHeader(companyCode);
            string msmqAddress = GetMSMQAddress(stockID);
            msg.Body = new InvoiceCollection(){ 
                new InvoiceItem()
                {
                    OrderSysNo = orderSysNo.ToString(),
                    StockID = stockID,
                    OrderType = InvoiceMsgOrderType.ADJUST,
                    MSMQAddress = msmqAddress
                }
            };
            EventPublisher.Publish<InvoiceADJUSTMessage>(msg);

        }

        private MessageHeader GenerateMessageHeader(string companyCode)
        {
            UserInfo userInfo = ExternalDomainBroker.GetUserInfo(ServiceContext.Current.UserSysNo);
            return new MessageHeader()
            {
                Language = Thread.CurrentThread.CurrentCulture.Name,
                Sender = "CN.ServiceManagement.Inbound.IPP01",
                CompanyCode = companyCode,
                StoreCompanyCode = companyCode,
                OperationUser = new OperationUser()
                {
                    FullName = userInfo.UserDisplayName,
                    LogUserName = userInfo.UserID,
                    SourceDirectoryKey = RMAConst.AutoRMASourceDirectoryKey,
                    CompanyCode = companyCode
                }
            };
        }

        /// <summary>
        /// 根据仓库获取MessageQueue发送路径
        /// </summary>
        /// <param name="stockID"></param>
        /// <returns></returns>
        private string GetMSMQAddress(string stockID)
        {
            return ObjectFactory<IRefundDA>.Instance.GetMSMQAddressByStockID(stockID);
        }


    }
}
