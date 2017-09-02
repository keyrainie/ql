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
using System.Collections.Generic;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignToAccountLogQueryVM : ModelBase
    {
        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }
        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { base.SetValue("ProductName", ref productName, value); }
        }
        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }
        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { base.SetValue("VendorName", ref vendorName, value); }
        }
        private ConsignToAccountLogStatus? status;

        public ConsignToAccountLogStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        private ConsignToAccountType? consignToAccType;

        public ConsignToAccountType? ConsignToAccType
        {
            get { return consignToAccType; }
            set { base.SetValue("ConsignToAccType", ref consignToAccType, value); }
        }
        private DateTime? createTimeFrom;

        public DateTime? CreateTimeFrom
        {
            get { return createTimeFrom; }
            set { base.SetValue("CreateTimeFrom", ref createTimeFrom, value); }
        }
        private DateTime? createTimeTo;

        public DateTime? CreateTimeTo
        {
            get { return createTimeTo; }
            set { base.SetValue("CreateTimeTo", ref createTimeTo, value); }
        }
        private SettleType? settleType;

        public SettleType? SettleType
        {
            get { return settleType; }
            set { base.SetValue("SettleType", ref settleType, value); }
        }
        private int? accLogType;

        public int? AccLogType
        {
            get { return accLogType; }
            set { base.SetValue("AccLogType", ref accLogType, value); }
        }
        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        public List<int> SysNoList { get; set; }
    }
}
