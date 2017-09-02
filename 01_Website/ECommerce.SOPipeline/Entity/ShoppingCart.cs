using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 每次加入购物车时，该次动作中的具体商品
    /// </summary>
    public class ShoppingItem
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 单位数量，买单个商品时，UnitQuantity=1；买捆绑商品时，则指这个套餐组合中，各商品设置的数量
        /// </summary>
        public int UnitQuantity { get; set; }

        /// <summary>
        /// 选择的赠品编号
        /// </summary>
        private List<ShoppingOrderGift> selectGiftSysNo;
        public List<ShoppingOrderGift> SelectGiftSysNo
        {
            set { selectGiftSysNo = value; }
            get
            {
                if (selectGiftSysNo == null)
                    selectGiftSysNo = new List<ShoppingOrderGift>();
                return selectGiftSysNo;
            }
        }

        /// <summary>
        /// 删除的赠品编号
        /// </summary>
        private List<ShoppingOrderGift> deleteGiftSysNo;
        public List<ShoppingOrderGift> DeleteGiftSysNo
        {
            set { deleteGiftSysNo = value; }
            get
            {
                if (deleteGiftSysNo == null)
                    deleteGiftSysNo = new List<ShoppingOrderGift>();
                return deleteGiftSysNo;
            }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool ProductChecked { get; set; }

    }

    /// <summary>
    /// 每次加入购物车时，该次动作的详细（可能是放一个商品，也可能是加入一组捆绑销售的商品）
    /// 该次动作就是一个ShoppingItemGroup，这里面的Quantity就是这一次操作选择的数量
    /// </summary>
    public class ShoppingItemGroup
    {
        /// <summary>
        /// 客户购物时，每次加入购物车操作的串号。
        /// 同一个商品，在以不同方式加入购物车时，是不同的体现形式
        /// </summary>
        public string ShoppingSerialNumber { get; set; }

        private List<ShoppingItem> shoppingItemList;
        
        /// <summary>
        /// 本次加入购物车时，本组商品的详细列表
        /// </summary>
        public List<ShoppingItem> ShoppingItemList
        {
            set { shoppingItemList = value; }
            get
            {
                if (shoppingItemList == null)
                {
                    shoppingItemList = new List<ShoppingItem>();
                }
                return shoppingItemList;
            }
        }

        /// <summary>
        /// 多个商品一起购买的操作类型（0，不是多个商品一起购买；1=套餐）
        /// </summary>
        public int PackageType { get; set; }

        /// <summary>
        /// 对应的活动编号
        /// </summary>
        public int PackageNo { get; set; }

        /// <summary>
        /// 本次加入购物车时，本组商品的数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool PackageChecked { get; set; }
    }

    /// <summary>
    /// 订单级别的赠品
    /// </summary>
    public class ShoppingOrderGift
    {
        /// <summary>
        /// 赠品活动编号
        /// </summary>
        public int ActivityNo { get; set; }

        /// <summary>
        /// 赠品编号
        /// </summary>
        public int GiftSysNo { get; set; }
    }

    public class ShoppingCart
    {
        private List<ShoppingItemGroup> shoppingItemGroupList;
        public List<ShoppingItemGroup> ShoppingItemGroupList
        {
            set { shoppingItemGroupList = value; }
            get
            {
                if (shoppingItemGroupList == null)
                {
                    shoppingItemGroupList = new List<ShoppingItemGroup>();
                }
                return shoppingItemGroupList;
            }
        }

        /// <summary>
        /// 网站渠道ID
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// 当前用户选择的语言，主要用于Service端给出对应语言的提示
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 购物车ID
        /// </summary>
        public string ShoppingCartID { get; set; }

        /// <summary>
        /// 订单级别选择的赠品编号
        /// </summary>
        private List<ShoppingOrderGift> orderSelectGiftSysNo;
        public List<ShoppingOrderGift> OrderSelectGiftSysNo
        {
            set { orderSelectGiftSysNo = value; }
            get
            {
                if (orderSelectGiftSysNo == null)
                    orderSelectGiftSysNo = new List<ShoppingOrderGift>();
                return orderSelectGiftSysNo;
            }
        }

        /// <summary>
        /// 订单级别删除的赠品编号
        /// </summary>
        private List<ShoppingOrderGift> orderDeleteGiftSysNo;
        public List<ShoppingOrderGift> OrderDeleteGiftSysNo
        {
            set { orderDeleteGiftSysNo = value; }
            get
            {
                if (orderDeleteGiftSysNo == null)
                    orderDeleteGiftSysNo = new List<ShoppingOrderGift>();
                return orderDeleteGiftSysNo;
            }
        }

        /// <summary>
        /// 加价购选择的商品列表
        /// </summary>
        private List<int> plusPriceProductSelectList;
        public List<int> PlusPriceProductSelectList
        {
            set { plusPriceProductSelectList = value; }
            get
            {
                if (plusPriceProductSelectList == null)
                    plusPriceProductSelectList = new List<int>();
                return plusPriceProductSelectList;
            }
        }

        /// <summary>
        /// 购买用户
        /// </summary>
        public int CustomerSysNo { get; set; }
    }

}
