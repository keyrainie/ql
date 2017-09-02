using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.IM
{
    [Serializable]
    [DataContract]
    public class ProductBatchManagementInfo
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public int? ProductSysNo { get; set; }

        [DataMember]
        public bool? IsBatch { get; set; }

        [DataMember]
        public CollectDateType? CollectType { get; set; }

        [DataMember]
        public bool? IsCollectBatchNo { get; set; }

        [DataMember]
        public int? MinReceiptDays { get; set; }

        [DataMember]
        public int? MaxDeliveryDays { get; set; }

        [DataMember]
        public int? GuaranteePeriodYear { get; set; }

        [DataMember]
        public int? GuaranteePeriodMonth { get; set; }

        [DataMember]
        public int? GuaranteePeriodDay { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public List<ProductBatchManagementInfoLog> Logs { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ProductBatchManagementInfoLog
    {
        [DataMember]
        public int? BatchManagementSysNo { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public DateTime? InDate { get; set; }
    }
}
