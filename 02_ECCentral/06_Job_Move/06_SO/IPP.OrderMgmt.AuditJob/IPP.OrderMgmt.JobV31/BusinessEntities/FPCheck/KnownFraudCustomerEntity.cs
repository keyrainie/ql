using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit;
using System.ComponentModel;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.FPCheck
{
    public class KnownFraudCustomerEntity : EntityBase
    {
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("ShippingContact", DbType.String)]
        public string ShippingContact { get; set; }

        [DataMapping("ShippingAddress", DbType.String)]
        public string ShippingAddress { get; set; }

        [DataMapping("BrowseInfo", DbType.String)]
        public String BrowseInfo { get; set; }

        [DataMapping("MobilePhone", DbType.String)]
        public String MobilePhone { get; set; }

        [DataMapping("Telephone", DbType.String)]
        public String Telephone { get; set; }

        [DataMapping("EmailAddress", DbType.String)]
        public String EmailAddress { get; set; }

        [DataMapping("IPAddress", DbType.String)]
        public String IPAddress { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("FraudType", DbType.Int32)]
        public FraudType FraudType { get; set; }

    }

    public class DubiousCustomerEntity : EntityBase
    {
        [DataMapping("DuType", DbType.Int32)]
        public int DuType { get; set; }

        [DataMapping("Catalog", DbType.Int32)]
        public int Catalog { get; set; }    

        [DataMapping("Content", DbType.String)]
        public String Content { get; set; }
    }

    public enum FraudType
    {
        [Description("普通")]
        Normal = 0,
        [Description("可疑")]
        SuspectCustomer = 1,
        [Description("欺诈")]
        BlockCustomer = 2
    }
}
