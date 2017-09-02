using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 产品咨询回复查询
    /// </summary>
    public class ProductConsultReplyQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public int? ProductCategory { get; set; }

        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 标题或正文关键字
        /// </summary>
        public string ReplyContent { get; set; }

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
        /// 标题或正文关键字
        /// </summary>
        //public string Title { get; set; }

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
        ///对应数据库 VendorID
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
        /// 商品状态
        /// </summary>
        public int? ProductStatus { get; set; }

        /// <summary>
        /// PM
        /// </summary>
        public int? PMUserSysNo { get; set; }

        /// <summary>
        /// 回复状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 回复人类型，是否是vendor 
        /// </summary>
        public ReplyVendor? Type { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public YNStatus? IsTop { get; set; }

        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
