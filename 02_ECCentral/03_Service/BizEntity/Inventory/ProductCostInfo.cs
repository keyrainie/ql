using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品库存成本序列
    /// </summary>
    public class ProductCostInfo
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 源单据类型
        /// </summary>
        public int BillType { get; set; }

        /// <summary>
        /// 源单据类型
        /// </summary>
        public string CostBillName
        {
            get
            {
                switch (BillType)
                {
                    case 1:
                        return "正常品退回";
                    case 2:
                        return "异常品退回";
                    case 3:
                        return "订单物流拒收";
                    case 4:
                        return "借货单还货";
                    case 40:
                        return "采购单";
                    case 41:
                        return "损益单";
                    case 42:
                        return "移仓单";
                    case 43:
                        return "转换单";
                    case 44:
                        return "成本变价单";
                    default:
                        return "其他单据";
                }
            }
        }

        /// <summary>
        /// 源单据编号
        /// </summary>
        public int BillSysNo { get; set; }

        /// <summary>
        /// 入库成本
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int LeftQuantity { get; set; }

        /// <summary>
        /// 锁定库存
        /// </summary>
        public int LockQuantity { get; set; }

        /// <summary>
        /// 入库税率
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public int? EditUser { get; set; }

        /// <summary>
        /// 编辑日期
        /// </summary>
        public DateTime? EditDate { get; set; }
    }

}
