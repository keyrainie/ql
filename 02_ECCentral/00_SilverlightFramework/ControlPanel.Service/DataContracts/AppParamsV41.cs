using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newegg.Oversea.Framework.Contract;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    [DataContract]
    public class AppParamsV41 : DefaultDataContract
    {
        [DataMember]
        public AppParamsMsg Body { get; set; }

        public AppParamsV41()
        {
            this.Body = new AppParamsMsg();
        }
    }

    [DataContract]
    public class AppParamsMsg
    {
        [DataMember]
        public string GlobalRegion { get; set; }

        [DataMember]
        public string LocalRegion { get; set; }

        [DataMember]
        public string DefaultPage { get; set; }

        [DataMember]
        public string ServerComputerName { get; set; }

        [DataMember]
        public string ClientComputerName { get; set; }

        [DataMember]
        public string ServerIPAddress { get; set; }

        [DataMember]
        public string HostAddress { get; set; }

        [DataMember]
        public string FrameworkXapName { get; set; }

        [DataMember]
        public string ClientIPAddress { get; set; }
    }
}
