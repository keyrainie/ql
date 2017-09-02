using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using ECCentral.BizEntity.Customer;
using IPP.ECommerceMgmt.SendAmbassadorPoints.BusinessEntities;
using IPP.ECommerceMgmt.SendAmbassadorPoints.DA;
using IPP.ECommerceMgmt.SendAmbassadorPoints.Utilities;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.ServiceConsole.Client;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints.Biz
{
    public static class SendAmbassadorPointsBP
    {
        private const int RecommendPointType = 51;
        private const int AgentPointType = 50;
        public static JobContext jobContext = null;  
        private static decimal rate = 0.015m;
        public static string SysAccount = ConfigurationManager.AppSettings["SysAccount"];
        private static int startNumber = 0;      

        public static void CheckAmbassadorOrder()
        {
            string endMsg = string.Empty;

            DateTime currentDate = DateTime.Now;

            try
            {
                WriteLog("\r\n" + currentDate + " ------------------- Begin-------------------------");
                WriteLog(currentDate + " 新蛋大使加积分job开始运行......");

                WriteLog("\r\n" + DateTime.Now + " 正在进行大使推荐订单的处理......");
                Process(currentDate, RecommendPointType);
                WriteLog("\r\n" + DateTime.Now + " 正在进行大使代购订单的处理......");
                Process(currentDate, AgentPointType);

                WriteLog("\r\n" + DateTime.Now + " 本次运行结束......");
                WriteLog("\r\n" + DateTime.Now + " --------------------- End---------------------------");
            }
            catch (Exception er)
            {
                endMsg = DateTime.Now + " job运行异常，异常信息如下：\r\n " + er.ToString();
                SendExceptionInfoEmail(endMsg);
                WriteLog(endMsg);
            }
        }

        private static void Process(DateTime currentDate, int pointType)
        {
            startNumber = 0;
            while (true)
            {
                int processedCount = DoProcess(currentDate, pointType);
                WriteLog(Environment.NewLine + DateTime.Now + " Batch processed "
                    + processedCount + " records.");
                if (processedCount < AppConfigHelper.PerTopCount)
                {
                    break;
                }
                if (AppConfigHelper.PerSleepSecond > 0)
                {
                    WriteLog(" Sleep " + AppConfigHelper.PerSleepSecond + " seconds.");
                    Thread.Sleep(AppConfigHelper.PerSleepSecond * 1000);
                }
            }
        }

        private static int DoProcess(DateTime currentDate, int pointType)
        {
            List<SOMasterEntity> list = SendAmbassadorPointsDA.GetSO(pointType, currentDate, startNumber);
            if (list == null || list.Count == 0)
            {
                return 0;
            }
            Dictionary<string, int> customerPointDictionary = new Dictionary<string, int>();
            List<CustomerPointsAddRequest> pointRequestList = new List<CustomerPointsAddRequest>();
            foreach (SOMasterEntity so in list)
            {
                if (pointRequestList.Exists(delegate(CustomerPointsAddRequest item)
                    { return item.SOSysNo.Value == so.SysNo; }
                ))
                {
                    continue;
                }
                #region Get Refund
                List<RmaRefundEntity> refundList = SendAmbassadorPointsDA.GetRefundBySO(so.SysNo);
                //退款单为待审核状态Job不加积分
                if (refundList.Exists(delegate(RmaRefundEntity item)
                {
                    return item.Status == 3;
                }))
                {
                    startNumber++;
                    continue;
                }

                decimal refundAmount = Convert.ToInt32(refundList.Sum(delegate(RmaRefundEntity item)
                {
                    if (item.Status == 0 || item.Status == 2)
                    {
                        return item.PointAmt * 0.1m + item.CashAmt;
                    }
                    return 0;
                }));
                #endregion

                decimal soAmount = so.CashPay + so.DiscountAmt + so.PointPay * 0.1m;

                //团购订单
                if (so.SOType == 7)
                {
                    SOMasterEntity groupBuyingSo = SendAmbassadorPointsDA.GetGroupBuyingSO(so.SysNo);
                    decimal soGroupBuyingAmount = groupBuyingSo.CashPay + groupBuyingSo.DiscountAmt + groupBuyingSo.PointPay * 0.1m;
                    soAmount = soAmount - soGroupBuyingAmount;
                }
                //拆单的情况
                if (so.Status == -6)
                {
                    //所有子单金额
                    List<SOMasterEntity> allSOList = SendAmbassadorPointsDA.GetAllSubSO(so.SysNo);
                    soAmount = 0.0m;
                    foreach (SOMasterEntity soEntity in allSOList)
                    {
                        soAmount += soEntity.CashPay + soEntity.DiscountAmt + soEntity.PointPay * 0.1m;
                    }

                    //作废子单金额
                    List<SOMasterEntity> invalidList = SendAmbassadorPointsDA.GetInValidSubSO(so.SysNo);
                    decimal invalidAmount = 0.0m;
                    foreach (SOMasterEntity invalidSO in invalidList)
                    {
                        invalidAmount += invalidSO.CashPay + invalidSO.DiscountAmt + invalidSO.PointPay * 0.1m;
                    }

                    //作废团购订单金额
                    SOMasterEntity groupbuyingSO = SendAmbassadorPointsDA.GetGroupBuyingSubSO(so.SysNo);
                    decimal groupbuyingAmount = groupbuyingSO.CashPay + groupbuyingSO.DiscountAmt + groupbuyingSO.PointPay * 0.1m;

                    soAmount = soAmount - invalidAmount - groupbuyingAmount;
                }

                int pointAmount = (int)(rate * (soAmount - refundAmount) * 10);

                if (pointAmount <= 0)
                {
                    startNumber++;
                    continue;
                }

                #region add AdjustPointRequest
                CustomerPointsAddRequest pointRequest = new CustomerPointsAddRequest();
                //pointRequest.CompanyCode = AppConfigHelper.CompanyCode;
                pointRequest.CustomerSysNo = so.AgentSysNo;
                pointRequest.Memo = so.SysNo.ToString();
                pointRequest.NewEggAccount = SysAccount;
                pointRequest.Note = "新蛋大使订单加积分";
                pointRequest.Point = pointAmount;
                pointRequest.PointType = pointType;
                pointRequest.SOSysNo = so.SysNo;
                pointRequest.Source = "IPP.MKT.SendAmbassadorPoints";
                pointRequestList.Add(pointRequest);
                #endregion
               
            }
            BatchAdjustPointService(pointRequestList);
            return pointRequestList.Count;
        }

        public static void BatchAdjustPointService(List<CustomerPointsAddRequest> body)
        {
            if (body == null || body.Count == 0)
            {
                return;
            }
            string pointLog = string.Empty;
            //IAdjustPointV31 service = ServiceBroker.FindService<IAdjustPointV31>();
            //BatchActionRequest<AdjustPointRequestMsg> actionPara = new BatchActionRequest<AdjustPointRequestMsg>();
            //actionPara.Body = new List<CustomerPointsAddRequest>();
            //foreach (var item in body)
            //{
            //    AdjustPointRequestMsg msg = new AdjustPointRequestMsg()
            //      {
            //          CustomerSysNo = item.CustomerSysNo,
            //          Point = item.Point,
            //          PointType = (AdjustPointType)item.PointLogType,
            //          NewEggAccount = item.NewEggAccount,
            //          Memo = item.Memo + "," + item.Note,
            //          OrderSysNo = item.SOSysNo,
            //          Source = "ECommerceMgmt",
            //          OperationType = 0
            //      };

            //    pointLog += "CustomerSysNo=" + item.CustomerSysNo + ", Point=" + item.Point + " ,PointType=" + item.PointLogType + ",SOSysNo=" + item.SOSysNo + ";\r\n";
            //    actionPara.Body.Add(msg);
            //}
            //actionPara.Header = new Newegg.Oversea.Framework.Contract.MessageHeader();
            //actionPara.Header.CompanyCode = AppConfigHelper.CompanyCode;
            //actionPara.Header.Language = "zh-CN";
            //actionPara.Header.OperationUser = new Newegg.Oversea.Framework.Contract.OperationUser();
            //actionPara.Header.OperationUser.CompanyCode = AppConfigHelper.CompanyCode;
            //actionPara.Header.OperationUser.LogUserName = "IPPSystemAdmin\\bitkoo\\IPPSystemAdmin[8601]";
            //actionPara.Header.OperationUser.UniqueUserName = "IPPSystemAdmin\\bitkoo\\IPPSystemAdmin[8601]";
            //actionPara.Header.OperationUser.SourceUserName = "IPPSystemAdmin";
            //actionPara.Header.OperationUser.FullName = "IPPSystemAdmin";
            //actionPara.Header.OperationUser.SourceDirectoryKey = "bitkoo";
            //BatchActionResponse<AdjustPointRequestMsg> result = service.BatchAdjustPoint(actionPara);
            //MessageFaultCollection faults = GetMessageFaults(result);
            //ServiceAdapterHelper.DealServiceFault(faults);

            body.ForEach(e => {
                pointLog += "CustomerSysNo=" + e.CustomerSysNo + ", Point=" + e.Point + " ,SOSysNo=" + e.SOSysNo + ";\r\n";
            });

            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CustomerRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/Point/BatchAdjustPoint", body, out error);
            if (error == null && error.Faults == null && error.Faults.Count == 0)
            {
                WriteLog("新蛋大使发订单积分成功，发放记录：\r\n" + pointLog);
            }
            else {
                WriteLog("Error：\r\n" + error.Faults[0].ErrorDescription);
            }
        }

        //private static MessageFaultCollection GetMessageFaults(BatchActionResponse<AdjustPointRequestMsg> response)
        //{
        //    MessageFaultCollection result = new MessageFaultCollection();
        //    if (response != null && response.Body != null)
        //    {
        //        foreach (ActionResponse<AdjustPointRequestMsg> item in response.Body)
        //        {
        //            if (item != null && item.Fault != null)
        //            {
        //                result.Add(item.Fault);
        //            }
        //        }
        //    }
        //    return result;
        //}

        public static void SendExceptionInfoEmail(string ErrorMsg)
        {
            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                SendAmbassadorPointsDA.SendMailAboutExceptionInfo(ErrorMsg, AppConfigHelper.CompanyCode);
            }
        }

        public static void WriteConsoleInfo(string content)
        {
            Console.WriteLine(content);
        }

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            string logFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["BizLogFile"]
                , DateTime.Today.ToString("yyyyMMdd") + ".log");
            Log.WriteLog(content, logFile);
            if (jobContext != null)
            {
                jobContext.Message += content + "\r\n";
            }
        }
    }
}
