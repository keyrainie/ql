using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage
{

    [Serializable]
    public class AbandonUnicomMessage : IEventMessage
    {
        [XmlElement("ordersn")]
        public string OrderNumber { get; set; }

        [XmlElement("phoneno")]
        public string PhoneNumber { get; set; }

        [XmlElement("remark")]
        public string Remark { get; set; }

        public string Subject
        {
            get { return "AbandonUnicomMessage"; }
        }
    }

    public class AbandonUnicomRequestMessage
    {
        [XmlElement("seqno")]
        public string SeqNo { get; set; }

        [XmlElement("servername")]
        public string ServerName { get; set; }

        [XmlElement("acct")]
        public string AcctountID { get; set; }

        [XmlElement("paswd")]
        public string Password { get; set; }

        [XmlElement("agentkey")]
        public string AgentKey { get; set; }

        [XmlElement("param")]
        public AbandonUnicomMessage Param { get; set; }
    }


    [XmlRoot("JSUCRes")]
    public class UnicomResponseMessage
    {
        [XmlElement("seqno")]
        public string SeqNo { get; set; }

        [XmlElement("servername")]
        public string ServerName { get; set; }

        [XmlElement("msginfo")]
        public MsgInfo MsgInfo { get; set; }

        [XmlElement("phoneno")]
        public string PhoneNumber { get; set; }

        [XmlElement("ordersn")]
        public string OrderNumber { get; set; }
    }


    public class MsgInfo
    {
        [XmlElement("msgcode")]
        public string MsgCode { get; set; }

        [XmlElement("msgdesc")]
        public string MsgDesc { get; set; }
    }
}