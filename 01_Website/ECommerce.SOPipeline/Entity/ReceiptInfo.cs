using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 订单的发票收据信息
    /// </summary>
    public class ReceiptInfo : ExtensibleObject
    {
        /// <summary>
        /// 是否开增值税发票
        /// </summary>
        public bool IsVATInvoice { get; set; }

        /// <summary>
        /// 个人发票的抬头
        /// </summary>
        public string PersonalInvoiceTitle { get; set; }

        /// <summary>
        /// 个人发票的内容
        /// </summary>
        public string PersonalInvoiceContent { get; set; }

        /// <summary>
        /// 增值税发票的公司名称
        /// </summary>
        public string VATInvoiceCompanyName { get; set; }

        /// <summary>
        /// 增值税发票的纳税人识别号
        /// </summary>
        public string VATInvoiceTaxPayerNo { get; set; }

        /// <summary>
        /// 增值税发票的公司注册地址
        /// </summary>
        public string VATInvoiceRegisteredAddress { get; set; }

        /// <summary>
        /// 增值税发票的公司注册电话
        /// </summary>
        public string VATInvoiceRegisteredPhone { get; set; }

        /// <summary>
        /// 增值税发票的公司开户银行
        /// </summary> 
        public string VATInvoiceCompanyBank { get; set; }

        /// <summary>
        /// 增值税发票的公司开户银行账户
        /// </summary>
        public string VATInvoiceCompanyBankAcct { get; set; }

        public override ExtensibleObject CloneObject()
        {
            return new ReceiptInfo()
            {
                IsVATInvoice = this.IsVATInvoice,
                PersonalInvoiceContent = this.PersonalInvoiceContent,
                PersonalInvoiceTitle = this.PersonalInvoiceTitle,
                VATInvoiceCompanyBank = this.VATInvoiceCompanyBank,
                VATInvoiceCompanyBankAcct = this.VATInvoiceCompanyBankAcct,
                VATInvoiceCompanyName = this.VATInvoiceCompanyName,
                VATInvoiceRegisteredAddress = this.VATInvoiceRegisteredAddress,
                VATInvoiceRegisteredPhone = this.VATInvoiceRegisteredPhone,
                VATInvoiceTaxPayerNo = this.VATInvoiceTaxPayerNo
            };
        }
    }
}
