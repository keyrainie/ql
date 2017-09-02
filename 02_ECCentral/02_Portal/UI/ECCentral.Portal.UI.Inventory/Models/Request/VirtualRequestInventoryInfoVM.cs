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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class VirtualRequestInventoryInfoVM : ModelBase
    {


        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { SetValue("IsChecked", ref isChecked, value); }
        }

        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { SetValue("StockSysNo", ref stockSysNo, value); }
        }
        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { SetValue("StockName", ref stockName, value); }
        }
        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { SetValue("ProductSysNo", ref productSysNo, value); }
        }
        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { SetValue("ProductID", ref productID, value); }
        }
        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { SetValue("ProductName", ref productName, value); }
        }
        private int? accountQty;

        public int? AccountQty
        {
            get { return accountQty; }
            set { SetValue("AccountQty", ref accountQty, value); }
        }
        private int? availableQty;

        public int? AvailableQty
        {
            get { return availableQty; }
            set { SetValue("AvailableQty", ref availableQty, value); }
        }
        private int? allocatedQty;

        public int? AllocatedQty
        {
            get { return allocatedQty; }
            set { SetValue("AllocatedQty", ref allocatedQty, value); }
        }
        private int? orderQty;

        public int? OrderQty
        {
            get { return orderQty; }
            set { SetValue("OrderQty", ref orderQty, value); }
        }
        private int? virtualQty;

        public int? VirtualQty
        {
            get { return virtualQty; }
            set { SetValue("VirtualQty", ref virtualQty, value); }
        }
        private int? consignQty;

        public int? ConsignQty
        {
            get { return consignQty; }
            set { SetValue("ConsignQty", ref consignQty, value); }
        }
        private int? purchaseQty;

        public int? PurchaseQty
        {
            get { return purchaseQty; }
            set { SetValue("PurchaseQty", ref purchaseQty, value); }
        }
        private string setVirtualQry;

        [Validate(ValidateType.Interger)]
        public string SetVirtualQry
        {
            get { return setVirtualQry; }
            set { SetValue("SetVirtualQry", ref setVirtualQry, value); }
        }
        private string beginDate;

        public string BeginDate
        {
            get { return beginDate; }
            set { SetValue("BeginDate", ref beginDate, value); }
        }
        private string endDate;

        public string EndDate
        {
            get { return endDate; }
            set { SetValue("EndDate", ref endDate, value); }
        }

        private string note;
        [Validate(ValidateType.Required)]
        public string Note
        {
            get { return note; }
            set { SetValue("Note", ref note, value); }
        }
    }
}
