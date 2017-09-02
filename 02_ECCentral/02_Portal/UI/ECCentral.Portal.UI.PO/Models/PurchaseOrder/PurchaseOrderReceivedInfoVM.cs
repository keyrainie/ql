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

namespace ECCentral.Portal.UI.PO.Models
{
    /// <summary>
    /// 采购单收货信息
    /// </summary>
    public class PurchaseOrderReceivedInfoVM : ModelBase
    {
        private int batchNumber;  //批次号

        public int BatchNumber
        {
            get { return batchNumber; }
            set { base.SetValue("BatchNumber", ref batchNumber, value); }
        }

        private int productSysNo;

        public int ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }

        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { base.SetValue("ProductName", ref productName, value); }
        }

        private string pOSysNo;  //PO单号

        public string POSysNo
        {
            get { return pOSysNo; }
            set { base.SetValue("POSysNo", ref pOSysNo, value); }
        }

        private int receivedQuantity;  //本次入库数量

        public int ReceivedQuantity
        {
            get { return receivedQuantity; }
            set { base.SetValue("ReceivedQuantity", ref receivedQuantity, value); }
        }

        private int purchaseQty;  //采购数量  

        public int PurchaseQty
        {
            get { return purchaseQty; }
            set { base.SetValue("PurchaseQty", ref purchaseQty, value); }
        }

        private int waitInQty;  //剩余待入库数量

        public int WaitInQty
        {
            get { return waitInQty; }
            set { base.SetValue("WaitInQty", ref waitInQty, value); }
        }

        private int totalReceQty;  //总入库数量

        public int TotalReceQty
        {
            get { return totalReceQty; }
            set { base.SetValue("TotalReceQty", ref totalReceQty, value); }
        }

        private DateTime receivedDate;  //入库日期

        public DateTime ReceivedDate
        {
            get { return receivedDate; }
            set { base.SetValue("ReceivedDate", ref receivedDate, value); }
        }
    }
}
