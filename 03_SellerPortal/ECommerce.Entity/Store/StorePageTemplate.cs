using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store
{/// <summary>
    ///页面模板，不允许随意修改，可供业务类型选用； 本表中保存各种页面类型中预设的几种模板，和几个通用型的自定义模板。
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorePageTemplate
    {

        public StorePageTemplate()
        {
            StorePageLayouts = new List<StorePageLayout>();
        }

        /// <summary>
        ///模板Key，默认为“Default”；
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        ///【枚举】模板类型：
        ///1=给店铺直接使用的预设好的模板；
        ///2=自定义模板，PageTypeKey此时为NULL；
        /// </summary>
        [DataMember]
        public int? PageTemplateType { get; set; }

        /// <summary>
        ///模板的默认Page数据,JSON格式
        /// </summary>
        [DataMember]
        public string DataValue { get; set; }

        /// <summary>
        ///页面类型的Key，允许为空。
        ///店铺管理时：
        ///用户选择页面类型，列出该页面类型Key对应的模板+所有自定义模板，以供选择。
        /// </summary>
        [DataMember]
        public string PageTypeKey { get; set; }

        /// <summary>
        ///模板名称，模板名称命名规范如下：套版名称_页面类型名称。
        ///例如：橙色温馨食品_首页
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///模板描述，该描述将在用户选择时作为Tips使用，帮助用户选用。
        /// </summary>
        [DataMember]
        public string Desc { get; set; }

        /// <summary>
        ///模板在网站的绝对路径，格式:~/Views/Store/{Key}.cshtml
        /// </summary>
        [DataMember]
        public string TemplateViewPath { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public string MockupUrl { get; set; }

        /// <summary>
        ///状态，0=无效的，1=有效的
        /// </summary>
        [DataMember]
        public int? Status { get; set; }

        /// <summary>
        ///备注说明，通常说明这个对象是干什么用的，以及修改记录
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        [DataMember]
        public string Priority { get; set; }

        public List<StorePageLayout> StorePageLayouts { get; set; }

        public int StorePageThemeSysNo { get; set; }
    }
}
