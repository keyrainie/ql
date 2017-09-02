using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 每一次加入购物车的商品就是一个ItemGroup；
    /// 如果有多次加入购物车的商品完全相同，那么这几次购物行为的商品，都会合并在一个ItemGroup中；
    /// 对于每次加入购物车这个操作行为，我们以ItemGroup作为加入购物车商品的【基本单位】，购买数量则反映在OrderItemGroup中的Quantity上，
    /// 正常情况下，每个商品都会形成一个ItemGroup，其中ProductItemList中有1个商品，并且OrderProductItem中的UnitQuantity为1。
    /// 举例说明：
    /// （1）用户先购买商品A 2个，那么生成一个OrderItemGroup, 
    ///     属性：SerialNumber=123456***, Quantity=2，ProductItemList中有商品A，UnitQantity=1
    /// （2）用户再购买商品A(1)+B(2)套餐3个，那么生成一个OrderItemGroup, 
    ///     属性：SerialNumber=23422***, Quantity=3，ProductItemList中有商品A，UnitQantity=1，商品B,UnitQuantity=2
    ///  合计：这个OrderInfo中有两条OrderItemGroup记录；商品A一共有 2+ 1*3=5个，商品B一共有 2*3 = 6个    
    ///  其它基于该购买行为形成的：金额折扣，赠品，赠送的积分/账户余额/减免运费，都是以OrderItemGroup为基本单位进行计算再分摊，也就是说，要汇总，需要*Quantity
    /// </summary>
    public class OrderItemGroup : ExtensibleObject
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
        public List<OrderProductItem> ProductItemList { get; set; }

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
        public decimal TotalSalePrice { get { return ProductItemList.Sum(x => x.TotalSalePrice) * Quantity; } }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool PackageChecked { get; set; }

        public override ExtensibleObject CloneObject()
        {
            OrderItemGroup op = new OrderItemGroup()
            { 
                MerchantSysNo = this.MerchantSysNo,
                MerchantName = this.MerchantName,
                Quantity = this.Quantity,
                MinCountPerSO = this.MinCountPerSO,
                MaxCountPerSO = this.MaxCountPerSO,
                ProductItemList = this.ProductItemList == null ? null : this.ProductItemList.ConvertAll<OrderProductItem>(x => x == null ? null : (OrderProductItem)x.Clone()),
                PackageNo = this.PackageNo,
                PackageType = this.PackageType,
                PackageChecked = this.PackageChecked, 
            };
            return op;
        }
    }
}
