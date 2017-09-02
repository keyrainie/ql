using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Entity.Topic;
using Nesoft.ECWeb.Facade.Topic;

namespace Nesoft.ECWeb.M.Controllers
{
    public class TopicController : Controller
    {
        //
        // GET: /Topic/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Ajax_QueryNews()
        {
            NewsQueryFilter query = new NewsQueryFilter();
            query.PageInfo = new PageInfo();
            query.PageInfo.PageSize = 5;
            query.PageInfo.PageIndex = 1;
            //新闻类型
            //query.ReferenceSysNo = Model;

            string strPageIndex = Request["page"];
            if (!string.IsNullOrWhiteSpace(strPageIndex))
            {
                int pageIndex = 1;
                int.TryParse(strPageIndex, out pageIndex);
                query.PageInfo.PageIndex = pageIndex;
            }
            ViewDataDictionary dic = new ViewDataDictionary();
            QueryResult<NewsInfo> newsList = TopicFacade.QueryNewsInfo(query);

            var result = new AjaxResult { Success = true };
            result.Data = newsList;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int sysNo)
        {

            return View(sysNo);
        }

    }
}
