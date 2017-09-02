using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
namespace ECCentral.Job.SO.SendAlarmMailJob.BusinessEntities.SendMail
{
   [Serializable]
   public class MoreThanTenDaysOrderInfoEntity
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// FP状态
        /// </summary>
        [DataMapping("IsFPSO", DbType.Int32)]
        public int? IsFPSO
        {
            get;
            set;
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMapping("OrderStatus", DbType.Int32)]
        public int OrderStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 总金额
        /// </summary>
        [DataMapping("SOAmt", DbType.Decimal)]
        public Decimal SOAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 订单时间
        /// </summary>
        [DataMapping("OrderDate", DbType.DateTime)]
        public DateTime OrderDate
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMapping("PayTypeName", DbType.String)]
        public string PayTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式
        /// </summary> 
        [DataMapping("ShipTypeName", DbType.String)]
        public string ShipTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 结账状态
        /// </summary>
        [DataMapping("CheckoutStatus", DbType.Int32)]
        public int? CheckoutStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 更新人名称
        /// </summary>
        [DataMapping("UpdateUserSysName", DbType.String)]
        public string UpdateUserSysName
        {
            get;
            set;
        }

        /// <summary>
        /// 是否超过90天
        /// </summary>
        [DataMapping("IsMoreThan90Days", DbType.String)]
        public string IsMoreThan90Days
        {
            get;
            set;
        }

    }
}
