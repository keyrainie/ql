using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    public class ProductReviewCreatedMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_ProductReview_Created";
            }
        }

        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
