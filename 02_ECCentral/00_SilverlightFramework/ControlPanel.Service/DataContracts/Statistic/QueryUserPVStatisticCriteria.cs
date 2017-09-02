using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class QueryUserPVStatisticCriteria
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Url { get; set; }
    }
}
