using System;
using IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.Contract;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.DataContracts;
using System.Configuration;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.ServiceAdapter
{
    public class ServiceAdapterHelper
    {
        internal static void DealServiceFault(MessageFault fault)
        {
            if (fault == null)
                return;

            MessageFaultCollection faults = new MessageFaultCollection();
            faults.Add(fault);

            DealServiceFault(faults);
        }

        internal static void DealServiceFault(MessageFaultCollection faults)
        {
            if (faults != null && faults.Count > 0)
            {
                string exceptionMessage = string.Empty;
                string exceptionCodes = string.Empty;
                foreach (MessageFault fault in faults)
                {
                    exceptionCodes += fault.ErrorCode + ";";
                    exceptionMessage += fault.ErrorDescription + System.Environment.NewLine;
                }
                exceptionCodes = exceptionCodes.Substring(0, exceptionCodes.Length - 1);
                BusinessException exception = new BusinessException(exceptionMessage);
                exception.ErrorCode = exceptionCodes;
                exception.ErrorDescription = exceptionMessage;
                throw exception;
            }
        }

        internal static void DealServiceFault(DefaultDataContract contract)
        {
            if (contract != null)
            {
                DealServiceFault(contract.Faults);
            }
        }

        public static void SendEmail(MailEntity mailEntity)
        {
            TxtFileLogger logger = LoggerManager.GetLogger();
            try
            {
                var mailService = ServiceBroker.FindService<IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.ServiceContracts.ISendMail>();
                var mail = new MailBodyV31
                {
                    Body = new MailBodyMsg
                    {
                        MailTo = mailEntity.To,
                        CCMailAddress = mailEntity.CC,
                        Subjuect = mailEntity.Subject,
                        MailBody = mailEntity.Body,
                        Status = 0,//0：未发送，1：已经发送
                        CreateDate = DateTime.Now,
                        Priority = 1// Normal
                    }
                };

                mail.Header = new MessageHeader();
                mail.Header.CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
                DefaultDataContract result = mailService.SendMail2IPP3Internal(mail);
                if (result.Faults != null && result.Faults.Count > 0)
                {
                    throw new Exception(result.Faults[0].ErrorDescription);
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog("邮件发送失败！\r\n" + ex.ToString());
            }
        }
    }
}
