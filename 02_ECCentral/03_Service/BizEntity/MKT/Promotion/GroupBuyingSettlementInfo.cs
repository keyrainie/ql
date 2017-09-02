using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    public class GroupBuyingSettlementInfo : IIdentity, ICompany
    {
        public int? SysNo { get; set; }
        public DateTime? AuditDate { get; set; }
        public int? AuditUserSysNo { get; set; }
        public decimal? SettleAmt { get; set; }
        public SettlementBillStatus? Status { get; set; }               
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string EditUser { get; set; }
        public DateTime? EditDate { get; set; }
        public string CompanyCode { get; set; }
        public int? VendorSysNo { get; set; }
        public string EditUserName { get; set; }
    }
}
