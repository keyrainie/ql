using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 移仓单跟进日志
    /// </summary>
    public class ShiftRequestMemoInfo : IIdentity
    {
        public int? SysNo { get; set; }

        public int? RequestSysNo { get; set; }

        public string Content { get; set; }

        public ShiftRequestMemoStatus MemoStatus { get; set; }

        public UserInfo CreateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public UserInfo EditUser { get; set; }

        public DateTime? EditDate { get; set; }

        public string Note { get; set; }

        public DateTime? RemindTime { get; set; }

    }
}
