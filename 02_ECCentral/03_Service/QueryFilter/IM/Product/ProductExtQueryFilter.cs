using System;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductExtQueryFilter
    {
       public PagingInfo PagingInfo { get; set; }

       /// <summary>
       /// 商品ID
       /// </summary>
        public string ProductID { get; set; }

       /// <summary>
       /// 状态
       /// </summary>
        public ProductStatus? ProductStatus { get; set; }

       /// <summary>
       /// 商品类型
       /// </summary>
        public ProductType? ProductType { get; set; }

        /// <summary>
        /// 类别1
        /// </summary>
        public int? Category1 { get; set; }

       /// <summary>
       /// 类别2
       /// </summary>
        public int? Category2 { get; set; }

          /// <summary>
          /// 类别3
          /// </summary>
        public int? Category3 { get; set; }

        /// <summary>
        /// 生产商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 是否可以退货
        /// </summary>
        public IsDefault? IsPermitRefund { get; set; }
    }
  
}
