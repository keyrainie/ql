using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class BasicInfoModel
    {
        public int ID { get; set; }
        public string Code { get; set; }

        /// <summary>
        /// 商品所属三级分类系统编号
        /// </summary>
        public int C3SysNo { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        public string ProductMode { get; set; }

        /// <summary>
        /// 商品产地
        /// </summary>
        public string ProducePlace { get; set; }

        /// <summary>
        /// 促销语
        /// </summary>
        public string PromotionTitle { get; set; }

        /// <summary>
        /// 商品默认图片(尺寸：Middle)
        /// </summary>
        public string DefaultImageUrl { get; set; }

        /// <summary>
        /// 商品状态(-1：已作废，0:仅展示,1:上架,2：不展示)
        /// </summary>
        public int ProductStatus { get; set; }

        /// <summary>
        /// 商品类型(Normal=0,
        /// SecondHand=1,
        /// Bad=2,
        /// Other=3)
        /// </summary>
        public int ProductType { get; set; }

        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 综合评分
        /// </summary>
        public decimal ReviewScore { get; set; }

        /// <summary>
        /// 被评论数量
        /// </summary>
        public int CommentQty { get; set; }

        /// <summary>
        /// 被收藏数量
        /// </summary>
        public int FavoriteQty { get; set; }

        /// <summary>
        /// 商品组系统编号
        /// </summary>
        public int ProductGroupSysNo { get; set; }
    }
}