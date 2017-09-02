using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.SyncSoStatusToWMS.BusinessEntities
{
    public class SoInfo
    {
        public int SoSysno { get; set; }

        public int SoStatus { get; set; }

        public int StockSysNo { get; set; }

        public DateTime? LastChangeStatusDate { get; set; }
    }
}
