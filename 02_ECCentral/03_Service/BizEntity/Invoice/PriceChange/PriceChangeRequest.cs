using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.Invoice.PriceChange
{
    [DataContract]
    public class PriceChangeMaster
    {
        [DataMember]
        public int SysNo { get; set; }

        [DataMember]
        public RequestPriceType PriceType { get; set; }

        [DataMember]
        public DateTime? BeginDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public DateTime? RealBeginDate { get; set; }

        [DataMember]
        public string Memo { get; set; }

        [DataMember]
        public string AuditMemo { get; set; }

        [DataMember]
        public RequestPriceStatus Status { get; set; }

        [DataMember]
        public int AuditUser { get; set; }

        [DataMember]
        public DateTime? AuditDate { get; set; }

        [DataMember]
        public int InUser { get; set; }

        [DataMember]
        public DateTime? InDate { get; set; }

        [DataMember]
        public int EditUser { get; set; }

        [DataMember]
        public DateTime? EditDate { get; set; }

        [DataMember]
        public List<PriceChangeItem> ItemList { get; set; }
    }

    [DataContract]
    public class PriceChangeItem
    {
        [DataMember]
        public int SysNo { get; set; }

        [DataMember]
        public int MasterSysNo { get; set; }

        [DataMember]
        public int ProductsysNo { get; set; }

        [DataMember]
        public string ProductID { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public Decimal OldShowPrice { get; set; }

        [DataMember]
        public Decimal NewShowPrice { get; set; }

        [DataMember]
        public Decimal OldPrice { get; set; }

        [DataMember]
        public Decimal NewPrice { get; set; }

        [DataMember]
        public Decimal OldInstockPrice { get; set; }

        [DataMember]
        public Decimal NewInstockPrice { get; set; }

        [DataMember]
        public Decimal UnitCost { get; set; }

        [DataMember]
        public Decimal CurrentPrice { get; set; }

        [DataMember]
        public Decimal MinMargin { get; set; }

        [DataMember]
        public PriceChangeItemStatus? Status { get; set; }
    }
}
