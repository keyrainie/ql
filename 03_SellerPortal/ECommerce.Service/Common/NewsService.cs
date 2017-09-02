using ECommerce.DataAccess.Common;
using ECommerce.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Service.Common
{
    public class NewsService
    {
        /// <summary>
        /// 查询sp首页10条新闻公告
        /// </summary>
        /// <returns></returns>
        public static List<NewsInfo> GetTop10NewsInfoList()
        {
            var result= NewsDA.GetTop10NewsInfoList();
            if (result == null)
            {
                result = new List<NewsInfo>();
            }
            return result;
        }

        /// <summary>
        /// 根据id查询新闻公告
        /// </summary>
        /// <returns></returns>
        public static NewsInfo GetNewsInfo(int Sysno)
        {
            var result= NewsDA.GetNewsInfo(Sysno);
            if (result == null)
            {
                result = new NewsInfo();
            }
            return result;
        }
    }
}
