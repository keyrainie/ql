using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    [Serializable]
    [DataContract]
    public class ControlPanelUser:IIdentity,ICompany
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public string LoginName { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string DepartmentCode { get; set; }

        [DataMember]
        public string DepartmentName { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public ControlPanelUserStatus? Status { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public DateTime? InDate { get; set; }

        [DataMember]
        public string EditUser { get; set; }

        [DataMember]
        public DateTime? EditDate { get; set; }

        [DataMember]
        public string SourceDirectory { get; set; }

        [DataMember]
        public string LogicUserId { get; set; }

        [DataMember]
        public string PhysicalUserId { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string Province { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public int? OrganizationID { get; set; }
    }
}
