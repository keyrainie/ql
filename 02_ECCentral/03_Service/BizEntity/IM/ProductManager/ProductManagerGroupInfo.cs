//************************************************************************
// 用户名				泰隆优选
// 系统名				商品管理员组
// 子系统名		        商品管理员组实体
// 作成者				Tom.H.Li
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// PM分组信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductManagerGroupInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// PM组名称
        /// </summary>
        [DataMember]
        public LanguageContent PMGroupName { get; set; }

        /// <summary>
        /// PM组Leader信息
        /// </summary>
        [DataMember]
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public PMGroupStatus Status { get; set; }

        /// <summary>
        /// PM组包含的PM
        /// </summary>
        [DataMember]
        public List<ProductManagerInfo> ProductManagerInfoList { get; set; }
    }
}
