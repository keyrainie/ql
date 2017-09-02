using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nesoft.ECWeb.M.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/

        public ActionResult Detail(int? productSysNo)
        {
            if (!productSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "该商品不存在！";
                return View("Error");
            }
            return View(productSysNo);
        }
        public ActionResult DetailDesc(int? productSysNo)
        {
            if (!productSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "该商品不存在！";
                return View("Error");
            }
            return View(productSysNo);
        }

        /// <summary>
        /// 热销排行
        /// </summary>
        /// <returns></returns>
        public ActionResult HotSale()
        {
            return View();
        }
    }
}
