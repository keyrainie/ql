using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.ServiceContracts;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.DataContracts;
using IPPOversea.POmgmt.ETA.Model;

namespace IPPOversea.POmgmt.ETA.Biz
{
    public class LogService
    {
        public static void WriteLog(LogInfo logInfo)
        {
            List<LogInfo> logInfos = new List<LogInfo>();

            logInfos.Add(logInfo);

            WriteLog(logInfos);
        }

        public static void WriteLog(List<LogInfo> logInfos)
        {
            ICreateLogV31 service = ServiceBroker.FindService<ICreateLogV31>();

            LogV31 log = new LogV31();

            try
            {
                log.Body = ToMessageList(logInfos);
                if (logInfos != null && logInfos.Count > 0)
                {
                    log.Header = new Newegg.Oversea.Framework.Contract.MessageHeader
                    {
                        CompanyCode = Settings.CompanyCode
                    };
                }
                else
                {
                    log.Header = new Newegg.Oversea.Framework.Contract.MessageHeader();
                }
                service.CreateLog(log);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ServiceBroker.DisposeService<ICreateLogV31>(service);
            }
        }

        private static LogMsg ToMessage(LogInfo logInfo)
        {
            if (logInfo == null)
            {
                return null;
            }

            LogMsg result = new LogMsg()
            {
                Note = logInfo.Note,
                OptIP = logInfo.OptIP,
                OptTime = logInfo.OptTime,
                OptUserSysNo = logInfo.OptUserSysNo,
                TicketSysNo = logInfo.TicketSysNo,
                TicketType = (int)logInfo.TicketType
            };

            return result;
        }

        private static List<LogMsg> ToMessageList(List<LogInfo> logInfos)
        {
            if (logInfos == null)
            {
                return null;
            }

            List<LogMsg> result = new List<LogMsg>();

            logInfos.ForEach(x => { result.Add(ToMessage(x)); });

            return result;
        }
    }
}
