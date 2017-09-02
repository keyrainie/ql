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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Service.Utility;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CostItemInfoVM : ModelBase
    {
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { this.SetValue("SysNo", ref sysNo, value); }
        }

        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { this.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { this.SetValue("ProductID", ref productID, value); }
        }

        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { this.SetValue("ProductName", ref productName, value); }
        }

        private int? poSysNo;

        public int? POSysNo
        {
            get { return poSysNo; }
            set { this.SetValue("POSysNo", ref poSysNo, value); }
        }

        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { this.SetValue("VendorName", ref vendorName, value); }
        }

        private string pmUserName;

        public string PMUserName
        {
            get { return pmUserName; }
            set { this.SetValue("PMUserName", ref pmUserName, value); }
        }

        private int? avaliableQty;

        public int? AvaliableQty
        {
            get { return avaliableQty; }
            set { this.SetValue("AvaliableQty", ref avaliableQty, value); }
        }

        private decimal? cost;

        public decimal? Cost
        {
            get { return cost; }
            set { this.SetValue("Cost", ref cost, value); }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { this.SetValue("CompanyCode", ref companyCode, value); }
        }

        //private bool isCheck;

        //public bool IsCheck
        //{
        //    get { return isCheck; }
        //    set { this.SetValue("IsCheck", ref isCheck, value); }
        //}
    }
}
