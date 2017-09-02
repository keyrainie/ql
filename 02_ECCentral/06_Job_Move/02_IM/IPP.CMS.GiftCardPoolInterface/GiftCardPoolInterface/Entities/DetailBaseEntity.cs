using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.ComponentModel;

namespace ContentMgmt.GiftCardPoolInterface.Entities
{
    [Serializable]
    public class DetailBaseEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("CompanyCode", DbType.AnsiStringFixedLength)]
        public string CompanyCode { get; set; }

        [DataMapping("Status", DbType.AnsiStringFixedLength)]
        public string Status { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime EditDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        public bool IsStatusActive
        {
            get
            {
                return Status == "A";
            }
            set
            {
                Status = value ? "A" : "D";
            }
        }
    }
}
