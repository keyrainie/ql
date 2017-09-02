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
using System.Collections.Generic;

namespace ECCentral.Portal.UI.PO.Models
{
    public class PurchaseOrderInfoVM : ModelBase
    {

        public PurchaseOrderInfoVM()
        {
            purchaseOrderBasicInfo = new PurchaseOrderBasicInfoVM();
            vendorInfo = new VendorInfoVM();
            pOItems = new List<PurchaseOrderItemInfoVM>();
            eIMSInfo = new PurchaseOrderEIMSInfoVM();
            receivedInfoList = new List<PurchaseOrderReceivedInfoVM>();
        }

        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { this.SetValue("SysNo", ref sysNo, value); }
        }
        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { this.SetValue("CompanyCode", ref companyCode, value); }
        }

        /// <summary>
        /// 采购单基本信息
        /// </summary>
        private PurchaseOrderBasicInfoVM purchaseOrderBasicInfo;

        public PurchaseOrderBasicInfoVM PurchaseOrderBasicInfo
        {
            get { return purchaseOrderBasicInfo; }
            set { this.SetValue("PurchaseOrderBasicInfo", ref purchaseOrderBasicInfo, value); }
        }

        /// <summary>
        /// 供应商信息
        /// </summary>
        private VendorInfoVM vendorInfo;

        public VendorInfoVM VendorInfo
        {
            get { return vendorInfo; }
            set { this.SetValue("VendorInfo", ref vendorInfo, value); }
        }

        /// <summary>
        /// 采购单商品
        /// </summary>
        private List<PurchaseOrderItemInfoVM> pOItems;

        public List<PurchaseOrderItemInfoVM> POItems
        {
            get { return pOItems; }
            set { this.SetValue("POItems", ref pOItems, value); }
        }

        /// <summary>
        /// 采购单收货信息
        /// </summary>
        private List<PurchaseOrderReceivedInfoVM> receivedInfoList;

        public List<PurchaseOrderReceivedInfoVM> ReceivedInfoList
        {
            get { return receivedInfoList; }
            set { this.SetValue("ReceivedInfoList", ref receivedInfoList, value); }
        }

        /// <summary>
        /// 返点信息
        /// </summary>
        private PurchaseOrderEIMSInfoVM eIMSInfo;

        public PurchaseOrderEIMSInfoVM EIMSInfo
        {
            get { return eIMSInfo; }
            set { this.SetValue("EIMSInfo", ref eIMSInfo, value); }
        }

        private decimal? userdEIMSTotal;
        /// <summary>
        /// 已扣减返点
        /// </summary>
        public decimal? UsedEIMSTotal
        {
            get { return userdEIMSTotal; }
            set { this.SetValue("UsedEIMSTotal", ref userdEIMSTotal, value); }
        }
    }
}
