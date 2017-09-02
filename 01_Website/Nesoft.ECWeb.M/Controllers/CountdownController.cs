using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Entity.Seckill;
using Nesoft.ECWeb.Facade.Seckill;

namespace Nesoft.ECWeb.M.Controllers
{
    public class CountdownController : Controller
    {
        //
        // GET: /Countdown/

        public ActionResult Index()
        {
            QueryResult<CountDownInfo> result = CountDownFacade.GetCountDownList(1, 10);
            return View(result);
        }

        public ActionResult AjaxCountdownProducts()
        {
            int pageIndex = 0, temp;
            if (int.TryParse(Request.QueryString["pageIndex"], out temp))
            {
                pageIndex = temp;
            }
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            QueryResult<CountDownInfo> result = CountDownFacade.GetCountDownList(pageIndex + 1, 10);
            return PartialView("_CountdownProductList", result);
        }
    }
}
