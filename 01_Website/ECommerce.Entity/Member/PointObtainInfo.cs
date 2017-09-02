using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class PointObtainInfo
    {
        public DateTime CreateDate { get; set; }

        public int CustomerSysNo { get; set; }

        public DateTime ExpireDate { get; set; }

        public PointType ObtainType { get; set; }

        public int Point { get; set; }

        public int SOSysNo { get; set; }

        public int SysNo { get; set; }

        /// <summary>
        /// 状态，A--有效，D--无效
        /// </summary>
        public string Status { get; set; }
    }
}
