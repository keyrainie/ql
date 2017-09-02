using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CollectionPaymentQueryVM : ModelBase
    {
        /// <summary>
        /// 结算单编号
        /// </summary>
        private string settleID;

        public string SettleID
        {
            get { return settleID; }
            set { base.SetValue("SettleID", ref settleID, value); }
        }
        /// <summary>
        /// 状态
        /// </summary>
        private POCollectionPaymentSettleStatus? status;

        public POCollectionPaymentSettleStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        /// <summary>
        /// 付款结算公司
        /// </summary>
        private PaySettleCompany? paySettleCompany;

        public PaySettleCompany? PaySettleCompany
        {
            get { return paySettleCompany; }
            set { base.SetValue("PaySettleCompany", ref paySettleCompany, value); }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        private string createUser;

        public string CreateUser
        {
            get { return createUser; }
            set { base.SetValue("CreateUser", ref createUser, value); }
        }
        /// <summary>
        /// 仓库
        /// </summary>
        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }
        /// <summary>
        /// 供应商编号
        /// </summary>
        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
        /// <summary>
        /// 供应商名称
        /// </summary>
        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { base.SetValue("VendorName", ref vendorName, value); }
        }
        /// <summary>
        /// 创建时间 -开始
        /// </summary>
        private DateTime? createDateFrom;

        public DateTime? CreateDateFrom
        {
            get { return createDateFrom; }
            set { base.SetValue("CreateDateFrom", ref createDateFrom, value); }
        }
        /// <summary>
        /// 创建时间 -结束
        /// </summary>
        private DateTime? createDateTo;

        public DateTime? CreateDateTo
        {
            get { return createDateTo; }
            set { base.SetValue("CreateDateTo", ref createDateTo, value); }
        }


        private DateTime? auditDateFrom;

        public DateTime? AuditDateFrom
        {
            get { return auditDateFrom; }
            set { base.SetValue("AuditDateFrom", ref auditDateFrom, value); }
        }
        private DateTime? auditDateTo;

        public DateTime? AuditDateTo
        {
            get { return auditDateTo; }
            set { base.SetValue("AuditDateTo", ref auditDateTo, value); }
        }

        /// <summary>
        /// 结算日期 -开始
        /// </summary>
        private DateTime? settledDateFrom;

        public DateTime? SettledDateFrom
        {
            get { return settledDateFrom; }
            set { base.SetValue("SettledDateFrom", ref settledDateFrom, value); }
        }
        /// <summary>
        /// 结算日期 -结束
        /// </summary>
        private DateTime? settledDateTo;

        public DateTime? SettledDateTo
        {
            get { return settledDateTo; }
            set { base.SetValue("SettledDateTo", ref settledDateTo, value); }
        }
        /// <summary>
        /// 备注
        /// </summary>
        private string memo;

        public string Memo
        {
            get { return memo; }
            set { base.SetValue("Memo", ref memo, value); }
        }
        /// <summary>
        /// 归属PM
        /// </summary>
        private int? pMSysno;

        public int? PMSysno
        {
            get { return pMSysno; }
            set { base.SetValue("PMSysno", ref pMSysno, value); }
        }

        /// <summary>
        /// 是否自动结算
        /// </summary>
        private string isAutoSettle;

        public string IsAutoSettle
        {
            get { return isAutoSettle; }
            set { base.SetValue("IsAutoSettle", ref isAutoSettle, value); }
        }
        private int? accLogType;

        public int? AccLogType
        {
            get { return accLogType; }
            set { base.SetValue("AccLogType", ref accLogType, value); }
        }
        /// <summary>
        /// 公司编码
        /// </summary>
        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }
       
        
    }
}
