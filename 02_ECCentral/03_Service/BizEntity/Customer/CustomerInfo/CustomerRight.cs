using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 用户权限对象
    /// </summary>
    public class CustomerRight
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public int? Right { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string RightDescription { get; set; }
    }

}
