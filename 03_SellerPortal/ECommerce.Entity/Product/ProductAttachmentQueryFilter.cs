using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess.SearchEngine;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProductAttachmentQueryFilter : QueryFilter
    {
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
        public string InDateStart { get; set; }

        /// <summary>
        /// 创建结束时间
        /// </summary>
        public string InDateEnd { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        public string EditUser { get; set; }

        public int? SellerSysNo { get; set; }

    }

}
