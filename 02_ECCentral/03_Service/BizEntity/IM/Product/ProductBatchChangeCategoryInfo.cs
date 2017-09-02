using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品质保信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductBatchChangeCategoryInfo
    {
        /// <summary>
        /// 商品ID列表
        /// </summary>
        [DataMember]
        public List<string> ProductIDs { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        [DataMember]
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

    }
}
