using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.WMS;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventConsumer.WMS
{
    /// <summary>
    /// PO单等待入库操作，向WMS发送SSB:
    /// </summary>
    public class POWaitingInStockMessageProcessor : IConsumer<PurchaseOrderWaitingInStockMessage>
    {
        #region IConsumer<PurchaseOrderWaitingInStockMessage> Members

        public void HandleEvent(PurchaseOrderWaitingInStockMessage eventMessage)
        {
            string getFromService = AppSettingManager.GetSetting("PO", "Po_WaitingInStock_SSB_FromService");
            string getToService = AppSettingManager.GetSetting("PO", "Po_WaitingInStock_SSB_ToService");
            string getArticleCategory = AppSettingManager.GetSetting("PO", "Po_WaitingInStock_SSB_ArticleCategory");
            string getArticleType1 = AppSettingManager.GetSetting("PO", "Po_WaitingInStock_SSB_ArticleType1");
            string getArticleType2 = AppSettingManager.GetSetting("PO", "Po_WaitingInStock_SSB_ArticleType2");
            string getDBName = AppSettingManager.GetSetting("PO", "Po_WaitingInStock_SSB_DataBaseName");
            string getVersion = AppSettingManager.GetSetting("PO", "Po_WaitingInStock_SSB_MessageHead_Version");

            PurchaseOrderWaitingInStockSendMsg msg = new PurchaseOrderWaitingInStockSendMsg()
            {
                MessageHead = new PurchaseOrderWaitingInStockMessageHead()
                {
                    MessageType = eventMessage.SendType,
                    CompanyCode = eventMessage.CompanyCode.Trim(),
                    Version = getVersion,
                    ReferenceNumber = eventMessage.PONumber
                },
                Body = new PurchaseOrderWaitingInStockMessageBody()
                {
                    message = eventMessage
                }
            };
            //发送SSB消息:
            msg.Body.message.CompanyCode = msg.Body.message.CompanyCode.Trim();
            SSBSender.SendV3<PurchaseOrderWaitingInStockSendMsg>(getFromService, getToService, getArticleCategory, getArticleType1, getArticleType2, msg, getDBName, true);
        }

        #endregion


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

    [Serializable]
    public class PurchaseOrderWaitingInStockSendMsg : IEventMessage
    {
        [XmlElement("MessageHead")]
        public PurchaseOrderWaitingInStockMessageHead MessageHead { get; set; }

        [XmlElement("Body")]
        public PurchaseOrderWaitingInStockMessageBody Body { get; set; }


        public string Subject
        {
            get { return "PurchaseOrderWaitingInStockSendMsg"; }
        }
    }

    [Serializable]
    public class PurchaseOrderWaitingInStockMessageHead
    {
        [XmlElement("MessageType")]
        public string MessageType { get; set; }
        [XmlElement("Version")]
        public string Version { get; set; }
        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }
        [XmlElement("ReferenceNumber")]
        public string ReferenceNumber { get; set; }

    }

    [Serializable]
    public class PurchaseOrderWaitingInStockMessageBody
    {
        [XmlElement("POMaster")]
        public PurchaseOrderWaitingInStockMessage message { get; set; }
    }

}
