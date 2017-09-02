using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce.Web.Models
{
    public class BatchActionVM
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 业务单据编号
        /// </summary>
        public int[] Ids { get; set; }
    }
}