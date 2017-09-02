using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace ECommerce.Entity.Promotion
{
    [Serializable]
    public class CouponCode 
    {
        public int SysNo { get; set; }

        public int CouponSysNo { get; set; }

        public string Code { get; set; }

        public string CodeType { get; set; }    

        /// <summary>
        /// 每个ID限用次数
        /// </summary>
        public int CustomerMaxFrequency { get; set; }

        /// <summary>
        /// 允许使用次数
        /// </summary>
        public int TotalCount { get; set; }

        public int UseCount { get; set; }     

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
