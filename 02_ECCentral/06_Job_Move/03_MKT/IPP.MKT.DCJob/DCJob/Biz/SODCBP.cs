using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Contract;
using IPP.ECommerceMgmt.ServiceJob.Common;
using IPP.ECommerceMgmt.ServiceJob.Dac;
using IPP.ECommerceMgmt.ServiceJob.BusinessEntities;
using System.Threading;

namespace IPP.ECommerceMgmt.ServiceJob.Biz
{
    public class SODCBP
    {
        public static string LogFile = string.Empty;
        public static string ErrorFile = string.Empty;
        public static void DoWork(string logFile, string errFile,AutoResetEvent are)
        {
            LogFile = logFile;
            ErrorFile = errFile;
            WriteLog("新一轮工作开始...");          
            List<SOEntity> list = SODCDA.GetSOInfo();
            WriteLog(string.Format("找到[15分钟前]待审核秒杀订单[{0}]笔.",list.Count));
            int index = 0;
            if (list.Count==0)
            {
                are.Set();
                WriteLog("本轮工作结束。");              
                return;
            }
            foreach (var item in list)
            {
                Thread th = new Thread((obj) =>
                {
                    SOEntity itemSO = obj as SOEntity;
                    try
                    {
                        if (itemSO.PayStatus == "-1"
                            || itemSO.IncomeStatus == "-1"
                            || string.IsNullOrEmpty(itemSO.PayStatus))
                        {
                            WriteLog(string.Format("订单[{0}]超时15分钟未支付...,进行作废", itemSO.SOSysNo));
                            if (BlankOutSO(itemSO.SOSysNo))
                            {
                                SODCDA.MakeOpered(itemSO.SOSysNo);
                            }
                        }
                        else
                        {
                            WriteLog(string.Format("订单[{0}]已支付.", itemSO.SOSysNo));
                            SODCDA.MakeOpered(itemSO.SOSysNo);
                        }                      

                    }
                    catch (Exception ex)
                    {
                        WriteErrorLog(string.Format("订单[{0}]处理异常\r\n{1}", item.SOSysNo, ex.Message));
                    }                   
                    index++;
                    if (index==list.Count)
                    {
                        are.Set();
                        WriteLog("本轮工作结束。");
                    }

                });
                th.Start(item);
            }          
        }
        public static bool BlankOutSO(int SoSysNo)
        {

            //IMaintainSOV31 service = ServiceBroker.FindService<IMaintainSOV31>();
            try
            {
                //    SOV31 actionPara = new SOV31()
                //{
                //    Header = new Newegg.Oversea.Framework.Contract.MessageHeader()
                //    {
                //        OperationUser = new OperationUser
                //        (Settings.UserDisplayName,
                //        Settings.UserLoginName,
                //        Settings.StoreSourceDirectoryKey,
                //        Settings.CompanyCode),
                //        CompanyCode = Settings.CompanyCode,
                //        StoreCompanyCode=Settings.CompanyCode,
                //        FromSystem = "127.0.0.1"
                //    }
                //    ,
                //    Body = new SOMsg { SOMaster = new SOMasterMsg { SystemNumber = SoSysNo } }
                //};
                //    actionPara = service.EmployeeAbandonSO(actionPara);

                SOAbandonReq req = new SOAbandonReq();
                req.SOSysNoList = new List<int>();
                req.SOSysNoList.Add(SoSysNo);
                req.ImmediatelyReturnInventory = true;
                req.IsCreateAO = false;

                string baseUrl = System.Configuration.ConfigurationManager.AppSettings["SORestFulBaseUrl"];
                string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
                string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
                ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
                ECCentral.Job.Utility.RestServiceError error;
                var ar = client.Update("/SO/Abandon", req, out error);

                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                    string tmpSendMessageException = error.Faults[0].ErrorDescription + "\r\n" +
                       error.Faults[0].ErrorCode;
                    WriteLog(string.Format("订单{0}作废失败：\r\n{1}.", SoSysNo, tmpSendMessageException));
                    return false;
                }
                else
                {
                    WriteLog(string.Format("订单{0}作废成功.", SoSysNo));
                    return true;
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(string.Format("订单[{0}]作废异常\r\n{1}", SoSysNo, ex.Message));
                return false;
            }

        }
        private static void WriteLog(string content)
        {
            Log.WriteLog(content, LogFile);
            Console.WriteLine(content);
        }
        private static void WriteErrorLog(string content)
        {
            Log.WriteLog(content, ErrorFile);
            Console.WriteLine(content);
        }
    }
}
