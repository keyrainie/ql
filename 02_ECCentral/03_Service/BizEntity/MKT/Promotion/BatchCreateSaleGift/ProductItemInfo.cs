using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.MKT 
{
    public class ProductItemInfo
    {

        /// <summary>
        /// 商品编号。
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品名称。
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品名称。
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品状态。
        /// </summary>
        public ProductStatus ProductStatus { get; set; }

        /// <summary>
        /// 当前价格。
        /// </summary>
        public decimal CurrentPrice { get; set; }
        
        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }


        /// <summary>
        /// 赠送数量
        /// </summary>
        public int? HandselQty { get; set; }

        /// <summary>
        /// 赠送数量
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 赠送数量
        /// </summary>
        public int? BrandSysNo { get; set; }
    }
}
