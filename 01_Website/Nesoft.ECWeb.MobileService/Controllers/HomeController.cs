using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Models.Home;
using Nesoft.ECWeb.MobileService.Core;
using System.Collections;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class HomeController : BaseApiController
    {
        /// <summary>
        /// App首页相关数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHomeInfo()
        {
            HomeManager homeManager = new HomeManager();
            HomeModel homeInfo = homeManager.GetHomeInfo();
            var result = new AjaxResult
            {
                Success = true,
                Data = homeInfo
            };
            return Json(result);
        }

        /// <summary>
        /// 获取首页新闻列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult NewsList(int pageIndex, int pageSize)
        {
            return Json(new AjaxResult() { Success = true, Data = HomeManager.GetNewsList(pageIndex, pageSize) });
        }

        /// <summary>
        /// 获取新闻详细内容
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult NewsDetail(int sysNo)
        {
            return Json(new AjaxResult() { Success = true, Data = HomeManager.GetNewsDetail(sysNo) });
        }

        public ActionResult ClearCache()
        {
            List<string> keys = new List<string>();
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!enumerator.Key.ToString().Contains("AppStartPage"))
                {

                    keys.Add(enumerator.Key.ToString());
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                HttpRuntime.Cache.Remove(keys[i]);
            }


            return Content("清除缓存成功。");
        }
    }
}
