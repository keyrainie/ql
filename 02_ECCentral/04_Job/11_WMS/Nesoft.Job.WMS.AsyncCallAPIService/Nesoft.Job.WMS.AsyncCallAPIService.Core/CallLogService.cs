using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nesoft.Utility.DataAccess;

namespace Nesoft.Job.WMS.AsyncCallAPIService.Core
{
    public class CallLogService
    {
        /// <summary>
        /// 查询当前需要调用的Service Log(按照写入时间升序排列）：
        /// </summary>
        /// <returns></returns>
        public static List<CallLogInfo> GetActiveCallLogList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryActiveCallLogs");
            return command.ExecuteEntityList<CallLogInfo>();
        }

        public static int UpdateCallLogResult(int sysNo, bool isSuccess, string responseDataString, DateTime callTime, long costMillionSeconds, CallLogStatus status, int? retryCount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCallLogResult");
            command.SetParameterValue("@LastCallTime", callTime);
            command.SetParameterValue("@RetryCount", retryCount);
            command.SetParameterValue("@LastCallIsSuccess", isSuccess);
            command.SetParameterValue("@LastCallResponseData", responseDataString);
            command.SetParameterValue("@LastCallCostMillionSeconds", costMillionSeconds);
            command.SetParameterValue("@Status", (int)status);
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteNonQuery();
        }
    }
}
