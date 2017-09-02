using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class SecondDomainInfo
    {
        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 商家二级域名
        /// </summary>
        public string SecondDomain { get; set; }
        /// <summary>
        /// 商家二级域名状态
        /// </summary>
        public SecondDomainStatus? SecondDomainStatus { get; set; }
    }
}
