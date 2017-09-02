using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 礼品券商品信息表产品关联信息表
    /// </summary>
    public class GiftVoucherProductRelation
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// GiftVoucherProduct表的SysNo
        /// </summary>
        [DataMember]
        public int GiftVoucherSysNo { get; set; }

        /// <summary>
        /// 商品SysNo
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 礼品券状态
        /// </summary>
        [DataMember]
        public GiftVoucherRelateProductStatus Status { get; set; }

        /// <summary>
        /// 礼品券兑换类型
        /// </summary>
        [DataMember]
        public GiftVoucherType GiftVoucherType { get; set; }

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
        /// 请求类型
        /// </summary>
        [DataMember]
        public GVRReqType Type { get; set; }

        /// <summary>
        /// 请求状态
        /// </summary>
        [DataMember]
        public GVRReqAuditStatus AuditStatus { get; set; }

        /// <summary>
        /// 请求记录
        /// </summary>
        [DataMember]
        public List<GiftVoucherProductRelationRequest> RequestLogs { get; set; }
    }
}
