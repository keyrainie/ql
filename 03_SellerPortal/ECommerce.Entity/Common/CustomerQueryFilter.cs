using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class CustomerQueryFilter : QueryFilter
    {
        public int? SysNo { get; set; }

        /// <summary>
        /// 顾客账号
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 顾客名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 顾客邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 顾客电话
        /// </summary>
        public string CellPhone { get; set; }

    }
}
