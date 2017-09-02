using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 顾客奖品信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomerGift :IIdentity
    {
        /// <summary>
        /// 顾客系统编号 
        /// </summary>
        [DataMember]
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 顾客ID
        /// </summary>
        [DataMember]
        public string CustomerID { get; set; }

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
        /// 商品数量
        /// </summary>
        [DataMember]
        public int? Quantity { get; set; }

        /// <summary>
        /// 奖品信息状态
        /// </summary>
        [DataMember]
        public CustomerGiftStatus? Status { get; set; }

        /// <summary>
        /// 订单系统编号
        /// </summary>
        [DataMember]
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        #region IIdentity Members
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
        #endregion
    }
}
