using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 商家描述
    /// </summary>
    public class SellerRelatedInfo
    {
        /// <summary>
        /// 商家备注
        /// </summary>
        public string SellerMemo { get; set; }

        /// <summary>
        /// 商家操作信息
        /// </summary>
        public string SellerOperationInfo { get; set; }

        /// <summary>
        /// 是否已收到
        /// </summary>
        public string SellerReceived { get; set; }
    }
}
