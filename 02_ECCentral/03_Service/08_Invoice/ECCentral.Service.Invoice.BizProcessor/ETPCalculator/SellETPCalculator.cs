using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.ETPCalculator
{
    /// <summary>
    /// 经销预计付款日计算类
    /// </summary>
    public class SellETPCalculator : ETPCalculatorBase, IETPCalculator
    {
        #region IETPCalculator Members

        public override DateTime? Calculate()
        {
            DateTime? resultDate = null;  //供应商代销账期（时间）
            int afterDayNum = -1;

            //录入发票时间
            DateTime enterInvoiceDate;
            DateTime compareEnterInvoiceDate;
            //分类计算账期
            switch (InputDat.PayPeriodType)
            {
                //款到发货（预付），银行汇票（60或90）
                case VendorPayPeriodType.MoneyInItemOut:
                case VendorPayPeriodType.MoneyInItemOut60:
                case VendorPayPeriodType.MoneyInItemOut90:
                    resultDate = InputDat.EnterInvoiceDate;
                    break;
                //货到后N天，且票到
                case VendorPayPeriodType.ItemInPayIn:
                case VendorPayPeriodType.ItemInAfter2PayIn:
                case VendorPayPeriodType.ItemInAfter3PayIn:
                case VendorPayPeriodType.ItemInAfter4PayIn:
                case VendorPayPeriodType.ItemInAfter5PayIn:
                case VendorPayPeriodType.ItemInAfter6PayIn:
                case VendorPayPeriodType.ItemInAfter7PayIn:
                case VendorPayPeriodType.ItemInAfter8PayIn:
                case VendorPayPeriodType.ItemInAfter9PayIn:
                case VendorPayPeriodType.ItemInAfter10PayIn:
                case VendorPayPeriodType.ItemInAfter14PayIn:
                case VendorPayPeriodType.ItemInAfter18PayIn:
                case VendorPayPeriodType.ItemInAfter20PayIn:
                case VendorPayPeriodType.ItemInAfter25PayIn:
                case VendorPayPeriodType.ItemInAfter30PayIn:
                case VendorPayPeriodType.ItemInAfter45PayIn:
                case VendorPayPeriodType.ItemInAfter60PayIn:
                    //预计付款定义：入库日期为A，录入发票的日期为B：
                    //    1）若 B > A + N，到期日=B+1；
                    //    2）若B <= A+N, 到期日=A+N；
                    enterInvoiceDate = InputDat.EnterInvoiceDate;
                    compareEnterInvoiceDate = new DateTime(enterInvoiceDate.Year, enterInvoiceDate.Month, enterInvoiceDate.Day);

                    DateTime enterDatabaseDate = InputDat.EnterDatabaseDate;
                    DateTime compareEnterDatabaseDate = new DateTime(enterDatabaseDate.Year, enterDatabaseDate.Month, enterDatabaseDate.Day);

                    afterDayNum = GetItemInAfterDay(InputDat.PayPeriodType);
                    if (DateTime.Compare(compareEnterInvoiceDate, compareEnterDatabaseDate.AddDays(afterDayNum)) > 0)
                    { //若 B > A + N，到期日=B+1
                        resultDate = enterInvoiceDate.AddDays(1);
                    }
                    else
                    {
                        resultDate = enterDatabaseDate.AddDays(afterDayNum);
                    }
                    break;
                //货到N天后付款（无票）
                case VendorPayPeriodType.ItemInMoneyIn:
                case VendorPayPeriodType.ItemInAfter3:
                case VendorPayPeriodType.ItemInAfter7:
                case VendorPayPeriodType.ItemInAfter14:
                case VendorPayPeriodType.ItemInAfter20:
                    //入库日期为A，到期日=A+N；
                    afterDayNum = GetItemInAfterDay(InputDat.PayPeriodType);
                    resultDate = InputDat.EnterDatabaseDate.AddDays(afterDayNum);
                    break;

                //月结
                case VendorPayPeriodType.ItemInPayByMonth:
                    //结算范围为上月全月入库，截止日期月末最后一天A，录入发票的日期为B，
                    //    1）若B >=13号，到期日=B+1号；
                    //    2）若B < 13号，到期日=13号；
                    DateTime tempDate = new DateTime(InputDat.EnterInvoiceDate.Year, InputDat.EnterInvoiceDate.Month, 13);
                    DateTime resultTempDate = new DateTime(InputDat.EnterInvoiceDate.Year, InputDat.EnterInvoiceDate.Month, 13, InputDat.EnterInvoiceDate.Hour, InputDat.EnterInvoiceDate.Minute, InputDat.EnterInvoiceDate.Second);

                    enterInvoiceDate = InputDat.EnterInvoiceDate;
                    compareEnterInvoiceDate = new DateTime(enterInvoiceDate.Year, enterInvoiceDate.Month, enterInvoiceDate.Day);

                    if (DateTime.Compare(compareEnterInvoiceDate, tempDate) > 0)
                    {   //若B >13号，到期日=B+1号；
                        resultDate = enterInvoiceDate.AddDays(1);
                    }
                    else
                    {
                        resultDate = resultTempDate;
                    }
                    break;
                default:
                    return DateTime.Parse("1900-01-01");
            }

            resultDate = this.ConvertWorkDate(resultDate);
            return resultDate;
        }

        #endregion IETPCalculator Members

        private SellCalcInputData InputDat
        {
            get;
            set;
        }

        public SellETPCalculator(SellCalcInputData inputData)
        {
            this.InputDat = inputData;
        }
    }
}
