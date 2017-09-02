using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 批量切换评论模式
    /// </summary>
    public class BatchSwitchover : IIdentity, IWebChannel
    {
        /// <summary>
        /// 商品三级类别编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 评论模式        0=字段展示  -1=手动展示
        /// </summary>
        public int? RemarkRule { get; set; }

        /// <summary>
        /// 节假日模式        0=自动展示   -1=手动展示
        /// </summary>
        public int? WeekendRule { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
