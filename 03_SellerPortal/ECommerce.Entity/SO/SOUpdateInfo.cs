using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.SO
{
    public class SOUpdateInfo
    {
        public int SOSysNo { get; set; }
        public string ReceiveContact { get; set; }
        public string ReceiveAddress { get; set; }
        public string ReceiveZip { get; set; }
        public string ReceivePhone { get; set; }
        public string ReceiveCellPhone { get; set; }
        public decimal ShipPrice { get; set; }
        public List<SOItemUpdateInfo> Items { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal TariffAmt { get; set; }
        /// <summary>
        /// 商品总价
        /// </summary>
        public decimal SOAmt { get; set; }
        /// <summary>
        /// 现金支付
        /// </summary>
        public decimal CashPay { get; set; }
        /// <summary>
        /// 最后的支付金额
        /// </summary>
        public decimal RealPayAmt { get; set; }


    }
}
