using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.IM
{
    /// <summary>
    /// 取消商品价格审核
    /// </summary>
    public class CanceledUpdateProductPriceRequestMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_UpdateProductPriceRequest_Canceled";
            }
        }
        /// <summary>
        /// 用户编号
        /// </summary>
        public int CancelUserSysNo { get; set; }
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
