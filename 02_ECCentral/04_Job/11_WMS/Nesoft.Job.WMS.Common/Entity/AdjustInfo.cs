using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.Common.Entity
{
    /// <summary>
    /// 损益单信息
    /// </summary>
    public class AdjustInfo : BaseEntity
    {
        public AdjustInfo()
        {
            //this.Items = new List<AdjustItemInfo>();
        }
        /// <summary>
        /// 商家损益单ID
        /// </summary>
        [JsonProperty("warehouseReduceID")]
        public string AdjustID
        {
            get
            {
                return this.SysNo.ToString();
            }
        }
        /// <summary>
        /// 请求时间
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 请求时间字符串
        /// </summary>
        [JsonProperty("datetime")]
        public string CommitTime
        {
            get
            {
                return this.CreateTime.ToString("yyyyMMddhhmmss");
            }
        }
        /// <summary>
        /// 损益备注信息
        /// </summary>
        [JsonProperty("remarks")]
        public string Memo { get; set; }
        /// <summary>
        /// 损溢责任方(为空)
        /// </summary>
        [JsonProperty("responsibility")]
        public string Responsibility { get; set; }
        ///// <summary>
        ///// 损益商品信息（集合）
        ///// </summary>
        //public List<AdjustItemInfo> Items { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }
        private int m_AdjustQty;
        /// <summary>
        /// 损益数量，都是正数
        /// </summary>
        [JsonProperty("lostNum")]
        public int AdjustQty
        {
            get
            {
                return Math.Abs(m_AdjustQty);
            }
            set
            {
                m_AdjustQty = value;
            }
        }
        /// <summary>
        /// 损益类型:
        /// </summary>
        [JsonProperty("type")]
        public AdjustItemType AdjustType
        {
            get
            {
                return m_AdjustQty > 0 ? AdjustItemType.Good : AdjustItemType.Damage;
            }
        }
        /// <summary>
        /// 商品计税单位
        /// </summary>
        [JsonProperty("units")]
        public string ItemUnit { get; set; }
    }

    /// <summary>
    /// 损益单商品信息
    /// </summary>
    public class AdjustItemInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ItemCode { get; set; }
        private int m_AdjustQty;
        /// <summary>
        /// 损益数量，都是正数
        /// </summary>
        [JsonProperty("LostNum")]
        public int AdjustQty
        {
            get
            {
                return Math.Abs(m_AdjustQty);
            }
            set
            {
                m_AdjustQty = value;
            }
        }
        /// <summary>
        /// 损益类型:
        /// </summary>
        [JsonProperty("Type")]
        public AdjustItemType AdjustType
        {
            get
            {
                return m_AdjustQty > 0 ? AdjustItemType.Good : AdjustItemType.Damage;
            }
        }
        /// <summary>
        /// 商品计税单位
        /// </summary>
        [JsonProperty("Units")]
        public string ItemUnit { get; set; }
    }
    /// <summary>
    /// 损益类型
    /// </summary>
    public enum AdjustItemType
    {
        /// <summary>
        /// 损
        /// </summary>
        [Description("损")]
        Damage = 1,
        /// <summary>
        /// 益
        /// </summary>
        [Description("益")]
        Good = 2,
    }
}
