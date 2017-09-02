using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.ETPCalculator
{
    /// <summary>
    /// 代销预计付款日计算类
    /// </summary>
    public class ConsignmentETPCalculator : ETPCalculatorBase, IETPCalculator
    {
        #region IETPCalculator Members

        public override DateTime? Calculate()
        {
            /*代销预计付款日(ETP)计算：
              * 月结，每月25日，且票到（系统）：
                 定义：结算时间截止每月20日，时间区间为3个月，录入发票的日期为B，
                     1）若B>25号，到期日=B+1；
                     2）若B<=25号，到期日=每月25日；

              * 月结，每月10日，且票到（系统）：
                 定义：结算时间截止每月5日，时间区间为3个月，录入发票的日期为B，
                     1）若B>10号，到期日=B+1；
                     2）若B<=10号，到期日=每月10日；

              * 半月结，每月10/25日，且票到（系统）：
                 定义：结算时间截止每月5/20日，录入发票的日期为B，
                     1）B < 10号       ： 到期日 = 本月10号
                     2）10号 <= B<25号 ： 到期日 = 本月25号
                     3）B >= 25号      ： 到期日 = 下个月10号

              * 手工结算代销:
                 定义：人工建立代销结算单，录入发票日期为B，到期日=B+1；

              * 预计付款日说明：
                 1）如果到期日是周六，付款日=到期日+2日；
                 2）如果到期日是周日，付款日=到期日+1日；
                 3）如果到期日是周三，付款日=到期日-1日，提前到周二；
                 4）如果到期日为非周三、周六和周日，付款日=到期日；
              */

            //1.计算账期
            DateTime? resultDate = null;  //供应商代销账期（时间）
            VendorPayPeriodType payPeriodType = InputData.PayPeriodType;
            DateTime enterInvoiceDate = InputData.EnterInvoiceDate;
            DateTime compareEnterInvoiceDate = new DateTime(enterInvoiceDate.Year, enterInvoiceDate.Month, enterInvoiceDate.Day);

            //待修改
            switch (payPeriodType)
            {
                case VendorPayPeriodType.InvoiceInNow:
                case VendorPayPeriodType.InvoiceIn2:
                case VendorPayPeriodType.InvoiceIn3:
                case VendorPayPeriodType.InvoiceIn4:
                case VendorPayPeriodType.InvoiceIn5:
                case VendorPayPeriodType.InvoiceIn6:
                case VendorPayPeriodType.InvoiceIn7:
                case VendorPayPeriodType.InvoiceIn8:
                case VendorPayPeriodType.InvoiceIn9:
                case VendorPayPeriodType.InvoiceIn10:
                case VendorPayPeriodType.InvoiceIn14:
                case VendorPayPeriodType.InvoiceIn15:
                case VendorPayPeriodType.InvoiceIn18:
                case VendorPayPeriodType.InvoiceIn20:
                case VendorPayPeriodType.InvoiceIn25:
                case VendorPayPeriodType.InvoiceIn30:
                case VendorPayPeriodType.InvoiceIn45:
                case VendorPayPeriodType.InvoiceIn60:
                    resultDate = enterInvoiceDate.AddDays(GetItemInAfterDay(payPeriodType));
                    break;
                default:
                    return DateTime.Parse("1900-01-01");
            }

            resultDate = this.ConvertWorkDate(resultDate);

            return resultDate;
        }

        #endregion IETPCalculator Members

        private ConsignmentCalcInputData InputData
        {
            get;
            set;
        }

        public ConsignmentETPCalculator(ConsignmentCalcInputData inputData)
        {
            this.InputData = inputData;
        }
    }
}
