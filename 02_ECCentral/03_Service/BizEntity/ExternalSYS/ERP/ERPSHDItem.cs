using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.ExternalSYS
{
    [Serializable]
    [DataContract]
    public class ERPSHDItem
    {
        /// <summary>
        /// 系统商品编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }
      
        /// <summary>
        /// 送货单中序号
        /// </summary>
        [DataMember]
        public int? INX { get; set; }

        /// <summary>
        /// 送货单记录编号
        /// </summary>
        [DataMember]
        public int? JLBH { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public decimal? SL { get; set; }

        /// <summary>
        /// ERP商品ID
        /// </summary>
        [DataMember]
        public int? ERP_SP_ID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string BZ { get; set; }
    }
}
