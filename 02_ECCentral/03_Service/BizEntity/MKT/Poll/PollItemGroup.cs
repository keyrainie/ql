using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 投票--问题组
    /// </summary>
    public class PollItemGroup
    {
        /// <summary>
        /// 投票编号
        /// </summary>
        public int? PollSysNo { get; set; }

        /// <summary>
        /// 投票标题组
        /// </summary>
        public LanguageContent GroupName { get; set; }

        /// <summary>
        /// 选项类型  S=单选  M=多选    C=混合
        /// </summary>
        public PollType? Type { get; set; }

        /// <summary>
        /// 投票选项
        /// </summary>
        public List<PollItem> PollItemList { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }
    }
}
