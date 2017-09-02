using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage
{
    public class InvoiceMessageBase
    {
        public MessageHeader Header { get; set; }
        public InvoiceCollection Body { get; set; }
    }

    public class InvoiceCollection : List<InvoiceItem>
    {
    }

    public class InvoiceItem
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderSysNo { get; set; }

        public InvoiceMsgOrderType OrderType { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string StockID { get; set; }

        /// <summary>
        /// 消息发送Path（暂时在Service这边查询数据库获取）
        /// </summary>
        public string MSMQAddress { get; set; }
    }

    public enum InvoiceMsgOrderType
    {
        RO, ADJUST
    }

    public enum InvoiceMsgDirection
    {
        In, Out
    }

    public enum InvoiceMsgDestination
    {
        Oversea, External
    }

    public enum InvoiceMsgType
    {
        Unknown, SO, Invoice
    }
}
