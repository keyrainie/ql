using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class CouponCode : DetailBaseEntity
    {
        [DataMapping("CouponSysNo", DbType.Int32)]
        public int CouponSysNo { get; set; }

        [DataMapping("CouponCode", DbType.String)]
        public string Code { get; set; }

        [DataMapping("CodeType", DbType.String)]
        public string CodeType { get; set; }    

        /// <summary>
        /// 每个ID限用次数
        /// </summary>
        [DataMapping("CustomerMaxFrequency", DbType.Int32)]
        public int CustomerMaxFrequency { get; set; }

        /// <summary>
        /// 允许使用次数
        /// </summary>
        [DataMapping("TotalCount", DbType.Int32)]
        public int TotalCount { get; set; }

        [DataMapping("UseCount", DbType.Int32)]
        public int UseCount { get; set; }     

        [DataMapping("BeginDate", DbType.DateTime)]
        public DateTime BeginDate { get; set; }

        [DataMapping("EndDate", DbType.DateTime)]
        public DateTime EndDate { get; set; }
    }
}
