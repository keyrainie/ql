using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor.ETPCalculator
{
    /// <summary>
    /// 预计付款时间计算类
    /// </summary>
    public static class ETPCalculatorHelper
    {
        /// <summary>
        /// 获取ETP时间
        /// </summary>
        /// <param name="pay">应付款实体类</param>
        /// <param name="payTime">录入发票时间</param>
        /// <returns>ETP计算结果</returns>
        public static DateTime? GetETPByPayPeriod(PayableInfo pay, DateTime payTime)
        {
            if (payTime == DateTime.MinValue)
            {
                payTime = DateTime.Parse("1900-01-01");
            }

            DateTime enterDatabaseDate = DateTime.MinValue; //PO入库时间
            int payTypeSysNo = -1;                          //账期类型
            IETPCalculator calculator = null;               //ETP计算器

            switch (pay.OrderType)
            {
                case PayableOrderType.PO:
                    var poInfo = ExternalDomainBroker.GetPurchaseOrderInfo(pay.OrderSysNo.Value, pay.BatchNumber.Value);
                    if (poInfo == null)
                    {
                        //throw new BizException("未找到PO单");
                        ThrowBizException("POVendorInvoice_PONotFound");
                    }
                    //如果是负PO（入库金额<0）,EGP和ETP不用计算都为入库时间。
                    if (poInfo.PurchaseOrderBasicInfo.TotalAmt < 0)
                    {
                        return ETPCalculatorHelper.ConvertWorkDate(poInfo.PurchaseOrderBasicInfo.CreateDate);
                    }
                    //PO代销采购单入库不计算ETP
                    if (poInfo.PurchaseOrderBasicInfo.ConsignFlag == PurchaseOrderConsignFlag.Consign)
                    {
                        return null;
                    }

                    if (!poInfo.PurchaseOrderBasicInfo.CreateDate.HasValue)
                    {
                        poInfo.PurchaseOrderBasicInfo.CreateDate = DateTime.Parse("1900-01-01");
                    }
                    enterDatabaseDate = poInfo.PurchaseOrderBasicInfo.CreateDate.Value;
                    payTypeSysNo = poInfo.PurchaseOrderBasicInfo.PayType.SysNo.Value;

                    //不计算票到的情况(payTime 为最小值表示 是PO入库)
                    if (payTime == DateTime.Parse("1900-01-01"))
                    {
                        switch ((VendorPayPeriodType)payTypeSysNo)
                        {
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
                            //月结
                            case VendorPayPeriodType.ItemInPayByMonth:
                            //预付账期【款到】
                            case VendorPayPeriodType.MoneyInItemOut:
                            case VendorPayPeriodType.MoneyInItemOut60:
                            case VendorPayPeriodType.MoneyInItemOut90:
                                return null;
                        }
                    }

                    //创建经销预计的ETP计算器
                    calculator = new SellETPCalculator(new SellCalcInputData(enterDatabaseDate, payTime, (VendorPayPeriodType)payTypeSysNo));
                    break;
                case PayableOrderType.VendorSettleOrder:    //代销结算单
                    var consignSettlementInfo = ExternalDomainBroker.GetConsignSettlementInfo(pay.OrderSysNo.Value);
                    if (consignSettlementInfo == null)
                    {
                        //throw new BizException("未找到代销结算单");
                        ThrowBizException("POVendorInvoice_VendorSettleOrderNotFound");
                    }
                    payTypeSysNo = consignSettlementInfo.VendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value;

                    //创建代销预计的ETP计算器
                    calculator = new ConsignmentETPCalculator(new ConsignmentCalcInputData(payTime, (VendorPayPeriodType)payTypeSysNo));
                    break;
                case PayableOrderType.CollectionSettlement:    //代收结算单
                    var collectionSettlement = ExternalDomainBroker.GetGatherSettlementInfo(pay.OrderSysNo.Value);
                    if (collectionSettlement == null)
                    {
                        //throw new BizException("未找到代收结算单");
                        ThrowBizException("POVendorInvoice_CollectionSettlementNotFound");
                    }
                    collectionSettlement.VendorInfo = ExternalDomainBroker.GetVendorBasicInfo(collectionSettlement.VendorInfo.SysNo.Value);
                    payTypeSysNo = collectionSettlement.VendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value;

                    //创建代销预计的ETP计算器
                    calculator = new ConsignmentETPCalculator(new ConsignmentCalcInputData(payTime, (VendorPayPeriodType)payTypeSysNo));
                    break;
                //代收代付 2012.11.28
                case PayableOrderType.CollectionPayment:
                    CollectionPaymentInfo poCollectionPaymentEntity = ExternalDomainBroker.GetCollectionPaymentInfo(pay.OrderSysNo.Value);
                    if (poCollectionPaymentEntity == null)
                    {
                        //throw new BizException("未找到代收代付结算单");CollectionPayment
                        ThrowBizException("POVendorInvoice_CollectionPaymentNotFound");
                    }
                    //createDate = poConsignSettleEntity.CreateTime.Value;
                    //createDate = DateTime.Now;  //代销结算单的结算时间（结算单的生成时间）
                    //payTypeSysNo = poCollectionPaymentEntity.PayTypeSysNo;
                    ////创建代销预估的EGP输入参数对象
                    //inputData = new EgpConsignmentInputData(createDate, (VendorPayPeriodType)payTypeSysNo);
                    poCollectionPaymentEntity.VendorInfo = ExternalDomainBroker.GetVendorBasicInfo(poCollectionPaymentEntity.VendorInfo.SysNo.Value);
                    payTypeSysNo = poCollectionPaymentEntity.VendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value;
                    calculator = new ConsignmentETPCalculator(new ConsignmentCalcInputData(payTime, (VendorPayPeriodType)payTypeSysNo));
                    break;
                default:
                    return null;
            }
            return calculator.Calculate();
        }

        /// <summary>
        /// 转换时间（调整周三、六、日的时间）---【可以重写】
        /// </summary>
        /// <param name="time">需要转换的时间</param>
        /// <returns>转换后的时间</returns>
        public static DateTime? ConvertWorkDate(DateTime? time)
        {
            /* 预计付款日说明：
                1）如果到期日是周六，付款日=到期日+2日；
                2）如果到期日是周日，付款日=到期日+1日；
                3）如果到期日是周三，付款日=到期日-1日，提前到周二；
                4）如果到期日为非周三、周六和周日，付款日=到期日；
             */
            if (time == null)
            {
                return null;
            }

            DateTime resultDate = Convert.ToDateTime(time);
            switch (resultDate.DayOfWeek)
            {
                case DayOfWeek.Wednesday:
                    resultDate = resultDate.AddDays(-1);
                    break;
                case DayOfWeek.Saturday:
                    resultDate = resultDate.AddDays(2);
                    break;
                case DayOfWeek.Sunday:
                    resultDate = resultDate.AddDays(1);
                    break;
            }

            return resultDate;
        }

        /// <summary>
        /// 获取到货时间（发票到和发票未到）
        /// </summary>
        /// <returns>天数信息</returns>
        public static int GetItemInAfterDay(VendorPayPeriodType payPeriodType)
        {
            int resultDayNum = -1;

            //货到N天
            switch (payPeriodType)
            {
                //货到，有票
                case VendorPayPeriodType.ItemInPayIn:
                    resultDayNum = 0;
                    break;
                case VendorPayPeriodType.ItemInAfter2PayIn:
                    resultDayNum = 2;
                    break;
                case VendorPayPeriodType.ItemInAfter3PayIn:
                    resultDayNum = 3;
                    break;
                case VendorPayPeriodType.ItemInAfter4PayIn:
                    resultDayNum = 4;
                    break;
                case VendorPayPeriodType.ItemInAfter5PayIn:
                    resultDayNum = 5;
                    break;
                case VendorPayPeriodType.ItemInAfter6PayIn:
                    resultDayNum = 6;
                    break;
                case VendorPayPeriodType.ItemInAfter7PayIn:
                    resultDayNum = 7;
                    break;
                case VendorPayPeriodType.ItemInAfter8PayIn:
                    resultDayNum = 8;
                    break;
                case VendorPayPeriodType.ItemInAfter9PayIn:
                    resultDayNum = 9;
                    break;
                case VendorPayPeriodType.ItemInAfter10PayIn:
                    resultDayNum = 10;
                    break;
                case VendorPayPeriodType.ItemInAfter14PayIn:
                    resultDayNum = 14;
                    break;
                case VendorPayPeriodType.ItemInAfter18PayIn:
                    resultDayNum = 18;
                    break;
                case VendorPayPeriodType.ItemInAfter20PayIn:
                    resultDayNum = 20;
                    break;
                case VendorPayPeriodType.ItemInAfter25PayIn:
                    resultDayNum = 25;
                    break;
                case VendorPayPeriodType.ItemInAfter30PayIn:
                    resultDayNum = 30;
                    break;
                case VendorPayPeriodType.ItemInAfter45PayIn:
                    resultDayNum = 45;
                    break;
                case VendorPayPeriodType.ItemInAfter60PayIn:
                    resultDayNum = 60;
                    break;

                //货到，无票
                case VendorPayPeriodType.ItemInMoneyIn:
                    resultDayNum = 0;
                    break;
                case VendorPayPeriodType.ItemInAfter3:
                    resultDayNum = 3;
                    break;
                case VendorPayPeriodType.ItemInAfter7:
                    resultDayNum = 7;
                    break;
                case VendorPayPeriodType.ItemInAfter14:
                    resultDayNum = 14;
                    break;
                case VendorPayPeriodType.ItemInAfter20:
                    resultDayNum = 20;
                    break;
                case VendorPayPeriodType.InvoiceInNow:
                    resultDayNum = 0;
                    break;
                case VendorPayPeriodType.InvoiceIn2:
                    resultDayNum = 2;
                    break;
                case VendorPayPeriodType.InvoiceIn3:
                    resultDayNum = 3;
                    break;
                case VendorPayPeriodType.InvoiceIn4:
                    resultDayNum = 4;
                    break;
                case VendorPayPeriodType.InvoiceIn5:
                    resultDayNum = 5;
                    break;
                case VendorPayPeriodType.InvoiceIn6:
                    resultDayNum = 6;
                    break;
                case VendorPayPeriodType.InvoiceIn7:
                    resultDayNum = 7;
                    break;
                case VendorPayPeriodType.InvoiceIn8:
                    resultDayNum = 8;
                    break;
                case VendorPayPeriodType.InvoiceIn9:
                    resultDayNum = 9;
                    break;
                case VendorPayPeriodType.InvoiceIn10:
                    resultDayNum = 10;
                    break;
                case VendorPayPeriodType.InvoiceIn14:
                    resultDayNum = 14;
                    break;
                case VendorPayPeriodType.InvoiceIn15:
                    resultDayNum = 15;
                    break;
                case VendorPayPeriodType.InvoiceIn18:
                    resultDayNum = 18;
                    break;
                case VendorPayPeriodType.InvoiceIn20:
                    resultDayNum = 20;
                    break;
                case VendorPayPeriodType.InvoiceIn25:
                    resultDayNum = 25;
                    break;
                case VendorPayPeriodType.InvoiceIn30:
                    resultDayNum = 30;
                    break;
                case VendorPayPeriodType.InvoiceIn45:
                    resultDayNum = 45;
                    break;
                case VendorPayPeriodType.InvoiceIn60:
                    resultDayNum = 60;
                    break;
                default:
                    resultDayNum = -1;
                    break;
            }
            return resultDayNum;
        }

        #region Helper Methods

        private static  void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private static string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.POVendorInvoice, msgKeyName), args);
        }

        #endregion Helper Methods
    }
}