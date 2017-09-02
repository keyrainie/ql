using System.Collections.Generic;
using System.Runtime.Serialization;

using Newegg.Oversea.Framework.Contract;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    [DataContract]
    public class AppVersionV40 : DefaultDataContract
    {
        [DataMember]
        public List<XapVersion> Body { get; set; }

        [DataMember]
        public string ComputerName { get; set; }

        public AppVersionV40()
        {
            Body = new List<XapVersion>();
        }
    }

    [DataContract]
    public class XapVersionV40 : DefaultDataContract
    {
        [DataMember]
        public XapVersion Body { get; set; }
    }

    [DataContract]
    public class XapVersion
    {
        [DataMember]
        public string XapName { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string PreVersion { get; set; }

        [DataMember]
        public string Title { get; set; }
        
        [DataMember]
        public string PublishDate { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string UpdateLevel { get; set; }
    }
}
