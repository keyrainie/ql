using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.BusinessEntities
{
    [Serializable]
    public class RemarkMode
    {
        [DataMapping("RemarkModeSysNo", DbType.Int32)]
        public int RemarkModeSysNo { get; set; }

        [DataMapping("RemarkType", DbType.String)]
        public string RemarkType { get; set; }

        [DataMapping("RemarkID", DbType.Int32)]
        public int RemarkID { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("LastEditUserSysNo", DbType.Int32)]
        public int LastEditUserSysNo { get; set; }

        [DataMapping("LastEditDate", DbType.DateTime)]
        public DateTime LastEditDate { get; set; }

        [DataMapping("WeekendRule", DbType.Int32)]
        public int WeekendRule { get; set; }
    }
}
