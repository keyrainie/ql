using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;
namespace ECCentral.Job.SO.SIMUnicomSO.Logic
{
   public class SOSIMCardEntity  
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("CustomerName", DbType.String)]
        public string CustomerName { get; set; }

        [DataMapping("CertificateType", DbType.Int32)]
        public int CertificateType { get; set; }

        [DataMapping("CertificateValue", DbType.AnsiStringFixedLength)]
        public string CertificateValue { get; set; }

        [DataMapping("CertificateDate", DbType.AnsiStringFixedLength)]
        public string CertificateDate { get; set; }

        [DataMapping("CertificateAddress", DbType.String)]
        public string CertificateAddress { get; set; }

        [DataMapping("CertificateAreaSysno", DbType.Int32)]
        public int? CertificateAreaSysno { get; set; }

        [DataMapping("Address", DbType.String)]
        public string Address { get; set; }

        [DataMapping("ZipCode", DbType.AnsiStringFixedLength)]
        public string ZipCode { get; set; }

        [DataMapping("Phone", DbType.AnsiString)]
        public string Phone { get; set; }

        [DataMapping("ProductSysno", DbType.Int32)]
        public int? ProductSysno { get; set; }

        [DataMapping("CellPhone", DbType.AnsiStringFixedLength)]
        public string CellPhone { get; set; }

        [DataMapping("SuitID", DbType.AnsiStringFixedLength)]
        public string SuitID { get; set; }

        [DataMapping("SuitName", DbType.String)]
        public string SuitName { get; set; }

        public string PaymentType { get; set; }

        [DataMapping("UnicomOrderNo", DbType.AnsiString)]
        public string UnicomOrderNo { get; set; }

        [DataMapping("FirstMonthPaymethod", DbType.String)]
        public string FirstMonthPaymethod { get; set; }

        [DataMapping("FirstMonthPaymethodNO", DbType.String)]
        public int FirstMonthPaymethodNO { get; set; }

        [DataMapping("SIMSN", DbType.AnsiString)]
        public string SIMSN { get; set; }

        [DataMapping("SIMStatus", DbType.AnsiStringFixedLength)]
        public string SIMStatus { get; set; }

        [DataMapping("Memo", DbType.String)]
        public string Memo { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("CompanyCode", DbType.AnsiStringFixedLength)]
        public string CompanyCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.AnsiString)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.AnsiStringFixedLength)]
        public string LanguageCode { get; set; }

        [DataMapping("IMEI", DbType.String)]
        public string IMEI { get; set; }
    }
}
