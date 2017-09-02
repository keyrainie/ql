using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store
{
    /// <summary>
    /// 店铺导航信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class StoreNavigation : EntityBase
    {
        /// <summary>
        ///系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///导航链接URL
        /// </summary>
        [DataMember]
        public string LinkUrl
        {
            get;
            set;
        }

        /// <summary>
        ///导航内容
        /// </summary>
        [DataMember]
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        ///【枚举】状态，0=无效的，1=有效的
        /// </summary>
        [DataMember]
        public int? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int? Priority
        {
            get;
            set;
        }

    }
}
