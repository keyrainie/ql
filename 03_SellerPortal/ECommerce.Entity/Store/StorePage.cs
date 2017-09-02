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
        ///商家编号
        /// </summary>
        [DataMember]
        public int? SellerSysNo
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
        ///草稿页面链接URL
        /// </summary>
        [DataMember]
        public string LinkUrl
        {
            get;
            set;
        }

        /// <summary>
        ///发布页面链接URL
        /// </summary>
        [DataMember]
        public string PLinkUrl
        {
            get;
            set;
        }

        public string PublishUrl
        {
            get
            {
                if (string.IsNullOrEmpty(SecondDomain))
                {
                    return string.Format("{0}{1}", ECommerce.Utility.AppSettingManager.GetSetting("Store", "PreviewBaseUrl"), PLinkUrl);
                }
                else
                {
                    if (PageTypeKey.Trim() == "Home")
                    {
                        return string.Format("http://{0}.malltl.com", SecondDomain);
                    }
                    else {
                        return string.Format("http://{0}.malltl.com/{1}", SecondDomain, SysNo);
                    }
                }
            }
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
        /// 【枚举】状态，0=无效的，1=保存草稿，2=待审核，3=审核通过待发布，4=审核未通过
        /// </summary>
        [DataMember]
        public int? StorePageStatus
        {
            get;
            set;
        }

        public int? StorePageSysNo { get { return SysNo; } }

        public StorePageType StorePageType { get; set; }
        public StorePageTemplate StorePageTemplate { get; set; }

        [DataMember]
        public string SecondDomain
        {
            get;
            set;
        }
    }
}
