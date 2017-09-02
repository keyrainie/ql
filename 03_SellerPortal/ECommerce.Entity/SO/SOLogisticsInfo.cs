using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.SO
{
    public class SOLogisticsInfo
    {
        /// <summary>
        /// 顺丰状态码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 接受时间
        /// </summary>
        public string AcceptTime { get; set; }
        /// <summary>
        /// 接受地点
        /// </summary>
        public string AcceptAddress { get; set; }
        /// <summary>
        /// 圆通接受人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 圆通状态值
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 物流类型
        /// </summary>
        public ExpressType Type { get; set; }
    }
}
