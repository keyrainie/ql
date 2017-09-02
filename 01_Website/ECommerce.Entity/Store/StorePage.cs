using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Entity;

namespace ECommerce.Entity.Store
{   /// <summary>
    ///店铺网页的主信息。
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorePage : EntityBase
    {
        public StorePage()
        {
            StorePageType = new StorePageType();
            StorePageTemplate = new StorePageTemplate();
        }

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
        ///关联PageType的Key
        /// </summary>
        [DataMember]
        public string PageTypeKey
        {
            get;
            set;
        }

        /// <summary>
        ///页面名称
        /// </summary>
        [DataMember]
        public string PageName
        {
            get;
            set;
        }

        /// <summary>
        ///Page的JSON数据,草稿信息的保存,和PublicshedStorePageInfo.DataValue有区别
        /// </summary>
        [DataMember]
        public string DataValue
        {
            get;
            set;
        }

        /// <summary>
        ///生成的页面链接URL
        /// </summary>
        [DataMember]
        public string LinkUrl
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

        [DataMember]
        public int SellerSysNo
        {
            get;
            set;
        }

        public int? StorePageSysNo { get { return SysNo; } }

        public StorePageType StorePageType { get; set; }
        public StorePageTemplate StorePageTemplate { get; set; }
    }
}
