using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit;
using System.ComponentModel;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.FetchDubiousUser
{
    public class DubiousUserEntity : EntityBase
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("DuType", DbType.Int32)]
        public int DuType { get; set; }

        [DataMapping("Catalog", DbType.Int32)]
        public int Catalog { get; set; }

        [DataMapping("Content", DbType.String)]
        public String Content { get; set; }
    }

    public class UsersOfOrderEntity : EntityBase
    {
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }
    }

    public class ExpiredSpiteCustomerEntity : EntityBase
    {
        [DataMapping("content", DbType.String)]
        public String Content { get; set; }
    }

    public class AddressOfOrderEntity : EntityBase
    {
        [DataMapping("ReceiveAddress", DbType.String)]
        public String ReceiveAddress { get; set; }
    }

    public class OccupyStockUserEntity : EntityBase
    {
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }
        
        [DataMapping("OrderDate", DbType.DateTime)]
        public DateTime OrderDate { get; set; }
        
        [DataMapping("ReceiveAddress", DbType.String)]
        public String ReceiveAddress { get; set; }
        
        [DataMapping("ReceivePhone", DbType.String)]
        public String ReceivePhone { get; set; }
        
        [DataMapping("ReceiveCellPhone", DbType.String)]
        public String ReceiveCellPhone { get; set; }
        
        [DataMapping("totalAMT", DbType.Decimal)]
        public Decimal totalAMT { get; set; }
    }

    public enum ContentCatalog
    {
        [Description("用户ID")]
        UserID = 0,
        [Description("手机号码")]
        CellPhone = 1,
        [Description("电话号码")]
        Phone = 2,
        [Description("联系地址")]
        Address = 3
    }

    public enum DuType
    {
        [Description("占库存")]
        OccupyStock = 0,
        [Description("拒收")]
        Rejection = 1,
        [Description("恶意")]
        Spite = 2
    }
}
