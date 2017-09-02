using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品价格信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductPriceInfo : EntityBase
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal BasicPrice { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        [DataMember]
        public decimal CurrentPrice { get; set; }
        /// <summary>
        /// 付款类型
        /// </summary>
        [DataMember]
        public ProductPointType PointType { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [DataMember]
        public decimal Discount { get; set; }

        private int m_MaxPerOrder = 10;
        /// <summary>
        /// 每单限购数量
        /// </summary>
        [DataMember]
        public int MaxPerOrder
        {
            get
            {
                return m_MaxPerOrder;
            }
            set
            {
                m_MaxPerOrder = value;
            }
        }


        private int m_MinCountPerOrder = 1;
        /// <summary>
        /// 最小订购量
        /// </summary>
        [DataMember]
        public int MinCountPerOrder
        {
            get
            {
                return m_MinCountPerOrder;
            }
            set
            {
                m_MinCountPerOrder = value;
            }
        }
        /// <summary>
        /// 采购价
        /// </summary>
        [DataMember]
        public decimal VirtualPrice { get; set; }
        /// <summary>
        /// 返现
        /// </summary>
        [DataMember]
        public int CashRebate { get; set; }
        /// <summary>
        /// 赠送积分
        /// </summary>
        [DataMember]
        public int Point { get; set; }
        /// <summary>
        /// 成本价格
        /// </summary>
        [DataMember]
        public decimal UnitCost { get; set; }
        /// <summary>
        /// 去税成本
        /// </summary>
        [DataMember]
        public decimal UnitCostWithoutTax { get; set; }
    }
}
