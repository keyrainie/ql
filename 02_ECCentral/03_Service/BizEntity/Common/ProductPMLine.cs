using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 用于产品线验证业务
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductPMLine
    {
        /// <summary>
        /// 商品SysNo
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 产品线SysNo
        /// </summary>
        [DataMember]
        public int? ProductLineSysNo { get; set; }
        /// <summary>
        /// 产品线OwnerSysNo
        /// </summary>
        [DataMember]
        public int? PMSysNo { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }
    }
}
