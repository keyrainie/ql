using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    /// <summary>
    /// 商品分组属性(最多有两个分组属性)
    /// </summary>
    public class GroupPropertyModel
    {
        /// <summary>
        /// 分组属性1名称
        /// </summary>
        public string PropertyDescription1 { get; set; }

        /// <summary>
        /// 分组属性1值
        /// </summary>
        public string ValueDescription1 { get; set; }

        /// <summary>
        /// 分组属性1值列表
        /// </summary>
        public List<GroupPropertyItemModel> Property1List { get; set; }

        public string PropertyDescription2 { get; set; }

        public string ValueDescription2 { get; set; }

        public List<GroupPropertyItemModel> Property2List { get; set; }
    }
}