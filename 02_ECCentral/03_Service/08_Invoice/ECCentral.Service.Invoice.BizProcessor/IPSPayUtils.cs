using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.EBank;
using ECCentral.Service.Invoice.BizProcessor.IPSPay;
using ECCentral.Service.Utility;
using System.Threading;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.BizEntity;

namespace ECCentral.Service.Invoice.BizProcessor
{
    public class IPSPayUtils
    {
        public RefundResult Refund(RefundEntity refundEntity)
        {
            RefundResult result = new RefundResult();
            result.IsSync = true;
            NetPayInfo netpayInfo = ObjectFactory<NetPayProcessor>.Instance.GetValidBySOSysNo(refundEntity.SOSysNo);
            string partner = AppSettingManager.GetSetting("Invoice", "IPSPartner");
            string IPSSecurityKey = AppSettingManager.GetSetting("Invoice", "IPSSecurityKey");
            string sign = Sign(string.Format("{0}{1}{2}{3}{4}", partner, refundEntity.SOSysNo.ToString(), refundEntity.OrderDate.ToString("yyyyMMdd"), refundEntity.RefundAmt.ToString("f2"), IPSSecurityKey));
            RefundMsg msg = new IPSPay.ServiceSoapClient().Refund(partner, sign, refundEntity.OrderDate.ToString("yyyyMMdd"), refundEntity.SOSysNo.ToString(), (double)refundEntity.RefundAmt, "");
            if (msg.ErrCode == "0000")
            {
                if (msg.Sign == Sign(string.Format("{0}{1}{2}{3}{4}{5}{6}", partner, msg.TradeBillNo, msg.TradeTime, msg.RealRefundAmount.ToString("f2"), msg.CanRefundAmount.ToString("f2"), msg.RefundBillNo, IPSSecurityKey)))
                {
                    result.Result = true;
                }
                else
                {
                    result.Result = false;
                    result.Message = "验签失败";
                }
            }
            else
            {
                result.Result = false;
                result.Message = GetMessage(msg.ErrCode);
                //result.Message += refundEntity.SOSysNo.ToString() + " " + refundEntity.OrderDate.ToString("YYYYMMDD");
            }

            return result;

        }

