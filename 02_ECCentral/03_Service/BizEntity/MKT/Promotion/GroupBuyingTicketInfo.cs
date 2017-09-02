using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.BizEntity.MKT
{
    public class GroupBuyingTicketInfo : IIdentity, ICompany
    {
        public int? SysNo { get; set; }
        public int? GroupBuyingSysNo { get; set; }
        public int? OrderSysNo { get; set; }
        public string TicketID { get; set; }
        public decimal? TicketAmt { get; set; }
        public decimal? CostAmt { get; set; }
        public int? PayType { get; set; }
        public GroupBuyingTicketStatus? Status { get; set; }
        public RefundStatus? RefundStatus { get; set; }
        public string RefundMemo { get; set; }
        public DateTime? AvailableDate { get; set; }
        public string Tel { get; set; }
        public int? UsedStoreSysNo { get; set; }
        public DateTime? UsedDate { get; set; }
        public int? CustomerSysNo { get; set; }
        public int? VenderSysNo { get; set; }
        public int? RefundUser { get; set; }
        public DateTime? RefundDate { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? EditUser { get; set; }
        public DateTime? EditDate { get; set; }
        public string CompanyCode { get; set; }
    }
}
