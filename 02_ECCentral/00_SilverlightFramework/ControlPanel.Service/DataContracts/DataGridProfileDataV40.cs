using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    [DataContract]
    public class DataGridProfileDataV40 : DefaultDataContract
    {
        [DataMember]
        public List<DataGridProfileItemMsg> Body { get; set; }
    }

    [DataContract]
    public class DataGridProfileItemMsg
    {
        [DataMember]
        public KeystoneAuthUserMsg Owner { get; set; }

        [DataMember]
        public string DataGridProfileXml { get; set; }
    }
}
