using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Job.SO.SendAlarmMailJob.BusinessEntities.SendMail;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Biz;
using ECCentral.Job.Utility.Email;
using ECCentral.Job.Utility.Email.DataContracts;
using Newegg.Oversea.Framework.ExceptionBase;

namespace ECCentral.Job.SO.SendAlarmMailJob.Dac.SendMail
{
    public class SendAlarmMailDA
    {
        public static Boolean SendAlarmMail(IList<MoreThanTenDaysOrderInfoEntity> UnauditedOrNotOutStockList, string strMailFrom,
            string strMailAddress, string strCCMailAddress, string strMailSubject, string strMailTitle) {
                EmailInfoEntity mail = new EmailInfoEntity();
                mail.MailFrom = strMailFrom;
                mail.CCMailAddress = strCCMailAddress;
                mail.MailAddress = strMailAddress;
                mail.MailSubject = strMailSubject;
                StringBuilder mailBody = new StringBuilder();
                mailBody.Append(strMailTitle);
                mailBody.Append("<br>");
                mailBody.Append("<table border=\"1\"><tr><td>订单编号</td><td>FP状态</td><td>订单状态</td><td>总金额</td><td>订单时间</td><td>付款方式</td><td>配送方式</td><td>收款状态</td><td>更新人</td><td>是否超过90天</td></tr>");
                if (UnauditedOrNotOutStockList.Count > 0)
                {
                    string CheckoutStatusName = string.Empty;
                    string OrderStatusName = string.Empty;
                    string FPStatus = string.Empty;
                    foreach (MoreThanTenDaysOrderInfoEntity orderItem in UnauditedOrNotOutStockList)
                    {
                        #region 订单状态
                        if (orderItem.OrderStatus == 0)
                        {
                            OrderStatusName = "待审核";
                        }
                        else if (orderItem.OrderStatus == 1)
                        {
                            OrderStatusName = "待出库";
                        }
                        else if (orderItem.OrderStatus == 2)
                        {
                            OrderStatusName = "待支付";
                        }
                        else if (orderItem.OrderStatus == 3)
                        {
                            OrderStatusName = "待主管审";
                        }
                        #endregion

                        #region FP状态设置
                        if (orderItem.IsFPSO.HasValue == false)
                        {
                            FPStatus = "";
                        }
                        else if (orderItem.IsFPSO.Value == 0)
                        {
                            FPStatus = "正常单";
                        }
                        else if (orderItem.IsFPSO.Value == 1)
                        {
                            FPStatus = "可疑订单";
                        }
                        else if (orderItem.IsFPSO.Value == 2)
                        {
                            FPStatus = "串货订单";
                        }
                        else if (orderItem.IsFPSO.Value == 3)
                        {
                            FPStatus = "炒货订单";
                        }


                        #endregion

                        #region 结账状态
                        if (orderItem.CheckoutStatus.HasValue == false)
                        {
                            CheckoutStatusName = "";
                        }
                        else if (orderItem.CheckoutStatus == 1)
                        {
                            CheckoutStatusName = "已结账";
                        }
                        else
                        {
                            CheckoutStatusName = "未结账";
                        }
                        #endregion
                        mailBody.Append("<tr>");
                        mailBody.Append("<td>" + orderItem.SysNo + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + FPStatus + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + OrderStatusName + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + orderItem.SOAmt + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + orderItem.OrderDate.ToString() + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + orderItem.PayTypeName + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + orderItem.ShipTypeName + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + CheckoutStatusName + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + orderItem.UpdateUserSysName + "&nbsp" + "</td>");
                        mailBody.Append("<td>" + orderItem.IsMoreThan90Days + "&nbsp" + "</td>");
                        mailBody.Append("</tr>");
                    }
                }
                mailBody.Append("</table>");
                mail.MailBody = mailBody.ToString();
                
            //把捞到的邮件数据插入表
                MailBodyV31 mailBodyV31 = new Utility.Email.DataContracts.MailBodyV31();
                try
                {
                    MailHelper.SendMail2IPP3Internal(mailBodyV31);
                }
                catch (BusinessException e)
                {
                    Console.WriteLine("错误原因" + e);
                }
                return true;
        }

        /// <summary>
        /// 0：待审核 1：待出库 2：待支付 3:待主管审
        /// 获取超过10天的订单信息
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        public static List<MoreThanTenDaysOrderInfoEntity> GetMoreThanTenDaysUnauditedOrNotOutStockOrderInfoList(string CompanyCode) {
            List<MoreThanTenDaysOrderInfoEntity> result = new List<MoreThanTenDaysOrderInfoEntity>();
            DataCommand  command=DataCommandManager.GetDataCommand("GetUnauditedOrNotOutStockOrderList");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            result = command.ExecuteEntityList<MoreThanTenDaysOrderInfoEntity>();
            return result;
        }

       
    }
}
 