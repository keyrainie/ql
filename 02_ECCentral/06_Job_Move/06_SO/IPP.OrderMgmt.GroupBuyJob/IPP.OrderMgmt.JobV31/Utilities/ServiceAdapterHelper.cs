using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ExceptionBase;
using System.Configuration;

namespace IPP.OrderMgmt.JobV31
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

        internal static MessageHeader ApplyMessageHeader()
        {
            MessageHeader msgHeader = new MessageHeader();

            msgHeader.CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
            msgHeader.StoreCompanyCode = ConfigurationManager.AppSettings["StoreCompanyCode"];
            

            msgHeader.OperationUser = new OperationUser(

                ConfigurationManager.AppSettings["UserDisplayName"],
                ConfigurationManager.AppSettings["UserLoginName"],
                ConfigurationManager.AppSettings["StoreSourceDirectoryKey"],
                ConfigurationManager.AppSettings["CompanyCode"]
            );

            return msgHeader;
        }

        internal static void DealServiceFault(DefaultDataContract contract)
        {
            if (contract != null)
            {
                DealServiceFault(contract.Faults);
            }
        }
    }
}
