using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;


namespace IPP.ContentMgmt.SendMailForPicture.Providers
{
    public class ServiceJobSection : ConfigurationSection
    {
        [ConfigurationProperty("JobCollection")]
        public ServiceJobInfoCollection JobCollection
        {
            get { return (ServiceJobInfoCollection)this["JobCollection"]; }
        }

        [ConfigurationProperty("JobProviders")]
        public ProviderSettingsCollection JobProviders
        {
            get { return (ProviderSettingsCollection)this["JobProviders"]; }
        }
    }
}
