using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store
{/// <summary>
    ///页面布局区块
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorePageLayout
    {
    public StorePageLayout()
    {
        StorePageElements=new List<StorePageElement>();
    }
        /// <summary>
        ///区块Key
        /// </summary>
        [DataMember]
        public string PageLayoutKey { get; set; }

        /// <summary>
        ///区块名称
        /// </summary>
        [DataMember]
        public string PageLayoutName { get; set; }

        /// <summary>
        ///【枚举】状态，0=无效的，1=有效的
        /// </summary>
        [DataMember]
        public int? Status { get; set; }

        /// <summary>
        ///备注说明，通常说明这个对象是干什么用的，以及修改记录
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        public List<StorePageElement> StorePageElements { get; set; }
    }
}
