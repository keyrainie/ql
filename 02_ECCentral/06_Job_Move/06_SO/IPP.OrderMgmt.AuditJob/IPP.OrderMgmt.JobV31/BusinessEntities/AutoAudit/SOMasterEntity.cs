using System;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
 public   class SOMasterEntity:EntityBase
    {
        [DataMapping("AllocatedManSysNo", DbType.Int32)]
        public int AllocatedManSysNo
        {
            get;
            set;
        }

        [DataMapping("OrderDate", DbType.DateTime)]
        public DateTime OrderDate
        {
            get;
            set;
        }

        [DataMapping("AuditTime", DbType.DateTime)]
        public DateTime AuditTime
        {
            get;
            set;
        }

        [DataMapping("AuditUserSysNo", DbType.Int32)]
        public int AuditUserSysNo
        {
            get;
            set;
        }

        [DataMapping("CashPay", DbType.Decimal)]
        public decimal CashPay
        {
            get;
            set;
        }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo
        {
            get;
            set;
        }

        [DataMapping("DeliveryDate", DbType.DateTime)]
        public DateTime DeliveryDate
        {
            get;
            set;
        }

        [DataMapping("DeliveryMemo", DbType.String)]
        public string DeliveryMemo
        {
            get;
            set;
        }

        [DataMapping("DeliveryTimeRange", DbType.Int32)]
        public int DeliveryTimeRange
        {
            get;
            set;
        }

        [DataMapping("DiscountAmt", DbType.Decimal)]
        public decimal DiscountAmt
        {
            get;
            set;
        }

        [DataMapping("FinanceNote", DbType.String)]
        public string FinanceNote
        {
            get;
            set;
        }

        [DataMapping("FreightUserSysNo", DbType.Int32)]
        public int FreightUserSysNo
        {
            get;
            set;
        }

        [DataMapping("HaveAutoRMA", DbType.Int32)]
        public int HaveAutoRMA
        {
            get;
            set;
        }

        [DataMapping("HoldDate", DbType.DateTime)]
        public DateTime HoldDate
        {
            get;
            set;
        }

        [DataMapping("HoldMark", DbType.Boolean)]
        public bool? HoldMark
        {
            get;
            set;
        }

        [DataMapping("HoldReason", DbType.String)]
        public string HoldReason
        {
            get;
            set;
        }

        [DataMapping("HoldUser", DbType.Int32)]
        public int HoldUser
        {
            get;
            set;
        }

        [DataMapping("InvoiceNo", DbType.String)]
        public string InvoiceNo
        {
            get;
            set;
        }

        [DataMapping("InvoiceNote", DbType.String)]
        public string InvoiceNote
        {
            get;
            set;
        }

        [DataMapping("IsLarge", DbType.Int32)]
        public int? IsLarge
        {
            get;
            set;
        }

        [DataMapping("IsMobilePhone", DbType.Int32)]
        public int? IsMobilePhone
        {
            get;
            set;
        }

        [DataMapping("IsPremium", DbType.Int32)]
        public int? IsPremium
        {
            get;
            set;
        }

        [DataMapping("IsPrintPackageCover", DbType.Int32)]
        public int? IsPrintPackageCover
        {
            get;
            set;
        }

        [DataMapping("IsUseChequesPay", DbType.Int32)]
        public int? IsUseChequesPay
        {
            get;
            set;
        }

        [DataMapping("IsUsePrepay", DbType.Int32)]
        public int? IsUsePrepay
        {
            get;
            set;
        }

        [DataMapping("IsVAT", DbType.Int32)]
        public int? IsVAT
        {
            get;
            set;
        }

        [DataMapping("IsWholeSale", DbType.Int32)]
        public int? IsWholeSale
        {
            get;
            set;
        }

        [DataMapping("ManagerAuditTime", DbType.DateTime)]
        public DateTime ManagerAuditTime
        {
            get;
            set;
        }

        [DataMapping("ManagerAuditUserSysNo", DbType.Int32)]
        public int ManagerAuditUserSysNo
        {
            get;
            set;
        }

        [DataMapping("Memo", DbType.String)]
        public string Memo
        {
            get;
            set;
        }

        [DataMapping("Note", DbType.String)]
        public string Note
        {
            get;
            set;
        }

        [DataMapping("OutTime", DbType.DateTime)]
        public DateTime OutTime
        {
            get;
            set;
        }

        [DataMapping("OutUserSysNo", DbType.Int32)]
        public int OutUserSysNo
        {
            get;
            set;
        }

        [DataMapping("PackageID", DbType.String)]
        public string PackageID
        {
            get;
            set;
        }

        [DataMapping("PayPrice", DbType.Decimal)]
        public decimal PayPrice
        {
            get;
            set;
        }

        [DataMapping("PayTypeSysNo", DbType.Int32)]
        public int PayTypeSysNo
        {
            get;
            set;
        }

        [DataMapping("PointAmt", DbType.Int32)]
        public int PointAmt
        {
            get;
            set;
        }

        [DataMapping("PointPay", DbType.Int32)]
        public int PointPay
        {
            get;
            set;
        }

        [DataMapping("PremiumAmt", DbType.Decimal)]
        public decimal PremiumAmt
        {
            get;
            set;
        }

        [DataMapping("PrepayAmt", DbType.Decimal)]
        public decimal PrepayAmt
        {
            get;
            set;
        }

        [DataMapping("PromotionCode", DbType.String)]
        public string PromotionCode
        {
            get;
            set;
        }

        [DataMapping("PromotionCodeSysNo", DbType.Int32)]
        public int? PromotionCodeSysNo
        {
            get;
            set;
        }

        [DataMapping("PromotionValue", DbType.Decimal)]
        public decimal PromotionValue
        {
            get;
            set;
        }

        [DataMapping("ReceiveAddress", DbType.String)]
        public string ReceiveAddress
        {
            get;
            set;
        }

        [DataMapping("ReceiveAreaSysNo", DbType.Int32)]
        public int ReceiveAreaSysNo
        {
            get;
            set;
        }

        [DataMapping("ReceiveCellPhone", DbType.String)]
        public string ReceiveCellPhone
        {
            get;
            set;
        }

        [DataMapping("ReceiveContact", DbType.String)]
        public string ReceiveContact
        {
            get;
            set;
        }

        [DataMapping("ReceiveName", DbType.String)]
        public string ReceiveName
        {
            get;
            set;
        }

        [DataMapping("ReceivePhone", DbType.String)]
        public string ReceivePhone
        {
            get;
            set;
        }

        [DataMapping("ReceiveZip", DbType.String)]
        public string ReceiveZip
        {
            get;
            set;
        }

        [DataMapping("SalesManSysNo", DbType.Int32)]
        public int SalesManSysNo
        {
            get;
            set;
        }

        [DataMapping("ShipPrice", DbType.Decimal)]
        public decimal ShipPrice
        {
            get;
            set;
        }

        [DataMapping("ShipTypeSysNo", DbType.Int32)]
        public int ShipTypeSysNo
        {
            get;
            set;
        }

        [DataMapping("ShoppingMasterSysNo", DbType.Int32)]
        public int ShoppingMasterSysNo
        {
            get;
            set;
        }

        [DataMapping("SOAmt", DbType.Decimal)]
        public decimal SOAmt
        {
            get;
            set;
        }

        [DataMapping("SOID", DbType.String)]
        public string SOID
        {
            get;
            set;
        }

        [DataMapping("Status", DbType.Int32)]
        public int? Status
        {
            get;
            set;
        }

        [DataMapping("CompanyCode", DbType.AnsiStringFixedLength)]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMapping("StoreCompanyCode", DbType.AnsiStringFixedLength)]
        public string StoreCompanyCode
        {
            get;
            set;
        }

        [DataMapping("IsFPSO", DbType.Int32)]
        public int? IsFPSO
        {
            get;
            set;
        }

        [DataMapping("IsFPCheck", DbType.Int32)]
        public int? IsFPCheck
        {
            get;
            set;
        }

        [DataMapping("IsDuplicateOrder", DbType.Int32)]
        public int IsDuplicateOrder
        {
            get;
            set;
        }

        [DataMapping("IsPayWhenRecv", DbType.Int32)]
        public int IsPayWhenRecv
        {
            get;
            set;
        }

        // 分期付款扩展属性，考虑中。
        public int InstalmentSysNo
        {
            get;
            set;
        }

        public decimal TotalAmount
        {
            get
            {
                decimal totalAmt =
                    this.CashPay
                    + this.PayPrice
                    + this.ShipPrice
                    + this.PremiumAmt
                    + this.DiscountAmt;

                return (totalAmt > 0M) ? totalAmt : 0M;
            }
        }

        /// <summary>
        /// 应收款=订单金额合计-现金帐户预付款
        /// </summary>
        public decimal GetReceivableAmt()
        {
            return Math.Max((this.TotalAmount - this.PrepayAmt), 0M);
        }
    }
}
