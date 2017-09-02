using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.DAL;
using IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.Model;
using IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.Biz;


namespace IPPOversea.Invoicemgmt.Biz
{
    public static class MonitorBP
    {
        #region Field
        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static Mutex mut = new Mutex(false);
        private static ILog log = LogerManger.GetLoger();
        // private static int AllOK = 0;
        #endregion

        public static void DoWork(AutoResetEvent are)
        {
            try
            {
                int count = 0;
                decimal diffmoney = 0.10m;
                decimal soTotal = 0;
                decimal ar =0;
                decimal invocie=0;
                string status = "A";
                DateTime begindate = DateTime.Parse(Settings.SOBeginDate);
                List<SOEntity> SOList = MonitorDA.GetSO(begindate, Settings.CompanyCode);
                ShowInfo(string.Format("找到{0}个订单", SOList.Count));
                string[] PayByJF = Settings.PayByJiFen;
                string simcard = MonitorDA.GetSIMCard();
                foreach (var item in SOList)
                {
                    soTotal = item.CashPay
                        + item.DiscountAmt
                        + item.PayPrice
                        + item.PremiumAmt
                        + item.ShipPrice
                        - item.GiftCardPay;
                    ar = MonitorDA.GetAR(item.SysNo);
                    if (item.SOType == 6)
                    {
                        invocie = MonitorDA.GetInvoice(item.SysNo) + MonitorDA.GetSIMCardPrice(item.SysNo, simcard);
                    }
                    else
                    {
                        invocie = MonitorDA.GetInvoice(item.SysNo);
                    }
                    status = "A";

                    //Modified by Nolan.Q.Chen in 2012.1.30 修改了模式六的Invoice金额校验
                    if (PayByJF.Contains(item.PayType.ToString()))
                    {
                        if ((item.StockType == "NEG"||item.InvoiceType == "NEG") && ar!=invocie)
                        {
                            count++;
                            MailBP.SendMail("积分支付",item.SysNo, soTotal, ar, invocie);
                            ShowInfo(string.Format("订单[{0}]为【积分支付】，不符合要求，已发送邮件", item.SysNo));
                            status = "D";
                        }
                    }
                    else
                    {
                        if (item.StockType != "NEG" && item.InvoiceType != "NEG")
                        {
                            if (Math.Abs((soTotal - ar)) >= diffmoney || Math.Abs(invocie) >= diffmoney)
                            {
                                count++;
                                MailBP.SendMail("模式六非积分支付", item.SysNo, soTotal, ar, invocie);
                                ShowInfo(string.Format("订单[{0}]为【非积分支付】，不符合要求，已发送邮件", item.SysNo));
                                status = "D";
                            }
                        }
                        else if (Math.Abs((soTotal - ar)) >= diffmoney ||
                                Math.Abs((soTotal - invocie)) >= diffmoney||
                                Math.Abs(ar-invocie)>=diffmoney)
                        {
                            count++;
                            MailBP.SendMail("非积分支付", item.SysNo, soTotal, ar, invocie);
                            ShowInfo(string.Format("订单[{0}]为【非积分支付】，不符合要求，已发送邮件", item.SysNo));
                            status = "D";
                        }
                    }              
                    MonitorDA.CreateMonitorLog(item.SysNo, soTotal, ar, invocie, status);
                }
                ShowInfo(string.Format("共有{0}个订单不符合要求",count));
                are.Set();

            }
            catch (Exception ex)
            {
                are.Set();
                throw ex;
            }
        }
    }
}
