using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Common
{
    /// <summary>
    /// 配送方式信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShippingType
    {
        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 配送方式名称
        /// </summary>
        [DataMember]
        public string ShippingTypeName { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        [DataMember]
        public string ShipTypeID { get; set; }
        /// <summary>
        /// 提供方
        /// </summary>
        [DataMember]
        public string Provider { get; set; }

        /// <summary>
        /// 保价费基数
        /// </summary>
        [DataMember]
        public decimal? PremiumBase { get; set; }

        /// <summary>
        /// 保价费率
        /// </summary>
        [DataMember]
        public decimal? PremiumRate { get; set; }


        /// <summary>
        /// 赔付金额上限
        /// </summary>
        [DataMember]
        public decimal? CompensationLimit { get; set; }

        /// <summary>
        /// 送货周期
        /// </summary>
        [DataMember]
        public string Period { get; set; }
    }

}
