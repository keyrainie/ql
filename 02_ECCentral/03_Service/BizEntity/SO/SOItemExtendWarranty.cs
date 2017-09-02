using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.SO
{

    /// <summary>
    /// 订单商品延保信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOExtendWarrantyInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        [DataMember]
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 延保系统编号
        /// </summary>
        [DataMember]
        public int? WarrantySysNo // ProductSysNO
        { get; set; }
        /// <summary>
        /// 商品延保名称
        /// </summary>
        [DataMember]
        public string BriefName
        { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public decimal? Price { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int? Quantity { get; set; }
        /// <summary>
        /// 购买延保的商品的系统编号
        /// </summary>
        [DataMember]
        public int? MasterProductSysNo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public SOExtendWarrantyStatus? Status { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 操作者编号
        /// </summary>
        [DataMember]
        public int? CreateUserSysNo { get; set; }
    }

}
