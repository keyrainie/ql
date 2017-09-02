using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerce.Web.Controllers
{
    public class MainController : SSLControllerBase
    {
        //
        // GET: /Main/
        
        public ActionResult Index()
        {
            return View();
        }

    }
}
