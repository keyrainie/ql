using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.Income
{
    public class SOFreightStatDetail : IIdentity
    {
        public int? SysNo { get; set; }

        public int? SOSysNo { get; set; }

        public string TrackingNumber { get; set; }

        public int? ShipTypeSysNo { get; set; }

        public decimal? SOWeight { get; set; }

        public decimal? SOFreight { get; set; }

        public decimal? RealWeight { get; set; }

        public decimal? RealPayFreight { get; set; }

        public decimal? RealOutFreight { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? OutDate { get; set; }

        public CheckStatus? SOFreightConfirm { get; set; }

        public DateTime? SOFreightConfirmDate { get; set; }

        public CheckStatus? RealFreightConfirm { get; set; }

        public DateTime? RealFreightConfirmDate { get; set; }

        public int? CurrencySysNo { get; set; }
    }
}
