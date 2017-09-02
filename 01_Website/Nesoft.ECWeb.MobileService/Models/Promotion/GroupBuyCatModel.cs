using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class GroupBuyCatModel
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        public int CateSysNo { get; set; }
        /// <summary>
        /// 分类名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否是热门团购
        /// </summary>
        public int IsHotKey { get; set; }
    }
}