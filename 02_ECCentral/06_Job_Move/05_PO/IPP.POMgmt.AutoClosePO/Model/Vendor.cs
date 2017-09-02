using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoClose.Model
{
    public class Vendor
    {
        [DataMapping("Account", DbType.String)]
        public string Account { get; set; }

        [DataMapping("AcctContactEmail", DbType.String)]
        public string AcctContactEmail { get; set; }

        [DataMapping("AcctContactName", DbType.String)]
        public string AcctContactName { get; set; }

        [DataMapping("AcctPhone", DbType.String)]
        public string AcctPhone { get; set; }

        [DataMapping("Address", DbType.String)]
        public string Address { get; set; }

        [DataMapping("Bank", DbType.String)]
        public string Bank { get; set; }

        [DataMapping("BriefName", DbType.String)]
        public string BriefName { get; set; }

        [DataMapping("Cellphone", DbType.String)]
        public string Cellphone { get; set; }

        [DataMapping("Comment", DbType.String)]
        public string Comment { get; set; }

        [DataMapping("Contact", DbType.String)]
        public string Contact { get; set; }

        [DataMapping("ContractAmt", DbType.Decimal)]
        public decimal? ContractAmt { get; set; }

        [DataMapping("CreateVendorTime", DbType.DateTime)]
        public DateTime CreateVendorTime { get; set; }

        [DataMapping("CreateVendorUserSysNo", DbType.Int32)]
        public int CreateVendorUserSysNo { get; set; }

        [DataMapping("District", DbType.String)]
        public string District { get; set; }

        [DataMapping("Email", DbType.String)]
        public string Email { get; set; }

        [DataMapping("EnglishName", DbType.String)]
        public string EnglishName { get; set; }

        [DataMapping("ExpiredDate", DbType.DateTime)]
        public DateTime? ExpiredDate { get; set; }

        [DataMapping("Fax", DbType.String)]
        public string Fax { get; set; }

        [DataMapping("HoldDate", DbType.DateTime)]
        public DateTime HoldDate { get; set; }

        [DataMapping("HoldMark", DbType.Boolean)]
        public bool HoldMark { get; set; }

        [DataMapping("HoldReason", DbType.String)]
        public string HoldReason { get; set; }

        [DataMapping("HoldUser", DbType.Int32)]
        public int HoldUser { get; set; }

        [DataMapping("IsConsign", DbType.Int32)]
        public int IsConsign { get; set; }

        [DataMapping("IsCooperate", DbType.Int32)]
        public int IsCooperate { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }

        [DataMapping("PayPeriod", DbType.Int32)]
        public int PayPeriod { get; set; }

        [DataMapping("PayPeriodType", DbType.Int32)]
        public int PayPeriodType { get; set; }

        [DataMapping("Phone", DbType.String)]
        public string Phone { get; set; }

        [DataMapping("RepairAddress", DbType.String)]
        public string RepairAddress { get; set; }

        [DataMapping("RepairAreaSysNo", DbType.Int32)]
        public int RepairAreaSysNo { get; set; }

        [DataMapping("RepairContact", DbType.String)]
        public string RepairContact { get; set; }

        [DataMapping("RepairContactPhone", DbType.String)]
        public string RepairContactPhone { get; set; }

        [DataMapping("RMAPolicy", DbType.String)]
        public string RMAPolicy { get; set; }

        [DataMapping("RMAServiceArea", DbType.String)]
        public string RMAServiceArea { get; set; }

        [DataMapping("Site", DbType.String)]
        public string Site { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("TaxNo", DbType.String)]
        public string TaxNo { get; set; }

        [DataMapping("TotalPOMoney", DbType.Decimal)]
        public decimal TotalPOMoney { get; set; }

        [DataMapping("ValidDate", DbType.DateTime)]
        public DateTime? ValidDate { get; set; }

        [DataMapping("VendorContractInfo", DbType.String)]
        public string VendorContractInfo { get; set; }

        [DataMapping("VendorID", DbType.String)]
        public string VendorID { get; set; }

        [DataMapping("VendorName", DbType.String)]
        public string VendorName { get; set; }

        [DataMapping("VendorType", DbType.Int32)]
        public int VendorType { get; set; }

        [DataMapping("Zip", DbType.String)]
        public string Zip { get; set; }

        [DataMapping("AuditStatus", DbType.Int32)]
        public int? AuditStatus { get; set; }

        [DataMapping("RANK", DbType.String)]
        public string RANK { get; set; }

        /// <summary>
        /// 申请级别
        /// </summary>
        [DataMapping("RequestRank", DbType.String)]
        public string RequestRank { get; set; }

        [DataMapping("AuditStatusName", DbType.String)]
        public string AuditStatusName { get; set; }

        [DataMapping("StatusName", DbType.String)]
        public string StatusName { get; set; }

        [DataMapping("SysUserName", DbType.String)]
        public string SysUserName { get; set; }

        [DataMapping("PayPeriodTypeName", DbType.String)]
        public string PayPeriodTypeName { get; set; }

        [DataMapping("PayPeriodTypeOld", DbType.Int32)]
        public int PayPeriodTypeOld { get; set; }

        [DataMapping("PayPeriodTypeNew", DbType.Int32)]
        public int PayPeriodTypeNew { get; set; }

        [DataMapping("ValidDateOld", DbType.String)]
        public string ValidDateOld { get; set; }

        [DataMapping("ValidDateNew", DbType.String)]
        public string ValidDateNew { get; set; }

        [DataMapping("ExpiredDateOld", DbType.String)]
        public string ExpiredDateOld { get; set; }

        [DataMapping("ExpiredDateNew", DbType.String)]
        public string ExpiredDateNew { get; set; }

        [DataMapping("ContractAmtOld", DbType.String)]
        public string ContractAmtOld { get; set; }

        [DataMapping("ContractAmtNew", DbType.String)]
        public string ContractAmtNew { get; set; }

        [DataMapping("CitySysNo", DbType.String)]
        public string CitySysNo { get; set; }

        [DataMapping("ProvinceSysNo", DbType.String)]
        public string ProvinceSysNo { get; set; }

        [DataMapping("CreateVendorTimeShow", DbType.String)]
        public string CreateVendorTimeShow { get; set; }
    }
}
