using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using System;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductShowQueryVM:ModelBase
    {


        public ProductShowQueryVM()
        {
            ProductStatusList = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
        }

        // <summary>
        /// 首次上架开始时间
        /// </summary>
        public DateTime? FirstOnlineTimeFrom { get; set; }
        /// <summary>
        /// 首次上架结束时间
        /// </summary>
        public DateTime? FirstOnlineTimeTo { get; set; }

        /// <summary>
        /// 更新时间开始时间
        /// </summary>
        public DateTime? EditDateFrom { get; set; }
        /// <summary>
        /// 更新时间结束时间 
        /// </summary>
        public DateTime? EditDateTo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus? Status { get; set; }

        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        public List<KeyValuePair<ProductStatus?, string>> ProductStatusList { get; set; }
    }
}
