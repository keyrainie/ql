using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    public class AutoAuditSwitchEntity
    {
        public bool IsCheckKeyWords { get; set; }

        public bool IsCheckPayType { get; set; }

        public bool IsCheckShipType { get; set; }

        public bool IsCheckVAT { get; set; }

        public bool IsCheckProductType { get; set; }

        public bool IsCheckFPStatus { get; set; }

        public bool IsCheckCustomerType { get; set; }

        public bool IsCheckPointPromotion { get; set; }

        public bool IsCheckOrderAmt { get; set; }

        public bool IsCheckAutoAuditTime { get; set; }

        public bool IsCheckShipServiceType { get; set; }


    }
}
