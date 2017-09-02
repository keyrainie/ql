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
    public class VirtualRequestExpireBP
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
            BizLogFile = "VirtualRequestExpireLog";

            GetBasicInfo();

            //出错的申请单
            List<int> errorRequestList = new List<int>();

            //已运行数据列表
            List<VirtualRequestEntity> requestList;

            //获取数据
            requestList = VirtualRequestExpireDA.GetRuningVirtualRequest(CompanyCode);
            

            if (requestList.Count == 0)
            {
                WriteLog("No request data need to Expire.");
                return;
            }

            foreach (VirtualRequestEntity request in requestList)
            {
                int result=0;
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        result=VirtualRequestExpireDA.CloseVirtualRequest(request.SysNo, request.Status, 1, "IPPSystem_Job", request.CompanyCode, request.StoreCompanyCode, request.LanguageCode);
                        
                    }
                    catch (Exception e)
                    {
                        string erroMessage;
                        if (e.InnerException != null)
                            erroMessage = e.InnerException.Message;
                        else
                            erroMessage = e.Message;

                        WriteLog(string.Format("VirtualRequest {0} Expire failed:{1}", request.SysNo,erroMessage));
                        continue;
                    }

                    scope.Complete();
                }

                if (result == 0)
                {
                    WriteLog(string.Format("VirtualRequest {0} Expire is closing", request.SysNo));
                    errorRequestList.Add(request.SysNo);
                }
                if(result==1)
                {
                    WriteLog(string.Format("VirtualRequest {0} Expire succees", request.SysNo));
                }
            }

            //发邮件
            foreach (int requestSysNo in errorRequestList)
            {
                SendMail(requestSysNo) ;
            }

        }

        private void SendMail(int requestSysNo)
        {
            //获取收件人
            List<string> EmailsTo = VirtualRequestCommonDA.GetUserMailByVirtualRequest(requestSysNo, CompanyCode);
         
            if (EmailsTo.Count == 0)
            {
                WriteLog("没有邮件接收人！");
                return;
            }
            else
            {
                MailTo = "";
                foreach (string mailAddress in EmailsTo)
                {
                    MailTo += ";" + mailAddress;
                }
                MailTo = MailTo.Remove(0,1);
            }

            //获取虚库申请单中的商品信息作为邮件Body
            ProductVirtualInfoEntity productInfo = VirtualRequestCommonDA.GetProductInfoByVirtualRequestSysNo(requestSysNo, CompanyCode);

            Subject = string.Format("<p style='font-size:10.5pt;color:red'>虚库申请单结束失败 {0} {1}</p>", productInfo.ProductID, productInfo.ProductName);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>商品名称:{0}</p>", productInfo.ProductName));
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>商品SysNo:{0}</p>", productInfo.ProductSysNo));
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>商品ID:{0}</p>", productInfo.ProductID));
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>商品链接:<a href='http://www.newegg.com.cn/Products/ProductDetail.aspx?sysno={0}'>http://www.newegg.com.cn/Products/ProductDetail.aspx?sysno={0}<a/></p>", productInfo.ProductSysNo));
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>设定虚库数量:{0}</p>", productInfo.VirtualQty));
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>生效虚库数量:{0}</p>", productInfo.HoldVirtualQty));
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>开始时间:{0}</p>", productInfo.StartTime));
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>结束时间:{0}</p>", productInfo.EndTime));
            sb.AppendLine(string.Format("<p style='font-size:10.5pt;font-family:宋体;color:blue'>状态:{0}</p>", productInfo.StatusDescription));

            MailBody = sb.ToString();

            //Email_InternalInfoEntity mail = new Email_InternalInfoEntity();
            MailInfo mail = new MailInfo();

            mail.FromName = MailFrom;
            mail.ToName = MailTo;
            mail.CCName = CCMailAddress;
            mail.BCCName = BCMailAddress;
            mail.Subject = Subject;
            mail.Body = MailBody;

            MailAdapter.Send(mail);
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

        private void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }

    }
}
