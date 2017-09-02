using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Order
{
    public class OrderItemGroupModel
    {
        /// <summary>
        /// 本订单所属的商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 本订单所属的商家名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 所购买的商品的集合
        /// </summary>
        public List<OrderProductItemModel> ProductItemList { get; set; }

        /// <summary>
        /// 购买商品组的数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 每单限购最小值
        /// </summary>
        public int MinCountPerSO { get; set; }

        /// <summary>
        /// 每单限购最大值
        /// </summary>
        public int MaxCountPerSO { get; set; }

        /// <summary>
        /// 多个商品一起购买的操作类型（0=单个商品购买；1=套餐）
        /// </summary>
        public int PackageType { get; set; }

        /// <summary>
        /// 对应的活动编号
        /// </summary>
        public int PackageNo { get; set; }


        /// <summary>
        /// 本次加入购物车商品销售价格的小计
        /// </summary>
        public decimal TotalSalePrice { get; set; }

        /// <summary>
        /// 本次加入购物车商品总数
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 套餐价
        /// </summary>
        public decimal PackagePrice { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool PackageChecked { get; set; }
    }
}