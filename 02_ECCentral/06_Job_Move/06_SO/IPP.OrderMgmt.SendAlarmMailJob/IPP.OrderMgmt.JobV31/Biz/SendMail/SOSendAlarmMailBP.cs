using System;
using System.Collections.Generic;
using System.Configuration;
using IPP.OrderMgmt.JobV31.BusinessEntities.SendMail;
using IPP.OrderMgmt.JobV31.Dac.SendMail;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Biz.SendMail
{
    public class SOSendAlarmMailBP
    {
        public static string Check(JobContext jobContext)
        {
            string CompanyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            string messageInfo = string.Empty;
            if (DateTime.Now.DayOfWeek.ToString().Trim().Equals("Monday"))
            {
                List<MoreThanTenDaysOrderInfoEntity> UnauditedOrNotOutStockList = SendAlarmMailDA.GetMoreThanTenDaysUnauditedOrNotOutStockOrderInfoList(CompanyCode);
                if (UnauditedOrNotOutStockList.Count > 0)
                {
                    #region 发送超过10天未审核提醒邮件
                    List<MoreThanTenDaysOrderInfoEntity> UnauditedList = UnauditedOrNotOutStockList.FindAll(x => { return (x.OrderStatus == 0 || x.OrderStatus == 3 || x.OrderStatus == 2); });                               
                    if (UnauditedList.Count > 0)  //判断是否发送超过10天未审核邮件提醒
                    {
                        string strMailFrom = ConfigurationManager.AppSettings["UnauditedMailFrom"].ToString();
                        string strMailAddress = ConfigurationManager.AppSettings["UnauditedMailTo"].ToString();
                        string strCCMailAddress = ConfigurationManager.AppSettings["UnauditedMailCC"].ToString();
                        if (UnauditedList.Count <= 250)
                        {                            
                            string strMailSubject = "超过10天未审核订单明细";
                            string strMailTitle = "请关注以下超过10天未审核的订单，请尽快处理";
                            Boolean sendResult = SendAlarmMailDA.SendAlarmMail(UnauditedList,strMailFrom, strMailAddress,strCCMailAddress, strMailSubject, strMailTitle);
                            if (sendResult)
                            {
                                messageInfo = messageInfo + "\n" + "超时10天未审核订单邮件已经发送至 " + strMailAddress + " 请注意查收";
                            }
                        }
                        else
                        {                       
                            int startIndex = 0;
                            int listCount = 250;
                            List<MoreThanTenDaysOrderInfoEntity> UnauditedListPart;
                            int partIndex = 1;
                            for (int i = 0; i <  UnauditedList.Count / 250; i++)
                            {
                                UnauditedListPart = UnauditedList.GetRange(startIndex, listCount);               
                                string strMailSubject = "超过10天未审核订单明细 Part:" + partIndex;
                                string strMailTitle = "请关注以下超过10天未审核的订单 Part:" + partIndex+ "，请尽快处理";
                                Boolean sendResult = SendAlarmMailDA.SendAlarmMail(UnauditedListPart,strMailFrom, strMailAddress,strCCMailAddress,strMailSubject, strMailTitle);
                                if (sendResult)
                                {
                                    messageInfo = messageInfo + "\n" + "超时10天未审核订单邮件 Part:" + partIndex + " 已经发送至" + strMailAddress + "  请注意查收";
                                }
                                startIndex = startIndex+250;
                                partIndex = partIndex + 1;
                            }
                            if (startIndex < UnauditedList.Count)
                            {
                                UnauditedListPart = UnauditedList.GetRange(startIndex, UnauditedList.Count - startIndex);
                               
                                string strMailSubject = "超过10天未审核订单明细 Part:" + partIndex;
                                string strMailTitle = "请关注以下超过10天未审核的订单 Part:" + partIndex + "，请尽快处理";
                                Boolean sendResult = SendAlarmMailDA.SendAlarmMail(UnauditedListPart,strMailFrom, strMailAddress,strCCMailAddress, strMailSubject, strMailTitle);
                                if (sendResult)
                                {
                                    messageInfo = messageInfo + "\n" + "超时10天未审核订单邮件 Part:" + partIndex + " 已经发送至" + strMailAddress + "  请注意查收";
                                }
                            }
                        }                       
                    }
                    #endregion
                   
                    #region 发送超过10天未出库提醒邮件
                    List<MoreThanTenDaysOrderInfoEntity> NotOutStockList = UnauditedOrNotOutStockList.FindAll(x => { return x.OrderStatus == 1; });
                    if (NotOutStockList.Count > 0)//判断是否发送超过10天待出库邮件提醒
                    {
                        string strMailFrom = ConfigurationManager.AppSettings["NotOutStockMailFrom"].ToString();
                        string strMailAddress = ConfigurationManager.AppSettings["NotOutStockMailTo"].ToString();
                        string strCCMailAddress = ConfigurationManager.AppSettings["NotOutStockMailCC"].ToString();
                        if (NotOutStockList.Count <= 250)
                        {                          
                            string strMailSubject = "超过10天未出库订单明细";
                            string strMailTitle = "请关注以下超过10天未出库的订单，请尽快处理";
                            Boolean sendResult = SendAlarmMailDA.SendAlarmMail(NotOutStockList,strMailFrom, strMailAddress,strCCMailAddress, strMailSubject, strMailTitle);
                            if (sendResult)
                            {
                                if (messageInfo != string.Empty)
                                {
                                    messageInfo = messageInfo + "\n" + "超时10天未出库邮件已经发送至 " + strMailAddress + " 请注意查收";
                                }
                            }
                        }
                        else
                        {
                            int startIndex = 0;
                            int listCount = 250;
                            List<MoreThanTenDaysOrderInfoEntity> NotOutStockListPart;
                            int partIndex = 1;
                            for (int i = 0; i < UnauditedList.Count / 250; i++)
                            {
                                NotOutStockListPart = NotOutStockList.GetRange(startIndex, listCount);                               
                                string strMailSubject = "超过10天未审核订单明细 Part:" + partIndex;
                                string strMailTitle = "请关注以下超过10天未审核的订单 Part:" + partIndex + "，请尽快处理";
                                Boolean sendResult = SendAlarmMailDA.SendAlarmMail(NotOutStockList, strMailFrom, strMailAddress, strCCMailAddress, strMailSubject, strMailTitle);
                                if (sendResult)
                                {
                                    messageInfo = messageInfo + "\n" + "超时10天未出库邮件 Part:" + partIndex + " 已经发送至 " + strMailAddress + "  请注意查收";
                                }
                                startIndex = startIndex + 250;
                                partIndex = partIndex + 1;
                            }
                            if (startIndex < UnauditedList.Count)
                            {
                                NotOutStockListPart = NotOutStockList.GetRange(startIndex, UnauditedList.Count - startIndex);                               
                                string strMailSubject = "超过10天未审核订单明细 Part:" + partIndex;
                                string strMailTitle = "请关注以下超过10天未审核的订单 Part:" + partIndex + "，请尽快处理";
                                Boolean sendResult = SendAlarmMailDA.SendAlarmMail(NotOutStockList, strMailFrom, strMailAddress, strCCMailAddress, strMailSubject, strMailTitle);
                                if (sendResult)
                                {
                                    messageInfo = messageInfo + "\n" + "超时10天未出库邮件 Part:" + partIndex + " 已经发送至" + strMailAddress + "请注意查收";
                                }
                            }
                        }                     
                    }
                    #endregion
                }
            }
            else
            {
                messageInfo = "今天不是周一 不发送提醒邮件";
            }
            return messageInfo;
        }
    }
}
