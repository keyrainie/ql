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

namespace ECCentral.Portal.UI.MKT.Models.Floor
{
    public class FloorSectionProductVM : FloorSectionItemVM
    {
        private int productSysNo ;
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

        private string title;
        public string ProductTitle
        {
            get { return title; }
            set { base.SetValue("ProductTitle", ref title, value); }
        }

        private string subTitle;
        public string ProductSubTitle
        {
            get { return subTitle; }
            set { base.SetValue("ProductSubTitle", ref subTitle, value); }
        }

        private decimal price;
        public decimal ProductPrice
        {
            get { return price; }
            set { base.SetValue("ProductPrice", ref price, value); }
        }

        private decimal discountPrice;
        public decimal ProductDiscount
        {
            get { return discountPrice; }
            set { base.SetValue("ProductDiscount", ref discountPrice, value); }
        }

        private string imageUrl;
        public string DefaultImage
        {
            get { return imageUrl; }
            set { base.SetValue("DefaultImage", ref imageUrl, value); }
        }

        //private int priority;
        //public int Priority
        //{
        //    get { return priority; }
        //    set { base.SetValue("Priority", ref priority, value); }
        //}
    }
}
