using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.SO
{
    [Serializable]
    public class SOSplitMessage : ECCentral.Service.Utility.EventMessage
    {
        public int SOSysNo
        {
            get;
            set;
        }

        public override string Subject
        {
            get
            {
                return "ECC_SO_Split";
            }
        }
        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 主订单编号，如果作废的是子订单，则此值才有效：表示子订单（SOSysNo）对应的主订单编号
        /// </summary> 
        public int? MasterSOSysNo { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 订单的拆分类型
        /// </summary>
        public ECCentral.Service.EventMessage.SO.SOSplitType SplitType { get; set; }

        /// <summary>
        /// 预订人信息
        /// </summary>
        public SOCustomer Customer
        {
            get;
            set;
        }

        /// <summary>
        /// 消费者信息
        /// </summary>
        [XmlArray(ElementName = "Consumers")]
        [XmlArrayItem(ElementName = "SOConsumer")]
        public List<SOConsumer> Consumers { get; set; }

        /// <summary>
        /// 订单商品
        /// </summary>
        [XmlArray(ElementName = "Items")]
        [XmlArrayItem(ElementName = "SOItem")]
        public List<SOItem> Items { get; set; }
        /// <summary>
        /// 渠道编号
        /// </summary>
        public string WebChannelID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 订单随机数
        /// </summary>
        public string CheckRandomNo { get; set; }

        public SOSplitMessage()
        {
            Consumers = new List<SOConsumer>();
            Items = new List<SOItem>();
        }
        /// <summary>
        /// 订单拆分操作者编号
        /// </summary>
        public int SplitUserSysNo { get; set; }
        /// <summary>
        /// 订单拆分操作人名称
        /// </summary>
        public string SplitUserName { get; set; }
    }

    /// <summary>
    /// 修改订单预订人信息
    /// </summary>
    [Serializable]
    public class SOCustomer
    {
        /// <summary>
        /// 预订人名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 预订的网站会员编号
        /// </summary>
        public int AcctSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 预订的网站会员登陆ID
        /// </summary>
        public string AcctID
        {
            get;
            set;
        }

        /// <summary>
        /// 预订的网站会员的真实姓名
        /// </summary>
        public string AcctName
        {
            get;
            set;
        }

        /// <summary>
        /// 预订人证件类型
        /// </summary>
        public ECCentral.Service.EventMessage.Customer.IDCardType IDCardType
        {
            get;
            set;
        }

        /// <summary>
        /// 预订人证件号
        /// </summary>
        public string IDCardNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人手机
        /// </summary>
        public string MobilePhone
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string Phone
        {
            get;
            set;
        }

        /// <summary>
        /// 收货详细地址
        /// </summary>
        public string Address
        {
            get;
            set;
        }


        /// <summary>
        /// 收货人邮编
        /// </summary>
        public string Zip
        {
            get;
            set;
        }
    }

    [Serializable]
    public class SOItem
    {
        public int ProductSysNo { get; set; }

        public int C3SysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        //public ECCentral.Service.EventMessage.IM.SalesPolicyType SalesPolicyType { get; set; }

        //public ECCentral.Service.EventMessage.IM.BusinessType BusinessType { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// 当前销售单价
        /// </summary>
        public decimal CurrentPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 成本价格
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// 订金
        /// </summary>         
        public decimal SubscriptionPrice { get; set; }

        [XmlArray(ElementName = "ItemDetails")]
        [XmlArrayItem(ElementName = "SOItemDetail")]
        public List<SOItemDetail> ItemDetails
        {
            get;
            set;
        }
        /// <summary>
        /// 预订的房号
        /// </summary>
        [XmlArray(ElementName = "SubscribedRooms")]
        [XmlArrayItem(ElementName = "SOSubscribedRoom")]
        public List<SOSubscribedRoom> SubscribedRooms
        {
            get;
            set;
        }
        public SOItem()
        {
            ItemDetails = new List<SOItemDetail>();
            SubscribedRooms = new List<SOSubscribedRoom>();
        }
    }

    /// <summary>
    /// 服务类型商品的明细
    /// </summary>
    [Serializable]
    public class SOItemDetail
    {
        /// <summary>
        /// 当前销售单价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 订金
        /// </summary>         
        public decimal SubscriptionPrice { get; set; }

        /// <summary>
        /// 订单数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 预订消费日期
        /// </summary>         
        public DateTime? SubscribeDate { get; set; }

        /// <summary>
        /// 成本价格
        /// </summary>
        public decimal CostPrice { get; set; }
    }

    /// <summary>
    /// 预订房号信息
    /// </summary>
    [Serializable]
    public class SOSubscribedRoom
    {
        /// <summary>
        /// 预订消费日期
        /// </summary>
        public DateTime? SubscribeDate { get; set; }

        /// <summary>
        /// 订购的房号
        /// </summary>
        public int RoomSysNo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 订单消费者信息
    /// </summary>
    [Serializable]
    public class SOConsumer
    {
        /// <summary>
        /// 消费者编号
        /// </summary>
        public int ConsumerSysNo { get; set; }

        /// <summary>
        /// 消费者名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 消费者证件类型
        /// </summary>
        public ECCentral.Service.EventMessage.Customer.IDCardType IDCardType
        {
            get;
            set;
        }

        /// <summary>
        /// 消费者证件号
        /// </summary>
        public string IDCardNo { get; set; }

        /// <summary>
        /// 消费的项目
        /// </summary>

        [XmlArray(ElementName = "ConsumeItems")]
        [XmlArrayItem(ElementName = "SOConsumerItem")]
        public List<SOConsumerItem> ConsumeItems
        {
            get;
            set;
        }
        /// <summary>
        /// 消费者消费项目
        /// </summary>
        public SOConsumer()
        {
            ConsumeItems = new List<SOConsumerItem>();
        }
    }

    /// <summary>
    /// 消费者预订消费的项目
    /// </summary>
    [Serializable]
    public class SOConsumerItem
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品预订消费日期
        /// </summary>
        public DateTime? SubscribeDate { get; set; }

        /// <summary>
        /// 商品物理编号，如：房号，门票号
        /// </summary>
        public string ProductPhysicalNo { get; set; }
    }
}
