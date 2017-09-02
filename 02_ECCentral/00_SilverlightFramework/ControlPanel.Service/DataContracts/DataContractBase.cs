using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Newegg.Oversea.Framework.Contract;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    [DataContract]
    public class DataContractBase<T> : DefaultDataContract where T : class, new()
    {
        [DataMember]
        public T Body { get; set; }
    }
}
