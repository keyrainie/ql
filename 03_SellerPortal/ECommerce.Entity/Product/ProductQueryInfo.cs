using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
    public class ProductQueryInfo : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// Gets or sets the group system no.
        /// </summary>
        /// <value>
        /// The group system no.
        /// </value>
        public int GroupSysNo { get; set; }

        /// <summary>
        /// 商品组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTitle { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string ProductMode { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string DefaultImage { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus Status { get; set; }
        /// <summary>
        /// 当前价格
        /// </summary>
        public decimal CurrentPrice { get; set; }
        /// <summary>
        /// 可用库存
        /// </summary>
        public int AvailableQty { get; set; }
        /// <summary>
        /// 在线库存
        /// </summary>
        public int OnlineQty { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public TradeType ProductTradeType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTimeString
        {
            get
            {
                return (InDate != null && InDate != new DateTime()) ? InDate.ToString("yyyy年MM月dd日 HH:mm:ss") : null;
            }
        }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public string EditTimeString
        {
            get
            {
                return (EditDate != null && EditDate != new DateTime()) ? EditDate.Value.ToString("yyyy年MM月dd日 HH:mm:ss") : null;
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public string StatusString
        {
            get
            {
                return EnumHelper.GetDescription(Status);
            }
        }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string ProductTradeTypeString
        {
            get
            {
                return EnumHelper.GetDescription(ProductTradeType);
            }
        }

        /// <summary>
        /// Gets or sets the upc code.
        /// </summary>
        /// <value>
        /// The upc code.
        /// </value>
        public string UPCCode
        {
            get;
            set;
        }
    }
}
