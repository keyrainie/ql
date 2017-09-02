using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    /// <summary>
    /// 积分使用记录
    /// </summary>
    public class PointConsumeViewModel
    {
        /// <summary>
        /// 积分使用类型
        /// </summary>
        public string ConsumeType { get; set; }

        /// <summary>
        /// 积分使用日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 使用积分数
        /// </summary>
        public int Point { get; set; }
    }
}