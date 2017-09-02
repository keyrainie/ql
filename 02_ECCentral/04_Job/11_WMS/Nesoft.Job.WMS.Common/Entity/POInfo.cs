using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.Common.Entity
{
    public class POInfo : BaseEntity
    {
        public POInfo()
        {
            this.Items = new List<POItemInfo>();
        }
        /// <summary>
        /// 入库单编号
        /// </summary>
        [JsonProperty("billNo")]
        public string BillNo { get; set; }
        /// <summary>
        /// 字符串，海关报关单号(为空)
        /// </summary>
        [JsonProperty("customsID")]
        public string CustomsID { get; set; }
        /// <summary>
        /// 字符串，提运单号(为空)
        /// </summary>
        [JsonProperty("assBillNo")]
        public string AssBillNo { get; set; }
        /// <summary>
        /// 字符串，调拨单号(为空)
        /// </summary>
        [JsonProperty("deliveryNo")]
        public string DeliveryNo { get; set; }
        /// <summary>
        /// 字符串，备注信息(为空)
        /// </summary>
        [JsonProperty("remarks")]
        public string Remarks { get; set; }
        ///// <summary>
        ///// 预计到货时间
        ///// </summary>
        //[JsonIgnore]
        //public DateTime ETATime { get; set; }
        ///// <summary>
        ///// 预计到货时间,传入数据中心用此字段
        ///// </summary>
        //[JsonProperty("ETATime")]
        //public string ETATimeP
        //{
        //    get
        //    {
        //        return this.ETATime.ToString("yyyyMMddhhmmss");
        //    }
        //}
        ///// <summary>
        ///// 预计到货时段，只能为[AM,PM]
        ///// </summary>
        //[JsonProperty("HalfDay")]
        //public string ETAHalfDay { get; set; }
        /// <summary>
        /// 入库商品信息（集合）
        /// </summary>
        [JsonProperty("cargoDetail")]
        public List<POItemInfo> Items { get; set; }
    }

    public class POItemInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }
        /// <summary>
        /// 商品入库数量，进境备案的预增数量，值必须大于0
        /// </summary>
        [JsonProperty("itemNum")]
        public int ItemNum { get; set; }
        /// <summary>
        /// 商品计税单位
        /// </summary>
        [JsonProperty("units")]
        public string Units { get; set; }
    }
}
