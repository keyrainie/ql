using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoClose.Model
{
    public class POMailInfo
    {
        [DataMapping("Sysno", DbType.Int32)]
        public int Sysno { get; set; }

        [DataMapping("MailAddress", DbType.String)]
        public string MailAddress { get; set; }

        [DataMapping("PMEmail", DbType.String)]
        public string PMEmail { get; set; }

        [DataMapping("CreateEmail", DbType.String)]
        public string CreateEmail { get; set; }
    }
}
