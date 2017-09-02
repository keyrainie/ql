using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace ContributeRankUpdating
{
    public class CustomerSysnoList
    {
        [DataMapping("CustomerSysno", DbType.Int32)]
        public int CustomerSysno { get; set; }
    }
}
