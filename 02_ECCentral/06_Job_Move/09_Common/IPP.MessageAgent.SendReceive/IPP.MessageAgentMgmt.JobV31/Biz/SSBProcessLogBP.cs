using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.MessageAgent.SendReceive.JobV31.DataAccess;
using System.Configuration;

namespace IPP.MessageAgent.SendReceive.JobV31.Biz
{
    public static class SSBProcessLogBP
    {
        public static int CreateLog(string message, string messageType)
        {
            return CreateLog(message, null, messageType);
        }

        public static int CreateLog(string message, string referenceNumber, string messageType)
        {
            string messageDirection = ConfigurationManager.AppSettings["MessageDirection"];
            string messageDestination = ConfigurationManager.AppSettings["MessageDestination"];
            
            int transactionNumber = CommonDA.CreateLog(message, messageDirection, messageDestination, referenceNumber, messageType);
            return transactionNumber;
        }

        public static void UpdateLog(int transactionNumber, string referenceNumber, string messageType)
        {
            CommonDA.UpdateLog(transactionNumber, referenceNumber, messageType);
        }
    }
}
