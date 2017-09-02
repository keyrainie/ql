using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace SendMessage.Entity
{
    [Serializable]
    public class SmsEntity : EntityBase
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("CellNumber", DbType.String)]
        public string CellNumber { get; set; }

        [DataMapping("SMSContent", DbType.String)]
        public string SMSContent { get; set; }

        [DataMapping("Priority", DbType.Int32)]
        public int Priority { get; set; }

        [DataMapping("RetryCount", DbType.Int32)]
        public int RetryCount { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataMapping("HandleTime", DbType.DateTime)]
        public DateTime HandleTime { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("CreateUserSysno", DbType.Int32)]
        public int CreateUserSysno { get; set; }

        [DataMapping("Type", DbType.String)]
        public string Type { get; set; }


        public bool CheckResult { get; set; }

        public string ProcessMessage { get; set; }
    }
}
