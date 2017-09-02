using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.SO
{
    [Serializable]
    public class SOApplyToAbandonMessage : ECCentral.Service.Utility.EventMessage
    {
        /// <summary>
        /// 要作废的订单编号（可能是子订单编号）
        /// </summary> 
        public int SOSysNo { get; set; }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 主订单编号，如果作废的是子订单，则此值才有效：表示子订单（SOSysNo）对应的主订单编号
        /// </summary> 
        public int? MasterSOSysNo { get; set; }

        /// <summary>
        /// 订单的拆分类型
        /// </summary>
        public SOSplitType SplitType { get; set; }

        [XmlArray(ElementName = "Items")]
        [XmlArrayItem(ElementName = "SOItem")]
        public List<SOItem> Items { get; set; }
        public SOApplyToAbandonMessage()
        {
            Items = new List<SOItem>();
        }
    }
}
