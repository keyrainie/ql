using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 投票基本信息
    /// </summary>
    public class PollMaster : IIdentity, IWebChannel
    {
        /// <summary>
        /// 投票标题
        /// </summary>
        public LanguageContent PollName { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType { get; set; }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID { get; set; }

        /// <summary>
        /// 投票数量
        /// </summary>
        public int? PollCount { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status { get; set; }

        /// <summary>
        /// 投票问题组
        /// </summary>
        public List<PollItemGroup> PollItemGroupList { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }
}