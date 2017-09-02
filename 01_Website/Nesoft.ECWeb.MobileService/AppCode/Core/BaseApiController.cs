using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Nesoft.ECWeb.WebFramework;
using System.Web.Mvc;

namespace Nesoft.ECWeb.MobileService.Core
{
    [RequireOrigin]
    [ApiExceptionFilter]
    public class BaseApiController : Controller
    {
        public string GetText(string key)
        {
            return LanguageHelper.GetText(key);
        }
    }
}