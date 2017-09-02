using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Newegg.Oversea.Framework.DataAccess;

namespace IPP.MessageAgent.SendReceive.JobV31.DataAccess
{
    public class CommonDA
    {
        /// <summary>
        /// Message
        /// </summary>
        /// <param name="maxRecord"></param>
        /// <returns></returns>
        public static List<string> GetMessageSuccess(int maxRecord,string dataCommand)
        {
            DataCommand command = DataCommandManager.GetDataCommand(dataCommand);
            command.SetParameterValue("@MaxRecord",maxRecord);

            List<string> messageList=new List<string>();
            using(IDataReader dr=command.ExecuteDataReader())
            {
                while(dr.Read())
                {
                    messageList.Add(Convert.ToString(dr[0]).Trim());
                }
            }
            return messageList;
        }

        /// <summary>
        /// 调用SP处理消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dataCommand"></param>
        public static void SPProcess(string message,string dataCommand)
        {
            DataCommand command = DataCommandManager.GetDataCommand(dataCommand);
            command.SetParameterValue("@Msg", message);
            command.ExecuteNonQuery();
        }

        public static int CreateLog(string message, string direction, string destination, string referenceNumber, string messageType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateLog");
            command.SetParameterValue("@Message", message);
            command.SetParameterValue("@ReferenceNumber", referenceNumber);
            command.SetParameterValue("@MessageType", messageType);
            command.SetParameterValue("@Direction", direction);
            command.SetParameterValue("@Destination", destination);
            
            command.ExecuteNonQuery();

            object number = command.GetParameterValue("@TranNumber");

            return (int)number;
        }

        public static int UpdateLog(int transactionNumber, string referenceNumber, string messageType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateLog");
            command.SetParameterValue("@ReferenceNumber", referenceNumber);
            command.SetParameterValue("@MessageType", messageType);
            command.SetParameterValue("@TransactionNumber", transactionNumber);
            return command.ExecuteNonQuery();
        }
    }
}
