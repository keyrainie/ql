using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    [Serializable]
    [DataContract]
    public class VendorStoreInfo : IIdentity, ICompany
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public int? VendorSysNo { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int? AreaSysNo { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string MapAddress { get; set; }

        [DataMember]
        public string Telephone { get; set; }

        [DataMember]
        public string OtherContact { get; set; }

        [DataMember]
        public DateTime? OpeningHoursFrom { get; set; }

        [DataMember]
        public DateTime? OpeningHoursTo { get; set; }

        [DataMember]
        public string CreateUser { get; set; }

        [DataMember]
        public DateTime? CreateDate { get; set; }

        [DataMember]
        public string EditUser { get; set; }

        [DataMember]
        public DateTime? EditDate { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string CityBusLine { get; set; }

        [DataMember]
        public string CarPark { get; set; }

        [DataMember]
        public string FloorSetting { get; set; }
    }
}
