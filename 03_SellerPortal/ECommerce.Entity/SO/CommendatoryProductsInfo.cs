using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.SO
{
    /// <summary>
    /// 该实体主要用来查询推荐商品信息，最终使用该数据发送邮件
    /// </summary>
    public class CommendatoryProductsInfo
    {
        public decimal Price { get; set; }

        public string DefaultImage { get; set; }

        public string ImageVersion { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public int ProductSysNo { get; set; }
    }
}
