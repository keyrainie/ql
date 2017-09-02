using System;
using System.Net;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newegg.Oversea.Silverlight.ControlPanel.Service;


namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost
{
    public partial class _Default : System.Web.UI.Page
    {
        protected string Name
        {
            get
            {
                return CPConfig.Application.Name;
            }
        }
    }
}
