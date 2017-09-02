using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce.Web.Handlers
{
    /// <summary>
    /// Summary description for ExportExcelHandler
    /// </summary>
    public class ExportExcelHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "application/json";

            string method = context.Request["method"];

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}