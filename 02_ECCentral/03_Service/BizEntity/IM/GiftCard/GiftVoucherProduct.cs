using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 礼品券商品信息表
    /// </summary>
    [DataContract]
    public class GiftVoucherProduct
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 礼品券ID
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// 商品SysNo
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }


        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 礼品券价值
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 礼品券商品状态
        /// </summary>
        [DataMember]
        public GiftVoucherProductStatus Status { get; set; }

        

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public int CreateUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public int AuditUser { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        [DataMember]
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [DataMember]
        public int EditUser { get; set; }

        /// <summary>
        /// 编辑日期
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 礼品券的商品范围
        /// </summary>
        [DataMember]
        public List<GiftVoucherProductRelation> RelationProducts { get; set; }
    }
}
