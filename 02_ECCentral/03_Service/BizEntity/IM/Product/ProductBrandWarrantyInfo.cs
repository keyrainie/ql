using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品品牌保修信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductBrandWarrantyInfo : IIdentity
    {
        [DataMember]
        public int? SysNo { get; set; }
        [DataMember]
        public int? BrandSysNo { get; set; }
        [DataMember]
        public int? C1SysNo { get; set; }
        [DataMember]
        public int? C2SysNo { get; set; }
        [DataMember]
        public int? C3SysNo { get; set; }
        [DataMember]
        public int WarrantyDay { get; set; }
        [DataMember]
        public string WarrantyDesc { get; set; }
        [DataMember]
        public UserInfo CreateUser { get; set; }
        [DataMember]
        public UserInfo EditUser { get; set; }
        [DataMember]
        public string LanguageCode { get; set; }
        [DataMember]
        public List<Int32> SysNos { get; set; }
    }
}
