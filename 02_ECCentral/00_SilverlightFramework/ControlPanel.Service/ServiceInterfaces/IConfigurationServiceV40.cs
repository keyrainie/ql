using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ExceptionHandler;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces
{
    [ServiceContract]
    public interface IConfigurationServiceV40
    {
        [OperationContract]
        List<ConfigKeyValueMsg> GetApplicationConfig();

        [OperationContract]
        ConfigKeyValueV40 CreateConfig(ConfigKeyValueV40 msg);

        [OperationContract]
        ConfigKeyValueV40 DeleteConfig(ConfigKeyValueV40 msg);

        [OperationContract]
        ConfigKeyValueV40 EditConfig(ConfigKeyValueV40 msg);

        [OperationContract]
        ECCentralMsg GetECCentralConfig();
    }

    [DataContract]
    public class ConfigKeyValueV40 : DefaultDataContract
    {
        [DataMember]
        public ConfigKeyValueMsg Body { get; set; }
    }

    [DataContract]
    public class ConfigKeyValueMsg
    {
        [DataMember]
        public int ConfigID { get; set; }

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public DateTime InDate { get; set; }

        [DataMember]
        public string EditUser { get; set; }

        [DataMember]
        public DateTime? EditDate { get; set; }
    }

    [DataContract]
    public class ECCentralMsg
    {
        [DataMember]
        public string ServiceURL { get; set; }

        [DataMember]
        public string ConfigPrefix { get; set; }
    }
}