using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetEPortSOJob.Dac.Common
{
    public class CommonDA
    {
        /// <summary>
        /// 申报成功后修改订单状态
        /// </summary>
        /// <param name="soSysNo"></param>
        public static void UpdateOrderStatus(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateOrderStatus");
            command.SetParameterValue("@SysNo", soSysNo);
            command.ExecuteNonQuery();
        }
    }
}
