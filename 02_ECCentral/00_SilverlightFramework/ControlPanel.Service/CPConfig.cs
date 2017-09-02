using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;

using Newegg.Oversea.Silverlight.ControlPanel.Service.Configuration;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    public class CPConfig
    {
        private static ApplicationSection s_applicationSection;
        private static KeystoneSection s_keystoneSection;
        private static ECCentralSection s_ecCentralSection;

        static CPConfig()
        {
            s_applicationSection = (ApplicationSection)ConfigurationManager.GetSection("oversea/application");
            s_keystoneSection = (KeystoneSection)ConfigurationManager.GetSection("oversea/keystone");
            Object obj = ConfigurationManager.GetSection("oversea/ecCentral");
            if (obj != null)
            {
                s_ecCentralSection = (ECCentralSection)obj;
            }
            else
            {
                s_ecCentralSection = null;
            }
        }

        public static ECCentralSection ECCentral
        {
            get
            {
                return s_ecCentralSection;
            }
        }

        public static ApplicationSection Application
        {
            get
            {
                return s_applicationSection;
            }
        }

        public static KeystoneSection Keystone
        {
            get
            {
                return s_keystoneSection;
            }
        }
    }   

}
