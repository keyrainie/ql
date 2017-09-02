using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.VendorPortal
{
    [Serializable]
    [XmlRoot("POMaster")]
    public class VendorPortalPOAuditMessage : VendorPortalMessageBodyBase, IEventMessage
    {
        [XmlElement("CompanyCode")]
        public string CompanyCode
        {
            get;
            set;
        }
        [XmlElement("CreateUserSysNo")]
        public string CreateUserSysNo
        {
            get;
            set;
        }
        [XmlElement("CurrencySysNo")]
        public int CurrencySysNo
        {
            get;
            set;
        }
        [XmlElement("ETAHalfDay")]
        public string ETAHalfDay
        {
            get;
            set;
        }
        [XmlElement("ETATime")]
        public DateTime ETATime
        {
            get;
            set;
        }
        [XmlElement("ExchangeRate")]
        public decimal ExchangeRate
        {
            get;
            set;
        }
        [XmlElement("IPPSysNo")]
        public int? IPPSysNo
        {
            get;
            set;
        }
        [XmlElement("IsConsign")]
        public int IsConsign
        {
            get;
            set;
        }
        [XmlElement("ITStockSysNo")]
        public int? ITStockSysNo
        {
            get;
            set;
        }
        [XmlElement("Memo")]
        public string Memo
        {
            get;
            set;
        }
        [XmlElement("PayTypeSysNo")]
        public int PayTypeSysNo
        {
            get;
            set;
        }
        [XmlElement("PMComfirmTime")]
        public DateTime? PMComfirmTime
        {
            get;
            set;
        }
        [XmlElement("PMSysNo")]
        public int PMSysNo
        {
            get;
            set;
        }
        [XmlArray("POItems"), XmlArrayItem("POItem")]
        public List<VendorPortalPORequestItemSSBMessage> POItems
        {
            get;
            set;
        }
        [XmlElement("POType")]
        public int POType
        {
            get;
            set;
        }
        [XmlElement("ShipTypeSysNo")]
        public int ShipTypeSysNo
        {
            get;
            set;
        }
        [XmlElement("Status")]
        public string Status
        {
            get;
            set;
        }
        [XmlElement("StockSysNo")]
        public int StockSysNo
        {
            get;
            set;
        }
        [XmlElement("SysNo")]
        public int SysNo
        {
            get;
            set;
        }
        [XmlElement("TaxRate")]
        public decimal TaxRate
        {
            get;
            set;
        }
        [XmlElement("TotalAmt")]
        public decimal TotalAmt
        {
            get;
            set;
        }
        [XmlElement("VendorSysNo")]
        public int VendorSysNo
        {
            get;
            set;
        }
        [XmlElement("VPOID")]
        public string VPOID
        {
            get;
            set;
        }

        public string Subject
        {
            get { return "VendorPortalPOAuditMessage"; }
        }
    }

    [Serializable]
    public class VendorPortalPORequestItemSSBMessage : IEventMessage
    {
        // Properties
        [XmlElement("BriefName")]
        public string BriefName
        {
            get;
            set;
        }
        [XmlElement("CompanyCode")]
        public string CompanyCode
        {
            get;
            set;
        }
        [XmlElement("CurrencySysNo")]
        public int CurrencySysNo
        {
            get;
            set;
        }
        [XmlElement("OrderPrice")]
        public decimal OrderPrice
        {
            get;
            set;
        }
        [XmlElement("POSysNo")]
        public int POSysNo
        {
            get;
            set;
        }
        [XmlElement("ProductID")]
        public string ProductID
        {
            get;
            set;
        }
        [XmlElement("ProductSysNo")]
        public int ProductSysNo
        {
            get;
            set;
        }
        [XmlElement("PurchaseQty")]
        public int PurchaseQty
        {
            get;
            set;
        }
        [XmlElement("Quantity")]
        public int Quantity
        {
            get;
            set;
        }
        [XmlElement("SysNo")]
        public int SysNo
        {
            get;
            set;
        }
        [XmlElement("UnitCost")]
        public decimal UnitCost
        {
            get;
            set;
        }
        [XmlElement("UnitCostWithoutTax")]
        public decimal UnitCostWithoutTax
        {
            get;
            set;
        }
        [XmlElement("Weight")]
        public int Weight
        {
            get;
            set;
        }

        public string Subject
        {
            get { return "VendorPortalPORequestItemSSBMessage"; }
        }
    }
}