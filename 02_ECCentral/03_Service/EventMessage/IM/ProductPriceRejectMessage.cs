using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.IM
{
    /// <summary>
    /// 价格审核拒绝
    /// </summary>
    public class ProductPriceRejectMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_ProductPrice_Rejected";
            }
        }
        /// <summary>
        /// 拒绝用户编号
        /// </summary>
        public int RejectUserSysNo { get; set; }
        /// <summary>
        /// 申请单编号
        /// </summary>
        public int RequestSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }
    }
}
