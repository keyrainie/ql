using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Services.Protocols;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Configuration;
using System.Security.Principal;

using Newegg.Oversea.Framework.ExceptionHandler;
using Newegg.Oversea.Framework.Utilities.String;

using Newegg.Oversea.Silverlight.ControlPanel.Service;


namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
          
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            string temStatusCode = context.Items["StatusCode"] as String;

            if (temStatusCode == "401")
            {
                context.Response.Clear();
                context.Response.ClearHeaders();

                context.Response.Expires = 0;

                context.Response.StatusCode = 401;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            Exception ex = context.Server.GetLastError();
            if (ex != null)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

    }

}
