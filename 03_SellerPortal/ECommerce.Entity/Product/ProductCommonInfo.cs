using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
    public class ProductCommonInfo : EntityBase
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
        /// 商品名称
        /// </summary>
        public string ProductTitle { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus Status { get; set; }
        /// <summary>
        /// 商品组编号
        /// </summary>
        public int GroupSysNo { get; set; }
        /// <summary>
        /// 商品组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 商品分类名称（前台）
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 可用库存
        /// </summary>
        public int AvailableQty { get; set; }
        /// <summary>
        ///  在线库存
        /// </summary>
        public int OnlineQty { get; set; }
        /// <summary>
        /// 当前价格
        /// </summary>
        public decimal CurrentPrice { get; set; }

        public string BrandName_Ch { get; set; }
        public string BrandName_En { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public TradeType ProductTradeType { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string BrandName
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(BrandName_Ch);
                if (!string.IsNullOrEmpty(BrandName_En))
                {
                    builder.Append("(" + BrandName_En + ")");

                }
                return builder.ToString();
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTimeString
        {
            get
            {
                return (InDate != null && InDate != new DateTime()) ? InDate.ToString("yyyy年MM月dd日 HH:mm:ss") : string.Empty;
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
        /// 成本
        /// </summary>
        public decimal? UnitCost { get; set; }
        /// <summary>
        /// 去税成本
        /// </summary>
        public decimal? UnitCostWithoutTax { get; set; }


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
        /// 商品图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal? TariffRate { get; set; }
    }
}
