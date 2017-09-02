using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class QueryEventLogCriteria
    {
        public int TopCount { get; set; }

        public string UserID { get; set; }

        public string Page { get; set; }

        public string Action { get; set; }

        public string Label { get; set; }

        public string EventDateFrom { get; set; }

        public string EventDateTo { get; set; }
    }
}
