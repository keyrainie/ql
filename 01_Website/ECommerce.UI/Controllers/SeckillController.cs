using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity;
using ECommerce.Entity.Seckill;
using ECommerce.Facade.Seckill;

namespace ECommerce.UI.Controllers
{
    public class SeckillController : WWWControllerBase
    {

        /// <summary>
        /// 限时抢购首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Countdown()
        {
            int pageIndex;
            if (Request["page"] == null || !int.TryParse(Request["page"].ToString(), out pageIndex) || pageIndex < 1)
            {
                pageIndex = 1;
            }

            QueryResult<CountDownInfo> result = CountDownFacade.GetCountDownList(pageIndex);

            //取最晚结束抢购商品的活动结束时间为本场抢购的结束时间
            DateTime timeLeft = DateTime.MinValue;
            if (result.ResultList.Count > 0)
            {
                TimeSpan ts;
                result.ResultList.ForEach(item =>
                {
                    ts = item.EndTime - DateTime.Now;
                    if (ts.TotalSeconds > 0d && item.EndTime > timeLeft)
                    {
                        timeLeft = item.EndTime;
                    }
                });
            }
            //计算距离本场抢购活动结束的剩余秒数
            long leftSeconds = 0L;
            DateTime now = DateTime.Now;
            if (timeLeft > now)
            {
                TimeSpan ts = timeLeft - now;
                leftSeconds = Convert.ToInt64(Decimal.Round(Convert.ToDecimal(ts.TotalSeconds), 0));
            }
            ViewBag.LeftSeconds = leftSeconds;

            return View(result);
        }
    }
}
