using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product.Review
{
    public class UICustomerReviewInfo
    {
        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string DefaultImage { get; set; }

        public string ProductTitle { get; set; }

        /// <summary>
        /// 评论编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 优点
        /// </summary>
        public string Prons { get; set; }

        /// <summary>
        /// 服务
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 评论人SysNo
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 评论人ID
        /// </summary>
        public string CustomerID { get; set; }
    }
}
