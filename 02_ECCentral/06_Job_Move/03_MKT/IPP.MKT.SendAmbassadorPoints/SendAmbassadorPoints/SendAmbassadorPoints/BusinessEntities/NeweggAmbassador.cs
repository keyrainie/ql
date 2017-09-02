using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints.BusinessEntities
{
    [Serializable]
    public class NeweggAmbassador
    {
        [DataMapping("CustomerMark", DbType.String)]
        public int? CustomerMark { get; set; }  //'大使状态'-- [1：未激活；2：已激活]

        [DataMapping("CustomerMarkDate", DbType.DateTime)]
        public DateTime? CustomerMarkDate { get; set; } // '生效日期' 

        [DataMapping("CustomerActiveDate", DbType.DateTime)]
        public DateTime? CustomerActiveDate { get; set; } // '激活日期'       

        [DataMapping("AmbassadorID", DbType.String)]//大使id
        public string AmbassadorID { get; set; }

        [DataMapping("AmbassadorName", DbType.String)]//大使姓名
        public string AmbassadorName { get; set; }

        [DataMapping("CustomerSysNo", DbType.Int32)]//大使sysno
        public int? CustomerSysNo { get; set; }

        [DataMapping("Email", DbType.String)]
        public string Email { get; set; }

        [DataMapping("CellPhone", DbType.String)]
        public string CellPhone { get; set; }

        [DataMapping("ProvinceName", DbType.String)]
        public string ProvinceName { get; set; }

        [DataMapping("CityName", DbType.String)]
        public string CityName { get; set; }

        [DataMapping("DistrictName", DbType.String)]
        public string DistrictName { get; set; }

        [DataMapping("BigProvinceName", DbType.String)]
        public string BigProvinceName { get; set; }

        [DataMapping("BigProvinceSysNo", DbType.Int32)]
        public int BigProvinceSysNo { get; set; }

        [DataMapping("CustomerName", DbType.String)]
        public string CustomerName { get; set; }//  '大使姓名'

        [DataMapping("SOID", DbType.String)]
        public string SOID { get; set; }

        //CashPay //+ SO.DiscountAmt + SO.PointPay/10
        [DataMapping("OrderAmt", DbType.Decimal)]
        public decimal? OrderAmt { get; set; } // '订单金额'

        [DataMapping("OrderDate", DbType.DateTime)]
        public DateTime? OrderDate { get; set; }

        [DataMapping("CustomerID", DbType.String)]
        public string CustomerID { get; set; } // '下单ID' 

        [DataMapping("PointAmount", DbType.Int32)]
        public int? PointAmount { get; set; }  //'奖励积分'

        [DataMapping("SOStatus", DbType.Int32)]  //订单状态
        public int? SOStatus { get; set; }

        [DataMapping("PayStatus", DbType.Int32)]
        public int? PayStatus { get; set; } //'付款状态'

        [DataMapping("ConfirmTime", DbType.DateTime)] //付款日期
        public DateTime? ConfirmTime { get; set; }

        [DataMapping("IsAddPoint", DbType.String)] //是否放积分
        public int? IsAddPoint { get; set; }

        [DataMapping("PointAddDate", DbType.DateTime)]
        public DateTime? PointAddDate { get; set; }//   '积分发放日期'

        [DataMapping("TotalOrderAmt", DbType.Decimal)] //已确认订单总金额
        public decimal TotalOrderAmt { get; set; }
    }
}
