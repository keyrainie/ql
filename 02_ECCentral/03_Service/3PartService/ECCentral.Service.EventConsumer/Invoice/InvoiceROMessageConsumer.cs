using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;
using System.Xml.Linq;
using System.Messaging;

namespace ECCentral.Service.EventConsumer.Invoice
{
    public class InvoiceROMessageConsumer : IConsumer<InvoiceROMessage>
    {
        public void HandleEvent(InvoiceROMessage eventMessage)
        {
            CheckMessage(eventMessage);
            //TODO:记录到数据库
            //Logger.CreateLog(
            //     SerializationUtility.XmlSerialize(eventMessage)
            //     , InvoiceMsgDirection.In
            //     , InvoiceMsgDestination.External
            //     , eventMessage.Body[0].OrderSysNo
            //     , InvoiceMsgType.Invoice);

            try
            {
                SendMessage(eventMessage);
            }
            catch (Exception e)
            {
                //TODO:记录失败信息
                //RetryProcessHelper.WriteLog<InvoiceV31>(eventMessage
                //    , SerializationUtility.XmlSerialize(eventMessage)
                //    , e.ToString()
                //    , Const.InvoiceMessageFailedLogTag);
            }
        }

        private void SendMessage(InvoiceROMessage eventMessage)
        {
            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-16", "yes")
               , new XElement("InvoiceNodes"
                   , from invoice in eventMessage.Body
                     select new XElement("InvoiceNode"
                       , new XElement("OrderSysNo", invoice.OrderSysNo)
                       , new XElement("OrderType", invoice.OrderType)
                       , new XElement("WarehouseNumber", invoice.StockID)
                       , new XElement("ComputerNo", 0)
                       )
                   )
               );
            string xmlMessage = xml.ToString();
            MessageQueue myQueue = new MessageQueue(eventMessage.Body[0].MSMQAddress);
            myQueue.Send(xmlMessage);
            //TODO:记录到数据库
            //Logger.CreateLog(xmlMessage, InvoiceMsgDirection.Out, InvoiceMsgDestination.External, eventMessage.Body[0].OrderSysNo, InvoiceMsgType.Invoice);


        }

        private void CheckMessage(InvoiceROMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (message.Body == null || message.Body.Count == 0)
            {
                throw new ArgumentException("message.Body can not be null or empty.");
            }
            if (StringUtility.IsNullOrEmpty(message.Body[0].MSMQAddress))
            {
                throw new ApplicationException("MSMQ address not found. WarehouseNumber: [" + message.Body[0].StockID + "]");
            }
        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
