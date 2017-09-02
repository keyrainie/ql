using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    /// <summary>
    /// 积分获得记录
    /// </summary>
    public class PointObtainViewModel
    {
        /// <summary>
        /// 积分获得日期
        /// </summary>
        public String CreateDate { get; set; }

        /// <summary>
        /// 积分失效日期
        /// </summary>
        public String ExpireDate { get; set; }

        /// <summary>
        /// 积分获得类型
        /// </summary>
        public String ObtainType { get; set; }

        /// <summary>
        /// 获得积分数
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        /// 状态，根据获得日期及失效日期计算出来
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Memo { get; set; }
    }
}