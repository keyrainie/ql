using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Seckill;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Seckill
{
    public class CountDownDA
    {
        public static List<CountDownInfo> GetAllCountDown(int limitBuyEarlyShowTimeSetting)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Seckill_GetAllCountDown");
            dataCommand.SetParameterValue("@LimitBuyEarlyShowTimeSetting", limitBuyEarlyShowTimeSetting);
            return dataCommand.ExecuteEntityList<CountDownInfo>();
        }
    }
}
