using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace ECCentral.Service.Utility
{
    public interface IEventMessage
    {
        string Subject { get; }
    }

    public abstract class EventMessage : IEventMessage
    {
        [XmlIgnore] // for XmlSerializer
        [ScriptIgnore] // for JavaScriptSerializer
        [IgnoreDataMember] // for DataContractJsonSerializer
        public virtual string Subject
        {
            get { return GetType().Name; }
        }
    }
}

