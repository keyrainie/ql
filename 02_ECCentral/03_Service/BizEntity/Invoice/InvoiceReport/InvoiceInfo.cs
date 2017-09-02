using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.InvoiceReport
{
    public class InvoiceInfo
    {
        
        public string ReceiveName { get; set; }

        
        public string ReceiveContact { get; set; }

        
        public string ReceiveAddress { get; set; }

        
        public string ReceivePhone { get; set; }

        
        public string InvoiceNote { get; set; }

        
        public int CustomerSysNo { get; set; }

        
        public string PayTypeName { get; set; }

        
        public int SOSysNo { get; set; }

        
        public int TotalWeight { get; set; }

        
        public int PayTypeSysNo { get; set; }

        
        public decimal InvoiceAmt { get; set; }

        public string ShipTypeName { get; set; }

        /// <summary>
        /// 发票明细
        /// </summary>
        public List<InvoiceItem> Items { get; set; }

        /// <summary>
        /// 拆分序号
        /// </summary>
        public int? InvoiceSeq { get; set; }

        /// <summary>
        /// 拆分序号-总拆分号
        /// </summary>
        public string InvoiceSeqEx { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int? InvoiceCurPageNum { get; set; }

        /// <summary>
        /// 总页码
        /// </summary>
        public int? InvoiceSumPageNum { get; set; }

        /// <summary>
        /// 人民币大写金额
        /// </summary>
        public string RMBConvert { get; set; }

        /// <summary>
        /// 特别说明
        /// </summary>
        public string Importance { get; set; }

        /// <summary>
        /// 服务热线
        /// </summary>
        public string ServicePhone { get; set; }

        /// <summary>
        /// 发票日期
        /// </summary>
        public string InvoiceDate { get; set; }

        /// <summary>
        /// 收货地址1
        /// </summary>
        public string ReceiveAddress1 { get; set; }

        /// <summary>
        /// 收货地址2
        /// </summary>
        public string ReceiveAddress2 { get; set; }
    }
}
