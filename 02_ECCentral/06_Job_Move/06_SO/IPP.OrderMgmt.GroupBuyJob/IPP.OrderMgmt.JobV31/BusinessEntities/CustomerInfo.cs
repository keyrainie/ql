using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    [Serializable]
    public class CustomerInfo : EntityBase
    {
        [DataMapping("AuctionRank", DbType.Int32)]
        public int? AuctionRank { get; set; }

        [DataMapping("AvailableCreditLimit", DbType.Decimal)]
        public decimal? AvailableCreditLimit { get; set; }

        [DataMapping("Birthday", DbType.DateTime)]
        public DateTime? Birthday { get; set; }

        [DataMapping("CellPhone", DbType.String)]
        public string CellPhone { get; set; }

        [DataMapping("CompanyCustomer", DbType.Int32)]
        public int? CompanyCustomer { get; set; }

        [DataMapping("ConfirmedTotalAmt", DbType.Decimal)]
        public decimal? ConfirmedTotalAmt { get; set; }

        [DataMapping("CustomerID", DbType.String)]
        public string CustomerID { get; set; }

        [DataMapping("CustomerName", DbType.String)]
        public string CustomerName { get; set; }

        [DataMapping("DwellAddress", DbType.String)]
        public string DwellAddress { get; set; }

        [DataMapping("DwellAreaSysNo", DbType.Int32)]
        public int? DwellAreaSysNo { get; set; }

        [DataMapping("DwellZip", DbType.String)]
        public string DwellZip { get; set; }

        [DataMapping("Email", DbType.String)]
        public string Email { get; set; }

        [DataMapping("Fax", DbType.String)]
        public string Fax { get; set; }

        [DataMapping("FromLinkSource", DbType.String)]
        public string FromLinkSource { get; set; }

        [DataMapping("Gender", DbType.Int32)]
        public int? Gender { get; set; }

        [DataMapping("IsAllowComment", DbType.Int32)]
        public int? IsAllowComment { get; set; }

        [DataMapping("IsEmailConfirmed", DbType.Int32)]
        public int? IsEmailConfirmed { get; set; }

        [DataMapping("IsSubscribe", DbType.Int32)]
        public int? IsSubscribe { get; set; }

        [DataMapping("IsUseChequesPay", DbType.Int32)]
        public int? IsUseChequesPay { get; set; }

        [DataMapping("LastPayTypeSysNo", DbType.Int32)]
        public int? LastPayTypeSysNo { get; set; }

        [DataMapping("LastReceiveAreaSysNo", DbType.Int32)]
        public int? LastReceiveAreaSysNo { get; set; }

        [DataMapping("LastShipTypeSysNo", DbType.Int32)]
        public int? LastShipTypeSysNo { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }

        [DataMapping("PayDays", DbType.Int32)]
        public int? PayDays { get; set; }

        [DataMapping("Phone", DbType.String)]
        public string Phone { get; set; }

        [DataMapping("PointExpiringDate", DbType.DateTime)]
        public DateTime? PointExpiringDate { get; set; }

        [DataMapping("PromotionRankSign", DbType.Int32)]
        public int? PromotionRankSign { get; set; }

        [DataMapping("Rank", DbType.Int32)]
        public int? Rank { get; set; }

        [DataMapping("RecommendedByCustomerID", DbType.String)]
        public string RecommendedByCustomerID { get; set; }

        [DataMapping("RegisterTime", DbType.DateTime)]
        public DateTime? RegisterTime { get; set; }

        [DataMapping("SendCustomerRankEmailDate", DbType.DateTime)]
        public DateTime? SendCustomerRankEmailDate { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int? Status { get; set; }

        [DataMapping("StudentFlag", DbType.String)]
        public string StudentFlag { get; set; }

        [DataMapping("TotalCreditLimit", DbType.Decimal)]
        public decimal? TotalCreditLimit { get; set; }

        [DataMapping("TotalScore", DbType.Int32)]
        public int? TotalScore { get; set; }

        [DataMapping("TotalSOMoney", DbType.Decimal)]
        public decimal? TotalSOMoney { get; set; }

        [DataMapping("ValidPrepayAmt", DbType.Decimal)]
        public decimal ValidPrepayAmt { get; set; }

        [DataMapping("ValidScore", DbType.Int32)]
        public int? ValidScore { get; set; }

        [DataMapping("VIPRank", DbType.Int32)]
        public int? VIPRank { get; set; }

        [DataMapping("IsBadCustomer", DbType.Int32)]
        public int? IsBadCustomer { get; set; }
    }
}
