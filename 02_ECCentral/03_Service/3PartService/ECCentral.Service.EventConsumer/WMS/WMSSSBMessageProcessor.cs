using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Collections.Generic;
using ECCentral.Service.EventMessage;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer
{
    public class WMSSSBMessageProcessor : IConsumer<WMSSOActionRequestMessage>
    {
        private string GetAction(WMSActionType actionType)
        {
            string action = String.Empty;
            switch (actionType)
            {
                case WMSActionType.Hold:
                    action = "Hold";
                    break;
                case WMSActionType.UnHold:
                    action = "UnHold";
                    break;
                case WMSActionType.Abandon:
                    action = "Void";
                    break;
                case WMSActionType.CancelAuditHold:
                    action = "CAHold";
                    break;
                case WMSActionType.AbandonHold:
                    action = "VoidHold";
                    break;
                default:
                    break;
            }
            return action;
        }

        public void HandleEvent(WMSSOActionRequestMessage eventMessage)
        {
            if (eventMessage == null)
            {
                throw new ArgumentNullException("eventMessage");
            }

            XElement xmlHead = new XElement("MessageHead"
                        , new XElement("Namespace", "CN.OrderManagement.OPC.ActionRequest.Create.V1.Publish")
                        , new XElement("Version", "V3.1")
                        , new XElement("Action", GetAction(eventMessage.ActionType))
                        , new XElement("Type", null)
                        , new XElement("Sender", "Oversea MessageAgent")
                        , new XElement("Language", eventMessage.Language)
                        , new XElement("FromSystem", "Oversea")
                        , new XElement("ToSystem", "WMS")
                        , new XElement("GlobalBusinessType", "Listing")
                        , new XElement("TransactionCode", eventMessage.TransactionSysNo));
            XElement xmlBody = new XElement("Body"
                  , new XElement("SONumber", eventMessage.SOSysNo)
                  , new XElement("WarehouseNumber", eventMessage.StockSysNo)
                  , new XElement("CompanyCode", eventMessage.CompanyCode)
                  , new XElement("ActionUser", eventMessage.ActionUser)
                  , new XElement("ActionDate", eventMessage.ActionDate)
                  , new XElement("ActionReason", eventMessage.ActionReason));
            SSBSender.SendV2(ConstValue.SSB_From_OverseaIPPWMSOffline, ConstValue.SSB_To_PubSubServic, String.Format("DC01OverseaIPPWMSOffline_{0}", eventMessage.StockID),
               xmlHead.ToString() + xmlBody.ToString(), ConstValue.DataBaseName_SSB);

            //Logger.CreateLog(message.Message, MessageDirection.Out, MessageDestination.External, request.Body.SONumber.ToString(), MessageType.SO);
        }




        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

    class WarehouseInfo
    {
        public int WarehouseNumber { get; set; }

        public string StockID { get; set; }
    }
}