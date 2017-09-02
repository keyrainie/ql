using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost
{
    public class ReportConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("Reports", IsRequired = true)]
        public Reports Reports
        {
            get
            {
                return this["Reports"] as Reports;
            }
        }
    }

    [ConfigurationCollection(typeof(ReportElement), AddItemName = "Report", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class Reports : ConfigurationElementCollection
    {
        public ReportElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as ReportElement;
            }
        }

        public ReportElement this[string key]
        {
            get
            {
                return base.BaseGet(key) as ReportElement;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ReportElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ReportElement).Key;
        }
    }


    public class ReportElement : ConfigurationElement
    {
        [ConfigurationProperty("Key", IsRequired = true)]
        public string Key
        {
            get
            {
                return this["Key"] as string;
            }
        }

        [ConfigurationProperty("Description", IsRequired = true)]
        public string Description
        {
            get
            {
                return this["Description"] as string;
            }
        }

        [ConfigurationProperty("ComponentType", IsRequired = true)]
        public string ComponentType
        {
            get
            {
                return this["ComponentType"] as string;
            }
        }

        [ConfigurationProperty("KeystoneRule", IsRequired = true)]
        public string KeystoneRule
        {
            get
            {
                return this["KeystoneRule"] as string;
            }
        }

    }
}