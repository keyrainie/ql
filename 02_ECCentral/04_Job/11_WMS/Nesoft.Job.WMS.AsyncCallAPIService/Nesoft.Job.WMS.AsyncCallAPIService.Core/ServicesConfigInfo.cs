using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.AsyncCallAPIService.Core
{
    public class ServicesConfigInfo
    {
        public string ServiceName { get; set; }

        public List<ServicesConfigProcessorInfo> Processors { get; set; }

        public ServicesConfigInfo()
        {
            Processors = new List<ServicesConfigProcessorInfo>();
        }
    }

    public class ServicesConfigProcessorInfo
    {
        public string ProcessorName { get; set; }
        public string ProcessorImplementType { get; set; }
    }
}
