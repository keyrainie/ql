using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Configuration;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using IPP.InventoryMgmt.JobV31.Dac;
using ECCentral.BizEntity.Common;

namespace IPP.InventoryMgmt.JobV31.Biz
{
    public class VirtualRequestRunBP
    {
        //业务日志文件
        private static string BizLogFile;

        //公共数据
        private string CompanyCode;
        private string StoreCompanyCode;
        private string StoreSourceDirectoryKey;

        private string MailTo;
        private string MailFrom;
        private string Subject;
        private string CCMailAddress;
        private string BCMailAddress;
        private string MailBody;

        //运行
        public void Process(JobContext jobContext)
        {
            //日志
            //BizLogFile = jobContext.Properties["BizLog"];
            BizLogFile = "VirtualRequestLauncherLog";

            GetBasicInfo();

            //出错的申请单
            List<int> errorRequestList = new List<int>();

            //已审核数据列表
            List<VirtualRequestEntity> requestList;

            //获取数据
            requestList = VirtualRequestRunDA.GetAuditedVirtualRequest(CompanyCode);

            if (requestList.Count == 0)
            {
                WriteLog("No request data need to run.");
                return;
            }

            foreach (VirtualRequestEntity request in requestList)
            {
                
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //如果还存在关闭中的申请单则直接结束（不调库存）
                        VirtualRequestRunDA.CloseRequestWithOutAdjustInventory(request);

                        //运行已审核的虚库申请单
                        VirtualRequestRunDA.LaunchVirtualRequest(request);

                    }
                    catch (Exception e)
                    {
                        string erroMessage;
                        if (e.InnerException != null)
                            erroMessage = e.InnerException.Message;
                        else
                            erroMessage = e.Message;

                        WriteLog(string.Format(" VirtualRequest {0} launch failed:{1}", request.SysNo, erroMessage));
                        continue;
                    }

                    scope.Complete();
                }
               
                WriteLog(string.Format(" VirtualRequest {0} launch succees", request.SysNo));

            }

            //发邮件
            //foreach (int requestSysNo in errorRequestList)
            //{
            //    SendMail(requestSysNo) ;
            //}

        }

        private void GetBasicInfo()
        {
            CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
            StoreCompanyCode = ConfigurationManager.AppSettings["StoreCompanyCode"];
            StoreSourceDirectoryKey = ConfigurationManager.AppSettings["StoreSourceDirectoryKey"];

            MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            CCMailAddress = ConfigurationManager.AppSettings["CCMailAddress"];
            BCMailAddress = ConfigurationManager.AppSettings["BCMailAddress"];
        }

        private void SendMail(int requestSysNo)
        {
            //获取收件人
            List<string> EmailsTo=VirtualRequestCommonDA.GetUserMailByVirtualRequest(requestSysNo,CompanyCode);

            if (EmailsTo.Count == 0)
            {
                WriteLog("没有邮件接收人！");
                return;
            }
            else
            {
                foreach (string mailAddress in EmailsTo)
                {
                    MailTo += ";" + mailAddress;
                }
                MailTo = MailTo.Remove(0, 1);
            }

            //获取虚库申请单中的商品信息作为邮件Body
            ProductVirtualInfoEntity productInfo = VirtualRequestCommonDA.GetProductInfoByVirtualRequestSysNo(requestSysNo, CompanyCode);

            Subject = "虚库记录无法结束  " + productInfo.ProductID + productInfo.ProductName;
            StringBuilder sb=new StringBuilder();
            sb.AppendLine(string.Format("商品名称:{0}<br/>",productInfo.ProductName));
            sb.AppendLine(string.Format("商品SysNo:{0}<br/>", productInfo.ProductSysNo));
            sb.AppendLine(string.Format("商品ID:{0}<br/>", productInfo.ProductID));
            sb.AppendLine(string.Format("商品链接:http:{0}<br/>", productInfo.ProductLink));
            sb.AppendLine(string.Format("设定虚库数量:{0}<br/>", productInfo.VirtualQty));
            sb.AppendLine(string.Format("生效虚库数量:{0}<br/>", productInfo.HoldVirtualQty));
            sb.AppendLine(string.Format("开始时间:{0}<br/>", productInfo.StartTime));
            sb.AppendLine(string.Format("结束时间:{0}<br/>", productInfo.EndTime));
            sb.AppendLine(string.Format("状态:{0}<br/>", productInfo.Status));

            MailBody = sb.ToString();

            //Email_InternalInfoEntity mail=new Email_InternalInfoEntity();
            MailInfo mail = new MailInfo();

            mail.FromName = MailFrom;
            mail.ToName = MailTo;
            mail.CCName = CCMailAddress;
            mail.BCCName = BCMailAddress;
            mail.Subject = Subject;
            mail.Body = MailBody;

            MailAdapter.Send(mail);
        }

        private void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }

    }
}
