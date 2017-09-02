using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newegg.Oversea.Framework.Contract;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    [DataContract]
    public class KeystoneAuthUserV41 : DefaultDataContract
    {
        [DataMember]
        public KeystoneAuthUserMsg Body { get; set; }
    }

    [DataContract]
    public class KeystoneAuthUserListV41 : DefaultDataContract
    {
        [DataMember]
        public List<KeystoneAuthUserMsg> Body { get; set; }
    }

    [DataContract]
    public class KeystoneAuthUserMsg
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string DepartmentNumber { get; set; }

        [DataMember]
        public string DepartmentName { get; set; }

        [DataMember]
        public int? UserSysNo { get; set; }
    }

}
