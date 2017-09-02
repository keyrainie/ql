using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.IM
{
    [DataContract]
    public class GiftVoucherProductRelationRequest
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }


        /// <summary>
        /// 关系信息SysNo
        /// </summary>
        [DataMember]
        public int RelationSysNo { get; set; }

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
        /// 创建人
        /// </summary>
        [DataMember]
        public int CreateUser { get; set; }

        [DataMember]
        public string CreaterName { get; set; }

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

        [DataMember]
        public string AuditerName { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        [DataMember]
        public DateTime? AuditDate { get; set; }
    }
}
