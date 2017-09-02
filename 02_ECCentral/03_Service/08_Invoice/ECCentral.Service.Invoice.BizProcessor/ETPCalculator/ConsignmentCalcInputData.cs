using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.ETPCalculator
{
    /// <summary>
    /// 代销预计付款日计算输入值
    /// </summary>
    public class ConsignmentCalcInputData : CalcInputData
    {
        /// <summary>
        /// 录入发票日期
        /// </summary>
        public DateTime EnterInvoiceDate
        {
            get;
            set;
        }

        public ConsignmentCalcInputData(DateTime enterInvoiceDate, VendorPayPeriodType payPeriodType)
            : base(payPeriodType)
        {
            this.EnterInvoiceDate = enterInvoiceDate;
        }
    }
}