using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.IM
{
    /// <summary>
    /// 价格审核通过
    /// </summary>
    public class ProductPriceAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_ProductPrice_Audited";
            }
        }
        /// <summary>
        /// 审核用户编号
        /// </summary>
        public int AuditUserSysNo { get; set; }
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
