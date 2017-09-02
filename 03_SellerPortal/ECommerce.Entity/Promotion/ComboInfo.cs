using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Enums.Promotion;

namespace ECommerce.Entity.Promotion
{
    /// <summary>
    /// 组合销售
    /// </summary>
    public class ComboInfo : EntityBase
    {
        public ComboInfo()
        {
            this.Items = new List<ComboItem>();
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string SaleRuleName { get; set; }

        /// <summary>
        /// 当前状态:无效 -1,有效 0,待审核 1
        /// </summary>
        public ComboStatus Status { get; set; }


        /// <summary>
        /// 商品清单
        /// </summary>
        public List<ComboItem> Items { get; set; }

    }
    /// <summary>
    /// 套餐商品信息
    /// </summary>
    public class ComboItem
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 套餐系统编号
        /// </summary>
        public int ComboSysNo { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 商品的折扣
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 是否为主商品
        /// </summary>
        public bool IsMasterItem { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
