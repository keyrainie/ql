using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT.Promotion
{
    [Serializable]
    [DataContract]
    public class ProductPayTypeInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 商品ID列表
        /// </summary>
        [DataMember]
        public string ProductIds { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public int? PayTypeSysNo { get; set; }

        /// <summary>
        /// 最后编辑人
        /// </summary>
        [DataMember]
        public string EditUser { get; set; }

        /// <summary>
        /// 最后编辑时间
        /// </summary>
        [DataMember]
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 商品支付方式ID
        /// </summary>
        [DataMember]
        public string ProductPayTypeIds { get; set; }

        /// <summary>
        /// 支付方式列表
        /// </summary>
        [DataMember]
        public List<PayTypeInfo> PayTypeList { get; set; }
    }
}
