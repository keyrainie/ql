using System;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT.Keyword
{
    /// <summary>
    /// 外网搜索优化关键字
    /// </summary>
    public class InternetKeywordInfo : IIdentity
    {
        public int? SysNo { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Searchkeyword { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public IsDefaultStatus Status { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateDate { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public UserInfo OperateUser { get; set; }
    }
}
