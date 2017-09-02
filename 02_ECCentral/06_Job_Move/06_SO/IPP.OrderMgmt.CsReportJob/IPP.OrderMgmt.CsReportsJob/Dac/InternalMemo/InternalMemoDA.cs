using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.ThirdPart.JobV31.BusinessEntities.IngramMicro;

namespace IPP.ThirdPart.JobV31.Dac.IngramMicro
{
    public class InternalMemoDA
    {       
        /// <summary>
        /// 获取所有3Part Item映射信息
        /// </summary>
        /// <returns></returns>
        public static List<InternalMemoEntity> GetInternalMemoList(DateTime startTime, DateTime endTime)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetInternalMemoList");
            command.SetParameterValue("@StartTime", startTime);
            command.SetParameterValue("@EndTime", endTime);

            return command.ExecuteEntityList<InternalMemoEntity>();
        }
    }
}
