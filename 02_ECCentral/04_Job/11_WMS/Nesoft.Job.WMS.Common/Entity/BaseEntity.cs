using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.Common.Entity
{
    public class BaseEntity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [JsonIgnore]
        public int SysNo { get; set; }
        /// <summary>
        /// 商家系统编号
        /// </summary>
        [JsonIgnore]
        public int MerchantSysNo { get; set; }
        /// <summary>
        /// 字符串，商家在账册中的编号(新增)
        /// </summary>
        [JsonProperty("kjtCode")]
        public string KjtCode { get; set; }
        /// <summary>
        /// 仓库ID
        /// </summary>
        [JsonProperty("warehouseId")]
        public string WarehouseID { get; set; }

        /// <summary>
        /// 平台编号
        /// </summary>
        [JsonIgnore]
        public string AppId { get; set; }

        /// <summary>
        /// MD5密钥
        /// </summary>
        [JsonIgnore]
        public string AppSecret { get; set; }
    }
}
