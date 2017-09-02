using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.ETPCalculator
{
    /// <summary>
    /// 经销预计付款日计算输入值
    /// </summary>
    public class SellCalcInputData : CalcInputData
    {
        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime EnterDatabaseDate
        {
            get;
            set;
        }
        /// <summary>
        /// 录入发票日期
        /// </summary>
        public DateTime EnterInvoiceDate
        {
            get;
            set;
        }

        public SellCalcInputData(DateTime enterDatabaseDate, DateTime enterInvoiceDate, VendorPayPeriodType payPeriodType)
            : base(payPeriodType)
        {
            this.EnterDatabaseDate = enterDatabaseDate;
            this.EnterInvoiceDate = enterInvoiceDate;
        }
    }
}