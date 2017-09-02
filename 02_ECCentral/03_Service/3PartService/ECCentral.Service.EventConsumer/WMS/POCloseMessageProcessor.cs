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
    /// PO单终止入库操作，向WMS发送SSB消息:
    /// </summary>
    public class POCloseMessageProcessor : IConsumer<PurchaseOrderCloseMessage>
    {
        #region IConsumer<PurchaseOrderCloseMessage> Members

        public void HandleEvent(PurchaseOrderCloseMessage eventMessage)
        {
            string getFromService = AppSettingManager.GetSetting("PO", "Po_Close_SSB_FromService");
            string getToService = AppSettingManager.GetSetting("PO", "Po_Close_SSB_ToService");
            string getArticleCategory = AppSettingManager.GetSetting("PO", "Po_Close_SSB_ArticleCategory");
            string getArticleType1 = AppSettingManager.GetSetting("PO", "Po_Close_SSB_ArticleType1");
            string getArticleType2 = AppSettingManager.GetSetting("PO", "Po_Close_SSB_ArticleType2");
            string getDBName = AppSettingManager.GetSetting("PO", "Po_Close_SSB_DataBaseName");

            PurchaseOrderCloseSendMsg msg = new PurchaseOrderCloseSendMsg()
            {
                MessageHead = new PurchaseOrderCloseMessageHead()
                {
                    Version = AppSettingManager.GetSetting("PO", "Po_Close_SSB_MessageHead_Version"),
                    MessageType = AppSettingManager.GetSetting("PO", "Po_Close_SSB_MessageHead_Type"),
                    CompanyCode = eventMessage.CompanyCode.Trim(),
                    ReferenceNumber = eventMessage.PONumber
                },
                message = eventMessage
            };
            //发送SSB消息:
            msg.message.CompanyCode = msg.message.CompanyCode.Trim();
            SSBSender.SendV3<PurchaseOrderCloseSendMsg>(getFromService, getToService, getArticleCategory, getArticleType1, getArticleType2, msg, getDBName, true);
        }

        #endregion


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

    [Serializable]
    public class PurchaseOrderCloseSendMsg
    {
        [XmlElement("MessageHead")]
        public PurchaseOrderCloseMessageHead MessageHead { get; set; }

        [XmlElement("Body")]
        public PurchaseOrderCloseMessage message { get; set; }

    }

    [Serializable]
    public class PurchaseOrderCloseMessageHead
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
}

