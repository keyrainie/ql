using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class QueryLoginStatisticCriteria
    {
        public int TopCount { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
