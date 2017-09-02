using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.VendorPortal
{
    [Serializable]
    [XmlRoot("POMaster")]
    public class VendorPortalPOConfirmMessage : VendorPortalMessageBodyBase, IEventMessage
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
            get { return "VendorPortalPOConfirmMessage"; }
        }
    }
}
