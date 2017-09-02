using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 活动次数规则
    /// </summary>
    public class PSActivityFrequencyCondition 
    {
        /// <summary>
        /// 每个Customer ID限定活动次数
        /// </summary>
        public int? CustomerMaxFrequency { get; set; }

        /// Customer 已使用的活动次数 
        /// </summary>
        public int? CustomerUsedFrequency { get; set; }

        /// <summary>
        /// 全网限定活动次数
        /// </summary>
        public int? MaxFrequency { get; set; }


        /// <summary>
        /// 全网已使用的活动次数 
        /// </summary>
        public int? UsedFrequency { get; set; }
         
    }
}
