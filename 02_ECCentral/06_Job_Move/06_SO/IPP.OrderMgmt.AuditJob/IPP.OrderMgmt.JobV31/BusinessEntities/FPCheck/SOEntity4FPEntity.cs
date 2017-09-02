using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.FPCheck
{
  public  class SOEntity4FPEntity
    {
      [DataMapping("SysNo", DbType.Int32)]
      public int SysNo { get; set; }

      [DataMapping("SOAmt", DbType.Decimal)]
      public decimal SOAmt { get; set; }

      [DataMapping("SOCreateTime", DbType.DateTime)]
      public DateTime SOCreateTime { get; set; }

      [DataMapping("CustomerSysNo", DbType.Int32)]
      public int CustomerSysNo { get; set; }

      [DataMapping("ReceiveZip", DbType.String)]
      public string ReceiveZip { get; set; }

      [DataMapping("Pwd", DbType.String)]
      public string Pwd { get; set; }

      [DataMapping("CustomerRank", DbType.Int32)]
      public int CustomerRank { get; set; }

      [DataMapping("RegisterTime", DbType.DateTime)]
      public DateTime RegisterTime { get; set; }

      [DataMapping("SOCount", DbType.Int32)]
      public int SOCount { get; set; }

      [DataMapping("IsBadCustomer", DbType.Int32)]
      public int IsBadCustomer { get; set; }

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

      [DataMapping("PayTypeSysNo", DbType.Int32)]
      public int PayTypeSysNo { get; set; }

      [DataMapping("ReceiveAreaSysNo", DbType.Int32)]
      public int ReceiveAreaSysNo { get; set; }

      [DataMapping("ReceivePhone", DbType.String)]
      public string ReceivePhone { get; set; }

      [DataMapping("ReceiveCellPhone", DbType.String)]
      public string ReceiveCellPhone { get; set; }

      [DataMapping("CustomerID", DbType.String)]
      public string CustomerID { get; set; }

      /// <summary>
      /// 积分
      /// </summary>
      [DataMapping("PointPay", DbType.Int32)]
      public int PointPay 
      {
          get;
          set;
      }

      /// <summary>
      /// 蛋券
      /// </summary>
      [DataMapping("PromotionValue", DbType.Decimal)]
      public decimal? PromotionValue
      {
          get;
          set;
      }

      /// <summary>
      /// 运费
      /// </summary>
      [DataMapping("ShipPrice", DbType.Decimal)]
      public decimal ShipPrice
      {
          get;
          set;
      }

      /// <summary>
      /// 是增值税发票
      /// </summary>
      [DataMapping("IsVAT", DbType.Int32)]
      public int? IsVAT
      {
          get;
          set;
      }
    }
}
