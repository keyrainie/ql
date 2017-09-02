using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 送货时间 区域  （上午、下午）
    /// </summary>
    [Serializable]
    [DataContract]
    public class DeliveryTimeRange
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 送货时间 区域  编号
        /// </summary>
        [DataMember]
        public string DeliveryTimeRangeID { get; set; }

        /// <summary>
        ///送货时间 区域  描述
        /// </summary>
        [DataMember]
        public string DeliveryTimeRangeName { get; set; }
    }
}
