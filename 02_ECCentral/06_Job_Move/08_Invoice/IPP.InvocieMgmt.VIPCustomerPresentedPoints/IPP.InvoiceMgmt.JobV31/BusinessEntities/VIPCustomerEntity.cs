using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace InvoiceMgmt.JobV31.BusinessEntities
{
    [Serializable]
    public class VIPCustomerEntity
    {
        [DataMapping("Sysno", DbType.Int32)]
        public int Sysno { get; set; }

        [DataMapping("TotalAmt", DbType.Decimal)]
        public decimal TotalAmt { get; set; }
    }
}
