using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 首页热销排行 
    /// </summary>
    public class HotSaleCategory : IIdentity, IWebChannel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 分类别名
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus Status { get; set; }

        /// <summary>
        /// 三级分类编号
        /// </summary>
        public int C3SysNo { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public int Position { get; set; }

       /// <summary>
        /// 页面类型
       /// </summary>
        public int PageType { get; set; }

        /// <summary>
        /// 页面编号
        /// </summary>
        public int PageId { get; set; }
    }
}
