using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SendNoticeMailForGiftCard
{
    [XmlRoot("Message", Namespace = null, IsNullable = false)]
    public class Message
    {
        [XmlElement("Header")]
        public Header Header { get; set; }
        [XmlElement("Body")]
        public Body Body { get; set; }
    }
    public class Header
    {
        [XmlElement("Action")]
        public string Action { get; set; }
        [XmlElement("Version")]
        public string Version { get; set; }
        [XmlElement("From")]
        public string From { get; set; }
        [XmlElement("CurrencySysNo")]
        public string CurrencySysNo { get; set; }
        [XmlElement("Language")]
        public string Language { get; set; }
        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }
        [XmlElement("StoreCompanyCode")]
        public string StoreCompanyCode { get; set; }
    }
    public class Body
    {
        [XmlElement("Memo")]
        public string Memo { get; set; }
        [XmlElement("EditUser")]
        public string EditUser { get; set; }
        [XmlElement("GiftCard")]
        public List<GiftCard> GiftCard { get; set; }
    }
    public class GiftCard
    {
        [XmlElement("Code")]
        public string Code { get; set; }
        [XmlElement("ConsumeAmount")]
        public string ConsumeAmount { get; set; }
        [XmlElement("CustomerSysno")]
        public string CustomerSysno { get; set; }
        [XmlElement("ReferenceSysno")]
        public string ReferenceSysno { get; set; }

        [XmlElement("InternalType")]
        public string InternalType { get; set; }

        [XmlElement("ItemInfo")]
        public ItemInfo ItemInfo { get; set; }
    }
    public class ItemInfo
    {
        [XmlElement("Item")]
        public List<Item> Item { get; set; }
    }
    public class Item
    {
        [XmlElement("TotalAmount")]
        public string TotalAmount { get; set; }
        [XmlElement("Quantity")]
        public string Quantity { get; set; }       
    }
}
