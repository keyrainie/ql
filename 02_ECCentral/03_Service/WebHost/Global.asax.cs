using System;
using System.Web;

using ECCentral.Service.Utility;

namespace ECCentral.Service.WebHost
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            AutorunManager.Startup(ex => ExceptionHelper.HandleException(ex));
        }

        protected void Application_End(object sender, EventArgs e)
        {
            AutorunManager.Shutdown(ex => ExceptionHelper.HandleException(ex));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = HttpContext.Current.Server.GetLastError();
            if (ex != null)
            {
                ExceptionHelper.HandleException(ex);
            }
        }
    }
}