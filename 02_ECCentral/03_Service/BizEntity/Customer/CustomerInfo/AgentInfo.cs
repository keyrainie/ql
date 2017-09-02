using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 代理信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class AgentInfo
    {
        /// <summary>
        /// 客户系统编号
        /// </summary>
        [DataMember]
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 代理类型
        /// </summary>
        [DataMember]
        public AgentType? AgentType { get; set; }
        /// <summary>
        /// 所在区域
        /// </summary>
        [DataMember]
        public int? AreaSysNo { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        [DataMember]
        public string CertificateNo { get; set; }

        /// <summary>
        /// 客户的qq号
        /// </summary>
        [DataMember]
        public string QQ { get; set; }
        /// <summary>
        /// 客户的邮箱地址
        /// </summary>
        [DataMember]
        public string MSN { get; set; }

        #region 校园代理相关
        /// <summary>
        /// 所属高校
        /// </summary>
        [DataMember]
        public string College { get; set; }
        /// <summary>
        /// 只修
        /// </summary>
        [DataMember]
        public string Major { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        [DataMember]
        public string Profession { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        [DataMember]
        public string StudentNo { get; set; }
        /// <summary>
        /// 同学电话
        /// </summary>
        [DataMember]
        public string SchoolmatePhone { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        [DataMember]
        public DateTime? GraduateDate { get; set; }
        /// <summary>
        /// 校园论坛地址
        /// </summary>
        [DataMember]
        public string SchoolBBS { get; set; }
        /// <summary>
        /// 是否有大海报需求
        /// </summary>
        [DataMember]
        public int? PosterRequest { get; set; }
        /// <summary>
        /// 是否有DM单需求
        /// </summary>
        [DataMember]
        public int? DMRequest { get; set; }
        /// <summary>
        /// 是否对外开发
        /// </summary>
        [DataMember]
        public int? OpenedToPublic { get; set; }
        #endregion
        /// <summary>
        /// 公司名称
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }
        /// <summary>
        /// 公司电话
        /// </summary>
        [DataMember]
        public string CompanyPhone { get; set; }
        /// <summary>
        /// 影响范围
        /// </summary>
        [DataMember]
        public string AffectRange { get; set; }
        /// <summary>
        /// 家庭电话
        /// </summary>
        [DataMember]
        public string HomePhone { get; set; }
        /// <summary>
        /// 建议
        /// </summary>
        [DataMember]
        public string Suggest { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public string Status { get; set; }

    }
}
