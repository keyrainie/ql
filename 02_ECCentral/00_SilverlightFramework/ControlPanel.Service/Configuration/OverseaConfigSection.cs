using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.Configuration
{
    public class ECCentralSection : ConfigurationSection
    {
        [ConfigurationProperty("ServiceURL", IsRequired = true)]
        public string ServiceURL
        {
            get
            {
                return ((string)this["ServiceURL"]);
            }
            set
            {
                this["ServiceURL"] = value;
            }
        }

        [ConfigurationProperty("ConfigPrefix", IsRequired = false)]
        public string ConfigPrefix
        {
            get
            {
                return ((string)this["ConfigPrefix"]);
            }
            set
            {
                this["ConfigPrefix"] = value;
            }
        }
    }

    public class ApplicationSection : ConfigurationSection
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get
            {
                return ((string)this["id"]).ToUpper();
            }
            set
            {
                this["id"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("businessCode", IsRequired = true)]
        public string BusinessCode
        {
            get
            {
                return (string)this["businessCode"];
            }
            set
            {
                this["businessCode"] = value;
            }
        }

        [ConfigurationProperty("defaultLanguage", IsRequired = true)]
        public string DefaultLanguage
        {
            get
            {
                return (string)this["defaultLanguage"];
            }
            set
            {
                this["defaultLanguage"] = value;
            }
        }

        [ConfigurationProperty("autoLogin", IsRequired = true)]
        public bool AutoLogin
        {
            get
            {
                return (bool)this["autoLogin"];
            }
            set
            {
                this["autoLogin"] = value;
            }
        }

        [ConfigurationProperty("gloalRegion", IsRequired = true)]
        public string GloalRegion
        {
            get
            {
                return (string)this["gloalRegion"];
            }
            set
            {
                this["gloalRegion"] = value;
            }
        }

        [ConfigurationProperty("localRegion", IsRequired = true)]
        public string LocalRegion
        {
            get
            {
                return (string)this["localRegion"];
            }
            set
            {
                this["localRegion"] = value;
            }
        }

        [ConfigurationProperty("defaultPage", IsRequired = false)]
        public string DefaultPage
        {
            get
            {
                string page = (string)this["defaultPage"];
                if (string.IsNullOrEmpty(page))
                {
                    return "/Main/Home";
                }
                return page;
            }
            set
            {
                this["defaultPage"] = value;
            }
        }
    }

    public class KeystoneSection : ConfigurationSection
    {
        [ConfigurationProperty("sourceDirectory", IsRequired = true)]
        public string SourceDirectory
        {
            get
            {
                return (string)this["sourceDirectory"];
            }
            set
            {
                this["sourceDirectory"] = value;
            }
        }

        [ConfigurationProperty("trustedDirectory", IsRequired = true)]
        public string TrustedDirectory
        {
            get
            {
                return (string)this["trustedDirectory"];
            }
            set
            {
                this["trustedDirectory"] = value;
            }
        }

        [ConfigurationProperty("trustedUserName", IsRequired = true)]
        public string TrustedUserName
        {
            get
            {
                return (string)this["trustedUserName"];
            }
            set
            {
                this["trustedUserName"] = value;
            }
        }

        [ConfigurationProperty("trustedPassword", IsRequired = true)]
        public string TrustedPassword
        {
            get
            {
                return (string)this["trustedPassword"];
            }
            set
            {
                this["trustedPassword"] = value;
            }
        }

        [ConfigurationProperty("primaryAuthUrl", IsRequired = true)]
        public string PrimaryAuthUrl
        {
            get
            {
                return (string)this["primaryAuthUrl"];
            }
            set
            {
                this["primaryAuthUrl"] = value;
            }
        }

        [ConfigurationProperty("secondaryAuthUrl", IsRequired = true)]
        public string SecondaryAuthUrl
        {
            get
            {
                return (string)this["secondaryAuthUrl"];
            }
            set
            {
                this["secondaryAuthUrl"] = value;
            }
        }

        [ConfigurationProperty("cacheName")]
        public string CacheName
        {
            get
            {
                return (string)this["cacheName"];
            }
            set
            {
                this["cacheName"] = value;
            }
        }

        [ConfigurationProperty("cacheExpires", DefaultValue = "00:30:00")]
        public TimeSpan CacheExpires
        {
            get
            {
                return (TimeSpan)this["cacheExpires"];
            }
            set
            {
                this["cacheExpires"] = value;
            }
        }

        [ConfigurationProperty("applications")]
        public KeystoneApplicationCollection Applications
        {
            get
            {
                return (KeystoneApplicationCollection)this["applications"];
            }
        }

        public string[] SourceDirectories
        {
            get
            {
                string sourceDirectory = this.SourceDirectory;
                string[] result = sourceDirectory == null ? new string[] { } : sourceDirectory.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                return result;
            }
        }

        public Dictionary<string, string> KeystoneApplications
        {
            get
            {
                var applications = new Dictionary<string, string>();

                if (this.Applications != null)
                {
                    foreach (KeystoneApplicationElement app in this.Applications)
                    {
                        applications.Add(app.Name, app.Id);
                    }
                }

                return applications;
            }
        }

        public string[] ApplicationIds
        {
            get
            {
                List<string> appIds = new List<string>();
                KeystoneApplicationCollection applications = this.Applications;
                if (applications != null)
                {
                    foreach (KeystoneApplicationElement app in applications)
                    {
                        appIds.Add(app.Id);
                    }
                }
                string[] result = appIds.ToArray();

                return result;
            }
        }
    }

    public class KeystoneApplicationElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        public string Id
        {
            get
            {
                return ((string)this["id"]).ToUpper();
            }
            set
            {
                this["id"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
    }

    public class KeystoneApplicationCollection : ConfigurationElementCollection
    {
        public KeystoneApplicationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as KeystoneApplicationElement;
            }
        }

        public KeystoneApplicationElement this[string name]
        {
            get
            {
                return base.BaseGet(name) as KeystoneApplicationElement;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new KeystoneApplicationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeystoneApplicationElement)element).Id;
        }
    }
}
