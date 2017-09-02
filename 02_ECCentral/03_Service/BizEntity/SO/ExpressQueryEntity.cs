using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 物流查询实体
    /// </summary>
    public class ExpressQueryEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExpressQueryEntity()
        {
            this.TrackingNumberList = new List<string>();
        }
        /// <summary>
        /// 物流类型
        /// YT 圆通
        /// SF 顺丰
        /// </summary>
        public ExpressType Type { get; set; }
        /// <summary>
        /// 运单列表
        /// </summary>
        public List<string> TrackingNumberList { get; set; }
    }
}
