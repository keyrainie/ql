using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 投票问题组--选项
    /// </summary>
    public class PollItem
    {
        /// <summary>
        /// 投票问题组编号
        /// </summary>
        public int? PollItemGroupSysNo { get; set; }

        /// <summary>
        /// 选项对象
        /// </summary>
        public LanguageContent ItemName { get; set; }

        /// <summary>
        /// 选中次数
        /// </summary>
        public int? PollCount { get; set; }

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

    /// <summary>
    /// 投票答案列表
    /// </summary>
    public class PollItemAnswer
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 投票组编号
        /// </summary>
        public int? PollItemGroupSysNo { get; set; }

        /// <summary>
        /// 答案内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// AgentID
        /// </summary>
        public int? AgentID { get; set; }

    }
}
