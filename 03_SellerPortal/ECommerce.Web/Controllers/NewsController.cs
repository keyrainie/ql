using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerce.Web.Controllers
{
    public class NewsController : SSLControllerBase
    {
        //
        // GET: /News/

        public ActionResult NewsDetails()
        {
            return View();
        }

    }
}
