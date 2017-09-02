using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.SO
{
    [Serializable]
    public class SOLogNote
    {
        public string ActionName { get; set; }

        public int? SOSysNo { get; set; }
        public int? CustomerSysNo { get; set; }
        public int? RecvSysNo { get; set; }
        public string RecvAddress { get; set; }
        public int? ShipType { get; set; }
        public int? PayType { get; set; }

        public List<SOLogItemEntity> SOItems { get; set; }
    }
    [Serializable]
    public class SOLogItemEntity
    {
        public int? ProductSysNo { get; set; }
        public int? Qty { get; set; }
        public Decimal? Price { get; set; }
    }
}
