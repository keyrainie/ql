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

namespace ECCentral.Portal.UI.Inventory.Models.Request
{
    public class ShiftRequestItemBasketItemVM : ModelBase
    {


        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private int? pMUserSysNo;

        public int? PMUserSysNo
        {
            get { return pMUserSysNo; }
            set { base.SetValue("PMUserSysNo", ref pMUserSysNo, value); }
        }

        private string pMName;

        public string PMName
        {
            get { return pMName; }
            set { base.SetValue("PMName", ref pMName, value); }
        }

        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private int? isConsign;

        public int? IsConsign
        {
            get { return isConsign; }
            set { base.SetValue("IsConsign", ref isConsign, value); }
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

        private int? outStockSysNo;

        public int? OutStockSysNo
        {
            get { return outStockSysNo; }
            set { base.SetValue("OutStockSysNo", ref outStockSysNo, value); }
        }

        private string outStockName;

        public string OutStockName
        {
            get { return outStockName; }
            set { base.SetValue("OutStockName", ref outStockName, value); }
        }

        private int? inStockSysNo;

        public int? InStockSysNo
        {
            get { return inStockSysNo; }
            set { base.SetValue("InStockSysNo", ref inStockSysNo, value); }
        }

        private string inStockName;

        public string InStockName
        {
            get { return inStockName; }
            set { base.SetValue("InStockName", ref inStockName, value); }
        }


        private string shiftQty;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string ShiftQty
        {
            get { return shiftQty; }
            set { base.SetValue("ShiftQty", ref shiftQty, value); }
        }


        private string inUser;

        public string InUser
        {
            get { return inUser; }
            set { base.SetValue("InUser", ref inUser, value); }
        }

        private DateTime? inDate;

        public DateTime? InDate
        {
            get { return inDate; }
            set { base.SetValue("InDate", ref inDate, value); }
        }

        private int? productLineSysNo;
        /// <summary>
        /// 产品线
        /// </summary>
        public int? ProductLineSysNo
        {
            get { return productLineSysNo; }
            set { base.SetValue("ProductLineSysNo", ref productLineSysNo, value); }
        }
    }
}
