using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store
{
    /// <summary>
    ///页面类型，不允许随意修改，页面类型配合页面ID可以定位到所有的数据页面，所有店铺模板的页面类型都是一样的
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorePageType
    {
        /// <summary>
        ///页面类型的Key
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        ///页面类型名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///页面类型描述
        /// </summary>
        [DataMember]
        public string Desc { get; set; }

        /// <summary>
        ///是否允许同一个店铺有多个页面,1:允许,0:不允许
        /// </summary>
        [DataMember]
        public int? AllowMultiPage { get; set; }

        /// <summary>
        ///【枚举】是否允许这个类型的页面可以设置模板
        ///0=不允许设置模板，1=允许设置模板
        /// </summary>
        [DataMember]
        public int? AllowSetTemplate { get; set; }

        /// <summary>
        ///是否是给店铺维护时使用
        /// </summary>
        [DataMember]
        public int? IsForStoreWebsite { get; set; }

        /// <summary>
        ///预留：是否是给主网站维护时使用，同一个页面类型的字典，可能会同时给主网站和店铺使用，比如首页
        /// </summary>
        [DataMember]
        public int? IsForMasterWebsite { get; set; }

        /// <summary>
        ///【枚举】需要与PageID结合起来定位到数据页。
        ///0=不需要PageID，如网站首页，帮助中心首页 等无参页
        ///1=需要结合前台商品类别的一级类别
        ///2=需要结合前台商品类别的二级类别
        ///3=需要结合前台商品类别的三级类别
        ///4=需要结合品牌编号
        ///5=需要结合商家编号
        ///6=需要结合促销活动页面模板
        /// </summary>
        [DataMember]
        public int? PageIDType { get; set; }

        /// <summary>
        ///【枚举】1=默认网站，2=App，3=微信；目前只使用1
        /// </summary>
        [DataMember]
        public int? WebsiteID { get; set; }

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
    }
}

