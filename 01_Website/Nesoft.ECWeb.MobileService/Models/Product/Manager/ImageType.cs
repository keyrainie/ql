using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public enum ImageType
    {
        /// <summary>
        /// 商品详情赠品，附件，配件等非常小的图片尺寸
        /// </summary>
        Tiny,
        /// <summary>
        /// 订单列表中的商品图片尺寸
        /// </summary>
        Small,
        /// <summary>
        /// 列表页面图片尺寸
        /// </summary>
        Middle,
        /// <summary>
        /// 商品详情Galley的图片尺寸
        /// </summary>
        Big,
        /// <summary>
        /// 大图，供商品详情用
        /// </summary>
        Huge
    }
}