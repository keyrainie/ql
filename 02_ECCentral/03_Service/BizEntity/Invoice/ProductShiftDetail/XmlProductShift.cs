using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.Invoice
{
    [XmlRoot("InvoiceNodes", Namespace = null, IsNullable = false)]
    public class XmlProductShift 
    {
        [XmlElement("InvoiceNode")]
        public InvoiceNode InvoiceNode { get; set; }
    }

    public class InvoiceNode
    {
        [XmlElement("OrderSysNo")]
        public string OrderSysNo { get; set; }

        [XmlElement("OrderType")]
        public string OrderType { get; set; }

        [XmlElement("WarehouseNumber")]
        public string WarehouseNumber { get; set; }

        [XmlElement("ComputerNo")]
        public int ComputerNo { get; set; }

        [XmlElement("Items")]
        public Items Items { get; set;}
    }

    public class Items
    {
        [XmlElement("Item")]
        public List<ItemForProductShift> Item{ get; set; }
    }

    public class ItemForProductShift
    {
        public ItemForProductShift()
        {
        }
        public ItemForProductShift(ProductShiftDetailEntity entity, string groubText)
        {
            if (entity.Price.HasValue)
            {
                entity.PriceWithoutTax = (entity.Price.Value / 1.17m);
                entity.Tax = entity.Price - entity.PriceWithoutTax;
            }
            Account = entity.Account;
            Contact = entity.Contact;
            CustomerID = entity.CustomerID;
            CustomerName = entity.CustomerName;
            Discount =entity.Discount.ToString();
            InvoiceType = entity.InvoiceType.ToString();
            IsSplit = entity.IsSplit.ToString();
            Notes = entity.Notes;
            OrderId = groubText;
            OrderType = "SHIFT";
            OrginComputerNo = entity.OrginComputerNo;
            OutTime = entity.OutTime > DateTime.Parse("1990-01-01") ? entity.OutTime.ToString() : DateTime.Now.AddDays(-30).ToString();
            PayType = entity.PayType;
            Price = entity.Price.ToString();
            PriceWithoutTax = entity.PriceWithoutTax.ToString();
            ProductID = entity.ProductID;
            ProductModel = entity.ProductModel;
            ProductName = entity.ProductName;
            Quantity = entity.Quantity.ToString();
            RelatedOrderID = entity.RelatedOrderID;
            Status = entity.Status.ToString();
            Tax = entity.Tax.ToString();
            TaxNumber = entity.TaxNumber;
            Unit = "";
            WarehouseNumber = (entity.WarehouseNumber == 50 || entity.WarehouseNumber == 59) ? "51" : entity.WarehouseNumber.ToString();


            #region 空值处理
            Discount = Discount == "" ? "0" : Discount;
            InvoiceType = InvoiceType == "" ? "0" : InvoiceType; 
            IsSplit = IsSplit == "" ? "0" : IsSplit; 
            Price = Price == "" ? "0" : Price; ;
            Quantity = Quantity == "" ? "0" : Quantity; 
            Status = Status == "" ? "0" : Status;
            OrginComputerNo = OrginComputerNo == "" ? "0" : OrginComputerNo;
            #endregion
        }

        [XmlAttribute("Account")]
        public string Account { get; set; }

        [XmlAttribute("Contact")]
        public string Contact { get; set; }

        [XmlAttribute("CustomerID")]
        public string CustomerID { get; set; }

        [XmlAttribute("CustomerName")]
        public string CustomerName { get; set; }

        [XmlAttribute("Discount")]
        public string Discount { get; set; }

        [XmlAttribute("InvoiceType")]
        public string InvoiceType { get; set; }

        [XmlAttribute("IsSplit")]
        public string IsSplit { get; set; }

        [XmlAttribute("Notes")]
        public string Notes { get; set; }

        [XmlAttribute("OrderId")]
        public string OrderId { get; set; }

        [XmlAttribute("OrderType")]
        public string OrderType { get; set; }

        [XmlAttribute("OrginComputerNo")]
        public string OrginComputerNo { get; set; }

        [XmlAttribute("OutTime")]
        public string OutTime { get; set; }

        [XmlAttribute("PayType")]
        public string PayType { get; set; }

        [XmlAttribute("Price")]
        public string Price { get; set; }

        [XmlAttribute("PriceWithoutTax")]
        public string PriceWithoutTax { get; set; }

        [XmlAttribute("ProductID")]
        public string ProductID { get; set; }

        [XmlAttribute("ProductModel")]
        public string ProductModel { get; set; }

        [XmlAttribute("ProductName")]
        public string ProductName { get; set; }

        [XmlAttribute("Quantity")]
        public string Quantity { get; set; }

        [XmlAttribute("RelatedOrderID")]
        public string RelatedOrderID { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }

        [XmlAttribute("Tax")]
        public string Tax { get; set; }

        [XmlAttribute("TaxNumber")]
        public string TaxNumber { get; set; }

        [XmlAttribute("Unit")]
        public string Unit { get; set; }

        [XmlAttribute("WarehouseNumber")]
        public string WarehouseNumber { get; set; }
    }
}
