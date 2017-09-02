using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Service.IM.Restful.RequestMsg
{
    public class ProductBatchUpdateRequestMsg
    {
        public ProductInfo ProductInfo { get; set; }

        public List<int> BatchUpdateProductSysNoList { get; set; }
    }


    public class ProductBatchEntryReq
    {
        /// <summary>
        /// 要处理的订单列表
        /// </summary>
        public List<int> ProductSysNoList { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 备案状态
        /// </summary>
        public ProductEntryStatus EntryStatus { get; set; }

        /// <summary>
        /// 备案扩展状态
        /// </summary>
        public ProductEntryStatusEx EntryStatusEx { get; set; }

    }
}
