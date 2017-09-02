using System;
using System.Collections.Generic;
using System.Threading;
using ECCentral.BizEntity.Common;
using IPP.MessageAgent.SendReceive.JobV31.Configuration;
using IPP.MessageAgent.SendReceive.JobV31.DataAccess;
using Newegg.Oversea.Framework.Core.Threading;
using Newegg.Oversea.Framework.ExceptionHandler;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.Retry;

namespace IPP.MessageAgent.SendReceive.JobV31.Biz
{
    public abstract class SSBProcessBase
    {
        private const string MaxRecourd = "MonitorMaxRecord";

        private List<string> _logs = new List<string>();

        protected SSBChannel _ssbChannel;

        private int _maxRecord;

        public List<string> Logs
        {
            get
            {
                return _logs;
            }
        }

        public abstract string GetAcitonType(string message);

        public abstract string GetReferenceKey(string message, SSBProcesser processer);

        public SSBProcessBase(SSBChannel ssbChannel)
        {
            _ssbChannel = ssbChannel;
            _maxRecord = Convert.ToInt32(ConfigHelper.GetAppSettings(MaxRecourd));
        }

        public void RunProcess()
        {
            List<string> messageList = CommonDA.GetMessageSuccess(_maxRecord, _ssbChannel.DataCommand);
            if (messageList.Count == 0)
            {
                this.WriteLog(string.Format("[{0}]没有要处理的消息", _ssbChannel.Name));
                return;
            }

            this.WriteLog(string.Format("========[{0}] Start========", _ssbChannel.Name));

            foreach (var item in messageList)
            {
                this.ProcessMessage(item);
            }

            #region 异步暂时无法解决按顺序调用服务的问题
            //List<List<string>> groups = SplitMessagesToGroup(messageList, 10);
            //foreach (List<string> group in groups)
            //{
            //    using (ThreadWaitHandle handler = new ThreadWaitHandle(group.Count))
            //    {
            //        foreach (string item in group)
            //        {
            //            string ssbMsg = item;
            //            ThreadPool.QueueUserWorkItem((object obj) =>
            //            {
            //                this.ProcessMessage(handler, ssbMsg);
            //            });
            //        }
            //    }
            //}
            #endregion

            this.WriteLog(string.Format("========[{0}] End========", _ssbChannel.Name));
        }

        private void ProcessMessage(string ssbMsg)
        {
            int transNumber = 0;
            try
            {
                string actionType = this.GetAcitonType(ssbMsg);
                transNumber = SSBProcessLogBP.CreateLog(ssbMsg, actionType);

                SSBProcesser processer = _ssbChannel.GetProcesser(actionType);
                if (processer == null)
                {
                    this.WriteLog("未找到有效的ActionType,请检查配置");
                    return;
                }

                this.ProcessMessagae(ssbMsg, processer);

                string referenceKey = this.GetReferenceKey(ssbMsg, processer);
                SSBProcessLogBP.UpdateLog(transNumber, referenceKey, actionType);

                this.WriteLog(string.Format("Success , ReferenceKey is {0}", referenceKey));
            }
            catch (Exception ex)
            {
                RetryProcessHelper.WriteLog<string>(
                    ssbMsg
                    , transNumber.ToString()
                    , ex.ToString()
                    , string.Concat("MessageAgent:", _ssbChannel.DataCommand));

                this.WriteLog(ex.Message);
            }
        }

        //[RetryErrorHandling]
        private void ProcessMessagae(string message, SSBProcesser processer)
        {
            string serviceName = processer.ProcessService;
            if (processer.CallType == CallType.SP)
            {
                CommonDA.SPProcess(message, serviceName);
            }
            else
            {
                RequestMessage req = new RequestMessage();
                req.Message = message;
                req.ActionType = processer.ActionType;

                //IProcessMessage service = ServiceBroker.GetService<IProcessMessage>(serviceName);
                //try
                //{
                //    service.Process(message, processer.ActionType);
                //}
                //finally
                //{
                //    ServiceBroker.Close(service);
                //}
                if (ConfigHelper.RestServiceConfig.RestServices.Contains(processer.ProcessService))
                {
                    RestService restServiceConfig = ConfigHelper.RestServiceConfig.RestServices[processer.ProcessService];
                    string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
                    string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
                    ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(restServiceConfig.BaseUrl, languageCode);
                    ECCentral.Job.Utility.RestServiceError error;
                    var ar = client.Update(restServiceConfig.RelativeUrl, req, out error);
                    if (error != null && error.Faults != null && error.Faults.Count > 0)
                    {
                        string errorMsg = "";
                        foreach (var errorItem in error.Faults)
                        {
                            errorMsg += errorItem.ErrorDescription;
                        }
                        ECCentral.Job.Utility.Logger.WriteLog(errorMsg, "JobConsole");
                    }
                }
                else
                {
                    throw new Exception(string.Format("未找到{0}对应的Rest Service配置!", processer.ProcessService));
                }
            }
        }

        #region [ Helper ]

        private void WriteLog(string message)
        {
            _logs.Add(string.Format("[{0}] {1}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), message));
        }

        private List<List<string>> SplitMessagesToGroup(List<string> messages, int groupMaxCount)
        {
            List<List<string>> groupList = new List<List<string>>();
            List<string> groupItem = new List<string>();

            for (int i = 0; i < messages.Count; i++)
            {
                groupItem.Add(messages[i]);
                if (groupItem.Count > 0 && groupItem.Count % groupMaxCount == 0)
                {
                    groupList.Add(groupItem);
                    groupItem = new List<string>();
                }
            }

            if (groupItem.Count > 0)
            {
                groupList.Add(groupItem);
            }
            return groupList;
        }

        #endregion
    }
}
