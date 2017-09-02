using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class BrandInfo
    {
        [DataMember]
        public int SysNo { get; set; }

        [DataMember]
        public int ManufacturerSysNo { get; set; }

        [DataMember]
        public string BrandName_Ch { get; set; }

        [DataMember]
        public string BrandName_En { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Website { get; set; }

        [DataMember]
        public string ServicePhone { get; set; }

        [DataMember]
        public string ServiceEmail { get; set; }

        [DataMember]
        public string ServiceUrl { get; set; }

        [DataMember]
        public string IsShowInZone { get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public int HasLogo { get; set; }

        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        public string NeweggUrl { get; set; }

        [DataMember]
        public string InitialPingYin { get; set; }

        /// <summary>
        /// 品牌LOGO图片地址
        /// </summary>
        [DataMember]
        public string ADImage { get; set; }

        [DataMember]
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string StoreCompanyCode { get; set; }
        public string LanguageCode { get; set; }
        
        [DataMember]
        public string BrandStory { get; set; }

        [DataMember]
        public string BrandCode { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
