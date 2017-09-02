//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理实体
// 子系统名		        品牌售后支持信息实体
// 作成者				Tom.H.Li
// 改版日				2012.4.21
// 改版内容				新建
//************************************************************************
using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 品牌支持信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrandSupportInfo
    {
        /// <summary>
        /// 客服电话
        /// </summary>
        [DataMember]
        public String ServicePhone { get; set; }

        /// <summary>
        /// 售后支持邮箱
        /// </summary>
        [DataMember]
        public String ServiceEmail { get; set; }

        /// <summary>
        /// 售后支持链接
        /// </summary>
        [DataMember]
        public String ServiceUrl { get; set; }

        /// <summary>
        /// 厂商链接
        /// </summary>
        [DataMember]
        public String ManufacturerUrl { get; set; }
    }
}