        /// <summary>
        /// 对账
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool SyncTradeBill(string date)
        {
            string partner = AppSettingManager.GetSetting("Invoice", "IPSPartner");
            string IPSSecurityKey = AppSettingManager.GetSetting("Invoice", "IPSSecurityKey");
            
            //环宇支付每分钟允许掉用10次
            int allowTimePer = 10;

            string startTime = DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "000000";
            string endTime = DateTime.Now.ToString("yyyyMMdd") + "235959";

            int page = 0;
            int size = 100;

            bool complete = false;

            do
            {
                page++;
                string sign = Sign(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", partner, 1, "NT", startTime, endTime, page, size, IPSSecurityKey));
                OrderMsg msg = new ServiceSoapClient().GetOrderByTime(partner, sign, 1, "NT", startTime, endTime, page, size);
                if (msg.ErrCode.Equals("0000"))
                {
                    if (msg.Total <= (page - 1) * size + msg.Count)
                    {
                        complete = true;
                    }

                    foreach (var temp in msg.OrderRecords)
                    {
                        string resign = Sign(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", temp.OrderNo, temp.IPSOrderNo, temp.Trd_Code, temp.Cr_Code, temp.Amount.ToString("f2"), temp.MerchantOrderTime, temp.IPSOrderTime, temp.Flag, IPSSecurityKey));
                        if (temp.Sign.Equals(resign))
                        {
                            TransactionCheckBill bill = new TransactionCheckBill()
                            {
                                //TransactionType = CheckTransactionType.P,
                                //SoSysNo = int.Parse(temp.OrderNo),
                                //SerialNo = string.Format("{0}{1}", itemArray[0].Equals("P") ? "P" : "R", itemArray[2]),
                                //SubOrderTime = itemArray[3],
                                //ProcessingTime = itemArray[4],
                                //TotalAmount = decimal.Parse(itemArray[5]),
                                //ProductAmount = decimal.Parse(itemArray[6]),
                                //ForexCurrency = itemArray[7],
                                //ForexAmount = decimal.Parse(itemArray[8]),
                                //Tariff = decimal.Parse(itemArray[10]),
                                //FareAmount = decimal.Parse(itemArray[11]),
                                //FareCurrency = itemArray[12],
                                //SubtotalAmount = decimal.Parse(itemArray[13]),
                                //MerchantName = itemArray[14]
                            };
                            bill.TransactionType = (CheckTransactionType)1;
                            int _OrderNo = 0;
                            if (int.TryParse(temp.OrderNo, out _OrderNo))
                            {
                                bill.SoSysNo = _OrderNo;
                            }
                            else
                            {
                                continue;
                            }
                            
                            bill.SerialNo = temp.IPSOrderNo;
                            bill.SubOrderTime = temp.MerchantOrderTime;
                            bill.ProcessingTime = temp.IPSOrderTime;
                            bill.TotalAmount = (decimal)temp.Amount;
                            bill.ForexCurrency = temp.Cr_Code;
                            ObjectFactory<IInvoiceDA>.Instance.InsertTransactionCheckBill(bill);
                        }
                    }
                }
                else
                {
                    //return getTradeBillReturnMessage(msg.ErrCode);
                    throw new BizException(getTradeBillReturnMessage(msg.ErrCode));
                }

                if (page % allowTimePer == 0)
                {
                    Thread.Sleep(60000);
                }

            } while (!complete);

            return true;
        }

        /// <summary>
        /// 对账message
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        internal static string getTradeBillReturnMessage(string errorCode)
        {
            switch (errorCode)
            {
                case "0000":
                    return "查询成功";
                case "1001":
                    return "身份验证失败,商户不存在";
                case "1002":
                    return "商户证书不存在";
                case "1003":
                    return "商户签名验证不通过";
                case "1004":
                    return "商户输入的IPS订单时间格式不合法";
                case "1005":
                    return "开始时间>结束时间";
                case "1006":
                    return "输入时间为空";
                case "1007":
                    return "调用服务失败";
                case "1008":
                    return "没有满足条件的记录";
                case "1009":
                    return "商户订单号不合法";
                case "1010":
                    return "开始订单号>结束订单号";
                case "2000":
                    return "合同过期";
                default:
                    return "未知的错误代码";
            }
        }

        /// <summary>
        /// 退款message
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static string GetMessage(string code)
        {
            switch (code)
            {
                case "1001":
                    return "商户不存在";
                case "1002":
                    return "商户证书不存在";
                case "1003":
                    return "商户签名验证不通过";
                case "1004":
                    return "输入的交易有误";
                case "1005":
                    return "输入的交易时间有误";
                case "1006":
                    return "找不到这笔交易";
                case "1007":
                    return "不是您的交易";
                case "1008":
                    return "不是您的消费交易";
                case "1009":
                    return "退款金额过大";
                case "1010":
                    return "退款金额过低";
                case "1011":
                    return "退款失败，请联系环迅技术支持";
                case "1012":
                    return "无法找到IPS订单号";
                case "1013":
                    return "无法找到 BankTypeBankType";
                case "1021":
                    return "商户号不能为空";
                case "1022":
                    return "签名数据不能为空";
                case "1023":
                    return "商户时间不能为空";
                case "1024":
                    return "商户订单号不能为空";
                case "1025":
                    return "退款金额不能为空";
                case "1026":
                    return "商户号格式不正确";
                case "1027":
                    return "商户时间格式不正确 ";
                case "1028":
                    return "退款金额格式不正确";
                case "1029":
                    return "退款备注字符最多";
                case "1111":
                    return "未知错误，请联系环迅技术支持";
                default:
                    return "未知的错误代码";
            }
        }
        internal static string Sign(string prestr)
        {
            StringBuilder sb = new StringBuilder(32);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.Default.GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }
    }
}
