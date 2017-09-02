using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Newegg.Oversea.Framework.Contract;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    [DataContract]
    public class AuthUserV41 : DefaultDataContract
	{
        [DataMember]
        public AuthUserMsg Body { get; set; }
	}

    [DataContract]
    public class AuthUserListV41 : DefaultDataContract
    {
        [DataMember]
        public List<AuthUserMsg> Body { get; set; }
    }

    [DataContract]
    public class AuthUserMsg
    {
        [DataMember]
        public string UniqueName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string FullName { get; set; }
    }


    [DataContract]
    public class AuthUserQueryV41 : DefaultDataContract
    {
        [DataMember]
        public AuthUserQueryMsg Body { get; set; }
    }

    [DataContract]
    public class AuthUserQueryMsg
    {
        [DataMember]
        public string ApplicationId { get; set; }

        [DataMember]
        public string RoleName { get; set; }

        [DataMember]
        public List<string> FunctionNames { get; set; }
    }
}
