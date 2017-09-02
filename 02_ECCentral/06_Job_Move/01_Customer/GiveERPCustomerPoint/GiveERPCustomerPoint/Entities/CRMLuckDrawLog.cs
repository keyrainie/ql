using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace GiveERPCustomerPoint.Entities
{
   public class CRMLuckDrawLog
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMapping("OrderSysNo", DbType.Int32)]
        public int OrderSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 会员卡号
        /// </summary>
        [DataMapping("MemberShipCardID", DbType.String)]
        public string MemberShipCardID
        {
            get;
            set;
        }
        /// <summary>
        /// 抽奖名称
        /// </summary>
        [DataMapping("LuckDrawName", DbType.String)]
        public String LuckDrawName
        {
            get;
            set;
        }
        /// <summary>
        /// 抽奖ID
        /// 
        /// </summary>
        [DataMapping("LuckDrawID", DbType.String)]
        public string LuckDrawID
        {
            get;
            set;
        }
        /// <summary>
        /// 抽奖号码
        /// 
        /// </summary>
        [DataMapping("LuckDrawCode", DbType.String)]
        public string LuckDrawCode
        {
            get;
            set;
        }
        /// <summary>
        /// 抽奖标记
        /// </summary>
        [DataMapping("LuckDrawMark", DbType.String)]
        public string LuckDrawMark
        {
            get;
            set;
        }

        /// <summary>
        /// 支付标记
        /// </summary>
        
        [DataMapping("PayMark", DbType.String)]
        public string PayMark
        {
            get;
            set;
        }
    }
}
