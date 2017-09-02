using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.IM.Product
{
    [Serializable]
    [DataContract]
    public class ProductStepPriceInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 本段起始数量
        /// </summary>
        [DataMember]
        public int BaseCount { get; set; }

        /// <summary>
        /// 本段截至数量
        /// </summary>
        [DataMember]
        public int TopCount { get; set; }

        /// <summary>
        /// 本段最高价格
        /// </summary>
        [DataMember]
        public string StepPrice { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [DataMember]
        public string InUser { get; set; }
        
        [DataMember]
        public string VendorSysNo { get; set; }

        [DataMember]
        public string VendorName { get; set; }

        [DataMember]
        public string ProductID { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public string Editdate { get; set; }

        [DataMember]
        public string Indate { get; set; }

        [DataMember]
        public string EditUser { get; set; }

    }
}
