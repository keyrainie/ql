using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// RMA移仓单明细表
    /// </summary>
    [Serializable]
    [DataContract]
    public class RMAShiftInfo
    {
        [DataMember]
        public int RegisterSysNo { get; set; }

        [DataMember]
        public string ProductID { get; set; }

        [DataMember]
        public string TargetBriefName { get; set; }

        [DataMember]
        public int TargetProductQty { get; set; }

        [DataMember]
        public string ShippedWarehouseName { get; set; }

        [DataMember]
        public int ShiftSysNo { get; set; }

        [DataMember]
        public RMAShiftType ShiftType { get; set; }

        [DataMember]
        public RMAShiftStatus Status { get; set; }

        [DataMember]
        public string StockSysNoAName { get; set; }

        [DataMember]
        public string StockSysNoBName { get; set; }

        [DataMember]
        public string Note { get; set; }
    }
}
