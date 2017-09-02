using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace IPP.MessageAgent.SendReceive.JobV31.Configuration
{
    [XmlRoot("ssbProcessConfig", Namespace = "http://IPP/ssb")]
    public class SSBProcessConfig
    {
        [XmlElement("ssbChannel")]
        public SSBChannelCollection SSBChannels { get; set; }
    }

    public class SSBChannel
    {
        public SSBChannel()
        {
            this.Version = SSBVersion.V2;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("dataCommand")]
        public string DataCommand { get; set; }

        [XmlAttribute("version")]
        public SSBVersion Version { get; set; }

        [XmlElement("actionTypeXpath")]
        public string ActionTypeXpath { get; set; }

        [XmlElement("ssbProcess")]
        public ProcesserCollection Processers { get; set; }

        public SSBProcesser GetProcesser(string actionType)
        {
            if (string.IsNullOrEmpty(actionType) || !this.Processers.Contains(actionType))
            {
                return null;
            }
            return Processers[actionType];
        }
    }

    public class SSBChannelCollection : KeyedCollection<string, SSBChannel>
    {
        protected override string GetKeyForItem(SSBChannel item)
        {
            return item.Name;
        }
    }

    public class SSBProcesser
    {
        public SSBProcesser()
        {
            this.CallType = CallType.WCF;
        }

        [XmlAttribute("actionType")]
        public string ActionType { get; set; }

        [XmlAttribute("callType")]
        public CallType CallType { get; set; }

        [XmlElement("processService")]
        public string ProcessService { get; set; }

        [XmlElement("referenceKeyXpath")]
        public string ReferenceKeyXPath { get; set; }
    }

    public class ProcesserCollection : KeyedCollection<string, SSBProcesser>
    {
        protected override string GetKeyForItem(SSBProcesser item)
        {
            return item.ActionType;
        }
    }

    public enum CallType
    {
        SP, WCF
    }

    public enum SSBVersion
    {
        V1, V2, V3,Customer
    }
}
