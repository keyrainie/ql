using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductAttachmentQueryFilter
    {

        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 商品SysNo
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 附件ID
        /// </summary>
        public string AttachmentID { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string AttachmentName { get; set; }

        /// <summary>
        /// 创建开始时间
        /// </summary>
        public DateTime? InDateStart { get; set; }

        /// <summary>
        /// 创建结束时间
        /// </summary>
        public DateTime? InDateEnd { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        public string EditUser { get; set; }

    }
}
