using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 库存调整批次表
    /// </summary>
    [Serializable]
    [DataContract]
    public class InventoryBatchDetailsInfo
    {
        /// <summary>
        /// 单据类型 TR 转换单  LD 借货单 AD损益单
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }


        /// <summary>
        /// 泰隆优选批号
        /// </summary>
        [DataMember]
        public string BatchNumber { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        [DataMember]
        public DateTime? InStockDate { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        [DataMember]
        public DateTime? MfgDate { get; set; }


        /// <summary>
        /// 过期日期
        /// </summary>
        [DataMember]
        public DateTime? ExpDate { get; set; }


        /// <summary>
        /// 最大可发货天数
        /// </summary>
        [DataMember]
        public int MaxDeliveryDays { get; set; }

        /// <summary>
        /// 厂商批号
        /// </summary>
        [DataMember]
        public String LotNo { get; set; }

        /// <summary>
        ///仓库号
        /// </summary>
        [DataMember]
        public int StockSysNo { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        [DataMember]
        public int ActualQty { get; set; }

        /// <summary>
        /// 被占用库存
        /// </summary>
        [DataMember]
        public int AllocatedQty { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public string Status { get; set; }


        /// <summary>
        /// 调整数量
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }


        /// <summary>
        /// 归还数量
        /// </summary>
        [DataMember]
        public int ReturnQty { get; set; }

    }


    #region [相关序列化Class]

    [XmlRoot("Message", Namespace = null, IsNullable = false)]
    public class BatchXMLMessage
    {
        [XmlElement("Header")]
        public InventoryHeader Header { get; set; }
        [XmlElement("Body")]
        public InventoryBody Body { get; set; }
    }

    public class InventoryHeader
    {
        [XmlElement("NameSpace")]
        public string NameSpace { get; set; }
        [XmlElement("Action")]
        public string Action { get; set; }
        [XmlElement("Version")]
        public string Version { get; set; }
        [XmlElement("Type")]
        public string Type { get; set; }
        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }
        [XmlElement("Tag")]
        public string Tag { get; set; }
        [XmlElement("Language")]
        public string Language { get; set; }
        [XmlElement("From")]
        public string From { get; set; }
        [XmlElement("GlobalBusinessType")]
        public string GlobalBusinessType { get; set; }
        [XmlElement("StoreCompanyCode")]
        public string StoreCompanyCode { get; set; }
        [XmlElement("TransactionCode")]
        public string TransactionCode { get; set; }
    }

    public class InventoryBody
    {
        [XmlElement("Number")]
        public string Number { get; set; }
        [XmlElement("InUser")]
        public string InUser { get; set; }
        [XmlElement("ItemBatchInfo")]
        public List<ItemBatchInfo> ItemBatchInfo { get; set; }
    }

    public class ItemBatchInfo
    {
        [XmlElement("BatchNumber")]
        public string BatchNumber { get; set; }
        [XmlElement("Status")]
        public string Status { get; set; }
        [XmlElement("ProductNumber")]
        public string ProductNumber { get; set; }
        [XmlElement("ExpDate")]
        public string ExpDate { get; set; }
        [XmlElement("MfgDate")]
        public string MfgDate { get; set; }
        [XmlElement("LotNo")]
        public string LotNo { get; set; }
        [XmlElement("Stocks")]
        public Stocks Stocks { get; set; }
    }

    public class Stocks
    {
        [XmlElement("Stock")]
        public List<Stock> Stock { get; set; }
    }

    public class Stock
    {
        [XmlElement("Quantity")]
        public string Quantity { get; set; }
        [XmlElement("AllocatedQty")]
        public string AllocatedQty { get; set; }
        [XmlElement("WarehouseNumber")]
        public string WarehouseNumber { get; set; }
    }
    #endregion
}
