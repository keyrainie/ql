//************************************************************************
// 用户名				泰隆优选
// 系统名				价格审核表
// 子系统名		        价格审核表
// 作成者				Tom.H.Li
// 改版日				2012.4.26
// 改版内容				新建
//************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.Restful.ResponseMsg
{
    public class ProductPriceRequestMsg 
    {
        /// <summary>
        /// 商品价格申请单据
        /// </summary>
        public ProductPriceRequestInfo PriceRequestMsg { get; set; }

        /// <summary>
        /// 商品活动描述
        /// </summary>
        public List<ProductPromotionMsg> PromotionMsg { get; set; }

        /// <summary>
        /// 最低毛利金额
        /// </summary>
        public decimal MinMarginAmount { get; set; }
    }

    public class ProductPromotionMsg
    {
        /// <summary>
        /// 促销类型
        /// </summary>
        public PromotionType PromotionType { get; set; }

        /// <summary>
        /// 促销编号
        /// </summary>
        public int ReferenceSysNo { get; set; }

        /// <summary>
        /// 抵扣金额
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal Margin { get; set; }
    }
}
