using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.ETPCalculator
{
    public class CalcInputData
    {
        /// <summary>
        /// 账期类型
        /// </summary>
        public VendorPayPeriodType PayPeriodType
        {
            get;
            set;
        }

        public CalcInputData(VendorPayPeriodType payPeriodType)
        {
            PayPeriodType = payPeriodType;
        }
    }
}