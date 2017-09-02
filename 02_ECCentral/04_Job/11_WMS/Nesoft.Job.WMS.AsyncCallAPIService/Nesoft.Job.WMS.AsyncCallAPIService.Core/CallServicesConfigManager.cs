using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nesoft.Job.WMS.AsyncCallAPIService.Core
{
    public static class CallServicesConfigManager
    {
        private static readonly string SERVICE_CONFIG_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/Services.config");

        private static object s_snycObj = new object();
        private static List<ServicesConfigInfo> servicesConfigList;

        static CallServicesConfigManager()
        {
            if (null == servicesConfigList || servicesConfigList.Count <= 0)
            {
                // lock (s_snycObj)
                // {
                LoadConfig();
                // }
            }
        }

        private static void LoadConfig()
        {
            servicesConfigList = new List<ServicesConfigInfo>();
            XDocument doc = XDocument.Load(SERVICE_CONFIG_PATH);
            if (null != doc)
            {
                var services = doc.Descendants("service");

                if (services != null && services.Count() > 0)
                {
                    foreach (var service in services)
                    {
                        ServicesConfigInfo serviceInfo = new ServicesConfigInfo()
                        {
                            ServiceName = service.Attribute("name").Value.ToString()
                        };


                        if (service.Descendants().Elements("processor").Count() > 0)
                        {
                            foreach (var processor in service.Descendants().Elements("processor"))
                            {
                                ServicesConfigProcessorInfo prosessorInfo = new ServicesConfigProcessorInfo()
                                {
                                    ProcessorName = processor.Attribute("name").Value.ToString(),
                                    ProcessorImplementType = processor.Attribute("implementType").Value.ToString()
                                };
                                serviceInfo.Processors.Add(prosessorInfo);
                            }
                        }
                        servicesConfigList.Add(serviceInfo);
                    }
                }
            }
        }

        public static List<ServicesConfigInfo> ServicesConfigList
        {
            get
            {
                return servicesConfigList;
            }
        }
    }
}
