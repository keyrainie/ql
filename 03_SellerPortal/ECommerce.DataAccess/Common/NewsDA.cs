using ECommerce.Entity.Common;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.DataAccess.Common
{
    public class NewsDA
    {
        public static List<NewsInfo> GetTop10NewsInfoList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetTop10NewsInfoList");
            return cmd.ExecuteEntityList<NewsInfo>();
        }

        public static NewsInfo GetNewsInfo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNewsInfo");
            cmd.SetParameterValue("@SysNo", SysNo);
            return cmd.ExecuteEntity<NewsInfo>();
        }
    }
}
