using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.OrderMgmt.ServiceJob.BusinessEntities.SecKill
{
    public class ProductNotAutoSetVirtualEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("CountDownSysNo", DbType.Int32)]
        public int CountDownSysNo { get; set; }

        [DataMapping("AbandonTime", DbType.DateTime)]
        public DateTime AbandonTime { get; set; }

        [DataMapping("AbandonUserSysNo", DbType.Int32)]
        public int AbandonUserSysNo { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int CreateUserSysNo { get; set; }

        [DataMapping("NotAutoSetVirtualType", DbType.Int32)]
        public int NotAutoSetVirtualType { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }
    }

    public enum NotAutoSetVirtualType
    {
        [Description("限时抢购")]
        CountDown = 1,
        [Description("PM手动设置虚库")]
        PM_SetManually = 2,
    }

}
