using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 产品咨询列表查询
    /// </summary>
    [Serializable]
    public class ProductConsultQueryFilter : QueryFilter
    {
        /// <summary>
        /// 商品类别
        /// </summary>
        public int? ProductCategory { get; set; }

        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int? SellerSysNo { get; set; }

        /// <summary>
        /// 标题或正文关键字
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? InDateTo { get; set; }

        /// <summary>
        /// 更新时间开始
        /// </summary>
        public DateTime? EditDateFrom { get; set; }

        /// <summary>
        /// 更新时间结束
        /// </summary>
        public DateTime? EditDateTo { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// ProductID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 处理人员
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 按组查询 
        /// </summary>
        public bool IsByGroup { get; set; }

        /// <summary>
        /// 商品组编号
        /// </summary>
        public int? ProductGroupNo { get; set; }

        /// <summary>
        /// 标记类型
        /// </summary>
        //public int? MarkCategory { get; set; }

        /// <summary>
        ///商家ID--对应数据库 VendorID
        /// </summary>
        public int? VendorType { get; set; }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 顾客类型
        /// </summary>
        public int? CustomerCategory { get; set; }

        /// <summary>
        /// 标记类型  D=商品咨询  S=库存配送
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 咨询状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public int? ProductStatus { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// PM
        /// </summary>
        public int? PMUserSysNo { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
