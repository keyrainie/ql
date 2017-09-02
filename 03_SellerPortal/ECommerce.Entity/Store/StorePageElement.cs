using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store
{/// <summary>
    ///预设模板的页面元素字典表；仅用于在店铺管理页面时，如果用户选择的是预设模板，第一次进入时从本字典表中预加载好。
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorePageElement
    {
        /// <summary>
        ///页面元素的Key，与店铺模板、页面类型一起形成主键
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        ///页面元素名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///页面元素描述
        /// </summary>
        [DataMember]
        public string Desc { get; set; }

        /// <summary>
        ///元素归组名称
        /// </summary>
        [DataMember]
        public string ElementGroupName { get; set; }

        /// <summary>
        ///0=不需要解析获取数据1=直接获取该Element的值（指的是HtmlEditor这种Element）
        /// </summary>
        [DataMember]
        public int? DataExecuteType { get; set; }

        /// <summary>
        ///解析器名称
        /// </summary>
        [DataMember]
        public string DataExecutorName { get; set; }

        /// <summary>
        ///解析器描述
        /// </summary>
        [DataMember]
        public string DataExecutorDesc { get; set; }

        /// <summary>
        ///解析器的实现类类型，解释可以通过泛型接口+反射模式获得
        /// </summary>
        [DataMember]
        public string DataExecutorImplType { get; set; }

        /// <summary>
        ///ElementType <> 0 时， 此元素的PartialView路径，格式:~/Views/Store/Element/{Key}.cshtml，为空则表示无PartialView。
        /// </summary>
        [DataMember]
        public string FrontPartialViewPath { get; set; }

        /// <summary>
        ///样本首页的Url。去除Host的绝对路径，如：/StoreTemplateMockup/Index.html
        /// </summary>
        [DataMember]
        public string MockupUrl { get; set; }

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

        /// <summary>
        /// 保存那些手动设置数据的element的数据,序列化好之后保存在PageInfo中
        /// </summary>
        public string DataValue { get; set; }

        public string Title { get; set; }
    }
}
