using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 团购需要保存的信息
    /// </summary>
    public class GroupBuySaveInfo
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }
        /// <summary>
        /// 价格1
        /// </summary>
        public decimal? Price1
        {
            get;
            set;
        }
        /// <summary>
        /// 价格2
        /// </summary>
        public decimal? Price2
        {
            get;
            set;
        }
        /// <summary>
        /// 价格3
        /// </summary>
        public decimal? Price3
        {
            get;
            set;
        }
        /// <summary>
        /// 毛利率1
        /// </summary>
        public decimal? MarginRate1
        {
            get;
            set;
        }
        /// <summary>
        /// 毛利率2
        /// </summary>
        public decimal? MarginRate2
        {
            get;
            set;
        }
        /// <summary>
        /// 毛利率3
        /// </summary>
        public decimal? MarginRate3
        {
            get;
            set;
        }
        /// <summary>
        /// 毛利1
        /// </summary>
        public decimal? MarginDollar1
        {
            get;
            set;
        }
        /// <summary>
        /// 毛利2
        /// </summary>
        public decimal? MarginDollar2
        {
            get;
            set;
        }
        /// <summary>
        /// 毛利3
        /// </summary>
        public decimal? MarginDollar3
        {
            get;
            set;
        }

        /// <summary>
        /// 折扣1
        /// </summary>
        public decimal? Discount1
        {
            get;
            set;
        }
        /// <summary>
        /// 折扣2
        /// </summary>
        public decimal? Discount2
        {
            get;
            set;
        }
        /// <summary>
        /// 折扣3
        /// </summary>
        public decimal? Discount3
        {
            get;
            set;
        }

        /// <summary>
        /// 节省金额1
        /// </summary>
        public decimal? SpareMoney1
        {
            get;
            set;
        }
        /// <summary>
        /// 节省金额2
        /// </summary>
        public decimal? SpareMoney2
        {
            get;
            set;
        }
        /// <summary>
        /// 节省金额3
        /// </summary>
        public decimal? SpareMoney3
        {
            get;
            set;
        }
    }
}
