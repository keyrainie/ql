using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 发布的店铺网页的主信息。
    /// </summary>
    public class PublishedStorePageInfo : StorePageInfo
    {
        /// <summary>
        /// 店铺网页原稿ID
        /// </summary>
        [DataMember]
        public int? StorePageSysNo
        {
            get; set;
        }
    }
}
