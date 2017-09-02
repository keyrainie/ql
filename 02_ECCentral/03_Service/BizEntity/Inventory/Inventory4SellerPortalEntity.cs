using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.Inventory
{
    #region Base Entity

    [Serializable]
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class SSBEntityBaseV3
    {
        [XmlElement("FromService")]
        public string FromService { get; set; }

        [XmlElement("ToService")]
        public string ToService { get; set; }

        [XmlElement("RouteTable")]
        public RouteTable RouteTable { get; set; }

    }

    public class RouteTable
    {
        [XmlElement("Article", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService")]
        public Article Article { get; set; }
    }

    public class Article
    {
        [XmlElement("ArticleCategory")]
        public string ArticleCategory { get; set; }

        [XmlElement("ArticleType1")]
        public string ArticleType1 { get; set; }

        [XmlElement("ArticleType2")]
        public string ArticleType2 { get; set; }
    }

    #endregion

    #region Adjust Inventory

    [Serializable]
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class SellerAdjustEntity : SSBEntityBaseV3
    {
        [XmlElement("Node")]
        public SellerAdjustNote Node { get; set; }
    }

    public class SellerAdjustNote
    {
        [XmlElement("RequestRoot")]
        public SellerAdjustRequestRoot RequestRoot { get; set; }
    }

    public class SellerAdjustRequestRoot
    {
        [XmlElement("MessageHeader")]
        public SellerAdjustMessageHeader MessageHeader { get; set; }

        [XmlElement("Body")]
        public SellerAdjustBody Body { get; set; }
    }

    public class SellerAdjustMessageHeader
    {
        [XmlElement("Language")]
        public string Language { get; set; }

        [XmlElement("Sender")]
        public string Sender { get; set; }

        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }

        [XmlElement("Action")]
        public string Action { get; set; }

        [XmlElement("Namespace")]
        public string Namespace { get; set; }

        [XmlElement("Version")]
        public string Version { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("OriginalGUID")]
        public string OriginalGUID { get; set; }
    }

    public class SellerAdjustBody
    {
        [XmlElement("AdjustInventoryMsg")]
        public SellerAdjustInventoryInfo AdjustInventoryMsg { get; set; }
    }

    public class SellerAdjustInventoryInfo
    {
        [XmlElement("Memo")]
        public string Memo { get; set; }

        [XmlElement("InUser")]
        public string InUser { get; set; }

        [XmlElement("ItemInventory")]
        public List<SellerAdjustItemInventory> ItemInventoryList { get; set; }
    }


    public class SellerAdjustItemInventory
    {
        [XmlElement("ProductSysNo")]
        public int ProductSysNo { get; set; }

        [XmlElement("Quantity")]
        public int Quantity { get; set; }

        [XmlElement("WareHouseNumber")]
        public int WareHouseNumber { get; set; }
    }

    #endregion

    #region Batch Product Update

    [Serializable]
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class BatchSellerAdjustEntity : SSBEntityBaseV3
    {
        [XmlElement("Node")]
        public BatchSellerAdjustNote BatchSellerAdjustNote { get; set; }
    }
    public class BatchSellerAdjustNote
    {
        [XmlElement("RequestRoot")]
        public BatchSellerAdjustRequestRoot BatchSellerAdjustRequestRoot { get; set; }
    }

    public class BatchSellerAdjustRequestRoot
    {
        [XmlElement("MessageHeader")]
        public BatchSellerAdjustMessageHeader BatchSellerAdjustMessageHeader { get; set; }

        [XmlElement("Body")]
        public BatchSellerAdjustBody BatchSellerAdjustBody { get; set; }
    }

    public class BatchSellerAdjustMessageHeader
    {
        [XmlElement("Language")]
        public string Language { get; set; }

        [XmlElement("Sender")]
        public string Sender { get; set; }

        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }

        [XmlElement("Action")]
        public string Action { get; set; }

        [XmlElement("Namespace")]
        public string Namespace { get; set; }

        [XmlElement("Version")]
        public string Version { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("OriginalGUID")]
        public string OriginalGUID { get; set; }
    }

    public class BatchSellerAdjustBody
    {
        [XmlElement("BatchInfos")]
        public BatchSellerAdjustBatchInfos BatchSellerAdjustBatchInfos { get; set; }
    }

    public class BatchSellerAdjustBatchInfos
    {
        [XmlElement("InUser")]
        public string InUser { get; set; }

        [XmlElement("BatchInfo")]
        public List<BatchSellerAdjustBatchInfo> BatchSellerAdjustBatchInfo { get; set; }
    }

    public class BatchSellerAdjustBatchInfo
    {
        [XmlElement("BatchID")]
        public string BatchID { get; set; }

        [XmlElement("ProductSysNo")]
        public string ProductSysNo { get; set; }

        [XmlElement("StockSysNo")]
        public string StockSysNo { get; set; }

        [XmlElement("Status")]
        public string Status { get; set; }

        [XmlElement("Quantity")]
        public string Quantity { get; set; }
    }
    #endregion
}
